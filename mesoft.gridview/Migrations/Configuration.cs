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
            AutomaticMigrationsEnabled = true;
            ContextKey = "mesoft.gridview.Models.MyDbContext";
        }

        protected override void Seed(mesoft.gridview.Models.MyDbContext context)
        {            
            var customers = new List<Customer>();
            string[] cities = {"Istanbul", "Trabzon", "Ankara", "Izmir", "Samsun", "Erzurum"};
            DateTime[] dates = { new DateTime(1982, 5, 2), new DateTime(1983, 3, 5), new DateTime(1988, 2, 9), new DateTime(1999, 12, 1), new DateTime(2005, 5, 15), new DateTime(2010, 01, 01) };
            
            for (int i = 0; i < 39; i++)
            {
                var rnd = new Random();
                var cust = new Customer()
                {
                    CompanyName = "Company Name " + i,
                    ContactTitle = "Contact Title " + i,
                    Country = "Turkey",
                    City = cities[rnd.Next(0, cities.Length-1)],
                    Phone = "6564811215",
                    Address = "Address For Company " + i,
                    Founded = dates[rnd.Next(0, dates.Length - 1)]
                };
                customers.Add(cust);
            }

            context.Customers.AddOrUpdate(customers.ToArray());
        }
    }
}
