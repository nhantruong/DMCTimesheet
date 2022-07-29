using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class TimesheetController : Controller
    {

        private readonly dmcDbcontext db = new dmcDbcontext();
        //string conn = "server = 103.27.60.66; user id=dmcAdmin;password=DmcNewVision@2022#; persistsecurityinfo = True; database =cbimtech_dmc";

        #region General
        public ActionResult ProjectTimesheet()
        {

            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            ViewBag.projects = CollectModelData.GetProjectsByUserId(logUser.UserID);
            List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();
            return View(mytimesheet);
        }


        public ActionResult MemberTimesheet()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;

            ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.WorkGroup = db.C07_WorkType.ToList();
            List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();
            return View(mytimesheet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTimesheet(string ProjectID, string RecordDate, int WorkID, string Hour, string OvertimeHour, string Description)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            DateTime inputDate = DateTime.Parse(RecordDate);
            DateTime recordDate = DateTime.Compare(inputDate, DateTime.Now) > 0 ? DateTime.Now : inputDate;

            //           var WorkName = db.C07_WorkType.Where(s => s.ID == WorkID).FirstOrDefault().WorkName;

            int Hours = int.Parse(Hour);
            int _OT = int.Parse(OvertimeHour);

            C08_Timesheet enity = new C08_Timesheet
            {
                MemberID = logUser.UserID,
                RecordDate = recordDate,
                ProjectId = ProjectID,
                WorkType = WorkID,
                Description = Description,
                Hour = Hours,
                OT = _OT
            };
            try
            {
                using (dmcDbcontext dbcontext = new dmcDbcontext())
                {
                    dbcontext.C08_Timesheet.Add(enity);
                    dbcontext.SaveChanges();
                }
                return RedirectToAction("MemberTimesheet");
            }
            catch (Exception ex)
            {
                ViewBag.WorkAlert = "Lưu công việc thất bại do lỗi " + ex.Message;
                return View("MemberTimesheet");
            }

        }


        #endregion



        #region Admin Timesheet

        #endregion






        #region User Timesheet

        public ActionResult UserEdit(int? _Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            try
            {
                C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == _Id);
                if (item == null)
                {
                    ViewBag.WorkAlert = "Không tìm thấy thông tin Timesheet này";
                    return View();
                }
                List<C01_Projects> myProject = CollectModelData.GetProjectsByUserId(logUser.UserID);
                if (myProject == null || myProject.Count()==0)
                {
                    ViewBag.WorkAlert = "Bạn chưa được chỉ định dự án nào";
                    return View("MemberTimesheet");
                }                
                ViewBag.MyProjects = myProject;                
                ViewBag.WorkType = db.C07_WorkType.ToList();
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.WorkAlert = $"Lưu Thất bại do {ex.Message}";
                return View("MemberTimesheet");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(int id, DateTime date, int workId, int hour, int ot, string des)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                if (ModelState.IsValid)
                {
                    C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == id);
                    item.Description = des;
                    item.OT = ot;
                    item.Hour = hour;
                    item.WorkType = workId;
                    item.RecordDate = date;

                    db.C08_Timesheet.Add(item);
                    db.SaveChanges();
                }
                return RedirectToAction("UserPage", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.WorkAlert = $"Có lỗi khi cập nhật timesheet số {id} do {ex.Message}";
                return View();
            }

        }


        #endregion

        #region WorkType
        /// <summary>
        /// Danh sách worktype
        /// </summary>
        /// <returns></returns>
        public ActionResult Worktype()
        {
            //db.Database.Connection.ConnectionString = conn;
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            List<C07_WorkType> worktypeList = db.C07_WorkType.ToList();
            ViewBag.Worklist = worktypeList;
            return View(worktypeList);
        }

        /// <summary>
        /// Tạo mới worktype
        /// </summary>
        /// <param name="WorkName"></param>
        /// <param name="WorkGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWorktype(string WorkName, int WorkGroup)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
                if (ModelState.IsValid)
                {
                    C07_WorkType item = new C07_WorkType
                    {
                        WorkName = WorkName,
                        WorkGroup = WorkGroup
                    };
                    db.C07_WorkType.Add(item);
                    db.SaveChanges();
                }
                //ViewBag.SaveContent = "Add new worktype success";
                return RedirectToAction("WorkType");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Có lỗi trong quá trình lưu data trên server do {ex.Message}";
                return View("WorkType");
            }
        }

        [HttpGet]
        public ActionResult EditWorktype(int id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C07_WorkType enity = db.C07_WorkType.Where(s => s.ID == id).FirstOrDefault();
            if (enity == null)
            {
                ViewBag.SaveContent = "Không tìm thấy Worktype này";
                return View();
            }
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWorktype(int Id, string WorkName, int? WorkGroup)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
                if (ModelState.IsValid)
                {
                    C07_WorkType item = db.C07_WorkType.Where(s => s.ID == Id).FirstOrDefault();
                    if (item == null)
                    {
                        ViewBag.SaveContent = "Không tìm thấy Worktype này";
                        return View();
                    }
                    item.WorkName = WorkName;
                    item.WorkGroup = WorkGroup ?? 1;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("WorkType");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Có lỗi trong quá trình Cập nhật do {ex.Message}";
                return View();
            }
        }


        /// <summary>
        /// Page hiển thị thông tin Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteWorktype(int? id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C07_WorkType enity = db.C07_WorkType.Where(s => s.ID == id).FirstOrDefault();
            if (enity == null)
            {
                ViewBag.SaveContent = "Không tìm thấy Worktype này";
                return View();
            }
            return View(enity);
        }

        [HttpPost, ActionName("DeleteWorktype")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteWorktype(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C07_WorkType enity = db.C07_WorkType.Where(s => s.ID == Id).FirstOrDefault();
                if (enity == null)
                {
                    ViewBag.SaveContent = "Không tìm thấy Worktype này";
                    return View();
                }
                db.C07_WorkType.Remove(enity);
                db.SaveChanges();
                //ViewBag.SaveContent = "Xóa thành công";
                return RedirectToAction("WorkType");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Xóa lỗi do {ex.Message}";
                return View("WorkType");
            }

        }


        public JsonResult GetWorktypebyID(int WorktypeId)
        {
            try
            {
                var worktype = db.C07_WorkType.Where(a => a.ID == WorktypeId).FirstOrDefault();
                return Json(worktype, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateWorktype(C07_WorkType workType)
        {

            string status = "success";
            try
            {
                db.Entry(workType).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                status = ex.Message;

            }
            return Json(workType, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
