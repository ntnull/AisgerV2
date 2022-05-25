using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.ControlModels
{
    public class SelectLinkListModel
    {
        public string CurrentLink { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
    }
}