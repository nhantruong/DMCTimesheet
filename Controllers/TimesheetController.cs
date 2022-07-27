using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class TimesheetController : Controller
    {

        private readonly dmcDbcontext db = new dmcDbcontext();
        string conn = "server = 103.27.60.66; user id=dmcAdmin;password=DmcNewVision@2022#; persistsecurityinfo = True; database =cbimtech_dmc";

        #region AutoCode

        // GET: Timesheet
        public ActionResult Index()
        {
            return View();
        }

        // GET: Timesheet/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Timesheet/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Timesheet/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Timesheet/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Timesheet/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Timesheet/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Timesheet/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Project Timesheet

        #endregion

        #region User Timesheet

        #endregion

        #region WorkType
        public ActionResult Worktype()
        {
            db.Database.Connection.ConnectionString = conn;
            List<C07_WorkType> worktypeList = db.C07_WorkType.ToList();
            return View(worktypeList);
        }

        #endregion
    }
}
