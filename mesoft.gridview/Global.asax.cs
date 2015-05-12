using mesoft.gridview.Migrations;
using mesoft.gridview.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace mesoft.gridview
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer(new InitializeDB());
        }        
    }

    public class InitializeDB : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            var customers = new List<Customer>();
            string[] cities = { "Istanbul", "Trabzon", "Ankara", "Izmir", "Samsun", "Erzurum" };
            DateTime[] dates = {new DateTime(1982, 5, 2), new DateTime(1983, 3, 5), new DateTime(1988,2,9), new DateTime(1999,12,1),new DateTime(2005,5,15),new DateTime(2010,01,01)};
            var rnd = new Random(0);            
            for (int i = 0; i < 39; i++)
            {
               
                var cust = new Customer()
                {
                    CompanyName = "Company Name " + i,
                    ContactTitle = "Contact Title " + i,
                    Country = "Turkey",
                    City = cities[rnd.Next(0, cities.Length - 1)],
                    Phone = "6564811215",
                    Address = "Address For Company " + i,
                    Founded = dates[rnd.Next(0, dates.Length-1)]
                };
                customers.Add(cust);
            }            

            context.Customers.AddRange(customers.ToArray());
        }
    }
}
