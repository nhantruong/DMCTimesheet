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
    
    public partial class C06_UserPermisRelationship
    {
        public int id_user_perm { get; set; }
        public Nullable<int> idUser { get; set; }
        public Nullable<int> idPer { get; set; }
        public Nullable<bool> actived { get; set; }
    
        public virtual C04_Permission C04_Permission { get; set; }
        public virtual C02_Members C02_Members { get; set; }
    }
}
