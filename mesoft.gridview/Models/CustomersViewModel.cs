using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MT.GridView.Models
{
    public class CustomersViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }        

        public string JsonPagingInfo { get; set; }
    }
}