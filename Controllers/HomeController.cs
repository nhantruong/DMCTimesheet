using DMCTimesheet.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
//using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class HomeController : Controller
    {
        private readonly dmcDbcontext db = new dmcDbcontext();
        readonly string conn = "server = 103.27.60.66; user id=dmcAdmin;password=DmcNewVision@2022#; persistsecurityinfo = True; database =cbimtech_dmc";


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
            //return RedirectToAction("Login");
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
                    ViewBag.AssignMemProjects = db.C03_ProjectMembers.ToList();
                    ViewBag.Projects = db.C01_Projects.ToList();
                    ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();





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
                    //Nhân sự
                    //ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).GetEnumerator();
                    IEnumerable<C02_Members> member =  GetDMCdata.GetDMCMembers();;/* = new IEnumerable<C02_Members>(); List<C02_Members>();*/
                   
                    ViewBag.Members = member;



                   //ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
                    //Dự án
                    ViewBag.Projects = GetDMCdata.GetDMCProjects();
                    
                    //ViewBag.Projects = db.C01_Projects.ToList();
                    #region Char 1 - Loại hình dự án - 
                    //Loại hình dự án
                    ViewBag.ProjectType = db.C13_ProjectType.ToList();

                    List<string> ProjectTypeName = new List<string>();
                    List<double> ProjectTypeCountTS = new List<double>();
                    var CountPTts = db.C01_Projects.GroupBy(s => s.ProjectTypeId);
                    foreach (var item in CountPTts)
                    {
                        if (item.Key != null)
                        {
                            ProjectTypeName.Add(db.C13_ProjectType.FirstOrDefault(s => s.TypeId == item.Key).TypeName);
                            ProjectTypeCountTS.Add(item.Count());
                        }
                        else
                        {
                            ProjectTypeName.Add("N/A");
                            ProjectTypeCountTS.Add(item.Count());
                        }

                    }
                    ViewBag.ProjectTypeName = ProjectTypeName;
                    ViewBag.ProjectTypeCountTS = ProjectTypeCountTS;
                    #endregion

                    #region Chart 2 Đếm dự án theo Giai đoạn
                    //Giai đoạn dự án
                    ViewBag.Stage = db.C20_Stage.ToList();

                    List<string> ProjectStageName = new List<string>();
                    List<double> ProjectStageCount = new List<double>();
                    var CountPjStage = db.C01_Projects.GroupBy(s => s.ProjectStage);
                    foreach (var item in CountPjStage)
                    {
                        if (item.Key != null)
                        {
                            ProjectStageName.Add(db.C20_Stage.FirstOrDefault(s => s.Id == item.Key).StageName);
                            ProjectStageCount.Add(item.Count());
                        }
                        else
                        {
                            ProjectStageName.Add("N/A");
                            ProjectStageCount.Add(item.Count());
                        }

                    }
                    ViewBag.ProjectStageName = ProjectStageName;
                    ViewBag.ProjectStageCount = ProjectStageCount;

                    #endregion

                    #region Chart 3 - Đếm dự án theo Tình trạng
                    //Tình trạng dự án
                    ViewBag.Status = db.C16_Status.ToList();

                    List<string> ProjectStatusName = new List<string>();
                    List<double> ProjectStatusCount = new List<double>();
                    var CountPjStatus = db.C01_Projects.GroupBy(s => s.ProjectStatusId);
                    foreach (var item in CountPjStatus)
                    {
                        if (item.Key != null)
                        {
                            ProjectStatusName.Add(db.C16_Status.FirstOrDefault(s => s.Id == item.Key).StatusName);
                            ProjectStatusCount.Add(item.Count());
                        }
                        else
                        {
                            ProjectStatusName.Add("N/A");
                            ProjectStatusCount.Add(item.Count());
                        }

                    }
                    ViewBag.ProjectStatusName = ProjectStatusName;
                    ViewBag.ProjectStatusCount = ProjectStatusCount;

                    #endregion

                    #region Chart 4 - Timesheet
                    ViewBag.Timesheet = db.C08_Timesheet.ToList();

                    #endregion

                    #region Chart 5 - Timesheet theo nhóm công việc
                    //Nhóm công việc
                    ViewBag.WorkGroup = db.C19_Workgroup.ToList();
                    ViewBag.WorkType = db.C07_WorkType.ToList();

                    List<string> workgroupName = new List<string>();
                    List<double> workgroupCountTS = new List<double>();
                    var countTS = db.C19_Workgroup.Include("C07_WorkType").Include("C08_Timesheet").GroupBy(s => s.GroupName).ToList();
                    List<KeyValuePair<string, double>> returnTSbyGroup = new List<KeyValuePair<string, double>>();
                    foreach (var item in countTS)
                    {
                        double? hot = 0;
                        foreach (var itm in item.ToList())
                        {
                            var ts = itm.C07_WorkType.Select(s => s.C08_Timesheet).ToList();
                            if (ts.Count > 0)
                            {
                                foreach (var it in ts)
                                {
                                    hot += it.AsEnumerable().Sum(s => s.Hour);
                                    hot += it.AsEnumerable().Sum(s => s.OT) * 1.5;
                                }
                            }
                            workgroupName.Add(item.Key);
                            workgroupCountTS.Add(double.Parse(hot.ToString()));
                        }
                        KeyValuePair<string, double> valuePair = new KeyValuePair<string, double>(item.Key, double.Parse(hot.ToString()));
                        returnTSbyGroup.Add(valuePair);

                    }
                    ViewBag.WorkGroupName = workgroupName;
                    ViewBag.WorkGroupCountTS = workgroupCountTS;
                    #endregion

                    #region Chart 6 & Table 1 - Thống kê dự án theo nhân sự - 
                    //Điều phối nhân sự trong dự án
                    ViewBag.AssignMemProjects = db.C03_ProjectMembers.ToList();
                    #endregion




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