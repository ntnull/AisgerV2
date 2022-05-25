using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
    public class MAP_DicObject
    {
        public int Id { get; set; }
        public string NAME { get; set; }
        public bool IsDeleted { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
    }
}