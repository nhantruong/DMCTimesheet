using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class HomeController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();
        public ActionResult Index()
        {

            return View();
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