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
        public ActionResult CreateNewProject(string MaDuAn, string ProjectName, string ProjectOtherName, DateTime StartDate, int Year, int? ProjectTypeId, int? ProjectStatusId, int? LocationId, int? OwnerId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");            
            C01_Projects _Projects = new C01_Projects()
            {                
                ProjectName = ProjectName,
                ProjectOtherName = ProjectOtherName,
                StartDate = StartDate,
                Year = Year,
                ProjectTypeId = ProjectTypeId,
                LocationId = LocationId,
                OwnerId = OwnerId,
                ProjectStatusId = ProjectStatusId,
                MaDuAn = MaDuAn

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
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.ProjectType = db.C13_ProjectType.ToList();
                ViewBag.Status = db.C16_Status.ToList();
                ViewBag.Location = db.C11_Location.ToList();
                ViewBag.Owner = db.C10_Owner.ToList();

                return View("Index", db.C01_Projects.ToList());
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
        public ActionResult AssignDesigner(DateTime Date, int ProjectId, int? ArcId, int? InteriorId, int? StructureId, int? MEPid, int? LandscapeId, int? LegalId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");            
            try
            {
                C09_ProjectSubCon current = db.C09_ProjectSubCon.FirstOrDefault(s => s.ProjectId == ProjectId);
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
                    //Session["ProjectAssignError"] = $"Đã có điều phối NTP cho dự án này";
                    ViewBag.Projects = db.C01_Projects.ToList();
                    ViewBag.SubCons = db.C12_SubContractor.ToList();
                    ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();
                    ViewBag.Error = $"Đã có điều phối NTP cho dự án này";
                    return View("AssignDesigner", db.C09_ProjectSubCon.ToList());
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
        public ActionResult Edit(int? Id)
        {
            if (Session["UserLogin"] == null || Id == null) return RedirectToAction("Login", "Home");
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
        public ActionResult Edit(int ProjectID,string MaDuAn, string ProjectName, string ProjectOtherName, DateTime StartDate, int Year,
            int? ProjectTypeId, int? ProjectStatusId, int? LocationId, int? OwnerId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");            
            try
            {
                C01_Projects enity = db.C01_Projects.First(s => s.ProjectID == ProjectID);
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
        public ActionResult EditAssign(int? Id)
        {
            if (Session["UserLogin"] == null || Id == null) return RedirectToAction("Login", "Home");
            C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("AssignDesigner");

            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.SubCons = db.C12_SubContractor.ToList();
            ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();

            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssign(int? Id, DateTime Date, int? ProjectId, int? ArcId, int? InteriorId, int? StructureId, int? MEPid, int? LandscapeId, int? LegalId)
        {
            if (Session["UserLogin"] == null || Id == null) return RedirectToAction("Login", "Home");
            if (ProjectId == null) return RedirectToAction("EditAssign");
            
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


        [HttpGet]
        public ActionResult DeleteProject(int? Id)
        {
            if (Session["UserLogin"] == null || Id == null) return RedirectToAction("Login", "Home");
            C01_Projects Enity = db.C01_Projects.FirstOrDefault(s => s.ProjectID == Id);
            if (Enity == null) return RedirectToAction("Index");
            //Thông tin chung
            ViewBag.ProjectType = db.C13_ProjectType.ToList();
            ViewBag.Status = db.C16_Status.ToList();
            ViewBag.Location = db.C11_Location.ToList();
            ViewBag.Owner = db.C10_Owner.ToList();

            return View(Enity);
        }


        [HttpPost, ActionName("DeleteConfirmed")]
       // [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? ProjectID)
        {   
            if (Session["UserLogin"] == null || ProjectID == null) return RedirectToAction("Login", "Home");            
            try
            {
                C01_Projects Enity = db.C01_Projects.FirstOrDefault(s => s.ProjectID == ProjectID);
                if (Enity == null) return RedirectToAction("DeleteProject");
                //Thông tin chung
                if(ModelState.IsValid)
                {
                    db.C01_Projects.Remove(Enity);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ProjectType = db.C13_ProjectType.ToList();
                ViewBag.Status = db.C16_Status.ToList();
                ViewBag.Location = db.C11_Location.ToList();
                ViewBag.Owner = db.C10_Owner.ToList();
                Session["Error"] =  $"Dự án còn dữ liệu liên quan (Timesheet, Điều phối nhân sự) cần xóa trước khi xóa dự án.";
                ViewBag.SaveContent = $"{ex.Message}";
                ViewBag.Bool = true;
                return View("DeleteProject");
            }
        }

        
        [HttpGet]
        public ActionResult DeleteAssign(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("AssignDesigner");

            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.SubCons = db.C12_SubContractor.ToList();
            ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();

            return View(enity);
        }


        [HttpPost, ActionName("DeleteAssignConfirmed")]
       // [ValidateAntiForgeryToken]
        public ActionResult DeleteAssignConfirmed(int Id)
        {   
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");            
            try
            {
                C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);
                if (enity == null) return RedirectToAction("AssignDesigner");
                //Thông tin chung
                if (ModelState.IsValid)
                {
                    db.C09_ProjectSubCon.Remove(enity);
                    db.SaveChanges();
                }
                return RedirectToAction("AssignDesigner");
            }
            catch (Exception ex)
            {
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.SubCons = db.C12_SubContractor.ToList();
                ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();
                //Session["Error"] =  $"Dự án còn dữ liệu liên quan cần xóa trước khi xóa dự án.";


                C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);

                ViewBag.SaveContent = $"Dự án còn dữ liệu liên quan cần xóa trước khi xóa dự án - {ex.Message}";
                ViewBag.Bool = true;
                return View("DeleteAssign");
            }
        }


    }

}