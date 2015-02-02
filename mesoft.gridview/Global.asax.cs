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

            for (int i = 0; i < 39; i++)
            {
                var cust = new Customer() { CompanyName = "Company Name " + i, ContactTitle = "Contact Title " + i, Country = "Turkey", City = "Istanbul", Phone = "6564811215", Address = "Address For Company " + i };
                customers.Add(cust);
            }

            context.Customers.AddRange(customers.ToArray());
        }
    }
}
