using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMCTimesheet.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        //public ReportViewer ReportManagement() {
        //    ReportViewer kq = new ReportViewer();
        //    return kq;
        //}

    }
}