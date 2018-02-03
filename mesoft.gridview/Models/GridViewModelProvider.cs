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
        internal static CustomersViewModel GetCustomersViewModel(MyDbContext db, PagingInfo PagingData)
        { 
            int totalItems = 0;
            var model = new CustomersViewModel()
            {

                Customers = GetData(db.Customers.AsQueryable(), PagingData, out totalItems),
                JsonPagingInfo = Json.Encode(new PagingInfo()
                {
                    CurrentPage = PagingData.CurrentPage,
                    ItemsPerPage = PagingData.ItemsPerPage,
                    PageOptions = new List<int>() { 10, 25, 50, 100 },
                    ShowPageOptions = true,
                    SearchTerm = PagingData.SearchTerm,
                    Sort = PagingData.Sort,
                    Filters = PagingData.Filters,
                    TotalItems = totalItems
                })               
            };            
            
            return model;
        }

        private static IEnumerable<Customer> GetData(IQueryable<Customer> customers, PagingInfo pagingData, out int totalItems)
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

            totalItems = customers.Count();

            //sort
            customers = customers.OrderBy(x => x.Id);

            customers = customers.Sort(pagingData.Sort);
            
            customers = customers
                .Skip((pagingData.CurrentPage - 1) * pagingData.ItemsPerPage).Take(pagingData.ItemsPerPage);

            return customers.ToList();
        }        
    }
}