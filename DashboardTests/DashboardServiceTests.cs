using System.Collections.Generic;
using System.Linq;
using Dashboard;
using Dashboard.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DashboardTests
{
    [TestClass]
    public class DashboardServiceTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var options = new DbContextOptionsBuilder<DashboardContext>()
                .UseInMemoryDatabase(databaseName: "Dashboard")
                .Options;

            using (var context = new DashboardContext(options))
            {
                context.Homes.AddRange(new List<Home>
                {
                    new Home { Id = 1 },
                    new Home { Id = 2 },
                    new Home { Id = 3 },
                });

                var service = new DashboardService(context);
                var result = service.GetEntityFillInformation<Home>(nameof(DashboardContext.Homes)).First();

                Assert.AreEqual(result.Accounted, 4);
                Assert.AreEqual(result.Filled, 3);
            }
        }

        [TestMethod]
        public void WhereNotFilledTwoHomes()
        {
            var options = new DbContextOptionsBuilder<DashboardContext>()
                .UseInMemoryDatabase(databaseName: "Dashboard")
                .Options;

            using (var context = new DashboardContext(options))
            {
                context.Homes.AddRange(new List<Home>
                {
                    new Home { Id = 1 },
                    new Home { Id = 2 },
                    new Home { Id = 3 },
                    new Home { Id = 4 },
                });

                var service = new DashboardService(context);
                var result =
                    service.GetEntityFillInformation<Home>(nameof(DashboardContext.Homes))
                        .Union(service.GetEntityFillInformation<Human>(nameof(DashboardContext.Humans)))
                        .Union(service.GetEntityFillInformation<Work>(nameof(DashboardContext.Works)))
                        .ToList()
                        .Aggregate(new CountingResult(), (accumulator, x) =>
                        {
                            accumulator.Accounted += x.Accounted;
                            accumulator.Filled += x.Filled;
                            return accumulator;
                        });

                Assert.AreEqual(result.Accounted, 10);
                Assert.AreEqual(result.Filled, 10);
            }
        }
    }
}
