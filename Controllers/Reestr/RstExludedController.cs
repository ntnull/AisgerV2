using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Reestr;
using Aisger.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Reestr
{
    public class RstExludedController : ACommonController
    {
        //
        // GET: /RstExluded/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View(new RstReestrRepository().GetCollectionList().Where(e => e.StatusId == CodeConstManager.EXTEND_STATUS_REESTR_ID));
  
        }
        [GerNavigateLogger]
        public ActionResult CommonView(int? year, string status, string expact, string idk, string biniin, string adress, string owner, string oblast, long? sortId)
        {
            var filter = new RST_ExecutedFilter();
            filter.ReportYear = year;
            filter.UserId = MyExtensions.GetCurrentUserId();
            var order = new RstReportRepository().GetExecutedByReportYear(year);
            if (order != null)
            {
                filter.NumberOrder = order.NumberOrder;
                filter.DateOrder = order.DateOrder;
                filter.Note = order.Note;
            } if (!string.IsNullOrEmpty(status))
            {
                filter.Statuses = new List<string>();
                var statuslList = status.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Statuses.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(expact))
            {
                filter.Expacts = new List<string>();
                var expactlList = expact.Split(',');
                foreach (var s in expactlList)
                {
                    filter.Expacts.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(biniin))
            {
                filter.BINIIN = biniin;
            }
            if (!string.IsNullOrEmpty(idk))
            {
                filter.IDK = idk;
            }
            if (!string.IsNullOrEmpty(adress))
            {
                filter.Adress = adress;
            }
            if (!string.IsNullOrEmpty(owner))
            {
                filter.SubjectName = owner;
            }
            if (!string.IsNullOrEmpty(oblast))
            {
                filter.Oblasts = new List<string>();
                var statuslList = oblast.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Oblasts.Add(s);
                }
            }
            if (sortId == null || sortId == 0)
            {
                filter.SortId = 1;
            }
            else
            {
                filter.SortId = sortId.Value;
            }
            FillCommonViewBag(filter);
            filter.RstReportReestrs = new RstReportRepository().GetExpactedCommonReestrsByFilter(filter);

            // FillViewFiltertBag(filter);
            return View(filter);
        }
        [GerNavigateLogger]
        public ActionResult ExecuteCommand(int? year)
        {
            var filter = new RST_ExecutedFilter();
            filter.ReportYear = year;
            filter.UserId = MyExtensions.GetCurrentUserId();
           
            var repository = new RstReportRepository();
            repository.UpdateExecuted(filter);
            var order = new RstReportRepository().GetExecutedByReportYear(year);
            if (order != null)
            {
                filter.NumberOrder = order.NumberOrder;
                filter.DateOrder = order.DateOrder;
                filter.Note = order.Note;
            }
            FillCommonViewBag(filter);
            filter.RstReportReestrs = repository.GetExpactedCommonReestrsByFilter(filter);

            // FillViewFiltertBag(filter);
            return View("CommonView",filter);
        }
        [HttpPost]
        public virtual ActionResult UpdateOrderInfo(int reportYear, long userId, string fieldName, string fieldValue)
        {

           var result = new RstReportRepository().UpdateOrderInfo(reportYear,userId, fieldName, fieldValue);

           return Json(new { Success = result });

        }
        public ActionResult ExportExcel(int? year)
        {

            var filter = new RST_ExecutedFilter();
            filter.ReportYear = year;
           
            FillCommonViewBag(filter);
            filter.RstReportReestrs = new RstReportRepository().GetExpactedCommonReestrsByFilter(filter);

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Отчет");
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 50;
            ws.Column(3).Width = 70;
            ws.Column(4).Width = 40;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 60;
            ws.Cells["A1"].Value = ResourceSetting.biniinSubject;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["B1"].Value = ResourceSetting.IDK;
            ws.Cells["B1"].Style.Font.Bold = true;
            ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["C1"].Value = ResourceSetting.SubPerson;
            ws.Cells["C1"].Style.Font.Bold = true;
            ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["D1"].Value = ResourceSetting.SendDate;
            ws.Cells["D1"].Style.Font.Bold = true;
            ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["E1"].Value = ResourceSetting.RegisterForm;
            ws.Cells["E1"].Style.Font.Bold = true;
            ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["F1"].Value = ResourceSetting.ExpectantName;
            ws.Cells["F1"].Style.Font.Bold = true;
            ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["G1"].Value = ResourceSetting.Oblast;
            ws.Cells["G1"].Style.Font.Bold = true;
            ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["H1"].Value = ResourceSetting.RstDicStatus;
            ws.Cells["H1"].Style.Font.Bold = true;
            ws.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["I1"].Value = ResourceSetting.ReportReason;
            ws.Cells["I1"].Style.Font.Bold = true;
            ws.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            var reportReestrFilters = filter.RstReportReestrs as RST_ReportReestrFilter[] ??
                                      filter.RstReportReestrs.ToArray();
            for (var i = 0; i < reportReestrFilters.Count(); i++)
            {
                var index = i + 2;
                var reestr = reportReestrFilters[i];
                ws.Cells["A" + index].Value = reestr.BINIIN;
                ws.Cells["B" + index].Value = reestr.IDK;
                ws.Cells["B" + index].Style.WrapText = true;
                ws.Cells["C" + index].Value = reestr.OwnerName + '/' + reestr.Address;
                ws.Cells["C" + index].Style.WrapText = true;
                if (reestr.SendDate != null)
                {
                    ws.Cells["D" + index].Value = reestr.SendDate.Value.ToShortDateString();
                }
                else
                {
                    ws.Cells["D" + index].Value = "";
                }
                ws.Cells["D" + index].Style.WrapText = true;

                ws.Cells["E" + index].Value = reestr.FormStatus;
                ws.Cells["E" + index].Style.WrapText = true;
                ws.Cells["F" + index].Value = reestr.ExpectantName;
                ws.Cells["F" + index].Style.WrapText = true;
                ws.Cells["G" + index].Value = reestr.Oblast;
                ws.Cells["G" + index].Style.WrapText = true;
                ws.Cells["H" + index].Value = reestr.StatusName;
                ws.Cells["H" + index].Style.WrapText = true;
                ws.Cells["I" + index].Value = reestr.ReasonName;
                ws.Cells["I" + index].Style.WrapText = true;
            }

            FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = "report.xlsx";
            return result;
        }
        [HttpPost]
        public virtual ActionResult UpdateExpact(long recordId, long? fieldValue)
        {

            new RstReportRepository().UpdateExpact(recordId, fieldValue, MyExtensions.GetCurrentUserId());

            return Json(new { Success = true });

        }

        [HttpPost]
        public virtual ActionResult UpdateState(long recordId, long? fieldValue)
        {

            new RstReportRepository().UpdateState(recordId, fieldValue, MyExtensions.GetCurrentUserId());

            return Json(new { Success = true });

        }
        private void FillCommonViewBag(RST_ExecutedFilter filter)
        {
            filter.StatusList = GetStatus(filter.Statuses);
            filter.ExpactList = GetExpact(filter.Expacts);
            filter.OblastList = GetOblasList(filter.Oblasts);
            var listanimal = new RstReportRepository().GetYears();
            if (filter.ReportYear == null)
            {
                filter.ReportYear = (int?)listanimal.Max(e => e.ID);
            }
            ViewData["ExpactList"] = new RstDicReasonRepository().GetAll().Where(e => e.IsExcluded);
            ViewData["StateList"] = new RstDicStatusRepository().GetAll();
            ViewData["Years"] = new SelectList(listanimal, "ID",
                                                 "NAME_RU", filter.ReportYear);
            var sortList = new List<UnMappedDictionary>();
            sortList.Add(new UnMappedDictionary(CodeConstManager.SORT_INDEX_DATESEND, CultureHelper.GetDictionaryName(CodeConstManager.SORT_NAME_DATESEND)));
            sortList.Add(new UnMappedDictionary(CodeConstManager.SORT_INDEX_DATEEDIT, CultureHelper.GetDictionaryName(CodeConstManager.SORT_NAME_DATEEDIT)));
            ViewData["SortList"] = new SelectList(sortList, "ID",
                                                 "NAME_RU", filter.SortId);
        }
        public MultiSelectList GetExpact(IList<string> selectedValues)
        {
            var plants = new RstDicReasonRepository().GetAll().Where(e => e.IsExcluded);
            return new MultiSelectList(plants, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }
        public MultiSelectList GetOblasList(IList<string> selectedValues)
        {
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            return new MultiSelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }
        public MultiSelectList GetStatus(IList<string> selectedValues)
        {
            var plants = new List<RST_DIC_Status>();
            plants.Add(new RST_DIC_Status { Id = 1, NameRu = ResourceSetting.Expands });
            plants.Add(new RST_DIC_Status { Id = 2, NameRu = ResourceSetting.Excluds });
            plants.Add(new RST_DIC_Status{Id=3,NameRu = ResourceSetting.all});
            return new MultiSelectList(plants, "Id", "NameRu", selectedValues);
        }
      
    }
}
