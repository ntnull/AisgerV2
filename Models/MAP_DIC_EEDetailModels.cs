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
    
    public partial class MAP_DIC_EEDetailModels
    {
        public MAP_DIC_EEDetailModels()
        {
            this.MAP_ApplicationEE2 = new HashSet<MAP_ApplicationEE2>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public int DICEEDetailsId { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
    
        public virtual ICollection<MAP_ApplicationEE2> MAP_ApplicationEE2 { get; set; }
        public virtual MAP_DIC_EEDetails MAP_DIC_EEDetails { get; set; }
    }
}
