using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mesoft.gridview.Models
{
    public class CustomersViewModel
    {
        public IQueryable<Customer> Customers { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public string JsonPagingInfo { get; set; }
    }
}