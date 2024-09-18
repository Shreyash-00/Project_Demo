using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Daley_Demo.Controllers
{
    public class HomeController : Controller
    {
       
        // Default action for the homepage
        public ActionResult Index()
        {
            return View();
        }

        // Returns the Introduction view
        public ActionResult Introduction()
        {
            return PartialView();
        }

        // Returns DataTable1 view
        public ActionResult DataTable1()
        {
            return PartialView();
        }

        // Returns DataTable2 view
        public ActionResult DataTable2()
        {
            return PartialView();
        }
    }
}