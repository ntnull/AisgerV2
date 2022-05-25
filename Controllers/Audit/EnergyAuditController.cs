using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Aisger.Controllers.Subject;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Constants;
using Aisger.Models.ControlModels;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.EnergyAudit;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;
using FlowDoc.Models.Repository.Dictionary;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Aisger.Controllers.Audit
{
    public class EnergyAuditController : Controller
    {
       
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var eauditRpository = new EnergyAuditRepository();
            IEnumerable<EAUDIT_Preamble> models = null;
            var userId = MyExtensions.GetCurrentUserId();
            if (userId != null)
            {
                var secUserRepository = new SecUserRepository();
                var user = secUserRepository.GetQuery(su => su.Id == userId, false, null).FirstOrDefault();
                if (user != null)
                {
                    if (user.SEC_UserKind.Any(uk => uk.DIC_KindUser.Code == UserKindCodes.EnergyAuditorConst))
                    {
                        models = eauditRpository.GetPreambleList(user.Id);
                    }
                    else if (user.SEC_Roles.Code == UserKindCodes.SpecialistDccConst)
                    {
                        models = eauditRpository.GetPreambleList(new List<string>()
                        {
                            EauditStatusConsts.Provided
                        });
                    }
                    else if (user.SEC_Roles.Code == UserKindCodes.SpecialistAcConst)
                    {
                        var statusCodes = new List<string>()
                        {
                            EauditStatusConsts.Checking,
                            EauditStatusConsts.Match,
                            EauditStatusConsts.NotMatch
                        };
                        models = eauditRpository.GetQuery(e => statusCodes.Contains(e.EAUDIT_DIC_Statuses.Code) && !e.IsDeleted && e.refInspector == userId).ToList();
                    }
                    else if (user.SEC_Roles.Code == UserKindCodes.ChiefAcConst)
                    {
                        models = eauditRpository.GetPreambleList(new List<string>()
                        {
                            EauditStatusConsts.Approved,
                            EauditStatusConsts.Checking,
                            EauditStatusConsts.Match,
                            EauditStatusConsts.NotMatch
                        });
                        models = models.OrderBy(m => m.EAUDIT_DIC_Statuses.Code);
                    }
                }
            }

            if (models == null)
                models = eauditRpository.GetPreambleList();

            return View(models);
        }

        [GerNavigateLogger]
        public ActionResult Create()
        {
            var eauditModel = new EAUDIT_Preamble()
            {
                BaseYear = DateTime.Now.Year - 1,
                ReportYear = DateTime.Now.Year,
            };
//            var eauditRepository = new EnergyAuditRepository();
//            eauditModel.DicStatusList = eauditRepository.GetDicStatuses();
            eauditModel.OperatorList = GetOperations(null);
            return View(eauditModel);
        }

        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            var eauditRepository = new EnergyAuditRepository();
            var eauditModel = eauditRepository.GetById(id);
            //eauditModel.DicStatusList = eauditRepository.GetDicStatuses();
            
            var operators = GetOperations(eauditModel);

            eauditModel.OperatorList = operators;
            return View("Create", eauditModel);
        }

        [GerNavigateLogger]
        public ActionResult ShowDetails(long id)
        {
            var eauditRepository = new EnergyAuditRepository();
            var eauditModel = eauditRepository.GetById(id);
            //eauditModel.DicStatusList = eauditRepository.GetDicStatuses();

            var operators = GetOperations(eauditModel);

            eauditModel.OperatorList = operators;
            eauditModel.IsReadOnly = true;

            if (Session["SignedValue"] != null)
                eauditModel.SignedEauditPreamble = (EAUDIT_Preamble) Session["SignedValue"];

            return View("Create", eauditModel);
        }

