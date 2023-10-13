using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DMCTimesheet.Models;
using Microsoft.Ajax.Utilities;

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
            //C02_Members logUser = Session["UserLogin"] as C02_Members;
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.Members = db.C02_Members.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.Workgroup = db.C19_Workgroup.ToList();
            List<C08_Timesheet> ProjectTimesheet = db.C08_Timesheet.ToList();
            ViewBag.Embedstring = db.C98_EmbedString.OrderByDescending(s => s.Version).FirstOrDefault();
            return View(ProjectTimesheet);
        }

        public ActionResult TimesheetManage()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;

            ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.Workgroup = db.C19_Workgroup.ToList();

            ViewBag.ProjectMember = GetDMCdata.DuAnTheoThanhVienID(logUser.UserID);


            List<string[]> group = new List<string[]>();    //Nhóm công việc theo workgroup
            List<string[]> listIdCV = new List<string[]>(); //List ID công việc để lọc
            foreach (var item in db.C07_WorkType.ToList())
            {
                string[] itm = new string[2];
                itm[0] = item.GroupId.ToString();
                itm[1] = item.WorkName;
                group.Add(itm);

                string[] idw = new string[2];
                idw[0] = item.ID.ToString();
                idw[1] = item.WorkName;
                listIdCV.Add(idw);
            }
            ViewBag.group = group;
            ViewBag.listIdCV = listIdCV;


            List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();

            //Công việc trong ngày
            List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongNgay = new List<DMCTimesheet.Models.C08_Timesheet>();
            foreach (var item in mytimesheet)
            {
                if (DateTime.Today == DateTime.Parse(item.RecordDate.ToString())) _ViecTrongNgay.Add(item);
            }
            //Công việc trong tuần
            List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongTuan = new List<DMCTimesheet.Models.C08_Timesheet>();

            var tuanhienhanh = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            int curYear = DateTime.Today.Year;
            int curMonth = DateTime.Today.Month;

            foreach (var item in mytimesheet)
            {
                DateTime itmDate = DateTime.Parse(item.RecordDate.ToString());
                int itmYear = DateTime.Parse(item.RecordDate.ToString()).Year;
                int itmMonth = DateTime.Parse(item.RecordDate.ToString()).Month;
                var tuanTS = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(itmDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                if (itmYear == curYear && itmMonth == curMonth & tuanTS == tuanhienhanh) _ViecTrongTuan.Add(item);
            }

            List<KeyValuePair<string, List<C08_Timesheet>>> TSbyYear = new List<KeyValuePair<string, List<C08_Timesheet>>>();
            List<KeyValuePair<string, List<C08_Timesheet>>> TSbyYear_Week = new List<KeyValuePair<string, List<C08_Timesheet>>>();

            //Lọc dữ liệu theo Năm và theo Tuần
            foreach (var item in mytimesheet.GroupBy(s => s.RecordDate.Value.Year))
            {
                KeyValuePair<string, List<C08_Timesheet>> tsY = new KeyValuePair<string, List<C08_Timesheet>>(item.Key.ToString(), item.ToList());
                TSbyYear.Add(tsY);//Timesheet theo năm
                var gp = item.ToList().GroupBy(s => new { weekName = GetIso8601WeekOfYear(s.RecordDate.Value) });

                foreach (var itm in gp)
                {
                    string WeekName = $"{item.Key}-W{itm.Key.weekName}";
                    KeyValuePair<string, List<C08_Timesheet>> tsYW = new KeyValuePair<string, List<C08_Timesheet>>(WeekName, itm.ToList<C08_Timesheet>());
                    TSbyYear_Week.Add(tsYW);
                }
            }


            //var _weeklyTS = mytimesheet.GroupBy(s=> new {_Year = s.RecordDate.Value.Year, _Week = GetIso8601WeekOfYear(s.RecordDate.Value) })
            //    .Select(p => new {_Year = p.Key._Year.ToString(), _Week = p.Key._Week.ToString(), ts = p.ToList() })
            //    .AsEnumerable().Select(s=>s.ToExpando());


            ViewBag.ViecTrongNgay = _ViecTrongNgay;
            ViewBag.ViecTrongTuan = _ViecTrongTuan;
            ViewBag.FullMyTimesheet = mytimesheet;
            ViewBag.YearlyTimesheet = TSbyYear;
            ViewBag.WeeklyTimesheet = TSbyYear_Week;

            ViewBag.Testjson = Json(TSbyYear, JsonRequestBehavior.AllowGet);

            //Detail actions
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
            // List<string[]> worktypeId = new List<string[]>();    //Nhóm công việc theo workgroup
            List<string[]> listdetailaction = new List<string[]>(); //List ID công việc để lọc
            foreach (var item in db.C21_DetailAction.ToList())
            {
                string[] itm = new string[2];
                itm[0] = item.WorktypeId.ToString();
                itm[1] = item.DetailAction;
                listdetailaction.Add(itm);
            }
            ViewBag.actiondetailList = listdetailaction;
            return View(mytimesheet);
        }



        // Calculate ISO 8601 week of year
        public static int GetIso8601WeekOfYear(DateTime date)
        {
            // Calculate the week of the year using ISO 8601 definition
            // DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }


        public void GetData()
        {
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            List<string[]> group = new List<string[]>();    //Nhóm công việc theo workgroup
            List<string[]> listIdCV = new List<string[]>(); //List ID công việc để lọc
            foreach (var item in db.C07_WorkType.ToList())
            {
                string[] itm = new string[2];
                itm[0] = item.GroupId.ToString();
                itm[1] = item.WorkName;
                group.Add(itm);

                string[] idw = new string[2];
                idw[0] = item.ID.ToString();
                idw[1] = item.WorkName;
                listIdCV.Add(idw);
            }
            ViewBag.group = group;
            ViewBag.listIdCV = listIdCV;


            List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();

            //Công việc trong ngày
            List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongNgay = new List<DMCTimesheet.Models.C08_Timesheet>();
            foreach (var item in mytimesheet)
            {
                //int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Today);
                if (DateTime.Today == DateTime.Parse(item.RecordDate.ToString()))
                {
                    _ViecTrongNgay.Add(item);
                }
            }
            //Công việc trong tuần
            List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongTuan = new List<DMCTimesheet.Models.C08_Timesheet>();

            DateTime startDate = MakeDate(7, false);

            foreach (var item in mytimesheet)
            {
                int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), startDate);
                if (value <= 7)
                {
                    _ViecTrongTuan.Add(item);
                }
            }
            ViewBag.ViecTrongNgay = _ViecTrongNgay;
            ViewBag.ViecTrongTuan = _ViecTrongTuan;
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
        }

        public DateTime MakeDate(int duration, bool TinhToi)
        {
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            int day = DateTime.Today.Day;

            int returnDate;

            if (TinhToi)
            {
                returnDate = day + duration;
            }
            else
            {
                int a = Math.Abs(day - duration);
                returnDate = a == 0 ? 1 : a;
            }
            return new DateTime(year, month, returnDate);
        }



        public ActionResult MemberTimesheet()
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;

            ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.Workgroup = db.C19_Workgroup.ToList();

            ViewBag.ProjectMember = GetDMCdata.DuAnTheoThanhVienID(logUser.UserID);

            List<string[]> group = new List<string[]>();    //Nhóm công việc theo workgroup
            List<string[]> listIdCV = new List<string[]>(); //List ID công việc để lọc
            foreach (var item in db.C07_WorkType.ToList())
            {
                string[] itm = new string[2];
                itm[0] = item.GroupId.ToString();
                itm[1] = item.WorkName;
                group.Add(itm);

                string[] idw = new string[2];
                idw[0] = item.ID.ToString();
                idw[1] = item.WorkName;
                listIdCV.Add(idw);
            }
            ViewBag.group = group;
            ViewBag.listIdCV = listIdCV;


            List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();

            //Công việc trong ngày
            List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongNgay = new List<DMCTimesheet.Models.C08_Timesheet>();
            foreach (var item in mytimesheet)
            {
                //int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Today);
                if (DateTime.Today == DateTime.Parse(item.RecordDate.ToString()))
                {
                    _ViecTrongNgay.Add(item);
                }
            }
            //Công việc trong tuần
            List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongTuan = new List<DMCTimesheet.Models.C08_Timesheet>();
            DateTime startDate = DateTime.Parse(((DateTime.Today.Day - 7).ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.Today.Year.ToString()));

            foreach (var item in mytimesheet)
            {
                int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), startDate);
                if (value <= 7)
                {
                    _ViecTrongTuan.Add(item);
                }
            }
            ViewBag.ViecTrongNgay = _ViecTrongNgay;
            ViewBag.ViecTrongTuan = _ViecTrongTuan;
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
            return View(mytimesheet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewTS(int? ProjectID, string RecordDate, int? WorkID, string Hour, string OvertimeHour, string Description, int NhomHoatDong)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            try
            {
                C08_Timesheet newEnity = new C08_Timesheet() { };
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTimesheet(int? ProjectID, string RecordDate, int? WorkID, string Hour, string OvertimeHour, string Description, int NhomHoatDong)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");

            C02_Members logUser = Session["UserLogin"] as C02_Members;
            if (NhomHoatDong == 2 && ProjectID == null)
            {
                ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.Workgroup = db.C19_Workgroup.ToList();
                ViewBag.WorkType = db.C07_WorkType.ToList();

                ViewBag.ProjectMember = GetDMCdata.DuAnTheoThanhVienID(logUser.UserID);

                #region GetData

                List<string[]> group = new List<string[]>();    //Nhóm công việc theo workgroup
                List<string[]> listIdCV = new List<string[]>(); //List ID công việc để lọc
                foreach (var item in db.C07_WorkType.ToList())
                {
                    string[] itm = new string[2];
                    itm[0] = item.GroupId.ToString();
                    itm[1] = item.WorkName;
                    group.Add(itm);

                    string[] idw = new string[2];
                    idw[0] = item.ID.ToString();
                    idw[1] = item.WorkName;
                    listIdCV.Add(idw);
                }
                ViewBag.group = group;
                ViewBag.listIdCV = listIdCV;


                List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();

                //Công việc trong ngày
                List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongNgay = new List<DMCTimesheet.Models.C08_Timesheet>();
                foreach (var item in mytimesheet)
                {
                    //int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Today);
                    if (DateTime.Today == DateTime.Parse(item.RecordDate.ToString()))
                    {
                        _ViecTrongNgay.Add(item);
                    }
                }
                //Công việc trong tuần
                List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongTuan = new List<DMCTimesheet.Models.C08_Timesheet>();
                DateTime startDate = MakeDate(7, false);
                foreach (var item in mytimesheet)
                {
                    int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), startDate);
                    if (value <= 7)
                    {
                        _ViecTrongTuan.Add(item);
                    }
                }
                ViewBag.ViecTrongNgay = _ViecTrongNgay;
                ViewBag.ViecTrongTuan = _ViecTrongTuan;
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
                #endregion

                ViewBag.WorkAlert = "Bạn cần có dự án khi chọn Nhóm hoạt động này";
                return View("TimesheetManage", db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList());
            }
            try
            {
                //Kiem tra số giờ thực hiện
                double sogioTrongNgay = 0;
                sogioTrongNgay = GetTimesheetDone(RecordDate, logUser.UserID) + double.Parse(Hour);
                if (sogioTrongNgay > 8)
                {
                    ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
                    ViewBag.Projects = db.C01_Projects.ToList();
                    ViewBag.Workgroup = db.C19_Workgroup.ToList();
                    ViewBag.WorkType = db.C07_WorkType.ToList();

                    ViewBag.ProjectMember = GetDMCdata.DuAnTheoThanhVienID(logUser.UserID);
                    #region GetData

                    List<string[]> group = new List<string[]>();    //Nhóm công việc theo workgroup
                    List<string[]> listIdCV = new List<string[]>(); //List ID công việc để lọc
                    foreach (var item in db.C07_WorkType.ToList())
                    {
                        string[] itm = new string[2];
                        itm[0] = item.GroupId.ToString();
                        itm[1] = item.WorkName;
                        group.Add(itm);

                        string[] idw = new string[2];
                        idw[0] = item.ID.ToString();
                        idw[1] = item.WorkName;
                        listIdCV.Add(idw);
                    }
                    ViewBag.group = group;
                    ViewBag.listIdCV = listIdCV;


                    List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();

                    //Công việc trong ngày
                    List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongNgay = new List<DMCTimesheet.Models.C08_Timesheet>();
                    foreach (var item in mytimesheet)
                    {
                        //int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Today);
                        if (DateTime.Today == DateTime.Parse(item.RecordDate.ToString()))
                        {
                            _ViecTrongNgay.Add(item);
                        }
                    }
                    //Công việc trong tuần
                    List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongTuan = new List<DMCTimesheet.Models.C08_Timesheet>();
                    DateTime startDate = MakeDate(7, false);

                    foreach (var item in mytimesheet)
                    {
                        int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), startDate);
                        if (value <= 7)
                        {
                            _ViecTrongTuan.Add(item);
                        }
                    }
                    ViewBag.ViecTrongNgay = _ViecTrongNgay;
                    ViewBag.ViecTrongTuan = _ViecTrongTuan;
                    ViewBag.DetailAction = db.C21_DetailAction.ToList();
                    #endregion

                    ViewBag.WorkAlert = "Số giờ làm việc trong ngày đã nhiều hơn 8h, cần chọn qua ô tăng ca";
                    return View("TimesheetManage", db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList());
                }
                else
                {
                    DateTime inputDate = DateTime.Parse(RecordDate);
                    DateTime recordDate = DateTime.Compare(inputDate, DateTime.Now) > 0 ? DateTime.Now : inputDate;
                    int Hours = int.Parse(Hour);
                    int _OT = int.Parse(OvertimeHour);

                    C08_Timesheet enity = new C08_Timesheet
                    {
                        MemberID = logUser.UserID,
                        RecordDate = recordDate,
                        ProjectId = ProjectID,
                        WorkType = WorkID ?? 43,
                        Description = Description,
                        Hour = Hours,
                        OT = _OT
                    };
                    using (dmcDbcontext dbcontext = new dmcDbcontext())
                    {
                        dbcontext.C08_Timesheet.Add(enity);
                        dbcontext.SaveChanges();
                    }

                }
                //Session["OverHourError"] = "Lưu thành công";
                ViewBag.WorkAlert = "Lưu thành công";
                return RedirectToAction("TimesheetManage");
            }
            catch (Exception ex)
            {
                ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.Workgroup = db.C19_Workgroup.ToList();
                ViewBag.WorkType = db.C07_WorkType.ToList();

                ViewBag.ProjectMember = GetDMCdata.DuAnTheoThanhVienID(logUser.UserID);
                #region GetData

                List<string[]> group = new List<string[]>();    //Nhóm công việc theo workgroup
                List<string[]> listIdCV = new List<string[]>(); //List ID công việc để lọc
                foreach (var item in db.C07_WorkType.ToList())
                {
                    string[] itm = new string[2];
                    itm[0] = item.GroupId.ToString();
                    itm[1] = item.WorkName;
                    group.Add(itm);

                    string[] idw = new string[2];
                    idw[0] = item.ID.ToString();
                    idw[1] = item.WorkName;
                    listIdCV.Add(idw);
                }
                ViewBag.group = group;
                ViewBag.listIdCV = listIdCV;


                List<C08_Timesheet> mytimesheet = db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList();

                //Công việc trong ngày
                List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongNgay = new List<DMCTimesheet.Models.C08_Timesheet>();

                foreach (var item in mytimesheet)
                {
                    //int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Today);
                    if (DateTime.Today == DateTime.Parse(item.RecordDate.ToString()))
                    {
                        _ViecTrongNgay.Add(item);
                    }
                }
                //Công việc trong tuần
                List<DMCTimesheet.Models.C08_Timesheet> _ViecTrongTuan = new List<DMCTimesheet.Models.C08_Timesheet>();
                DateTime startDate = MakeDate(7, false);

                foreach (var item in mytimesheet)
                {
                    int value = DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), startDate);
                    if (value <= 7) _ViecTrongTuan.Add(item);
                }
                ViewBag.ViecTrongNgay = _ViecTrongNgay;
                ViewBag.ViecTrongTuan = _ViecTrongTuan;
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
                #endregion
                ViewBag.WorkAlert = "Lưu công việc thất bại do lỗi " + ex.Message;
                return View("TimesheetManage", db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList());
            }

        }

        private double GetTimesheetDone(string recordDate, int userID)
        {
            double kq = 0;
            DateTime inputDate = DateTime.Parse(recordDate);
            List<C08_Timesheet> data = db.C08_Timesheet.Where(s => s.RecordDate == inputDate && s.MemberID == userID).ToList();
            if (data == null || data.Count() == 0) return 0;
            try
            {
                foreach (var item in data) if (DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Parse(recordDate)) == 0) kq += item.Hour.Value;
                return kq;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public ActionResult UserEdit(int Id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;

            C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == Id);
            if (item == null)
            {
                ViewBag.WorkAlert = "Không tìm thấy thông tin Timesheet này";
                return View();
            }
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.DetailAction = db.C21_DetailAction.ToList();

            return View(item);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(int Id, DateTime RecordDate, int WorkType, int Hour, int? OT, string Description)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                if (ModelState.IsValid)
                {
                    C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == Id);
                    item.Description = Description;
                    item.OT = OT ?? 0;
                    item.Hour = Hour;
                    item.WorkType = WorkType;
                    item.RecordDate = RecordDate;

                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                // return RedirectToAction("UserPage", "Home");
                return RedirectToAction("TimesheetManage");
            }
            catch (Exception ex)
            {
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.WorkType = db.C07_WorkType.ToList();
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
                ViewBag.WorkAlert = $"Có lỗi khi cập nhật timesheet số {Id} do {ex.Message}";
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserModalEdit(int Id, DateTime RecordDate, string ProjectName, string WorkType, int Hour, int? OvertimeHour, string Description)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                if (ModelState.IsValid)
                {
                    C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == Id);
                    if (item != null)
                    {
                        int _Ot = OvertimeHour ?? 0;
                        C02_Members loginUsr = Session["UserLogin"] as C02_Members;
                        bool ktrasogio = DailyCheck(Id, RecordDate, loginUsr.UserID, Hour, _Ot);

                        if (ktrasogio)
                        {
                            //Đổi dự án
                            switch (ProjectName)
                            {
                                case "Công việc chung":
                                    {
                                        item.ProjectId = null;
                                        break;
                                    }
                                default:
                                    {
                                        int pID = db.C01_Projects.FirstOrDefault(s => s.ProjectOtherName.Contains(ProjectName)).ProjectID;
                                        item.ProjectId = pID;
                                        break;
                                    }

                            }
                            item.Description = Description;

                            item.OT = OvertimeHour ?? 0;
                            item.Hour = Hour;

                            item.WorkType = db.C07_WorkType.FirstOrDefault(s => s.WorkName.Contains(WorkType)).ID;// đổi công việc
                            item.RecordDate = RecordDate;

                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            //LỖI CHỖ NÀY- ĐANG TƯƠNG TÁC TRÊN MODAL
                            ViewBag.WorkAlert = $"Tổng số giờ trong ngày {RecordDate} đã hơn tiêu chuẩn (8h), cần chuyển qua tăng ca Overtime";
                            //return View("TimesheetManage","Timesheet", ViewBag.WorkAlert);
                            return RedirectToAction("TimesheetManage");
                        }
                    }
                }
                // return RedirectToAction("UserPage", "Home");
                return RedirectToAction("TimesheetManage");
            }
            catch (Exception ex)
            {
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.WorkType = db.C07_WorkType.ToList();
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
                ViewBag.WorkAlert = $"Có lỗi khi cập nhật timesheet số {Id} do {ex.Message}";
                return RedirectToAction("TimesheetManage");
            }

        }
        #region Javascript SAVE MODAL TIMESHEET


        public string SaveTS(int Id, DateTime RecordDate, string ProjectName, string WorkType, int Hour, int? OvertimeHour, string Description)
        {
            try
            {
                C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == Id);
                if (item != null)
                {
                    int _Ot = OvertimeHour ?? 0;
                    C02_Members loginUsr = Session["UserLogin"] as C02_Members;
                    bool ktrasogio = DailyCheck(Id, RecordDate, loginUsr.UserID, Hour, _Ot);
                    if (ktrasogio)
                    {
                        //Đổi dự án
                        switch (ProjectName)
                        {
                            case "Công việc chung":
                                {
                                    item.ProjectId = null;
                                    break;
                                }
                            default:
                                {
                                    int pID = db.C01_Projects.FirstOrDefault(s => s.ProjectOtherName.Contains(ProjectName)).ProjectID;
                                    item.ProjectId = pID;
                                    break;
                                }
                        }
                        item.Description = Description;
                        item.OT = OvertimeHour ?? 0;
                        item.Hour = Hour;
                        item.WorkType = db.C07_WorkType.FirstOrDefault(s => s.WorkName.Contains(WorkType)).ID;// đổi công việc
                        item.RecordDate = RecordDate;

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        return $"Cập nhật timesheet Id {Id} thành công";
                    }
                    else
                    {
                        return ($"Tổng số giờ trong ngày {RecordDate} đã hơn tiêu chuẩn (8h), cần chuyển qua tăng ca Overtime");
                    }
                }
                else
                {
                    return $"Không tìm thấy Timesheet này";
                }
            }
            catch (Exception ex)
            {
                return ($"Có lỗi xãy ra do {ex.Message}");
            }

        }

        public JsonResult GetTS()
        {
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            return Json(db.C08_Timesheet.Where(s => s.MemberID == logUser.UserID).ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion
        /// <summary>
        /// Kiểm tra số giờ trong ngày
        /// Nếu true: tổng số giờ trong ngày <=8 => OK/ True
        /// Nếu false: tổng số giờ trong ngày > 8 => not OK/False
        /// </summary>
        /// <param name="recordDate"></param>
        /// <param name="hour"></param>
        /// <param name="overtimeHour"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool DailyCheck(int id, DateTime recordDate, int userId, int hour, int? overtimeHour)
        {
            try
            {
                double total = 0;
                //Lấy các TS theo ngày input
                var DailyTS = db.C08_Timesheet.Where(s => s.RecordDate == recordDate && s.MemberID == userId).ToList();
                switch (DailyTS.Count)
                {
                    case 0: //ko tìm thấy TS
                        {
                            return true;
                        }
                    case 1: //chỉ có 1 record ==> Update Record hiện có nếu Id trùng
                        {
                            if (DailyTS.FirstOrDefault().Id == id)
                            {
                                if (hour > 8)
                                {
                                    return false;
                                }
                                return true;
                            }
                            return false;
                        }
                    default: //Nhiều hơn 1 record
                        {
                            foreach (var item in DailyTS)
                            {
                                if (item.Id != id) // tính tổng số giờ hiện có không bao gồm record edit
                                {
                                    total += (double)item.Hour + (double)item.OT;
                                }
                            }
                            //Nếu số giờ hiện có + edit
                            if ((total + hour + overtimeHour) > 8) return false;
                            else return true;
                        }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }




        // GET: TimeSheet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            C08_Timesheet Timesheet = db.C08_Timesheet.FirstOrDefault(s => s.Id == id);
            if (Timesheet == null) return HttpNotFound();

            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
            return View(Timesheet);
        }


        // POST: TimeSheet/Delete/5
        /// <summary>
        /// Xóa Timesheet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");

            try
            {
                C08_Timesheet c15_TimeSheet = db.C08_Timesheet.FirstOrDefault(s => s.Id == id);
                if (ModelState.IsValid)
                {
                    db.C08_Timesheet.Remove(c15_TimeSheet);
                    db.SaveChanges();
                }
                return RedirectToAction("TimesheetManage");
            }
            catch (Exception ex)
            {
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.WorkType = db.C07_WorkType.ToList();
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
                ViewBag.WorkAlert = $"Xóa Thất bại do {ex.Message}";
                return RedirectToAction("Delete", db.C08_Timesheet.FirstOrDefault(s => s.Id == id));
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
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
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
        public ActionResult CreateWorktype(string WorkName, int GroupId)
        {
            try
            {
                if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
                if (ModelState.IsValid)
                {
                    C07_WorkType item = new C07_WorkType
                    {
                        WorkName = WorkName,
                        GroupId = GroupId
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
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
            return View(enity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWorktype(int Id, string WorkName, int GroupId)
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
                    item.GroupId = GroupId;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
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
                ViewBag.DetailAction = db.C21_DetailAction.ToList();
                return View("WorkType");
            }

        }


        public ActionResult TimeSheetAjaxSearch(int UserSearch)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            var userLogin = Session["UserLogin"] as C02_Members;
            ViewBag.Members = db.C02_Members.ToList();
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.DetailAction = db.C21_DetailAction.ToList();
            if (UserSearch == -1) return PartialView("_TimeSheetAjaxSearch", db.C08_Timesheet.Where(s => s.MemberID == userLogin.UserID).OrderByDescending(s => s.RecordDate).ToList());
            List<C08_Timesheet> data = db.C08_Timesheet.Where(s => s.ProjectId == UserSearch).OrderByDescending(s => s.RecordDate).ToList();
            if (data == null) return PartialView("_TimeSheetAjaxSearch", null);
            return PartialView("_TimeSheetAjaxSearch", data);

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
                return Json(workType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                status = $"Có lỗi {ex.Message}"; ;
                return Json(status, JsonRequestBehavior.AllowGet);
            }

        }


        #endregion

    }
}
