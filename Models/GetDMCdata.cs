using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DMCTimesheet.Models;
using Microsoft.Ajax.Utilities;

namespace DMCTimesheet.Models
{
    public static class GetDMCdata
    {
        public static readonly dmcDbcontext db = new dmcDbcontext();
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
        /// Lọc thành viên trong dự án
        /// </summary>
        /// <param name="projectId">ProjectId</param>
        /// <returns>List[UserId]</returns>
        public static List<int?> DanhSachThanhVienTheoDuAn(int projectId)
        {
            List<int?> users = new List<int?>();
            try
            {
                //List<int?> _users = new List<int?>();
                C03_ProjectMembers project = db.C03_ProjectMembers.FirstOrDefault(s => s.ProjectID == projectId);
                if (project != null)
                {
                    if (project.ChuTriChinh != null) users.Add(project.ChuTriChinh);
                    if (project.ChuTriKienTruc != null) users.Add(project.ChuTriKienTruc);
                    if (project.ChuTriKetCau != null) users.Add(project.ChuTriKetCau);
                    if (project.ChuTriMEP != null) users.Add(project.ChuTriMEP);
                    if (project.LegalManager != null) users.Add(project.LegalManager);
                    var thanhvienkhac = project.ThanhVienKhac.Split(';');
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
        /// Lấy danh sách UserId dựa trên danh sách
        /// </summary>
        /// <param name="usernameList"></param>
        /// <returns></returns>
        public static List<int> GetUserIdList(string usernameList)
        {
            List<int> users = new List<int>();
            try
            {
                char sp = usernameList.Contains(';') ? ';':',' ;
                                
                var listname = usernameList.Split(sp);
                foreach (var item in listname) {
                    int uId = db.C02_Members.First(s => s.FullName == item).UserID;
                    users.Append(uId); 
                }
                return users;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}