using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Entity.Map
{
    public partial class MAP_EE2Filters :MAP_FormEE2
    {
        public IEnumerable<MAP_ApplicationEE2Info> MapApplicationEE2List { get; set; }
        public List<SelectListItem> DicTypeApplicationList { get; set; }
        public List<SelectListItem> FormEE2RecordList { get; set; }
        public List<SelectListItem> DicEEStatusList { get; set; }

        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }
    }

}