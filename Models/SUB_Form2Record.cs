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
    
    public partial class SUB_Form2Record
    {
        public long Id { get; set; }
        public Nullable<long> FormId { get; set; }
        public Nullable<long> TypeResourceId { get; set; }
        public Nullable<double> ExtractVolume { get; set; }
        public Nullable<double> NotOwnSource { get; set; }
        public Nullable<double> LosEnergy { get; set; }
        public Nullable<double> OwnSource { get; set; }
        public Nullable<double> TransferOtherLegal { get; set; }
        public Nullable<double> ExpenceEnergy { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> editdate { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
        public bool isdeleted { get; set; }
    
        public virtual SUB_DIC_TypeResource SUB_DIC_TypeResource { get; set; }
        public virtual SUB_Form SUB_Form { get; set; }
    }
}
