using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DMCTimesheet.com.cbimtech.MemberServices;
using DMCTimesheet.Models;
using Microsoft.Ajax.Utilities;

namespace DMCTimesheet.Models
{
    public static class GetDMCdata
    {
        public static readonly dmcDbcontext db = new dmcDbcontext();

        #region DMC

        public static IEnumerable<C02_Members> GetDMCMembers()
        {
            using (dmcDbcontext db = new dmcDbcontext())
            {
                try
                {
                    List<C02_Members> members = new List<C02_Members>();
                    members = db.C02_Members.Where(s => s.Deactived == false).ToList();
                    return members;
                }
                catch (Exception)
                {
                    return Enumerable.Empty<C02_Members>();
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<C01_Projects> GetDMCProjects()
        {
            using (dmcDbcontext db = new dmcDbcontext())
            {
                try
                {
                    List<C01_Projects> project = new List<C01_Projects>();
                    project = db.C01_Projects.ToList();
                    return project;
                }
                catch (Exception)
                {
                    return Enumerable.Empty<C01_Projects>();
                }
            }
        }

        /// <summary>
        /// Tính toán dự án từng thành viên, trả về danh sách KeyValue - UserID-List<Project> 
        /// </summary>
        /// <returns>KeyValue - UserID-List[ProjectId]</returns>
        public static List<KeyValuePair<int, List<int?>>> DuAnCacThanhVien()
        {
            List<KeyValuePair<int, List<int?>>> ProjectOfMember = new List<KeyValuePair<int, List<int?>>>();
            try
            {
                foreach (C02_Members thanhvien in db.C02_Members.ToList())
                {
                    if (thanhvien.Deactived == true || thanhvien.SystemMember == true) continue;
                    KeyValuePair<int, List<int?>> duanTungThanhVien = DuAnTheoThanhVien(thanhvien.UserID);
                    ProjectOfMember.Add(duanTungThanhVien);
                }
                return ProjectOfMember;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// danh sách dự án theo ID thành viên
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>KeyValuePair{userID,List[ProjectId]}</returns>
        public static KeyValuePair<int, List<int?>> DuAnTheoThanhVien(int userId)
        {
            KeyValuePair<int, List<int?>> _ProjectOfMember = new KeyValuePair<int, List<int?>>();
            try
            {
                foreach (C02_Members thanhvien in db.C02_Members.ToList())
                {
                    if (thanhvien.Deactived != true && thanhvien.UserID == userId)
                    {
                        List<int?> DsachDuan = new List<int?>();
                        foreach (C03_ProjectMembers duan in db.C03_ProjectMembers.ToList())
                        {
                            if (db.C01_Projects.FirstOrDefault(s => s.ProjectID == duan.ProjectID && s.ProjectStage == 5) != null) continue; //Kiểm tra dự án đã kết thúc chưa
                                                                                                                                             //Vai trò khác                                                                                                                                                                                                                            
                            bool otherRole = false;
                            if (string.IsNullOrEmpty(duan.ThanhVienKhac) || string.IsNullOrWhiteSpace(duan.ThanhVienKhac)) { continue; }//Kiểm tra có thành viên khác
                            var other = duan.ThanhVienKhac.Split(',').ToList();//Nếu có, tách lấy từng thành viên ID 
                            for (int i = 0; i < other.Count; i++)//Mỗi thành viênID trong tập hợp dc tách ra
                            {
                                int uId = int.Parse(other[i]);// convert sang int
                                if (thanhvien.UserID == uId)
                                {
                                    otherRole = true;
                                    continue;
                                } // nếu thành viênID trùng với ID của UserID                              
                            }
                            //Vai trò Chính
                            if (duan.ChuTriChinh == thanhvien.UserID
                                || duan.ChuTriKienTruc == thanhvien.UserID
                                || duan.ChuTriKetCau == thanhvien.UserID
                                || duan.ChuTriMEP == thanhvien.UserID
                                || duan.LegalManager == thanhvien.UserID) DsachDuan.Add(duan.ProjectID);
                            else if (otherRole) DsachDuan.Add(duan.ProjectID);
                        }
                        KeyValuePair<int, List<int?>> DuanTungThanhVien = new KeyValuePair<int, List<int?>>(thanhvien.UserID, DsachDuan);
                        _ProjectOfMember = DuanTungThanhVien;
                    }
                }
                return _ProjectOfMember;
            }
            catch (Exception)
            {
                return new KeyValuePair<int, List<int?>>(userId, null);
            }

        }

        /// <summary>
        /// Danh sách Dự án của thành viên
        /// </summary>
        /// <param name="UserId">ID của thành viên</param>
        /// <returns>C01_Project</returns>
        public static List<C01_Projects> DuAnTheoThanhVienID(int UserId)
        {
            List<C01_Projects> myProject = new List<C01_Projects>();
            try
            {
                KeyValuePair<int, List<int?>> _myProject = DuAnTheoThanhVien(UserId);
                foreach (var item in _myProject.Value.ToList())
                {

                    C01_Projects myProject2 = db.C01_Projects.FirstOrDefault(s => s.ProjectID == item);
                    myProject.Add(myProject2);
                }
                return myProject;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Lấy danh sách các thành viên tham gia trong dự án - ProjectId
        /// </summary>
        /// <param name="projectId">ProjectId</param>
        /// <returns>List[UserId]</returns>
        public static List<int?> DanhSachUserIDTheoDuAn(int projectId)
        {
            List<int?> users = new List<int?>();
            try
            {
                //List<int?> _users = new List<int?>();
                C03_ProjectMembers project = db.C03_ProjectMembers.FirstOrDefault(s => s.ID == projectId);
                if (project != null)
                {
                    if (project.ChuTriChinh != null) users.Add(project.ChuTriChinh);
                    if (project.ChuTriKienTruc != null) users.Add(project.ChuTriKienTruc);
                    if (project.ChuTriKetCau != null) users.Add(project.ChuTriKetCau);
                    if (project.ChuTriMEP != null) users.Add(project.ChuTriMEP);
                    if (project.LegalManager != null) users.Add(project.LegalManager);
                    var thanhvienkhac = project.ThanhVienKhac.Split(',');
                    foreach (var item in thanhvienkhac) if (!string.IsNullOrEmpty(item)) if (users.Any(s => s != int.Parse(item))) users.Add(int.Parse(item));

                    return users.Distinct().ToList();
                }
                else { return null; }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Danh sách Users - C02_member trong dự án
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List[C02_Member]</returns>
        public static List<C02_Members> DanhSachC02MemberTheoDuAn(int projectId)
        {
            List<C02_Members> users = new List<C02_Members>();
            try
            {
                C03_ProjectMembers project = db.C03_ProjectMembers.FirstOrDefault(s => s.ProjectID == projectId);
                if (project != null)
                {
                    if (project.ChuTriChinh != null) users.Add(db.C02_Members.FirstOrDefault(s=>s.UserID == project.ChuTriChinh));
                    if (project.ChuTriKienTruc != null) users.Add(db.C02_Members.FirstOrDefault(s => s.UserID == project.ChuTriKienTruc));
                    if (project.ChuTriKetCau != null) users.Add(db.C02_Members.FirstOrDefault(s => s.UserID == project.ChuTriKetCau));
                    if (project.ChuTriMEP != null) users.Add(db.C02_Members.FirstOrDefault(s => s.UserID == project.ChuTriMEP));
                    if (project.LegalManager != null) users.Add(db.C02_Members.FirstOrDefault(s => s.UserID == project.LegalManager));

                    var thanhvienkhac = project.ThanhVienKhac.Split(',');
                    for (int i = 0; i < thanhvienkhac.Length; i++)
                    {
                        int uId = int.Parse(thanhvienkhac[i]);
                        if (!string.IsNullOrEmpty(thanhvienkhac[i])) if (db.C02_Members.Any(s => s.UserID != uId)) users.Add(db.C02_Members.FirstOrDefault(p => p.UserID == uId));
                    }
                    //foreach (var item in thanhvienkhac) if (!string.IsNullOrEmpty(item)) if (db.C02_Members.Any(s=>s.UserID != int.Parse(item))) users.Add(db.C02_Members.FirstOrDefault(p=>p.UserID == int.Parse(item)));

                    return users.Distinct().ToList();
                }
                else { return null; }
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Lấy danh sách UserId dựa trên danh sách Fullname từng người
        /// </summary>
        /// <param name="usernameList">string-Danh sách Fullname từng người</param>
        /// <returns>List[UserID]</returns>
        public static List<int> GetUserIdList(string usernameList)
        {
            List<int> userlist;
            string _usernameList = "";
            try
            {
                if (usernameList.Contains(';'))
                {
                    _usernameList = usernameList.Replace(";", ",");
                }
                else { _usernameList = usernameList; }
                //char sp = usernameList.Contains(';') ? ';' : ',';
                //char sp = _usernameList.Contains(';') ? ';' : ',';
                //var listname = usernameList.Split(sp);
                var listname = _usernameList.Split(',');
                userlist = new List<int>();
                foreach (var item in listname)
                {
                    string itm = item.Trim();
                    if (!string.IsNullOrEmpty(itm))
                    {                        
                        int uId = db.C02_Members.First(s => s.FullName == itm).UserID;
                        userlist.Add(uId);
                    }

                }
                return userlist;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region BIM team
        public static List<MemberOutput> GetBIMmemberList()
        {
            List<MemberOutput> BIMer = new List<MemberOutput>();
            try
            {
                using (MemberService memberService = new MemberService())
                {
                    BIMer.AddRange(memberService.DanhSachMember().ToList());
                }
                return BIMer;
            }
            catch (Exception)
            {
                return null;
            }


        }
        #endregion

    }
}