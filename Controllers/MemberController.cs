using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;
using DMCTimesheet.com.cbimtech.MemberServices;

namespace DMCTimesheet.Controllers
{
    public class MemberController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();


        #region CRUD timesheet

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
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (id < 0) return RedirectToAction("Index");

            ViewBag.Position = db.C17_Position.ToList();
            ViewBag.Descipline = db.C18_Descipline.ToList();
            C02_Members enity = db.C02_Members.FirstOrDefault(s => s.UserID == id);
            if (enity == null) return RedirectToAction("Index");
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int UserID, string UserName, string FullName, string ShortName, string Email, int Descipline, int Position)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (UserID < 0) return RedirectToAction("Index");
            try
            {
                C02_Members enity = db.C02_Members.FirstOrDefault(s => s.UserID == UserID);
                if (enity == null) return RedirectToAction("Index");
                enity.UserName = UserName;
                enity.FullName = FullName;
                enity.ShortName = ShortName;
                enity.Email = Email;
                enity.Descipline = Descipline;
                enity.Position = Position;
                if (ModelState.IsValid)
                {
                    db.Entry(enity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi cập nhật thông tin thành viên do {ex.Message}";
                return View();
            }
        }


        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMember(string UserName, string FullName, string ShortName, string Email, int Descipline, int Position)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
                C02_Members newMember = new C02_Members()
                {
                    UserName = UserName,
                    Pass = HomeController.MaHoaPass("password"),
                    FullName = FullName,
                    ShortName = ShortName,
                    Email = Email,
                    Descipline = Descipline,
                    Position = Position,
                    Deactived = false
                };
                //Set quyen User mac dinh
                C06_UserPermisRelationship c06_UserPermis = new C06_UserPermisRelationship()
                {
                    idPer = 2,//Mã Id quyền User
                    actived = true
                };
                if (ModelState.IsValid)
                {
                    db.C02_Members.Add(newMember);
                    db.SaveChanges();

                    int recordCount = db.C02_Members.Select(s => s.UserID).Max();
                    c06_UserPermis.idUser = recordCount;
                    db.C06_UserPermisRelationship.Add(c06_UserPermis);

                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Member");

                //else
                //{
                //    ViewBag.Error = $"Có lỗi khi tạo mới thành viên do không thể kết nối đến server";
                //    return View();
                //}
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi khi tạo mới thành viên do {ex.Message}";
                return View();
            }
        }


        // GET: Member/Delete/5
        public ActionResult OnOffline(int id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members enity = db.C02_Members.FirstOrDefault(s => s.UserID == id);
            if (enity == null) return RedirectToAction("Index");
            ViewBag.Position = db.C17_Position.ToList();
            ViewBag.Descipline = db.C18_Descipline.ToList();
            return View(enity);
        }

        // POST: Member/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnOffline(int id, bool Deactived)
        {
            try
            {
                if (Session["UserLogin"] == null || id <= 0) return RedirectToAction("Login", "Home");
                C02_Members enity = db.C02_Members.FirstOrDefault(s => s.UserID == id);
                if (enity == null) return RedirectToAction("Index");
                if (!Deactived) return RedirectToAction("Index");
                enity.Deactived = true;
                if (ModelState.IsValid)
                {
                    db.Entry(enity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi trong quá trình chuyển trạng thái của nhân sự do {ex.Message}";
                return View();
            }
        }


        #endregion

        #region Dieu phoi nhan su cho du an

        /// <summary>
        /// Điều phối nhân sự - Index Page
        /// </summary>
        /// <returns></returns>
        public ActionResult AssignMember()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
            List<MemberOutput> BIMer = new List<MemberOutput>();

            using (MemberService memberService = new MemberService())
            {
                BIMer.AddRange(memberService.DanhSachMember().ToList());
            }
            ViewBag.BIMer = BIMer;           

            return View(db.C03_ProjectMembers.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewAssignMember(string ProjectID, int ChuTriChinh, int ChuTriKienTruc, int ChuTriKetCau, int ChuTriMEP, DateTime Ngay,
            int BIMManager, int LegalManager)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C03_ProjectMembers current = db.C03_ProjectMembers.First(s => s.ProjectID == ProjectID);
                if (current == null)
                {
                    C03_ProjectMembers newAssignment = new C03_ProjectMembers()
                    {
                        ProjectID = ProjectID,
                        ChuTriChinh = ChuTriChinh,
                        ChuTriKienTruc = ChuTriKienTruc,
                        ChuTriKetCau = ChuTriKetCau,
                        ChuTriMEP = ChuTriMEP,
                        BIMManager = BIMManager,
                        LegalManager = LegalManager,
                        Ngay = Ngay
                    };
                    if (ModelState.IsValid)
                    {
                        db.C03_ProjectMembers.Add(newAssignment);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Session["MemberAssignError"] = $"Đã có điều phối Thành viên cho dự án này";
                    return RedirectToAction("AssignMember");
                }
                return RedirectToAction("AssignMember");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi trong quá trình tạo mới do {ex.Message}";
                return View();
            }


        }

        public ActionResult EditAssign(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (Id < 0) return RedirectToAction("AssignMember");
            C03_ProjectMembers enity = db.C03_ProjectMembers.FirstOrDefault(s => s.ID == Id);
            if (enity == null)
            {
                Session["MemberAssignError"] = $"Không tìm thấy dự án này";
                return RedirectToAction("AssignMember");
            }
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
            List<MemberOutput> BIMer = new List<MemberOutput>();

            using (MemberService memberService = new MemberService())
            {
                BIMer.AddRange(memberService.DanhSachMember().ToList());
            }
            ViewBag.BIMer = BIMer;

            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssignMember(int ID, string ProjectID, int ChuTriChinh, int ChuTriKienTruc, int ChuTriKetCau, int ChuTriMEP, DateTime Ngay,
            int BIMManager, int LegalManager)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (ID < 0) return RedirectToAction("AssignMember");
            try
            {
                C03_ProjectMembers enity = db.C03_ProjectMembers.FirstOrDefault(s => s.ID == ID);
                if (enity == null)
                {
                    ViewBag.Error = $"Không tìm thấy dự án này";
                    return View("EditAssign");
                }
                enity.ProjectID = ProjectID;
                enity.ChuTriChinh = ChuTriChinh;
                enity.ChuTriKienTruc = ChuTriKienTruc;
                enity.ChuTriKetCau = ChuTriKetCau;
                enity.ChuTriMEP = ChuTriMEP;
                enity.BIMManager = BIMManager;
                enity.Ngay = Ngay;
                enity.LegalManager = LegalManager;

                if (ModelState.IsValid)
                {
                    db.Entry(enity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    Session["MemberAssignError"] = $"Không kết nối được server";
                    return RedirectToAction("AssignMember");
                }
                return RedirectToAction("AssignMember");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi trong khi cập nhật data do {ex.Message}";
                return View("EditAssign");
            }


        }





        #endregion

        #region EditPermission

        public ActionResult EditPermission()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            List<C06_UserPermisRelationship> userPermiss = db.C06_UserPermisRelationship.ToList();
            ViewBag.Members = db.C02_Members.ToList();
            ViewBag.Permissions = db.C04_Permission.ToList();

            return View(userPermiss);
        }

        public ActionResult PermissionEdit(int Id, int idPer)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                C06_UserPermisRelationship userPermiss = db.C06_UserPermisRelationship.FirstOrDefault(s => s.id_user_perm == Id);
                if (userPermiss == null) return RedirectToAction("EditPermission");
                userPermiss.idPer = idPer;

                if (ModelState.IsValid)
                {
                    db.Entry(userPermiss).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("EditPermission");
            }
            catch (Exception ex)
            {
                Session["MemberPermissionError"] = $"Có lỗi khi cập nhật quyền của User do {ex.Message}";
                return View();
            }

        }



        #endregion

    }
}
