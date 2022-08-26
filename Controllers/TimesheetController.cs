using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
            //C02_Members logUser = Session["UserLogin"] as C02_Members;
            ViewBag.Projects = db.C01_Projects.ToList();
            ViewBag.Members = db.C02_Members.ToList();
            ViewBag.WorkType = db.C07_WorkType.ToList();
            ViewBag.Workgroup = db.C19_Workgroup.ToList();
            List<C08_Timesheet> ProjectTimesheet = db.C08_Timesheet.ToList();

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

            ViewBag.ProjectMember = db.C03_ProjectMembers.Where(
              s => s.ChuTriKienTruc == logUser.UserID
                || s.ChuTriChinh == logUser.UserID
                || s.ChuTriKetCau == logUser.UserID
                || s.ChuTriMEP == logUser.UserID
                || s.LegalManager == logUser.UserID
                || s.Admin == logUser.UserID
                ).ToList();

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

            return View(mytimesheet);
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
                returnDate = Math.Abs(day - duration);
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

            ViewBag.ProjectMember = db.C03_ProjectMembers.Where(
              s => s.ChuTriKienTruc == logUser.UserID
                || s.ChuTriChinh == logUser.UserID
                || s.ChuTriKetCau == logUser.UserID
                || s.ChuTriMEP == logUser.UserID
                || s.LegalManager == logUser.UserID
                || s.Admin == logUser.UserID
                ).ToList();

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

            return View(mytimesheet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTimesheet(int? ProjectID, string RecordDate, int WorkID, string Hour, string OvertimeHour, string Description, int NhomHoatDong)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            C02_Members logUser = Session["UserLogin"] as C02_Members;
            if (NhomHoatDong == 2 && ProjectID == null)
            {
                ViewBag.Members = db.C02_Members.Where(s => s.Deactived == false).ToList();
                ViewBag.Projects = db.C01_Projects.ToList();
                ViewBag.Workgroup = db.C19_Workgroup.ToList();
                ViewBag.WorkType = db.C07_WorkType.ToList();

                ViewBag.ProjectMember = db.C03_ProjectMembers.Where(s => s.ChuTriKienTruc == logUser.UserID
                || s.ChuTriChinh == logUser.UserID
                || s.ChuTriKetCau == logUser.UserID
                || s.ChuTriMEP == logUser.UserID
                || s.LegalManager == logUser.UserID
                || s.Admin == logUser.UserID
                ).ToList();

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

                    ViewBag.ProjectMember = db.C03_ProjectMembers.Where(s => s.ChuTriKienTruc == logUser.UserID
                    || s.ChuTriChinh == logUser.UserID
                    || s.ChuTriKetCau == logUser.UserID
                    || s.ChuTriMEP == logUser.UserID
                    || s.LegalManager == logUser.UserID
                    || s.Admin == logUser.UserID
                    ).ToList();
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
                        WorkType = WorkID,
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

                ViewBag.ProjectMember = db.C03_ProjectMembers.Where(s => s.ChuTriKienTruc == logUser.UserID
                || s.ChuTriChinh == logUser.UserID
                || s.ChuTriKetCau == logUser.UserID
                || s.ChuTriMEP == logUser.UserID
                || s.LegalManager == logUser.UserID
                || s.Admin == logUser.UserID
                ).ToList();
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
                foreach (var item in data)
                {
                    if (DateTime.Compare(DateTime.Parse(item.RecordDate.ToString()), DateTime.Parse(recordDate)) == 0)
                    {
                        kq += item.Hour.Value;

                    }
                }
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


            return View(item);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(int Id, DateTime RecordDate, int WorkType, int Hour, int OT, string Description)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            try
            {
                if (ModelState.IsValid)
                {
                    C08_Timesheet item = db.C08_Timesheet.FirstOrDefault(s => s.Id == Id);
                    item.Description = Description;
                    item.OT = OT;
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

                ViewBag.WorkAlert = $"Có lỗi khi cập nhật timesheet số {Id} do {ex.Message}";
                return View();
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


        public ActionResult TimeSheetAjaxSearch(int UserSearch)
        {
            if (Session["UserLogin"] == null) return RedirectToAction("Login", "Home");
            var userLogin = Session["UserLogin"] as C02_Members;
            ViewBag.Members = db.C02_Members.ToList();
            ViewBag.Projects = db.C01_Projects.ToList();

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
