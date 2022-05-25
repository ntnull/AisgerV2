using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Map;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;

namespace Aisger.Controllers.Map
{
    public class MapApplicationController : AMapController
    {
        [GerNavigateLogger]
        public ActionResult Index()
        {
            ViewBag.IsValidInfo = new SubFormRepository().GetIsValidInfo(MyExtensions.GetCurrentUserId());
            var list = new MapApplicationRepository().GetListCurrentByUser(MyExtensions.GetCurrentUserId());
            return View(list);
        }
     
        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            FillCollection(model);
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
            return View("Create", model);
        }
         
        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Copy(long id)
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return RedirectToAction("Index"); 
            var repository = new MapApplicationRepository();
            var entity = repository.GetById(id);

            if (entity.AttachFiles == null)
            {
                entity.AttachFiles = new List<string>();
            }
            entity.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            entity.ProjectPowers = new List<MAP_ApplicationProduct>();
            entity.InKinds = new List<MAP_ApplicationProduct>();
            entity.InValueTerms = new List<MAP_ApplicationProduct>();
            entity.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            foreach (var activity in entity.MAP_ApplicationProduct)
            {
                activity.Id = 0;
                switch (activity.Disrciminator)
                {

                    case CodeConstManager.DISC_PRODUCT:
                    {
                       
                            entity.MapApplicationProducts.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_POWER:
                        {
                            entity.ProjectPowers.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_KIND:
                        {
                            entity.InKinds.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_VALUE_TERM:
                        {
                            entity.InValueTerms.Add(activity);
                            break;
                        }
                }

            }
            entity.MapApplicationEvents = new List<MAP_ApplicationEvent>();
            foreach (var activity in entity.MAP_ApplicationEvent)
            {
                activity.Id = 0;
                entity.MapApplicationEvents.Add(activity);
            }
            entity.StatusId = 1;
            entity.UserId = currentUserId;
            entity.Editor = currentUserId;
            entity.SEC_User1 = new SecUserRepository().GetById(currentUserId.Value);
            entity.Id = 0;
            entity.IsCopy = true;

            FillViewBag(entity);
            return View("Create", entity);
        }
        
        public ActionResult Delete(long id)
        {
            new MapApplicationRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [GerNavigateLogger]
        public ActionResult Create()
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();
            var model = new MAP_Application
            {
                StatusId = 1,
                UserId = currentUserId,
                Editor = currentUserId,
                SEC_User1 = new SecUserRepository().GetById(currentUserId.Value),
                MapApplicationEvents = new List<MAP_ApplicationEvent>(),
                MapApplicationProducts = new List<MAP_ApplicationProduct>(),
                ProjectPowers = new List<MAP_ApplicationProduct>(),
                InKinds = new List<MAP_ApplicationProduct>(),
                InValueTerms = new List<MAP_ApplicationProduct>(),
                CurrentState = "В ожидании финансирования"
            };
            FillViewBag(model);
       
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(MAP_Application model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model.IsCopy)
            {
                model.Id = 0;
            }
            var repository = new MapApplicationRepository();
            if (ModelState.IsValid)
            {
                
                if (files == null)
                {
                    files = new List<HttpPostedFileBase>();
                }
                if (model.AttachFiles == null)
                {
                    model.AttachFiles = new List<string>();
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
                model.IsCollectionEdit = true;
                model.DesignDate = DateTime.Now;

                if (model.IsSaveSend)
                {
                    model.StatusId = CodeConstManager.STATUS_SEND_ID;
                    model.SendDate = DateTime.Now;

                    var date = DateTime.Now;
                    var holiday = 0;
                    for (int i = 1; i < CodeConstManager.CountDay + 1; i++)
                    {
                        date = date.AddDays(1);
                        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
                        if (day == DayOfWeek.Sunday || day == DayOfWeek.Wednesday)
                        {
                            holiday++;
                        }
                    }
                    model.Deadline = date.AddDays(holiday);
                    var index = new DicHolidaysRepository().CountDate(DateTime.Now, model.Deadline.Value);
                    model.Deadline = model.Deadline.Value.AddDays(index);
                   
                    repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                    model.AppNumber = repository.GetApplicationId(model.Id);
                    new SendMessageManager().SendMapRegistred(model.Id);
                    var history = new MAP_ApplicationHistory
                    {
                        CreateDate = DateTime.Now,
                        ApplcationId = model.Id,
                        StatusId = CodeConstManager.STATUS_SEND_ID,
                        UserId = MyExtensions.GetCurrentUserId()
                    };
                    repository.SaveHistory(history);
                }
                else
                {
                    repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                }
                var dirpath = Server.MapPath("~/uploads/maps/" + model.Id);
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
                if (!string.IsNullOrEmpty(model.TempPathFile))
                {
                    var dirtemp = Server.MapPath("~/uploads/maps/" + model.TempPathFile);
                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                    }
                    if (Directory.Exists(dirtemp))
                    {
                        var tempfiles = Directory.GetFiles(dirtemp);
                        foreach (var file in tempfiles)
                        {
                            if (System.IO.File.Exists(file))
                            {
                                 var fullname = file.Split('\\');
                                name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;
                                System.IO.File.Move(file, dirpath + "//" + name);
                            }
                        }
                        Directory.Delete(dirtemp);
                    }

                }

                return RedirectToAction("Index");
            }
            bool isErrorSecondPage = false;
            bool isErrorThirdPage = false;
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Count > 0)
                {
                    if (!isErrorSecondPage && state.Key.Contains("MapApplicationProducts"))
                    {
                        isErrorSecondPage = true;
                    }
                    if (!isErrorSecondPage && state.Key.Contains("ProjectPowers"))
                    {
                        isErrorSecondPage = true;
                    }
                    if (!isErrorSecondPage && state.Key.Contains("InKinds"))
                    {
                        isErrorSecondPage = true;
                    }
                    if (!isErrorSecondPage && state.Key.Contains("InValueTerms"))
                    {
                        isErrorSecondPage = true;
                    }
                    if (!isErrorSecondPage && state.Key=="TermCost")
                    {
                        isErrorSecondPage = true;
                    }
                    if (!isErrorThirdPage && state.Key.Contains("MapApplicationEvents"))
                    {
                        isErrorThirdPage = true;
                    }
                    string[] thirdList = { "OwnFonds", "BudgetFonds", "RequiredResource" };
                    if (!isErrorThirdPage && thirdList.Contains(state.Key))
                    {
                        isErrorThirdPage = true;
                    }
                }
            }
            ViewBag.IsErrorSecondPage = isErrorSecondPage ? "1" : "0";
            ViewBag.IsErrorThirdPage = isErrorThirdPage ? "1" : "0";
            if (model.Id == 0 && files != null)
            {
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
                if (string.IsNullOrEmpty(model.TempPathFile))
                {
                    model.TempPathFile = Guid.NewGuid().ToString();
                }
                var tempPath = Server.MapPath("~/uploads/maps/" + model.TempPathFile);
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                if (model.AttachFiles == null)
                {
                    model.AttachFiles = new List<string>();
                }
                var tempList = new List<string>();
                foreach (var file in model.AttachFiles)
                {
                    tempList.Add(file);
                }
                foreach (var file in httpPostedFileBases)
                {
                    if (file == null || file.ContentLength <= 0) continue;
                    var uploadFileName = Path.GetFileName(file.FileName);
                    if (uploadFileName == null) continue;
                    var uploadFilePathAndName = Path.Combine(tempPath, uploadFileName);
                    ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                    tempList.Add(uploadFileName);
                }
                model.AttachFiles = tempList;
            }

            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            FillViewBag(model);
            return View("Create", model);
        }
        [HttpGet]
        public ActionResult Send(long id)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.StatusId = CodeConstManager.STATUS_SEND_ID;
            model.SendDate = DateTime.Now;
            
