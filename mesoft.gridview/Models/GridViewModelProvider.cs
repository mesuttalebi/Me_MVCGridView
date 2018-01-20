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

        private static IQueryable<Customer> GetData(IQueryable<Customer> Customers, PagingInfo PagingData, out int TotalItems)
        {
            var customers = Customers;

            //search
            if (!string.IsNullOrEmpty(PagingData.SearchTerm))
            {
                customers = customers.Where(x => (x.CompanyName.Contains(PagingData.SearchTerm) || x.ContactTitle.Contains(PagingData.SearchTerm)));
            }

            //filter
            if (PagingData.Filters != null)
            {                
                foreach (var filterObj in PagingData.Filters)
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


            TotalItems = customers.Count();

            //sort
            customers = customers.OrderBy(x => x.Id);
            if (PagingData.Sort != null)
            {
                switch (PagingData.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        if (PagingData.Sort.SortColumn == "CompanyName")
                        {
                            customers = customers.OrderBy(x => x.CompanyName);
                        }
                        else if (PagingData.Sort.SortColumn == "ContactTitle")
                        {
                            customers = customers.OrderBy(x => x.ContactTitle);
                        }
                        break;
                    case SortDirection.Descending:
                        if (PagingData.Sort.SortColumn == "CompanyName")
                        {
                            customers = customers.OrderByDescending(x => x.CompanyName);
                        }
                        else if (PagingData.Sort.SortColumn == "ContactTitle")
                        {
                            customers = customers.OrderByDescending(x => x.ContactTitle);
                        }
                        break;
                    case SortDirection.NotSet:
                    default:
                        break;
                }
            }
            customers = customers
                .Skip((PagingData.CurrentPage - 1) * PagingData.ItemsPerPage).Take(PagingData.ItemsPerPage);

            return customers;
        }        
    }
}