using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class MemberController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();
        // GET: Member
        public ActionResult Index()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.ProjectType = db.C13_ProjectType.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.Status = db.C16_Status.ToList();
            ViewBag.Location = db.C11_Location.ToList();
            ViewBag.Owner = db.C10_Owner.ToList();

            ViewBag.Position = db.C17_Position.ToList();
            ViewBag.Descipline = db.C18_Descipline.ToList();

            List<C02_Members> mem = db.C02_Members.Where(s => s.Deactived == false).ToList();
            return View(mem);
        }

        // GET: Member/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMember(string UserName, string Pass, string FullName, string ShortName, string Email, int Descipline, int Position)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Member/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Member/Edit/5
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

        // GET: Member/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Member/Delete/5
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
    }
}
