using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class SettingController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();

        #region 1- WorkType
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

        #region 2-Chủ Đầu tư

        public ActionResult Ownerlist()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Location = db.C11_Location.ToList();
            return View(db.C10_Owner.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewOwner(string OwnerName, string ShortName, string KeyPerson, string Email, int OnwerLocation, string OnwerDescription)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C10_Owner curEnity = db.C10_Owner.FirstOrDefault(s => s.OwnerName.Contains(OwnerName));
                if (curEnity != null)
                {
                    ViewBag.Error = $"Đã có CDT tên {OwnerName}";
                    ViewBag.Location = db.C11_Location.ToList();
                    return View("Ownerlist", db.C10_Owner.ToList());
                }
                C10_Owner newEnity = new C10_Owner()
                {
                    OwnerName = OwnerName,
                    ShortName = ShortName,
                    Email = Email,
                    OnwerLocation = OnwerLocation,
                    OnwerDescription = OnwerDescription,
                    KeyPerson = KeyPerson,
                    Active = true
                };
                db.C10_Owner.Add(newEnity);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                ViewBag.Location = db.C11_Location.ToList();
                return View("Ownerlist", db.C10_Owner.ToList());
            }
            return RedirectToAction("Ownerlist");
        }

        public ActionResult EditOwner(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C10_Owner enity = db.C10_Owner.FirstOrDefault(s => s.OnwerId == Id);
            if (enity == null)
            {
                ViewBag.Error = $"Không tìm thấy thông tin CDT này";
                ViewBag.Location = db.C11_Location.ToList();
                return View();
            }
            ViewBag.Location = db.C11_Location.ToList();
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOwner(int Id, string OwnerName, string ShortName, string KeyPerson, string Email, int OnwerLocation, string OnwerDescription)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C10_Owner enity = db.C10_Owner.FirstOrDefault(s => s.OnwerId == Id);
                if (enity == null) 
                {
                    ViewBag.Error = $"Không tìm thấy thông tin CDT này";
                    ViewBag.Location = db.C11_Location.ToList();
                    return View();
                }
                enity.OwnerName = OwnerName;
                enity.ShortName = ShortName;
                enity.KeyPerson = KeyPerson;
                enity.Email = Email;
                enity.OnwerLocation = OnwerLocation;
                enity.OnwerDescription = OnwerDescription;
                enity.Active = true;


                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi cập nhật thông tin của CDT này do {ex.Message}";
                ViewBag.Location = db.C11_Location.ToList();
                return View();
            }
            return RedirectToAction("Ownerlist");
        }
        public ActionResult DeleteOwner(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C10_Owner enity = db.C10_Owner.FirstOrDefault(s => s.OnwerId == Id);
            if (enity == null)
            {
                ViewBag.Error = $"Không tìm thấy thông tin CDT này";
                ViewBag.Location = db.C11_Location.ToList();
                return View();
            }
            ViewBag.Location = db.C11_Location.ToList();
            return View(enity);
        }


        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int OnwerId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C10_Owner enity = db.C10_Owner.FirstOrDefault(s => s.OnwerId == OnwerId);
                if (enity == null)
                {
                    ViewBag.Error = $"Không tìm thấy thông tin CDT này";
                    ViewBag.Location = db.C11_Location.ToList();
                    return View();
                }                
                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi cập nhật thông tin của CDT này do {ex.Message}";
                ViewBag.Location = db.C11_Location.ToList();
                return View();
            }
            return RedirectToAction("Ownerlist");
        }
        #endregion

        #region 3-Thầu phụ thiết kế

        #endregion

        #region 4-Dịch vụ NTP

        #endregion

        #region 5-Dịch vụ

        #endregion

        #region 6-Tình trạng

        #endregion

        #region 7-Loại hình dự án

        #endregion

        #region 8-Địa điểm

        #endregion

    }
}
