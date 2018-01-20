using MT.GridView.Migrations;
using MT.GridView.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace MT.GridView
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
            var path = AppDomain.CurrentDomain.BaseDirectory+ "MOCK_DATA.json";
            var mockdata = File.ReadAllText(path);

            customers = JsonConvert.DeserializeObject<List<Customer>>(mockdata, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy"});
                        
            context.Customers.AddRange(customers.ToArray());
        }
    }
}
