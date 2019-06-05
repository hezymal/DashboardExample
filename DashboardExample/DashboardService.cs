using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DashboardExample.Entities;
using DashboardExample.Metadata;
using Microsoft.EntityFrameworkCore;

namespace DashboardExample
{
    public class DashboardService
    {
        private readonly DashboardContext _dbContext;

        public DashboardService(DashboardContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<CountingResult> GetEntityFillInformation<TEntity>(string tableName)
        {
            var entityType = typeof(TEntity);
            var entityAttribute = GetCountableAttribute(entityType);
            if (entityAttribute == null)
            {
                throw new ArgumentException($"In type {nameof(TEntity)} not set {nameof(CountableAttribute)}");
            }

            var entityMetadata = GetEntityMetadata(entityType, entityAttribute);
            if (entityMetadata.AccountedProperties == 0)
            {
                return new List<CountingResult>().AsQueryable();
            }

            var query = BuildQueryForCountingEntity(tableName, entityMetadata);
            return _dbContext.CountingResults.FromSql(query);
        }

        private EntityMetadata GetEntityMetadata(Type entityType, CountableAttribute entityAttribute)
        {
            var entityMetadata = new EntityMetadata()
            {
                CountTimes = entityAttribute.CountTimes,
            };

            foreach (var propertyMetadata in GetCountablePropertiesMetadata(entityType.GetProperties()))
            {
                entityMetadata.AccountedProperties++;

                if (!propertyMetadata.MustBeFilled)
                {
                    entityMetadata.RequiredProperties++;
                }
                else
                {
                    entityMetadata.NullablePropertiesNames.Add(propertyMetadata.PropertyName);
                }
            }

            return entityMetadata;
        }

        private IEnumerable<PropertyMetadata> GetCountablePropertiesMetadata(IEnumerable<PropertyInfo> properties) =>
            properties
                .Where(p => p.IsDefined(typeof(CountableAttribute), false))
                .Select(p => new PropertyMetadata
                {
                    PropertyName = p.Name,
                    MustBeFilled = GetCountableAttribute(p).MustBeFilled,
                });

        private CountableAttribute GetCountableAttribute(MemberInfo memberInfo) =>
            memberInfo
                .GetCustomAttributes(typeof(CountableAttribute), false)
                .FirstOrDefault() as CountableAttribute;

        private string BuildQueryForCountingEntity(string tableName, EntityMetadata entityMetadata)
        {
            var propsQuery = BuildQueryForCountingProperties(entityMetadata.NullablePropertiesNames);

            return entityMetadata.CountTimes == 0
                ?   $"  SELECT      ({entityMetadata.AccountedProperties} * C.Lines) AS Accounted,          " + // маппинг в CountingResult
                    $"              ({entityMetadata.RequiredProperties} * C.Lines + C.Filled) AS Filled    " +
                    $"  FROM        (                                                                       " +
                    $"      SELECT  SUM({propsQuery}) AS Filled,                                            " + // C: суммирование всех строк
                    $"              COUNT(*) AS Lines                                                       " + // C: получаем количество строк
                    $"      FROM    [dbo].[{tableName}]                                                     " +
                    $"  ) AS C                                                                              "

                :   $"  SELECT      {entityMetadata.AccountedProperties * entityMetadata.CountTimes} AS Accounted,          " + // маппинг в CountingResult
                    $"              ({entityMetadata.RequiredProperties * entityMetadata.CountTimes} + C.Filled) AS Filled  " +
                    $"  FROM        (                                                                                       " +
                    $"      SELECT  SUM(B.Filled) AS Filled                                                                 " + // C: суммирование всех строк
                    $"      FROM    (                                                                                       " +
                    $"          SELECT  TOP {entityMetadata.CountTimes} A.Filled                                            " + // B: получение самых заполненных строк
                    $"          FROM    (                                                                                   " +
                    $"              SELECT  {propsQuery} AS Filled                                                          " + // A: суммирование каждой строки отдельно
                    $"              FROM    [dbo].[{tableName}]                                                             " +
                    $"          )       AS A                                                                                " +
                    $"          ORDER   BY A.Filled DESC                                                                    " +
                    $"      ) AS B                                                                                          " +
                    $"  ) AS C                                                                                              ";
        }

        private string BuildQueryForCountingProperties(IEnumerable<string> propertiesNames) =>
            string.Join(" + ", propertiesNames.Select(BuildQueryForCountingProperty));

        private string BuildQueryForCountingProperty(string propertyName) => 
            $"(CASE WHEN LEN([{propertyName}]) > 0 THEN 1 ELSE 0 END)";

        private class EntityMetadata
        {
            public int CountTimes { get; set; }

            public int AccountedProperties { get; set; }

            public int RequiredProperties { get; set; }

            public List<string> NullablePropertiesNames { get; set; } = new List<string>();
        }

        private class PropertyMetadata
        {
            public string PropertyName { get; set; }

            public bool MustBeFilled { get; set; }
        }
    }
}
