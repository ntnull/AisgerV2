using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Map;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using Microsoft.Ajax.Utilities;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Aisger.Controllers.Map
{
    public abstract class AMapController : ACommonController
    {
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
            var dir = Server.MapPath("~/uploads/maps/" + path);
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
        protected static void FillCollection(MAP_Application model)
        {
            model.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            model.ProjectPowers = new List<MAP_ApplicationProduct>();
            model.InKinds = new List<MAP_ApplicationProduct>();
            model.InValueTerms = new List<MAP_ApplicationProduct>();
            model.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            foreach (var activity in model.MAP_ApplicationProduct)
            {
                switch (activity.Disrciminator)
                {
                    case CodeConstManager.DISC_PRODUCT:
                        {
                            model.MapApplicationProducts.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_POWER:
                        {
                            model.ProjectPowers.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_KIND:
                        {
                            model.InKinds.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_VALUE_TERM:
                        {
                            model.InValueTerms.Add(activity);
                            break;
                        }
                }
            }
            model.MapApplicationEvents = new List<MAP_ApplicationEvent>();
            foreach (var activity in model.MAP_ApplicationEvent)
            {
                model.MapApplicationEvents.Add(activity);
            }
        }

        protected void FillViewBag(MAP_Application model)
        {
            if (model.Id == 0)
            {
                var user = new AccountRepository().GetUserById(MyExtensions.GetCurrentUserId());
                if (user != null)
                {
                    model.OkedString = FillOkeds(user);
                }
            }
            else
            {
                var user = new AccountRepository().GetUserById(model.UserId);
                if (user != null)
                {
                    model.OkedString = FillOkeds(user);
                }
            }

            if (model.MapApplicationProducts.Count == 0)
            {
                model.MapApplicationProducts.Add(new MAP_ApplicationProduct(){Disrciminator = CodeConstManager.DISC_PRODUCT});
            }
            if (model.ProjectPowers.Count == 0)
            {
                model.ProjectPowers.Add(new MAP_ApplicationProduct() { Disrciminator = CodeConstManager.DISC_POWER });
            }
            if (model.InKinds.Count == 0)
            {
                model.InKinds.Add(new MAP_ApplicationProduct() { Disrciminator = CodeConstManager.DISC_IN_KIND });
            }
            if (model.InValueTerms.Count == 0)
            {
                model.InValueTerms.Add(new MAP_ApplicationProduct() { Disrciminator = CodeConstManager.DISC_IN_VALUE_TERM });
            }
            if (model.MapApplicationEvents.Count == 0)
            {
                model.MapApplicationEvents.Add(new MAP_ApplicationEvent());
            }

            model.DesignAttachFiles = new List<string>();
            var dir = Server.MapPath("~/uploads/mapapp/" + model.Id + "/");
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');
                    string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    model.DesignAttachFiles.Add(name);
                }
            }

        }

        protected string FillOkeds(SEC_User user)
        {
            var okeds = new List<string>();
            if (user.DIC_OKED != null)
            {
                okeds.Add(user.DIC_OKED.Code);
            }
            okeds.AddRange(user.SEC_UserOked.Select(code => code.DIC_OKED.Code));
          return string.Join(", ", okeds);
        }

        [HttpGet]
        public ActionResult Design(long id, bool? isFile)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            model.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            model.ProjectPowers = new List<MAP_ApplicationProduct>();
            model.InKinds = new List<MAP_ApplicationProduct>();
            model.InValueTerms = new List<MAP_ApplicationProduct>();
            foreach (var activity in model.MAP_ApplicationProduct.Where(e=>e.Disrciminator==CodeConstManager.DISC_PRODUCT))
            {
                model.MapApplicationProducts.Add(activity);
            }
            foreach (var activity in model.MAP_ApplicationProduct.Where(e => e.Disrciminator == CodeConstManager.DISC_POWER))
            {
                model.ProjectPowers.Add(activity);
            }
            foreach (var activity in model.MAP_ApplicationProduct.Where(e => e.Disrciminator == CodeConstManager.DISC_IN_KIND))
            {
                model.InKinds.Add(activity);
            }
            foreach (var activity in model.MAP_ApplicationProduct.Where(e => e.Disrciminator == CodeConstManager.DISC_IN_VALUE_TERM))
            {
                model.InValueTerms.Add(activity);
            }
            model.MapApplicationEvents = new List<MAP_ApplicationEvent>();
            foreach (var activity in model.MAP_ApplicationEvent)
            {
                model.MapApplicationEvents.Add(activity);
            }
            var dir = Server.MapPath("~/uploads/maps/" + model.Id + "/");
            model.AttachFiles = new List<string>();
            model.DesignDate = DateTime.Now;
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
            FillDesginBag(model);
            if (isFile != null && isFile.Value)
            {
                ViewBag.MainActive = "";
                ViewBag.FilesActive = "active";
            }
            else
            {
                ViewBag.MainActive = "active";
                ViewBag.FilesActive = "";
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Design(MAP_Application model, IEnumerable<HttpPostedFileBase> files)
        {
            var repository = new MapApplicationRepository();
            if (model.Editor == null)
            {
                ModelState.AddModelError("Editor", ResourceSetting.NotEmpty);
            }
          
           /* if (string.IsNullOrEmpty(model.DesignNote))
            {
                ModelState.AddModelError("DesignNote", ResourceSetting.NotEmpty);
            }*/
            if (files != null && files.Any())
            {
                string path = Server.MapPath("~/uploads/mapapp/" + model.Id + "/");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        file.SaveAs(Path.Combine(path, file.FileName));
                    }
                }
                ViewBag.MainActive = "";
                ViewBag.FilesActive = "active";
                return RedirectToAction("Design", "MapApp", new { id = model.Id, isFile=true });
            }
          
            if (ModelState.IsValid)
            {
                var sendEmail = false;
                if (model.StatusId != null)
                {
                    var dics = new MapDicStatusRepository().GetById(model.StatusId.Value);
                    if (dics != null)
                    {
                        if (dics.Code == CodeConstManager.MAP_STATUS_REJECT ||
                            dics.Code == CodeConstManager.MAP_STATUS_ACCEPT)
                        {
                            model.FinishDate = model.DesignDate;
                            sendEmail = true;
                        }
                    }
                }
                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());

                var history = new MAP_ApplicationHistory
                {
                    CreateDate = DateTime.Now,
                    ApplcationId = model.Id,
                    StatusId = CodeConstManager.STATUS_SEND_ID,
                    UserId = MyExtensions.GetCurrentUserId()
                };
                repository.SaveHistory(history);
              /*  var dir = Server.MapPath("~/uploads/mapapp/" + model.Id + "/");
                var filesload = new string[] {};
                if (Directory.Exists(dir))
                {
                     filesload = Directory.GetFiles(dir);
                }
                if (sendEmail)
                {
                    new SendMessageManager().SendMapDesign(model.Id, filesload);
                }*/
                return RedirectToAction("Index");
            }
            else
            {
                model.SEC_User1 = new AccountRepository().GetUserById(model.UserId);
            }
            FillDesginBag(model);
            return View(model);

        }
        protected void FillDesginBag(MAP_Application model)
        {
            var repository = new MapDicStatusRepository();
            var listanimal = repository.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID && e.Code!=CodeConstManager.MAP_STATUS_PROJECT));
            ViewData["statusList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.StatusId);
            if (model.Editor == null)
            {
                model.Editor = MyExtensions.GetCurrentUserId();
            }
            ViewData["UserList"] = new SelectList(new SecUserRepository().GetEmployeesByOrgId(MyExtensions.GetCurrentUserId()), "Id", "FullName",
              model.Editor);
            FillViewBag(model);

        }
        [HttpGet]
        public ActionResult ShowDetails(long id)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            model.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            model.ProjectPowers = new List<MAP_ApplicationProduct>();
            model.InKinds = new List<MAP_ApplicationProduct>();
            model.InValueTerms = new List<MAP_ApplicationProduct>();
            model.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            foreach (var activity in model.MAP_ApplicationProduct)
            {
                switch (activity.Disrciminator)
                {
                    case CodeConstManager.DISC_PRODUCT:
                        {
                            model.MapApplicationProducts.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_POWER:
                        {
                            model.ProjectPowers.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_KIND:
                        {
                            model.InKinds.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_VALUE_TERM:
                        {
                            model.InValueTerms.Add(activity);
                            break;
                        }
                }

            }
            model.MapApplicationEvents = new List<MAP_ApplicationEvent>();
            foreach (var activity in model.MAP_ApplicationEvent)
            {
                model.MapApplicationEvents.Add(activity);
            }
            var dir = Server.MapPath("~/uploads/maps/" + model.Id + "/");
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
            FillViewBag(model);
            return View(model);
        }
        public ActionResult ExportExcel(long id)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);

            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Основное");
            ws.Column(1).Width = 50;
            ws.Column(2).Width = 100;
           
            ws.Cells["A1"].Value = "Наименование проекта";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A2"].Value = "Цель проекта";
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A3"].Value = "Место реализации (указать полный адрес места, включая улицу и дом)";
            ws.Cells["A3"].Style.Font.Bold = true;
            ws.Cells["A4"].Value = "Предполагаемый период реализации";
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A5"].Value = "Ожидаемые результаты";
            ws.Cells["A5"].Style.Font.Bold = true;
            ws.Cells["A5"].Value = "Ожидаемые результаты";
            ws.Cells["A5"].Style.Font.Bold = true;

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

            /*var reportReestrFilters = filter.RstReportReestrs as RST_ReportReestrFilter[] ??
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
*/
            FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = "report.xlsx";
            return result;
        }
   

    }
}