using Dashboard.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            using (var dashboardContext = new DashboardContext())
            {
                var dashboardService = new DashboardService(dashboardContext);
                var homes = dashboardService.GetEntityFillInformation<Home>(nameof(dashboardContext.Homes)).First();
                var humans = dashboardService.GetEntityFillInformation<Human>(nameof(dashboardContext.Humans)).First();
                var works = dashboardService.GetEntityFillInformation<Work>(nameof(dashboardContext.Works)).First();

                Console.WriteLine("Home = {0}", homes);
                Console.WriteLine("Human = {0}", humans);
                Console.WriteLine("Work = {0}", works);
                Console.ReadKey();
            }
        }
    }
}
