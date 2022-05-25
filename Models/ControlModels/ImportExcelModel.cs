using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.ControlModels
{
    public class ImportExcelModel
    {
        public long ObjectId { get; set; }
        public int Year { get; set; }

        public HttpPostedFileBase FileContent { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
    }
}