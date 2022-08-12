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
    
    public partial class C09_ProjectSubCon
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ArcId { get; set; }
        public Nullable<int> InteriorId { get; set; }
        public Nullable<int> StructureId { get; set; }
        public Nullable<int> MEPid { get; set; }
        public Nullable<int> LandscapeId { get; set; }
        public Nullable<int> LegalId { get; set; }
    
        public virtual C01_Projects C01_Projects { get; set; }
        public virtual C12_SubContractor C12_SubContractor { get; set; }
    }
}
