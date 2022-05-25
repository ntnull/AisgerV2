using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
    public class MAP_DIC_EEDetailsold
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}