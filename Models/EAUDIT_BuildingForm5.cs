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
    
    public partial class EAUDIT_BuildingForm5
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DesignationAndUnit { get; set; }
        public Nullable<double> ValueCritic { get; set; }
        public Nullable<double> ValueEstimatedProject { get; set; }
        public long refPreamble { get; set; }
        public long refBuilding { get; set; }
        public long refTypeResource { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> editdate { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
        public bool isdeleted { get; set; }
    
        public virtual EAUDIT_Building EAUDIT_Building { get; set; }
        public virtual EAUDIT_Preamble EAUDIT_Preamble { get; set; }
        public virtual EAUDIT_DIC_TypeResource EAUDIT_DIC_TypeResource { get; set; }
    }
}
