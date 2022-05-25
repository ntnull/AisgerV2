using Aisger.Models.Repository.Reestr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Dictionary
{
    public class DicYearController : Controller
    {
        // GET: DicYear
        public ActionResult Index()
        {
            var model = new RstReportRepository().GetRstReportCustom();
            return View(model);
        }
        
        public ActionResult SaveIsActive(long id)
        {
            string errorMessage = new RstReportRepository().UpdateRstReportIsActive(id);
            Dictionary<string, object> item = new Dictionary<string, object>();
            item["ErrorMessage"] = errorMessage;
            return Json(item);
        }

        public ActionResult UpdateRstReportIsEditSubjectReport(long id,bool isedit)
        {
            string errorMessage = new RstReportRepository().UpdateRstReportIsEditSubjectReport(id,isedit);
            Dictionary<string, object> item = new Dictionary<string, object>();
            item["ErrorMessage"] = errorMessage;
            return Json(item);
        }

        public ActionResult UpdateRstReportIsCreateSubjectReport(long id, bool iscreate)
        {
            string errorMessage = new RstReportRepository().UpdateRstReportIsCreateSubjectReport(id, iscreate);
            Dictionary<string, object> item = new Dictionary<string, object>();
            item["ErrorMessage"] = errorMessage;
            return Json(item);
        }

        public ActionResult UpdateRstReportIsEditSubjectReportByManager(long id, bool isedit)
        {
            string errorMessage = new RstReportRepository().UpdateRstReportIsEditSubjectReportByManager(id, isedit);
            Dictionary<string, object> item = new Dictionary<string, object>();
            item["ErrorMessage"] = errorMessage;
            return Json(item);
        }

        public ActionResult UpdateRstReportIsStatisticMainPageByManager(long id, bool isedit)
        {
            string errorMessage = new RstReportRepository().UpdateRstReportIsStatisticMainPageByManager(id, isedit);
            Dictionary<string, object> item = new Dictionary<string, object>();
            item["ErrorMessage"] = errorMessage;
            return Json(item);
        }
    }
}