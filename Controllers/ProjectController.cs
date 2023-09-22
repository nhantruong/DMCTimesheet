using DMCTimesheet.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DMCTimesheet.Controllers
{
    public class ProjectController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();
        
        #region C# MVC

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
            ViewBag.Stage = db.C20_Stage.ToList();
            ViewBag.NguonViec = db.C23_NguonViec.ToList();

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
        public ActionResult CreateNewProject(string MaDuAn, string ProjectName, string ProjectOtherName, DateTime StartDate, int Year, int? ProjectTypeId, int? ProjectStatusId, int? LocationId, int? OwnerId, int? ProjectStage, int? NguonViec)
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
                MaDuAn = MaDuAn,
                ProjectStage = ProjectStage,
                NguonViec = NguonViec

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
                ViewBag.Stage = db.C20_Stage.ToList();
                ViewBag.NguonViec = db.C23_NguonViec.ToList();

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

            ViewBag.Services = db.C14_Services.ToList();
            ViewBag.ContractorServices = db.C15_SubConServices.ToList();

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
        public ActionResult AssignDesigner(DateTime Date, int ProjectId, int? SubConId, int? ServicesAssign)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                //C09_ProjectSubCon current = db.C09_ProjectSubCon.FirstOrDefault(s => s.ProjectId == ProjectId);
                //if (current == null)
                //{
                C09_ProjectSubCon assignProject = new C09_ProjectSubCon
                {
                    Date = Date,
                    ProjectId = ProjectId,
                    SubConId = SubConId,
                    ServicesAssign = ServicesAssign
                };
                if (ModelState.IsValid)
                {
                    db.C09_ProjectSubCon.Add(assignProject);
                    db.SaveChanges();
                }
                //}
                //else
                //{
                //    //Session["ProjectAssignError"] = $"Đã có điều phối NTP cho dự án này";
                //    ViewBag.Projects = db.C01_Projects.ToList();
                //    ViewBag.SubCons = db.C12_SubContractor.ToList();
                //    ViewBag.AssignProject = db.C09_ProjectSubCon.ToList();
                //    ViewBag.Stage = db.C20_Stage.ToList();
                //    ViewBag.Error = $"Đã có điều phối NTP cho dự án này";

                //    ViewBag.Services = db.C14_Services.ToList();
                //    ViewBag.ContractorServices = db.C15_SubConServices.ToList();

                //    return View("AssignDesigner", db.C09_ProjectSubCon.ToList());
                //}
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
                ViewBag.Stage = db.C20_Stage.ToList();
                ViewBag.Services = db.C14_Services.ToList();
                ViewBag.ContractorServices = db.C15_SubConServices.ToList();
                ViewBag.nguonviec = db.C23_NguonViec.ToList();
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
        public ActionResult Edit(int ProjectID, string MaDuAn, string ProjectName, string ProjectOtherName, DateTime StartDate, int Year,
            int? ProjectTypeId, int? ProjectStatusId, int? LocationId, int? OwnerId, int? ProjectStage, int? NguonViec, DateTime? ngayKetThuc)
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
                enity.MaDuAn = MaDuAn;
                enity.ProjectStage = ProjectStage;
                enity.NguonViec = NguonViec;
                enity.NgayKetThuc = ngayKetThuc;
                if (ModelState.IsValid)
                {
                    db.Entry(enity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in ex.EntityValidationErrors)
                {

                    sb.Append($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                    }
                }
                ViewBag.SaveContent = $"Có lỗi trong quá trình Hiển thị thông tin do {sb.ToString()}";
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
            ViewBag.Stage = db.C20_Stage.ToList();
            ViewBag.Services = db.C14_Services.ToList();
            ViewBag.ContractorServices = db.C15_SubConServices.ToList();
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssign(int? Id, DateTime Date, int? ProjectId, int? SubConId, int? ServicesAssign)
        {
            if (Session["UserLogin"] == null || Id == null) return RedirectToAction("Login", "Home");
            if (ProjectId == null) return RedirectToAction("EditAssign");

            C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("AssignDesigner");

            enity.Date = Date;
            enity.ProjectId = ProjectId;
            enity.SubConId = SubConId;
            enity.ServicesAssign = ServicesAssign;
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
            ViewBag.Stage = db.C20_Stage.ToList();

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
                if (ModelState.IsValid)
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
                Session["Error"] = $"Dự án còn dữ liệu liên quan (Timesheet, Điều phối nhân sự) cần xóa trước khi xóa dự án.";
                ViewBag.SaveContent = $"{ex.Message}";
                ViewBag.Bool = true;
                ViewBag.Stage = db.C20_Stage.ToList();

                ViewBag.Services = db.C14_Services.ToList();
                ViewBag.ContractorServices = db.C15_SubConServices.ToList();

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
            ViewBag.Stage = db.C20_Stage.ToList();
            ViewBag.Services = db.C14_Services.ToList();
            ViewBag.ContractorServices = db.C15_SubConServices.ToList();
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
                ViewBag.Stage = db.C20_Stage.ToList();
                ViewBag.Services = db.C14_Services.ToList();
                ViewBag.ContractorServices = db.C15_SubConServices.ToList();
                //Session["Error"] =  $"Dự án còn dữ liệu liên quan cần xóa trước khi xóa dự án.";


                C09_ProjectSubCon enity = db.C09_ProjectSubCon.FirstOrDefault(s => s.Id == Id);

                ViewBag.SaveContent = $"Dự án còn dữ liệu liên quan cần xóa trước khi xóa dự án - {ex.Message}";
                ViewBag.Bool = true;
                return View("DeleteAssign");
            }
        }
        
        
        #endregion


        #region JAVASCRIPT-JQUERY

        #region 1. POST from Views
        //POST
        /// <summary>
        /// Add New ProjectType
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="Description"></param>
        public void SaveProjectType(string typename, string Description)
        {
            C13_ProjectType enity = db.C13_ProjectType.FirstOrDefault(s => s.TypeName.Contains(typename));
            if (enity == null)
            {
                try
                {
                    C13_ProjectType newEnity = new C13_ProjectType() { TypeName = string.IsNullOrEmpty(typename) ? "Chưa cập nhật" : typename, Description = Description };

                    if (ModelState.IsValid)
                    {
                        db.C13_ProjectType.Add(newEnity);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Có lỗi xãy ra do {ex.Message}");
                }
            }
            else
            {
                throw new Exception("Đã có record này");
            }

        }

        /// <summary>
        /// Add New Project Status
        /// </summary>
        /// <param name="StatusName"></param>
        /// <param name="ColorCode"></param>
        public void SaveProjectStatus(string StatusName, string ColorCode)
        {
            C16_Status enity = db.C16_Status.FirstOrDefault(s => s.StatusName.Contains(StatusName));
            if (enity == null)
            {
                try
                {
                    string _status = !string.IsNullOrEmpty(StatusName) ? StatusName: "Chưa cập nhật thông tin";
                    C16_Status status = new C16_Status() { StatusName = _status, ColorCode = ColorCode };
                    db.C16_Status.Add(status);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Có lỗi xãy ra do {ex.Message}");
                }
            }
            else
            {
                throw new Exception("Đã có record này");
            }
        }

        /// <summary>
        /// Add New Location
        /// </summary>
        /// <param name="LocationName"></param>
        /// <exception cref="Exception"></exception>
        public void SaveProjectLocation(string LocationName)
        {
            try
            {
                C11_Location location = db.C11_Location.FirstOrDefault(s => s.LocationName.Contains(LocationName.Trim()) || s.LocationName == LocationName.Trim());
                if (location != null)
                {
                    ViewBag.Error = $"Đã có tên địa phương này";
                    throw new Exception("");
                }
                else
                {
                    string _name = string.IsNullOrEmpty(LocationName) ? LocationName : "Chưa cập nhật thông tin";
                    C11_Location newEnity = new C11_Location() { LocationName = _name };
                    db.C11_Location.Add(newEnity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xãy ra do {ex.Message}");
            }

        }

        /// <summary>
        /// Add new Owner
        /// </summary>
        /// <param name="OwnerName"></param>
        /// <param name="ShortName"></param>
        /// <param name="KeyPerson"></param>
        /// <param name="Email"></param>
        /// <param name="OnwerLocation"></param>
        /// <param name="OnwerDescription"></param>
        /// <exception cref="Exception"></exception>
        public void SaveProjectOwner(string OwnerName, string ShortName, string KeyPerson, string Email, int? OnwerLocation, string OnwerDescription)
        {
            try
            {
                C10_Owner curEnity = db.C10_Owner.FirstOrDefault(s => s.OwnerName.Contains(OwnerName));
                if (curEnity == null)
                {
                    C10_Owner newEnity = new C10_Owner()
                    {
                        OwnerName = string.IsNullOrEmpty(OwnerName)?"Chưa cập nhật":OwnerName,
                        ShortName = string.IsNullOrEmpty(ShortName)? "Chưa cập nhật" :ShortName,
                        Email = Email,
                        OnwerLocation = OnwerLocation,
                        OnwerDescription = OnwerDescription,
                        KeyPerson = KeyPerson,
                        Active = true
                    };
                    db.C10_Owner.Add(newEnity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xãy ra do {ex.Message}");
            }

        }

        /// <summary>
        /// Add giai đoạn dự án
        /// </summary>
        /// <param name="LocationName"></param>
        /// <exception cref="Exception"></exception>
        public void SaveProjectState(string StageName)
        {
            try
            {
                C20_Stage location = db.C20_Stage.FirstOrDefault(s => s.StageName.Contains(StageName.Trim()) || s.StageName == StageName.Trim());
                if (location == null)
                {
                    string _name = string.IsNullOrEmpty(StageName) ? "Chưa cập nhật": StageName;
                    C20_Stage newEnity = new C20_Stage() { StageName = _name };
                    db.C20_Stage.Add(newEnity);
                    db.SaveChanges();                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xãy ra do {ex.Message}");

            }

        }



        #endregion
        #region 2. GET data from Views
        //GET
        /// <summary>
        /// Get ProjectType
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProjectTypes()
        {
            return Json(db.C13_ProjectType.Select(s => new { s.TypeId, s.TypeName, s.Description }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Status
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStatus()
        {
            return Json(db.C16_Status.Select(s => new { s.Id, s.StatusName, s.ColorCode }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Locations
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLocations()
        {
            return Json(db.C11_Location.Select(s => new { s.LocationId, s.LocationName }).ToList(), JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Get OwnerList
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOwners()
        {
            return Json(db.C10_Owner.Select(s => new { s.OnwerId, s.OwnerName, s.KeyPerson, s.Email,s.OnwerLocation,s.ShortName,s.Active,s.OnwerDescription }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStates()
        {
            return Json(db.C20_Stage.Select(s => new { s.Id, s.StageName}).ToList(), JsonRequestBehavior.AllowGet);
        }



        #endregion


        #endregion

    }

}