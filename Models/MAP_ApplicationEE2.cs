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
    
    public partial class MAP_ApplicationEE2
    {
        public long Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public int DicDetailsId { get; set; }
        public int DicDetailModelsId { get; set; }
        public Nullable<double> CountOfFixtures { get; set; }
        public Nullable<double> CountOfLamps { get; set; }
        public Nullable<double> Power { get; set; }
        public string CPRA { get; set; }
        public Nullable<double> AggregatePower { get; set; }
        public Nullable<double> AverageTariff { get; set; }
        public string WorkingHours { get; set; }
        public Nullable<double> MaintenanceCosts { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public long SecUserId { get; set; }
        public string Comments { get; set; }
        public string authorlogin { get; set; }
    
        public virtual MAP_DIC_EEDetails MAP_DIC_EEDetails { get; set; }
        public virtual MAP_DIC_EEDetailModels MAP_DIC_EEDetailModels { get; set; }
    }
}