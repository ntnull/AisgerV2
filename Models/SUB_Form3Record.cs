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
    
    public partial class SUB_Form3Record
    {
        public long Id { get; set; }
        public Nullable<long> FormId { get; set; }
        public Nullable<long> KindResourceId { get; set; }
        public Nullable<double> ConsumptionVolume { get; set; }
        public Nullable<double> LosTransportVolume { get; set; }
        public string Note { get; set; }
        public Nullable<double> ConsumptionPrice { get; set; }
        public Nullable<double> LosTransportPrice { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> editdate { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
        public bool isdeleted { get; set; }
    
        public virtual SUB_DIC_KindResource SUB_DIC_KindResource { get; set; }
        public virtual SUB_Form SUB_Form { get; set; }
    }
}
