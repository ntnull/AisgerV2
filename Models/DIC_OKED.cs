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
    
    public partial class DIC_OKED
    {
        public DIC_OKED()
        {
            this.DIC_OKED1 = new HashSet<DIC_OKED>();
            this.ESCO_DIC_ProductKindOked = new HashSet<ESCO_DIC_ProductKindOked>();
            this.RST_ReportReestr = new HashSet<RST_ReportReestr>();
            this.SEC_User = new HashSet<SEC_User>();
            this.SEC_UserOked = new HashSet<SEC_UserOked>();
            this.rst_reportreestroked = new HashSet<rst_reportreestroked>();
        }
    
        public long Id { get; set; }
        public Nullable<long> refParent { get; set; }
        public string Code { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> RootId { get; set; }
    
        public virtual ICollection<DIC_OKED> DIC_OKED1 { get; set; }
        public virtual DIC_OKED DIC_OKED2 { get; set; }
        public virtual ICollection<ESCO_DIC_ProductKindOked> ESCO_DIC_ProductKindOked { get; set; }
        public virtual ICollection<RST_ReportReestr> RST_ReportReestr { get; set; }
        public virtual ICollection<SEC_User> SEC_User { get; set; }
        public virtual ICollection<SEC_UserOked> SEC_UserOked { get; set; }
        public virtual ICollection<rst_reportreestroked> rst_reportreestroked { get; set; }
    }
}
