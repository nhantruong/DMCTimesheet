using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using DMCTimesheet.Models;

namespace DMCTimesheet.Controllers
{
    public class CollectModelData
    {
        private static readonly dmcDbcontext db = new dmcDbcontext();
        static string conn = "server = 103.27.60.66; user id=dmcAdmin;password=DmcNewVision@2022#; persistsecurityinfo = True; database =cbimtech_dmc";

        public static List<C01_Projects> GetProjectsByUserId(int userId)
        {
            //if (!DbOpenConnection()) return null;
            //if (userId == null) return null;

            //Lấy danh sách dự án theo User ID
            List<int?> projectsAssigned = new List<int?>();
            projectsAssigned = db.C03_ProjectMembers.Where(
                s => s.ChuTriKienTruc == userId
                || s.ChuTriChinh == userId
                || s.ChuTriKetCau == userId
            || s.ChuTriMEP == userId
            || s.LegalManager == userId
            || s.Admin == userId
            ).Select(p => p.ProjectID).ToList();
            List<C01_Projects> myProjects = new List<C01_Projects>();
            foreach (var item in projectsAssigned)
            {
                myProjects.AddRange(db.C01_Projects.Where(s => s.ProjectID == item && s.ProjectStatusId == 1).ToList());
            }
            return myProjects;

        }

        public static List<C08_Timesheet> GetTimesheetByUserId(int? userId)
        {
            //Lấy danh sách timesheet đã làm
            if (!DbOpenConnection()) return null;
            if (userId == null) return null;

            List<C08_Timesheet> myWorks = db.C08_Timesheet.Where(s => s.MemberID == userId).OrderByDescending(p => p.RecordDate).ToList();
            return myWorks;
        }

        /// <summary>
        /// Lấy thông tin dự án được chỉ định theo tất cả của User
        /// </summary>
        /// <param name="enity">danh sách Thành viên trong dự án</param>
        /// <returns></returns>
        public static List<KeyValuePair<int, List<int?>>> GetAssignedProjects(List<C03_ProjectMembers> enity)
        {
            List<KeyValuePair<int, List<int?>>> ProjectOfMember = new List<KeyValuePair<int, List<int?>>>();//UserId + ProjectId?
            try
            {
                //1. Get all active members
                List<C02_Members> members = db.C02_Members.Where(s => s.Deactived == false).ToList();

                foreach (var member in members)
                {
                    int key = member.UserID;
                    List<int?> assignedMainRole = enity.Where(s => s.ChuTriChinh == key
                        || s.ChuTriKienTruc == key
                        || s.ChuTriKienTruc2 == key
                        || s.ChuTriKetCau == key
                        || s.ChuTriKetCau2 == key
                        || s.ChuTriMEP == key
                        || s.ChuTriMEP2 == key
                        || s.Admin == key
                        || s.LegalManager2 == key
                        || s.LegalManager == key).Select(p => p.ProjectID).ToList(); ;

                    List<int?> subRole = new List<int?>();

                    foreach (var project in enity)
                    {
                        if (!string.IsNullOrEmpty(project.ThanhVienKhac))
                        {
                            var set = project.ThanhVienKhac.Split(',');
                            for (int i = 0; i < set.Length; i++)
                            {
                                if (int.Parse(set[i]) == key) { subRole.Add(project.ProjectID); break; }
                            }
                        }
                    }

                    List<int?> myproject = new List<int?>();
                    myproject = subRole != null? assignedMainRole.Concat(subRole).ToList(): assignedMainRole;
                    ProjectOfMember.Add(new KeyValuePair<int, List<int?>>(key, myproject));

                }
                return ProjectOfMember;
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// danh sách dự án theo ID user cần tìm
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enity"></param>
        /// <returns></returns>
        public static List<KeyValuePair<int, List<int?>>> GetAssignedProjectsByUserId(int userId, List<C03_ProjectMembers> enity)
        {
            List<KeyValuePair<int, List<int?>>> ProjectOfMember = new List<KeyValuePair<int, List<int?>>>();//UserId + ProjectId?
            try
            {
                int key = userId;
                List<int?> assignedMainRole = enity.Where(s => s.ChuTriChinh == key
                    || s.ChuTriKienTruc == key
                    || s.ChuTriKienTruc2 == key
                    || s.ChuTriKetCau == key
                    || s.ChuTriKetCau2 == key
                    || s.ChuTriMEP == key
                    || s.ChuTriMEP2 == key
                    || s.Admin == key
                    || s.LegalManager2 == key
                    || s.LegalManager == key).Select(p => p.ProjectID).ToList();

                List<int?> subRole = new List<int?>();

                foreach (var project in enity)
                {
                    if (!string.IsNullOrEmpty(project.ThanhVienKhac))
                    {
                        var set = project.ThanhVienKhac.Split(',');
                        for (int i = 0; i < set.Length; i++)
                        {
                            if (int.Parse(set[i]) == key) { subRole.Add(project.ProjectID); break; }
                        }
                    }
                }

                List<int?> myproject = new List<int?>();
                myproject = subRole != null ? assignedMainRole.Concat(subRole).ToList(): assignedMainRole;
                ProjectOfMember.Add(new KeyValuePair<int, List<int?>>(key, myproject));


                return ProjectOfMember;
            }
            catch (Exception)
            {

                return null;
            }
        }


        static bool DbOpenConnection()
        {
            try
            {
                db.Database.Connection.ConnectionString = conn;
                db.Database.Connection.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}