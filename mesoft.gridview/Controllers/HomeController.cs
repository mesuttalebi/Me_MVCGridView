using mesoft.gridview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mesoft.gridview.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new CustomersViewModel()
            {
                Customers = null,
                PagingInfo = new PagingInfo()
                {
                    CurrentPage=1,
                    ItemsPerPage= 10,
                    PageOptions = new List<int>() { 10,25, 50, 100},
                    ShowPageOptions= true,
                    TotalItems=1
                }
            };
            return View(model);
        }

        public ActionResult GetCustomers(PagingInfo PagingData)
        {
            var db = new MyDbContext();
            var model = GridViewModelProvider.GetCustomersViewModel(db, PagingData);            
            return PartialView("_CustomersPartial", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}