using Aisger.Models;
using Aisger.Models.Repository.Report;
using Aisger.Models.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Report
{
   public class ReportAskuerCaseController : ACommonController
    {
        //
        // GET: /ReportShowCase/
        ReportAnalyseRepository bi = new ReportAnalyseRepository();

        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult COLLECTOR_Cmdevice()
		{

			string ErrorMessage = "";
			Dictionary<string, object> item = new Dictionary<string, object>();
			try
			{
				aisgerEntities AppContext = new aisgerEntities();
				List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

				var _object = AppContext.COLLECTOR_Cmdevice.Where(x => x.IsDeleted == false).ToList();
				foreach (var o in _object)
				{
					list.Add(new Dictionary<string, object>());
					list.Last()["id"] = Convert.ToString(o.Id);
					list.Last()["device_name"] = o.NameRu;
				}

				item["ListItems"] = list;
				item["ErrorMessage"] = "";
				//var checkSubForm = AppContext.Database.SqlQuery<SUB_Form>(check_query).ToList();
			}
			catch (Exception ex)
			{
				item["ListItems"] = null;
				item["ErrorMessage"] = ex.Message;
			}

			return Json(item);
		}

		public ActionResult GetDetailing()
		{
			string ErrorMessage = "";
			Dictionary<string, object> item = new Dictionary<string, object>();
			try
			{
				List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

				list.Add(new Dictionary<string, object>());
				list.Last()["id"] = 1;
				list.Last()["name"] = "За день";
				list.Add(new Dictionary<string, object>());
				list.Last()["id"] = 2;
				list.Last()["name"] = "За месяц";
				

				item["ListItems"] = list;
				item["ErrorMessage"] = "";
			}
			catch (Exception ex)
			{
				item["ListItems"] = null;
				item["ErrorMessage"] = ex.Message;
			}

			return Json(item);
		}

		//---- 
		public JsonResult GetViewAskuerDay(int device_id = 1)
		{
			var item = bi.viewAskuerDay(device_id);
			return Json(item);
		}

		//---- 
		public JsonResult GetViewAskuerMonth(int device_id = 1)
		{
			var item = bi.viewAskuerMonth(device_id);
			return Json(item);
		}

    }
}
