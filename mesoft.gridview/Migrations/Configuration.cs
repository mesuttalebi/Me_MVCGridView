namespace mesoft.gridview.Migrations
{
    using mesoft.gridview.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<mesoft.gridview.Models.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "mesoft.gridview.Models.MyDbContext";
        }

        protected override void Seed(mesoft.gridview.Models.MyDbContext context)
        {            
            var customers = new List<Customer>();            

            for (int i = 0; i < 39; i++)
            {
                var cust = new Customer() { CompanyName = "Company Name " + i , ContactTitle = "Contact Title " + i, Country = "Turkey", City = "Istanbul", Phone = "6564811215", Address = "Address For Company " + i };
                customers.Add(cust);
            }

            context.Customers.AddOrUpdate(customers.ToArray());
        }
    }
}
