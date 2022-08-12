using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DMCTimesheet.Models;
using System.Security.Cryptography;

namespace DMCTimesheet.Controllers
{
    public class HomeController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();
        string conn = "server = 103.27.60.66; user id=dmcAdmin;password=DmcNewVision@2022#; persistsecurityinfo = True; database =cbimtech_dmc";


        public ActionResult Index()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            C06_UserPermisRelationship userper = db.C06_UserPermisRelationship.FirstOrDefault(s => s.idUser == logUser.UserID);

            switch (userper.idPer)
            {
                case 1: //WebAdmin
                    {
                        Session["UserLogin"] = logUser;
                        Session["UserPermission"] = userper;
                        return RedirectToAction("WebAdminPage", "Home");
                    }
                case 2: //WebUser
                    {
                        Session["UserLogin"] = logUser;
                        Session["UserPermission"] = userper;
                        return RedirectToAction("UserPage", "Home");
                    }
                case 3: //SysAdmin
                    {
                        Session["UserLogin"] = logUser;
                        Session["UserPermission"] = userper;
                        return RedirectToAction("SysAdminPage", "Home");
                    }
                case 4: //SysMod
                    {
                        Session["UserLogin"] = logUser;
                        Session["UserPermission"] = userper;
                        return RedirectToAction("SysModPage", "Home");
                    }
                default: return RedirectToAction("UserPage");

            }
            return RedirectToAction("Login");
        }
        #region Login

        /// <summary>
        /// Trang Đăng Nhập
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// Dùng Login chính
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SubmitLogin(string Username, string Password)
        {
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    ViewBag.LoginNote = "Nhập username hoặc Password";
                    return RedirectToAction("Login");
                }
                else
                {
                    //dmcDbcontext db2 = new dmcDbcontext();
                    //db2.Database.Connection.Open();
                    try
                    {
                        string SercurityPass = MaHoaPass(Password);
                        db.Database.Connection.ConnectionString = conn;
                        db.Database.Connection.Open();

                        C02_Members logUser = db.C02_Members.FirstOrDefault(s => s.UserName == Username.Trim());
                        if (logUser == null || logUser.Deactived == true)
                        {
                            ViewBag.LoginNote = "User không tồn tại, vui lòng liên hệ admin";
                            return View("Login");
                        }
                        if (logUser.Pass == SercurityPass)
                        {
                            C06_UserPermisRelationship userper = db.C06_UserPermisRelationship.FirstOrDefault(s => s.idUser == logUser.UserID);
                            if (userper.actived == false)
                            {
                                ViewBag.LoginNote = "Không tìm thấy User này trên Hệ thống";
                                return View();
                            }
                            if (userper == null)
                            {
                                ViewBag.LoginNote = $"Có lỗi khi kết nối server hoặc User chưa được set quyền, vui lòng liên hệ admin";
                                return View();
                            }

                            //ViewBag.LogUserPermission = userper.idPer;

                            switch (userper.idPer)
                            {
                                case 1: //WebAdmin
                                    {
                                        Session["UserLogin"] = logUser;
                                        Session["UserPermission"] = userper;
                                        return RedirectToAction("WebAdminPage", "Home");
                                    }
                                case 2: //WebUser
                                    {
                                        Session["UserLogin"] = logUser;
                                        Session["UserPermission"] = userper;
                                        return RedirectToAction("UserPage", "Home");
                                    }
                                case 3: //SysAdmin
                                    {
                                        Session["UserLogin"] = logUser;
                                        Session["UserPermission"] = userper;
                                        return RedirectToAction("SysAdminPage", "Home");
                                    }
                                case 4: //SysMod
                                    {
                                        Session["UserLogin"] = logUser;
                                        Session["UserPermission"] = userper;
                                        return RedirectToAction("SysModPage", "Home");
                                    }
                                default: return RedirectToAction("UserPage");
                            }

                        }
                        else
                        {
                            ViewBag.LoginNote = "Sai Password";
                            ViewBag.UserName = Username;
                            return View("Login");
                        }

                    }
                    catch (Exception ex)
                    {
                        ViewBag.LoginNote = $"Có lỗi khi kết nối server do {ex.Message}";
                        return View("Login");
                    }

                }
            }
            catch (Exception ex)
            {
                ViewBag.LoginNote = $"Có lỗi do {ex.Message}";
                return View("Login");
            }
        }

        /// <summary>
        /// Mã hóa password
        /// </summary>
        /// <param name="password">Chuỗi cần mã hóa</param>
        /// <returns>Chuỗi Pass đã mã hóa</returns>
        public static string MaHoaPass(string password)
        {
            MD5 mD5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            byte[] hash = mD5.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login");
        }

        public ActionResult ForgotPass(string UserName, string newPass, string ConfirmPass)
        {
            if (newPass.Trim() != ConfirmPass.Trim() || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(ConfirmPass))
            {
                ViewBag.LoginNote = "Kiểm tra Confirm password";
                return View("Login");
            }
            else
            {
                try
                {
                    using (dmcDbcontext context = new dmcDbcontext())
                    {
                        context.Database.Connection.ConnectionString = conn;
                        var mem = context.C02_Members.Where(s => s.UserName.ToLower() == UserName.ToLower()).FirstOrDefault();
                        if (mem != null && mem.Deactived == false)
                        {
                            mem.Pass = MaHoaPass(ConfirmPass);
                            context.Entry(mem).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            ViewBag.LoginNote = "Cập nhật password thành công!";
                            Session["UserLogin"] = mem;
                        }
                    }
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ViewBag.LoginNote = $"Có lỗi khi tạo Password mới do {ex.Message}";
                    return RedirectToAction("Login");
                }
            }
        }

        public static string LoaiBoDauTiengViet(string noiDung)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = noiDung.Normalize(NormalizationForm.FormD).Trim();
            return regex.Replace(temp, String.Empty)
                        .Replace('\u0111', 'd')
                        .Replace('\u0110', 'D')
                        .Replace(",", "_")
                        .Replace(".", "_")
                        .Replace("!", "")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace(";", "_")
                        .Replace("/", "_")
                        .Replace("%", "ptram")
                        .Replace("&", "va")
                        .Replace("?", "")
                        .Replace('"', '_')
                        .Replace(' ', '_');
        }
        #endregion

        #region SysAdmin - System Administrator

        /// <summary>
        /// Quyền quản trị cao nhất
        /// </summary>
        /// <returns></returns>
        public ActionResult SysAdminPage()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    if (!(Session["UserLogin"] is C02_Members logUser) || logUser.Deactived == true)
                    {
                        ViewBag.LoginNote = "User không tồn tại, vui lòng liên hệ admin";
                        return View("Login");
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.LoginNote = $"Có lỗi khi kết nối server do {ex.Message}";
                    return View("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }






        #endregion

        #region SysMod - System Moderator
        /// <summary>
        /// Quản lý hệ thống
        /// </summary>
        /// <returns></returns>
        public ActionResult SysModPage()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    if (!(Session["UserLogin"] is C02_Members logUser) || logUser.Deactived == true)
                    {
                        ViewBag.LoginNote = "User không tồn tại, vui lòng liên hệ admin";
                        return View("Login");
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.LoginNote = $"Có lỗi khi kết nối server do {ex.Message}";
                    return View("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        #endregion

        #region Admin - Website Admin

        public ActionResult WebAdminPage()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    if (!(Session["UserLogin"] is C02_Members logUser) || logUser.Deactived == true)
                    {
                        ViewBag.LoginNote = "User không tồn tại, vui lòng liên hệ admin";
                        return View("Login");
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.LoginNote = $"Có lỗi khi kết nối server do {ex.Message}";
                    return View("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion

        #region User - Website typical user
        public ActionResult UserPage()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    if (!(Session["UserLogin"] is C02_Members logUser) || logUser.Deactived == true)
                    {
                        ViewBag.LoginNote = "User không tồn tại, vui lòng liên hệ admin";
                        return View("Login");
                    }
                    List<int?> projectsAssigned = db.C03_ProjectMembers.Where(
                        s => s.ChuTriKienTruc == logUser.UserID 
                        || s.ChuTriChinh == logUser.UserID 
                        || s.ChuTriKetCau == logUser.UserID 
                        || s.ChuTriMEP == logUser.UserID 
                        || s.LegalManager == logUser.UserID
                        || s.Admin == logUser.UserID
                        ).Select(p => p.ProjectID).ToList();
                    List<C01_Projects> myProjects = new List<C01_Projects>();
                    foreach (var item in projectsAssigned)
                    {
                        myProjects.AddRange(db.C01_Projects.Where(s => s.ProjectID == item).ToList());
                    }

                    //Lấy danh sách dự án theo User ID - đang chạy
                    //ViewBag.MyProjects = CollectModelData.GetProjectsByUserId(logUser.UserID).Where(s=>s.ProjectStatusId == 1);
                    ViewBag.MyProjects = myProjects;
                    ViewBag.AllProject = db.C01_Projects.ToList();
                    ViewBag.AssignedProjects = db.C03_ProjectMembers.ToList();

                    //Lấy danh sách timesheet đã làm
                    ViewBag.MyWorks = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).OrderByDescending(p => p.RecordDate).ToList();
                    ViewBag.WorkType = db.C07_WorkType.ToList();

                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.LoginNote = $"Có lỗi khi kết nối server do {ex.Message}";
                    return View("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion



        #region About

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        #endregion




    }
}