using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aisger.Controllers.Action;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Repository.Action;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;
using Aisger.Models.Entity.Subject;
using System.Web.UI.WebControls;

namespace Aisger.Controllers.Subject
{
    public abstract class ASubjectController : ACommonController
    {
        SubFormRepository _subRepo;
        SubDicTypeResourceRepository _subDicTypeResRepo;
        RstReportRepository _rstRepo;
        SecUserRepository _userRepo;
        SubDicStatusRepository _statusDicRepo;
        SubDicKindTabTwoRepository _subDicKindTwoRepo;
        SubDicKindResourceRepository _subDicKindResRepo;
        SubDicEnergyindicator _subDicEnergyindicatorRepo;
        SubDicKindTabOneRepository _subDicKindTabOneRepositoryRepo;
        SubDicEventRepository _subDicEventRepositoryRepo;
        DicOkedRepository _dicOkedRepositoryRepo;
        SubDucTypeCounterRepository _subDicTypeCounterRepo;

        SubActionPlanRepository _subActionPlanRepo;
        public ASubjectController()
        {
            _subRepo = new SubFormRepository();
            _subDicTypeResRepo = new SubDicTypeResourceRepository();
            _rstRepo = new RstReportRepository();
            _userRepo = new SecUserRepository();
            _statusDicRepo = new SubDicStatusRepository();
            _subDicKindTwoRepo = new SubDicKindTabTwoRepository();
            _subDicKindResRepo = new SubDicKindResourceRepository();
            _subDicEnergyindicatorRepo = new SubDicEnergyindicator();
            _subDicKindTabOneRepositoryRepo = new SubDicKindTabOneRepository();
            _subDicEventRepositoryRepo = new SubDicEventRepository();
            _dicOkedRepositoryRepo = new DicOkedRepository();
            _subDicTypeCounterRepo = new SubDucTypeCounterRepository();
            _subActionPlanRepo = new SubActionPlanRepository();
        }

		[HttpPost]
		public virtual ActionResult UpdateUserInfo(int ReportYear, long ownerId, string fieldName, string fieldValue)
		{
            _subRepo.UpdateUserInfo(ReportYear, ownerId, fieldName, fieldValue);
			return Json(new { Success = true });
		}

        [HttpPost]
        public virtual ActionResult GetInfoReportYear(long id, int year, long modelId)
        {
            var subForm = _subRepo.GetSubFormByYearAndUserId(id, year, modelId);

            if (modelId > 0 && year>2010 && year<2030)
            {
                var model = _subRepo.GetById(modelId);
                model.ReportYear = year;
                _subRepo.SaveOrUpdate(model, id);

            }

            bool status = subForm != null;
            var message = "";
            if (status)
            {
                message = ResourceSetting.yearsExists;
            }
            else
            {
                var isReestr = _rstRepo.CheckHaveInReestr(id, year);
                if (!isReestr)
                {
                    message = ResourceSetting.InNotReestr;
                    status = true;
                }
            }

            return Json(new { Success = status, message });

        }

        [HttpPost]
        public virtual ActionResult GetInfoApplicant(long id, int year, long modelId)
        {
            var user =_userRepo.GetById(id) ?? new SEC_User();
            var subForm = _subRepo.GetSubFormByYearAndUserId(id, year, modelId);

            if (subForm != null)
            {
                return Json(new
                {
                    SubjectName = "",
                    SubjectBoss1 = "",
                    SubjectBoss = "",
                    SubjectAddress = "",
                    SubjectPost = "",
                    IsCvaziStr = "",
                    OkedList = "",
                    responceInfo = "",
                    IsCvazy = false,
                    isExist = true,
                    modelId = subForm.Id,
                });
            }
            var tmp = new SUB_Form { SEC_User1 = user };

            var builder = new StringBuilder();

            foreach (var secUserOked in user.SEC_UserOked)
            {
                builder.Append(secUserOked.DIC_OKED.NameRu).Append(",");
            }
            var nameOked = "";
            if (user.DIC_OKED != null)
            {
                nameOked = user.DIC_OKED.FullName;
            }
            return Json(new
            {
                SubjectName = tmp.SubjectName,
                SubjectBoss1 = tmp.SubjectBoss,
                SubjectBoss = tmp.SubjectBoss,
                SubjectAddress = tmp.SubjectAddress,
                SubjectPost = tmp.SubjectPost,
                IsCvaziStr = tmp.IsCvaziStr,
                IsCvazy = tmp.IsCvazy,
                ResponceFIO = user.ResponceFIO,
                ResponcePost = user.ResponcePost,
                ContactInfo = user.ContactInfo,
//                OkedList = builder.ToString(),
                OkedName = nameOked,
                responceInfo = user.ResponceFIO + ", " + user.ResponcePost,
                isExist = false
            });
        }

        [HttpPost]
        public virtual ActionResult DeleteRecord(string code, long recordId)
        {
            _subRepo.DeleteRecord(code, recordId);
            return Json(new { Success = true });
        }

        [HttpPost]
        [GerNavigateLogger]
        public virtual ActionResult UpdateModel(string code, long modelId, long userId, long editorId, long recordId, int year, string fieldName, string fieldValue, long typeId)
        {
            var filter = _subRepo.UpdateModel(code, modelId, userId, recordId, year, fieldName, fieldValue, editorId, typeId);

            if (code.Equals("form2Gu") || code.Equals("form3Gu"))
            {
                return Json(new { Success = true, formId = filter.ModelId, fromRecordId = filter.RecordId });
            }
            return Json(new { Success = true, formId = filter.ModelId, fromRecordId = filter.RecordId, unique = filter.Unique });
        }

        public ActionResult Delete(long id)
        {
            // казірше керек емес бірік истейді!!!
            //  _subRepo.Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult ShowDetails(long id)
        {       
            var model = _subRepo.GetById(id);
            if (_subRepo.GetFSCode(model.UserId.Value, model.ReportYear) == 3)   // 
            {
                if (model.ReportYear >= 2018 && model.ReportYear < 2019)
                {
                    return RedirectToAction("ShowDetailsGu", new { id = id });
                }
                else
                {
                    return RedirectToAction("ShowDetailsGuNew", new { id = id });
                }
            }

            var dicsAll = _subDicTypeResRepo.GetAll();
            model.SubDicTypeResources = dicsAll;
            //model.SubDicTypeResources = dics;
            var dics = dicsAll.OrderBy(e => e.Id);

            //FillViewBag(model);
            #region fill view bag

            
            var listanimal = IsAcceptReport() ? _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID) && (e.Id != CodeConstManager.STATUS_ACCEPT_ID));
            ViewData["statusList"] = new SelectList(listanimal, "Id",
                                                 CultureHelper.GetDictionaryName("NameRu"), model.StatusId);
            
            var years = _subRepo.GetPastYears(model.UserId).Where(e => e.ReportYear != model.ReportYear);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }

