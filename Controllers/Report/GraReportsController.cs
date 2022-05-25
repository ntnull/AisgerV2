using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Report
{
	public class GraReportsController : ACommonController
    {

        //---- ReestrReport
        [GerNavigateLogger]
        public ActionResult ReestrReport()
        {
            return View();
        }

        //----
        [GerNavigateLogger]
        public ActionResult GerReport()
		{
			return View();
		}

        //----Отчетные формы по Энергоэффективность 2.0
        [GerNavigateLogger]
        public ActionResult EE2Report()
		{
			return View();
		}

        //---- Энергоаудит
        [GerNavigateLogger]
        public ActionResult AuditReport()
		{
			return View();
		}

        //----Карта ЭЭ и ЭСКО
        [GerNavigateLogger]
        public ActionResult MapEnergyEscoReport()
		{
			return View();
		}

		public JsonResult getDicRstReport()
		{
			string errorMessage = "";
			var item = new Dictionary<string, object>();
			try
			{
				var years = new RstReportRepository().GetRstReport();
				item["ListItems"] = years;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			item["ErrorMessage"] = errorMessage;

			return Json(item);
		}

        public JsonResult GetOKED(string lang)
        {
            return Json(new RstReportRepository().GetOKED(lang));
        }

    }
}