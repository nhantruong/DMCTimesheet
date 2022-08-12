using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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