            var date = DateTime.Now;
            var holiday = 0;
            for (int i = 1; i < CodeConstManager.CountDay + 1; i++)
            {
                 date = date.AddDays(1);
                 DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
                 if (day ==DayOfWeek.Sunday || day == DayOfWeek.Wednesday)
                 {
                     holiday++;
                 }
            }
            model.Deadline =date.AddDays(holiday);
            var index = new DicHolidaysRepository().CountDate(DateTime.Now, model.Deadline.Value);
            model.Deadline = model.Deadline.Value.AddDays(index);
            model.AppNumber = repository.GetApplicationId(model.Id);
            repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SendMessageManager().SendMapRegistred(model.Id);
            var history = new MAP_ApplicationHistory
            {
                CreateDate = DateTime.Now,
                ApplcationId = model.Id,
                StatusId = CodeConstManager.STATUS_SEND_ID,
                UserId = MyExtensions.GetCurrentUserId()
            };
            repository.SaveHistory(history);
            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel(long id)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);
            FillCollection(model);
            if (model == null)
            {
                return new JsonResult()
                {
                    Data = new { result = false, errorMsg = "No data for Id: " + id },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            /* var handle = Guid.NewGuid().ToString();
             var path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/mapApplication.xlsx");
             
            var fiTemplate = new FileInfo(path);*/
           

          /*  var fs =
   new FileStream(Server.MapPath(@"~/App_Data/ExcelTemplates/mapApplication.xlsx"), FileMode.Open, FileAccess.Read);*/
            string path = HttpContext.Server.MapPath("~/App_Data/ExcelTemplates/mapApplication.xlsx");
            var templateWorkbook = new XSSFWorkbook(path);
            string name = templateWorkbook.GetSheetName(0);
            var sheet = templateWorkbook.GetSheet(name);


            var user = model.SEC_User1;
            IRow row = sheet.GetRow(4);
            row.Cells[1].SetCellValue(user.ApplicationName);
            row = sheet.GetRow(5);
            row.Cells[1].SetCellValue(user.Address);
            row = sheet.GetRow(6);
            row.Cells[1].SetCellValue(user.FactAddress);
            row = sheet.GetRow(7);
            var contactInfo = "";
            if (!string.IsNullOrEmpty(user.Mobile))
            {
                contactInfo =contactInfo+ user.Mobile;
            }
            if (!string.IsNullOrEmpty(user.WorkPhone))
            {
                contactInfo = contactInfo+"/"  + user.WorkPhone;
                if (!string.IsNullOrEmpty(user.InternalPhone))
                {
                    contactInfo = contactInfo + " (" + user.InternalPhone+")";
                }
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                contactInfo = contactInfo + "/" + user.Email;
            }
            row.Cells[1].SetCellValue(contactInfo);
            row = sheet.GetRow(8);
            row.Cells[1].SetCellValue(user.BINIIN);
            row = sheet.GetRow(9);
            row.Cells[1].SetCellValue(user.Certificate);

            row = sheet.GetRow(13);
            row.Cells[1].SetCellValue(model.ProjectName);
            row = sheet.GetRow(14);
            row.Cells[1].SetCellValue(model.ProjectObjective);
            row = sheet.GetRow(15);
            row.Cells[1].SetCellValue(model.ProjectLocation);
            row = sheet.GetRow(16);
            row.Cells[1].SetCellValue(model.EstimatedPeriod);

            row = sheet.GetRow(17);
            row.Cells[1].SetCellValue(model.ExpectedResult);

            row = sheet.GetRow(18);
            row.Cells[1].SetCellValue(model.CurrentState);

            if (model.TotalCost != null)
            {
                row = sheet.GetRow(19);
                row.Cells[1].SetCellValue(model.TotalCost.Value);
            }

              row = sheet.GetRow(22);
            var okedstr = FillOkeds(user);
            row.Cells[2].SetCellValue(okedstr);

            int count = model.MapApplicationProducts.Count;
            var startRowIndex = 25;
     
            if (count > 0)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var product = model.MapApplicationProducts[i];
                    InsertRowsProduct(ref sheet, i, 1, product, startRowIndex);
                }
                IRow summtr = sheet.GetRow(startRowIndex + count-1);
                SetValueRow(model.MapApplicationProducts[count-1],summtr,1);
            }

            startRowIndex = 27+count;
            count = model.ProjectPowers.Count;

            if (count > 0)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var product = model.ProjectPowers[i];
                    InsertRowsProduct(ref sheet, i, 1, product, startRowIndex);
                }
                IRow summtr = sheet.GetRow(startRowIndex + count - 1);
                SetValueRow(model.ProjectPowers[count - 1], summtr, 1);
            }
            startRowIndex = startRowIndex + count+2;
            count = model.InKinds.Count;

            if (count > 0)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var product = model.InKinds[i];
                    InsertRowsProduct(ref sheet, i, 1, product, startRowIndex);
                }
                IRow summtr = sheet.GetRow(startRowIndex + count - 1);
                SetValueRow(model.InKinds[count - 1], summtr, 1);
            }

            startRowIndex = startRowIndex + count + 2;
            count = model.InValueTerms.Count;

            if (count > 0)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var product = model.InValueTerms[i];
                    InsertRowsProduct(ref sheet, i, 1, product, startRowIndex);
                }
                IRow summtr = sheet.GetRow(startRowIndex + count - 1);
                SetValueRow(model.InValueTerms[count - 1], summtr, 1);
            }

            FillSewcondPage(templateWorkbook, model);


            var ms = new MemoryStream();
            templateWorkbook.Write(ms);
            var reportname = "application_" + DateTime.Now.ToString("yyyy-mm-dd hh.mm.ss") + ".xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
        
        }

        private static void FillSewcondPage(XSSFWorkbook templateWorkbook, MAP_Application model)
        {
            string nameSecond = templateWorkbook.GetSheetName(1);
            var sheet = templateWorkbook.GetSheet(nameSecond);
            int count = model.MapApplicationEvents.Count;
            var startRowIndex = 2;

            if (count > 0)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var product = model.MapApplicationEvents[i];
                    InsertRowsEvent(ref sheet, i, 1, product, startRowIndex);
                }
                IRow summtr = sheet.GetRow(startRowIndex + count - 1);
                SetEventValueRow(model.MapApplicationEvents[count - 1], summtr, 1);
            }
            IRow row = sheet.GetRow(5 + count);
            if (model.OwnFonds != null) row.Cells[2].SetCellValue(model.OwnFonds.Value);

            row = sheet.GetRow(6 + count);
            if (model.BudgetFonds != null) row.Cells[2].SetCellValue(model.BudgetFonds.Value);

            row = sheet.GetRow(7 + count);
            if (model.RequiredResource != null) row.Cells[2].SetCellValue(model.RequiredResource.Value);
        }

        private static void InsertRowsEvent(ref ISheet sheet, int fromRowIndex, int rowCount, MAP_ApplicationEvent product, int start)
        {
            int i = fromRowIndex;
            fromRowIndex = fromRowIndex + start;
            sheet.ShiftRows(fromRowIndex, sheet.LastRowNum, rowCount, true, false);

            for (int rowIndex = fromRowIndex; rowIndex < fromRowIndex + rowCount; rowIndex++)
            {
                IRow rowSource = sheet.GetRow(rowIndex + rowCount);
                IRow rowInsert = sheet.CreateRow(rowIndex);
                for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
                {
                    ICell cellSource = rowSource.GetCell(colIndex);
                    ICell cellInsert = rowInsert.CreateCell(colIndex);
                    if (cellSource != null)
                    {
                        cellInsert.CellStyle = cellSource.CellStyle;
                    }

                }
                SetEventValueRow(product, rowInsert, i + 1);
            }
        }

        private static void SetEventValueRow(MAP_ApplicationEvent product, IRow rowInsert, int i)
        {
            rowInsert.GetCell(1).SetCellValue(product.EventName);
            if (product.PlanExpend != null) rowInsert.GetCell(2).SetCellValue(product.PlanExpend.Value);
            rowInsert.GetCell(3).SetCellValue(product.SavedEnergy);
            rowInsert.GetCell(4).SetCellValue(product.SavedCost);
            if (product.PaybackPeriod != null) rowInsert.GetCell(5).SetCellValue((double) product.PaybackPeriod);
            rowInsert.GetCell(6).SetCellValue(product.Note);
        }

        private void InsertRowsProduct(ref ISheet sheet, int fromRowIndex, int rowCount, MAP_ApplicationProduct product, int start)
        {
            int i = fromRowIndex;
            fromRowIndex = fromRowIndex + start;
            sheet.ShiftRows(fromRowIndex, sheet.LastRowNum, rowCount, true, false);

            for (int rowIndex = fromRowIndex; rowIndex < fromRowIndex + rowCount; rowIndex++)
            {
                IRow rowSource = sheet.GetRow(rowIndex + rowCount);
                IRow rowInsert = sheet.CreateRow(rowIndex);
                for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
                {
                    ICell cellSource = rowSource.GetCell(colIndex);
                    ICell cellInsert = rowInsert.CreateCell(colIndex);
                    if (cellSource != null)
                    {
                        cellInsert.CellStyle = cellSource.CellStyle;
                    }

                }
                SetValueRow(product, rowInsert, i + 1);
            }
        }
        static void SetValueRow(MAP_ApplicationProduct purchase, IRow dataRow, int i)
        {
            dataRow.GetCell(1).SetCellValue(purchase.ProductName);
            dataRow.GetCell(2).SetCellValue(purchase.ProductUnit);
            dataRow.GetCell(3).SetCellValue(purchase.ProductVolume);
        }
        
        [HttpPost]
        public virtual ActionResult CheckSend(long id)
        {
            var repository = new MapApplicationRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return Json(new { Success = false, ErrorMessage = ResourceSetting.RequiredNotEqual });
            }
            var sum = model.MAP_ApplicationEvent.Sum(e => e.PlanExpend) ?? 0;
            var res = model.RequiredResource ?? 0;
            var result = Equals(sum, res);

            return Json(new { Success = result, ErrorMessage = ResourceSetting.RequiredNotEqual });

        }
    }
}