//        public ActionResult CreateWizard()
//        {
//            var eauditModel = new EAUDIT_Preamble()
//            {
//                BaseYear = DateTime.Now.Year - 1,
//                ReportYear = DateTime.Now.Year,
//            };
//            return View(eauditModel);
//        }
//
//        public ActionResult EditWizard(long id)
//        {
//            var eauditRepository = new EnergyAuditRepository();
//            var eauditModel = eauditRepository.GetById(id);
//            return View("CreateWizard", eauditModel);
//        }
        
        [HttpGet]
        public ActionResult LoadPreamble(long id, bool? isReadOnly)
        {
            var eaduditPreamble = new EAUDIT_Preamble()
            {
                BaseYear = DateTime.Now.Year - 1,
                ReportYear = DateTime.Now.Year,
            };
            var energyAuditRepository = new EnergyAuditRepository();
            if (id != 0)
            {
                eaduditPreamble = energyAuditRepository.GetById(id);
            }
            var userId = MyExtensions.GetCurrentUserId();
            if (userId != null)
            {
                var secUserRepository = new SecUserRepository();
                var user = secUserRepository.GetQuery(su => su.Id == userId, false, null).FirstOrDefault();
                if (user != null && user.SEC_UserKind.Any(uk => uk.DIC_KindUser.Code == UserKindCodes.EnergyAuditorConst))
                {
                    eaduditPreamble.refAuditor = user.Id;
                    eaduditPreamble.SEC_User1 = user;
                    eaduditPreamble.AuditorAddress = user.Address;
                    eaduditPreamble.AuditorName = user.JuridicalName;
                    // eaduditPreamble.AuditorBINIINName = user.BINIIN + " " + user.JuridicalName;
                    eaduditPreamble.AuditorBankrequisites = user.BankRequisites;
                    eaduditPreamble.AuditorFormOfIncorporation = user.DIC_TypeApplication.NameRu; // CultureHelper.GetCurrentCulture() == CultureHelper.FieldRu ? user.DIC_TypeApplication.NameRu : user.DIC_TypeApplication.NameKz;
                    eaduditPreamble.AuditorHead = user.FullName;
                    eaduditPreamble.IsCurrentUserAuditor = true;
                }
            }
            eaduditPreamble.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            if (Session["SignedValue"] != null)
                eaduditPreamble.SignedEauditPreamble = (EAUDIT_Preamble)Session["SignedValue"];

            // eaduditPreamble.DicStatusList = energyAuditRepository.GetDicStatuses();
            return PartialView("Preamble/_PreambleView", eaduditPreamble);
        }
        [HttpPost]
        public ActionResult LoadPreamble(EAUDIT_Preamble eauditPreamble)
        {
            var energyAuditRepository = new EnergyAuditRepository();
            if (eauditPreamble.Id == 0)
            {
                eauditPreamble.CreateDate = DateTime.Now;
                energyAuditRepository.Add(eauditPreamble);
            }
            else
            {
                energyAuditRepository.Update(eauditPreamble);
            }
            eauditPreamble = new EnergyAuditRepository().GetById(eauditPreamble.Id);
            
            return PartialView("Preamble/_PreambleView", eauditPreamble);
            //return RedirectToAction("Edit", eauditPreamble.Id);
        }
        
        public JsonResult SavePreambleAsync(EAUDIT_Preamble eauditPreamble)
        {
            bool isSuccess = true;
            try
            {
                var energyAuditRepository = new EnergyAuditRepository();
                if (eauditPreamble.Id == 0)
                {
                    eauditPreamble.CreateDate = DateTime.Now;
                    var status = energyAuditRepository.GetDicStatusByCode(EauditStatusConsts.Created);
                    if (status != null)
                    {
                        // eauditPreamble.EAUDIT_DIC_Statuses = status;
                        eauditPreamble.refDicStatus = status.Id;
                    }
                    energyAuditRepository.Add(eauditPreamble);
                }
                else
                {
                    var ePreamble = energyAuditRepository.GetById(eauditPreamble.Id);
                    ePreamble.AuditorAddress = eauditPreamble.AuditorAddress;
                    ePreamble.AuditorBankrequisites = eauditPreamble.AuditorBankrequisites;
                    ePreamble.AuditorFormOfIncorporation = eauditPreamble.AuditorFormOfIncorporation;
                    ePreamble.AuditorHead = eauditPreamble.AuditorHead;
                    ePreamble.AuditorName = eauditPreamble.AuditorName;
                    ePreamble.refAuditor = eauditPreamble.refAuditor;

                    ePreamble.EauditObjectAddress = eauditPreamble.EauditObjectAddress;
                    ePreamble.EauditObjectBankrequisites = eauditPreamble.EauditObjectBankrequisites;
                    ePreamble.EauditObjectsFormOfIncorporation = eauditPreamble.EauditObjectsFormOfIncorporation;
                    ePreamble.EauditObjectHead = eauditPreamble.EauditObjectHead;
                    ePreamble.EauditObjectName = eauditPreamble.EauditObjectName;
                    ePreamble.refEauditObject = eauditPreamble.refEauditObject;

                    ePreamble.ContractNumber = eauditPreamble.ContractNumber;
                    ePreamble.ContractDate = eauditPreamble.ContractDate;
                    ePreamble.FinishDate = eauditPreamble.FinishDate;
                    ePreamble.BaseYear = eauditPreamble.BaseYear;
                    ePreamble.ReportYear = eauditPreamble.ReportYear;

                    energyAuditRepository.Update(ePreamble);
                }

                eauditPreamble = energyAuditRepository.GetById(eauditPreamble.Id);
            }
            catch (Exception e)
            {
                isSuccess = false;
            }

            return Json(new
            {
                success = isSuccess,
                formId = eauditPreamble.Id
                // form = eauditPreamble
            }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Edit", eauditPreamble.Id);
        }
        
        // Additional

        [HttpGet]
        public ActionResult GetAuditorList(string searchTerm, int pageSize, int pageNum)
        {
            Select2PagedResult pageAuditors = new SecUserRepository()
                .GetPageSecUserByUserKind(searchTerm, pageSize, pageNum, UserKindCodes.EnergyAuditorConst);

            return new JsonpResult()
            {
                Data = pageAuditors,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public ActionResult GetEauditorObjectList(string searchTerm, int pageSize, int pageNum)
        {
            Select2PagedResult pageCompanies = new SecUserRepository().GetPageSecUserByUserKind(searchTerm, pageSize, pageNum);

            return new JsonpResult()
            {
                Data = pageCompanies,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        [HttpGet]
        public ActionResult GetSpecialistAcList(string searchTerm, int pageSize, int pageNum)
        {
            const string roleCode = UserKindCodes.SpecialistAcConst;
            Select2PagedResult pageCompanies = new SecUserRepository().GetPageSecUserByRoleCode(searchTerm, pageSize, pageNum, roleCode);

            return new JsonpResult()
            {
                Data = pageCompanies,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult GetDetailInfo(long id)
        {
            var secUserRepository = new SecUserRepository();
            var secUser = secUserRepository.GetById(id);

            return Json(new
            {
                FormOfIncorporation = secUser.DIC_TypeApplication.NameRu,
                Address = secUser.Address,
                BankRequisites = secUser.BankRequisites,
                Head = secUser.FullName,
                Name = secUser.JuridicalName,
                BIN = secUser.BINIIN
            });
        }

        public JsonResult GetUnitComplete(string prefix)
        {
            var dicUnitRepository = new DicUnitRepository();
            var unitNames =
                dicUnitRepository.GetQuery(du => du.NameRu.ToLower().StartsWith(prefix.ToLower()), false, null)
                    .Select(du => new {Name = du.NameRu}).OrderBy(n => n.Name);

            return Json(unitNames, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetFileUploader(long preambleId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            if (preamble != null)
                preamble.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            return PartialView("Preamble/_UploadFilesView", preamble);
        }
        
        /// <summary>
        /// Загрузка файла
        /// </summary>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileUpload(long id, IEnumerable<HttpPostedFileBase> files)
        {
            string path = Server.MapPath("~/uploads/energyaudit/" + id + "/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var repository = new EnergyAuditRepository();
            var preamble = repository.GetById(id);


            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(Path.Combine(path, file.FileName));
                    
                    preamble.EAUDIT_AttachedFiles.Add(new EAUDIT_AttachedFiles()
                    {
                        UploadDatetime = DateTime.Now,
                        refPreamble = id,
                        FileName = file.FileName,
                        Path = path,
                        EAUDIT_Preamble = preamble
                    });
                    
                }
            }
            repository.Update(preamble);
            var eauditModel = repository.GetById(id);
            return RedirectToAction("Edit", "EnergyAudit", new { id = id });
        }

        public FileResult FileDownload(long id, long refPreamble)
        {
            string path = Server.MapPath("~/uploads/energyaudit/" + refPreamble + "/");
            var repository = new EnergyAuditRepository();
            var preamble = repository.GetById(refPreamble);

            var fileInfo = preamble.EAUDIT_AttachedFiles.FirstOrDefault(af => af.Id == id);
            if (fileInfo == null)
            {
                return null;
            }
            
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(path, fileInfo.FileName));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileInfo.FileName);  
        }

        public JsonResult FileRemove(long id, long refPreamble)
        {
            string path = Server.MapPath("~/uploads/energyaudit/" + refPreamble + "/");
            var repository = new EnergyAuditRepository();
            var preamble = repository.GetById(refPreamble);
            var isSuccess = true;

            var fileInfo = preamble.EAUDIT_AttachedFiles.FirstOrDefault(af => af.Id == id);
            if (fileInfo != null)
            {
                try
                {
                    if (System.IO.File.Exists(path + fileInfo.FileName))
                    {
                        System.IO.File.Delete(path + fileInfo.FileName);
                    }
                    isSuccess = repository.RemoveAttachedFileData(fileInfo);

                    //preamble.EAUDIT_AttachedFiles.Remove(fileInfo);
                    //repository.Update(preamble);
                }
                catch (Exception e)
                {
                    isSuccess = false;
                }
            }
            else
            {
                isSuccess = false;
            }

            return Json(new
            {
               IsSuccess = isSuccess,
            }, JsonRequestBehavior.AllowGet); 
        }


        /// <summary>
        /// Загрузка приложения 1
        /// </summary>
        /// <param name="preambleId"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public ActionResult LoadApplication1(long? preambleId, bool? isReadOnly)
        {
            var repository = new EnergyAuditRepository();
            var preamble = preambleId.HasValue ? repository.GetById(preambleId.Value) : null;
            if (preamble != null)
            {
                preamble.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            }
            return PartialView("Preamble/_IndustryFormsView", preamble);
        }

        /// <summary>
        /// Загрузка приложения 2
        /// </summary>
        /// <param name="preambleId"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public ActionResult LoadApplication2(long? preambleId, bool? isReadOnly)
        {
            var repository = new EnergyAuditRepository();
            var preamble = preambleId.HasValue ? repository.GetById(preambleId.Value) : null;
            if (preamble != null)
            {
                preamble.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            }
            return PartialView("Preamble/_BuildingFormsView", preamble);
        }

        /// <summary>
        /// Загрузка приложения 3
        /// </summary>
        /// <param name="preambleId"></param>
        /// <returns></returns>
        public ActionResult LoadApplication3(long? preambleId, bool? isReadOnly)
        {
            var repository = new EnergyAuditRepository();
            var preamble = preambleId.HasValue ? repository.GetById(preambleId.Value) : null;
            if (preamble != null)
            {
                preamble.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            }
            return PartialView("Preamble/_IndustryBuildingFormsView", preamble);
        }
        
        public ActionResult GetFieldComments(string formCode, long rowId, string fieldName)
        {
            var repository = new EnergyAuditRepository();
            var model = new EauditFieldCommentViewModel();

            model.FieldCommentHistory = repository.GetFieldComments(formCode, rowId, fieldName);

            return PartialView("Preamble/_FieldCommentsPartialView", model);
        }

        public JsonResult SaveFieldComment(EAUDIT_FieldComments fieldComment)
        {
            var repository = new EnergyAuditRepository();
            
            var userId = MyExtensions.GetCurrentUserId();
            if (userId != null)
            {
                var secUserRepository = new SecUserRepository();
                var user = secUserRepository.GetQuery(su => su.Id == userId, false, null).FirstOrDefault();
                if (user != null)
                {
                    fieldComment.UserName = user.FullName;
                }
            }
            fieldComment.DatetimeStamp = DateTime.Now;
            bool isSuccess = repository.SaveFieldComments(fieldComment);
            
            return Json(new { isSuccess }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ImportConclutions(ImportExcelModel excelModel)
        {
            try
            {
                IWorkbook book = new XSSFWorkbook(excelModel.FileContent.InputStream);
                ISheet sheet = null;
                IRow row = null;

                var repository = new EnergyAuditRepository();
                var suRepo = new SecUserRepository();
                int startRowIndex = 0;

                sheet = book.GetSheetAt(0);
                var rowIndex = startRowIndex = 2;
                var preambles = new List<EAUDIT_Preamble>();
                
                while (sheet.GetRow(rowIndex) != null)
                {
                    row = sheet.GetRow(rowIndex++);
                    //all cells are empty, so is a 'blank row'
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) break;
                    if (row.Cells[0].CellType != CellType.Blank && row.Cells.Skip(1).All(d => d.CellType == CellType.Blank))
                        continue;
                    var record = new EAUDIT_Preamble()
                    {
                        CreateDate = DateTime.Now,
                        IsDeleted =  false,
                        refAuditor = null
                    };
                    
                    string idk = row.Cells[2].StringCellValue;
                    if (!string.IsNullOrEmpty(idk))
                    {
                        var user = suRepo.GetQuery(su => su.IDK == idk).FirstOrDefault();
                        if (user != null)
                        {
                            bool isSuccess = true;
                            try
                            {
                                record.EauditObjectName = user.JuridicalName;
                                record.EauditObjectAddress = user.Address;
                                record.EauditObjectBankrequisites = user.BankRequisites;
                                record.EauditObjectHead = user.FullName;
                                record.EauditObjectsFormOfIncorporation = user.DIC_TypeApplication.NameRu;
                                record.refEauditObject = user.Id;
                                // record.SEC_User = user;

                                var auditorName = row.Cells[5].StringCellValue;
                                int year = DateTime.Now.Year;
                                if (row.Cells[4].CellType == CellType.Numeric)
                                    year = Convert.ToInt32(row.Cells[4].NumericCellValue);
                                else
                                    year = Convert.ToInt32(row.Cells[4].StringCellValue);
                                var auditor = suRepo.GetQuery(su => su.JuridicalName == auditorName).FirstOrDefault();
                                if (auditor != null)
                                {
                                    record.refAuditor = auditor.Id;
                                    record.AuditorName = user.JuridicalName;
                                    record.AuditorAddress = user.Address;
                                    record.AuditorBankrequisites = user.BankRequisites;
                                    record.AuditorHead = user.FullName;
                                    record.AuditorFormOfIncorporation = user.DIC_TypeApplication.NameRu;
                                    record.refAuditor = user.Id;
                                    record.SEC_User = user;
                                }
                                else
                                {
                                    record.AuditorName = auditorName;
                                }
                                // record.IDK = row.Cells[2].StringCellValue;
                                record.ReportYear = year;
                            }
                            catch (Exception e)
                            {
                                isSuccess = false;
                                Console.WriteLine(e);
                            }
                            if (isSuccess)
                                repository.SaveOrUpdate(record, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Content(ex.ToString());
            }

            return Content("Good");
        }

        /// <summary>
        /// Получить операции по роли или типу организации
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetOperations(EAUDIT_Preamble eauditModel)
        {
            var operators = new List<SelectListItem>();
            var userId = MyExtensions.GetCurrentUserId();
            if (userId.HasValue)
            {
                var code = EauditHelper.GetUserKindCode(userId.Value);

                switch (code)
                {
                    case UserKindCodes.EnergyAuditorConst:
                        if (eauditModel != null && eauditModel.EAUDIT_DIC_Statuses != null &&
                            eauditModel.EAUDIT_DIC_Statuses.Code == EauditStatusConsts.Provided)
                        {
                            operators = null;
                        }
                        else
                        {
                            operators = new List<SelectListItem>()
                            {
                                new SelectListItem()
                                {
                                    Text =
                                        DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                            EAuditOperationConsts.Send),
                                    Value = EAuditOperationConsts.Send
                                }
                            };
                        }
                        break;
                    case UserKindCodes.SpecialistDccConst:
                        operators = new List<SelectListItem>()
                        {
                            new SelectListItem()
                            {
                                Text = DisplayNameHelper.GetFieldDisplayName(typeof(EAuditOperationConsts), EAuditOperationConsts.Return),
                                Value = EAuditOperationConsts.Return
                            },
                            new SelectListItem()
                            {
                                Text = DisplayNameHelper.GetFieldDisplayName(typeof(EAuditOperationConsts), EAuditOperationConsts.Approve),
                                Value = EAuditOperationConsts.Approve
                            }
                        };
                        break;
                    case UserKindCodes.ChiefAcConst:
                        if (eauditModel != null && eauditModel.EAUDIT_DIC_Statuses != null)
                        {
                            if (eauditModel.EAUDIT_DIC_Statuses.Code == EauditStatusConsts.Checking)
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Match),
                                        Value = EAuditOperationConsts.Match
                                    },
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.NotMatch),
                                        Value = EAuditOperationConsts.NotMatch
                                    }
                                };
                            }
                            else if (eauditModel.EAUDIT_DIC_Statuses.Code == EauditStatusConsts.Match)
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Checking),
                                        Value = EAuditOperationConsts.Checking
                                    },
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.NotMatch),
                                        Value = EAuditOperationConsts.NotMatch
                                    }
                                };
                            }
                            else if (eauditModel.EAUDIT_DIC_Statuses.Code == EauditStatusConsts.NotMatch)
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Checking),
                                        Value = EAuditOperationConsts.Checking
                                    },
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Match),
                                        Value = EAuditOperationConsts.Match
                                    },
                                };
                            }
                            else
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Checking),
                                        Value = EAuditOperationConsts.Checking
                                    }
                                };
                            }
                        }
                        break;
                    case UserKindCodes.SpecialistAcConst:
                        if (eauditModel != null && eauditModel.EAUDIT_DIC_Statuses != null)
                        {
                            if (eauditModel.EAUDIT_DIC_Statuses.Code == EauditStatusConsts.Match)
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.NotMatch),
                                        Value = EAuditOperationConsts.NotMatch
                                    }
                                };
                            }
                            else if (eauditModel.EAUDIT_DIC_Statuses.Code == EauditStatusConsts.NotMatch)
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Match),
                                        Value = EAuditOperationConsts.Match
                                    }
                                };
                            }
                            else
                            {
                                operators = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.NotMatch),
                                        Value = EAuditOperationConsts.NotMatch
                                    },
                                    new SelectListItem()
                                    {
                                        Text =
                                            DisplayNameHelper.GetFieldDisplayName(typeof (EAuditOperationConsts),
                                                EAuditOperationConsts.Match),
                                        Value = EAuditOperationConsts.Match
                                    }
                                };
                            }
                        }
                        break;
                }
            }
            return operators;
        }

        public ActionResult DoOperation(long euditPreambleId, string operationCode, long? refInspector)
        {
            var repository = new EnergyAuditRepository();
            var preamble = repository.GetPreamble(euditPreambleId);
            EAUDIT_DIC_Statuses status = null;
            bool IsSuccess = true;
            if (preamble != null)
            {
                switch (operationCode)
                {
                    case EAuditOperationConsts.Sign:
                      
                        break;
                    case EAuditOperationConsts.Send:
                        IsSuccess = true;
                        string preambleXml = string.Empty;
                        try
                        {
                            preambleXml = SerializeHelper.SerializeDataContract<EAUDIT_Preamble>(preamble);
//                            preambleXml = SerializeHelper.Serialize<EAUDIT_Preamble>(preamble);
                            preambleXml = preambleXml.Replace("utf-16", "utf-8");
                        }
                        catch (Exception e)
                        {
                            IsSuccess = false;
                        }

                        return Json(new
                        {
                            IsSuccess,
                            IsRedirect = false,
                            preambleXml,
                            Bin = MyExtensions.GetCurrentUserBIN()
                        }, JsonRequestBehavior.AllowGet);
                        break;
                    case EAuditOperationConsts.Return:
                        status = repository.GetDicStatusByCode(EauditStatusConsts.Returned);
                        if (status != null)
                            preamble.refDicStatus = status.Id;
                        break;
                    case EAuditOperationConsts.Approve:
                        status = repository.GetDicStatusByCode(EauditStatusConsts.Approved);
                        if (status != null)
                            preamble.refDicStatus = status.Id;
                        break;
                    case EAuditOperationConsts.Checking:
                        preamble.refInspector = refInspector;
                        status = repository.GetDicStatusByCode(EauditStatusConsts.Checking);
                        if (status != null)
                            preamble.refDicStatus = status.Id;
                        break;
                    case EAuditOperationConsts.Match:
                        status = repository.GetDicStatusByCode(EauditStatusConsts.Match);
                        if (status != null)
                            preamble.refDicStatus = status.Id;
                        break;
                    case EAuditOperationConsts.NotMatch:
                        status = repository.GetDicStatusByCode(EauditStatusConsts.NotMatch);
                        if (status != null)
                            preamble.refDicStatus = status.Id;
                        break;
                }
                repository.SaveOrUpdate(preamble, MyExtensions.GetCurrentUserId());
            }

            return Json(new
            {
                IsSuccess = true,
                IsRedirect = true,
                Url = Url.Action("Index", "EnergyAudit")
            }, JsonRequestBehavior.AllowGet); 
        }
        
        public ActionResult SignForm(long preambleId, string xmlAuditForm, string bin, string iin, string name)
        {
            var repository = new EnergyAuditRepository();
            var isSuccess = repository.SaveHistory(new EAUDIT_FormHistory()
            {
                refPreamble = preambleId,
                XmlSign = xmlAuditForm,
                DatetimeStamp = DateTime.Now,
                Bin = bin,
                Iin = iin,
                Name = name
            });

            // Set Status Sended
            if (isSuccess)
            {
                EAUDIT_DIC_Statuses status = repository.GetDicStatusByCode(EauditStatusConsts.Provided);
                var preamble = repository.GetById(preambleId);
                preamble.IsSigned = true;
                if (status != null)
                {
                    preamble.refDicStatus = status.Id;
                }
                repository.SaveOrUpdate(preamble, MyExtensions.GetCurrentUserId());
            }
            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowSigned(long id, bool isShowSigned)
        {
            if (isShowSigned)
            {
                var repository = new EnergyAuditRepository();
                var formHistory = repository.GetFormHistory(id);
                if (formHistory != null && !string.IsNullOrEmpty(formHistory.XmlSign))
                    Session["SignedValue"] = SerializeHelper.DeserializeDataContract<EAUDIT_Preamble>(formHistory.XmlSign);
                else
                    Session["SignedValue"] = null;
            }
            else
                Session["SignedValue"] = null;

            return RedirectToAction("ShowDetails", new {id});
        }
    }
}