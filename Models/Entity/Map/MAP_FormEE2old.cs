using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
    public class MAP_FormEE2old
    {
        public long Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public long SecUserId { get; set; }
        public int FSCode { get; set; }
        public Nullable<int> DicEEStatusId { get; set; }
        public Nullable<double> TotalArea { get; set; }
        public Nullable<double> NumberOfStoreys { get; set; }
        public string Comments { get; set; }
    }
}