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

        #region 1- WorkType - workgroup
        /// <summary>
        /// Danh sách worktype
        /// </summary>
        /// <returns></returns>
        public ActionResult Worktype()
        {
            //db.Database.Connection.ConnectionString = conn;
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.WorkGroup = db.C19_Workgroup.ToList();
            return View(db.C07_WorkType.ToList());
        }

        /// <summary>
        /// Tạo mới worktype
        /// </summary>
        /// <param name="WorkName"></param>
        /// <param name="WorkGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWorktype(string WorkName, int GroupId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C07_WorkType ExistingName = db.C07_WorkType.FirstOrDefault(s => s.WorkName == WorkName);
                if (string.IsNullOrEmpty(WorkName) || GroupId <= 0 || ExistingName != null)
                {
                    ViewBag.SaveContent = $"Phải có tên nhóm hoặc tên nhóm đã có trong database";
                    ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                    return View("WorkType", db.C07_WorkType.ToList());
                }
                if (ModelState.IsValid)
                {
                    C07_WorkType item = new C07_WorkType()
                    {
                        WorkName = WorkName,
                        GroupId = GroupId
                    };
                    db.C07_WorkType.Add(item);
                    db.SaveChanges();
                }
                ViewBag.SaveContent = "Add new worktype success";
                return RedirectToAction("Worktype");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Có lỗi trong quá trình lưu data trên server do {ex.InnerException.Message}";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View("Worktype", db.C07_WorkType.ToList());
            }
        }

        [HttpGet]
        public ActionResult EditWorktype(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("WorkType");
            C07_WorkType enity = db.C07_WorkType.FirstOrDefault(s => s.ID == Id);
            if (enity == null)
            {
                ViewBag.SaveContent = "Không tìm thấy Worktype này";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
            }
            ViewBag.WorkGroup = db.C19_Workgroup.ToList();
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWorktype(int? Id, string WorkName, int? GroupId)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
                if (ModelState.IsValid)
                {
                    C07_WorkType item = db.C07_WorkType.FirstOrDefault(s => s.ID == Id);
                    if (item == null)
                    {
                        ViewBag.SaveContent = "Không tìm thấy Worktype này";
                        ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                        return View(db.C07_WorkType.ToList());
                    }
                    item.WorkName = WorkName;
                    item.GroupId = (int)GroupId;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Worktype");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Có lỗi trong quá trình Cập nhật do {ex.Message}";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
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
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
            }
            ViewBag.WorkGroup = db.C19_Workgroup.ToList();
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
                    ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                    return View(db.C07_WorkType.ToList());
                }
                db.C07_WorkType.Remove(enity);
                db.SaveChanges();
                //ViewBag.SaveContent = "Xóa thành công";
                return RedirectToAction("Worktype");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Xóa lỗi do {ex.Message}";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
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

        #region 1A-WORKGROUP - Nhóm công việc

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWorkGroup(string GroupName, int? MaNhom)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C19_Workgroup ExistingName = db.C19_Workgroup.FirstOrDefault(s => s.GroupName.Contains(GroupName) || s.MaNhom == MaNhom);
            if (string.IsNullOrEmpty(GroupName) || ExistingName != null)
            {
                ViewBag.SaveContent = $"Không bỏ trống tên nhóm hoặc Tên nhóm đã có trong database";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
            }
            try
            {
                C19_Workgroup newEnity = new C19_Workgroup() { GroupName = GroupName.Trim(), MaNhom = (int)MaNhom };
                db.C19_Workgroup.Add(newEnity);
                db.SaveChanges();
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return RedirectToAction("Worktype");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Tạo mới bị lỗi do {ex.Message}";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
            }
        }

        public ActionResult EditWorkGroup(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("Worktype");
            C19_Workgroup enity = db.C19_Workgroup.FirstOrDefault(s => s.GroupId == Id);
            if (enity == null) return RedirectToAction("Worktype");
            return View(enity);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWorkGroup(int? Id, string GroupName, int? MaNhom)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null || string.IsNullOrEmpty(GroupName)) return RedirectToAction("WorkType");
            C19_Workgroup enity = db.C19_Workgroup.FirstOrDefault(s => s.GroupId == Id);
            if (enity == null) return RedirectToAction("Worktype");
            try
            {
                enity.GroupName = GroupName;
                enity.MaNhom = MaNhom;
                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Worktype");
            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Cập nhật bị lỗi do {ex.Message}";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
            }

        }

        public ActionResult DeleteWorkGroup(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("Worktype");
            C19_Workgroup enity = db.C19_Workgroup.FirstOrDefault(s => s.GroupId == Id);
            if (enity == null) return RedirectToAction("Worktype");
            return View(enity);
        }


        [HttpPost, ActionName("DeleteWorkGroupConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteWorkGroupConfirmed(int? GroupId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            //if (GroupId == null) return RedirectToAction("WorkType");
            if (GroupId == null)
            {
                ViewBag.SaveContent = "Không tìm thấy WorkGroup này";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View("Worktype", db.C07_WorkType.ToList());
            }
            C19_Workgroup enity = db.C19_Workgroup.FirstOrDefault(s => s.GroupId == GroupId);
            if (enity == null) return RedirectToAction("Worktype");
            try
            {
                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Worktype");

            }
            catch (Exception ex)
            {
                ViewBag.SaveContent = $"Xóa bị lỗi do {ex.Message}";
                ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                return View(db.C07_WorkType.ToList());
            }
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
        public ActionResult CreateNewOwner(string OwnerName, string ShortName, string KeyPerson, string Email, int? OnwerLocation, string OnwerDescription)
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
        public ActionResult EditOwner(int? Id, string OwnerName, string ShortName, string KeyPerson, string Email, int? OnwerLocation, string OnwerDescription)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("Ownerlist", db.C10_Owner.ToList());
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
        public ActionResult DeleteOwner(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("Ownerlist", db.C10_Owner.ToList());
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
        public ActionResult DeleteConfirmed(int? OnwerId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (OnwerId == null) return RedirectToAction("Ownerlist", db.C10_Owner.ToList());
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

        public ActionResult SubContractors()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Location = db.C11_Location.ToList();
            return View(db.C12_SubContractor.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewSubContractor(string ContractorName, string ShortName, int? ContractorLocation,
            bool? ARC, bool? STR, bool? MEP, bool? Interior, bool? Infrastructure, bool? DTM, bool? Render, bool? Landscape)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
                C12_SubContractor newEnity = new C12_SubContractor()
                {
                    ContractorName = ContractorName,
                    ShortName = ShortName,
                    ContractorLocation = ContractorLocation,
                    ARC = ARC,
                    STR = STR,
                    MEP = MEP,
                    Interior = Interior,
                    Infrastructure = Infrastructure,
                    Render = Render,
                    Landscape = Landscape,
                    DTM = DTM
                };
                db.C12_SubContractor.Add(newEnity);
                ViewBag.SaveContent = "Lưu thành công";
                db.SaveChanges();

                ViewBag.Location = db.C11_Location.ToList();
                return View("SubContractors", db.C12_SubContractor.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                ViewBag.Location = db.C11_Location.ToList();
                return View("SubContractors", db.C12_SubContractor.ToList());
            }
        }

        public ActionResult EditNTP(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("SubContractors");
            C12_SubContractor enity = db.C12_SubContractor.FirstOrDefault(s => s.SubConId == Id);
            if (enity == null) return RedirectToAction("SubContractors");
            ViewBag.Location = db.C11_Location.ToList();
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNTP(int? Id, string ContractorName, string ShortName, int? ContractorLocation,
            bool? ARC, bool? STR, bool? MEP, bool? Interior, bool? Infrastructure, bool? DTM, bool? Render, bool? Landscape)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("SubContractors");
            C12_SubContractor enity = db.C12_SubContractor.FirstOrDefault(s => s.SubConId == Id);
            if (enity == null) return RedirectToAction("SubContractors");
            try
            {
                enity.ContractorName = ContractorName;
                enity.ShortName = ShortName;
                enity.ContractorLocation = ContractorLocation;
                enity.ARC = ARC;
                enity.STR = STR;
                enity.MEP = MEP;
                enity.Interior = Interior;
                enity.Infrastructure = Infrastructure;
                enity.DTM = DTM;
                enity.Render = Render;
                enity.Landscape = Landscape;

                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Location = db.C11_Location.ToList();
                return View("SubContractors", db.C12_SubContractor.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                ViewBag.Location = db.C11_Location.ToList();
                return View("SubContractors", db.C12_SubContractor.ToList());
            }

        }

        public ActionResult DeleteNTP(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("SubContractors");
            C12_SubContractor enity = db.C12_SubContractor.FirstOrDefault(s => s.SubConId == Id);
            if (enity == null) return RedirectToAction("SubContractors");
            ViewBag.Location = db.C11_Location.ToList();
            return View(enity);
        }


        [HttpPost, ActionName("DeleteNTPConfirm")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNTPConfirm(int? SubConId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (SubConId == null) return RedirectToAction("SubContractors");
            C12_SubContractor enity = db.C12_SubContractor.FirstOrDefault(s => s.SubConId == SubConId);
            if (enity == null) return RedirectToAction("SubContractors");
            try
            {
                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("SubContractors");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                ViewBag.Location = db.C11_Location.ToList();
                return View("DeleteNTP");
            }

        }



        #endregion

        #region 4-Tình trạng
        public ActionResult StatusList()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            return View(db.C16_Status.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewStatus(string StatusName, string ColorCode)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (string.IsNullOrEmpty(StatusName) || string.IsNullOrEmpty(ColorCode)) return RedirectToAction("StatusList");
            C16_Status status = new C16_Status() { StatusName = StatusName, ColorCode = ColorCode };
            try
            {
                db.C16_Status.Add(status);
                db.SaveChanges();
                return RedirectToAction("StatusList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                return View("StatusList", db.C16_Status.ToList());
            }

        }




        public ActionResult EditStatus(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("StatusList");
            C16_Status enity = db.C16_Status.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("StatusList");

            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStatus(int? Id, string StatusName, string ColorCode)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("StatusList");

            try
            {
                C16_Status enity = db.C16_Status.FirstOrDefault(s => s.Id == Id);
                if (enity == null) return RedirectToAction("StatusList");

                enity.StatusName = StatusName;
                enity.ColorCode = ColorCode;
                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();
                return View("StatusList", db.C16_Status.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                return View("StatusList", db.C16_Status.ToList());
            }
        }

        public ActionResult DeleteStatus(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("StatusList");
            C16_Status enity = db.C16_Status.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("StatusList");

            return View(enity);
        }

        [HttpPost, ActionName("DeleteStatusConfirm")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStatusConfirm(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("StatusList");

            try
            {
                C16_Status enity = db.C16_Status.FirstOrDefault(s => s.Id == Id);
                if (enity == null) return RedirectToAction("StatusList");

                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("StatusList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                return View("DeleteStatus");
            }
        }

        #endregion

        #region 5. Position - Chức danh
        public ActionResult PositionList()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            return View(db.C17_Position.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewPosition(string PositionName, int? Active)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (string.IsNullOrEmpty(PositionName)) return RedirectToAction("PositionList");
            try
            {
                bool active = false;
                if (Active == null || Active == 0) active = false; else active = true;
                C17_Position newEnity = new C17_Position() { PositionName = PositionName, Active = active };
                db.C17_Position.Add(newEnity);
                db.SaveChanges();
                return RedirectToAction("PositionList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.Message}";
                return View("PositionList", db.C17_Position.ToList());
            }
        }

        public ActionResult EditPosition(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("PositionList");
            C17_Position enity = db.C17_Position.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("PositionList");
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPosition(int? Id, string PositionName, int? Active)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("PositionList");
            C17_Position enity = db.C17_Position.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("PositionList");

            try
            {
                enity.PositionName = PositionName;
                enity.Active = Active != 0;
                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PositionList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View();
            }
        }

        public ActionResult DeletePosition(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("PositionList");
            C17_Position enity = db.C17_Position.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("PositionList");
            return View(enity);
        }

        [HttpPost, ActionName("DeletePositionConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePositionConfirmed(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("PositionList");
            C17_Position enity = db.C17_Position.FirstOrDefault(s => s.Id == Id);
            if (enity == null) return RedirectToAction("PositionList");
            try
            {
                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("PositionList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View();
            }

        }



        #endregion

        #region 6-Loại hình dự án
        public ActionResult ProjectType()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            return View(db.C13_ProjectType.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewProjectType(string TypeName, string Description)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C13_ProjectType newEnity = new C13_ProjectType() { TypeName = TypeName, Description = Description };
                db.C13_ProjectType.Add(newEnity);
                db.SaveChanges();
                return RedirectToAction("ProjectType");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View("ProjectType", db.C13_ProjectType.ToList());
            }

        }

        public ActionResult EditProjectType(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("ProjectType");
            C13_ProjectType enity = db.C13_ProjectType.FirstOrDefault(s => s.TypeId == Id);
            if (enity == null) return RedirectToAction("ProjectType");
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectType(int? TypeId, string TypeName, string Description)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (TypeId == null) return RedirectToAction("ProjectType");
            C13_ProjectType enity = db.C13_ProjectType.FirstOrDefault(s => s.TypeId == TypeId);
            if (enity == null) return RedirectToAction("ProjectType");

            try
            {
                enity.TypeName = TypeName;
                enity.Description = Description;
                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProjectType");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View();
            }
        }

        public ActionResult DeleteProjectType(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("ProjectType");
            C13_ProjectType enity = db.C13_ProjectType.FirstOrDefault(s => s.TypeId == Id);
            if (enity == null) return RedirectToAction("ProjectType");
            return View(enity);
        }

        [HttpPost, ActionName("DeleteProjectTypeConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProjectTypeConfirmed(int? TypeId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (TypeId == null) return RedirectToAction("ProjectType");
            C13_ProjectType enity = db.C13_ProjectType.FirstOrDefault(s => s.TypeId == TypeId);
            if (enity == null) return RedirectToAction("ProjectType");
            try
            {
                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("ProjectType");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View();
            }
        }



        #endregion

        #region 7-Địa điểm
        public ActionResult Locations()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            return View(db.C11_Location.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewLocation(string LocationName)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C11_Location location = db.C11_Location.FirstOrDefault(s => s.LocationName.Contains(LocationName.Trim()) || s.LocationName == LocationName.Trim());
                //C11_Location location = db.C11_Location.Find(LocationName.Trim());
                if (location != null)
                {
                    ViewBag.Error = $"Đã có tên địa phương này";
                    return View("Locations", db.C11_Location.ToList()) ;
                }
                else
                {
                    C11_Location newEnity = new C11_Location() { LocationName = LocationName };
                    db.C11_Location.Add(newEnity);
                    db.SaveChanges();
                    return RedirectToAction("Locations");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View("Locations", db.C11_Location.ToList());
            }

        }

        public ActionResult EditLocation(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("Locations");
            C11_Location enity = db.C11_Location.FirstOrDefault(s => s.LocationId == Id);
            if (enity == null) return RedirectToAction("Locations");
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLocation(int? LocationId, string LocationName)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (LocationId == null) return RedirectToAction("Locations");
            C11_Location enity = db.C11_Location.FirstOrDefault(s => s.LocationId == LocationId);
            if (enity == null) return RedirectToAction("Locations");
            try
            {
                enity.LocationName = LocationName;

                db.Entry(enity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Locations");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View();
            }
        }

        public ActionResult DeleteLocation(int? Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id == null) return RedirectToAction("Locations");
            C11_Location enity = db.C11_Location.FirstOrDefault(s => s.LocationId == Id);
            if (enity == null) return RedirectToAction("Locations");
            return View(enity);
        }

        [HttpPost, ActionName("DeleteLocationConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLocationConfirmed(int? LocationId)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (LocationId == null) return RedirectToAction("Locations");
            C11_Location enity = db.C11_Location.FirstOrDefault(s => s.LocationId == LocationId);
            if (enity == null) return RedirectToAction("Locations");
            try
            {
                db.Entry(enity).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Locations");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xãy ra do {ex.InnerException.Message}";
                return View("Locations");
            }
        }
        #endregion

        #region 8-Dịch vụ NTP

        #endregion

        #region 9-Dịch vụ

        #endregion





    }
}
