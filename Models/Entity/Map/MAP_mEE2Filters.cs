using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Entity.Map
{
    public class MAP_mEE2Filters
    {
        public string BINIIN { get; set; }
        public string Name { get; set; }
        public List<string> Oblasts { get; set; }
        public MultiSelectList OblastList { get; set; }
        public IEnumerable<MAP_FormEE2Manager> ListItems { get; set; }

        public List<string> Statuses { get; set; }
        public MultiSelectList StatusList { get; set; }

		public List<string> Okeds { get; set; }
		public MultiSelectList OkedList { get; set; }
    }
}