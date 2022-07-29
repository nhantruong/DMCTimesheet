using DMCTimesheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMCTimesheet.Controllers
{
    public class ProjectController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();
        // GET: Project
        public ActionResult Index()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.ProjectType = db.C13_ProjectType.ToList();
            ViewBag.Status = db.C16_Status.ToList();
            ViewBag.Location = db.C11_Location.ToList();
            ViewBag.Owner = db.C10_Owner.ToList();

            return View(db.C01_Projects.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewProject(string ProjectId, string ProjectName, string ProjectOtherName, DateTime StartDate, int Year, int ProjectTypeId, int ProjectStatusId, int LocationId, int OwnerId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;

            C01_Projects _Projects = new C01_Projects()
            {
                ProjectID = ProjectId,
                ProjectName = ProjectName,
                ProjectOtherName = ProjectOtherName,
                StartDate = StartDate,
                Year = Year,
                ProjectTypeId = ProjectTypeId,
                LocationId = LocationId,
                OwnerId = OwnerId,
                ProjectStatusId = ProjectStatusId
            };
            try
            {
                if (ModelState.IsValid)
                {
                    db.C01_Projects.Add(_Projects);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi trong quá trình tạo dự án do {ex.Message}";
                return View("Index");
            }

        }


        public ActionResult AssignDesigner()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.SubCons = db.C12_SubContractor.ToList();
            ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();

            return View(db.C09_ProjectSubCon.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDesigner(DateTime Date, string ProjectId, int ArcId, int InteriorId, int StructureId, int MEPid, int LandscapeId, int LegalId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C09_ProjectSubCon assignProject = new C09_ProjectSubCon
            {
                Date = Date,
                ProjectId = ProjectId,
                ArcId = ArcId,
                StructureId = StructureId,
                InteriorId = InteriorId,
                MEPid = MEPid,
                LandscapeId = LandscapeId,
                LegalId = LegalId
            };
            try
            {
                if (ModelState.IsValid)
                {
                    db.C09_ProjectSubCon.Add(assignProject);
                    db.SaveChanges();
                }
                return RedirectToAction("AssignDesigner");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi Lưu do {ex.Message}";
                return View("AssignDesigner");
            }

        }
    }
}