using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Controllers.Subject;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using NPOI.SS.Formula.Functions;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Entity.Map;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace Aisger.Controllers.Audit
{
    public class AuditReestrController : AGestController
    {
        //
        // GET: /EscoReestr/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var repository = new SecUserRepository();
            var list = repository.GetQuery(s => s.SEC_UserKind.Any(suk => suk.KindId == CodeConstManager.KIND_USER_AUDIT)).OrderByDescending(s => s.CreateDate);
            
            return View(list);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Create(string bin)
        {
            var user = new AccountRepository().GetUserByBin(bin);
            var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_AUDIT);
            FillBagRegistrationGuest(model);
            return View(model);
        }
        
        [HttpPost]
        public ActionResult Create(SEC_Guest model)
        {
            RemoveManadatoryFields();
            FillBagRegistrationGuest(model);
           
            if (ModelState.IsValid)
            {
                var repository = new SecUserRepository();
                if (model.Village == 0)
                {
                    model.Village = null;
                }
                if (model.SubRegion == 0)
                {
                    model.SubRegion = null;
                }
                model.Kinds = new List<string>();
                model.Kinds.Add(CodeConstManager.KIND_USER_AUDIT.ToString(CultureInfo.InvariantCulture));
                repository.RegisteredUser(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(model);

        }
        public ActionResult Delete(long id)
        {
            new SecUserRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            SEC_User user = new SecUserRepository().GetById(id);

            var model = new AccountController().SecGuest(user,CodeConstManager.CODE_USER_AUDIT);
            FillBagRegistrationGuest(model);
            return View("Create", model);
        }

        public ActionResult ShowDetails(long id, bool isCheck = false)
        {
            SEC_User user = new SecUserRepository().GetById(id);
            var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_AUDIT);
            FillBagRegistrationGuest(model);
            ViewBag.IsCheck = isCheck;
            return View(model);
        }

        public ActionResult Accept(long id)
        {
            var repository = new SecUserRepository();
            SEC_User user = repository.GetById(id);
            //var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_AUDIT);

            if (!user.IsChecked)
            {
                EAuditStatusRepository statuses = new EAuditStatusRepository();

                var status = statuses.GetQuery(s => s.Code == "checked").FirstOrDefault();
                if (status != null)
                {
                    EAUDIT_AuditorReestr checkedAuditor = new EAUDIT_AuditorReestr()
                    {
                        Id = user.Id,
                        refStatus = status.Id
                    };
                    repository.SaveAuditorStatus(checkedAuditor);
                }    
            }
            return RedirectToAction("Index");
        }

        public ActionResult RequiredAudits()
        {
            var repository = new SecUserRepository();
            var min4Year = DateTime.Now.AddYears(-3);
            var min5Year = DateTime.Now.AddYears(-4);
            // get all who should pass audit in this year
            var list = repository.GetQuery(a => a.SEC_UserKind.Any(suk => suk.KindId == CodeConstManager.KIND_USER_SUBJECT)
                && (a.EAUDIT_Preamble.All(p => p.CreateDate < min4Year)
                    || a.CreateDate < min5Year)).OrderByDescending(p => p.Id);
            // var listList = list.ToList();
            return View(list);
        }

		public ActionResult RequiredAudits2()
		{
			return View();
		}

		public ActionResult GetRstReportReestr(string oblast_ids,string name_idk_bin,int pageNum = 1,int year=2016,int calcDeep=1)
		{
			RObjectClass<RST_ReportReestrClass> item = new RObjectClass<RST_ReportReestrClass>();

			List<RST_ReportReestrClass> ListItems = new List<RST_ReportReestrClass>();
			var ErrorMessage = new RstReportRepository().GetRstReportReestr(ref ListItems, pageNum, year, calcDeep, oblast_ids, name_idk_bin);
			if (ErrorMessage == "")
			{
				item.ListItems = ListItems;
				item.Count = ListItems.Count;

				int allcount = 0;
				ErrorMessage = new RstReportRepository().GetRstReportReestrPageCount(ref allcount,year,calcDeep,oblast_ids);
				item.AllCount = allcount;
			}

			item.ErrorMessage = ErrorMessage;
			return this.Json(item, JsonRequestBehavior.AllowGet);
		}

		//----
		public ActionResult ExportExcel(string oblast_ids, int year = 2016, int calcDeep = 1)
		{
			#region excell create begin
			ExcelPackage pck = new ExcelPackage();
			var ws = pck.Workbook.Worksheets.Add("Отчет");
			ws.Column(1).Width = 15;
			ws.Column(2).Width = 30;
			ws.Column(3).Width = 50;
			ws.Column(4).Width = 30;
			ws.Column(5).Width = 30;
			ws.Column(6).Width = 30;
			ws.Column(7).Width = 30;
			ws.Column(8).Width = 30;
			ws.Column(9).Width = 30;

			int rowCount = 1;
			int cellCount = 7;

			#region fill header
			ws.Cells[1, 1, 1, 1].Value = ResourceSetting.IDK;
			ws.Cells[1, 1, 1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 2, 1, 2].Value = ResourceSetting.BININ;
			ws.Cells[1, 2, 1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 3, 1, 3].Value = ResourceSetting.Name;
			ws.Cells[1, 3, 1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 4, 1, 4].Value = ResourceSetting.boss;
			ws.Cells[1, 4, 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 5, 1, 5].Value = ResourceSetting.ResponceFIO;
			ws.Cells[1, 5, 1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 6, 1, 6].Value = ResourceSetting.Oblast;
			ws.Cells[1, 6, 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 7, 1, 7].Value = ResourceSetting.Address;
			ws.Cells[1, 7, 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 1, 1,7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			ws.Cells[1, 1, 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells[1, 1, 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells[1, 1, 1, 7].Style.Font.Bold = true;

			string ErrorMessage = "";
			#region fill type resources
			List<RST_ReportReestrClass> ListItems = new List<RST_ReportReestrClass>();
			ErrorMessage = new RstReportRepository().GetRstReportReestr(ref ListItems, 0,year,calcDeep,oblast_ids,"");

			if (ErrorMessage == "")
			{
				int rowIndex = 0;
				rowCount = rowCount + ListItems.Count;
				foreach (var item in ListItems)
				{
					ws.Cells[2 + rowIndex, 1].Value = item.IDK;
					ws.Cells[2 + rowIndex, 2].Value = item.BINIIN;
					ws.Cells[2 + rowIndex, 3].Value = item.OwnerName;
					ws.Cells[2 + rowIndex, 4].Value = CheckIsEmptyOrWhiteSpace(item.usrlastname) + " " + CheckIsEmptyOrWhiteSpace(item.usrfirstname) + " " + CheckIsEmptyOrWhiteSpace(item.usrsecondname);
					ws.Cells[2 + rowIndex, 5].Value = item.usrresponcefio;
					ws.Cells[2 + rowIndex, 6].Value = item.OblastName;
					ws.Cells[2 + rowIndex, 7].Value = item.Address;
					rowIndex++;
				}
			}
			#endregion

			#endregion


			#region fill border
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < cellCount; j++)
				{
					ws.Cells[1 + i, 1 + j].Style.Border.BorderAround(
						ExcelBorderStyle.Thin);
				}
			}
			#endregion

			#endregion

			FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.FileDownloadName = "Потребление энергоресурсов, полученные не из собственных источников.xlsx";
			return result;
		}

		private string CheckIsEmptyOrWhiteSpace(string term)
		{
			if (string.IsNullOrWhiteSpace(term))
			{
				term = "";
			}
			return term;
		}
    }
}