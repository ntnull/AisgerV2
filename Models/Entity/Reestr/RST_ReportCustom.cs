using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Reestr
{
    public class RST_ReportCustom
    {
        public long Id { get; set; }
        public Nullable<int> ReportYear { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<bool> isactive { get; set; }
        public Nullable<bool> IsCreateSubjectReport { get; set; }
        public Nullable<bool> IsEditSubjectReport { get; set; }
        public Nullable<bool> IsEditSubjectReportByManager { get; set; }
        public Nullable<bool> IsStatisticMainPage { get; set; }
    }
}