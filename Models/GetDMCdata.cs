using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DMCTimesheet.Models;

namespace DMCTimesheet.Models
{
    public static class GetDMCdata
    {
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



    }
}