            //----
            var dicsTwo = _subDicKindTwoRepo.GetAll();
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
            model.SubFormTab3s = new List<SUB_FormTab3>();
            var forms4 = model.SUB_FormTab3.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubFormTab3s.Add(record);
            }
            if (model.SubFormTab3s.Count == 0)
            {
                model.SubFormTab3s.Add(new SUB_FormTab3());
            }

            model.SubDicKindTabOnes = _subDicKindTabOneRepositoryRepo.GetAll();
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

            //---- sub form kadastr view
            model.SubFormKadastrs = new List<SUB_FormKadastr>();
            var formsKadasre = model.SUB_FormKadastr.OrderBy(e => e.Id);
            foreach (var record in formsKadasre)
            {
                model.SubFormKadastrs.Add(record);
            }
            if (model.SubFormKadastrs.Count == 0)
            {
                model.SubFormKadastrs.Add(new SUB_FormKadastr());
            }

            model.WastList = GetPlants(model.Wastes);
            if (model.SubDicTypeResources == null)
            {
                model.SubDicTypeResources = new List<SUB_DIC_TypeResource>();
            }
            long? okedId = 0;
            if (model.SEC_User1 != null)
            {
                okedId = model.SEC_User1.OkedId;
            }

            ViewData["OKEDList"] = new SelectList(_dicOkedRepositoryRepo.GetAll(), "Id", "FullName", okedId);

            model.SubDicEnergyindicatorList = _subDicEnergyindicatorRepo.GetAll().ToList();
            string oblastName = "";
            string ErrorMessage = _subRepo.GetOblastName(model.UserId.Value, model.ReportYear, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;
            #endregion


            ViewBag.SubReadonly = true;

            if (Request.UrlReferrer != null) model.PreviousUrl = Request.UrlReferrer.ToString();
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }

            var dir = Server.MapPath("~/uploads/appform/" + model.Id + "/");
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

            model.SignedSubForm = null;
            if (Session["SignedValue"] is SUB_Form)
            {
                model.SignedSubForm = (SUB_Form)Session["SignedValue"];
            }

            //----
            var bufferRstRR = _subRepo.GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear);
            if (bufferRstRR != null)
            {
                model.Wastes = _subRepo.GetRstReportReestrOked(model.UserId.Value, model.ReportYear);

                model.SEC_User1.LastName = bufferRstRR.usrlastname;
                model.SEC_User1.SecondName = bufferRstRR.usrsecondname;
                model.SEC_User1.FirstName = bufferRstRR.usrfirstname;
                model.SEC_User1.JuridicalName = bufferRstRR.usrjuridicalname;
                model.SEC_User1.Post = bufferRstRR.usrpost;
                model.SEC_User1.Mobile = bufferRstRR.usrmobile;
                model.SEC_User1.WorkPhone = bufferRstRR.usrworkphone;
                model.SEC_User1.InternalPhone = bufferRstRR.usrinternalphone;
                model.SEC_User1.Address = bufferRstRR.usraddress;

                model.SEC_User1.IsCvazy = (bufferRstRR.usriscvazy != null) ? Convert.ToBoolean(bufferRstRR.usriscvazy) : false;
                model.SEC_User1.ResponceFIO = bufferRstRR.usrresponcefio;
                model.SEC_User1.ResponcePost = bufferRstRR.usrresponcepost;
                model.SEC_User1.Oblast = bufferRstRR.usroblast;
                model.SEC_User1.Region = bufferRstRR.usrregion;
                model.SEC_User1.SubRegion = bufferRstRR.usrsubregion;
                model.SEC_User1.Village = bufferRstRR.usrvillage;
                model.SEC_User1.TypeApplicationId = Convert.ToInt64(bufferRstRR.usrtypeapplicationid);
                model.SEC_User1.OkedId = bufferRstRR.usrokedid;
                model.SEC_User1.FSCode = bufferRstRR.usrfscode;
                model.SEC_User1.IDK = bufferRstRR.usridk;
                model.SEC_User1.Email = bufferRstRR.usremail;

                if (model.SEC_User1.OkedId != null)
                    model.SEC_User1.DIC_OKED = _dicOkedRepositoryRepo.GetAll().FirstOrDefault(x => x.Id == model.SEC_User1.OkedId.Value);
            }

            Session["ShowDetails"] = model;
            return View(model);
        }
        
        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult ShowDetailsGu(long id)
        {            
            SUB_Form _subForm = _subRepo.GetById(id);
            Sub_FormGu model = new Sub_FormGu
            {
                Id = _subForm.Id,
                StatusId = _subForm.StatusId,
                UserId = _subForm.UserId,
                ReportYear = new int?(_subForm.ReportYear),
                BeginPlanYear = _subForm.BeginPlanYear,
                EndPlanYear = _subForm.EndPlanYear,
                IsNotEvents = true,
                IsPlan = _subForm.IsPlan,
                IsEnergyManagementSystem = _subForm.IsEnergyManagementSystem,
                DesignDateStr=_subForm.DesignDateStr,
                DesignNote=_subForm.DesignNote,
                Note=_subForm.Note,
                IsRent=_subForm.IsRent
            };
            if (!model.Editor.HasValue)
            {
                model.Editor = MyExtensions.GetCurrentUserId();
            }
            model.SubDicTypeResources = _subDicTypeResRepo.GetAll();
            var _userId = model.UserId;
            var years = _subRepo.GetPastYears(_userId).Where(e =>e.ReportYear!= model.ReportYear.Value);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            var listanimal = IsAcceptReport() ? _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID) && (e.Id != CodeConstManager.STATUS_ACCEPT_ID));
            ViewData["statusList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.StatusId);

            model.SignedSubForm = null;
            if (Session["SignedValue"] is SUB_Form)
            {
                model.SignedSubForm = (SUB_Form)Session["SignedValue"];
            }

            ViewBag.SubReadonly = true;
            if (Request.UrlReferrer != null)
                model.PreviousUrl = Request.UrlReferrer.ToString();
         

            #region ---- form2

            //----dic resource 1
            var dics = _subDicTypeResRepo.GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);
            model.SUB_Form2RecordGu = new List<SUB_Form2RecordGu>();
            foreach (var rptDicKindWaste in dics)
            {
                var item = new SUB_Form2RecordGu();
                item.TypeResourceId = rptDicKindWaste.Id;
                item.TypeResourceName = rptDicKindWaste.Name;
                item.TypeResourceUnitName = rptDicKindWaste.DIC_Unit.Name;

                var form2 = _subForm.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                if (form2 != null)
                {
                    item.Form2RecordId = form2.Id;
                    item.ExpenceEnergy = form2.ExpenceEnergy;
                    item.NotOwnSource = form2.NotOwnSource;
                    item.SUB_DIC_TypeResource = form2.SUB_DIC_TypeResource;
                }
                else item.SUB_DIC_TypeResource = rptDicKindWaste;
                
                model.SUB_Form2RecordGu.Add(item);
            }

            //----dic resource 2
            var kinds = _subDicKindResRepo.GetAll();
            model.SUB_Form3RecordGu = new List<SUB_Form3RecordGu>();
            foreach (var rptDicKindWaste in kinds)
            {
                var item = new SUB_Form3RecordGu();
                item.KindResourceId = rptDicKindWaste.Id;
                item.KindResourceUnitName = rptDicKindWaste.DIC_Unit.Name;
                item.KindResourceName = rptDicKindWaste.Name;

                var form3 = _subForm.SUB_Form3Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                if (form3 != null)
                {
                    item.Form3RecordId = form3.Id;
                    item.ConsumptionPrice = form3.ConsumptionPrice;
                    item.ConsumptionVolume = form3.ConsumptionVolume;
                    item.LosTransportPrice = form3.LosTransportPrice;
                    item.LosTransportVolume = form3.LosTransportVolume;
                }

                var form6 = _subForm.SUB_Form6Record.FirstOrDefault(e => e.KindResourceId == rptDicKindWaste.Id);
                if (form6 != null)
                {
                    item.Form6RecordId = form6.Id;
                    item.CountDevice = form6.CountDevice;
                    item.Equipment = form6.Equipment;
                }

                model.SUB_Form3RecordGu.Add(item);
            }

            #endregion

            #region ---- form gu
            model.SUB_Form2Gu = (_subForm.SUB_Form2Gu.Count != 0) ? _subForm.SUB_Form2Gu.FirstOrDefault() : new SUB_Form2Gu();
            model.SUB_Form3Gu = (_subForm.SUB_Form3Gu.Count != 0) ? _subForm.SUB_Form3Gu.FirstOrDefault() : new SUB_Form3Gu();
            model.SUB_FormGuLightingInfo = _subForm.SUB_FormGuLightingInfo.ToList();
            if (model.SUB_FormGuLightingInfo.Count == 0)
            {
                model.SUB_FormGuLightingInfo = new List<SUB_FormGuLightingInfo>();
                var item = new SUB_FormGuLightingInfo();
                model.SUB_FormGuLightingInfo.Add(item);
            }

            //----       
            List<SelectListItem> newList = new List<SelectListItem>();
            SelectListItem _item1 = new SelectListItem() { Value = "0", Text = "Нет" };
            newList.Add(_item1);

            SelectListItem _item2 = new SelectListItem() { Value = "1", Text = "Да" };
            newList.Add(_item2);     
            //Add select list item to lis
            var _automateItem = (model.SUB_Form3Gu.AutomateItem == null) ? 0: model.SUB_Form3Gu.AutomateItem;
            ViewData["Forma3GuAutomateItems"] = new SelectList(newList, "Value", "Text", _automateItem);
            #endregion

            var typeCounter = _subDicTypeCounterRepo.GetAll();
            var dicevents = _subDicEventRepositoryRepo.GetAll(model.UserId);

            #region form4 gu
            model.SubForm4Records = new List<SUB_Form4Record>();
            model.SubForm4RecordsOther = new List<SUB_Form4Record>();
            var forms4 = _subForm.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                if (record.KindIndex == 2)
                {
                    model.SubForm4RecordsOther.Add(record);
                }
                else
                {
                    model.SubForm4Records.Add(record);
                }
            }

            //---- if is null
            if (model.SubForm4Records == null || model.SubForm4Records.Count == 0)
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                var waste = new SUB_Form4Record();
                model.SubForm4Records.Add(waste);
            }
            if (model.SubForm4RecordsOther == null || model.SubForm4RecordsOther.Count == 0)
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                var waste = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(waste);
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                ViewData["TypeCounters" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
                ViewData["DicEvents" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4Records[i].EventId);
                if (model.SubForm4Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
                if (model.SubForm4Records[i].EventId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }
            }

            for (int i = 0; i < model.SubForm4RecordsOther.Count; i++)
            {
                ViewData["TypeCountersOther" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[i].TypeCounterId);
                ViewData["DicEventsOther" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4RecordsOther[i].EventId);
            }

            #endregion

            #region form5
            model.SubForm5Records = new List<SUB_Form5Record>();
            model.SubForm5RecordsOther = new List<SUB_Form5Record>();
            var forms5 = _subForm.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
            {
                if (record.KindIndex == 2)
                {
                    model.SubForm5RecordsOther.Add(record);
                }
                else
                {
                    model.SubForm5Records.Add(record);
                }
            }

            if (model.SubForm5Records == null || model.SubForm5Records.Count == 0)
            {
                model.SubForm5Records = new List<SUB_Form5Record>();
                var item = new SUB_Form5Record();
                model.SubForm5Records.Add(item);
            }
            if (model.SubForm5RecordsOther == null || model.SubForm5RecordsOther.Count == 0)
            {
                model.SubForm5RecordsOther = new List<SUB_Form5Record>();
                var item = new SUB_Form5Record();
                model.SubForm5RecordsOther.Add(item);
            }

            #endregion

            #region form6
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = _subForm.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
            {
                model.SubForm6Records.Add(record);
            }

            #region
            if (model.SubForm6Records == null || model.SubForm6Records.Count == 0)
            {
                model.SubForm6Records = new List<SUB_Form6Record>();
                var waste = new SUB_Form6Record();
                model.SubForm6Records.Add(waste);
            }

            
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);

            for (int i = 0; i < model.SubForm6Records.Count; i++)
            {
                ViewData["TypesFrom6" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm6Records[i].TypeCounterId);
                if (model.SubForm6Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm6Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
            }
            #endregion
            #endregion

            #region form1 user info
            var bufferRstRR = new SubFormRepository().GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear.Value);
            if (bufferRstRR != null)
            {
                model.SEC_User1 = _subForm.SEC_User1;
                model.Wastes = _subForm.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();
                model.SEC_User1.Id = _subForm.SEC_User1.Id;
                model.SEC_User1.LastName = bufferRstRR.usrlastname;
                model.SEC_User1.SecondName = bufferRstRR.usrsecondname;
                model.SEC_User1.FirstName = bufferRstRR.usrfirstname;
                model.SEC_User1.JuridicalName = bufferRstRR.usrjuridicalname;
                model.SEC_User1.Post = bufferRstRR.usrpost;
                model.SEC_User1.Mobile = bufferRstRR.usrmobile;
                model.SEC_User1.WorkPhone = bufferRstRR.usrworkphone;
                model.SEC_User1.InternalPhone = bufferRstRR.usrinternalphone;
                model.SEC_User1.Address = bufferRstRR.usraddress;
                model.SEC_User1.BINIIN = bufferRstRR.BINIIN;
                model.SEC_User1.IsCvazy = (bufferRstRR.usriscvazy != null) ? Convert.ToBoolean(bufferRstRR.usriscvazy) : false;
                model.SEC_User1.ResponceFIO = bufferRstRR.usrresponcefio;
                model.SEC_User1.ResponcePost = bufferRstRR.usrresponcepost;
                model.SEC_User1.Oblast = bufferRstRR.usroblast;
                model.SEC_User1.Region = bufferRstRR.usrregion;
                model.SEC_User1.SubRegion = bufferRstRR.usrsubregion;
                model.SEC_User1.Village = bufferRstRR.usrvillage;
                model.SEC_User1.TypeApplicationId = Convert.ToInt64(bufferRstRR.usrtypeapplicationid);
                model.SEC_User1.OkedId = bufferRstRR.usrokedid;
                model.SEC_User1.FSCode = bufferRstRR.usrfscode;
                model.SEC_User1.IDK = bufferRstRR.usridk;
                model.SEC_User1.IsGuest = _subForm.SEC_User1.IsGuest;
                model.SEC_User1.Email = bufferRstRR.usremail;

                if (model.SEC_User1.OkedId != null)
                    model.SEC_User1.DIC_OKED = _dicOkedRepositoryRepo.GetAll().FirstOrDefault(x => x.Id == model.SEC_User1.OkedId.Value);
            }
            else model.SEC_User1 = _subForm.SEC_User1;

            model.WastList = new AccountController().GetPlants(model.Wastes);
            #endregion

            #region history
            model.SUB_FormHistory = _subForm.SUB_FormHistory.ToList();
            foreach (var rstReestrHistory in model.SUB_FormHistory)
            {
                var dir1 = Server.MapPath("~/uploads/subhsitory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }
            #endregion

            #region attached file
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }

            var dir = Server.MapPath("~/uploads/appform/" + model.Id + "/");
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
            #endregion

            #region dics            
            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();
            ViewData["DicEvents"] = new SelectList(dicevents, "Id", "NameRu", 0);
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);

            model.SubDicEnergyindicatorList = _subDicEnergyindicatorRepo.GetAll().Where(x=>x.forgu==true).ToList();
            ViewData["OKEDList"] = new SelectList(_dicOkedRepositoryRepo.GetAll(), "Id", "FullName", model.SEC_User1.OkedId);

            //string oblastName = "";
            //string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            //ViewData["Oblast"] = oblastName;

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(model.UserId.Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;

            #endregion

            model.SUB_FormComment = _subForm.SUB_FormComment.ToList();
            return View(model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult ShowDetailsGuNew(long id)
        {
            SUB_Form _subForm = _subRepo.GetById(id);

            var model = new SUB_FormGuNew
            {
                Id = _subForm.Id,
                StatusId = _subForm.StatusId,
                UserId = _subForm.UserId,
                ReportYear = new int?(_subForm.ReportYear),
                BeginPlanYear = _subForm.BeginPlanYear,
                EndPlanYear = _subForm.EndPlanYear,
                IsNotEvents = true,
                IsPlan = _subForm.IsPlan,
                IsEnergyManagementSystem = _subForm.IsEnergyManagementSystem,
                DesignDateStr = _subForm.DesignDateStr,
                DesignNote = _subForm.DesignNote,
                Note = _subForm.Note,
                IsRent = _subForm.IsRent
            };

            if (!model.Editor.HasValue)
            {
                model.Editor = MyExtensions.GetCurrentUserId();
            }

            model.SubDicTypeResources = _subDicTypeResRepo.GetAll();
            var _userId = model.UserId;
            var years = _subRepo.GetPastYears(_userId).Where(e => e.ReportYear != model.ReportYear.Value);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            var listanimal = IsAcceptReport() ? _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID) && (e.Id != CodeConstManager.STATUS_ACCEPT_ID));
            ViewData["statusList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.StatusId);

            model.SignedSubForm = null;
            if (Session["SignedValue"] is SUB_Form)
            {
                model.SignedSubForm = (SUB_Form)Session["SignedValue"];
            }

            ViewBag.SubReadonly = true;
            if (Request.UrlReferrer != null)
                model.PreviousUrl = Request.UrlReferrer.ToString();


            #region form2
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2")).OrderBy(e => e.PosIndex);
            var list = new List<SUB_Form2Record>();
            foreach (var rptDicKindWaste in dics)
            {
                var report = _subForm.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                if (report != null)
                {
                    list.Add(report);
                }
                else
                {
                    var kind = new SUB_Form2Record { TypeResourceId = rptDicKindWaste.Id, SUB_DIC_TypeResource = rptDicKindWaste };
                    list.Add(kind);
                }
            }
            model.SubForm2Records = list;
            #endregion

            #region dic
            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();        
            model.SubDicEnergyindicatorList = _subDicEnergyindicatorRepo.GetAll().Where(x => x.forgu == true).ToList();            
            
            var typeCounter = new SubDucTypeCounterRepository().GetAll();
            var dicevents = new SubDicEventRepository().GetAll(model.UserId);
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
            ViewData["DicEvents"] = new SelectList(dicevents, "Id", "NameRu", 0);

            var dicGuList = new DicGuRepository().GetAll();
            var dicGu1List = dicGuList.Where(x => x.Code.Equals("1")).ToList();
            var dicGu2List = dicGuList.Where(x => x.Code.Equals("2")).ToList();
            var dicGu3List = dicGuList.Where(x => x.Code.Equals("3")).ToList();

            var dicGu1ItemList = new List<ListItem>();
            foreach (var item in dicGu1List)
            {
                dicGu1ItemList.Add(new ListItem { Value = item.Id.ToString(), Text = item.NameRu });
            }

            var dicGu2ItemList = new List<ListItem>();
            foreach (var item in dicGu2List)
            {
                dicGu2ItemList.Add(new ListItem { Value = item.Id.ToString(), Text = item.NameRu });
            }

            var dicGu3ItemList = new List<ListItem>();
            foreach (var item in dicGu3List)
            {
                dicGu3ItemList.Add(new ListItem { Value = item.Id.ToString(), Text = item.NameRu });
            }

            ViewData["DicGu1List"] = dicGu1ItemList; //new SelectList(dicGu1List, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
            ViewData["DicGu2List"] = dicGu2ItemList;//new SelectList(dicGu2List, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
            ViewData["DicGu3List"] = dicGu3ItemList;

            var typeOfHeatingList = new List<ListItem>{
            new ListItem { Text = "Центральное отопление", Value = "1" },
            new ListItem { Text = "Автономное отопление", Value = "2" }
            };
            ViewData["TypeOfHeatingList"] = typeOfHeatingList; //new SelectList(typeOfHeatingList, "Value", "Text", 0);

            var automateItemList = new List<ListItem> { new ListItem { Text = "Да", Value = "1" }, new ListItem { Text = "Нет", Value = "2" } };
            ViewData["AutomateItemList"] = automateItemList; //new SelectList(automateItemList, "Value", "Text", 0);
            #endregion

            #region form3 gu
            //---- form3 gu
            var list3 = new List<SUB_Form3GuRecord>();
            var form3Gu = _subForm.SUB_Form3GuRecord.OrderBy(x => x.Id);
            foreach (var record in form3Gu)
            {
                list3.Add(record);
            }
            model.SUB_Form3GuRecords = list3;

            if (model.SUB_Form3GuRecords == null || model.SUB_Form3GuRecords.Count == 0)
            {
                model.SUB_Form3GuRecords = new List<SUB_Form3GuRecord>();
                var item = new SUB_Form3GuRecord() { AutomateItem = 1 };
                model.SUB_Form3GuRecords.Add(item);
            }

            //---- form4
            model.SubForm4Records = new List<SUB_Form4Record>();
            model.SubForm4RecordsOther = new List<SUB_Form4Record>();
            var forms4 = _subForm.SUB_Form4Record.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                if (record.KindIndex == 2)
                {
                    model.SubForm4RecordsOther.Add(record);
                }
                else
                {
                    model.SubForm4Records.Add(record);
                }
            }

            //---- if is null
            if (model.SubForm4Records == null || model.SubForm4Records.Count == 0)
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                var waste = new SUB_Form4Record();
                model.SubForm4Records.Add(waste);
            }
            if (model.SubForm4RecordsOther == null || model.SubForm4RecordsOther.Count == 0)
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                var waste = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(waste);
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                ViewData["TypeCounters" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
                ViewData["DicEvents" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4Records[i].EventId);
                if (model.SubForm4Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
                if (model.SubForm4Records[i].EventId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }
            }

            for (int i = 0; i < model.SubForm4RecordsOther.Count; i++)
            {
                ViewData["TypeCountersOther" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[i].TypeCounterId);
                ViewData["DicEventsOther" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4RecordsOther[i].EventId);
            }
            #endregion

            #region form3a gu

            //---- form5
            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();
            model.SubForm5Records = new List<SUB_Form5Record>();
            model.SubForm5RecordsOther = new List<SUB_Form5Record>();
            var forms5 = _subForm.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
            {
                if (record.KindIndex == 2)
                {
                    model.SubForm5RecordsOther.Add(record);
                }
                else
                {
                    model.SubForm5Records.Add(record);
                }
            }

            if (model.SubForm5Records == null || model.SubForm5Records.Count == 0)
            {
                model.SubForm5Records = new List<SUB_Form5Record>();
                var item = new SUB_Form5Record() { TypeOfHeating = 1 };
                model.SubForm5Records.Add(item);
            }
            if (model.SubForm5RecordsOther == null || model.SubForm5RecordsOther.Count == 0)
            {
                model.SubForm5RecordsOther = new List<SUB_Form5Record>();
                var item = new SUB_Form5Record() { TypeOfHeating = 1 };
                model.SubForm5RecordsOther.Add(item);
            }

            //---- form3a gu1
            model.SUB_Form3aGuRecord1s = new List<SUB_Form3aGuRecord1>();
            model.SUB_Form3aGuRecord1sOther = new List<SUB_Form3aGuRecord1>();
            var form3aGu1 = _subForm.SUB_Form3aGuRecord1.OrderBy(e => e.Id);
            foreach (var record in form3aGu1)
            {
                if (record.KindIndex == null)
                    continue;
                if (record.KindIndex == 1)
                    model.SUB_Form3aGuRecord1s.Add(record);
                else model.SUB_Form3aGuRecord1sOther.Add(record);
            }

            if (model.SUB_Form3aGuRecord1s == null || model.SUB_Form3aGuRecord1s.Count == 0)
            {
                model.SUB_Form3aGuRecord1s = new List<SUB_Form3aGuRecord1>();
                var item = new SUB_Form3aGuRecord1();
                model.SUB_Form3aGuRecord1s.Add(item);
            }

            if (model.SUB_Form3aGuRecord1sOther == null || model.SUB_Form3aGuRecord1sOther.Count == 0)
            {
                model.SUB_Form3aGuRecord1sOther = new List<SUB_Form3aGuRecord1>();
                var item = new SUB_Form3aGuRecord1();
                model.SUB_Form3aGuRecord1sOther.Add(item);
            }

            //---- form3a gu2
            model.SUB_Form3aGuRecord2s = new List<SUB_Form3aGuRecord2>();
            model.SUB_Form3aGuRecord2sOther = new List<SUB_Form3aGuRecord2>();
            var form3aGu2 = _subForm.SUB_Form3aGuRecord2.OrderBy(e => e.Id);
            foreach (var record in form3aGu2)
            {
                if (record.KindIndex == null)
                    continue;

                if (record.KindIndex == 1)
                {
                    model.SUB_Form3aGuRecord2s.Add(record);
                }
                else
                {
                    model.SUB_Form3aGuRecord2sOther.Add(record);
                }
            }

            if (model.SUB_Form3aGuRecord2s == null || model.SUB_Form3aGuRecord2s.Count == 0)
            {
                model.SUB_Form3aGuRecord2s = new List<SUB_Form3aGuRecord2>();
                var item = new SUB_Form3aGuRecord2();
                model.SUB_Form3aGuRecord2s.Add(item);
            }

            if (model.SUB_Form3aGuRecord2sOther == null || model.SUB_Form3aGuRecord2sOther.Count == 0)
            {
                model.SUB_Form3aGuRecord2sOther = new List<SUB_Form3aGuRecord2>();
                var item = new SUB_Form3aGuRecord2();
                model.SUB_Form3aGuRecord2sOther.Add(item);
            }

            //---- form3a gu3
            model.SUB_Form3aGuRecord3s = new List<SUB_Form3aGuRecord3>();
            model.SUB_Form3aGuRecord3sOther = new List<SUB_Form3aGuRecord3>();
            var form3aGu3 = _subForm.SUB_Form3aGuRecord3.OrderBy(e => e.Id);
            foreach (var record in form3aGu3)
            {
                if (record.KindIndex == null)
                    continue;

                if (record.KindIndex == 1)
                {
                    model.SUB_Form3aGuRecord3s.Add(record);
                }
                else
                {
                    model.SUB_Form3aGuRecord3sOther.Add(record);
                }
            }

            if (model.SUB_Form3aGuRecord3s == null || model.SUB_Form3aGuRecord3s.Count == 0)
            {
                model.SUB_Form3aGuRecord3s = new List<SUB_Form3aGuRecord3>();
                var item = new SUB_Form3aGuRecord3();
                model.SUB_Form3aGuRecord3s.Add(item);
            }

            if (model.SUB_Form3aGuRecord3sOther == null || model.SUB_Form3aGuRecord3sOther.Count == 0)
            {
                model.SUB_Form3aGuRecord3sOther = new List<SUB_Form3aGuRecord3>();
                var item = new SUB_Form3aGuRecord3();
                model.SUB_Form3aGuRecord3sOther.Add(item);
            }
            #endregion

            #region kadastr

            model.SubFormKadastrs = new List<SUB_FormKadastr>();
            var formsKadasre = _subForm.SUB_FormKadastr.OrderBy(e => e.Id);
            foreach (var record in formsKadasre)
            {
                model.SubFormKadastrs.Add(record);
            }

            if (model.SubFormKadastrs.Count == 0)
            {
                model.SubFormKadastrs.Add(new SUB_FormKadastr());
            }
            #endregion

            #region form1 user info
            var bufferRstRR = new SubFormRepository().GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear.Value);
            if (bufferRstRR != null)
            {
                model.SEC_User1 = _subForm.SEC_User1;
                model.Wastes = _subForm.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();
                model.SEC_User1.Id = _subForm.SEC_User1.Id;
                model.SEC_User1.LastName = bufferRstRR.usrlastname;
                model.SEC_User1.SecondName = bufferRstRR.usrsecondname;
                model.SEC_User1.FirstName = bufferRstRR.usrfirstname;
                model.SEC_User1.JuridicalName = bufferRstRR.usrjuridicalname;
                model.SEC_User1.Post = bufferRstRR.usrpost;
                model.SEC_User1.Mobile = bufferRstRR.usrmobile;
                model.SEC_User1.WorkPhone = bufferRstRR.usrworkphone;
                model.SEC_User1.InternalPhone = bufferRstRR.usrinternalphone;
                model.SEC_User1.Address = bufferRstRR.usraddress;
                model.SEC_User1.BINIIN = bufferRstRR.BINIIN;
                model.SEC_User1.IsCvazy = (bufferRstRR.usriscvazy != null) ? Convert.ToBoolean(bufferRstRR.usriscvazy) : false;
                model.SEC_User1.ResponceFIO = bufferRstRR.usrresponcefio;
                model.SEC_User1.ResponcePost = bufferRstRR.usrresponcepost;
                model.SEC_User1.Oblast = bufferRstRR.usroblast;
                model.SEC_User1.Region = bufferRstRR.usrregion;
                model.SEC_User1.SubRegion = bufferRstRR.usrsubregion;
                model.SEC_User1.Village = bufferRstRR.usrvillage;
                model.SEC_User1.TypeApplicationId = Convert.ToInt64(bufferRstRR.usrtypeapplicationid);
                model.SEC_User1.OkedId = bufferRstRR.usrokedid;
                model.SEC_User1.FSCode = bufferRstRR.usrfscode;
                model.SEC_User1.IDK = bufferRstRR.usridk;
                model.SEC_User1.IsGuest = _subForm.SEC_User1.IsGuest;
                model.SEC_User1.Email = bufferRstRR.usremail;

                if (model.SEC_User1.OkedId != null)
                    model.SEC_User1.DIC_OKED = _dicOkedRepositoryRepo.GetAll().FirstOrDefault(x => x.Id == model.SEC_User1.OkedId.Value);
            }
            else model.SEC_User1 = _subForm.SEC_User1;

            model.WastList = new AccountController().GetPlants(model.Wastes);
            #endregion

            #region history
            model.SUB_FormHistory = _subForm.SUB_FormHistory.ToList();
            foreach (var rstReestrHistory in model.SUB_FormHistory)
            {
                var dir1 = Server.MapPath("~/uploads/subhsitory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }
            #endregion

            #region attached file
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }

            var dir = Server.MapPath("~/uploads/appform/" + model.Id + "/");
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
            #endregion

            #region dics         
            if (model.SEC_User1.OkedId != null)
                ViewData["OKEDList"] = new SelectList(_dicOkedRepositoryRepo.GetAll(), "Id", "FullName", model.SEC_User1.OkedId);
            else ViewData["OKEDList"] = new SelectList(_dicOkedRepositoryRepo.GetAll(), "Id", "FullName", null);

            model.SubDicEnergyindicatorList = new SubDicEnergyindicator().GetAll().Where(x => x.forgu == true).ToList();
            var dicEnergyIndicatorItemList = new List<ListItem>();
            var dicEnergyIndicatorList = new List<Dictionary<string, object>>();
            foreach (var item in model.SubDicEnergyindicatorList)
            {
                dicEnergyIndicatorItemList.Add(new ListItem { Value = item.id.ToString(), Text = item.nameru });
                dicEnergyIndicatorList.Add(new Dictionary<string, object>());
                dicEnergyIndicatorList.Last()["id"] = item.id;
                dicEnergyIndicatorList.Last()["name"] = item.nameru;
                dicEnergyIndicatorList.Last()["unitname"] = item.unitnameru;
            }
            ViewData["DicEnergyIndicatorList"] = dicEnergyIndicatorItemList;
            ViewData["DicEnergyIndicatorListJson"] = Json(dicEnergyIndicatorList);

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(model.UserId.Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;
            #endregion

            model.SUB_FormComment = _subForm.SUB_FormComment.ToList();
            return View(model);
        }
        
        public ActionResult ShowDetailsF(long id, string formName)
		{
			if (Session["ShowDetails"] is SUB_Form)
			{
				var model = (SUB_Form)Session["ShowDetails"];
				if (formName.Equals("#form2tab"))
				{
                   
                    var dics = model.SubDicTypeResources.Where(e => e.Code != null && e.Code.Contains("2")).OrderBy(e => e.PosIndex).ToList();
                    var list = new List<SUB_Form2Record>();
					foreach (var rptDicKindWaste in dics)
					{
						var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
						if (report != null)
						{
							list.Add(report);
						}
						else
						{
							var kind = new SUB_Form2Record
							{
								TypeResourceId = rptDicKindWaste.Id,
								SUB_DIC_TypeResource = rptDicKindWaste
							};
							list.Add(kind);
						}
					}

					model.SubForm2Records = list;
					Session["ShowDetails"] = model;
					return PartialView("~/Views/RegisterForm/Form2View.cshtml", model);
				}

				if (formName.Equals("#form3tab"))
				{
					var kinds = _subDicKindResRepo.GetAll();
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
					Session["ShowDetails"] = model;
					return PartialView("~/Views/RegisterForm/Form3View.cshtml", model);
				}

				if (formName.Equals("#form4tab"))
				{
					model.SubForm4Records = new List<SUB_Form4Record>();
					model.SubForm4RecordsOther = new List<SUB_Form4Record>();
					var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
					foreach (var record in forms4)
					{
						if (record.EventId == null)
						{
							model.SubForm4RecordsOther.Add(record);
						}
						else
						{
							model.SubForm4Records.Add(record);
						}
					}

					if (model.SubForm4Records == null || model.SubForm4Records.Count == 0)
					{
						model.SubForm4Records = new List<SUB_Form4Record>();
						var waste = new SUB_Form4Record();
						model.SubForm4Records.Add(waste);
					}
					if (model.SubForm4RecordsOther == null || model.SubForm4RecordsOther.Count == 0)
					{
						model.SubForm4RecordsOther = new List<SUB_Form4Record>();
						var waste = new SUB_Form4Record();
						model.SubForm4RecordsOther.Add(waste);
					}

					#region

					var typeCounter = _subDicTypeCounterRepo.GetAll();
					var dicevents = _subDicEventRepositoryRepo.GetAll(model.UserId);
					ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
					ViewData["DicEvents"] = new SelectList(dicevents, "Id", "NameRu", 0);
				
					for (int i = 0; i < model.SubForm4Records.Count; i++)
					{
						ViewData["TypeCounters" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
						ViewData["DicEvents" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4Records[i].EventId);

						if (model.SubForm4Records[i].TypeCounterId == 0)
						{
							ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
						}
						if (model.SubForm4Records[i].EventId == 0)
						{
							ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
						}
					}

					for (int i = 0; i < model.SubForm4RecordsOther.Count; i++)
					{
						ViewData["TypeCountersOther" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[i].TypeCounterId);
						ViewData["DicEventsOther" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4RecordsOther[i].EventId);
					}

					

					#endregion


					Session["ShowDetails"] = model;
					return PartialView("~/Views/RegisterForm/Form4View.cshtml", model);
				}

				if (formName.Equals("#form5tab"))
				{
					model.SubForm5Records = new List<SUB_Form5Record>();
					model.SubForm5RecordsOther = new List<SUB_Form5Record>();
					var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
					foreach (var record in forms5)
					{
						if (record.energyindicator_id==0 || record.energyindicator_id == null)
						{
							model.SubForm5RecordsOther.Add(record);
						}
						else
						{
							model.SubForm5Records.Add(record);
						}
					}

					#region

					if (model.SubForm5Records == null || model.SubForm5Records.Count == 0)
					{
						model.SubForm5Records = new List<SUB_Form5Record>();
						var waste = new SUB_Form5Record();
						model.SubForm5Records.Add(waste);
					}
					if (model.SubForm5RecordsOther == null || model.SubForm5RecordsOther.Count == 0)
					{
						model.SubForm5RecordsOther = new List<SUB_Form5Record>();
						var waste = new SUB_Form5Record();
						model.SubForm5RecordsOther.Add(waste);
					}

					model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();
					if (model.IsPlan!=null && model.IsPlan==true)
					{
						var action = _subActionPlanRepo.GetListCurrentByUser(model.UserId).OrderBy(e => e.SendDate).LastOrDefault(e => e.StatusId == 2);
						if (action != null)
						{
							new AppActionController().FillViewBag(action);
							model.SubActionPlan = action;
						}
					}

					#endregion

					Session["ShowDetails"] = model;
					return PartialView("~/Views/RegisterForm/Form5View.cshtml", model);
				}

				if (formName.Equals("#form6tab"))
				{
					model.SubForm6Records = new List<SUB_Form6Record>();
					var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
					foreach (var record in forms6)
					{
						model.SubForm6Records.Add(record);
					}

					#region
					if (model.SubForm6Records == null || model.SubForm6Records.Count == 0)
					{
						model.SubForm6Records = new List<SUB_Form6Record>();
						var waste = new SUB_Form6Record();
						model.SubForm6Records.Add(waste);
					}

					var typeCounter = _subDicTypeCounterRepo.GetAll();
					ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);

					for (int i = 0; i < model.SubForm6Records.Count; i++)
					{
						ViewData["TypesFrom6" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm6Records[i].TypeCounterId);
						if (model.SubForm6Records[i].TypeCounterId == 0)
						{
							ModelState.AddModelError("model.SubForm6Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
						}
					}					
										
					#endregion

					Session["ShowDetails"] = model;
					return PartialView("~/Views/RegisterForm/Form6View.cshtml", model);
				}

				if (formName.Equals("#formHistorytab"))
				{
					FillHistory(model);
					Session["ShowDetails"] = model;
					return PartialView("~/Views/RegisterForm/FormHistoryView.cshtml", model);
				}
				return View(model);
			}
			else
			{
				return View();
			}

		}

		[HttpGet]
		public virtual ActionResult ShowDetailsCopy(long id)
		{
			var repository = new SubFormRepository();
			var model = repository.GetById(id);
			
			var dics = _subDicTypeResRepo.GetAll().OrderBy(e => e.Id);		

			var list = new List<SUB_Form2Record>();
			foreach (var rptDicKindWaste in dics)
			{
				var report = model.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
				if (report != null)
				{
					list.Add(report);
				}
				else
				{
					var kind = new SUB_Form2Record
					{
						TypeResourceId = rptDicKindWaste.Id,
						SUB_DIC_TypeResource = rptDicKindWaste
					};
					list.Add(kind);
				}
			}
			model.SubForm2Records = list;
			var kinds = _subDicKindResRepo.GetAll();
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

			model.SubForm4Records = new List<SUB_Form4Record>();
			model.SubForm4RecordsOther = new List<SUB_Form4Record>();
			var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
			foreach (var record in forms4)
			{
				if (record.EventId == null)
				{
					model.SubForm4RecordsOther.Add(record);
				}
				else
				{

				}				

			}

			model.SubForm5Records = new List<SUB_Form5Record>();
			model.SubForm5RecordsOther = new List<SUB_Form5Record>();
			var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
			foreach (var record in forms5)
			{
				if (record.KindIndex == 2)
				{
					model.SubForm5RecordsOther.Add(record);
				}
				else
				{
					model.SubForm5Records.Add(record);
				}
			}

			model.SubForm6Records = new List<SUB_Form6Record>();
			var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
			foreach (var record in forms6)
			{
				model.SubForm6Records.Add(record);
			}

			FillHistory(model);
			ViewBag.SubReadonly = true;
			FillViewBag(model);
            
            model.SubDicTypeResources = _subDicTypeResRepo.GetAll(); ;


            if (Request.UrlReferrer != null) model.PreviousUrl = Request.UrlReferrer.ToString();
			if (model.AttachFiles == null)
			{
				model.AttachFiles = new List<string>();
			}

			var dir = Server.MapPath("~/uploads/appform/" + model.Id + "/");
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

			model.SignedSubForm = null;
			if (Session["SignedValue"] is SUB_Form)
			{
				model.SignedSubForm = (SUB_Form)Session["SignedValue"];
			}

			return View(model);
		}

        protected void FillHistory(SUB_Form model)
        {
            foreach (var rstReestrHistory in model.SUB_FormHistory)
            {
                var dir1 = Server.MapPath("~/uploads/subhsitory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }
        }

        protected virtual void ViewSubFormMessage(SUB_Form model)
        {
            var histories = new SubFormRepository().GetListCurrentByUser(model.UserId).Where(e=>e.Id!=model.Id).OrderByDescending(e=>e.ReportYear).FirstOrDefault();
            if (histories == null)
            {
                return;
            }

            model.SubFornMessages = new List<SubFornMessage>();
            foreach (var tab2 in model.SUB_Form2Record)
            {
                var records = histories.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == tab2.TypeResourceId);
                if (records != null)
                {
                    CheckDiffDouble(model, tab2.ExtractVolume, records.ExtractVolume, 2,"4", tab2.SUB_DIC_TypeResource.PosIndex.ToString());
                    CheckDiffDouble(model, tab2.NotOwnSource, records.NotOwnSource, 2, "5a", tab2.SUB_DIC_TypeResource.PosIndex.ToString());
                    CheckDiffDouble(model, tab2.LosEnergy, records.LosEnergy, 2, "5б", tab2.SUB_DIC_TypeResource.PosIndex.ToString());
                    CheckDiffDouble(model, tab2.OwnSource, records.OwnSource, 2, "6", tab2.SUB_DIC_TypeResource.PosIndex.ToString());
                    CheckDiffDouble(model, tab2.TransferOtherLegal, records.OwnSource, 2, "7", tab2.SUB_DIC_TypeResource.PosIndex.ToString());
                    CheckDiffDouble(model, tab2.ExpenceEnergy, records.ExpenceEnergy, 2, "8", tab2.SUB_DIC_TypeResource.PosIndex.ToString());
                   
                }
            }
            var message = "";
            if (model.SEC_User1 != null)
            {
                if (model.SEC_User1.OkedId == null || model.SEC_User1.OkedId == 0)
                {
                    message = " строка: 1; колонка: 7 (Основные виды деятельности субъекта ГЭР*) не должно быть пустым";
                    model.SubFornMessages.Add(new SubFornMessage { FormIndex = 1, TypeMessage = 2, Message = message });

                }
            }
           

            foreach (var subForm2Record in model.SUB_Form2Record)
            {
                if (subForm2Record.OwnSource > 0 && string.IsNullOrEmpty(subForm2Record.Note))
                {
                    message = " строка:" + subForm2Record.SUB_DIC_TypeResource.PosIndex + "; колонка:" + 9 + "; не должно быть пустым";
                    model.SubFornMessages.Add(new SubFornMessage { FormIndex = 2, TypeMessage = 2, Message = message });

                }
            }
            var index6 = 1;
            foreach (var record in model.SUB_Form6Record)
            {
                if (record.TypeCounterId == null && record.CountDevice == null && record.Equipment == null)
                {
                
                }
                else
                {

                    if (record.TypeCounterId == null)
                    {
                        message = " строка:" + index6 + "; колонка:" + 2 + " 'Учитываемый энергоресурс (электричество, газ, тепло и др.)'; не должно быть пустым";
                        model.SubFornMessages.Add(new SubFornMessage { FormIndex = 6, TypeMessage = 2, Message = message });
                    }
                    if (record.CountDevice == null)
                    {
                        message = " строка:" + index6 + "; колонка:" + 3 + " 'Количество приборов, шт.'; не должно быть пустым";
                        model.SubFornMessages.Add(new SubFornMessage { FormIndex = 6, TypeMessage = 2, Message = message });
                    }
                    if (record.Equipment == null)
                    {
                        message = " строка:" + index6 + "; колонка:" + 4 + " 'общий % оснащенности приборами учета'; не должно быть пустым";
                        model.SubFornMessages.Add(new SubFornMessage { FormIndex = 6, TypeMessage = 2, Message = message });
                    }
                  

                }
                index6++;
            }   
        }

        private void CheckDiffDouble(SUB_Form model, double? newVal, double? historyVal, int tabIndex, string colIndex, string rowIndex)
        {
            var message = "Предупреждение: строка:" + rowIndex + "; колонка:" + colIndex + ";";
            if (newVal == null || historyVal == null)
            {
                return;
            }
            double? koef;
           
            if (newVal > historyVal)
            {
                koef = 100*(historyVal/newVal);
                if (koef < 90)
                {
//                    koef = Math.Round(koef,2);
                    message = message + " значение больше предыдущего на " +Math.Round((double) (100 - koef),1) + "%";
                    model.SubFornMessages.Add(new SubFornMessage { FormIndex = tabIndex, TypeMessage = 1, Message = message });
                    return;
                }
            }
            koef = 100 * (newVal / historyVal);
            if (koef < 90)
            {
                message = message + " значение меньше предыдущего на " + +Math.Round((double)(100 - koef), 1) + "%";
                model.SubFornMessages.Add(new SubFornMessage { FormIndex = tabIndex, TypeMessage = 1, Message = message });
            }
        }
        
        protected virtual void FillViewBag(SUB_Form model)
        {
			var listanimal = IsAcceptReport() ? _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : _statusDicRepo.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));   //(e.Id != CodeConstManager.STATUS_ACCEPT_ID)
            ViewData["statusList"] = new SelectList(listanimal, "Id",
                                                 CultureHelper.GetDictionaryName("NameRu"), model.StatusId);

            var years = _subRepo.GetPastYears(model.UserId).Where(e => e.ReportYear != model.ReportYear);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            var dics =_subDicTypeResRepo.GetAll();
            var wrapResource = new List<SUB_DIC_TypeResource>();
            foreach (var subDicTypeResource in dics)
            {
                if (subDicTypeResource.DIC_Unit != null)
                {
                    subDicTypeResource.NameRu += " (" + subDicTypeResource.DIC_Unit.Name + ")";
                }
                wrapResource.Add(subDicTypeResource);
            }
//            model.SubDicTypeResources = dics;
            ViewData["Types"] = new SelectList(wrapResource, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
            if (model.SubForm4Records == null || model.SubForm4Records.Count == 0)
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                var waste = new SUB_Form4Record();
                model.SubForm4Records.Add(waste);
            }
            if (model.SubForm4RecordsOther == null || model.SubForm4RecordsOther.Count == 0)
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                var waste = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(waste);
            }
            if (model.SubForm5Records == null || model.SubForm5Records.Count == 0)
            {
                model.SubForm5Records = new List<SUB_Form5Record>();
                var waste = new SUB_Form5Record();
                model.SubForm5Records.Add(waste);
            }
            if (model.SubForm5RecordsOther == null || model.SubForm5RecordsOther.Count == 0)
            {
                model.SubForm5RecordsOther = new List<SUB_Form5Record>();
                var waste = new SUB_Form5Record();
                model.SubForm5RecordsOther.Add(waste);
            }
            if (model.SubForm6Records == null || model.SubForm6Records.Count == 0)
            {
                model.SubForm6Records = new List<SUB_Form6Record>();
                var waste = new SUB_Form6Record();
                model.SubForm6Records.Add(waste);
            }
        
            var typeCounter = _subDicTypeCounterRepo.GetAll();
            var dicevents = _subDicEventRepositoryRepo.GetAll(model.UserId);
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id",  CultureHelper.GetDictionaryName("NameRu"), 0);
            ViewData["DicEvents"] = new SelectList(dicevents, "Id", "NameRu", 0);
            for (int i = 0; i < model.SubForm6Records.Count; i++)
            {
                ViewData["TypesFrom6" + i] = new SelectList(typeCounter, "Id",  CultureHelper.GetDictionaryName("NameRu"), model.SubForm6Records[i].TypeCounterId);
                if (model.SubForm6Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm6Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
            }
            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                ViewData["TypeCounters" + i] = new SelectList(typeCounter, "Id",  CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
                ViewData["DicEvents" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4Records[i].EventId);
                if (model.SubForm4Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
                if (model.SubForm4Records[i].EventId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }
            }
            for (int i = 0; i < model.SubForm4RecordsOther.Count; i++)
            {
                ViewData["TypeCountersOther" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[i].TypeCounterId);
                ViewData["DicEventsOther" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4RecordsOther[i].EventId);
               /* if (model.SubForm4Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
                if (model.SubForm4Records[i].EventId == 0)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }*/
            }
            model.SubDicKindTabOnes = _subDicKindTabOneRepositoryRepo.GetAll();
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
                        model.SubFormTab1s.Add(new SUB_FormTab1() { Code = codeIndex,KindId = tabOne.Id});
                    }
                }
                var notIn = list.Where(e => !codes.Contains(e.Code));
                foreach (var subFormTab1 in notIn)
                {
                    model.SubFormTab1s.Add(subFormTab1);
                }
            }
            var dicsTwo =_subDicKindTwoRepo.GetAll();
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
            model.SubFormTab3s = new List<SUB_FormTab3>();
            var forms4 = model.SUB_FormTab3.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubFormTab3s.Add(record);
            }
            if (model.SubFormTab3s.Count == 0)
            {
                model.SubFormTab3s.Add(new SUB_FormTab3());
            }
            model.SubFormKadastrs = new List<SUB_FormKadastr>();
            var formsKadasre = model.SUB_FormKadastr.OrderBy(e => e.Id);
            foreach (var record in formsKadasre)
            {
                model.SubFormKadastrs.Add(record);
            }
            if (model.SubFormKadastrs.Count == 0)
            {
                model.SubFormKadastrs.Add(new SUB_FormKadastr());
            }

            model.WastList = GetPlants(model.Wastes);
            if (model.SubDicTypeResources == null)
            {
               model.SubDicTypeResources = new List<SUB_DIC_TypeResource>(); 
            }
            long? okedId = 0;
            if (model.SEC_User1 != null)
            {
                okedId = model.SEC_User1.OkedId;
            }

            ViewData["OKEDList"] = new SelectList(_dicOkedRepositoryRepo.GetAll(), "Id", "FullName", okedId);

            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();
            if (model.IsPlan!=null && model.IsPlan==true)
            {
                var action = _subActionPlanRepo.GetListCurrentByUser(model.UserId).OrderBy(e=>e.SendDate).LastOrDefault(e=>e.StatusId==2);
                if (action != null)
                {
                    new AppActionController().FillViewBag(action);
                    model.SubActionPlan = action;
                }
            }

            //----
            model.SubDicEnergyindicatorList = _subDicEnergyindicatorRepo.GetAll().ToList();

            //----
            string oblastName="";
			string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value,model.ReportYear, ref oblastName, CultureHelper.GetCurrentCulture());
			 ViewData["Oblast"]=oblastName;
			var rst_id = 1;
        }

        public MultiSelectList GetPlants(IList<string> selectedValues)
        {
            var plants = _dicOkedRepositoryRepo.GetList();
            return new MultiSelectList(plants, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetHistoryValues(long? userId, int year)
        {
            var history = new SubFormRepository().GetListCurrentByUser(userId).FirstOrDefault(e => e.ReportYear==year);
            if (history == null)
            {
                return null;
            }
            var form2 = new List<SelectListItem>();
            foreach (var subForm2Record in history.SUB_Form2Record)
            {
                GetSelectItem("ExpenceEnergy", subForm2Record.ExpenceEnergy, subForm2Record.TypeResourceId, form2);
                GetSelectItem("ExtractVolume", subForm2Record.ExtractVolume, subForm2Record.TypeResourceId, form2);
                GetSelectItem("LosEnergy", subForm2Record.LosEnergy, subForm2Record.TypeResourceId, form2);
                GetSelectItem("NotOwnSource", subForm2Record.NotOwnSource, subForm2Record.TypeResourceId, form2);
                GetSelectItem("Note", subForm2Record.Note, subForm2Record.TypeResourceId, form2);
                GetSelectItem("OwnSource", subForm2Record.OwnSource, subForm2Record.TypeResourceId, form2);
                GetSelectItem("TransferOtherLegal", subForm2Record.TransferOtherLegal, subForm2Record.TypeResourceId, form2);
            }

            var form3 = new List<SelectListItem>();
            foreach (var item in history.SUB_Form3Record)
            {
                GetSelectItem("ConsumptionPrice", item.ConsumptionPrice, item.KindResourceId, form3);
                GetSelectItem("ConsumptionVolume", item.ConsumptionVolume, item.KindResourceId, form3);
                GetSelectItem("LosTransportPrice", item.LosTransportPrice, item.KindResourceId, form3);
                GetSelectItem("LosTransportVolume", item.LosTransportVolume, item.KindResourceId, form3);
            }

            var form4 = new List<SUB_Form4Record>();
            foreach (var record in history.SUB_Form4Record)
            {
                var item = new SUB_Form4Record();
                item.ActualInvest = record.ActualInvest;
                item.EmplPeriodStr = record.EmplPeriodStr;
                item.InKind = record.InKind;
                item.InMoney = record.InMoney;
                if (record.SUB_DIC_TypeCounter != null)
                {
                    item.Note = record.SUB_DIC_TypeCounter.NameRu;
                    if (record.SUB_DIC_TypeCounter.Id == 5)
                    {
                        item.Note = item.Note + " (" + record.Note + ")";
                    }
                }
                item.PlanExpend = record.PlanExpend;
                if (record.KindIndex == 2)
                {
                    item.EventName = record.EventName;
                }
                else if (record.SUB_DIC_Event !=null)
                {
                    item.EventName = record.SUB_DIC_Event.NameRu;
                }
                form4.Add(item);
            }
            var form5 = new List<SUB_Form5Record>();
            foreach (var record in history.SUB_Form5Record)
            {
                var item = new SUB_Form5Record();
                item.CalcFormula = record.CalcFormula;
                item.EnergyValue = record.EnergyValue;
                item.RegularStandart = record.RegularStandart;
                item.UnitMeasure = record.UnitMeasure;
                item.IndicatorName = record.UnitMeasure;
               
                if (record.KindIndex == 2)
                {
                    item.IndicatorName = record.IndicatorName;
                }
                else if (record.SUB_DIC_NormEnergy != null)
                {
                    item.IndicatorName = record.SUB_DIC_NormEnergy.NameRu;
                    item.Note = record.SUB_DIC_NormEnergy.Rate;
                }
                form5.Add(item);
            }
            var form6 = new List<SUB_Form6Record>();
            foreach (var record in history.SUB_Form6Record)
            {
                var item = new SUB_Form6Record();
                item.CountDevice = record.CountDevice;
                item.Equipment = record.Equipment;
                if (record.SUB_DIC_TypeCounter != null)
                {
                    item.Note = record.SUB_DIC_TypeCounter.NameRu;
                }
                form6.Add(item);
            }
            return Json(new { form2, form3, form4, form5, form6 }, JsonRequestBehavior.AllowGet);
        }

        private void GetSelectItem(string fieldName, object value, long? rowIndex, ICollection<SelectListItem> list)
        {
            if (rowIndex == null)
            {
                return;
            }
            if (value == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return;
            }
            var item = new SelectListItem();
            item.Value = fieldName + "_History_" + rowIndex;
            item.Text = value.ToString();
            list.Add(item);

        }

        public bool IsAcceptReport()
        {
            return new AccountRepository().IsAcceptReport(MyExtensions.GetCurrentUserId());
        }
    }
}