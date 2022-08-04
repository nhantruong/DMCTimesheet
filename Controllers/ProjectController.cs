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

        /// <summary>
        /// Trang Index - toàn bộ dự án
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Tạo mới dự án
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProjectOtherName"></param>
        /// <param name="StartDate"></param>
        /// <param name="Year"></param>
        /// <param name="ProjectTypeId"></param>
        /// <param name="ProjectStatusId"></param>
        /// <param name="LocationId"></param>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Gửi thông tin lên trang Asssignment
        /// </summary>
        /// <returns></returns>
        public ActionResult AssignDesigner()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.SubCons = db.C12_SubContractor.ToList();
            ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();

            return View(db.C09_ProjectSubCon.ToList());
        }

        /// <summary>
        /// Thêm mới NTP cho dự án - SubCont Assignment
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="ProjectId"></param>
        /// <param name="ArcId"></param>
        /// <param name="InteriorId"></param>
        /// <param name="StructureId"></param>
        /// <param name="MEPid"></param>
        /// <param name="LandscapeId"></param>
        /// <param name="LegalId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDesigner(DateTime Date, string ProjectId, int ArcId, int InteriorId, int StructureId, int MEPid, int LandscapeId, int LegalId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");            
            try
            {
                C09_ProjectSubCon current = db.C09_ProjectSubCon.First(s => s.ProjectId == ProjectId);
                if (current == null)
                {
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
                    if (ModelState.IsValid)
                    {
                        db.C09_ProjectSubCon.Add(assignProject);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Session["ProjectAssignError"] = $"Đã có điều phối NTP cho dự án này";
                    RedirectToAction("AssignDesigner");
                }
                return RedirectToAction("AssignDesigner");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi Lưu do {ex.Message}";
                return View("AssignDesigner");
            }


        }

        /// <summary>
        /// Send thông tin lên trang edit
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Edit(string Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (string.IsNullOrEmpty(Id)) return RedirectToAction("Index");
            try
            {
                C01_Projects Enity = db.C01_Projects.FirstOrDefault(s => s.ProjectID == Id);
                if (Enity == null) return RedirectToAction("Index");
                //Thông tin chung
                ViewBag.ProjectType = db.C13_ProjectType.ToList();
                ViewBag.Status = db.C16_Status.ToList();
                ViewBag.Location = db.C11_Location.ToList();
                ViewBag.Owner = db.C10_Owner.ToList();

                return View(Enity);
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Có lỗi trong quá trình Hiển thị thông tin do {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Edit thông tin dự án
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProjectOtherName"></param>
        /// <param name="StartDate"></param>
        /// <param name="Year"></param>
        /// <param name="ProjectTypeId"></param>
        /// <param name="ProjectStatusId"></param>
        /// <param name="LocationId"></param>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string ProjectID, string ProjectName, string ProjectOtherName, DateTime StartDate, int Year,
            int ProjectTypeId, int ProjectStatusId, int LocationId, int OwnerId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (string.IsNullOrEmpty(ProjectID)) return RedirectToAction("Index");
            try
            {
                C01_Projects enity = db.C01_Projects.First(s => s.ProjectID == ProjectID.Trim());
                if (enity == null)
                {
                    ViewBag.SaveContent = $"Có lỗi trong do không tìm thấy dự án trong database";
                    return View();
                }
                enity.ProjectName = ProjectName;
                enity.ProjectOtherName = ProjectOtherName;
                enity.StartDate = StartDate;
                enity.Year = Year;
                enity.ProjectTypeId = ProjectTypeId;
                enity.ProjectStatusId = ProjectStatusId;
                enity.LocationId = LocationId;
                enity.OwnerId = OwnerId;
                if (ModelState.IsValid)
                {
                    db.Entry(enity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Có lỗi trong quá trình Hiển thị thông tin do {ex.Message}";
                return View();
            }
        }




        /// <summary>
        /// Edit thông tin điều phối thầu phụ thiết kế cho dự án
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditAssign(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("AssignDesigner");

            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.SubCons = db.C12_SubContractor.ToList();
            ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();

            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssign(int Id, DateTime Date, string ProjectId, int ArcId, int InteriorId, int StructureId, int MEPid, int LandscapeId, int LegalId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("AssignDesigner");

            enity.Date = Date;
            enity.ProjectId = ProjectId;
            enity.InteriorId = InteriorId;
            enity.StructureId = StructureId;
            enity.MEPid = MEPid;
            enity.LandscapeId = LandscapeId;
            enity.LegalId = LegalId;
            enity.ArcId = ArcId;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(enity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AssignDesigner");
                }
                else
                {
                    ViewBag.Error = $"Có lỗi khi kết nối server";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi cập nhật dữ liệu do {ex.Message}";
                return View();
            }

        }

    }

}