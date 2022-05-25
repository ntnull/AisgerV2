using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Entity;
using Aisger.Models.Entity.Security;
using Aisger.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Aisger.Models
{
     [MetadataType(typeof(RstReportMetaData))]

    public partial class RST_Report : IObject
    {
         public int CountNewReestr
         {
             get { return RST_ReportReestr.Count(e => e.StatusId == CodeConstManager.NEW_STATUS_REESTR_ID); }
         }
         public int CountOldReestr
         {
             get { return RST_ReportReestr.Count(e => e.StatusId == CodeConstManager.OLD_STATUS_REESTR_ID); }
         }
         public int CountExclude
         {
             get { return RST_ReportReestr.Count(e => e.StatusId == CodeConstManager.EXTEND_STATUS_REESTR_ID); }
         }
         public string OwnerName
         {
             get
             {
                 if (SEC_User == null)
                 {
                     return null;
                 }
                 return SEC_User.FullName;
             }
         }
         public IEnumerable<RST_TEMP_ReportReestr> RstReportReestrs { get; set; }
    }

    public partial class RST_ReestrReportHistory : IEntity
    {
        public IList<string> AttachFiles { get; set; }

    }

    public class RstReportMetaData
    {
        
    }

    public class RST_ReportFilter
    {
        public int? ReportYear { get; set; }
        public long ReportId { get; set; }
        public string BINIIN { get; set; }
        public string IDK { get; set; }
        public string SubjectName { get; set; }
        public string Adress { get; set; }

        public List<string> Oblasts { get; set; }
        public MultiSelectList OblastList { get; set; }

        public List<string> Statuses { get; set; }
        public MultiSelectList StatusList { get; set; }
        public long? UserId { get; set; }
        public List<string> Reasons { get; set; }
        public MultiSelectList ReasonList { get; set; }

        public List<string> Expacts { get; set; }
        public MultiSelectList ExpactList { get; set; }

        public List<string> SubDicStatuses { get; set; }
        public MultiSelectList SubDicStatusList { get; set; }

        public List<string> FsCodes { get; set; }
        public MultiSelectList FsCodeList { get; set; }

        public IEnumerable<RST_ReportReestrFilter> RstReportReestrs { get; set; }
        public long SortId { get; set; }

        public long? SendId { get; set; }
        public long? ExcludedId { get; set; }

        public string ImportErrorMsg {get; set;}
        public int PageNum { get; set; }
        public int PageCount { get; set; }
        public int Count { get; set; }
    }

  
    public class RST_ReportReestrFilter : RST_ReportReestr
    {
        public DateTime? SendDate { get; set; }
        public string StatusName { get; set; }
        public string ExcludedName { get; set; }
        public string OblastName { get; set; }
        public string ReasonName { get; set; }
        public string ExpectantName { get; set; }
        public string FormStatus { get; set; }
        public string FormStatusCode { get; set; }
        public long? FormId { get; set; }
        public bool IsBack { get; set; }
        public string AuthorName { get; set; }
        public long? Editor { get; set; }
    }

    public class SubjectInfo
    {
        public SEC_Guest SecUser { get; set; }
        public List<SUB_Form> SubForms { get; set; }

        public List<RST_ReportReestr> RstReportReestrs { get; set; }
        public List<RST_ReestrReportHistory> RstReestrHistories { get; set; }
        public List<MAP_Application> MapApplications { get; set; }
    }
}