using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Constants;
using Aisger.Models.ControlModels;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Schema;
using NPOI.HSSF.Record.Chart;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Aisger.Models.Entity.Subject;
using Aspose.Words;
using Aspose.Words.Tables;
using Aspose.Words.Replacing;
using QRCoder;
using System.Drawing;
using Aisger.Models.Entity.Dictionary;
using Newtonsoft.Json;
using Aisger.Models.Entity.Reestr;

namespace Aisger.Controllers.Reestr
{
    public class RstReportController : ACommonController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string _actionName = filterContext.ActionDescriptor.ActionName;

            if (MyExtensions.GetRolesId() == 4 && _actionName.IndexOf("Export") == -1 && _actionName.IndexOf("Download") == -1)
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }
            //base.OnActionExecuting(filterContext);
        }

        [GerNavigateLogger]
        public ActionResult Index()
        {
            var list = new RstReportRepository().GetCollectionList();
            return View(list);
        }

        [GerNavigateLogger]
        public ActionResult EvadersCommand(int? year)
        {
            var repository = new RstReportRepository();
            var filter = new RST_ReportFilter();
            filter.ReportYear = year;
            filter.UserId = MyExtensions.GetCurrentUserId();
            FillCommonViewBag(filter);

            repository.UpdateEvaders(filter);
            filter.RstReportReestrs = repository.GetCommonReestrsByFilter(filter);
            // FillViewFiltertBag(filter);
            return View("CommonView", filter);
        }

        [GerNavigateLogger]
        public ActionResult CommonView(int? year, string idk, string biniin, string adress, string owner, string status, string reason, string oblast, string expact, string subDicStatus, string fsCode, long? sortId, long? sendId, long? excludedId, string msg, int pageNum = 1, int pageCount = 0)
        {
            var filter = new RST_ReportFilter();
            filter.ReportYear = year;
            filter.SendId = sendId;
            filter.ExcludedId = excludedId;
            if (pageNum == 0)
                pageNum = 1;

            filter.PageNum = pageNum;
            filter.PageCount = pageCount;


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
            if (!string.IsNullOrEmpty(status))
            {
                filter.Statuses = new List<string>();
                var statuslList = status.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Statuses.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(reason))
            {
                filter.Reasons = new List<string>();
                var statuslList = reason.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Reasons.Add(s);
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
            if (!string.IsNullOrEmpty(oblast))
            {
                filter.Oblasts = new List<string>();
                var statuslList = oblast.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Oblasts.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(subDicStatus))
            {
                filter.SubDicStatuses = new List<string>();
                var statuslList = subDicStatus.Split(',');
                foreach (var s in statuslList)
                {
                    filter.SubDicStatuses.Add(s);
                }
            }

            if (!string.IsNullOrEmpty(fsCode))
            {
                filter.FsCodes = new List<string>();
                var fsCodeList = fsCode.Split(',');
                foreach (var s in fsCodeList)
                {
                    filter.FsCodes.Add(s);
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
            try
            {
                filter.RstReportReestrs = new RstReportRepository().GetCommonReestrsByFilter(filter);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            filter.ImportErrorMsg = msg;

            return View(filter);
        }

        public ActionResult GetPageCountByYear(int ReportYear)
        {
            RST_ReportFilter filter = new RST_ReportFilter();
            filter.ReportYear = ReportYear;
            IEnumerable<RST_ReportReestrFilter> result = new RstReportRepository().GetCommonReestrsByFilter(filter);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["AllCount"] = result.ToList().Count;

            return Json(dict);
        }

        private void FillCommonViewBag(RST_ReportFilter filter)
        {
            filter.StatusList = GetStatus(filter.Statuses);
            filter.ReasonList = GetReason(filter.Reasons);
            filter.ExpactList = GetExpact(filter.Expacts);
            filter.OblastList = GetOblasList(filter.Oblasts);
            filter.FsCodeList = GetFsCode(filter.FsCodes);

            filter.SubDicStatusList = GetSubDicStatus(filter.SubDicStatuses);
            var listanimal = new RstReportRepository().GetYears();
            if (filter.ReportYear == null)
            {
                filter.ReportYear = (int?)listanimal.Max(e => e.ID);
            }

            ViewData["ReportYear"] = filter.ReportYear;
            ViewData["ReasonList"] = filter.ReasonList;
            ViewData["FsCodeList"] = filter.FsCodeList;
            ViewData["FsCodeList2"] = GetFsCode2(filter.FsCodes);
            ViewData["ExpactList"] = new RstDicReasonRepository().GetAll().Where(e => e.IsExcluded || e.Id == CodeConstManager.STATUS_EVADERS_ID);
            ViewData["Years"] = new SelectList(listanimal, "ID",
                                                 "NAME_RU", filter.ReportYear);
            var sortList = new List<UnMappedDictionary>();
            sortList.Add(new UnMappedDictionary(CodeConstManager.SORT_INDEX_DATESEND, CultureHelper.GetDictionaryName(CodeConstManager.SORT_NAME_DATESEND)));
            sortList.Add(new UnMappedDictionary(CodeConstManager.SORT_INDEX_DATEEDIT, CultureHelper.GetDictionaryName(CodeConstManager.SORT_NAME_DATEEDIT)));
            ViewData["SortList"] = new SelectList(sortList, "ID",
                                                 "NAME_RU", filter.SortId);

            var reason = new List<UnMappedDictionary>
            {
                new UnMappedDictionary(CodeConstManager.SUB_REASON_SEND_ID, CultureHelper.GetDictionaryName(CodeConstManager.SUB_REASON_SEND)),
                new UnMappedDictionary(CodeConstManager.SUB_REASON_NOTSEND_ID,CultureHelper.GetDictionaryName(CodeConstManager.SUB_REASON_NOTSEND)),
                new UnMappedDictionary(CodeConstManager.SUB_REASON_ALL_ID, CultureHelper.GetDictionaryName(CodeConstManager.SUB_REASON_ALL))
            };

            ViewData["Reasons"] = new SelectList(reason, "ID",
                                               "NAME_RU", filter.SendId);

            var exluldes = new List<UnMappedDictionary>
            {
                new UnMappedDictionary(CodeConstManager.RST_EXCLUDED_ID, CultureHelper.GetDictionaryName(CodeConstManager.RST_EXCLUDED_NAME)),
                new UnMappedDictionary(CodeConstManager.RST_NOTEXCLUDED_ID, CultureHelper.GetDictionaryName(CodeConstManager.RST_NOTEXCLUDED_NAME)),
                new UnMappedDictionary(CodeConstManager.RST_EXCLUDED_ALL_ID, CultureHelper.GetDictionaryName(CodeConstManager.RST_EXCLUDED_ALL))
            };

            ViewData["Exluldes"] = new SelectList(exluldes, "ID",
                                               "NAME_RU", filter.ExcludedId);
            ViewBag.IsAcceptReport = new AccountRepository().IsAcceptReport(MyExtensions.GetCurrentUserId());

        }
        [HttpPost]
        public virtual ActionResult UpdateExpact(long recordId, long? fieldValue)
        {
            new RstReportRepository().UpdateExpact(recordId, fieldValue, MyExtensions.GetCurrentUserId());
            return Json(new { Success = true });

        }
        [HttpPost]
        public virtual ActionResult UpdateReason(long recordId, long? fieldValue)
        {
            new RstReportRepository().UpdateReason(recordId, fieldValue, MyExtensions.GetCurrentUserId());
            return Json(new { Success = true });

        }

        [HttpPost]
        public virtual ActionResult UpdateFsCode(long recordId, int? fieldValue)
        {
            new RstReportRepository().UpdateFsCode(recordId, fieldValue, MyExtensions.GetCurrentUserId());
            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult EditRecord(long id)
        {
            var repository = new RstReportRepository();
            var model = repository.GetRecord(id);
            ViewData["ReasonId"] = new SelectList(new RstDicReasonRepository().GetAll().Where(e => e.Code == CodeConstManager.CODE_REPORT_REESTR), "Id", "NameRu", model.ReasonId);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditRecord(RST_ReportReestr model)
        {
            if (ModelState.IsValid)
            {
                var repository = new RstReportRepository();
                repository.UpdateReportReestr(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Edit", new { id = model.ReportId });
            }
            ViewData["ReasonId"] = new SelectList(new RstDicReasonRepository().GetAll().Where(e => e.Code == CodeConstManager.CODE_REPORT_REESTR), "Id", "NameRu", model.ReasonId);

            return View(model);
        }
        [HttpGet]
        public ActionResult Exluded(long id)
        {
            var repository = new RstReportRepository();
            var model = repository.GetRecord(id);
            ViewData["ReasonId"] = new SelectList(new RstDicReasonRepository().GetAll().Where(e => e.IsExcluded), "Id", "NameRu", model.ReasonId);
            var dir = Server.MapPath("~/uploads/exculded/" + model.Id + "/");
            model.AttachFiles = new List<string>();
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');
                    string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    model.AttachFiles.Add(name);
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Exluded(RST_ReportReestr model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (files == null)
                {
                    files = new List<HttpPostedFileBase>();
                }
                var httpPostedFileBases = files as HttpPostedFileBase[] ?? files.ToArray();
                var name =
                    (from file in httpPostedFileBases
                     where file != null && file.ContentLength > 0
                     select file.FileName).FirstOrDefault();
                if (name != null)
                {
                    var clientfile = name.Split('\\');
                    name = clientfile.Length > 0 ? clientfile[clientfile.Length - 1] : name;
                }

                var repository = new RstReportRepository();
                model.StatusId = CodeConstManager.EXTEND_STATUS_REESTR_ID;
                repository.UpdateReportReestr(model, MyExtensions.GetCurrentUserId());
                var dirpath = Server.MapPath("~/uploads/exculded/" + model.Id);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                var oldfiles = Directory.GetFiles(dirpath);

                foreach (var file in oldfiles)
                {
                    var fullname = file.Split('\\');

                    name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    var exist = model.AttachFiles.Any(existfile => name == existfile);
                    if (exist) continue;
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                foreach (var file in httpPostedFileBases)
                {
                    if (file == null || file.ContentLength <= 0) continue;
                    var uploadFileName = Path.GetFileName(file.FileName);
                    if (uploadFileName == null) continue;
                    var uploadFilePathAndName = Path.Combine(dirpath, uploadFileName);
                    ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                }
                return RedirectToAction("Edit", new { id = model.ReportId });
            }
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            ViewData["ReasonId"] = new SelectList(new RstDicReasonRepository().GetAll().Where(e => e.IsExcluded), "Id", "NameRu", model.ReasonId);
            return View(model);
        }
        public ActionResult Delete(long id)
        {
            new RstReportRepository().DeleteReestr(id, MyExtensions.GetCurrentUserId());
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult Create(int? id)
        {
            var model = new RST_Report
            {
                UserId = MyExtensions.GetCurrentUserId(),
            };
            model.ReportYear = id;
            model.RstReportReestrs = new RstReportRepository().GetReestr(id).OrderBy(e => e.Id);
            FillViewBag(model);
            return View(model);
        }

        public ActionResult RegistredYear()
        {
            var model = new RST_Report
            {
                UserId = MyExtensions.GetCurrentUserId(),
            };
            model.ReportYear = new RstReportRepository().GetNewYear();
            model.RstReportReestrs = new RstReportRepository().GetNewReestr(model.ReportYear).OrderBy(e => e.Id);
            FillViewBag(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult RegistredYear(RST_Report model)
        {
            if (ModelState.IsValid)
            {
                var repository = new RstReportRepository();
                model.CreateDate = DateTime.Now;
                model.Id = 0;
                //                model.RstReportReestrs = new RstReportRepository().GetNewReestr(model.ReportYear).OrderBy(e => e.Id);
                repository.RegistredYear(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("CommonView");
            }
            FillViewBag(model);
            return View(model);

        }

        public ActionResult ShowHistory(long userId)
        {
            var list = new RstReportRepository().GetReestrReportByUserId(userId);
            if (list == null)
            {
                list = new List<RST_ReestrReportHistory>();
            }
            //            var user = new SecUserRepository().GetById(id);
            return View(list);
        }
        public ActionResult Edit(long id, string biniin, string adress, string owner, string status, string reason, string oblast)
        {
            var model = new RstReportRepository().GetById(id);
            var filter = new RST_ReportFilter();
            filter.ReportYear = model.ReportYear;
            filter.ReportId = id;
            if (!string.IsNullOrEmpty(biniin))
            {
                filter.BINIIN = biniin;
            }
            if (!string.IsNullOrEmpty(adress))
            {
                filter.Adress = adress;
            }
            if (!string.IsNullOrEmpty(owner))
            {
                filter.SubjectName = owner;
            }
            if (!string.IsNullOrEmpty(status))
            {
                filter.Statuses = new List<string>();
                var statuslList = status.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Statuses.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(reason))
            {
                filter.Reasons = new List<string>();
                var statuslList = reason.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Reasons.Add(s);
                }
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
            filter.RstReportReestrs = new RstReportRepository().GetRstReportReestrsByFilter(filter);
            FillViewFiltertBag(filter);
            return View(filter);
        }

        public ActionResult ExportExcel(int? year, string idk, string biniin, string adress, string owner, string status, string reason, string oblast, string expact, string subDicStatus, string fsCode)
        {

            var filter = new RST_ReportFilter();
            filter.ReportYear = year;
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
            if (!string.IsNullOrEmpty(status))
            {
                filter.Statuses = new List<string>();
                var statuslList = status.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Statuses.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(reason))
            {
                filter.Reasons = new List<string>();
                var statuslList = reason.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Reasons.Add(s);
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
            if (!string.IsNullOrEmpty(oblast))
            {
                filter.Oblasts = new List<string>();
                var statuslList = oblast.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Oblasts.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(subDicStatus))
            {
                filter.SubDicStatuses = new List<string>();
                var statuslList = subDicStatus.Split(',');
                foreach (var s in statuslList)
                {
                    filter.SubDicStatuses.Add(s);
                }
            }

            if (!string.IsNullOrEmpty(fsCode))
            {
                filter.FsCodes = new List<string>();
                var fsCodeList = fsCode.Split(',');
                foreach (var s in fsCodeList)
                {
                    filter.FsCodes.Add(s);
                }
            }

            FillCommonViewBag(filter);
            filter.RstReportReestrs = new RstReportRepository().GetCommonReestrsByFilter(filter);

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

        private void FillViewFiltertBag(RST_ReportFilter filter)
        {
            filter.StatusList = GetStatus(filter.Statuses);
            filter.ReasonList = GetReason(filter.Reasons);
            filter.OblastList = GetOblasList(filter.Oblasts);
        }
        public MultiSelectList GetStatus(IList<string> selectedValues)
        {
            //var plants = new RstDicStatusRepository().GetAll().Where(e => e.Id == 1 ||  e.Id == 4);
            var plants = new RstDicStatusRepository().GetAll().Where(e => e.IsDeleted == false);
            return new MultiSelectList(plants, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        public MultiSelectList GetReason(IList<string> selectedValues)
        {
            var plants = new RstDicReasonRepository().GetAll();
            return new MultiSelectList(plants, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        public MultiSelectList GetFsCode(IList<string> selectedValues)
        {
            var list = new List<FsCode>();
            list.Add(new FsCode() { Id = 5, NameRu = "" });
            list.Add(new FsCode() { Id = 1, NameRu = "юр" });
            list.Add(new FsCode() { Id = 2, NameRu = "кв" });
            list.Add(new FsCode() { Id = 3, NameRu = "гу" });
            list.Add(new FsCode() { Id = 4, NameRu = "ип" });
            return new MultiSelectList(list, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        public MultiSelectList GetFsCode2(IList<string> selectedValues)
        {
            var list = new List<FsCode>();

            list.Add(new FsCode() { Id = 1, NameRu = "юр" });
            list.Add(new FsCode() { Id = 2, NameRu = "кв" });
            list.Add(new FsCode() { Id = 3, NameRu = "гу" });
            list.Add(new FsCode() { Id = 4, NameRu = "ип" });

            return new MultiSelectList(list, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        public MultiSelectList GetSubDicStatus(IList<string> selectedValues)
        {
            var plants = new SubDicStatusRepository().GetAll();
            var list = new List<SUB_DIC_Status>();
            list.Add(new SUB_DIC_Status() { Id = 0, NameRu = CultureHelper.GetDictionaryName(CodeConstManager.SUB_DIC_STATUS_NOTGIVED) });
            foreach (var subDicStatuse in plants)
            {
                if (subDicStatuse.Id != 1)
                {
                    list.Add(subDicStatuse);
                }
            }
            return new MultiSelectList(list, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        public MultiSelectList GetExpact(IList<string> selectedValues)
        {
            var plants = new RstDicReasonRepository().GetAll().Where(e => e.IsExcluded || e.Id == CodeConstManager.STATUS_EVADERS_ID);
            return new MultiSelectList(plants, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }
        public MultiSelectList GetOblasList(IList<string> selectedValues)
        {
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            return new MultiSelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        [HttpPost]
        public ActionResult Create(RST_Report model)
        {
            if (ModelState.IsValid)
            {
                var repository = new RstReportRepository();
                model.CreateDate = DateTime.Now;
                model.Id = 0;
                model.RstReportReestrs = new RstReportRepository().GetReestr(model.ReportYear).OrderBy(e => e.Id);
                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            FillViewBag(model);
            return View(model);

        }

        private void FillViewBag(RST_Report model)
        {
            var listanimal = new RstReportRepository().GetYears();
            ViewData["Years"] = new SelectList(listanimal, "ID",
                                                 "NAME_RU", model.ReportYear);
        }
        [HttpGet]
        public ActionResult ShowFile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var list = id.Split('#');
            string path = list[0];
            string filename = null;
            if (list.Count() > 1)
            {
                filename = list[1].Replace(",", ".");
            }
            var dir = Server.MapPath("~/uploads/exculded/" + path);
            if (!Directory.Exists(dir))
            {
                return RedirectToAction("Index");
            }

            var files = Directory.GetFiles(dir);
            if (files.Length == 0)
            {
                return RedirectToAction("Index");
            }
            var fullname = (from file in files let split = file.Split('\\') let name = split.Length > 0 ? split[split.Length - 1] : file where filename == name select file).FirstOrDefault() ??
                              files[0];
            var fi = new FileInfo(fullname);
            return File(fi.FullName, GetContentType(fi.Name), fi.Name);

        }

        public ActionResult SignedObjectsList(long id)
        {
            var repository = new SubFormRepository();
            var histories = repository.GetHistoryListBySubFormId(id);
            return View(histories);
        }

        public ActionResult SubjectCard(long secId, int reportYear)
        {
            return View();
        }

        public ActionResult ChangeStatus(long userId, int reportYear, int statusId, int formId)
        {
            string ErrorMessage = new SubFormRepository().ChangeStatus(userId, reportYear, statusId, formId);
            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeIDK(long userId, int reestrId, string idk)
        {
            string ErrorMessage = new SubFormRepository().ChangeIDK(userId, reestrId, idk);
            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSendDate(long reestrId, string sendDate)
        {
            string ErrorMessage = new SubFormRepository().UpdateSendDate(reestrId, sendDate, MyExtensions.GetCurrentUserId());
            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }
        #region  Export To Excel
        public ActionResult ExportToExcelEx(long id)
        {
            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/form1Tmpl.xlsx");

            var handle = Guid.NewGuid().ToString();
            FileInfo fiTemplate = new FileInfo(path);

            ExcelPackage package = new ExcelPackage(fiTemplate, true);
            //            var wBook = package.Workbook;
            //            var wSheet = wBook.Worksheets[1];
            //            wSheet.Cells["B9"].Value = model.SEC_User1.ApplicationName;
            //            wSheet.Cells["C9"].Value = model.SEC_User1.Address;
            //            wSheet.Cells["D9"].Value = model.SEC_User1.FullName;
            //            wSheet.Cells["E9"].Value = model.SEC_User1.Post;
            //            wSheet.Cells["F9"].Value = model.SEC_User1.IsCvazy ? "Да": "Нет";

            using (var memoryStream = new MemoryStream())
            {
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;
                Session[handle] = memoryStream.ToArray();
            }

            //            FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
            //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //            result.FileDownloadName = "reportForm1.xlsx";
            //            return result;

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "TestReportOutput.xlsx" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ASubjectExportToExcelEx(long id)
        {
            var handle = Guid.NewGuid().ToString();
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/aSubjectTmpl.xlsx");

            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            if (model.SEC_User1.FSCode == 3)
            {
                if (model.ReportYear >= 2018 && model.ReportYear < 2019)
                {
                    return RedirectToAction("ASubjectExportToExcelExGu", new { id = id });
                }
                else
                {
                    return RedirectToAction("ASubjectExportToExcelExGuNew", new { id = id });
                }
            }

            IWorkbook book = new XSSFWorkbook(fiTemplate);
            ISheet sheet = null;
            IRow row = null;
            string year = model.ReportYear.ToString()
                , companyName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;
            CellRangeAddress cellRange = null;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                companyName = model.SEC_User1.JuridicalName;
            int count = 0;
            ISheet sheetSub = null;
            int lastNum = 0;

            #region F-1

            if (model.SEC_User1 != null)
            {
                sheet = book.GetSheetAt(0);
                row = sheet.GetRow(9 - 1);
                row.Cells[1].SetCellValue(model.SEC_User1.ApplicationName);
                row.Cells[2].SetCellValue(model.SEC_User1.Address);
                row.Cells[3].SetCellValue(model.SEC_User1.FullName);
                row.Cells[4].SetCellValue(model.SEC_User1.Post);
                row.Cells[5].SetCellValue(model.SEC_User1.IsCvazy ? "Да" : "Нет");

                string oked = string.Empty;
                oked = model.SEC_User1.DIC_OKED.FullName;
                //                foreach (var secUserOked in model.SEC_User1.SEC_UserOked)
                //                {
                //                    oked += secUserOked.DIC_OKED.FullName;
                //                }

                row.Cells[6].SetCellValue(oked);

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;

                    row = sheet.GetRow(18 - 1);
                    row.Cells[5].SetCellValue(responceFIO);
                }

                //  Полностью ФИО, подпись руководителя организации
                row = sheet.GetRow(19 - 1);
                headFIO = model.SEC_User1.FullName;
                row.Cells[5].SetCellValue(headFIO);
            }

            #endregion

            // Ф-2

            #region F-2
            {
                // Init 
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                sheet = book.GetSheetAt(1); // Ф-2
                // Заговолок
                row = sheet.GetRow(5 - 1);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("year", year);
                row.Cells[0].SetCellValue(strValue);
                // Название компании
                row = sheet.GetRow(9 - 1);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("companyname", companyName);
                row.Cells[0].SetCellValue(strValue);


                // InsertRows(ref sheet, 16 - 2, sheet.LastRowNum, model.SubForm2Records.Count - 2); 
                startRowIndex = 14;
                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    //row = sheet.CreateRow(1);
                    var rowNum = startRowIndex - 1 + i;
                    row = sheet.GetRow(rowNum);

                    // 1
                    row.Cells[0].SetCellValue(i + 1);

                    // 2
                    var typeResourceName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu;
                    row.Cells[1].SetCellValue(typeResourceName);

                    // 3
                    var unitName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                                   || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu;
                    row.Cells[2].SetCellValue(unitName);

                    // 4
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        row.Cells[3].SetCellValue(string.Empty);
                    else
                        row.Cells[3].SetCellValue(model.SubForm2Records[i].ExtractVolume.Value);

                    // 5а
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        row.Cells[4].SetCellValue(string.Empty);
                    else
                        row.Cells[4].SetCellValue(model.SubForm2Records[i].NotOwnSource.Value);

                    // 5б
                    if (!model.SubForm2Records[i].LosEnergy.HasValue)
                        row.Cells[5].SetCellValue(string.Empty);
                    else
                        row.Cells[5].SetCellValue(model.SubForm2Records[i].LosEnergy.Value);

                    // 6
                    if (!model.SubForm2Records[i].OwnSource.HasValue)
                        row.Cells[6].SetCellValue(string.Empty);
                    else
                        row.Cells[6].SetCellValue(model.SubForm2Records[i].OwnSource.Value);

                    // 7
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        row.Cells[7].SetCellValue(string.Empty);
                    else
                        row.Cells[7].SetCellValue(model.SubForm2Records[i].TransferOtherLegal.Value);

                    // 8
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        row.Cells[8].SetCellValue(string.Empty);
                    else
                        row.Cells[8].SetCellValue(model.SubForm2Records[i].ExpenceEnergy.Value);

                    // 9
                    var note = model.SubForm2Records[i].Note ?? string.Empty;
                    row.Cells[9].SetCellValue(note);
                }

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(responceFIO))
                {
                    row = sheet.GetRow(startRowIndex + model.SubForm2Records.Count + 11 - 1); // 53
                    row.Cells[5].SetCellValue(responceFIO);
                }
                //  Полностью ФИО, подпись руководителя организации
                if (!string.IsNullOrEmpty(headFIO))
                {
                    row = sheet.GetRow(startRowIndex + model.SubForm2Records.Count + 12 - 1);
                    row.Cells[5].SetCellValue(headFIO);
                }
            }

            #endregion

            // Ф-3

            #region F-3

            sheet = book.GetSheetAt(2);
            // Заговолок
            row = sheet.GetRow(5 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("year", year);
            row.Cells[0].SetCellValue(strValue);


            // Название компании
            row = sheet.GetRow(7 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            var kinds = new SubDicKindResourceRepository().GetAll();
            var form3 = new List<SUB_Form3Record>();
            foreach (var rptDicKindWaste in kinds)
            {
                var report = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                if (report != null)
                {
                    form3.Add(report);
                }
                else
                {
                    var kind = new SUB_Form3Record
                    {
                        KindResourceId = rptDicKindWaste.Id,
                        SUB_DIC_KindResource = rptDicKindWaste
                    };
                    form3.Add(kind);
                }
            }
            model.SubForm3Records = form3;
            startRowIndex = 11;

            for (int i = 0; i < model.SubForm3Records.Count; i++)
            {
                row = sheet.GetRow(startRowIndex - 1 + 2 * i);
                // 4 - Потребление воды
                if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(model.SubForm3Records[i].ConsumptionVolume.Value);
                // 5 - Потери воды при транспортировке
                if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(model.SubForm3Records[i].LosTransportVolume.Value);

                row = sheet.GetRow(startRowIndex + 2 * i);
                // 4 - Потребление воды
                if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(model.SubForm3Records[i].ConsumptionPrice.Value);
                // 5 - Потери воды при транспортировке
                if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(model.SubForm3Records[i].LosTransportPrice.Value);
            }

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + model.SubForm3Records.Count * 2 + 8 - 1); // 53
                row.Cells[4].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + model.SubForm3Records.Count * 2 + 9 - 1);
                row.Cells[4].SetCellValue(headFIO);
            }

            #endregion


            // Ф-4

            #region F-4

            sheet = book.GetSheetAt(3);
            // Заговолок
            row = sheet.GetRow(5 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("year", year);
            row.Cells[0].SetCellValue(strValue);

            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                sheet.GetRow(7).GetCell(0).SetCellValue("X");
                sheet.GetRow(9).GetCell(0).SetCellValue("");
            }
            else
            {
                sheet.GetRow(7).GetCell(0).SetCellValue("");
                sheet.GetRow(9).GetCell(0).SetCellValue("X");
            }

            // Не проводились планы мероприятий
            if (model.IsNotEvents == false)
            {
                sheet.GetRow(11).GetCell(0).SetCellValue("");
            }

            // Название компании
            row = sheet.GetRow(14 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 19;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
                InsertRows(ref sheet, startRowIndex - 1, count);

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                row = sheet.GetRow(startRowIndex + i - 1);
                row.Cells[0].SetCellValue(i + 1);
                if (record.SUB_DIC_Event != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_Event.NameRu);
                else
                    row.Cells[1].SetCellValue(record.EventName);
                row.Cells[2].SetCellValue(record.EmplPeriodStr);
                if (record.ActualInvest == null)
                    row.Cells[3].SetCellValue(string.Empty);
                else
                    row.Cells[3].SetCellValue(record.PlanExpend);
                if (record.ActualInvest == null)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(record.ActualInvest.Value);
                if (record.SUB_DIC_TypeCounter == null)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(record.SUB_DIC_TypeCounter.NameRu);
                if (record.InKind == null)
                    row.Cells[6].SetCellValue(string.Empty);
                else
                    row.Cells[6].SetCellValue(record.InKind.Value);
                if (record.InMoney == null)
                    row.Cells[7].SetCellValue(string.Empty);
                else
                    row.Cells[7].SetCellValue(record.InMoney.Value);
            }

            // copy footer
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 3);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(4);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[4].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[4].SetCellValue(headFIO);
            }

            #endregion

            // Ф-5

            #region F-5

            sheet = book.GetSheetAt(4);
            // Заговолок
            row = sheet.GetRow(6 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("year", year);
            row.Cells[0].SetCellValue(strValue);
            // Название компании
            row = sheet.GetRow(8 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            // get data for 
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 12;
            count = model.SubForm5Records.Count;
            if (count > 1)
                InsertRows(ref sheet, startRowIndex - 1, count);

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];
                row = sheet.GetRow(startRowIndex + i - 1);
                row.Cells[0].SetCellValue(i + 1);

                if (record.sub_dic_energyindicator != null)
                    row.Cells[1].SetCellValue(record.sub_dic_energyindicator.nameru);
                else
                    row.Cells[1].SetCellValue(record.IndicatorName);
                row.Cells[2].SetCellValue(record.RegularStandart);
                row.Cells[3].SetCellValue(record.UnitMeasure);
                row.Cells[4].SetCellValue(record.CalcFormula);

                if (record.EnergyValue == null)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(record.EnergyValue.Value);
            }

            // copy footer
            sheetSub = book.GetSheetAt(5);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(5);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[2].SetCellValue(headFIO);
            }

            #endregion

            // Ф-6

            #region F-6

            sheet = book.GetSheetAt(5);

            // Название компании
            row = sheet.GetRow(7 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            startRowIndex = 11;
            count = model.SubForm6Records.Count;
            if (count > 1)
                InsertRows(ref sheet, startRowIndex - 1, count);

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];
                row = sheet.GetRow(startRowIndex + i - 1);
                row.Cells[0].SetCellValue(i + 1);
                if (record.SUB_DIC_TypeCounter != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_TypeCounter.NameRu);
                else
                    row.Cells[1].SetCellValue(String.Empty);

                if (record.CountDevice != null)
                    row.Cells[2].SetCellValue(record.CountDevice.Value);
                else
                    row.Cells[2].SetCellValue(string.Empty);

                if (record.Equipment != null)
                    row.Cells[3].SetCellValue(record.Equipment.Value);
                else
                    row.Cells[3].SetCellValue(string.Empty);
            }

            sheetSub = book.GetSheetAt(6);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(6);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[2].SetCellValue(headFIO);
            }

            #endregion


            // План мероприятий

            #region План мероприятий

            if (model.IsPlan != null && model.IsPlan == true && model.BeginPlanYear != null && model.EndPlanYear != null)
            {
                #region Таблица 1

                sheet = book.GetSheetAt(6);
                startRowIndex = 8;

                model.SubDicKindTabOnes = new SubDicKindTabOneRepository().GetAll();
                model.SubFormTab1s = new List<SUB_FormTab1>();

                foreach (var tabOne in model.SubDicKindTabOnes)
                {
                    var list = model.SUB_FormTab1.Where(e => e.KindId == tabOne.Id).ToList();
                    var codes = new List<string>();
                    for (var t = 1; t < 3; t++)
                    {
                        var codeIndex = tabOne.IndexCode + ".0" + t;
                        codes.Add(codeIndex);
                        if (list.Any(e => e.Code == codeIndex))
                        {
                            model.SubFormTab1s.Add(list.First(e => e.Code == codeIndex));
                        }
                        else
                        {
                            model.SubFormTab1s.Add(new SUB_FormTab1() { Code = codeIndex, KindId = tabOne.Id });
                        }
                    }
                    var notIn = list.Where(e => !codes.Contains(e.Code));
                    foreach (var subFormTab1 in notIn)
                    {
                        model.SubFormTab1s.Add(subFormTab1);
                    }
                }

                for (int i = model.BeginPlanYear.Value; i <= model.EndPlanYear.Value; i++)
                {
                    row = sheet.GetRow(6 - 1);
                    row.Cells[3 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                    row.Cells[8 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                }

                count = model.SubDicKindTabOnes.Count + model.SubFormTab1s.Count
                        + model.SubDicKindTabOnes.Count * 2 + 2 - 1;

                InsertRows(ref sheet, 8 - 1, count);

                var rowIndex = 0;
                foreach (SUB_DIC_KindTabOne sdk in model.SubDicKindTabOnes)
                {
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, row.LastCellNum - 1);
                    sheet.AddMergedRegion(cellRange);

                    var subDicKind = sdk;
                    var cell = row.Cells[0];
                    cell.SetCellValue(subDicKind.NameRu);

                    ICellStyle cellStyle = book.CreateCellStyle();
                    IFont font = book.CreateFont();
                    font.IsBold = font.IsItalic = true;

                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cellStyle.SetFont(font);
                    cell.CellStyle = cellStyle;

                    foreach (var subFormTab in model.SubFormTab1s.Where(sft => sft.KindId == subDicKind.Id))
                    {
                        row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                        row.Cells[0].SetCellValue(subFormTab.Code);
                        row.Cells[1].SetCellValue(subFormTab.Events);
                        row.Cells[2].SetCellValue(subFormTab.Year1);
                        row.Cells[3].SetCellValue(subFormTab.Year2);
                        row.Cells[4].SetCellValue(subFormTab.Year3);
                        row.Cells[5].SetCellValue(subFormTab.Year4);
                        row.Cells[6].SetCellValue(subFormTab.Year5);

                        if (subFormTab.Expend1 != null)
                            row.Cells[7].SetCellValue(subFormTab.Expend1.Value);
                        if (subFormTab.Expend2 != null)
                            row.Cells[8].SetCellValue(subFormTab.Expend2.Value);
                        if (subFormTab.Expend3 != null)
                            row.Cells[9].SetCellValue(subFormTab.Expend3.Value);
                        if (subFormTab.Expend4 != null)
                            row.Cells[10].SetCellValue(subFormTab.Expend4.Value);
                        if (subFormTab.Expend5 != null)
                            row.Cells[11].SetCellValue(subFormTab.Expend5.Value);
                        row.Cells[12].SetCellValue(subFormTab.Note);
                    }

                    // TODO нету инициализации полей
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                    sheet.AddMergedRegion(cellRange);
                    row.Cells[0].SetCellValue("Итого:");

                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                    sheet.AddMergedRegion(cellRange);
                    row.Cells[0].SetCellValue("Всего:");

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 7, 11);
                    sheet.AddMergedRegion(cellRange);
                }
                // TODO нету инициализации полей
                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Итого по плану:");

                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Всего по плану:");

                #endregion

                #region Таблица 2

                sheet = book.GetSheetAt(7);
                startRowIndex = 9;

                var dicsTwo = new SubDicKindTabTwoRepository().GetAll();
                var listTwo = new List<SUB_FormTab2>();
                foreach (var rptDicKindWaste in dicsTwo)
                {
                    var report = model.SUB_FormTab2.FirstOrDefault(e => e.KindId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        listTwo.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_FormTab2 { KindId = rptDicKindWaste.Id, SUB_DIC_KindTabTwo = rptDicKindWaste };
                        listTwo.Add(kind);
                    }
                }
                model.SubFormTab2s = listTwo;

                for (int i = model.BeginPlanYear.Value; i <= model.EndPlanYear.Value; i++)
                {
                    row = sheet.GetRow(6 - 1);
                    row.Cells[3 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                    row.Cells[8 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                }

                count = model.SubFormTab2s.Count + 2 - 1;
                InsertRows(ref sheet, startRowIndex - 1, count);

                rowIndex = 0;
                foreach (var sft in model.SubFormTab2s)
                {
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    var subFormTab2 = sft;

                    row.Cells[0].SetCellValue(subFormTab2.SUB_DIC_KindTabTwo.IndexCode);
                    row.Cells[1].SetCellValue(subFormTab2.SUB_DIC_KindTabTwo.NameRu);
                    if (subFormTab2.Volume1.HasValue)
                        row.Cells[2].SetCellValue(subFormTab2.Volume1.Value);
                    if (subFormTab2.Volume2.HasValue)
                        row.Cells[3].SetCellValue(subFormTab2.Volume2.Value);
                    if (subFormTab2.Volume3.HasValue)
                        row.Cells[4].SetCellValue(subFormTab2.Volume3.Value);
                    if (subFormTab2.Volume4.HasValue)
                        row.Cells[5].SetCellValue(subFormTab2.Volume4.Value);
                    if (subFormTab2.Volume5.HasValue)
                        row.Cells[6].SetCellValue(subFormTab2.Volume5.Value);

                    if (subFormTab2.Expend1.HasValue)
                        row.Cells[7].SetCellValue(subFormTab2.Expend1.Value);
                    if (subFormTab2.Expend2.HasValue)
                        row.Cells[8].SetCellValue(subFormTab2.Expend2.Value);
                    if (subFormTab2.Expend3.HasValue)
                        row.Cells[9].SetCellValue(subFormTab2.Expend3.Value);
                    if (subFormTab2.Expend4.HasValue)
                        row.Cells[10].SetCellValue(subFormTab2.Expend4.Value);
                    if (subFormTab2.Expend5.HasValue)
                        row.Cells[11].SetCellValue(subFormTab2.Expend5.Value);

                    row.Cells[12].SetCellValue(subFormTab2.Possible);
                    row.Cells[13].SetCellValue(subFormTab2.Note);
                }

                // TODO нету инициализации полей
                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Итого:");

                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Всего:");

                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 2, 6);
                sheet.AddMergedRegion(cellRange);

                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 7, 11);
                sheet.AddMergedRegion(cellRange);

                #endregion

                #region Таблица 3

                sheet = book.GetSheetAt(8);
                startRowIndex = 8;

                model.SubFormTab3s = new List<SUB_FormTab3>();
                var formtab3 = model.SUB_FormTab3.OrderBy(e => e.Id);
                foreach (var record in formtab3)
                {
                    model.SubFormTab3s.Add(record);
                }
                if (model.SubFormTab3s.Count == 0)
                {
                    model.SubFormTab3s.Add(new SUB_FormTab3());
                }

                for (int i = model.BeginPlanYear.Value; i <= model.EndPlanYear.Value; i++)
                {
                    row = sheet.GetRow(6 - 1);
                    row.Cells[4 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                }

                count = model.SubFormTab3s.Count + 2 - 1;
                InsertRows(ref sheet, startRowIndex - 1, count);
                rowIndex = 0;
                foreach (var sft in model.SubFormTab3s)
                {
                    var subformTab = sft;
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    row.Cells[0].SetCellValue(rowIndex);
                    row.Cells[1].SetCellValue(subformTab.ShareIndex);
                    row.Cells[2].SetCellValue(subformTab.UnitKoef);
                    if (subformTab.Volume1.HasValue)
                        row.Cells[3].SetCellValue(subformTab.Volume1.Value);
                    if (subformTab.Volume2.HasValue)
                        row.Cells[4].SetCellValue(subformTab.Volume2.Value);
                    if (subformTab.Volume3.HasValue)
                        row.Cells[5].SetCellValue(subformTab.Volume3.Value);
                    if (subformTab.Volume4.HasValue)
                        row.Cells[6].SetCellValue(subformTab.Volume4.Value);
                    if (subformTab.Volume5.HasValue)
                        row.Cells[7].SetCellValue(subformTab.Volume5.Value);
                }

                // TODO нету инициализации полей
                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 2);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Итого:");

                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 2);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Всего:");

                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 3, 7);
                sheet.AddMergedRegion(cellRange);

                #endregion

            }
            else
            {
                book.RemoveSheetAt(6);
                book.RemoveSheetAt(6);
                book.RemoveSheetAt(6);
            }



            #endregion

            HSSFFormulaEvaluator.EvaluateAllFormulaCells(book);
            // }
            using (var memoryStream = new MemoryStream())
            {
                book.Write(memoryStream);
                //memoryStream.Position = 0;
                Session[handle] = memoryStream.ToArray();
            }

            #region check company name
            while (companyName.IndexOf("\"") != -1)
            {
                companyName = companyName.Replace("\"", "");
            }

            while (companyName.IndexOf("|") != -1)
            {
                companyName = companyName.Replace("|", "");
            }

            while (companyName.IndexOf("?") != -1)
            {
                companyName = companyName.Replace("?", "");
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                if (companyName.Length > 100)
                    companyName.Substring(0, 99);
            }
            #endregion

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = companyName + ".xlsx" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ASubjectExportToPdf(long id)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/");

            Aspose.Words.License lis = new Aspose.Words.License();
            Aspose.Words.License lisdocs = new Aspose.Words.License();
            lis.SetLicense(path + "Aspose.Words.lic");


            var doc = new Aspose.Words.Document(path + "aSubjectTmplWord.docx");
            var handle = Guid.NewGuid().ToString();


            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            if (model.ReportYear >= 2018 && model.SEC_User1.FSCode == 3)
            {
                return RedirectToAction("ASubjectExportToExcelExGu", new { id = id });
            }

            string year = model.ReportYear.ToString()
                , juridicalName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                juridicalName = model.SEC_User1.JuridicalName;
            int count = 0;
            int lastNum = 0;

            #region F-1
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 0, true);
            if (model.SEC_User1 != null)
            {
                Row row = table.LastRow;
                row.Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.ApplicationName));

                if (model.SEC_User1.Address != null)
                    row.Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Address));

                row.Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));

                if (model.SEC_User1.Post != null)
                    row.Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Post));

                row.Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.IsCvazy ? "Да" : "Нет"));

                string oked = string.Empty;
                oked = (model.SEC_User1.DIC_OKED != null) ? model.SEC_User1.DIC_OKED.FullName : "";
                row.Cells[6].FirstParagraph.AppendChild(new Run(doc, oked));

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                Aspose.Words.Tables.Table table7 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 6, true);
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;

                    table7.Rows[0].Cells[1].FirstParagraph.AppendChild(new Run(doc, responceFIO));
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
                table7.Rows[1].Cells[1].FirstParagraph.AppendChild(new Run(doc, headFIO));
            }

            #endregion

            FindReplaceOptions replaceOptions = new FindReplaceOptions();
            replaceOptions.MatchCase = true;
            replaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("_year_", year, replaceOptions);
            doc.Range.Replace("juridical_name", juridicalName, replaceOptions);
            if (model.IsRent == true)
                doc.Range.Replace("_isarenda", "x", replaceOptions);
            else doc.Range.Replace("_isarenda", " ", replaceOptions);

            // Ф-2
            #region F-2
            {
                // Init 
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                // InsertRows(ref sheet, 16 - 2, sheet.LastRowNum, model.SubForm2Records.Count - 2); 
                startRowIndex = 14;
                Aspose.Words.Tables.Table table2 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 1, true);
                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    //row = sheet.CreateRow(1);
                    var rowNum = startRowIndex - 1 + i;

                    // 1
                    table2.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (1 + i).ToString()));

                    // 2
                    var typeResourceName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu;
                    table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, typeResourceName));

                    // 3
                    var unitName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                                   || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu;
                    table2.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, unitName));

                    // 4
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExtractVolume.Value.ToString()));

                    // 5а
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].NotOwnSource.Value.ToString()));

                    // 5б
                    if (!model.SubForm2Records[i].LosEnergy.HasValue)
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].LosEnergy.Value.ToString()));

                    // 6
                    if (!model.SubForm2Records[i].OwnSource.HasValue)
                        table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, ""));
                    else table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].OwnSource.Value.ToString()));

                    // 7
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        table2.Rows[2 + i].Cells[7].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[7].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].TransferOtherLegal.Value.ToString()));

                    // 8
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        table2.Rows[2 + i].Cells[8].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[8].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExpenceEnergy.Value.ToString()));

                    // 9
                    var note = model.SubForm2Records[i].Note ?? string.Empty;
                    table2.Rows[2 + i].Cells[9].FirstParagraph.AppendChild(new Run(doc, note));
                }

            }

            #endregion

            // Ф-3

            #region F-3
            Aspose.Words.Tables.Table table3 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);
            var kinds = new SubDicKindResourceRepository().GetAll();
            var form3 = new List<SUB_Form3Record>();
            foreach (var rptDicKindWaste in kinds)
            {
                var report = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                if (report != null)
                {
                    form3.Add(report);
                }
                else
                {
                    var kind = new SUB_Form3Record
                    {
                        KindResourceId = rptDicKindWaste.Id,
                        SUB_DIC_KindResource = rptDicKindWaste
                    };
                    form3.Add(kind);
                }
            }
            model.SubForm3Records = form3;
            startRowIndex = 11;

            for (int i = 0; i < model.SubForm3Records.Count; i++)
            {

                if (model.SubForm3Records[i].SUB_DIC_KindResource.Id == 1)
                {
                    // 4 - Потребление воды
                    if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                        table3.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                        table3.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                        table3.Rows[3].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[3].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionPrice.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                        table3.Rows[3].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[3].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportPrice.Value.ToString()));
                }


                if (model.SubForm3Records[i].SUB_DIC_KindResource.Id == 2)
                {
                    // 4 - Потребление воды
                    if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                        table3.Rows[4].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[4].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                        table3.Rows[4].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[4].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportVolume.Value.ToString()));


                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                        table3.Rows[5].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[5].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionPrice.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                        table3.Rows[5].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[5].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportPrice.Value.ToString()));
                }

                if (model.SubForm3Records[i].SUB_DIC_KindResource.Id == 3)
                {
                    // 4 - Потребление воды
                    if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                        table3.Rows[6].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[6].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                        table3.Rows[6].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[6].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportVolume.Value.ToString()));


                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                        table3.Rows[7].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[7].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionPrice.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                        table3.Rows[7].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[7].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportPrice.Value.ToString()));
                }
            }

            #endregion


            // Ф-4

            #region F-4
            // проверить энергоаудит проводился или ...
            Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            if (model.IsPlan == true)
            {
                doc.Range.Replace("_isplan", "x", replaceOptions);
                doc.Range.Replace("_isnotplan", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isplan", "", replaceOptions);
                doc.Range.Replace("_isnotplan", "x", replaceOptions);
            }

            // Не проводились планы мероприятий
            if (model.IsNotEvents == false)
            {
                doc.Range.Replace("_isnotevents", "x", replaceOptions);
            }
            else doc.Range.Replace("_isnotevents", "", replaceOptions);

            // Название компании
            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 19;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table4.LastRow.Clone(true);
                    table4.Rows.Insert(i + 3, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];

                table4.Rows[i + 3].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.SUB_DIC_Event != null)
                    table4.Rows[i + 3].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_Event.NameRu));
                else
                {
                    if (record.EventName != null)
                        table4.Rows[i + 3].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EventName));
                }

                if (record.EmplPeriodStr != null)
                    table4.Rows[i + 3].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.EmplPeriodStr));
                else table4.Rows[i + 3].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                if (record.PlanExpend == null)
                    table4.Rows[i + 3].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.PlanExpend));

                if (record.ActualInvest == null)
                    table4.Rows[i + 3].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.ActualInvest.Value.ToString()));

                if (record.SUB_DIC_TypeResource == null)
                    table4.Rows[i + 3].Cells[5].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeResource.NameRu));

                if (record.InKind == null)
                    table4.Rows[i + 3].Cells[6].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[6].FirstParagraph.AppendChild(new Run(doc, record.InKind.Value.ToString()));

                if (record.InMoney == null)
                    table4.Rows[i + 3].Cells[7].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[7].FirstParagraph.AppendChild(new Run(doc, record.InMoney.Value.ToString()));
            }
            #endregion

            // Ф-5

            #region F-5
            Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 12;
            count = model.SubForm5Records.Count;

            if (count > 1)
            {

                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table5.LastRow.Clone(true);
                    table5.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];

                table5.Rows[i + 2].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.sub_dic_energyindicator != null)
                    table5.Rows[i + 2].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.IndicatorName))
                        table5.Rows[i + 2].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));
                }

                if (!string.IsNullOrWhiteSpace(record.RegularStandart))
                    table5.Rows[i + 2].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.RegularStandart));

                if (record.UnitMeasure != null)
                    table5.Rows[i + 2].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));
                else table5.Rows[i + 2].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                if (record.CalcFormula != null)
                    table5.Rows[i + 2].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));
                else table5.Rows[i + 2].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                if (record.EnergyValue == null)
                    table5.Rows[i + 2].Cells[5].FirstParagraph.AppendChild(new Run(doc, ""));
                else table5.Rows[i + 2].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));
            }
            #endregion

            // Ф-6
            #region F-6
            Aspose.Words.Tables.Table table6 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 5, true);
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            startRowIndex = 11;
            count = model.SubForm6Records.Count;

            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table6.LastRow.Clone(true);
                    table6.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];
                table6.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.SUB_DIC_TypeCounter != null)
                    table6.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));

                if (record.CountDevice != null)
                    table6.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountDevice.Value.ToString()));

                if (record.Equipment != null)
                    table6.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Equipment.Value.ToString()));
            }
            #endregion

            #region check company name
            while (juridicalName.IndexOf("\"") != -1)
            {
                juridicalName = juridicalName.Replace("\"", "");
            }

            while (juridicalName.IndexOf("|") != -1)
            {
                juridicalName = juridicalName.Replace("|", "");
            }

            while (juridicalName.IndexOf("?") != -1)
            {
                juridicalName = juridicalName.Replace("?", "");
            }

            while (juridicalName.IndexOf(":") != -1)
            {
                juridicalName = juridicalName.Replace(":", "");
            }

            if (!string.IsNullOrWhiteSpace(juridicalName))
            {
                if (juridicalName.Length > 100)
                    juridicalName.Substring(0, 99);
            }
            #endregion

            //----write qrcode in word file
            #region
            string text = @"<Deal>
                      <Id>358</Id>
                      <DealNo>GAS1232</DealNo>
                      <DateCreate>2018-12-06T14:38:00</DateCreate>
                      <DateUpdate>2019-01-21T15:18:36.063</DateUpdate>
                      <AuctionTypeId>1</AuctionTypeId>
                      <AuctionTypeName>Двойной встречный анонимный аукцион</AuctionTypeName>
                      <AuctionTypeCode/>
                      <ProductTypeId>1</ProductTypeId>
                      <ProductTypeName>Сжиженный нефтяной газ</ProductTypeName>
                      <ProductTypeCode>СНГ</ProductTypeCode>
                      <StartDateAuction>21.01.2019</StartDateAuction>
                      <StartTimeAuction>10:00</StartTimeAuction>
                      <EndDateAuction>21.01.2019</EndDateAuction>
                      <EndTimeAuction>19:00</EndTimeAuction>
                      <LotsCount>8</LotsCount>
                      <LotsVolume>16</LotsVolume>
                      <LotPrice>191</LotPrice>
                      <LotsSum>1528</LotsSum>
                      <LotVolume>3</LotVolume>
                      <BuyerId>3</BuyerId>
                      <BuyerOrganizationJuridicalName>Покупатель 1</BuyerOrganizationJuridicalName>
                      <BuyerOrganizationBIN>123456789012</BuyerOrganizationBIN>
                      <BuyerFirstHeadFIO>Адилов Ерлан3 Айкенович</BuyerFirstHeadFIO>
                      <SellerId>8</SellerId>
                      <SellerOrganizationJuridicalName>Продавец 1</SellerOrganizationJuridicalName>
                      <SellerOrganizationBIN>123456789031</SellerOrganizationBIN>
                      <SellerFirstHeadFIO>Сериков Жандос3 Ахметович</SellerFirstHeadFIO>
                      <OperatorId>13</OperatorId>
                      <OperatorFIO>Сатибалдиев Берик Шегирбаевич</OperatorFIO>
                      <IsSignBuyer>true</IsSignBuyer>
                      <IsSignSeller>false</IsSignSeller>
                      <IsSignOperator>false</IsSignOperator>
                    <ds:Signature>
                    <ds:SignedInfo>
                    <ds:CanonicalizationMethod />
                    <ds:SignatureMethod />
                    <ds:Reference >
                    <ds:Transforms>
                    <ds:Transform/>
                    <ds:Transform />
                    </ds:Transforms>
                    <ds:DigestMethod />
                    <ds:DigestValue>wsiMSkvDfwb5z+j/bCseAG86U3CZriKYTCEvT6L/T44=</ds:DigestValue>
                    </ds:Reference>
                    </ds:SignedInfo>
                    <ds:SignatureValue>
                    8wMvLxBuATlBdMpYH1i2SolXU4zb7LKnBcqFprVVAv7nq8HsUshtwGaMn4lfT1LfpliWM5aWejcR
                    K8mXfTMUBw==
                    </ds:SignatureValue>
                    <ds:KeyInfo>
                    <ds:X509Data>
                    <ds:X509Certificate>
                    MIIEMDCCA9qgAwIBAgIUIPG1ftMYUFa98/cATOOIEVer+EQwDQYJKoMOAwoBAQECBQAwUzELMAkG
                    A1UEBhMCS1oxRDBCBgNVBAMMO9Kw0JvQotCi0KvSmiDQmtCj05jQm9CQ0J3QlNCr0KDQo9Co0Ksg
                    0J7QoNCi0JDQm9Cr0pogKEdPU1QpMB4XDTE4MDgyMzA0MTEyNVoXDTE5MDgyMzA0MTEyNVowgdsx
                    HjAcBgNVBAMMFdCi0JXQodCi0J7QkiDQotCV0KHQojEVMBMGA1UEBAwM0KLQldCh0KLQntCSMRgw
                    FgYDVQQFEw9JSU4xMjM0NTY3ODkwMTExCzAJBgNVBAYTAktaMRUwEwYDVQQHDAzQkNCb0JzQkNCi
                    0KsxFTATBgNVBAgMDNCQ0JvQnNCQ0KLQqzEYMBYGA1UECgwP0JDQniAi0KLQldCh0KIiMRgwFgYD
                    VQQLDA9CSU4xMjM0NTY3ODkwMjExGTAXBgNVBCoMENCi0JXQodCi0J7QktCY0KcwbDAlBgkqgw4D
                    CgEBAQEwGAYKKoMOAwoBAQEBAQYKKoMOAwoBAwEBAANDAARA01B7RhwfNOTWV/RewpPjuEwRyAG/
                    hmQLgxOCqAS48SmMpOXJ/0sg2WBW1cvhIl9Wk3flv+ZyrdOqlmA86/h0EaOCAeswggHnMA4GA1Ud
                    DwEB/wQEAwIGwDAoBgNVHSUEITAfBggrBgEFBQcDBAYIKoMOAwMEAQIGCSqDDgMDBAECATAPBgNV
                    HSMECDAGgARbanPpMB0GA1UdDgQWBBT4pCIjggm251MRZDG2mwkXjxTp1zBeBgNVHSAEVzBVMFMG
                    ByqDDgMDAgEwSDAhBggrBgEFBQcCARYVaHR0cDovL3BraS5nb3Yua3ovY3BzMCMGCCsGAQUFBwIC
                    MBcMFWh0dHA6Ly9wa2kuZ292Lmt6L2NwczBYBgNVHR8EUTBPME2gS6BJhiJodHRwOi8vY3JsLnBr
                    aS5nb3Yua3ovbmNhX2dvc3QuY3JshiNodHRwOi8vY3JsMS5wa2kuZ292Lmt6L25jYV9nb3N0LmNy
                    bDBcBgNVHS4EVTBTMFGgT6BNhiRodHRwOi8vY3JsLnBraS5nb3Yua3ovbmNhX2RfZ29zdC5jcmyG
                    JWh0dHA6Ly9jcmwxLnBraS5nb3Yua3ovbmNhX2RfZ29zdC5jcmwwYwYIKwYBBQUHAQEEVzBVMC8G
                    CCsGAQUFBzAChiNodHRwOi8vcGtpLmdvdi5rei9jZXJ0L25jYV9nb3N0LmNlcjAiBggrBgEFBQcw
                    AYYWaHR0cDovL29jc3AucGtpLmdvdi5rejANBgkqgw4DCgEBAQIFAANBAHogeaStou18/GXPBmf+
                    YhazmKrTdsu+Uxp2x3FSQURvraDDaSjOBia/pubfyQ5grUh61AML0/t03jrqfmaqEns=
                    </ds:X509Certificate>
                    </ds:X509Data>
                    </ds:KeyInfo>
                    </ds:Signature></Deal>";
            #endregion
            var signXmlRow = model.SUB_FormHistory.OrderByDescending(x => x.Id).FirstOrDefault(x => x.StatusId == 2);
            if (signXmlRow != null && signXmlRow.XmlSign != null)
                AddQrCode(doc, signXmlRow.XmlSign, 7);

            // doc.Save(path + "output1.pdf", Aspose.Words.SaveFormat.Pdf);
            var memoryStream = new MemoryStream();

            doc.Save(memoryStream, SaveFormat.Pdf);
            memoryStream.Position = 0;
            Session[handle] = memoryStream;

            const string applicationtype = "application/pdf";
            return File(memoryStream, applicationtype, juridicalName + ".pdf");
        }

        public ActionResult ASubjectExportToExcelExGu(long id)
        {
            var handle = Guid.NewGuid().ToString();
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/aSubjectTmplGu2.xlsx");

            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            IWorkbook book = new XSSFWorkbook(fiTemplate);
            ISheet sheet = null;
            IRow row = null;
            string year = model.ReportYear.ToString()
                , companyName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;
            CellRangeAddress cellRange = null;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                companyName = model.SEC_User1.JuridicalName;
            int count = 0;
            ISheet sheetSub = null;
            int lastNum = 0;

            #region F-1

            if (model.SEC_User1 != null)
            {
                sheet = book.GetSheetAt(0);
                row = sheet.GetRow(9 - 1);
                row.Cells[1].SetCellValue(model.SEC_User1.BINIIN);
                row.Cells[2].SetCellValue(model.SEC_User1.ApplicationName);
                row.Cells[3].SetCellValue(model.SEC_User1.Address);
                row.Cells[4].SetCellValue(model.SEC_User1.FullName);
                row.Cells[5].SetCellValue(model.SEC_User1.Post);

                string oked = string.Empty;
                oked = model.SEC_User1.DIC_OKED.FullName;
                row.Cells[6].SetCellValue(oked);
                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;


                    row = sheet.GetRow(17);
                    row.Cells[3].SetCellValue(responceFIO);
                    row = sheet.GetRow(18);
                    row.Cells[3].SetCellValue(model.SEC_User1.FullName);
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
            }

            #endregion

            // Ф-2
            #region F-2
            {

                sheet = book.GetSheetAt(1); // Ф-2
                // Заговолок
                row = sheet.GetRow(4);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("_year_", year);
                row.Cells[0].SetCellValue(strValue);
                // Название компании
                row = sheet.GetRow(11);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("companyname", companyName);
                row.Cells[0].SetCellValue(strValue);

                //----form2gu             
                var form2Gu = model.SUB_Form2Gu.FirstOrDefault();
                if (form2Gu != null)
                {
                    row = sheet.GetRow(6);
                    if (form2Gu.CountOfEmployees != null && form2Gu.CountOfEmployees != 0)
                        row.Cells[0].SetCellValue(form2Gu.CountOfEmployees.ToString());

                    row = sheet.GetRow(7);
                    if (form2Gu.CountOfStudents != null && form2Gu.CountOfStudents != 0)
                        row.Cells[0].SetCellValue(form2Gu.CountOfStudents.ToString());

                    row = sheet.GetRow(8);
                    if (form2Gu.CountOfBeds != null && form2Gu.CountOfBeds != 0)
                        row.Cells[0].SetCellValue(form2Gu.CountOfBeds.ToString());
                }

                //----dic resource 1  
                var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);
                var SUB_Form2RecordGuList = new List<SUB_Form2RecordGu>();
                foreach (var rptDicKindWaste in dics)
                {
                    var item = new SUB_Form2RecordGu();
                    item.TypeResourceId = rptDicKindWaste.Id;
                    item.TypeResourceName = rptDicKindWaste.Name;
                    item.TypeResourceUnitName = rptDicKindWaste.DIC_Unit.Name;

                    var form2 = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (form2 != null)
                    {
                        item.Form2RecordId = form2.Id;
                        item.ExpenceEnergy = form2.ExpenceEnergy;
                        item.NotOwnSource = form2.NotOwnSource;
                    }
                    SUB_Form2RecordGuList.Add(item);
                }

                //----dic resource 2
                var kinds = new SubDicKindResourceRepository().GetAll();
                var SUB_Form3RecordGuList = new List<SUB_Form3RecordGu>();
                foreach (var rptDicKindWaste in kinds)
                {
                    var item = new SUB_Form3RecordGu();
                    item.KindResourceId = rptDicKindWaste.Id;
                    item.KindResourceUnitName = rptDicKindWaste.DIC_Unit.Name;
                    item.KindResourceName = rptDicKindWaste.Name;

                    var form3 = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                    if (form3 != null)
                    {
                        item.Form3RecordId = form3.Id;
                        item.ConsumptionPrice = form3.ConsumptionPrice;
                        item.ConsumptionVolume = form3.ConsumptionVolume;
                        item.LosTransportPrice = form3.LosTransportPrice;
                        item.LosTransportVolume = form3.LosTransportVolume;
                    }

                    SUB_Form3RecordGuList.Add(item);
                }

                startRowIndex = 17;
                for (int i = 0; i < SUB_Form2RecordGuList.Count; i++)
                {
                    #region dic 1
                    var rowNum = startRowIndex + i;
                    row = sheet.GetRow(rowNum);
                    // 1
                    row.Cells[0].SetCellValue(i + 1);

                    // 2
                    row.Cells[1].SetCellValue(SUB_Form2RecordGuList[i].TypeResourceName);

                    // 3
                    row.Cells[2].SetCellValue(SUB_Form2RecordGuList[i].TypeResourceUnitName);

                    // 4
                    if (!SUB_Form2RecordGuList[i].NotOwnSource.HasValue)
                        row.Cells[3].SetCellValue(string.Empty);
                    else
                        row.Cells[3].SetCellValue(SUB_Form2RecordGuList[i].NotOwnSource.Value);

                    // 5
                    if (!SUB_Form2RecordGuList[i].ExpenceEnergy.HasValue)
                        row.Cells[4].SetCellValue(string.Empty);
                    else
                        row.Cells[4].SetCellValue(SUB_Form2RecordGuList[i].ExpenceEnergy.Value);
                    #endregion
                }
                startRowIndex = 32;
                for (int i = 0; i < SUB_Form3RecordGuList.Count; i++)
                {
                    #region dic 2
                    var rowNum = startRowIndex + i;
                    row = sheet.GetRow(rowNum);
                    // 1
                    row.Cells[0].SetCellValue(16 + i);

                    // 2                  
                    row.Cells[1].SetCellValue(SUB_Form3RecordGuList[i].KindResourceName);

                    // 3
                    row.Cells[2].SetCellValue(SUB_Form3RecordGuList[i].KindResourceUnitName);

                    // 4
                    if (!SUB_Form3RecordGuList[i].ConsumptionVolume.HasValue)
                        row.Cells[3].SetCellValue(string.Empty);
                    else
                        row.Cells[3].SetCellValue(SUB_Form3RecordGuList[i].ConsumptionVolume.Value);

                    // 5
                    if (!SUB_Form3RecordGuList[i].LosTransportVolume.HasValue)
                        row.Cells[4].SetCellValue(string.Empty);
                    else
                        row.Cells[4].SetCellValue(SUB_Form3RecordGuList[i].LosTransportVolume.Value);
                    #endregion
                }

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(responceFIO))
                {
                    row = sheet.GetRow(38); // 53
                    row.Cells[3].SetCellValue(responceFIO);
                }
                //  Полностью ФИО, подпись руководителя организации
                if (!string.IsNullOrEmpty(headFIO))
                {
                    row = sheet.GetRow(39);
                    row.Cells[3].SetCellValue(headFIO);
                }
            }

            #endregion

            // Ф-3
            #region F-3

            sheet = book.GetSheetAt(2);

            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                sheet.GetRow(4).GetCell(0).SetCellValue("X");
                sheet.GetRow(5).GetCell(0).SetCellValue("");
            }

            //  система энергоменеджмента внедрена или ...
            if (model.IsEnergyManagementSystem == true)
            {
                sheet.GetRow(6).GetCell(0).SetCellValue("X");
                sheet.GetRow(7).GetCell(0).SetCellValue("");
            }

            // Название компании
            row = sheet.GetRow(9);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 14;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
                InsertRows(ref sheet, startRowIndex, count);

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                row = sheet.GetRow(startRowIndex + i);
                row.Cells[0].SetCellValue(i + 1);

                //
                if (record.SUB_DIC_Event != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_Event.NameRu);
                else
                    row.Cells[1].SetCellValue(record.EventName);

                //
                row.Cells[2].SetCellValue(record.EmplPeriodStr);

                //
                if (record.ActualInvest == null)
                    row.Cells[3].SetCellValue(string.Empty);
                else
                    row.Cells[3].SetCellValue(record.ActualInvest.Value);

                //
                if (record.SUB_DIC_TypeCounter == null)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(record.SUB_DIC_TypeCounter.NameRu);

                //
                if (record.InKind == null)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(record.InKind.Value);

                //
                if (record.InMoney == null)
                    row.Cells[6].SetCellValue(string.Empty);
                else
                    row.Cells[6].SetCellValue(record.InMoney.Value);
            }

            // copy footer
            sheetSub = book.GetSheetAt(3);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 2);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 3, 4);
                    sheet.AddMergedRegion(cellRange);
                }
            }

            book.RemoveSheetAt(3);

            //Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + 8);
                row.Cells[3].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + 9);
                row.Cells[3].SetCellValue(headFIO);
            }
            #endregion

            // Ф-4
            #region F-4
            sheet = book.GetSheetAt(3);

            // Заговолок
            row = sheet.GetRow(2);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("_year_", year);
            row.Cells[0].SetCellValue(strValue);

            var form3Gu = model.SUB_Form3Gu.FirstOrDefault();
            if (form3Gu != null)
            {
                #region form3gu
                //
                row = sheet.GetRow(5);
                if (form3Gu.YearOfConstruction != null)
                    row.Cells[0].SetCellValue(form3Gu.YearOfConstruction.ToString());

                //
                row = sheet.GetRow(6);
                if (form3Gu.AutomateItem != null)
                {
                    if (form3Gu.AutomateItem == 1)
                        row.Cells[0].SetCellValue("да");
                    else row.Cells[0].SetCellValue("нет");
                }

                //
                row = sheet.GetRow(7);
                if (form3Gu.TotalAreaOfBuilding != null)
                    row.Cells[0].SetCellValue(form3Gu.TotalAreaOfBuilding.ToString());

                //
                row = sheet.GetRow(8);
                if (form3Gu.HeatedAreaOfBuilding != null)
                    row.Cells[0].SetCellValue(form3Gu.HeatedAreaOfBuilding.ToString());

                // центральное отопление
                if (form3Gu.CentralHeating != null && form3Gu.CentralHeating == 1)
                {
                    row = sheet.GetRow(11);
                    row.Cells[0].SetCellValue("X");
                }
                // автономное отопление
                if (form3Gu.IndependentHeating != null && form3Gu.IndependentHeating == 1)
                {
                    row = sheet.GetRow(12);
                    row.Cells[0].SetCellValue("X");
                }
                #endregion
            }

            // Название компании
            row = sheet.GetRow(14);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            // get data for 
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 18;
            count = model.SubForm5Records.Count;
            if (count > 1)
                InsertRows(ref sheet, startRowIndex, count);

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];
                row = sheet.GetRow(startRowIndex + i);
                row.Cells[0].SetCellValue(i + 1);

                if (record.sub_dic_energyindicator != null)
                    row.Cells[1].SetCellValue(record.sub_dic_energyindicator.nameru);
                else
                    row.Cells[1].SetCellValue(record.IndicatorName);

                row.Cells[2].SetCellValue(record.UnitMeasure);
                row.Cells[3].SetCellValue(record.CalcFormula);

                if (record.EnergyValue == null)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(record.EnergyValue.Value);
            }

            // copy footer
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 2);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 3, 4);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(4);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[3].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[3].SetCellValue(headFIO);
            }

            #endregion

            // Ф-5
            #region F-5
            sheet = book.GetSheetAt(4);

            // Название компании
            row = sheet.GetRow(1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            startRowIndex = 7;
            count = model.SubForm6Records.Count;
            if (count > 1)
                InsertRows(ref sheet, startRowIndex, count);

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];
                row = sheet.GetRow(startRowIndex + i);
                row.Cells[0].SetCellValue(i + 1);

                if (record.SUB_DIC_TypeCounter != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_TypeCounter.NameRu);
                else
                    row.Cells[1].SetCellValue(String.Empty);

                if (record.CountDevice != null)
                    row.Cells[2].SetCellValue(record.CountDevice.Value);
                else
                    row.Cells[2].SetCellValue(string.Empty);

                if (record.Equipment != null)
                    row.Cells[3].SetCellValue(record.Equipment.Value);
                else
                    row.Cells[3].SetCellValue(string.Empty);
            }

            sheetSub = book.GetSheetAt(5);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 2, 3);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(5);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[2].SetCellValue(headFIO);
            }

            #endregion

            HSSFFormulaEvaluator.EvaluateAllFormulaCells(book);

            using (var memoryStream = new MemoryStream())
            {
                book.Write(memoryStream);
                Session[handle] = memoryStream.ToArray();
            }

            #region check companyName
            while (companyName.IndexOf("\"") != -1)
            {
                companyName = companyName.Replace("\"", "");
            }

            while (companyName.IndexOf("|") != -1)
            {
                companyName = companyName.Replace("|", "");
            }

            while (companyName.IndexOf("?") != -1)
            {
                companyName = companyName.Replace("?", "");
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                if (companyName.Length > 100)
                    companyName = companyName.Substring(0, 99);
            }
            #endregion

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = companyName + ".xlsx" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ASubjectExportToPdfGu(long id)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/");
            Aspose.Words.License lis = new Aspose.Words.License();
            Aspose.Words.License lisdocs = new Aspose.Words.License();
            lis.SetLicense(path + "Aspose.Words.lic");

            var doc = new Aspose.Words.Document(path + "aSubjectTmplWordGu.docx");
            var handle = Guid.NewGuid().ToString();


            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();

            var model = repository.GetById(id);

            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            string year = model.ReportYear.ToString()
                , juridicalName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                juridicalName = model.SEC_User1.JuridicalName;

            int count = 0;
            #region F-1
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 0, true);
            if (model.SEC_User1 != null)
            {

                table.Rows[2].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.BINIIN));
                table.Rows[2].Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.ApplicationName));
                if (model.SEC_User1.Address != null)
                    table.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Address));

                table.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));

                if (model.SEC_User1.Post != null)
                    table.Rows[2].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Post));

                string oked = string.Empty;
                oked = (model.SEC_User1.DIC_OKED != null) ? model.SEC_User1.DIC_OKED.FullName : "";
                table.Rows[2].Cells[6].FirstParagraph.AppendChild(new Run(doc, oked));

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                Aspose.Words.Tables.Table table6 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 5, true);
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;


                    table6.Rows[0].Cells[1].FirstParagraph.AppendChild(new Run(doc, responceFIO));
                    table6.Rows[1].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
            }

            #endregion

            FindReplaceOptions replaceOptions = new FindReplaceOptions();
            replaceOptions.MatchCase = true;
            replaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("_year_", year, replaceOptions);
            doc.Range.Replace("juridical_name", juridicalName, replaceOptions);

            if (model.IsRent == true)
                doc.Range.Replace("_isarenda", "x", replaceOptions);
            else doc.Range.Replace("_isarenda", " ", replaceOptions);

            // Ф-2
            #region F-2
            {
                Aspose.Words.Tables.Table table2 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 1, true);
                //----form2gu             
                var form2Gu = model.SUB_Form2Gu.FirstOrDefault();
                if (form2Gu != null)
                {
                    if (form2Gu.CountOfEmployees != null && form2Gu.CountOfEmployees != 0)
                        doc.Range.Replace("_countOfEmployees", form2Gu.CountOfEmployees.ToString(), replaceOptions);
                    else doc.Range.Replace("_countOfEmployees", "", replaceOptions);

                    if (form2Gu.CountOfStudents != null && form2Gu.CountOfStudents != 0)
                        doc.Range.Replace("_students", form2Gu.CountOfStudents.ToString(), replaceOptions);
                    else doc.Range.Replace("_students", "", replaceOptions);

                    if (form2Gu.CountOfBeds != null && form2Gu.CountOfBeds != 0)
                        doc.Range.Replace("_countOfBeds", form2Gu.CountOfBeds.ToString(), replaceOptions);
                    else doc.Range.Replace("_countOfBeds", "", replaceOptions);
                }
                else
                {
                    doc.Range.Replace("_countOfEmployees", "", replaceOptions);
                    doc.Range.Replace("_students", "", replaceOptions);
                    doc.Range.Replace("_countOfBeds", "", replaceOptions);
                }

                //----dic resource 1  
                var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);
                var SUB_Form2RecordGuList = new List<SUB_Form2RecordGu>();
                foreach (var rptDicKindWaste in dics)
                {
                    var item = new SUB_Form2RecordGu();
                    item.TypeResourceId = rptDicKindWaste.Id;
                    item.TypeResourceName = rptDicKindWaste.Name;
                    item.TypeResourceUnitName = rptDicKindWaste.DIC_Unit.Name;

                    var form2 = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (form2 != null)
                    {
                        item.Form2RecordId = form2.Id;
                        item.ExpenceEnergy = form2.ExpenceEnergy;
                        item.NotOwnSource = form2.NotOwnSource;
                    }
                    SUB_Form2RecordGuList.Add(item);
                }

                //----dic resource 2
                var kinds = new SubDicKindResourceRepository().GetAll();
                var SUB_Form3RecordGuList = new List<SUB_Form3RecordGu>();
                foreach (var rptDicKindWaste in kinds)
                {
                    var item = new SUB_Form3RecordGu();
                    item.KindResourceId = rptDicKindWaste.Id;
                    item.KindResourceUnitName = rptDicKindWaste.DIC_Unit.Name;
                    item.KindResourceName = rptDicKindWaste.Name;

                    var form3 = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                    if (form3 != null)
                    {
                        item.Form3RecordId = form3.Id;
                        item.ConsumptionPrice = form3.ConsumptionPrice;
                        item.ConsumptionVolume = form3.ConsumptionVolume;
                        item.LosTransportPrice = form3.LosTransportPrice;
                        item.LosTransportVolume = form3.LosTransportVolume;
                    }

                    SUB_Form3RecordGuList.Add(item);
                }

                startRowIndex = 17;
                for (int i = 0; i < SUB_Form2RecordGuList.Count; i++)
                {
                    #region dic 1

                    // 1
                    table2.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                    // 2
                    table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].TypeResourceName));

                    // 3
                    table2.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].TypeResourceUnitName));

                    // 4
                    if (!SUB_Form2RecordGuList[i].NotOwnSource.HasValue)
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].NotOwnSource.Value.ToString()));

                    // 5
                    if (!SUB_Form2RecordGuList[i].ExpenceEnergy.HasValue)
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].ExpenceEnergy.Value.ToString()));
                    #endregion
                }
                startRowIndex = 32;
                for (int i = 0; i < SUB_Form3RecordGuList.Count; i++)
                {
                    #region dic 2
                    // 1
                    table2.Rows[17 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (16 + i).ToString()));

                    // 2                  
                    table2.Rows[17 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].KindResourceName));

                    // 3
                    table2.Rows[17 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].KindResourceUnitName));

                    // 4
                    if (!SUB_Form3RecordGuList[i].ConsumptionVolume.HasValue)
                        table2.Rows[17 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[17 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].ConsumptionVolume.Value.ToString()));

                    // 5
                    if (!SUB_Form3RecordGuList[i].LosTransportVolume.HasValue)
                        table2.Rows[17 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[17 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].LosTransportVolume.Value.ToString()));
                    #endregion
                }

            }

            #endregion

            // Ф-3
            #region F-3
            Aspose.Words.Tables.Table table3 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);

            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                doc.Range.Replace("_isplan", "x", replaceOptions);
                doc.Range.Replace("_isnotplan", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isplan", "", replaceOptions);
                doc.Range.Replace("_isnotplan", "x", replaceOptions);
            }

            //  система энергоменеджмента внедрена или ...
            if (model.IsEnergyManagementSystem == true)
            {
                doc.Range.Replace("_isenergymanagementsystem", "x", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isenergymanagementsystem", "", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "x", replaceOptions);
            }

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 14;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table3.LastRow.Clone(true);
                    table3.Rows.Insert(i + 3, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                table3.Rows[3 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                //
                if (record.SUB_DIC_Event != null)
                    table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_Event.NameRu));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.EventName))
                        table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EventName));
                }

                //
                if (record.EmplPeriodStr != null)
                    table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.EmplPeriodStr));
                else table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                //
                if (record.ActualInvest != null)
                    table3.Rows[3 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.ActualInvest.Value.ToString()));

                //
                if (record.SUB_DIC_TypeCounter != null)
                    table3.Rows[3 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));

                //
                if (record.InKind != null)
                    table3.Rows[3 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.InKind.Value.ToString()));

                //
                if (record.InMoney != null)
                    table3.Rows[3 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, record.InMoney.Value.ToString()));
            }

            #endregion

            // Ф-4
            #region F-4
            Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            var form3Gu = model.SUB_Form3Gu.FirstOrDefault();
            if (form3Gu != null)
            {
                #region form3gu       

                if (form3Gu.YearOfConstruction != null)
                    doc.Range.Replace("_yearOfConstruction", form3Gu.YearOfConstruction.ToString(), replaceOptions);
                else doc.Range.Replace("_yearOfConstruction", "", replaceOptions);

                //
                if (form3Gu.AutomateItem != null)
                {
                    if (form3Gu.AutomateItem == 1)
                        doc.Range.Replace("_automateItem", "да", replaceOptions);
                    else doc.Range.Replace("_automateItem", "нет", replaceOptions);
                }
                else
                {
                    doc.Range.Replace("_automateItem", "нет", replaceOptions);
                }

                //
                if (form3Gu.TotalAreaOfBuilding != null)
                    doc.Range.Replace("_totalAreaOfBuilding", form3Gu.TotalAreaOfBuilding.ToString(), replaceOptions);
                else doc.Range.Replace("_totalAreaOfBuilding", "", replaceOptions);

                //
                if (form3Gu.HeatedAreaOfBuilding != null)
                    doc.Range.Replace("_heatedAreaOfBuilding", form3Gu.HeatedAreaOfBuilding.ToString(), replaceOptions);
                else doc.Range.Replace("_heatedAreaOfBuilding", "", replaceOptions);

                // центральное отопление
                if (form3Gu.CentralHeating != null && form3Gu.CentralHeating == 1)
                {
                    doc.Range.Replace("_central", "x", replaceOptions);
                }
                else doc.Range.Replace("_central", "", replaceOptions);

                // автономное отопление
                if (form3Gu.IndependentHeating != null && form3Gu.IndependentHeating == 1)
                {
                    doc.Range.Replace("_independent", "x", replaceOptions);
                }
                else doc.Range.Replace("_independent", "", replaceOptions);
                #endregion
            }
            else
            {
                doc.Range.Replace("_yearOfConstruction", "", replaceOptions);
                doc.Range.Replace("_automateItem", "нет", replaceOptions);
                doc.Range.Replace("_totalAreaOfBuilding", "", replaceOptions);
                doc.Range.Replace("_heatedAreaOfBuilding", "", replaceOptions);
                doc.Range.Replace("_central", "", replaceOptions);
                doc.Range.Replace("_independent", "", replaceOptions);
            }

            // get data for 
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 18;
            count = model.SubForm5Records.Count;
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table4.LastRow.Clone(true);
                    table4.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];

                table4.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.sub_dic_energyindicator != null)
                    table4.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.IndicatorName))
                        table4.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));
                }

                if (record.UnitMeasure != null)
                    table4.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));
                else table4.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                if (record.CalcFormula != null)
                    table4.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));
                else table4.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));


                if (record.EnergyValue == null)
                    table4.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                else
                    table4.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));
            }

            #endregion

            // Ф-5
            #region F-5
            Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            count = model.SubForm6Records.Count;
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table5.LastRow.Clone(true);
                    table5.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];

                table5.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.SUB_DIC_TypeCounter != null)
                    table5.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));
                else
                    table5.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, ""));

                if (record.CountDevice != null)
                    table5.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountDevice.Value.ToString()));
                else
                    table5.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, ""));

                if (record.Equipment != null)
                    table5.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Equipment.Value.ToString()));
            }


            #endregion


            #region check companyName
            while (juridicalName.IndexOf("\"") != -1)
            {
                juridicalName = juridicalName.Replace("\"", "");
            }

            while (juridicalName.IndexOf("|") != -1)
            {
                juridicalName = juridicalName.Replace("|", "");
            }

            while (juridicalName.IndexOf("?") != -1)
            {
                juridicalName = juridicalName.Replace("?", "");
            }

            while (juridicalName.IndexOf(":") != -1)
            {
                juridicalName = juridicalName.Replace(":", "");
            }

            if (!string.IsNullOrWhiteSpace(juridicalName))
            {
                if (juridicalName.Length > 100)
                    juridicalName = juridicalName.Substring(0, 99);
            }
            #endregion

            var signXmlRow = model.SUB_FormHistory.OrderByDescending(x => x.Id).FirstOrDefault(x => x.StatusId == 2);
            if (signXmlRow != null && signXmlRow.XmlSign != null)
                AddQrCode(doc, signXmlRow.XmlSign, 6);

            // doc.Save(path + "output1.pdf", Aspose.Words.SaveFormat.Pdf);
            var memoryStream = new MemoryStream();

            doc.Save(memoryStream, SaveFormat.Pdf);
            memoryStream.Position = 0;
            Session[handle] = memoryStream;

            string _fileName = model.SEC_User1.DIC_Kato.NameRu.Substring(0, 5) + "_" + model.SubjectIDK + "_" + model.SEC_User1.BINIIN + "_" + juridicalName;
            //----
            var dirpath = Server.MapPath("~/uploads/pdf/2018/");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            doc.Save(dirpath + "/" + _fileName + ".pdf", SaveFormat.Pdf);


            const string applicationtype = "application/pdf";
            return File(memoryStream, applicationtype, _fileName + ".pdf");
        }

        public ActionResult ASubjectExportToExcelExGuNew(long id)
        {
            var handle = Guid.NewGuid().ToString();
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/aSubjectTmplGuNew.xlsx");

            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            IWorkbook book = new XSSFWorkbook(fiTemplate);
            ISheet sheet = null;
            IRow row = null;
            string year = model.ReportYear.ToString()
                , companyName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;
            CellRangeAddress cellRange = null;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                companyName = model.SEC_User1.JuridicalName;
            int count = 0;
            ISheet sheetSub = null;
            int lastNum = 0;

            #region F-1

            if (model.SEC_User1 != null)
            {
                sheet = book.GetSheetAt(0);
                row = sheet.GetRow(9 - 1);
                row.Cells[1].SetCellValue(model.SEC_User1.BINIIN);
                row.Cells[2].SetCellValue(model.SEC_User1.ApplicationName);
                row.Cells[3].SetCellValue(model.SEC_User1.Address);
                row.Cells[4].SetCellValue(model.SEC_User1.FullName);
                row.Cells[5].SetCellValue(model.SEC_User1.Post);

                string oked = string.Empty;
                oked = model.SEC_User1.DIC_OKED.FullName;
                row.Cells[6].SetCellValue(oked);
                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;


                    row = sheet.GetRow(17);
                    row.Cells[3].SetCellValue(responceFIO);
                    row = sheet.GetRow(18);
                    row.Cells[3].SetCellValue(model.SEC_User1.FullName);
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
            }

            #endregion

            // Ф-2
            #region F-2
            {

                sheet = book.GetSheetAt(1); // Ф-2
                // Заговолок
                row = sheet.GetRow(4);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("_year_", year);
                row.Cells[0].SetCellValue(strValue);
                // Название компании
                row = sheet.GetRow(8);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("companyname", companyName);
                row.Cells[0].SetCellValue(strValue);

                // Init 
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                startRowIndex = 12;
                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    //row = sheet.CreateRow(1);
                    var rowNum = startRowIndex + i;
                    row = sheet.GetRow(rowNum);

                    // 0
                    row.Cells[0].SetCellValue(i + 1);

                    // 1
                    var typeResourceName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu;
                    row.Cells[1].SetCellValue(typeResourceName);

                    // 2
                    var unitName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                                   || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu;
                    row.Cells[2].SetCellValue(unitName);

                    // 3
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        row.Cells[3].SetCellValue(string.Empty);
                    else
                        row.Cells[3].SetCellValue(model.SubForm2Records[i].NotOwnSource.Value);

                    // 4
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        row.Cells[4].SetCellValue(string.Empty);
                    else
                        row.Cells[4].SetCellValue(model.SubForm2Records[i].ExpenceEnergy.Value);

                    // 5
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        row.Cells[5].SetCellValue(string.Empty);
                    else
                        row.Cells[5].SetCellValue(model.SubForm2Records[i].ExtractVolume.Value);

                    // 6
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        row.Cells[6].SetCellValue(string.Empty);
                    else
                        row.Cells[6].SetCellValue(model.SubForm2Records[i].TransferOtherLegal.Value);

                }

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(responceFIO))
                {
                    row = sheet.GetRow(43); // 53
                    row.Cells[3].SetCellValue(responceFIO);
                }
                //  Полностью ФИО, подпись руководителя организации
                if (!string.IsNullOrEmpty(headFIO))
                {
                    row = sheet.GetRow(44);
                    row.Cells[3].SetCellValue(headFIO);
                }
            }

            #endregion

            // Ф-3
            #region F-3
            sheet = book.GetSheetAt(2);
            startRowIndex = 5;
            if (model.SUB_Form3GuRecord != null)
            {
                count = model.SUB_Form3GuRecord.Count;
                if (count > 1)
                    InsertRows(ref sheet, startRowIndex, count - 1);

                foreach (var item in model.SUB_Form3GuRecord)
                {
                    row = sheet.GetRow(startRowIndex);

                    // 0
                    row.Cells[0].SetCellValue((startRowIndex - 4));

                    // 1
                    if (item.CountOfBuildings == null)
                        row.Cells[1].SetCellValue(string.Empty);
                    else
                        row.Cells[1].SetCellValue(Convert.ToDouble(item.CountOfBuildings));

                    // 2
                    if (item.YearOfConstruction == null)
                        row.Cells[2].SetCellValue(string.Empty);
                    else
                        row.Cells[2].SetCellValue(Convert.ToDouble(item.YearOfConstruction));

                    // 3
                    if (item.AutomateItem == null)
                    {
                        row.Cells[3].SetCellValue("Нет");
                    }
                    else
                    {
                        if (item.AutomateItem == 1)
                        {
                            row.Cells[3].SetCellValue("Да");
                        }
                        else
                        {
                            row.Cells[3].SetCellValue("Нет");
                        }
                    }

                    // 4
                    if (item.TotalAreaOfBuilding == null)
                        row.Cells[4].SetCellValue(string.Empty);
                    else
                        row.Cells[4].SetCellValue(Convert.ToDouble(item.TotalAreaOfBuilding));

                    // 5
                    if (item.HeatedAreaOfBuilding == null)
                        row.Cells[5].SetCellValue(string.Empty);
                    else
                        row.Cells[5].SetCellValue(Convert.ToDouble(item.HeatedAreaOfBuilding));

                    // 6
                    if (item.CountOfEmployees == null)
                        row.Cells[6].SetCellValue(string.Empty);
                    else
                        row.Cells[6].SetCellValue(Convert.ToDouble(item.CountOfEmployees));

                    // 7
                    if (item.CountOfStudents == null)
                        row.Cells[7].SetCellValue(string.Empty);
                    else
                        row.Cells[7].SetCellValue(Convert.ToDouble(item.CountOfStudents));

                    // 8
                    if (item.CountOfBeds == null)
                        row.Cells[8].SetCellValue(string.Empty);
                    else
                        row.Cells[8].SetCellValue(Convert.ToDouble(item.CountOfBeds));

                    startRowIndex++;
                }
            }

            //----form 4
            sheet = book.GetSheetAt(3);
            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                sheet.GetRow(5).GetCell(0).SetCellValue("X");
                sheet.GetRow(6).GetCell(0).SetCellValue("");
            }

            //  система энергоменеджмента внедрена или ...
            if (model.IsEnergyManagementSystem == true)
            {
                sheet.GetRow(7).GetCell(0).SetCellValue("X");
                sheet.GetRow(8).GetCell(0).SetCellValue("");
            }

            // Название компании
            row = sheet.GetRow(10);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            int startRowIndex31 = 15;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
                InsertRows(ref sheet, startRowIndex31, count - 1);

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                row = sheet.GetRow(startRowIndex31 + i);
                row.Cells[0].SetCellValue(i + 1);

                // 1
                if (record.SUB_DIC_Event != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_Event.NameRu);
                else
                    row.Cells[1].SetCellValue(record.EventName);

                // 2
                row.Cells[2].SetCellValue(record.EmplPeriodStr);

                // 3
                if (record.ActualInvest == null)
                    row.Cells[3].SetCellValue(string.Empty);
                else
                    row.Cells[3].SetCellValue(record.ActualInvest.Value);

                // 4
                if (record.SUB_DIC_TypeCounter == null)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(record.SUB_DIC_TypeCounter.NameRu);

                // 5
                if (record.InKind == null)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(record.InKind.Value);

                // 6
                if (record.InMoney == null)
                    row.Cells[6].SetCellValue(string.Empty);
                else
                    row.Cells[6].SetCellValue(record.InMoney.Value);
            }
            startRowIndex31 += count;

            // copy
            sheet = book.GetSheetAt(2);
            sheetSub = book.GetSheetAt(3);
            lastNum = sheetSub.LastRowNum;
            count = 0;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                if (rowSource == null)
                    continue;

                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    if (sourceCell != null)
                    {
                        if (sourceCell.CellType == CellType.Numeric)
                        {
                            destCell.SetCellValue(sourceCell.NumericCellValue);
                        }
                        else
                        {
                            destCell.SetCellValue(sourceCell.StringCellValue);
                        }

                        destCell.CellStyle = sourceCell.CellStyle;
                    }
                }

                if (i == 2)
                {
                    cellRange = new CellRangeAddress(row.RowNum - 1, row.RowNum, 0, 6);
                    sheet.AddMergedRegion(cellRange);
                }

                if (i == 13)
                {
                    cellRange = new CellRangeAddress(row.RowNum - 1, row.RowNum, 0, 0);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum - 1, row.RowNum, 1, 1);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum - 1, row.RowNum, 2, 2);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum - 1, row.RowNum, 3, 3);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum - 1, row.RowNum - 1, 4, 6);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            startRowIndex += startRowIndex31;
            book.RemoveSheetAt(3);

            // footer
            startRowIndex += 2;
            sheet = book.GetSheetAt(3);
            //Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(2);
                row.Cells[2].SetCellValue(headFIO);
            }

            // copy footer
            sheet = book.GetSheetAt(2);
            sheetSub = book.GetSheetAt(3);
            lastNum = sheetSub.LastRowNum;
            count = 0;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                if (rowSource == null)
                    continue;

                row = sheet.CreateRow(startRowIndex + i);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    if (sourceCell != null)
                    {
                        if (sourceCell.CellType == CellType.Numeric)
                        {
                            destCell.SetCellValue(sourceCell.NumericCellValue);
                        }
                        else
                        {
                            destCell.SetCellValue(sourceCell.StringCellValue);
                        }

                        destCell.CellStyle = sourceCell.CellStyle;
                    }
                }

                if (i == 1 || i == 2)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 2, 3);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(3);
            #endregion

            #region Ф-3а
            //---1
            sheet = book.GetSheetAt(3);
            if (model.SUB_Form5Record != null)
            {
                startRowIndex = 6;
                count = model.SUB_Form5Record.Count;
                if (count > 1)
                    InsertRows(ref sheet, startRowIndex, count - 1);

                foreach (var record in model.SUB_Form5Record.OrderBy(e => e.Id))
                {

                    row = sheet.GetRow(startRowIndex);
                    row.Cells[0].SetCellValue(startRowIndex - 5);

                    if (record.TypeOfHeating == null)
                    {
                        row.Cells[1].SetCellValue(string.Empty);
                    }
                    else
                    {
                        if (record.TypeOfHeating == 1)
                        {
                            row.Cells[1].SetCellValue("Центральное отопление");
                        }
                        else if (record.TypeOfHeating == 2)
                        {
                            row.Cells[1].SetCellValue("Автономное отопление");
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                    }

                    if (record.sub_dic_energyindicator != null)
                        row.Cells[2].SetCellValue(record.sub_dic_energyindicator.nameru);
                    else
                        row.Cells[2].SetCellValue(record.IndicatorName);

                    row.Cells[3].SetCellValue(record.UnitMeasure);
                    row.Cells[4].SetCellValue(record.CalcFormula);

                    if (record.EnergyValue == null)
                        row.Cells[5].SetCellValue(string.Empty);
                    else
                        row.Cells[5].SetCellValue(record.EnergyValue.Value);

                    startRowIndex++;
                }

            }

            //----2
            sheet = book.GetSheetAt(4);
            int startRowIndex2 = 6;
            if (model.SUB_Form3aGuRecord1 != null)
            {

                count = model.SUB_Form3aGuRecord1.Count;
                if (count > 1)
                    InsertRows(ref sheet, startRowIndex2, count - 1);
                foreach (var record in model.SUB_Form3aGuRecord1.OrderBy(e => e.Id))
                {
                    row = sheet.GetRow(startRowIndex2);
                    row.Cells[0].SetCellValue(startRowIndex2 - 5);

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(record.DIC_GU.NameRu);
                        }
                    }
                    else
                    {
                        if (record.SourceName == null)
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(record.SourceName);
                        }
                    }

                    // 2
                    if (record.CountOfHeatingSources == null)
                    {
                        row.Cells[2].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[2].SetCellValue(Convert.ToDouble(record.CountOfHeatingSources));
                    }

                    // 3
                    if (record.CoefficientOfPerformance == null)
                    {
                        row.Cells[3].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[3].SetCellValue(Convert.ToDouble(record.CoefficientOfPerformance));
                    }

                    // 4
                    if (record.PowerOfHeatingSources == null)
                    {
                        row.Cells[4].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[4].SetCellValue(Convert.ToDouble(record.PowerOfHeatingSources));
                    }

                    // 5
                    if (record.YearOfCommissioning == null)
                    {
                        row.Cells[5].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[5].SetCellValue(Convert.ToDouble(record.YearOfCommissioning));
                    }

                    startRowIndex2++;
                }
            }

            // copy
            sheet = book.GetSheetAt(3);
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            count = 0;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                if (rowSource == null)
                    continue;

                row = sheet.CreateRow(startRowIndex + i);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    if (sourceCell != null)
                    {
                        if (sourceCell.CellType == CellType.Numeric)
                        {
                            destCell.SetCellValue(sourceCell.NumericCellValue);
                        }
                        else
                        {
                            destCell.SetCellValue(sourceCell.StringCellValue);
                        }

                        destCell.CellStyle = sourceCell.CellStyle;
                    }
                }

                if (i == 1)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                    sheet.AddMergedRegion(cellRange);
                }

            }
            startRowIndex += startRowIndex2;
            book.RemoveSheetAt(4);

            //----3
            sheet = book.GetSheetAt(4);
            int startRowIndex3 = 6;
            if (model.SUB_Form3aGuRecord2 != null)
            {

                count = model.SUB_Form3aGuRecord2.Count;
                if (count > 1)
                    InsertRows(ref sheet, startRowIndex3, count - 1);
                foreach (var record in model.SUB_Form3aGuRecord2.OrderBy(e => e.Id))
                {
                    row = sheet.GetRow(startRowIndex3);
                    row.Cells[0].SetCellValue(startRowIndex3 - 5);

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(record.DIC_GU.NameRu);
                        }
                    }
                    else
                    {
                        if (record.DeviceName == null)
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(record.DeviceName);
                        }
                    }

                    // 2
                    if (record.Amount == null)
                    {
                        row.Cells[2].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[2].SetCellValue(Convert.ToDouble(record.Amount));
                    }

                    // 3
                    if (record.Power == null)
                    {
                        row.Cells[3].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[3].SetCellValue(Convert.ToDouble(record.Power));
                    }

                    // 4
                    if (record.HoursPerDay == null)
                    {
                        row.Cells[4].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[4].SetCellValue(Convert.ToDouble(record.HoursPerDay));
                    }

                    startRowIndex3++;
                }
            }
            // copy
            sheet = book.GetSheetAt(3);
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            count = 0;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                if (rowSource == null)
                    continue;

                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    if (sourceCell != null)
                    {
                        if (sourceCell.CellType == CellType.Numeric)
                        {
                            destCell.SetCellValue(sourceCell.NumericCellValue);
                        }
                        else
                        {
                            destCell.SetCellValue(sourceCell.StringCellValue);
                        }

                        destCell.CellStyle = sourceCell.CellStyle;
                    }
                }

                if (i == 1)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 5);
                    sheet.AddMergedRegion(cellRange);
                }

            }
            startRowIndex += startRowIndex3;
            book.RemoveSheetAt(4);

            //----4
            sheet = book.GetSheetAt(4);
            int startRowIndex4 = 6;
            if (model.SUB_Form3aGuRecord3 != null)
            {
                count = model.SUB_Form3aGuRecord3.Count;
                if (count > 1)
                    InsertRows(ref sheet, startRowIndex4, count - 1);
                foreach (var record in model.SUB_Form3aGuRecord3.OrderBy(e => e.Id))
                {
                    row = sheet.GetRow(startRowIndex4);
                    row.Cells[0].SetCellValue(startRowIndex4 - 5);

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(record.DIC_GU.NameRu);
                        }
                    }
                    else
                    {
                        if (record.EnergyConsumEquipName == null)
                        {
                            row.Cells[1].SetCellValue(string.Empty);
                        }
                        else
                        {
                            row.Cells[1].SetCellValue(record.EnergyConsumEquipName);
                        }
                    }

                    // 2
                    if (record.Amount == null)
                    {
                        row.Cells[2].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[2].SetCellValue(Convert.ToDouble(record.Amount));
                    }

                    // 3
                    if (record.Power == null)
                    {
                        row.Cells[3].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[3].SetCellValue(Convert.ToDouble(record.Power));
                    }

                    // 4
                    if (record.HoursPerDay == null)
                    {
                        row.Cells[4].SetCellValue(string.Empty);
                    }
                    else
                    {
                        row.Cells[4].SetCellValue(Convert.ToDouble(record.HoursPerDay));
                    }

                    startRowIndex4++;
                }
            }
            // copy
            sheet = book.GetSheetAt(3);
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            count = 0;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                if (rowSource == null)
                    continue;

                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    if (sourceCell != null)
                    {
                        if (sourceCell.CellType == CellType.Numeric)
                        {
                            destCell.SetCellValue(sourceCell.NumericCellValue);
                        }
                        else
                        {
                            destCell.SetCellValue(sourceCell.StringCellValue);
                        }

                        destCell.CellStyle = sourceCell.CellStyle;
                    }
                }

                if (i == 1)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 5);
                    sheet.AddMergedRegion(cellRange);
                }

            }
            startRowIndex += startRowIndex4;
            book.RemoveSheetAt(4);


            // footer
            startRowIndex += 2;
            sheet = book.GetSheetAt(4);
            //Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(2);
                row.Cells[2].SetCellValue(headFIO);
            }

            // copy footer
            sheet = book.GetSheetAt(3);
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            count = 0;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                if (rowSource == null)
                    continue;

                row = sheet.CreateRow(startRowIndex + i);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    if (sourceCell != null)
                    {
                        if (sourceCell.CellType == CellType.Numeric)
                        {
                            destCell.SetCellValue(sourceCell.NumericCellValue);
                        }
                        else
                        {
                            destCell.SetCellValue(sourceCell.StringCellValue);
                        }

                        destCell.CellStyle = sourceCell.CellStyle;
                    }
                }

                if (i == 1 || i == 2)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 2, 3);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(4);
            #endregion

            HSSFFormulaEvaluator.EvaluateAllFormulaCells(book);

            using (var memoryStream = new MemoryStream())
            {
                book.Write(memoryStream);
                Session[handle] = memoryStream.ToArray();
            }

            #region check companyName
            while (companyName.IndexOf("\"") != -1)
            {
                companyName = companyName.Replace("\"", "");
            }

            while (companyName.IndexOf("|") != -1)
            {
                companyName = companyName.Replace("|", "");
            }

            while (companyName.IndexOf("?") != -1)
            {
                companyName = companyName.Replace("?", "");
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                if (companyName.Length > 100)
                    companyName = companyName.Substring(0, 99);
            }
            #endregion

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = companyName + ".xlsx" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ASubjectExportToPdfGuNew(long id)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/");
            Aspose.Words.License lis = new Aspose.Words.License();
            Aspose.Words.License lisdocs = new Aspose.Words.License();
            lis.SetLicense(path + "Aspose.Words.lic");

            var doc = new Aspose.Words.Document(path + "aSubjectTmplWordGuNew.docx");
            var handle = Guid.NewGuid().ToString();


            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();

            var model = repository.GetById(id);
            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            string year = model.ReportYear.ToString()
                , juridicalName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                juridicalName = model.SEC_User1.JuridicalName;

            int count = 0;
            #region F-1
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 0, true);
            if (model.SEC_User1 != null)
            {

                table.Rows[2].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.BINIIN));
                table.Rows[2].Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.ApplicationName));
                if (model.SEC_User1.Address != null)
                    table.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Address));

                table.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));

                if (model.SEC_User1.Post != null)
                    table.Rows[2].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Post));

                string oked = string.Empty;
                oked = (model.SEC_User1.DIC_OKED != null) ? model.SEC_User1.DIC_OKED.FullName : "";
                table.Rows[2].Cells[6].FirstParagraph.AppendChild(new Run(doc, oked));

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                Aspose.Words.Tables.Table table8 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 8, true);
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;


                    table8.Rows[0].Cells[1].FirstParagraph.AppendChild(new Run(doc, responceFIO));
                    table8.Rows[1].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
            }

            #endregion

            FindReplaceOptions replaceOptions = new FindReplaceOptions();
            replaceOptions.MatchCase = true;
            replaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("_year_", year, replaceOptions);
            doc.Range.Replace("juridical_name", juridicalName, replaceOptions);

            if (model.IsRent == true)
                doc.Range.Replace("_isarenda", "x", replaceOptions);
            else doc.Range.Replace("_isarenda", " ", replaceOptions);

            // Ф-2
            #region F-2
            {
                Aspose.Words.Tables.Table table2 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 1, true);
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    #region for

                    // 1
                    table2.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                    // 2
                    if (model.SubForm2Records[i].SUB_DIC_TypeResource == null || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null)
                        table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu));

                    // 3
                    table2.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu));

                    // 4
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].NotOwnSource.Value.ToString()));

                    // 5
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExpenceEnergy.Value.ToString()));

                    // 5
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExtractVolume.Value.ToString()));

                    // 6
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].TransferOtherLegal.Value.ToString()));

                    #endregion
                }

            }
            #endregion

            // Ф-3
            #region F-3
            Aspose.Words.Tables.Table table31 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);
            if (model.SUB_Form3GuRecord != null)
            {
                count = model.SUB_Form3GuRecord.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table31.LastRow.Clone(true);
                        table31.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var item in model.SUB_Form3GuRecord)
                {
                    // 0
                    table31.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    // 1
                    if (item.CountOfBuildings == null)
                        table31.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, item.CountOfBuildings.ToString()));

                    // 2
                    if (item.YearOfConstruction == null)
                        table31.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, item.YearOfConstruction.ToString()));

                    // 3
                    if (item.AutomateItem == null)
                    {
                        table31.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, "Нет"));
                    }
                    else
                    {
                        if (item.AutomateItem == 1)
                        {
                            table31.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, "Да"));
                        }
                        else
                        {
                            table31.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, "Нет"));
                        }
                    }

                    // 4
                    if (item.TotalAreaOfBuilding == null)
                        table31.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, item.TotalAreaOfBuilding.ToString()));

                    // 5
                    if (item.HeatedAreaOfBuilding == null)
                        table31.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, item.HeatedAreaOfBuilding.ToString()));

                    // 6
                    if (item.CountOfEmployees == null)
                        table31.Rows[2 + startRowIndex].Cells[6].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[6].FirstParagraph.AppendChild(new Run(doc, item.CountOfEmployees.ToString()));

                    // 7
                    if (item.CountOfStudents == null)
                        table31.Rows[2 + startRowIndex].Cells[7].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[7].FirstParagraph.AppendChild(new Run(doc, item.CountOfStudents.ToString()));

                    // 8
                    if (item.CountOfBeds == null)
                        table31.Rows[2 + startRowIndex].Cells[8].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[8].FirstParagraph.AppendChild(new Run(doc, item.CountOfBeds.ToString()));

                    startRowIndex++;
                }
            }

            Aspose.Words.Tables.Table table3 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                doc.Range.Replace("_isplan", "x", replaceOptions);
                doc.Range.Replace("_isnotplan", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isplan", "", replaceOptions);
                doc.Range.Replace("_isnotplan", "x", replaceOptions);
            }

            //  система энергоменеджмента внедрена или ...
            if (model.IsEnergyManagementSystem == true)
            {
                doc.Range.Replace("_isenergymanagementsystem", "x", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isenergymanagementsystem", "", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "x", replaceOptions);
            }

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 14;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table3.LastRow.Clone(true);
                    table3.Rows.Insert(i + 3, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                table3.Rows[3 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                //
                if (record.SUB_DIC_Event != null)
                    table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_Event.NameRu));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.EventName))
                        table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EventName));
                }

                //
                if (record.EmplPeriodStr != null)
                    table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.EmplPeriodStr));
                else table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                //
                if (record.ActualInvest != null)
                    table3.Rows[3 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.ActualInvest.Value.ToString()));

                //
                if (record.SUB_DIC_TypeCounter != null)
                    table3.Rows[3 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));

                //
                if (record.InKind != null)
                    table3.Rows[3 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.InKind.Value.ToString()));

                //
                if (record.InMoney != null)
                    table3.Rows[3 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, record.InMoney.Value.ToString()));
            }

            #endregion

            #region Ф-3а
            //----1
            Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            if (model.SUB_Form5Record != null)
            {
                count = model.SUB_Form5Record.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table4.LastRow.Clone(true);
                        table4.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form5Record.OrderBy(e => e.Id))
                {
                    // 0
                    table4.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    if (record.TypeOfHeating == null)
                    {
                        table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        if (record.TypeOfHeating == 1)
                        {
                            table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, "Центральное отопление"));
                        }
                        else if (record.TypeOfHeating == 2)
                        {
                            table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, "Автономное отопление"));
                        }
                        else
                        {
                            table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                    }

                    if (record.sub_dic_energyindicator != null)
                        table4.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
                    else if (record.IndicatorName == null)
                        table4.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[3 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));

                    if (record.UnitMeasure == null)
                        table4.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));

                    if (record.CalcFormula == null)
                        table4.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));

                    if (record.EnergyValue == null)
                        table4.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));

                    startRowIndex++;
                }
            }

            //----2
            Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 5, true);
            if (model.SUB_Form3aGuRecord1 != null)
            {

                count = model.SUB_Form3aGuRecord1.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table5.LastRow.Clone(true);
                        table5.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form3aGuRecord1.OrderBy(e => e.Id))
                {
                    // 0
                    table5.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    // 1
                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DIC_GU.NameRu));
                        }
                    }
                    else
                    {
                        if (record.SourceName == null)
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SourceName));
                        }
                    }

                    // 2
                    if (record.CountOfHeatingSources == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountOfHeatingSources.ToString()));
                    }

                    // 3
                    if (record.CoefficientOfPerformance == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.CoefficientOfPerformance.ToString()));
                    }

                    // 4
                    if (record.PowerOfHeatingSources == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.PowerOfHeatingSources.ToString()));

                    }

                    // 5
                    if (record.YearOfCommissioning == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.YearOfCommissioning.ToString()));
                    }

                    startRowIndex++;
                }
            }

            //----3
            Aspose.Words.Tables.Table table6 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 6, true);
            if (model.SUB_Form3aGuRecord2 != null)
            {

                count = model.SUB_Form3aGuRecord2.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table6.LastRow.Clone(true);
                        table6.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form3aGuRecord2.OrderBy(e => e.Id))
                {
                    table6.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DIC_GU.NameRu));
                        }
                    }
                    else
                    {
                        if (record.DeviceName == null)
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DeviceName));

                        }
                    }

                    // 2
                    if (record.Amount == null)
                    {
                        table6.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table6.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.Amount.ToString()));
                    }

                    // 3
                    if (record.Power == null)
                    {
                        table6.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table6.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Power.ToString()));

                    }

                    // 4
                    if (record.HoursPerDay == null)
                    {
                        table6.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table6.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.HoursPerDay.ToString()));

                    }

                    startRowIndex++;
                }
            }

            //----4
            Aspose.Words.Tables.Table table7 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 7, true);
            if (model.SUB_Form3aGuRecord3 != null)
            {

                count = model.SUB_Form3aGuRecord3.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table7.LastRow.Clone(true);
                        table7.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form3aGuRecord3.OrderBy(e => e.Id))
                {
                    //
                    table7.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DIC_GU.NameRu));
                        }
                    }
                    else
                    {
                        if (record.EnergyConsumEquipName == null)
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EnergyConsumEquipName));
                        }
                    }

                    // 2
                    if (record.Amount == null)
                    {
                        table7.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table7.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.Amount.ToString()));
                    }

                    // 3
                    if (record.Power == null)
                    {
                        table7.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table7.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Power.ToString()));
                    }

                    // 4
                    if (record.HoursPerDay == null)
                    {
                        table7.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table7.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.HoursPerDay.ToString()));
                    }

                    startRowIndex++;
                }
            }
            #endregion
            // Ф-4
            //#region F-4
            //Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            //var form3Gu = model.SUB_Form3Gu.FirstOrDefault();
            //if (form3Gu != null)
            //{
            //    #region form3gu       

            //    if (form3Gu.YearOfConstruction != null)
            //        doc.Range.Replace("_yearOfConstruction", form3Gu.YearOfConstruction.ToString(), replaceOptions);
            //    else doc.Range.Replace("_yearOfConstruction", "", replaceOptions);

            //    //
            //    if (form3Gu.AutomateItem != null)
            //    {
            //        if (form3Gu.AutomateItem == 1)
            //            doc.Range.Replace("_automateItem", "да", replaceOptions);
            //        else doc.Range.Replace("_automateItem", "нет", replaceOptions);
            //    }
            //    else
            //    {
            //        doc.Range.Replace("_automateItem", "нет", replaceOptions);
            //    }

            //    //
            //    if (form3Gu.TotalAreaOfBuilding != null)
            //        doc.Range.Replace("_totalAreaOfBuilding", form3Gu.TotalAreaOfBuilding.ToString(), replaceOptions);
            //    else doc.Range.Replace("_totalAreaOfBuilding", "", replaceOptions);

            //    //
            //    if (form3Gu.HeatedAreaOfBuilding != null)
            //        doc.Range.Replace("_heatedAreaOfBuilding", form3Gu.HeatedAreaOfBuilding.ToString(), replaceOptions);
            //    else doc.Range.Replace("_heatedAreaOfBuilding", "", replaceOptions);

            //    // центральное отопление
            //    if (form3Gu.CentralHeating != null && form3Gu.CentralHeating == 1)
            //    {
            //        doc.Range.Replace("_central", "x", replaceOptions);
            //    }
            //    else doc.Range.Replace("_central", "", replaceOptions);

            //    // автономное отопление
            //    if (form3Gu.IndependentHeating != null && form3Gu.IndependentHeating == 1)
            //    {
            //        doc.Range.Replace("_independent", "x", replaceOptions);
            //    }
            //    else doc.Range.Replace("_independent", "", replaceOptions);
            //    #endregion
            //}
            //else
            //{
            //    doc.Range.Replace("_yearOfConstruction", "", replaceOptions);
            //    doc.Range.Replace("_automateItem", "нет", replaceOptions);
            //    doc.Range.Replace("_totalAreaOfBuilding", "", replaceOptions);
            //    doc.Range.Replace("_heatedAreaOfBuilding", "", replaceOptions);
            //    doc.Range.Replace("_central", "", replaceOptions);
            //    doc.Range.Replace("_independent", "", replaceOptions);
            //}

            //// get data for 
            //model.SubForm5Records = new List<SUB_Form5Record>();
            //var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            //foreach (var record in forms5)
            //    model.SubForm5Records.Add(record);

            //startRowIndex = 18;
            //count = model.SubForm5Records.Count;
            //if (count > 1)
            //{
            //    for (int i = 0; i < count - 1; i++)
            //    {
            //        var cloneRow = table4.LastRow.Clone(true);
            //        table4.Rows.Insert(i + 2, cloneRow);
            //    }
            //}

            //for (int i = 0; i < model.SubForm5Records.Count; i++)
            //{
            //    var record = model.SubForm5Records[i];

            //    table4.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

            //    if (record.sub_dic_energyindicator != null)
            //        table4.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
            //    else
            //    {
            //        if (!string.IsNullOrWhiteSpace(record.IndicatorName))
            //            table4.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));
            //    }

            //    if (record.UnitMeasure != null)
            //        table4.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));
            //    else table4.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

            //    if (record.CalcFormula != null)
            //        table4.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));
            //    else table4.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));


            //    if (record.EnergyValue == null)
            //        table4.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
            //    else
            //        table4.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));
            //}

            //#endregion

            // Ф-5
            //#region F-5
            //Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            //model.SubForm6Records = new List<SUB_Form6Record>();
            //var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            //foreach (var record in forms6)
            //    model.SubForm6Records.Add(record);

            //count = model.SubForm6Records.Count;
            //if (count > 1)
            //{
            //    for (int i = 0; i < count - 1; i++)
            //    {
            //        var cloneRow = table5.LastRow.Clone(true);
            //        table5.Rows.Insert(i + 2, cloneRow);
            //    }
            //}

            //for (int i = 0; i < count; i++)
            //{
            //    var record = model.SubForm6Records[i];

            //    table5.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

            //    if (record.SUB_DIC_TypeCounter != null)
            //        table5.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));
            //    else
            //        table5.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, ""));

            //    if (record.CountDevice != null)
            //        table5.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountDevice.Value.ToString()));
            //    else
            //        table5.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, ""));

            //    if (record.Equipment != null)
            //        table5.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Equipment.Value.ToString()));
            //}


            //#endregion


            #region check companyName
            while (juridicalName.IndexOf("\"") != -1)
            {
                juridicalName = juridicalName.Replace("\"", "");
            }

            while (juridicalName.IndexOf("|") != -1)
            {
                juridicalName = juridicalName.Replace("|", "");
            }

            while (juridicalName.IndexOf("?") != -1)
            {
                juridicalName = juridicalName.Replace("?", "");
            }

            while (juridicalName.IndexOf(":") != -1)
            {
                juridicalName = juridicalName.Replace(":", "");
            }

            if (!string.IsNullOrWhiteSpace(juridicalName))
            {
                if (juridicalName.Length > 100)
                    juridicalName = juridicalName.Substring(0, 99);
            }
            #endregion

            var signXmlRow = model.SUB_FormHistory.OrderByDescending(x => x.Id).FirstOrDefault(x => x.StatusId == 2);
            if (signXmlRow != null && signXmlRow.XmlSign != null)
                AddQrCode(doc, signXmlRow.XmlSign, 9);

            // doc.Save(path + "output1.pdf", Aspose.Words.SaveFormat.Pdf);
            var memoryStream = new MemoryStream();

            doc.Save(memoryStream, SaveFormat.Pdf);
            memoryStream.Position = 0;
            Session[handle] = memoryStream;

            string _fileName = model.SEC_User1.DIC_Kato.NameRu.Substring(0, 5) + "_" + model.SubjectIDK + "_" + model.SEC_User1.BINIIN + "_" + juridicalName;
            //----
            var dirpath = Server.MapPath("~/uploads/pdf/2018/");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            doc.Save(dirpath + "/" + _fileName + ".pdf", SaveFormat.Pdf);


            const string applicationtype = "application/pdf";
            return File(memoryStream, applicationtype, _fileName + ".pdf");
        }

        public ActionResult ASubjectExportToExcelSigned(long id)
        {
            var repository = new SubFormRepository();
            var formHistory = repository.GetHistory(id);

            if (formHistory == null)
            {
                return new JsonResult()
                {
                    Data = new { FileGuid = string.Empty, FileName = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            var model = SerializeHelper.DeserializeDataContract<SUB_Form>(formHistory.XmlSign);
            var handle = Guid.NewGuid().ToString();
            string companyName = CreateExcelFile(model, handle);

            //            return new JsonResult()
            //            {
            //                Data = new { FileGuid = handle, FileName = companyName + ".xlsx" },
            //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //            };
            return Download(handle, companyName + ".xlsx");
        }

        private string CreateExcelFile(SUB_Form model, string handle)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/aSubjectTmpl.xlsx");
            FileInfo fiTemplate = new FileInfo(path);

            IWorkbook book = new XSSFWorkbook(fiTemplate);
            ISheet sheet = null;
            IRow row = null;
            string year = model.ReportYear.ToString()
                , companyName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;
            CellRangeAddress cellRange = null;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                companyName = model.SEC_User1.JuridicalName;
            int count = 0;
            ISheet sheetSub = null;
            int lastNum = 0;
            // if (false)
            //{
            // Ф-1

            #region F-1

            if (model.SEC_User1 != null)
            {
                sheet = book.GetSheetAt(0);
                row = sheet.GetRow(9 - 1);
                row.Cells[1].SetCellValue(model.SEC_User1.ApplicationName);
                row.Cells[2].SetCellValue(model.SEC_User1.Address);
                row.Cells[3].SetCellValue(model.SEC_User1.FullName);
                row.Cells[4].SetCellValue(model.SEC_User1.Post);
                row.Cells[5].SetCellValue(model.SEC_User1.IsCvazy ? "Да" : "Нет");

                string oked = string.Empty;
                if (model.SEC_User1.DIC_OKED != null)
                    oked = model.SEC_User1.DIC_OKED.FullName;
                else
                    oked = model.SubjectMainOked;
                //                foreach (var secUserOked in model.SEC_User1.SEC_UserOked)
                //                {
                //                    oked += secUserOked.DIC_OKED.FullName;
                //                }

                row.Cells[6].SetCellValue(oked);

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    row = sheet.GetRow(18 - 1);
                    row.Cells[5].SetCellValue(responceFIO);
                }

                //  Полностью ФИО, подпись руководителя организации
                row = sheet.GetRow(19 - 1);
                headFIO = model.SEC_User1.FullName;
                row.Cells[5].SetCellValue(headFIO);
            }

            #endregion

            // Ф-2

            #region F-2

            {
                // Init 
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        if (report.SUB_DIC_TypeResource == null)
                            report.SUB_DIC_TypeResource = rptDicKindWaste;
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                sheet = book.GetSheetAt(1); // Ф-2
                // Заговолок
                row = sheet.GetRow(5 - 1);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("year", year);
                row.Cells[0].SetCellValue(strValue);
                // Название компании
                row = sheet.GetRow(9 - 1);
                strValue = row.Cells[0].StringCellValue;
                strValue = strValue.Replace("companyname", companyName);
                row.Cells[0].SetCellValue(strValue);


                // InsertRows(ref sheet, 16 - 2, sheet.LastRowNum, model.SubForm2Records.Count - 2); 
                startRowIndex = 14;
                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    //row = sheet.CreateRow(1);
                    var rowNum = startRowIndex - 1 + i;
                    row = sheet.GetRow(rowNum);

                    // 1
                    row.Cells[0].SetCellValue(i + 1);

                    // 2
                    var typeResourceName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu;
                    row.Cells[1].SetCellValue(typeResourceName);

                    // 3
                    var unitName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                                   || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu;
                    row.Cells[2].SetCellValue(unitName);

                    // 4
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        row.Cells[3].SetCellValue(string.Empty);
                    else
                        row.Cells[3].SetCellValue(model.SubForm2Records[i].ExtractVolume.Value);

                    // 5а
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        row.Cells[4].SetCellValue(string.Empty);
                    else
                        row.Cells[4].SetCellValue(model.SubForm2Records[i].NotOwnSource.Value);

                    // 5б
                    if (!model.SubForm2Records[i].LosEnergy.HasValue)
                        row.Cells[5].SetCellValue(string.Empty);
                    else
                        row.Cells[5].SetCellValue(model.SubForm2Records[i].LosEnergy.Value);

                    // 6
                    if (!model.SubForm2Records[i].OwnSource.HasValue)
                        row.Cells[6].SetCellValue(string.Empty);
                    else
                        row.Cells[6].SetCellValue(model.SubForm2Records[i].OwnSource.Value);

                    // 7
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        row.Cells[7].SetCellValue(string.Empty);
                    else
                        row.Cells[7].SetCellValue(model.SubForm2Records[i].TransferOtherLegal.Value);

                    // 8
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        row.Cells[8].SetCellValue(string.Empty);
                    else
                        row.Cells[8].SetCellValue(model.SubForm2Records[i].ExpenceEnergy.Value);

                    // 9
                    var note = model.SubForm2Records[i].Note ?? string.Empty;
                    row.Cells[9].SetCellValue(note);
                }

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                if (!string.IsNullOrEmpty(responceFIO))
                {
                    row = sheet.GetRow(startRowIndex + model.SubForm2Records.Count + 11 - 1); // 53
                    row.Cells[5].SetCellValue(responceFIO);
                }
                //  Полностью ФИО, подпись руководителя организации
                if (!string.IsNullOrEmpty(headFIO))
                {
                    row = sheet.GetRow(startRowIndex + model.SubForm2Records.Count + 12 - 1);
                    row.Cells[5].SetCellValue(headFIO);
                }
            }

            #endregion

            // Ф-3

            #region F-3

            sheet = book.GetSheetAt(2);
            // Заговолок
            row = sheet.GetRow(5 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("year", year);
            row.Cells[0].SetCellValue(strValue);
            // Название компании
            row = sheet.GetRow(7 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            var kinds = new SubDicKindResourceRepository().GetAll();
            var form3 = new List<SUB_Form3Record>();
            foreach (var rptDicKindWaste in kinds)
            {
                var report = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                if (report != null)
                {
                    if (report.SUB_DIC_KindResource == null)
                        report.SUB_DIC_KindResource = rptDicKindWaste;
                    form3.Add(report);
                }
                else
                {
                    var kind = new SUB_Form3Record
                    {
                        KindResourceId = rptDicKindWaste.Id,
                        SUB_DIC_KindResource = rptDicKindWaste
                    };
                    form3.Add(kind);
                }
            }
            model.SubForm3Records = form3;
            startRowIndex = 11;

            for (int i = 0; i < model.SubForm3Records.Count; i++)
            {
                row = sheet.GetRow(startRowIndex - 1 + 2 * i);
                // 4 - Потребление воды
                if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(model.SubForm3Records[i].ConsumptionVolume.Value);
                // 5 - Потери воды при транспортировке
                if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(model.SubForm3Records[i].LosTransportVolume.Value);

                row = sheet.GetRow(startRowIndex + 2 * i);
                // 4 - Потребление воды
                if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(model.SubForm3Records[i].ConsumptionPrice.Value);
                // 5 - Потери воды при транспортировке
                if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(model.SubForm3Records[i].LosTransportPrice.Value);
            }

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + model.SubForm3Records.Count * 2 + 8 - 1); // 53
                row.Cells[4].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + model.SubForm3Records.Count * 2 + 9 - 1);
                row.Cells[4].SetCellValue(headFIO);
            }

            #endregion


            // Ф-4

            #region F-4

            sheet = book.GetSheetAt(3);
            // Заговолок
            row = sheet.GetRow(5 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("year", year);
            row.Cells[0].SetCellValue(strValue);
            // Название компании
            row = sheet.GetRow(14 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 19;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
                InsertRows(ref sheet, startRowIndex - 1, count);

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                row = sheet.GetRow(startRowIndex + i - 1);
                row.Cells[0].SetCellValue(i + 1);
                if (record.SUB_DIC_Event != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_Event.NameRu);
                else
                    row.Cells[1].SetCellValue(record.EventName);
                row.Cells[2].SetCellValue(record.EmplPeriodStr);
                if (record.ActualInvest == null)
                    row.Cells[3].SetCellValue(string.Empty);
                else
                    row.Cells[3].SetCellValue(record.PlanExpend);
                if (record.ActualInvest == null)
                    row.Cells[4].SetCellValue(string.Empty);
                else
                    row.Cells[4].SetCellValue(record.ActualInvest.Value);
                if (record.SUB_DIC_TypeResource == null)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(record.SUB_DIC_TypeResource.NameRu);
                if (record.InKind == null)
                    row.Cells[6].SetCellValue(string.Empty);
                else
                    row.Cells[6].SetCellValue(record.InKind.Value);
                if (record.InMoney == null)
                    row.Cells[7].SetCellValue(string.Empty);
                else
                    row.Cells[7].SetCellValue(record.InMoney.Value);
            }

            // copy footer
            sheetSub = book.GetSheetAt(4);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 3);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(4);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[4].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[4].SetCellValue(headFIO);
            }

            #endregion

            // Ф-5

            #region F-5

            sheet = book.GetSheetAt(4);
            // Заговолок
            row = sheet.GetRow(6 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("year", year);
            row.Cells[0].SetCellValue(strValue);
            // Название компании
            row = sheet.GetRow(8 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            // get data for 
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 12;
            count = model.SubForm5Records.Count;
            if (count > 1)
                InsertRows(ref sheet, startRowIndex - 1, count);

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];
                row = sheet.GetRow(startRowIndex + i - 1);
                row.Cells[0].SetCellValue(i + 1);

                if (record.SUB_DIC_NormEnergy != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_NormEnergy.NameRu);
                else
                    row.Cells[1].SetCellValue(record.IndicatorName);
                row.Cells[2].SetCellValue(record.RegularStandart);
                row.Cells[3].SetCellValue(record.UnitMeasure);
                row.Cells[4].SetCellValue(record.CalcFormula);

                if (record.EnergyValue == null)
                    row.Cells[5].SetCellValue(string.Empty);
                else
                    row.Cells[5].SetCellValue(record.EnergyValue.Value);
            }

            // copy footer
            sheetSub = book.GetSheetAt(5);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(5);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[2].SetCellValue(headFIO);
            }

            #endregion

            // Ф-6

            #region F-6

            sheet = book.GetSheetAt(5);

            // Название компании
            row = sheet.GetRow(7 - 1);
            strValue = row.Cells[0].StringCellValue;
            strValue = strValue.Replace("companyname", companyName);
            row.Cells[0].SetCellValue(strValue);

            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            startRowIndex = 11;
            count = model.SubForm6Records.Count;
            if (count > 1)
                InsertRows(ref sheet, startRowIndex - 1, count);

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];
                row = sheet.GetRow(startRowIndex + i - 1);
                row.Cells[0].SetCellValue(i + 1);
                if (record.SUB_DIC_TypeCounter != null)
                    row.Cells[1].SetCellValue(record.SUB_DIC_TypeCounter.NameRu);
                else
                    row.Cells[1].SetCellValue(String.Empty);

                if (record.CountDevice != null)
                    row.Cells[2].SetCellValue(record.CountDevice.Value);
                else
                    row.Cells[2].SetCellValue(string.Empty);

                if (record.Equipment != null)
                    row.Cells[3].SetCellValue(record.Equipment.Value);
                else
                    row.Cells[3].SetCellValue(string.Empty);
            }

            sheetSub = book.GetSheetAt(6);
            lastNum = sheetSub.LastRowNum;
            for (int i = 0; i <= lastNum; i++)
            {
                var rowSource = sheetSub.GetRow(i);
                row = sheet.CreateRow(startRowIndex + count + 2 + i - 1);
                for (int j = 0; j < rowSource.LastCellNum; j++)
                {
                    ICell sourceCell = rowSource.GetCell(j);
                    ICell destCell = row.CreateCell(j);
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    destCell.CellStyle = sourceCell.CellStyle;
                }

                if (i == lastNum - 1 || i == lastNum)
                {
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                    sheet.AddMergedRegion(cellRange);
                }
            }
            book.RemoveSheetAt(6);

            // Полностью ФИО, должность, контакты и подпись ответственного лица
            if (!string.IsNullOrEmpty(responceFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 1 - 1);
                row.Cells[2].SetCellValue(responceFIO);
            }
            //  Полностью ФИО, подпись руководителя организации
            if (!string.IsNullOrEmpty(headFIO))
            {
                row = sheet.GetRow(startRowIndex + count + lastNum + 2 - 1);
                row.Cells[2].SetCellValue(headFIO);
            }

            #endregion


            // План мероприятий

            #region План мероприятий

            if (model.IsPlan != null && model.IsPlan == true && model.BeginPlanYear != null && model.EndPlanYear != null)
            {
                #region Таблица 1

                sheet = book.GetSheetAt(6);
                startRowIndex = 8;

                model.SubDicKindTabOnes = new SubDicKindTabOneRepository().GetAll();
                model.SubFormTab1s = new List<SUB_FormTab1>();

                foreach (var tabOne in model.SubDicKindTabOnes)
                {
                    var list = model.SUB_FormTab1.Where(e => e.KindId == tabOne.Id).ToList();
                    var codes = new List<string>();
                    for (var t = 1; t < 3; t++)
                    {
                        var codeIndex = tabOne.IndexCode + ".0" + t;
                        codes.Add(codeIndex);
                        if (list.Any(e => e.Code == codeIndex))
                        {
                            model.SubFormTab1s.Add(list.First(e => e.Code == codeIndex));
                        }
                        else
                        {
                            model.SubFormTab1s.Add(new SUB_FormTab1() { Code = codeIndex, KindId = tabOne.Id });
                        }
                    }
                    var notIn = list.Where(e => !codes.Contains(e.Code));
                    foreach (var subFormTab1 in notIn)
                    {
                        model.SubFormTab1s.Add(subFormTab1);
                    }
                }

                for (int i = model.BeginPlanYear.Value; i <= model.EndPlanYear.Value; i++)
                {
                    row = sheet.GetRow(6 - 1);
                    row.Cells[3 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                    row.Cells[8 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                }

                count = model.SubDicKindTabOnes.Count + model.SubFormTab1s.Count
                        + model.SubDicKindTabOnes.Count * 2 + 2 - 1;

                InsertRows(ref sheet, 8 - 1, count);

                var rowIndex = 0;
                foreach (SUB_DIC_KindTabOne sdk in model.SubDicKindTabOnes)
                {
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, row.LastCellNum - 1);
                    sheet.AddMergedRegion(cellRange);

                    var subDicKind = sdk;
                    var cell = row.Cells[0];
                    cell.SetCellValue(subDicKind.NameRu);

                    ICellStyle cellStyle = book.CreateCellStyle();
                    IFont font = book.CreateFont();
                    font.IsBold = font.IsItalic = true;

                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cellStyle.SetFont(font);
                    cell.CellStyle = cellStyle;

                    foreach (var subFormTab in model.SubFormTab1s.Where(sft => sft.KindId == subDicKind.Id))
                    {
                        row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                        row.Cells[0].SetCellValue(subFormTab.Code);
                        row.Cells[1].SetCellValue(subFormTab.Events);
                        row.Cells[2].SetCellValue(subFormTab.Year1);
                        row.Cells[3].SetCellValue(subFormTab.Year2);
                        row.Cells[4].SetCellValue(subFormTab.Year3);
                        row.Cells[5].SetCellValue(subFormTab.Year4);
                        row.Cells[6].SetCellValue(subFormTab.Year5);

                        if (subFormTab.Expend1 != null)
                            row.Cells[7].SetCellValue(subFormTab.Expend1.Value);
                        if (subFormTab.Expend2 != null)
                            row.Cells[8].SetCellValue(subFormTab.Expend2.Value);
                        if (subFormTab.Expend3 != null)
                            row.Cells[9].SetCellValue(subFormTab.Expend3.Value);
                        if (subFormTab.Expend4 != null)
                            row.Cells[10].SetCellValue(subFormTab.Expend4.Value);
                        if (subFormTab.Expend5 != null)
                            row.Cells[11].SetCellValue(subFormTab.Expend5.Value);
                        row.Cells[12].SetCellValue(subFormTab.Note);
                    }

                    // TODO нету инициализации полей
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                    sheet.AddMergedRegion(cellRange);
                    row.Cells[0].SetCellValue("Итого:");

                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                    sheet.AddMergedRegion(cellRange);
                    row.Cells[0].SetCellValue("Всего:");

                    cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 7, 11);
                    sheet.AddMergedRegion(cellRange);
                }
                // TODO нету инициализации полей
                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Итого по плану:");

                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 6);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Всего по плану:");

                #endregion

                #region Таблица 2

                sheet = book.GetSheetAt(7);
                startRowIndex = 9;

                var dicsTwo = new SubDicKindTabTwoRepository().GetAll();
                var listTwo = new List<SUB_FormTab2>();
                foreach (var rptDicKindWaste in dicsTwo)
                {
                    var report = model.SUB_FormTab2.FirstOrDefault(e => e.KindId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        listTwo.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_FormTab2 { KindId = rptDicKindWaste.Id, SUB_DIC_KindTabTwo = rptDicKindWaste };
                        listTwo.Add(kind);
                    }
                }
                model.SubFormTab2s = listTwo;

                for (int i = model.BeginPlanYear.Value; i <= model.EndPlanYear.Value; i++)
                {
                    row = sheet.GetRow(6 - 1);
                    row.Cells[3 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                    row.Cells[8 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                }

                count = model.SubFormTab2s.Count + 2 - 1;
                InsertRows(ref sheet, startRowIndex - 1, count);

                rowIndex = 0;
                foreach (var sft in model.SubFormTab2s)
                {
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    var subFormTab2 = sft;

                    row.Cells[0].SetCellValue(subFormTab2.SUB_DIC_KindTabTwo.IndexCode);
                    row.Cells[1].SetCellValue(subFormTab2.SUB_DIC_KindTabTwo.NameRu);
                    if (subFormTab2.Volume1.HasValue)
                        row.Cells[2].SetCellValue(subFormTab2.Volume1.Value);
                    if (subFormTab2.Volume2.HasValue)
                        row.Cells[3].SetCellValue(subFormTab2.Volume2.Value);
                    if (subFormTab2.Volume3.HasValue)
                        row.Cells[4].SetCellValue(subFormTab2.Volume3.Value);
                    if (subFormTab2.Volume4.HasValue)
                        row.Cells[5].SetCellValue(subFormTab2.Volume4.Value);
                    if (subFormTab2.Volume5.HasValue)
                        row.Cells[6].SetCellValue(subFormTab2.Volume5.Value);

                    if (subFormTab2.Expend1.HasValue)
                        row.Cells[7].SetCellValue(subFormTab2.Expend1.Value);
                    if (subFormTab2.Expend2.HasValue)
                        row.Cells[8].SetCellValue(subFormTab2.Expend2.Value);
                    if (subFormTab2.Expend3.HasValue)
                        row.Cells[9].SetCellValue(subFormTab2.Expend3.Value);
                    if (subFormTab2.Expend4.HasValue)
                        row.Cells[10].SetCellValue(subFormTab2.Expend4.Value);
                    if (subFormTab2.Expend5.HasValue)
                        row.Cells[11].SetCellValue(subFormTab2.Expend5.Value);

                    row.Cells[12].SetCellValue(subFormTab2.Possible);
                    row.Cells[13].SetCellValue(subFormTab2.Note);
                }

                // TODO нету инициализации полей
                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Итого:");

                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 1);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Всего:");

                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 2, 6);
                sheet.AddMergedRegion(cellRange);

                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 7, 11);
                sheet.AddMergedRegion(cellRange);

                #endregion

                #region Таблица 3

                sheet = book.GetSheetAt(8);
                startRowIndex = 8;

                model.SubFormTab3s = new List<SUB_FormTab3>();
                var formtab3 = model.SUB_FormTab3.OrderBy(e => e.Id);
                foreach (var record in formtab3)
                {
                    model.SubFormTab3s.Add(record);
                }
                if (model.SubFormTab3s.Count == 0)
                {
                    model.SubFormTab3s.Add(new SUB_FormTab3());
                }

                for (int i = model.BeginPlanYear.Value; i <= model.EndPlanYear.Value; i++)
                {
                    row = sheet.GetRow(6 - 1);
                    row.Cells[4 + i - model.BeginPlanYear.Value - 1].SetCellValue(i);
                }

                count = model.SubFormTab3s.Count + 2 - 1;
                InsertRows(ref sheet, startRowIndex - 1, count);
                rowIndex = 0;
                foreach (var sft in model.SubFormTab3s)
                {
                    var subformTab = sft;
                    row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                    row.Cells[0].SetCellValue(rowIndex);
                    row.Cells[1].SetCellValue(subformTab.ShareIndex);
                    row.Cells[2].SetCellValue(subformTab.UnitKoef);
                    if (subformTab.Volume1.HasValue)
                        row.Cells[3].SetCellValue(subformTab.Volume1.Value);
                    if (subformTab.Volume2.HasValue)
                        row.Cells[4].SetCellValue(subformTab.Volume2.Value);
                    if (subformTab.Volume3.HasValue)
                        row.Cells[5].SetCellValue(subformTab.Volume3.Value);
                    if (subformTab.Volume4.HasValue)
                        row.Cells[6].SetCellValue(subformTab.Volume4.Value);
                    if (subformTab.Volume5.HasValue)
                        row.Cells[7].SetCellValue(subformTab.Volume5.Value);
                }

                // TODO нету инициализации полей
                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 2);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Итого:");

                row = sheet.GetRow(startRowIndex + rowIndex++ - 1);
                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 0, 2);
                sheet.AddMergedRegion(cellRange);
                row.Cells[0].SetCellValue("Всего:");

                cellRange = new CellRangeAddress(row.RowNum, row.RowNum, 3, 7);
                sheet.AddMergedRegion(cellRange);

                #endregion

            }
            else
            {
                book.RemoveSheetAt(6);
                book.RemoveSheetAt(6);
                book.RemoveSheetAt(6);
            }



            #endregion

            HSSFFormulaEvaluator.EvaluateAllFormulaCells(book);
            // }
            using (var memoryStream = new MemoryStream())
            {
                book.Write(memoryStream);
                //memoryStream.Position = 0;
                Session[handle] = memoryStream.ToArray();
            }

            return companyName;
        }

        public ActionResult Download(string fileGuid, string fileName)
        {
            if (Session[fileGuid] == null)
                return new EmptyResult();

            byte[] data = Session[fileGuid] as byte[];
            //MemoryStream ms = new MemoryStream(data);
            //ms.Position = 0;
            //Response.AddHeader("content-disposition", String.Format(CultureInfo.InvariantCulture, "attachment; filename={0}", fileName));
            const string applicationtype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(data, applicationtype, fileName);
        }

        public ActionResult DownloadPdf(string fileGuid, string fileName)
        {
            if (Session[fileGuid] == null)
                return new EmptyResult();

            byte[] data = Session[fileGuid] as byte[];
            const string applicationtype = "application/pdf";
            return File(data, applicationtype, fileName);
        }

        private void InsertRows(ref ISheet sheet, int fromRowIndex, int rowCount)
        {
            sheet.ShiftRows(fromRowIndex, fromRowIndex + rowCount, rowCount, true, false);
            IRow rowSource = sheet.GetRow(fromRowIndex + rowCount);
            bool flag = false;
            for (int rowIndex = fromRowIndex; rowIndex < fromRowIndex + rowCount; rowIndex++)
            {
                IRow rowInsert = sheet.CreateRow(rowIndex);
                if (rowSource == null)
                    continue;

                rowInsert.Height = rowSource.Height;
                for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
                {
                    ICell cellSource = rowSource.GetCell(colIndex);
                    ICell cellInsert = rowInsert.CreateCell(colIndex);
                    if (cellSource != null)
                    {
                        cellInsert.CellStyle = cellSource.CellStyle;
                    }
                }
            }
        }
        #endregion

        #region Import From Excel

        public ActionResult ImportFromExcel(ImportExcelModel excelModel)
        {
            // var e = excelModel;
            //            var fileName = excelModel.FileContent.FileName;
            //            string fileContentType = excelModel.FileContent.ContentType;  
            //            byte[] fileBytes = new byte[excelModel.FileContent.ContentLength];
            //var data = excelModel.FileContent.InputStream.Read(fileBytes, 0, Convert.ToInt32(excelModel.FileContent.ContentLength));
            long Id = 0;
            ViewBag.ImportErrorMsg = null;

            try
            {
                IWorkbook book = new XSSFWorkbook(excelModel.FileContent.InputStream);
                ISheet sheet = null;
                IRow row = null;

                sheet = book.GetSheetAt(0);
                row = sheet.GetRow(5 - 1);

                if (book.NumberOfSheets < 6 || !row.Cells[0].IsMergedCell)
                {
                    throw new Exception("Wrong format");
                }

                var repository = new SubFormRepository();
                var model = repository.GetQuery(
                    sf => sf.UserId == excelModel.ObjectId && sf.ReportYear == excelModel.Year && !sf.IsDeleted)
                    .FirstOrDefault() ??
                            new SUB_Form()
                            {
                                UserId = excelModel.ObjectId,
                                ReportYear = excelModel.Year,
                                CreateDate = DateTime.Now,
                                IsDeleted = false,
                            };
                repository.SaveOrUpdate(model, excelModel.ObjectId);
                int startRowIndex = 0;

                // Ф-1

                #region F-1

                if (model.SEC_User1 != null)
                {
                    sheet = book.GetSheetAt(0);
                    row = sheet.GetRow(9 - 1);
                    model.SEC_User1.JuridicalName = row.Cells[1].StringCellValue;
                    model.SEC_User1.Address = row.Cells[2].StringCellValue;
                    model.SEC_User1.FullName = row.Cells[3].StringCellValue;
                    model.SEC_User1.Post = row.Cells[4].StringCellValue;
                    model.SEC_User1.IsCvazy = row.Cells[5].StringCellValue == "Да";

                    var okeds = row.Cells[6].StringCellValue;
                    if (!string.IsNullOrEmpty(okeds))
                    {
                        var okedRepository = new DicOkedRepository();
                        foreach (var code in okeds.Split(';'))
                        {
                            string c = code;
                            var dicOked = okedRepository.GetQuery(oked => oked.Code == c).FirstOrDefault();
                            if (dicOked != null)
                            {
                                model.SEC_User1.SEC_UserOked.Add(new SEC_UserOked()
                                {
                                    OkedId = dicOked.Id,
                                    UserId = model.SEC_User1.Id
                                });
                            }
                        }
                    }
                }

                var userRepository = new SecUserRepository();
                userRepository.SaveOrUpdate(model.SEC_User1, MyExtensions.GetCurrentUserId());
                #endregion

                // Ф-2

                #region F-2

                sheet = book.GetSheetAt(1);
                startRowIndex = 14;

                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var kind = new SUB_Form2Record { TypeResourceId = rptDicKindWaste.Id, FormId = model.Id };
                    subForm2Records.Add(kind);
                }
                subForm2Records = subForm2Records.OrderBy(s => s.TypeResourceId).ToList();

                for (int i = 0; i < subForm2Records.Count; i++)
                {
                    var rowNum = startRowIndex - 1 + i;
                    row = sheet.GetRow(rowNum);


                    // 4
                    if (row.Cells[3].CellType == CellType.Numeric)
                        subForm2Records[i].ExtractVolume = row.Cells[3].NumericCellValue;

                    // 5а
                    if (row.Cells[4].CellType == CellType.Numeric)
                        subForm2Records[i].NotOwnSource = row.Cells[4].NumericCellValue;

                    // 5б
                    if (row.Cells[5].CellType == CellType.Numeric)
                        subForm2Records[i].LosEnergy = row.Cells[5].NumericCellValue;

                    // 6
                    if (row.Cells[6].CellType == CellType.Numeric)
                        subForm2Records[i].OwnSource = row.Cells[6].NumericCellValue;

                    // 7
                    if (row.Cells[7].CellType == CellType.Numeric)
                        subForm2Records[i].TransferOtherLegal = row.Cells[7].NumericCellValue;

                    // 8
                    if (row.Cells[8].CellType == CellType.Numeric)
                        subForm2Records[i].ExpenceEnergy = row.Cells[8].NumericCellValue;

                    // 9
                    subForm2Records[i].Note = row.Cells[9].StringCellValue;

                    repository.SaveOrUpdateSubjectForm(subForm2Records[i], SubjectFormConsts.SubjectForm2);
                }
                model.SUB_Form2Record = subForm2Records;

                #endregion

                // Ф-3

                #region F-3

                sheet = book.GetSheetAt(2);
                startRowIndex = 11;

                var kinds = new SubDicKindResourceRepository().GetAll();
                var form3Records = new List<SUB_Form3Record>();
                foreach (var rptDicKindWaste in kinds)
                {
                    var kind = new SUB_Form3Record
                    {
                        KindResourceId = rptDicKindWaste.Id,
                        FormId = model.Id,
                    };
                    form3Records.Add(kind);
                }


                for (int i = 0; i < form3Records.Count; i++)
                {
                    row = sheet.GetRow(startRowIndex - 1 + 2 * i);
                    // 4 - Потребление воды
                    if (row.Cells[4].CellType == CellType.Numeric)
                        form3Records[i].ConsumptionVolume = Convert.ToSingle(row.Cells[4].NumericCellValue);

                    // 5 - Потери воды при транспортировке
                    if (row.Cells[5].CellType == CellType.Numeric)
                        form3Records[i].LosTransportVolume = Convert.ToSingle(row.Cells[5].NumericCellValue);

                    row = sheet.GetRow(startRowIndex + 2 * i);
                    // 4 - Потребление воды
                    if (row.Cells[4].CellType == CellType.Numeric)
                        form3Records[i].ConsumptionPrice = Convert.ToSingle(row.Cells[4].NumericCellValue);

                    // 5 - Потери воды при транспортировке
                    if (row.Cells[5].CellType == CellType.Numeric)
                        form3Records[i].LosTransportPrice = Convert.ToSingle(row.Cells[5].NumericCellValue);
                    repository.SaveOrUpdateSubjectForm(form3Records[i], SubjectFormConsts.SubjectForm3);
                }
                model.SUB_Form3Record = form3Records;

                #endregion

                // Ф-4

                #region F-4

                sheet = book.GetSheetAt(3);
                startRowIndex = 19;
                int rowIndex = startRowIndex;

                while (sheet.GetRow(rowIndex) != null)
                {
                    row = sheet.GetRow(rowIndex++);
                    //all cells are empty, so is a 'blank row'
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) break;
                    if (row.Cells[0].CellType != CellType.Blank &&
                        row.Cells.Skip(1).All(d => d.CellType == CellType.Blank))
                        continue;
                    var record = new SUB_Form4Record()
                    {
                        FormId = model.Id,
                    };
                    record.EventName = row.Cells[1].StringCellValue;
                    record.EmplPeriodStr = row.Cells[2].StringCellValue;
                    if (row.Cells[3].CellType == CellType.Numeric)
                        record.ActualInvest = Convert.ToSingle(row.Cells[3].NumericCellValue);

                    var typeResourceName = row.Cells[4].StringCellValue;
                    var typeResource =
                        new SubDicTypeResourceRepository().GetQuery(tr => tr.NameRu == typeResourceName)
                            .FirstOrDefault();
                    if (typeResource != null)
                        record.TypeResourceId = typeResource.Id;
                    if (row.Cells[5].CellType == CellType.Numeric)
                        record.InKind = Convert.ToSingle(row.Cells[5].NumericCellValue);
                    if (row.Cells[6].CellType == CellType.Numeric)
                        record.InMoney = Convert.ToSingle(row.Cells[6].NumericCellValue);

                    repository.SaveOrUpdateSubjectForm(record, SubjectFormConsts.SubjectForm4);

                    model.SUB_Form4Record.Add(record);
                }

                #endregion

                // Ф-5

                #region F-5

                sheet = book.GetSheetAt(4);
                rowIndex = startRowIndex = 12;
                // get data for 
                model.SubForm5Records = new List<SUB_Form5Record>();

                while (sheet.GetRow(rowIndex) != null)
                {
                    row = sheet.GetRow(rowIndex++);
                    //all cells are empty, so is a 'blank row'
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) break;
                    if (row.Cells[0].CellType != CellType.Blank &&
                        row.Cells.Skip(1).All(d => d.CellType == CellType.Blank))
                        continue;
                    var record = new SUB_Form5Record()
                    {
                        FormId = model.Id
                    };
                    record.IndicatorName = row.Cells[1].StringCellValue;
                    record.RegularStandart = row.Cells[2].StringCellValue;
                    record.UnitMeasure = row.Cells[3].StringCellValue;
                    record.CalcFormula = row.Cells[4].StringCellValue;

                    if (row.Cells[5].CellType == CellType.Numeric)
                        record.EnergyValue = Convert.ToSingle(row.Cells[5].NumericCellValue);

                    repository.SaveOrUpdateSubjectForm(record, SubjectFormConsts.SubjectForm5);
                    model.SUB_Form5Record.Add(record);
                }

                #endregion

                // Ф-6

                #region F-6

                sheet = book.GetSheetAt(5);
                model.SubForm6Records = new List<SUB_Form6Record>();
                rowIndex = startRowIndex = 11;
                while (sheet.GetRow(rowIndex) != null)
                {
                    row = sheet.GetRow(rowIndex++);
                    //all cells are empty, so is a 'blank row' and end
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) break;
                    if (row.Cells[0].CellType != CellType.Blank &&
                        row.Cells.Skip(1).All(d => d.CellType == CellType.Blank))
                        continue;

                    var record = new SUB_Form6Record()
                    {
                        FormId = model.Id
                    };

                    var typeResourceName = row.Cells[1].StringCellValue;
                    var typeResource =
                        new SubDicTypeResourceRepository().GetQuery(tr => tr.NameRu == typeResourceName)
                            .FirstOrDefault();
                    if (typeResource != null)
                    {
                        record.TypeResourceId = typeResource.Id;
                    }
                    if (row.Cells[2].CellType == CellType.Numeric)
                        record.CountDevice = Convert.ToInt32(row.Cells[2].NumericCellValue);
                    if (row.Cells[3].CellType == CellType.Numeric)
                        record.Equipment = Convert.ToSingle(row.Cells[3].NumericCellValue);

                    repository.SaveOrUpdateSubjectForm(record, SubjectFormConsts.SubjectForm6);
                    model.SUB_Form6Record.Add(record);
                }

                #endregion

                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                Id = model.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ImportErrorMsg = "Проверьте формат и наполнение файла";
                return RedirectToAction("CommonView", new { msg = ViewBag.ImportErrorMsg });
            }

            //            return new JsonResult()
            //            {
            //                Data = new {IsSuccess = true},
            //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //            };
            if (Id != 0)
                return RedirectToAction("ShowDetails",
                    "AppForm", new { Id });

            return RedirectToAction("CommonView");
        }

        #endregion

        #region qrcoder   

        static Aspose.Words.Tables.Cell FillCell(string text, Document doc)
        {
            var _cell = new Aspose.Words.Tables.Cell(doc);
            var _paragraph = new Paragraph(doc);
            _paragraph.AppendChild(new Run(doc, text));
            _cell.AppendChild(_paragraph);

            return _cell;
        }

        static void AddQrCode(Document doc, string text, int indexTable)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);
            var _qrlist = GetQrByteList(text);

            Aspose.Words.Tables.Table
                table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, indexTable, true);
            int limit = 0;
            for (int i = 0; i < _qrlist.Count; i++)
            {
                if (limit < 50)
                {
                    Aspose.Words.Tables.Cell cell = table.LastRow.Cells[0];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.InsertImage(_qrlist[i]);
                    limit++;
                }
                else
                {
                    var row = new Aspose.Words.Tables.Row(doc);
                    row.AppendChild(FillCell("", doc));
                    table.Rows.Add(row);
                    table.LastRow.RowFormat.Borders.LineStyle = Aspose.Words.LineStyle.None;
                    builder.MoveTo(table.LastRow.Cells[0].FirstParagraph);
                    builder.InsertImage(_qrlist[i]);
                    limit = 0;
                }
            }
        }

        static Bitmap SetSizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }

            return result;
        }

        static List<byte[]> GetQrByteList(string text)
        {
            var result = new List<byte[]>();
            int textLength = 400;
            var _count = 1.0 * text.Length / textLength;
            int qrCodeCount = (int)Math.Ceiling(_count);

            for (int i = 0; i < qrCodeCount; i++)
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    var begin = i * textLength;
                    var len = text.Length - i * textLength;
                    len = (textLength < len) ? textLength : len;
                    string tt = text.Substring(begin, len);
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(tt, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);


                    Bitmap qrCodeImage = qrCode.GetGraphic(1);

                    //if (qrCodeImage.Height < 133)
                    {
                        qrCodeImage = SetSizeBitmap(qrCodeImage, 130, 130);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        result.Add(byteImage);
                    }

                    //Bitmap qrCodeImage = new Bitmap(qrCode.GetGraphic(1), 130, 130);
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //    byte[] byteImage = ms.ToArray();
                    //    result.Add(byteImage);
                    //}
                }
            }

            return result;
        }
        #endregion

        #region  export all to pdf
        public ActionResult PdfUpload()
        {
            int year = new SubFormRepository().GetActiveReportYear();
            ViewBag.ActiveYear = year;
            //var list = new List<Pdf_State>();
            //try
            //{                
            //    var path = Server.MapPath("~/uploads/pdf/" + year + "/settings.txt");
            //    using (StreamReader reader = new StreamReader(path))
            //    {
            //        while (!reader.EndOfStream)
            //        {
            //            list.Add(JsonConvert.DeserializeObject<Pdf_State>(reader.ReadLine()));
            //        }
            //    }
            //}
            //catch (Exception ex) { }

            var model = new RstReportRepository().GetOblastList(year);
            return View(model);
        }

        [HttpPost]
        public ActionResult PdfUploadByOblastId(long oblastId, string kato, int year)
        {
            var repository = new RstReportRepository();
            var rows = repository.GetReportListByOblId(year, oblastId);
            var ids = rows.Select(x => x.form_id).ToArray();
            var modelList = new SubFormRepository().GetQuery(x => ids.Contains(x.Id)).ToList();

            // write to pdf state
            //repository.WriteUploadPdfState(oblastId, "start");

            string fileName = string.Empty;
            foreach (var model in modelList)
            {
                if (model != null)
                {
                    var rst_id = rows.Find(x => x.form_id == model.Id).rst_id;
                   
                        var bufferRstRR = model.RST_ReportReestr.FirstOrDefault(x => x.FormId == model.Id);
                        if (bufferRstRR == null)
                        {
                            repository.WriteToPdfResult(rst_id, null, "not found FormId");
                            continue;
                        }

                        #region copy rst_reportreestr user info
                        model.SEC_User1.LastName = bufferRstRR.usrlastname;
                        model.SEC_User1.SecondName = bufferRstRR.usrsecondname;
                        model.SEC_User1.FirstName = bufferRstRR.usrfirstname;
                        model.SEC_User1.JuridicalName = bufferRstRR.usrjuridicalname;
                        model.SEC_User1.Post = bufferRstRR.usrpost;
                        model.SEC_User1.Mobile = bufferRstRR.usrmobile;
                        model.SEC_User1.WorkPhone = bufferRstRR.usrworkphone;
                        model.SEC_User1.Address = bufferRstRR.usraddress;

                        model.SEC_User1.IsCvazy = (bufferRstRR.usriscvazy != null) ? Convert.ToBoolean(bufferRstRR.usriscvazy) : false;
                        model.SEC_User1.ResponceFIO = bufferRstRR.usrresponcefio;
                        model.SEC_User1.ResponcePost = bufferRstRR.usrresponcepost;
                        model.SEC_User1.Oblast = bufferRstRR.usroblast;
                        model.SEC_User1.Region = bufferRstRR.usrregion;
                        model.SEC_User1.IDK = bufferRstRR.usridk;
                        model.SEC_User1.IsGuest = bufferRstRR.SEC_User.IsGuest;
                        #endregion

                        try
                        {
                            fileName = string.Empty;
                            if (model.RST_ReportReestr.FirstOrDefault(x => x.IsDeleted == false).usrfscode == 3)
                            {
                                if (model.ReportYear >= 2018 && model.ReportYear < 2019)
                                    fileName = ASubjectExportToPdfGuAll(model, kato);
                                else fileName = ASubjectExportToPdfGuNewAll(model, kato);
                            }
                            else fileName = ASubjectExportToPdfAll(model, kato);

                            // write to file
                            repository.WriteToPdfResult(rst_id, fileName, "");
                        }
                        catch (Exception ex)
                        {
                            repository.WriteToPdfResult(rst_id, null, ex.Message);
                        }                    
                }
            }

            // write to pdf state
            //repository.WriteUploadPdfState(oblastId, "end");

            return Json(new { IsSuccess = true });
        }

        [HttpPost]
        public ActionResult ClearPdfFilesByOblastId(long oblastId, string kato,int year)
        {
            new RstReportRepository().ClearPdfFiles(year, oblastId);

            var path = Server.MapPath("~/uploads/pdf/" + year + "/" + kato);
            if (Directory.Exists(path))
            {
                string[] pdfList = Directory.GetFiles(path, "*.pdf");
                foreach (var pdf in pdfList)
                {
                    System.IO.File.Delete(pdf);
                }
                Directory.Delete(path);
            }

            return Json(new { IsSuccess = true });
        }
        public ActionResult ExportAllReportToPdf(string katoCode)
        {
            var repository = new SubFormRepository();
            var _pdfTableList = repository.GetPdfTable(katoCode);
            #region write logFile
            var path = Server.MapPath("~/uploads/pdf/" + katoCode + ".txt");
            var path2 = Server.MapPath("~/uploads/pdf/" + katoCode + "-error.txt");

            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                using (var sw = System.IO.File.AppendText(path))
                {
                    sw.WriteLine("===============Pdf Export " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "================ ");
                }
            }
            else
            {
                using (var sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine("===============Pdf Export " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "================ ");
                }
            }


            FileInfo fileInf2 = new FileInfo(path2);
            if (fileInf2.Exists)
            {
                using (var sw = System.IO.File.AppendText(path2))
                {
                    sw.WriteLine("===============Pdf Export " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "================ ");
                }
            }
            else
            {
                using (var sw = System.IO.File.CreateText(path2))
                {
                    sw.WriteLine("===============Pdf Export " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "================ ");
                }
            }
            #endregion

            foreach (var item in _pdfTableList)
            {
                try
                {
                    var model = repository.GetById(item.FormId);
                    if (model != null)
                    {
                        if (model.ReportYear >= 2018 && (model.SEC_User1.FSCode != null && model.SEC_User1.FSCode == 3))
                        {
                            ASubjectExportToPdfGuAll(model, katoCode);
                        }
                        else ASubjectExportToPdfAll(model, katoCode);

                        using (var sw = System.IO.File.AppendText(path))
                        {
                            sw.WriteLine("№" + item.Id + " SubFormId:" + item.FormId + " ,   " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  completed!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    #region write log    
                    using (var sw = System.IO.File.AppendText(path))
                    {
                        sw.WriteLine("№" + item.Id + " SubFormId:" + item.FormId + " ,   " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  error:" + ex.Message);
                    }

                    using (var sw = System.IO.File.AppendText(path2))
                    {
                        sw.WriteLine("№" + item.Id + " SubFormId:" + item.FormId + " ,   " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  error:" + ex.Message);
                    }
                    #endregion
                }
            }

            return View();
        }

        public string ASubjectExportToPdfAll(SUB_Form model, string kato)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/");

            Aspose.Words.License lis = new Aspose.Words.License();
            Aspose.Words.License lisdocs = new Aspose.Words.License();
            lis.SetLicense(path + "Aspose.Words.lic");

            var doc = new Aspose.Words.Document(path + "aSubjectTmplWord.docx");
            var handle = Guid.NewGuid().ToString();

            FileInfo fiTemplate = new FileInfo(path);

            string year = model.ReportYear.ToString()
                , juridicalName = string.Empty
                , strValue
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                juridicalName = model.SEC_User1.JuridicalName;
            int count = 0;
            int lastNum = 0;

            #region F-1
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 0, true);
            if (model.SEC_User1 != null)
            {
                Row row = table.LastRow;
                row.Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.ApplicationName));

                if (model.SEC_User1.Address != null)
                    row.Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Address));

                row.Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));

                if (model.SEC_User1.Post != null)
                    row.Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Post));

                row.Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.IsCvazy ? "Да" : "Нет"));

                string oked = string.Empty;

                oked = (model.SEC_User1.DIC_OKED != null) ? model.SEC_User1.DIC_OKED.FullName : "";
                row.Cells[6].FirstParagraph.AppendChild(new Run(doc, oked));

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                Aspose.Words.Tables.Table table7 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 6, true);
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;

                    table7.Rows[0].Cells[1].FirstParagraph.AppendChild(new Run(doc, responceFIO));
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
                table7.Rows[1].Cells[1].FirstParagraph.AppendChild(new Run(doc, headFIO));
            }

            #endregion

            FindReplaceOptions replaceOptions = new FindReplaceOptions();
            replaceOptions.MatchCase = true;
            replaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("_year_", year, replaceOptions);
            doc.Range.Replace("juridical_name", juridicalName, replaceOptions);
            if (model.IsRent != null && model.IsRent == true)
                doc.Range.Replace("_isarenda", "x", replaceOptions);
            else doc.Range.Replace("_isarenda", " ", replaceOptions);

            // Ф-2
            #region F-2
            {
                // Init 
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                // InsertRows(ref sheet, 16 - 2, sheet.LastRowNum, model.SubForm2Records.Count - 2); 
                startRowIndex = 14;
                Aspose.Words.Tables.Table table2 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 1, true);
                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    //row = sheet.CreateRow(1);
                    var rowNum = startRowIndex - 1 + i;

                    // 1
                    table2.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (1 + i).ToString()));

                    // 2
                    var typeResourceName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu;
                    table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, typeResourceName));

                    // 3
                    var unitName = model.SubForm2Records[i].SUB_DIC_TypeResource == null
                                   || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null
                        ? string.Empty
                        : model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu;
                    table2.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, unitName));

                    // 4
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExtractVolume.Value.ToString()));

                    // 5а
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].NotOwnSource.Value.ToString()));

                    // 5б
                    if (!model.SubForm2Records[i].LosEnergy.HasValue)
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].LosEnergy.Value.ToString()));

                    // 6
                    if (!model.SubForm2Records[i].OwnSource.HasValue)
                        table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, ""));
                    else table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].OwnSource.Value.ToString()));

                    // 7
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        table2.Rows[2 + i].Cells[7].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[7].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].TransferOtherLegal.Value.ToString()));

                    // 8
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        table2.Rows[2 + i].Cells[8].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table2.Rows[2 + i].Cells[8].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExpenceEnergy.Value.ToString()));

                    // 9
                    var note = model.SubForm2Records[i].Note ?? string.Empty;
                    table2.Rows[2 + i].Cells[9].FirstParagraph.AppendChild(new Run(doc, note));
                }

            }

            #endregion

            // Ф-3
            #region F-3
            Aspose.Words.Tables.Table table3 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);
            var kinds = new SubDicKindResourceRepository().GetAll();
            var form3 = new List<SUB_Form3Record>();
            foreach (var rptDicKindWaste in kinds)
            {
                var report = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                if (report != null)
                {
                    form3.Add(report);
                }
                else
                {
                    var kind = new SUB_Form3Record
                    {
                        KindResourceId = rptDicKindWaste.Id,
                        SUB_DIC_KindResource = rptDicKindWaste
                    };
                    form3.Add(kind);
                }
            }
            model.SubForm3Records = form3;
            startRowIndex = 11;

            for (int i = 0; i < model.SubForm3Records.Count; i++)
            {

                if (model.SubForm3Records[i].SUB_DIC_KindResource.Id == 1)
                {
                    // 4 - Потребление воды
                    if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                        table3.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                        table3.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                        table3.Rows[3].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[3].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionPrice.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                        table3.Rows[3].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[3].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportPrice.Value.ToString()));
                }


                if (model.SubForm3Records[i].SUB_DIC_KindResource.Id == 2)
                {
                    // 4 - Потребление воды
                    if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                        table3.Rows[4].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[4].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                        table3.Rows[4].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[4].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportVolume.Value.ToString()));


                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                        table3.Rows[5].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[5].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionPrice.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                        table3.Rows[5].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[5].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportPrice.Value.ToString()));
                }

                if (model.SubForm3Records[i].SUB_DIC_KindResource.Id == 3)
                {
                    // 4 - Потребление воды
                    if (!model.SubForm3Records[i].ConsumptionVolume.HasValue)
                        table3.Rows[6].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[6].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionVolume.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportVolume.HasValue)
                        table3.Rows[6].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[6].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportVolume.Value.ToString()));


                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].ConsumptionPrice.HasValue)
                        table3.Rows[7].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[7].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].ConsumptionPrice.Value.ToString()));

                    // 5 - Потери воды при транспортировке
                    if (!model.SubForm3Records[i].LosTransportPrice.HasValue)
                        table3.Rows[7].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                    else
                        table3.Rows[7].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm3Records[i].LosTransportPrice.Value.ToString()));
                }
            }

            #endregion

            // Ф-4
            #region F-4
            // проверить энергоаудит проводился или ...
            Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            if (model.IsPlan == true)
            {
                doc.Range.Replace("_isplan", "x", replaceOptions);
                doc.Range.Replace("_isnotplan", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isplan", "", replaceOptions);
                doc.Range.Replace("_isnotplan", "x", replaceOptions);
            }

            // Не проводились планы мероприятий
            if (model.IsNotEvents == false)
            {
                doc.Range.Replace("_isnotevents", "x", replaceOptions);
            }
            else doc.Range.Replace("_isnotevents", "", replaceOptions);

            // Название компании
            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 19;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table4.LastRow.Clone(true);
                    table4.Rows.Insert(i + 3, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];

                table4.Rows[i + 3].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.SUB_DIC_Event != null)
                    table4.Rows[i + 3].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_Event.NameRu));
                else
                {
                    if (record.EventName != null)
                        table4.Rows[i + 3].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EventName));
                }

                if (record.EmplPeriodStr != null)
                    table4.Rows[i + 3].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.EmplPeriodStr));
                else table4.Rows[i + 3].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                if (record.PlanExpend == null)
                    table4.Rows[i + 3].Cells[3].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.PlanExpend));

                if (record.ActualInvest == null)
                    table4.Rows[i + 3].Cells[4].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.ActualInvest.Value.ToString()));

                if (record.SUB_DIC_TypeResource == null)
                    table4.Rows[i + 3].Cells[5].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeResource.NameRu));

                if (record.InKind == null)
                    table4.Rows[i + 3].Cells[6].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[6].FirstParagraph.AppendChild(new Run(doc, record.InKind.Value.ToString()));

                if (record.InMoney == null)
                    table4.Rows[i + 3].Cells[7].FirstParagraph.AppendChild(new Run(doc, ""));
                else
                    table4.Rows[i + 3].Cells[7].FirstParagraph.AppendChild(new Run(doc, record.InMoney.Value.ToString()));
            }
            #endregion

            // Ф-5
            #region F-5
            Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 12;
            count = model.SubForm5Records.Count;

            if (count > 1)
            {

                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table5.LastRow.Clone(true);
                    table5.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];

                table5.Rows[i + 2].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.sub_dic_energyindicator != null)
                    table5.Rows[i + 2].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.IndicatorName))
                        table5.Rows[i + 2].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));
                }

                if (!string.IsNullOrWhiteSpace(record.RegularStandart))
                    table5.Rows[i + 2].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.RegularStandart));

                //----
                if (record.UnitMeasure != null)
                    table5.Rows[i + 2].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));
                else table5.Rows[i + 2].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                //----
                if (record.CalcFormula != null)
                    table5.Rows[i + 2].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));
                else table5.Rows[i + 2].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                if (record.EnergyValue == null)
                    table5.Rows[i + 2].Cells[5].FirstParagraph.AppendChild(new Run(doc, ""));
                else table5.Rows[i + 2].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));
            }
            #endregion

            // Ф-6
            #region F-6
            Aspose.Words.Tables.Table table6 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 5, true);
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            startRowIndex = 11;
            count = model.SubForm6Records.Count;

            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table6.LastRow.Clone(true);
                    table6.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];
                table6.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.SUB_DIC_TypeCounter != null)
                    table6.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));

                if (record.CountDevice != null)
                    table6.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountDevice.Value.ToString()));

                if (record.Equipment != null)
                    table6.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Equipment.Value.ToString()));
            }
            #endregion

            #region check company name
            while (juridicalName.IndexOf("\"") != -1)
            {
                juridicalName = juridicalName.Replace("\"", "");
            }

            while (juridicalName.IndexOf("/") != -1)
            {
                juridicalName = juridicalName.Replace("/", "");
            }

            while (juridicalName.IndexOf("|") != -1)
            {
                juridicalName = juridicalName.Replace("|", "");
            }

            while (juridicalName.IndexOf(":") != -1)
            {
                juridicalName = juridicalName.Replace(":", "");
            }

            while (juridicalName.IndexOf("?") != -1)
            {
                juridicalName = juridicalName.Replace("?", "");
            }

            while (juridicalName.IndexOf("'") != -1)
            {
                juridicalName = juridicalName.Replace("'", "");
            }

            if (!string.IsNullOrWhiteSpace(juridicalName))
            {
                if (juridicalName.Length > 100)
                    juridicalName = juridicalName.Substring(0, 99);
            }
            #endregion

            //----write qrcode in word file       
            var signXmlRow = model.SUB_FormHistory.OrderByDescending(x => x.Id).FirstOrDefault(x => x.StatusId == 2);
            if (signXmlRow != null && signXmlRow.XmlSign != null)
                AddQrCode(doc, signXmlRow.XmlSign, 7);

            //---- file path
            string _fileName = model.SEC_User1.DIC_Kato.NameRu.Substring(0, 5) + "_" + model.SubjectIDK + "_" + model.SEC_User1.BINIIN + "_" + juridicalName;
            var dirpath = Server.MapPath("~/uploads/pdf/" + model.ReportYear + "/" + kato + "/");

            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            string fullFilePath = string.Empty;
            if (!System.IO.File.Exists(dirpath + "/" + _fileName + ".pdf"))
            {
                fullFilePath = dirpath + "/" + _fileName + ".pdf";
                doc.Save(fullFilePath, SaveFormat.Pdf);
            }
            else
            {
                long time = DateTime.Now.Ticks;
                String ticks = Convert.ToString(time);
                fullFilePath = dirpath + "/" + _fileName + "-" + ticks + ".pdf";
                doc.Save(fullFilePath, SaveFormat.Pdf);
            }

            return fullFilePath;
        }

        public string ASubjectExportToPdfGuAll(SUB_Form model, string kato)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/");
            Aspose.Words.License lis = new Aspose.Words.License();
            Aspose.Words.License lisdocs = new Aspose.Words.License();
            lis.SetLicense(path + "Aspose.Words.lic");

            var doc = new Aspose.Words.Document(path + "aSubjectTmplWordGu.docx");
            var handle = Guid.NewGuid().ToString();

            FileInfo fiTemplate = new FileInfo(path);

            string year = model.ReportYear.ToString()
                , juridicalName = string.Empty
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                juridicalName = model.SEC_User1.JuridicalName;

            int count = 0;
            #region F-1
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 0, true);
            if (model.SEC_User1 != null)
            {

                table.Rows[2].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.BINIIN));
                table.Rows[2].Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.ApplicationName));

                if (model.SEC_User1.Address != null)
                    table.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Address));

                table.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));

                if (model.SEC_User1.Post != null)
                    table.Rows[2].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Post));

                string oked = string.Empty;
                oked = (model.SEC_User1.DIC_OKED != null) ? model.SEC_User1.DIC_OKED.FullName : "";
                table.Rows[2].Cells[6].FirstParagraph.AppendChild(new Run(doc, oked));

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                Aspose.Words.Tables.Table table6 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 5, true);
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;


                    table6.Rows[0].Cells[1].FirstParagraph.AppendChild(new Run(doc, responceFIO));
                    table6.Rows[1].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
            }

            #endregion

            FindReplaceOptions replaceOptions = new FindReplaceOptions();
            replaceOptions.MatchCase = true;
            replaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("_year_", year, replaceOptions);
            doc.Range.Replace("juridical_name", juridicalName, replaceOptions);
            if (model.IsRent == true)
                doc.Range.Replace("_isarenda", "x", replaceOptions);
            else doc.Range.Replace("_isarenda", " ", replaceOptions);

            // Ф-2
            #region F-2
            {
                Aspose.Words.Tables.Table table2 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 1, true);
                //----form2gu             
                var form2Gu = model.SUB_Form2Gu.FirstOrDefault();
                if (form2Gu != null)
                {
                    if (form2Gu.CountOfEmployees != null && form2Gu.CountOfEmployees != 0)
                        doc.Range.Replace("_countOfEmployees", form2Gu.CountOfEmployees.ToString(), replaceOptions);
                    else doc.Range.Replace("_countOfEmployees", "", replaceOptions);

                    if (form2Gu.CountOfStudents != null && form2Gu.CountOfStudents != 0)
                        doc.Range.Replace("_students", form2Gu.CountOfStudents.ToString(), replaceOptions);
                    else doc.Range.Replace("_students", "", replaceOptions);

                    if (form2Gu.CountOfBeds != null && form2Gu.CountOfBeds != 0)
                        doc.Range.Replace("_countOfBeds", form2Gu.CountOfBeds.ToString(), replaceOptions);
                    else doc.Range.Replace("_countOfBeds", "", replaceOptions);
                }
                else
                {
                    doc.Range.Replace("_countOfEmployees", "", replaceOptions);
                    doc.Range.Replace("_students", "", replaceOptions);
                    doc.Range.Replace("_countOfBeds", "", replaceOptions);
                }

                //----dic resource 1  
                var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);
                var SUB_Form2RecordGuList = new List<SUB_Form2RecordGu>();
                foreach (var rptDicKindWaste in dics)
                {
                    var item = new SUB_Form2RecordGu();
                    item.TypeResourceId = rptDicKindWaste.Id;
                    item.TypeResourceName = rptDicKindWaste.Name;
                    item.TypeResourceUnitName = rptDicKindWaste.DIC_Unit.Name;

                    var form2 = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (form2 != null)
                    {
                        item.Form2RecordId = form2.Id;
                        item.ExpenceEnergy = form2.ExpenceEnergy;
                        item.NotOwnSource = form2.NotOwnSource;
                    }
                    SUB_Form2RecordGuList.Add(item);
                }

                //----dic resource 2
                var kinds = new SubDicKindResourceRepository().GetAll();
                var SUB_Form3RecordGuList = new List<SUB_Form3RecordGu>();
                foreach (var rptDicKindWaste in kinds)
                {
                    var item = new SUB_Form3RecordGu();
                    item.KindResourceId = rptDicKindWaste.Id;
                    item.KindResourceUnitName = rptDicKindWaste.DIC_Unit.Name;
                    item.KindResourceName = rptDicKindWaste.Name;

                    var form3 = model.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                    if (form3 != null)
                    {
                        item.Form3RecordId = form3.Id;
                        item.ConsumptionPrice = form3.ConsumptionPrice;
                        item.ConsumptionVolume = form3.ConsumptionVolume;
                        item.LosTransportPrice = form3.LosTransportPrice;
                        item.LosTransportVolume = form3.LosTransportVolume;
                    }

                    SUB_Form3RecordGuList.Add(item);
                }

                startRowIndex = 17;
                for (int i = 0; i < SUB_Form2RecordGuList.Count; i++)
                {
                    #region dic 1

                    // 1
                    table2.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                    // 2
                    table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].TypeResourceName));

                    // 3
                    table2.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].TypeResourceUnitName));

                    // 4
                    if (!SUB_Form2RecordGuList[i].NotOwnSource.HasValue)
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].NotOwnSource.Value.ToString()));

                    // 5
                    if (!SUB_Form2RecordGuList[i].ExpenceEnergy.HasValue)
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, SUB_Form2RecordGuList[i].ExpenceEnergy.Value.ToString()));
                    #endregion
                }
                startRowIndex = 32;
                for (int i = 0; i < SUB_Form3RecordGuList.Count; i++)
                {
                    #region dic 2
                    // 1
                    table2.Rows[17 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (16 + i).ToString()));

                    // 2                  
                    if (SUB_Form3RecordGuList[i].KindResourceName != null)
                        table2.Rows[17 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].KindResourceName));
                    else table2.Rows[17 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                    // 3
                    if (SUB_Form3RecordGuList[i].KindResourceUnitName != null)
                        table2.Rows[17 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].KindResourceUnitName));
                    else table2.Rows[17 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                    // 4
                    if (!SUB_Form3RecordGuList[i].ConsumptionVolume.HasValue)
                        table2.Rows[17 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[17 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].ConsumptionVolume.Value.ToString()));

                    // 5
                    if (!SUB_Form3RecordGuList[i].LosTransportVolume.HasValue)
                        table2.Rows[17 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[17 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, SUB_Form3RecordGuList[i].LosTransportVolume.Value.ToString()));
                    #endregion
                }

            }

            #endregion

            // Ф-3
            #region F-3
            Aspose.Words.Tables.Table table3 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);

            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                doc.Range.Replace("_isplan", "x", replaceOptions);
                doc.Range.Replace("_isnotplan", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isplan", "", replaceOptions);
                doc.Range.Replace("_isnotplan", "x", replaceOptions);
            }

            //  система энергоменеджмента внедрена или ...
            if (model.IsEnergyManagementSystem == true)
            {
                doc.Range.Replace("_isenergymanagementsystem", "x", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isenergymanagementsystem", "", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "x", replaceOptions);
            }

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 14;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table3.LastRow.Clone(true);
                    table3.Rows.Insert(i + 3, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                table3.Rows[3 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                //
                if (record.SUB_DIC_Event != null)
                    table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_Event.NameRu));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.EventName))
                        table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EventName));
                }

                //
                if (record.EmplPeriodStr != null)
                    table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.EmplPeriodStr));
                else table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                //
                if (record.ActualInvest != null)
                    table3.Rows[3 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.ActualInvest.Value.ToString()));

                //
                if (record.SUB_DIC_TypeCounter != null)
                    table3.Rows[3 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));

                //
                if (record.InKind != null)
                    table3.Rows[3 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.InKind.Value.ToString()));

                //
                if (record.InMoney != null)
                    table3.Rows[3 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, record.InMoney.Value.ToString()));
            }

            #endregion

            // Ф-4
            #region F-4
            Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            var form3Gu = model.SUB_Form3Gu.FirstOrDefault();
            if (form3Gu != null)
            {
                #region form3gu       

                if (form3Gu.YearOfConstruction != null)
                    doc.Range.Replace("_yearOfConstruction", form3Gu.YearOfConstruction.ToString(), replaceOptions);
                else doc.Range.Replace("_yearOfConstruction", "", replaceOptions);

                //
                if (form3Gu.AutomateItem != null)
                {
                    if (form3Gu.AutomateItem == 1)
                        doc.Range.Replace("_automateItem", "да", replaceOptions);
                    else doc.Range.Replace("_automateItem", "нет", replaceOptions);
                }
                else
                {
                    doc.Range.Replace("_automateItem", "нет", replaceOptions);
                }

                //
                if (form3Gu.TotalAreaOfBuilding != null)
                    doc.Range.Replace("_totalAreaOfBuilding", form3Gu.TotalAreaOfBuilding.ToString(), replaceOptions);
                else doc.Range.Replace("_totalAreaOfBuilding", "", replaceOptions);

                //
                if (form3Gu.HeatedAreaOfBuilding != null)
                    doc.Range.Replace("_heatedAreaOfBuilding", form3Gu.HeatedAreaOfBuilding.ToString(), replaceOptions);
                else doc.Range.Replace("_heatedAreaOfBuilding", "", replaceOptions);

                // центральное отопление
                if (form3Gu.CentralHeating != null && form3Gu.CentralHeating == 1)
                {
                    doc.Range.Replace("_central", "x", replaceOptions);
                }
                else doc.Range.Replace("_central", "", replaceOptions);

                // автономное отопление
                if (form3Gu.IndependentHeating != null && form3Gu.IndependentHeating == 1)
                {
                    doc.Range.Replace("_independent", "x", replaceOptions);
                }
                else doc.Range.Replace("_independent", "", replaceOptions);
                #endregion
            }
            else
            {
                doc.Range.Replace("_yearOfConstruction", "", replaceOptions);
                doc.Range.Replace("_automateItem", "нет", replaceOptions);
                doc.Range.Replace("_totalAreaOfBuilding", "", replaceOptions);
                doc.Range.Replace("_heatedAreaOfBuilding", "", replaceOptions);
                doc.Range.Replace("_central", "", replaceOptions);
                doc.Range.Replace("_independent", "", replaceOptions);
            }

            // get data for 
            model.SubForm5Records = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
                model.SubForm5Records.Add(record);

            startRowIndex = 18;
            count = model.SubForm5Records.Count;
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table4.LastRow.Clone(true);
                    table4.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm5Records.Count; i++)
            {
                var record = model.SubForm5Records[i];

                table4.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.sub_dic_energyindicator != null)
                    table4.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.IndicatorName))
                        table4.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));
                }

                if (record.UnitMeasure != null)
                {
                    table4.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));
                }
                else
                {
                    table4.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                }

                if (record.CalcFormula != null)
                {
                    table4.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));
                }
                else
                {
                    table4.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                }

                if (record.EnergyValue == null)
                    table4.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                else
                    table4.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));
            }

            #endregion

            // Ф-5
            #region F-5
            Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
                model.SubForm6Records.Add(record);

            count = model.SubForm6Records.Count;
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table5.LastRow.Clone(true);
                    table5.Rows.Insert(i + 2, cloneRow);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var record = model.SubForm6Records[i];

                table5.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                if (record.SUB_DIC_TypeCounter != null)
                    table5.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));
                else
                    table5.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, ""));

                if (record.CountDevice != null)
                    table5.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountDevice.Value.ToString()));
                else
                    table5.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, ""));

                if (record.Equipment != null)
                    table5.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Equipment.Value.ToString()));
            }


            #endregion

            #region check companyName
            while (juridicalName.IndexOf("\"") != -1)
            {
                juridicalName = juridicalName.Replace("\"", "");
            }

            while (juridicalName.IndexOf("/") != -1)
            {
                juridicalName = juridicalName.Replace("/", "");
            }

            while (juridicalName.IndexOf("|") != -1)
            {
                juridicalName = juridicalName.Replace("|", "");
            }

            while (juridicalName.IndexOf("?") != -1)
            {
                juridicalName = juridicalName.Replace("?", "");
            }

            while (juridicalName.IndexOf(":") != -1)
            {
                juridicalName = juridicalName.Replace(":", "");
            }

            while (juridicalName.IndexOf("'") != -1)
            {
                juridicalName = juridicalName.Replace("'", "");
            }

            if (!string.IsNullOrWhiteSpace(juridicalName))
            {
                if (juridicalName.Length > 100)
                    juridicalName = juridicalName.Substring(0, 99);
            }
            #endregion

            var signXmlRow = model.SUB_FormHistory.OrderByDescending(x => x.Id).FirstOrDefault(x => x.StatusId == 2);
            if (signXmlRow != null && signXmlRow.XmlSign != null)
                AddQrCode(doc, signXmlRow.XmlSign, 6);

            string _fileName = model.SEC_User1.DIC_Kato.NameRu.Substring(0, 5) + "_" + model.SubjectIDK + "_" + model.SEC_User1.BINIIN + "_" + juridicalName;

            //----
            var dirpath = Server.MapPath("~/uploads/pdf/" + model.ReportYear + "/" + kato + "/");

            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            string fullFilePath = string.Empty;
            if (!System.IO.File.Exists(dirpath + "/" + _fileName + ".pdf"))
            {
                fullFilePath = dirpath + "/" + _fileName + ".pdf";
                doc.Save(fullFilePath, SaveFormat.Pdf);
            }
            else
            {
                long time = DateTime.Now.Ticks;
                String ticks = Convert.ToString(time);
                fullFilePath = dirpath + "/" + _fileName + "-" + ticks + ".pdf";
                doc.Save(fullFilePath, SaveFormat.Pdf);
            }

            return fullFilePath;
        }

        public string ASubjectExportToPdfGuNewAll(SUB_Form model, string kato)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/");
            Aspose.Words.License lis = new Aspose.Words.License();
            Aspose.Words.License lisdocs = new Aspose.Words.License();
            lis.SetLicense(path + "Aspose.Words.lic");

            var doc = new Aspose.Words.Document(path + "aSubjectTmplWordGuNew.docx");
            var handle = Guid.NewGuid().ToString();

            FileInfo fiTemplate = new FileInfo(path);
            var repository = new SubFormRepository();

            string year = model.ReportYear.ToString()
                , juridicalName = string.Empty
                , responceFIO = string.Empty
                , headFIO = string.Empty;
            int startRowIndex = 0;

            if (model.SEC_User1 != null && !string.IsNullOrEmpty(model.SEC_User1.JuridicalName))
                juridicalName = model.SEC_User1.JuridicalName;

            int count = 0;
            #region F-1
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 0, true);
            if (model.SEC_User1 != null)
            {

                table.Rows[2].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.BINIIN));
                table.Rows[2].Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.ApplicationName));
                if (model.SEC_User1.Address != null)
                    table.Rows[2].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Address));

                table.Rows[2].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));

                if (model.SEC_User1.Post != null)
                    table.Rows[2].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.Post));

                string oked = string.Empty;
                oked = (model.SEC_User1.DIC_OKED != null) ? model.SEC_User1.DIC_OKED.FullName : "";
                table.Rows[2].Cells[6].FirstParagraph.AppendChild(new Run(doc, oked));

                // Полностью ФИО, должность, контакты и подпись ответственного лица
                Aspose.Words.Tables.Table table8 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 8, true);
                if (!string.IsNullOrEmpty(model.SEC_User1.ResponceFIO))
                {
                    responceFIO = model.SEC_User1.ResponceFIO;
                    if (!string.IsNullOrEmpty(model.SEC_User1.ResponcePost))
                        responceFIO += ", " + model.SEC_User1.ResponcePost;

                    if (!string.IsNullOrEmpty(model.SEC_User1.ContactInfo))
                        responceFIO += ", " + model.SEC_User1.ContactInfo;


                    table8.Rows[0].Cells[1].FirstParagraph.AppendChild(new Run(doc, responceFIO));
                    table8.Rows[1].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SEC_User1.FullName));
                }

                //  Полностью ФИО, подпись руководителя организации
                headFIO = model.SEC_User1.FullName;
            }

            #endregion

            FindReplaceOptions replaceOptions = new FindReplaceOptions();
            replaceOptions.MatchCase = true;
            replaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("_year_", year, replaceOptions);
            doc.Range.Replace("juridical_name", juridicalName, replaceOptions);

            if (model.IsRent == true)
                doc.Range.Replace("_isarenda", "x", replaceOptions);
            else doc.Range.Replace("_isarenda", " ", replaceOptions);

            // Ф-2
            #region F-2
            {
                Aspose.Words.Tables.Table table2 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 1, true);
                var dics = new SubDicTypeResourceRepository().GetAll().OrderBy(e => e.Id);
                var subForm2Records = new List<SUB_Form2Record>();
                foreach (var rptDicKindWaste in dics)
                {
                    var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                    if (report != null)
                    {
                        subForm2Records.Add(report);
                    }
                    else
                    {
                        var kind = new SUB_Form2Record
                        {
                            TypeResourceId = rptDicKindWaste.Id,
                            SUB_DIC_TypeResource = rptDicKindWaste
                        };
                        subForm2Records.Add(kind);
                    }
                }
                model.SubForm2Records = subForm2Records.OrderBy(s => s.SUB_DIC_TypeResource.Id).ToList();

                for (int i = 0; i < model.SubForm2Records.Count; i++)
                {
                    #region for

                    // 1
                    table2.Rows[2 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                    // 2
                    if (model.SubForm2Records[i].SUB_DIC_TypeResource == null || model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit == null)
                        table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].SUB_DIC_TypeResource.NameRu));

                    // 3
                    table2.Rows[2 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].SUB_DIC_TypeResource.DIC_Unit.NameRu));

                    // 4
                    if (!model.SubForm2Records[i].NotOwnSource.HasValue)
                        table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else table2.Rows[2 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].NotOwnSource.Value.ToString()));

                    // 5
                    if (!model.SubForm2Records[i].ExpenceEnergy.HasValue)
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExpenceEnergy.Value.ToString()));

                    // 5
                    if (!model.SubForm2Records[i].ExtractVolume.HasValue)
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].ExtractVolume.Value.ToString()));

                    // 6
                    if (!model.SubForm2Records[i].TransferOtherLegal.HasValue)
                        table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table2.Rows[2 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, model.SubForm2Records[i].TransferOtherLegal.Value.ToString()));

                    #endregion
                }

            }
            #endregion

            // Ф-3
            #region F-3
            Aspose.Words.Tables.Table table31 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);
            if (model.SUB_Form3GuRecord != null)
            {
                count = model.SUB_Form3GuRecord.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table31.LastRow.Clone(true);
                        table31.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var item in model.SUB_Form3GuRecord)
                {
                    // 0
                    table31.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    // 1
                    if (item.CountOfBuildings == null)
                        table31.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, item.CountOfBuildings.ToString()));

                    // 2
                    if (item.YearOfConstruction == null)
                        table31.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, item.YearOfConstruction.ToString()));

                    // 3
                    if (item.AutomateItem == null)
                    {
                        table31.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, "Нет"));
                    }
                    else
                    {
                        if (item.AutomateItem == 1)
                        {
                            table31.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, "Да"));
                        }
                        else
                        {
                            table31.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, "Нет"));
                        }
                    }

                    // 4
                    if (item.TotalAreaOfBuilding == null)
                        table31.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, item.TotalAreaOfBuilding.ToString()));

                    // 5
                    if (item.HeatedAreaOfBuilding == null)
                        table31.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, item.HeatedAreaOfBuilding.ToString()));

                    // 6
                    if (item.CountOfEmployees == null)
                        table31.Rows[2 + startRowIndex].Cells[6].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[6].FirstParagraph.AppendChild(new Run(doc, item.CountOfEmployees.ToString()));

                    // 7
                    if (item.CountOfStudents == null)
                        table31.Rows[2 + startRowIndex].Cells[7].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[7].FirstParagraph.AppendChild(new Run(doc, item.CountOfStudents.ToString()));

                    // 8
                    if (item.CountOfBeds == null)
                        table31.Rows[2 + startRowIndex].Cells[8].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table31.Rows[2 + startRowIndex].Cells[8].FirstParagraph.AppendChild(new Run(doc, item.CountOfBeds.ToString()));

                    startRowIndex++;
                }
            }

            Aspose.Words.Tables.Table table3 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 3, true);
            // проверить энергоаудит проводился или ...
            if (model.IsPlan == true)
            {
                doc.Range.Replace("_isplan", "x", replaceOptions);
                doc.Range.Replace("_isnotplan", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isplan", "", replaceOptions);
                doc.Range.Replace("_isnotplan", "x", replaceOptions);
            }

            //  система энергоменеджмента внедрена или ...
            if (model.IsEnergyManagementSystem == true)
            {
                doc.Range.Replace("_isenergymanagementsystem", "x", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "", replaceOptions);
            }
            else
            {
                doc.Range.Replace("_isenergymanagementsystem", "", replaceOptions);
                doc.Range.Replace("_isnotenergymanagementsystem", "x", replaceOptions);
            }

            model.SubForm4Records = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubForm4Records.Add(record);
            }

            startRowIndex = 14;
            count = model.SubForm4Records.Count;

            // insert
            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    var cloneRow = table3.LastRow.Clone(true);
                    table3.Rows.Insert(i + 3, cloneRow);
                }
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                var record = model.SubForm4Records[i];
                table3.Rows[3 + i].Cells[0].FirstParagraph.AppendChild(new Run(doc, (i + 1).ToString()));

                //
                if (record.SUB_DIC_Event != null)
                    table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_Event.NameRu));
                else
                {
                    if (!string.IsNullOrWhiteSpace(record.EventName))
                        table3.Rows[3 + i].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EventName));
                }

                //
                if (record.EmplPeriodStr != null)
                    table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.EmplPeriodStr));
                else table3.Rows[3 + i].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));

                //
                if (record.ActualInvest != null)
                    table3.Rows[3 + i].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.ActualInvest.Value.ToString()));

                //
                if (record.SUB_DIC_TypeCounter != null)
                    table3.Rows[3 + i].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.SUB_DIC_TypeCounter.NameRu));

                //
                if (record.InKind != null)
                    table3.Rows[3 + i].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.InKind.Value.ToString()));

                //
                if (record.InMoney != null)
                    table3.Rows[3 + i].Cells[6].FirstParagraph.AppendChild(new Run(doc, record.InMoney.Value.ToString()));
            }

            #endregion

            // Ф-3а
            #region Ф-3а
            //----1
            Aspose.Words.Tables.Table table4 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 4, true);
            if (model.SUB_Form5Record != null)
            {
                count = model.SUB_Form5Record.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table4.LastRow.Clone(true);
                        table4.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form5Record.OrderBy(e => e.Id))
                {
                    // 0
                    table4.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    if (record.TypeOfHeating == null)
                    {
                        table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        if (record.TypeOfHeating == 1)
                        {
                            table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, "Центральное отопление"));
                        }
                        else if (record.TypeOfHeating == 2)
                        {
                            table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, "Автономное отопление"));
                        }
                        else
                        {
                            table4.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                    }

                    if (record.sub_dic_energyindicator != null)
                        table4.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.sub_dic_energyindicator.nameru));
                    else if (record.IndicatorName == null)
                        table4.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.IndicatorName));

                    if (record.UnitMeasure == null)
                        table4.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.UnitMeasure));

                    if (record.CalcFormula == null)
                        table4.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.CalcFormula));

                    if (record.EnergyValue == null)
                        table4.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    else
                        table4.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.EnergyValue.Value.ToString()));

                    startRowIndex++;
                }
            }

            //----2
            Aspose.Words.Tables.Table table5 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 5, true);
            if (model.SUB_Form3aGuRecord1 != null)
            {

                count = model.SUB_Form3aGuRecord1.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table5.LastRow.Clone(true);
                        table5.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form3aGuRecord1.OrderBy(e => e.Id))
                {
                    // 0
                    table5.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    // 1
                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DIC_GU.NameRu));
                        }
                    }
                    else
                    {
                        if (record.SourceName == null)
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table5.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.SourceName));
                        }
                    }

                    // 2
                    if (record.CountOfHeatingSources == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.CountOfHeatingSources.ToString()));
                    }

                    // 3
                    if (record.CoefficientOfPerformance == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.CoefficientOfPerformance.ToString()));
                    }

                    // 4
                    if (record.PowerOfHeatingSources == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.PowerOfHeatingSources.ToString()));

                    }

                    // 5
                    if (record.YearOfCommissioning == null)
                    {
                        table5.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table5.Rows[2 + startRowIndex].Cells[5].FirstParagraph.AppendChild(new Run(doc, record.YearOfCommissioning.ToString()));
                    }

                    startRowIndex++;
                }
            }

            //----3
            Aspose.Words.Tables.Table table6 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 6, true);
            if (model.SUB_Form3aGuRecord2 != null)
            {

                count = model.SUB_Form3aGuRecord2.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table6.LastRow.Clone(true);
                        table6.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form3aGuRecord2.OrderBy(e => e.Id))
                {
                    table6.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DIC_GU.NameRu));
                        }
                    }
                    else
                    {
                        if (record.DeviceName == null)
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table6.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DeviceName));

                        }
                    }

                    // 2
                    if (record.Amount == null)
                    {
                        table6.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table6.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.Amount.ToString()));
                    }

                    // 3
                    if (record.Power == null)
                    {
                        table6.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table6.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Power.ToString()));

                    }

                    // 4
                    if (record.HoursPerDay == null)
                    {
                        table6.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table6.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.HoursPerDay.ToString()));

                    }

                    startRowIndex++;
                }
            }

            //----4
            Aspose.Words.Tables.Table table7 = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 7, true);
            if (model.SUB_Form3aGuRecord3 != null)
            {

                count = model.SUB_Form3aGuRecord3.Count;
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        var cloneRow = table7.LastRow.Clone(true);
                        table7.Rows.Insert(i + 3, cloneRow);
                    }
                }

                startRowIndex = 0;
                foreach (var record in model.SUB_Form3aGuRecord3.OrderBy(e => e.Id))
                {
                    //
                    table7.Rows[2 + startRowIndex].Cells[0].FirstParagraph.AppendChild(new Run(doc, (startRowIndex + 1).ToString()));

                    if (record.KindIndex == 1)
                    {
                        if (record.DicId == null)
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.DIC_GU.NameRu));
                        }
                    }
                    else
                    {
                        if (record.EnergyConsumEquipName == null)
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                        }
                        else
                        {
                            table7.Rows[2 + startRowIndex].Cells[1].FirstParagraph.AppendChild(new Run(doc, record.EnergyConsumEquipName));
                        }
                    }

                    // 2
                    if (record.Amount == null)
                    {
                        table7.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table7.Rows[2 + startRowIndex].Cells[2].FirstParagraph.AppendChild(new Run(doc, record.Amount.ToString()));
                    }

                    // 3
                    if (record.Power == null)
                    {
                        table7.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table7.Rows[2 + startRowIndex].Cells[3].FirstParagraph.AppendChild(new Run(doc, record.Power.ToString()));
                    }

                    // 4
                    if (record.HoursPerDay == null)
                    {
                        table7.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, string.Empty));
                    }
                    else
                    {
                        table7.Rows[2 + startRowIndex].Cells[4].FirstParagraph.AppendChild(new Run(doc, record.HoursPerDay.ToString()));
                    }

                    startRowIndex++;
                }
            }
            #endregion

            #region check companyName
            while (juridicalName.IndexOf("\"") != -1)
            {
                juridicalName = juridicalName.Replace("\"", "");
            }

            while (juridicalName.IndexOf("/") != -1)
            {
                juridicalName = juridicalName.Replace("/", "");
            }

            while (juridicalName.IndexOf("|") != -1)
            {
                juridicalName = juridicalName.Replace("|", "");
            }

            while (juridicalName.IndexOf("?") != -1)
            {
                juridicalName = juridicalName.Replace("?", "");
            }

            while (juridicalName.IndexOf(":") != -1)
            {
                juridicalName = juridicalName.Replace(":", "");
            }

            while (juridicalName.IndexOf("'") != -1)
            {
                juridicalName = juridicalName.Replace("'", "");
            }

            if (!string.IsNullOrWhiteSpace(juridicalName))
            {
                if (juridicalName.Length > 100)
                    juridicalName = juridicalName.Substring(0, 99);
            }
            #endregion

            var signXmlRow = model.SUB_FormHistory.OrderByDescending(x => x.Id).FirstOrDefault(x => x.StatusId == 2);
            if (signXmlRow != null && signXmlRow.XmlSign != null)
                AddQrCode(doc, signXmlRow.XmlSign, 9);

            // doc.Save(path + "output1.pdf", Aspose.Words.SaveFormat.Pdf);
            var memoryStream = new MemoryStream();

            doc.Save(memoryStream, SaveFormat.Pdf);
            memoryStream.Position = 0;
            Session[handle] = memoryStream;

            string _fileName = model.SEC_User1.DIC_Kato.NameRu.Substring(0, 5) + "_" + model.SubjectIDK + "_" + model.SEC_User1.BINIIN + "_" + juridicalName;
            //----
            var dirpath = Server.MapPath("~/uploads/pdf/" + model.ReportYear + "/" + kato + "/");

            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            string fullFilePath = string.Empty;
            if (!System.IO.File.Exists(dirpath + "/" + _fileName + ".pdf"))
            {
                fullFilePath = dirpath + "/" + _fileName + ".pdf";
                doc.Save(fullFilePath, SaveFormat.Pdf);
            }
            else
            {
                long time = DateTime.Now.Ticks;
                String ticks = Convert.ToString(time);
                fullFilePath = dirpath + "/" + _fileName + "-" + ticks + ".pdf";
                doc.Save(fullFilePath, SaveFormat.Pdf);
            }

            return fullFilePath;
        }

        #endregion
    }
}
