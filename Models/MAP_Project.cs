//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aisger.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MAP_Project
    {
        public long Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> ApplicationId { get; set; }
        public Nullable<long> EscoId { get; set; }
        public Nullable<long> Edtor { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public string SourceBudject { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
    
        public virtual MAP_Application MAP_Application { get; set; }
        public virtual SEC_User SEC_User { get; set; }
        public virtual SEC_User SEC_User1 { get; set; }
    }
}
