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
    
    public partial class COLLECTOR_DIC_CmdeviceTypes
    {
        public COLLECTOR_DIC_CmdeviceTypes()
        {
            this.COLLECTOR_Cmdevice = new HashSet<COLLECTOR_Cmdevice>();
        }
    
        public long Id { get; set; }
        public string Code { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
    
        public virtual ICollection<COLLECTOR_Cmdevice> COLLECTOR_Cmdevice { get; set; }
    }
}
