//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DMCTimesheet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class C08_Timesheet
    {
        public int Id { get; set; }
        public Nullable<int> MemberID { get; set; }
        public string ProjectId { get; set; }
        public Nullable<int> WorkType { get; set; }
        public Nullable<double> Hour { get; set; }
        public Nullable<double> OT { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> RecordDate { get; set; }
    
        public virtual C02_Members C02_Members { get; set; }
        public virtual C07_WorkType C07_WorkType { get; set; }
        public virtual C01_Projects C01_Projects { get; set; }
    }
}
