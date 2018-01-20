using MT.GridView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MT.GridView.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCustomers(PagingInfo PagingData)
        {
            var db = new MyDbContext();
            var model = GridViewModelProvider.GetCustomersViewModel(db, PagingData);            
            return PartialView("_CustomersPartial", model);
        }

        public PartialViewResult GetGridViewFilter()
        {
            using (var db = new MyDbContext())
            {
                var model = db.Customers.Select(x => x.Country).Distinct().ToList();
                return PartialView("_GridViewFilterPartial",model);
            }
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