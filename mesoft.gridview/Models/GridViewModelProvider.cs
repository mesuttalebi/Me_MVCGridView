using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;


namespace MT.GridView.Models
{
    public class GridViewModelProvider
    {        
        internal static GridIndexViewModel<Customer> GetCustomersViewModel(MyDbContext db, PagingInfo PagingData)
        {             
            var model = new GridIndexViewModel<Customer>()
            {
                Data = GetData(db.Customers.AsQueryable(), PagingData),
                PagingInfo = PagingData               
            };            
            
            return model;
        }

        private static IEnumerable<Customer> GetData(IQueryable<Customer> customers, PagingInfo pagingData)
        {
            //search
            if (!string.IsNullOrEmpty(pagingData.SearchTerm))
            {
                customers = customers.Where(x => (x.CompanyName.Contains(pagingData.SearchTerm) 
                                               || x.ContactTitle.Contains(pagingData.SearchTerm)));
            }

            //filter
            if (pagingData.Filters != null)
            {                
                foreach (var filterObj in pagingData.Filters)
                {
                    switch (filterObj.Column)
                    {
                        case "Country":
                            if (filterObj.Value.ToLower() != "all")
                                customers = customers.Where(Extensions.GetWhereClause(filterObj, typeof(string)));
                            break;

                        //Add Other Filter Columns Here
                    }
                }                                
            }

            pagingData.TotalItems = customers.Count();

            //sort
            customers = customers.OrderBy(x => x.Id);

            customers = customers.Sort(pagingData.Sort);
            
            customers = customers
                .Skip((pagingData.CurrentPage - 1) * pagingData.ItemsPerPage).Take(pagingData.ItemsPerPage);

            return customers.ToList();
        }        
    }
}