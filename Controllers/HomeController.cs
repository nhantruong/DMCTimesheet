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
            if (Session["UserLogin"] != null)
            {
                db.Database.Connection.ConnectionString = conn;
                ViewBag.Projectlist = db.C01_Projects.ToList();
                ViewBag.Permission = db.C04_Permission.ToList();
                ViewBag.Members = db.C02_Members.ToList();
                ViewBag.UserPermRelate = db.C06_UserPermisRelationship.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
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
                            switch (userper.idPer)
                            {
                                case 1: //WebAdmin
                                    {
                                        Session["UserLogin"] = logUser;
                                        return RedirectToAction("WebAdmin", "Home");
                                    }
                                case 2: //WebUser
                                    {
                                        Session["UserLogin"] = logUser;
                                        return RedirectToAction("UserLogin", "Home");
                                    }
                                case 3: //SysAdmin
                                    {
                                        Session["UserLogin"] = logUser;
                                        return RedirectToAction("SysAdmin", "Home");
                                    }
                                case 4: //SysMod
                                    {
                                        Session["UserLogin"] = logUser;
                                        return RedirectToAction("SysMod", "Home");
                                    }
                                default: return RedirectToAction("UserLogin");
                            }

                        }
                        else
                        {
                            ViewBag.LoginNote = "Sai Password";
                            ViewBag.UserName = Username;
                            return RedirectToAction("Login");
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
                return RedirectToAction("Login");
            }
        }

        private string MaHoaPass(string password)
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
                            mem.Pass = ConfirmPass;
                            context.Entry(mem).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            ViewBag.LoginNote = "Cập nhật password thành công!";
                            Session["UserLogin"] = mem;
                        }
                    }
                    return RedirectToAction("Index");
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

        #region SysAdmin
        public ActionResult SysAdmin()
        {
            if (Session["UserLogin"] != null)
            {
                try
                {
                    db.Database.Connection.ConnectionString = conn;
                    C02_Members logUser = Session["UserLogin"] as C02_Members;
                    C06_UserPermisRelationship memberPermis = db.C06_UserPermisRelationship
                        .Include("C05_PermissionDetail")
                        .Include("C04_Permission")
                        .FirstOrDefault(s => s.idUser == logUser.UserID);            

                    ViewBag.Userpermis = memberPermis.C04_Permission.C05_PermissionDetail.ToList();

                    //Session["UserPermission"] = UserPers;
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.LoginNote = $"Có lỗi khi kết nối server do {ex.Message}";
                    return RedirectToAction("Login");
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