using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;
using Aisger.Models.Entity.Subject;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Aisger.Controllers.Subject
{
    public class RegisterFormController : ASubjectController
    {
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var list = new SubFormRepository().GetListCurrentByUser(MyExtensions.GetCurrentUserId());
            ViewBag.IsValidInfo = new SubFormRepository().GetIsValidInfo(MyExtensions.GetCurrentUserId());
            ViewBag.IsCommentNotFill = new SubFormRepository().GetIsCommentNotFill(MyExtensions.GetCurrentUserId());
            if (MyExtensions.GetCurrentUserFSCode() == 3)
            {
                ViewBag.IsForm4NotFill = new SubFormRepository().GetIsForm4NotFillGu(MyExtensions.GetCurrentUserId());
            }
            else
            {
                ViewBag.IsForm4NotFill = new SubFormRepository().GetIsForm4NotFill(MyExtensions.GetCurrentUserId());
            }

            //----get current year
            ViewBag.ActiveYear = new SubFormRepository().GetActiveReportYear();
            var rst_report = new SubFormRepository().GetRST_Report();

            var loginWithoutECP = (HttpContext.Session[CodeConstManager.LOGINWITHOUTECP] != null) ? (bool)HttpContext.Session[CodeConstManager.LOGINWITHOUTECP] : false;

            if (loginWithoutECP == true && rst_report.IsEditSubjectReportByManager == true)
            {
                ViewBag.IsEdit = true;
                ViewBag.IsCreate = true;
            }
            else
            {
                ViewBag.IsEdit = rst_report.IsEditSubjectReport;
                ViewBag.IsCreate = rst_report.IsCreateSubjectReport;
            }

            return View(list);
        }

        [GerNavigateLogger]
        public ActionResult Create()
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();

            var repo = new SubFormRepository();
            if (repo.CheckIsExistSubForm(currentUserId))
            {
                return Redirect("Index");
            }

            int ReportYear = repo.CreateReportYearActual(currentUserId);
            if (ReportYear == 0)
            {
                return Redirect("Index");
            }
            
            if (repo.GetFSCode(MyExtensions.GetCurrentUserId().Value, ReportYear) == 3)
            {
                if (ReportYear >= 2018 && ReportYear < 2019)
                    return Redirect("CreateGu");
                else return Redirect("CreateGuNew");
            }

            var model = new SUB_Form
            {
                StatusId = 1,
                UserId = currentUserId,
                ReportYear = ReportYear,
                Editor = currentUserId,
                SEC_User1 = new SecUserRepository().GetById(currentUserId.Value),
                BeginPlanYear = DateTime.Now.Year,
                EndPlanYear = DateTime.Now.Year + 4,
                IsNotEvents = true
            };
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2")).OrderBy(e => e.PosIndex);

            var list = new List<SUB_Form2Record>();
            foreach (var rptDicKindWaste in dics)
            {
                var kind = new SUB_Form2Record { TypeResourceId = rptDicKindWaste.Id, SUB_DIC_TypeResource = rptDicKindWaste };
                list.Add(kind);
            }
            model.SubForm2Records = list;

            var kinds = new SubDicKindResourceRepository().GetAll();
            var form3 = new List<SUB_Form3Record>();
            foreach (var rptDicKindWaste in kinds)
            {
                var kind = new SUB_Form3Record { KindResourceId = rptDicKindWaste.Id, SUB_DIC_KindResource = rptDicKindWaste };
                form3.Add(kind);
            }
            model.SubForm3Records = form3;
            model.SubForm5Records = new List<SUB_Form5Record> { new SUB_Form5Record() };

            //----капироваться реквизиты
            repo.CopySecUserToRstReportReestr();

            //----
            var bufferRstRR = repo.GetRstReportReestrByUserId(currentUserId.Value, ReportYear);
            if (bufferRstRR != null)
            {
                model.Wastes = repo.GetRstReportReestrOked(model.UserId.Value, model.ReportYear);

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

            }

            FillViewBag(model);
            return View(model);
        }

        [GerNavigateLogger]
        public ActionResult CreateGu()
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();

            if (new SubFormRepository().CheckIsExistSubForm(currentUserId))
            {
                return Redirect("Index");
            }

            int ReportYear = new SubFormRepository().CreateReportYearActual(currentUserId);
            if (ReportYear == 0)
            {
                return Redirect("Index");
            }

            var model = new Sub_FormGu
            {
                StatusId = 1,
                UserId = currentUserId,
                ReportYear = ReportYear,
                Editor = currentUserId,
                SEC_User1 = new SecUserRepository().GetById(currentUserId.Value),
                BeginPlanYear = DateTime.Now.Year,
                EndPlanYear = DateTime.Now.Year + 4,
                IsNotEvents = true,
                IsPlan = false,
                IsEnergyManagementSystem = false,
            };


            #region form2 
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);

            model.SUB_Form2RecordGu = new List<SUB_Form2RecordGu>();
            foreach (var rptDicKindWaste in dics)
            {
                var item = new SUB_Form2RecordGu { FormId = 0, TypeResourceId = rptDicKindWaste.Id, TypeResourceName = rptDicKindWaste.Name, TypeResourceUnitName = rptDicKindWaste.DIC_Unit.Name, SUB_DIC_TypeResource = rptDicKindWaste };
                model.SUB_Form2RecordGu.Add(item);
            }

            //----resource 2
            var kinds = new SubDicKindResourceRepository().GetAll();
            model.SUB_Form3RecordGu = new List<SUB_Form3RecordGu>();
            foreach (var rptDicKindWaste in kinds)
            {
                var item = new SUB_Form3RecordGu { FormId = 0, KindResourceId = rptDicKindWaste.Id, KindResourceName = rptDicKindWaste.Name, KindResourceUnitName = rptDicKindWaste.DIC_Unit.Name };
                model.SUB_Form3RecordGu.Add(item);
            }

            //----new gu tables
            model.SUB_Form2Gu = new SUB_Form2Gu();
            model.SUB_Form3Gu = new SUB_Form3Gu();

            model.SUB_FormGuLightingInfo = new List<SUB_FormGuLightingInfo>();

            if (model.SUB_FormGuLightingInfo.Count == 0)
            {
                var item = new SUB_FormGuLightingInfo();
                model.SUB_FormGuLightingInfo.Add(item);
            }

            //----       
            List<SelectListItem> newList = new List<SelectListItem>();
            SelectListItem _item1 = new SelectListItem() { Value = "0", Text = "Нет" };
            newList.Add(_item1);
            SelectListItem _item2 = new SelectListItem() { Value = "1", Text = "Да" };
            newList.Add(_item2);
            ViewData["Forma3GuAutomateItems"] = new SelectList(newList, "Value", "Text", "0");
            #endregion


            //----form3-------------------------------------------------------------------------
            var typeCounter = new SubDucTypeCounterRepository().GetAll();
            var dicevents = new SubDicEventRepository().GetAll(model.UserId);

            #region form4 gu
            model.SubForm4Records = new List<SUB_Form4Record>();
            model.SubForm4RecordsOther = new List<SUB_Form4Record>();

            //---- if is null
            if (model.SubForm4Records == null || model.SubForm4Records.Count == 0)
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                var item = new SUB_Form4Record();
                model.SubForm4Records.Add(item);
            }
            if (model.SubForm4RecordsOther == null || model.SubForm4RecordsOther.Count == 0)
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                var item = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(item);
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

            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();

            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
            ViewData["DicEvents"] = new SelectList(dicevents, "Id", "NameRu", 0);

            #endregion

            #region
            if (model.SubForm6Records == null || model.SubForm6Records.Count == 0)
            {
                model.SubForm6Records = new List<SUB_Form6Record>();
                var waste = new SUB_Form6Record();
                model.SubForm6Records.Add(waste);
            }

            for (int i = 0; i < model.SubForm6Records.Count; i++)
            {
                ViewData["TypesFrom6" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm6Records[i].TypeCounterId);
                if (model.SubForm6Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm6Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
            }
            #endregion

            //----капироваться реквизиты
            new SubFormRepository().CopySecUserToRstReportReestr();

            #region ----form1 user info
            var bufferRstRR = new SubFormRepository().GetRstReportReestrByUserId(currentUserId.Value, ReportYear);
            if (bufferRstRR != null)
            {
                model.Wastes = new SubFormRepository().GetRstReportReestrOked(model.UserId.Value, ReportYear);

                //----edit sec_user1 takes data from rst_report_reestr

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
                model.SEC_User1.IsGuest = bufferRstRR.SEC_User.IsGuest;

            }
            model.WastList = new AccountController().GetPlants(model.Wastes);
            #endregion

            #region dics
            model.SubDicEnergyindicatorList = new SubDicEnergyindicator().GetAll().Where(x => x.forgu == true).ToList();
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);


            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, ReportYear, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;


            #endregion
            //FillViewBag(model);
            return View(model);
        }

        [GerNavigateLogger]
        public ActionResult CreateGuNew()
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();

            if (new SubFormRepository().CheckIsExistSubForm(currentUserId))
            {
                return Redirect("Index");
            }

            int ReportYear = new SubFormRepository().CreateReportYearActual(currentUserId);
            if (ReportYear == 0)
            {
                return Redirect("Index");
            }

            var model = new SUB_FormGuNew
            {
                StatusId = 1,
                UserId = currentUserId,
                ReportYear = ReportYear,
                Editor = currentUserId,
                SEC_User1 = new SecUserRepository().GetById(currentUserId.Value),
                BeginPlanYear = DateTime.Now.Year,
                EndPlanYear = DateTime.Now.Year + 4,
                IsNotEvents = true,
                IsPlan = false,
                IsEnergyManagementSystem = false,
            };


            #region form2 
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2")).OrderBy(e => e.PosIndex);

            var list = new List<SUB_Form2Record>();
            foreach (var rptDicKindWaste in dics)
            {
                var kind = new SUB_Form2Record { TypeResourceId = rptDicKindWaste.Id, SUB_DIC_TypeResource = rptDicKindWaste };
                list.Add(kind);
            }
            model.SubForm2Records = list;
            #endregion

            #region dics
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
            if (model.SUB_Form3GuRecords == null || model.SUB_Form3GuRecords.Count == 0)
            {
                model.SUB_Form3GuRecords = new List<SUB_Form3GuRecord>();
                var item = new SUB_Form3GuRecord();
                model.SUB_Form3GuRecords.Add(item);
            }

            //----form4      
            model.SubForm4Records = new List<SUB_Form4Record>();
            model.SubForm4RecordsOther = new List<SUB_Form4Record>();

            //---- if is null
            if (model.SubForm4Records == null || model.SubForm4Records.Count == 0)
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                var item = new SUB_Form4Record();
                model.SubForm4Records.Add(item);
            }
            if (model.SubForm4RecordsOther == null || model.SubForm4RecordsOther.Count == 0)
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                var item = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(item);
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

            //---- form3a gu1              
            if (model.SUB_Form3aGuRecord1s == null || model.SUB_Form3aGuRecord1s.Count == 0)
            {
                model.SUB_Form3aGuRecord1s = new List<SUB_Form3aGuRecord1>();
                var item = new SUB_Form3aGuRecord1();
                model.SUB_Form3aGuRecord1s.Add(item);
            }

            if(model.SUB_Form3aGuRecord1sOther==null || model.SUB_Form3aGuRecord1sOther.Count == 0)
            {
                model.SUB_Form3aGuRecord1sOther = new List<SUB_Form3aGuRecord1>();
                var item = new SUB_Form3aGuRecord1();
                model.SUB_Form3aGuRecord1sOther.Add(item);
            }

            //---- form3a gu2
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
            if(model.SUB_Form3aGuRecord3s==null || model.SUB_Form3aGuRecord3s.Count == 0)
            {
                model.SUB_Form3aGuRecord3s = new List<SUB_Form3aGuRecord3>();
                var item = new SUB_Form3aGuRecord3();
                model.SUB_Form3aGuRecord3s.Add(item);
            }

            if(model.SUB_Form3aGuRecord3sOther==null || model.SUB_Form3aGuRecord3sOther.Count == 0)
            {
                model.SUB_Form3aGuRecord3sOther = new List<SUB_Form3aGuRecord3>();
                var item = new SUB_Form3aGuRecord3();
                model.SUB_Form3aGuRecord3sOther.Add(item);
            }
            #endregion

            #region kadastr
            model.SubFormKadastrs = new List<SUB_FormKadastr>();
            if (model.SubFormKadastrs.Count == 0)
            {
                model.SubFormKadastrs.Add(new SUB_FormKadastr());
            }
            #endregion

            var kinds = new SubDicKindResourceRepository().GetAll();
            //----       
            List<SelectListItem> newList = new List<SelectListItem>();
            SelectListItem _item1 = new SelectListItem() { Value = "0", Text = "Нет" };
            newList.Add(_item1);
            SelectListItem _item2 = new SelectListItem() { Value = "1", Text = "Да" };
            newList.Add(_item2);
            ViewData["Forma3GuAutomateItems"] = new SelectList(newList, "Value", "Text", "0");

            //----капироваться реквизиты
            new SubFormRepository().CopySecUserToRstReportReestr();

            #region ----form1 user info
            var bufferRstRR = new SubFormRepository().GetRstReportReestrByUserId(currentUserId.Value, ReportYear);
            if (bufferRstRR != null)
            {
                model.Wastes = new SubFormRepository().GetRstReportReestrOked(model.UserId.Value, ReportYear);

                //----edit sec_user1 takes data from rst_report_reestr

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
                model.SEC_User1.IsGuest = bufferRstRR.SEC_User.IsGuest;

            }
            model.WastList = new AccountController().GetPlants(model.Wastes);
            #endregion

            #region dics
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

            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, ReportYear, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;

            #endregion
            //FillViewBag(model);
            return View(model);
        }

        [AllowAnonymous]
        [Authorize]
        public ActionResult CheckReportIsExist()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string errorMessage = "";
            if (new SubFormRepository().CheckIsExistSubForm(MyExtensions.GetCurrentUserId()))
            {
                errorMessage = ResourceSetting.ReportIsExist;
            }

            int year = new SubFormRepository().CreateReportYearActual(MyExtensions.GetCurrentUserId());
            if (year == 0)
            {
                errorMessage = ResourceSetting.InNotReestr;
            }

            dict["ErrorMessage"] = errorMessage;
            return Json(dict);
        }

        [HttpGet]
        public ActionResult Send(long id)
        {
            var repository = new SubFormRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.StatusId = CodeConstManager.STATUS_SEND_ID;
            model.SendDate = DateTime.Now;
            var history = new SUB_FormHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                FormId = model.Id,
                StatusId = CodeConstManager.STATUS_SEND_ID,
                UserId = MyExtensions.GetCurrentUserId()
            };
            model.SUB_FormHistory.Add(history);
            new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SubFormRepository().UpdateReestr(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id, string url)
        {
            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            //----проверка гу           
            if (repository.GetFSCode(model.UserId.Value, model.ReportYear) == 3)
            {
                if (model.ReportYear >= 2018 && model.ReportYear < 2019)
                    return RedirectToAction("EditGu", new { id = id, url = url });
                else return RedirectToAction("EditGuNew", new { id = id, url = url });
            }

            model.PreviousUrl = url;
            model.AttachFiles = new List<string>();

            //----
            if (model.Editor == null)
                model.Editor = MyExtensions.GetCurrentUserId();

            //----
            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll();

            //----
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2")).OrderBy(e => e.PosIndex);
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
                    var kind = new SUB_Form2Record { TypeResourceId = rptDicKindWaste.Id, SUB_DIC_TypeResource = rptDicKindWaste };
                    list.Add(kind);
                }
            }
            model.SubForm2Records = list;
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
                    var kind = new SUB_Form3Record { KindResourceId = rptDicKindWaste.Id, SUB_DIC_KindResource = rptDicKindWaste };
                    form3.Add(kind);
                }

            }
            model.SubForm3Records = form3;


            model.SubForm4Records = new List<SUB_Form4Record>();
            model.SubForm4RecordsOther = new List<SUB_Form4Record>();
            var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
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


            //var rst_reportreestr = model.RST_ReportReestr.FirstOrDefault();
            var bufferRstRR = repository.GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear);
            if (bufferRstRR != null)
            {
                model.Wastes = repository.GetRstReportReestrOked(model.UserId.Value, model.ReportYear);

                //model.Wastes = model.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();			
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
            }

            FillViewBag(model);
            ViewSubFormMessage(model);
            return View("Create", model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult EditGu(long id, string url)
        {
            //----
            var repository = new SubFormRepository();
            var _subForm = repository.GetById(id);

            var model = new Sub_FormGu
            {
                Id = _subForm.Id,
                StatusId = _subForm.StatusId,
                UserId = _subForm.UserId,
                ReportYear = _subForm.ReportYear,
                BeginPlanYear = _subForm.BeginPlanYear,
                EndPlanYear = _subForm.EndPlanYear,
                IsNotEvents = true,
                IsPlan = _subForm.IsPlan,
                IsEnergyManagementSystem = _subForm.IsEnergyManagementSystem,
                DesignDate = _subForm.DesignDate,
                DesignNote = _subForm.DesignNote,
                DesignDateStr = _subForm.DesignDateStr,
                Note = _subForm.Note,
                IsRent = _subForm.IsRent
            };

            //----
            if (model.Editor == null)
                model.Editor = MyExtensions.GetCurrentUserId();

            #region ---- form2

            //----dic resource 1
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);
            model.SUB_Form2RecordGu = new List<SUB_Form2RecordGu>();
            foreach (var rptDicKindWaste in dics)
            {
                var item = new SUB_Form2RecordGu();
                item.TypeResourceId = rptDicKindWaste.Id;
                item.TypeResourceName = rptDicKindWaste.Name;
                item.TypeResourceUnitName = rptDicKindWaste.DIC_Unit.Name;
                item.SUB_DIC_TypeResource = rptDicKindWaste;

                var form2 = _subForm.SUB_Form2Record.FirstOrDefault(e => e.TypeResourceId == rptDicKindWaste.Id);
                if (form2 != null)
                {
                    item.Form2RecordId = form2.Id;
                    item.ExpenceEnergy = form2.ExpenceEnergy;
                    item.NotOwnSource = form2.NotOwnSource;
                }

                model.SUB_Form2RecordGu.Add(item);
            }

            //----dic resource 2
            var kinds = new SubDicKindResourceRepository().GetAll();
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
            var _automateItem = (model.SUB_Form3Gu.AutomateItem == null) ? 0 : model.SUB_Form3Gu.AutomateItem;
            ViewData["Forma3GuAutomateItems"] = new SelectList(newList, "Value", "Text", _automateItem);
            #endregion

            var typeCounter = new SubDucTypeCounterRepository().GetAll();
            var dicevents = new SubDicEventRepository().GetAll(model.UserId);

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

            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().Where(e => e.refParent == null).ToList();


            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);
            ViewData["DicEvents"] = new SelectList(dicevents, "Id", "NameRu", 0);

            #endregion

            #region form6
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = _subForm.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
            {
                model.SubForm6Records.Add(record);
            }
            if (model.SubForm6Records == null || model.SubForm6Records.Count == 0)
            {
                model.SubForm6Records = new List<SUB_Form6Record>();
                var waste = new SUB_Form6Record();
                model.SubForm6Records.Add(waste);
            }

            for (int i = 0; i < model.SubForm6Records.Count; i++)
            {
                ViewData["TypesFrom6" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm6Records[i].TypeCounterId);
                if (model.SubForm6Records[i].TypeCounterId == 0)
                {
                    ModelState.AddModelError("model.SubForm6Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }
            }
            #endregion

            #region user info
            var bufferRstRR = repository.GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear.Value);
            if (bufferRstRR != null)
            {
                model.Wastes = repository.GetRstReportReestrOked(model.UserId.Value, model.ReportYear.Value);

                model.SEC_User1 = new SEC_User();
                model.SEC_User1.Id = _subForm.SEC_User1.Id;
                //model.Wastes = model.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();			
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
                model.SEC_User1.Email = bufferRstRR.usremail;
                model.SEC_User1.IsGuest = _subForm.SEC_User1.IsGuest;
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

            #region dics
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);

            model.SubDicEnergyindicatorList = new SubDicEnergyindicator().GetAll().Where(x => x.forgu == true).ToList();
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;

            #endregion

            model.SUB_FormComment = _subForm.SUB_FormComment.ToList();
            return View("CreateGu", model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult EditGuNew(long id, string url)
        {
            //----
            var repository = new SubFormRepository();
            var _subForm = repository.GetById(id);

            var model = new SUB_FormGuNew
            {
                Id = _subForm.Id,
                StatusId = _subForm.StatusId,
                UserId = _subForm.UserId,
                ReportYear = _subForm.ReportYear,
                BeginPlanYear = _subForm.BeginPlanYear,
                EndPlanYear = _subForm.EndPlanYear,
                IsNotEvents = true,
                IsPlan = _subForm.IsPlan,
                IsEnergyManagementSystem = _subForm.IsEnergyManagementSystem,
                DesignDate = _subForm.DesignDate,
                DesignNote = _subForm.DesignNote,
                DesignDateStr = _subForm.DesignDateStr,
                Note = _subForm.Note,
                IsRent = _subForm.IsRent
            };

            //----
            if (model.Editor == null)
                model.Editor = MyExtensions.GetCurrentUserId();

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

                if (record.KindIndex==1)
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
            foreach(var record in form3aGu3)
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

            #region user info
            var bufferRstRR = repository.GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear.Value);
            if (bufferRstRR != null)
            {
                model.Wastes = repository.GetRstReportReestrOked(model.UserId.Value, model.ReportYear.Value);

                model.SEC_User1 = new SEC_User();
                model.SEC_User1.Id = _subForm.SEC_User1.Id;
                //model.Wastes = model.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();			
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

            #region dics
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);                             
           
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

            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;

            #endregion

            model.SUB_FormComment = _subForm.SUB_FormComment.ToList();
            return View("CreateGuNew", model);
        }

        [HttpGet]
        public ActionResult ShowComment(long modelId, string nameTable, int colIndex, int rowIndex, long? rowId, string fieldName)
        {
            var repository = new SubFormRepository();
            SUB_FormComment model = repository.GetComments(modelId, nameTable, colIndex, rowIndex);
            model.SubFormRecordHistories = repository.GetSubFormRecordHistories(modelId, nameTable, fieldName, rowId);
            if (model.SubFormRecordHistories == null)
            {
                model.SubFormRecordHistories = new List<SUB_FormRecordHistory>();
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult SaveComment(long modelId, string nameTable, int colIndex, int rowIndex, bool isError, string comment, long rowId, string fieldName, string fieldValue)
        {

            new SubFormRepository().SaveComment(modelId, nameTable, colIndex, rowIndex, isError, comment, rowId, fieldName, fieldValue, MyExtensions.GetCurrentUserId());

            return Json(new { Success = true });

        }
        public ActionResult DoOperation(long id)
        {
            string preambleXml = string.Empty;
            bool IsSuccess = DoOperationHelper(id, ref preambleXml);
            return Json(new
            {
                IsSuccess,
                preambleXml
            }, JsonRequestBehavior.AllowGet);
        }

        private bool DoOperationHelper(long id, ref string preambleXml)
        {
            var repository = new SubFormRepository();
            var model = repository.GetPreamble(id);

            bool IsSuccess = true;
            try
            {
                preambleXml = SerializeHelper.SerializeDataContract<SUB_Form>(model);
                preambleXml = preambleXml.Replace("utf-16", "utf-8");
            }
            catch (Exception e)
            {
                IsSuccess = false;
            }
            return IsSuccess;
        }

        public ActionResult SignForm(long preambleId, string xmlAuditForm, string signFio)
        {
            var isSuccess = true;
            var repository = new SubFormRepository();
            var model = repository.GetById(preambleId);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            
            #region original
            model.StatusId = CodeConstManager.STATUS_SEND_ID;
            model.SendDate = DateTime.Now;
            model.DesignNote = null;

            var history = new SUB_FormHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                FormId = model.Id,
                XmlSign = xmlAuditForm,
                IsSign = true,
                StatusId = CodeConstManager.STATUS_SEND_ID,
                UserId = MyExtensions.GetCurrentUserId(),
                Note = "Отчет предоставлен. Дата отправки:" + DateTime.Now,
                SignFio = model.SubjectBin + ", " + signFio
            };
            #endregion

            new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SubFormRepository().SaveHistory(history);
            new SubFormRepository().UpdateReestr(model);

            //---- Уведомление об успешной отправке отчета ГЭР
            new SendMessageManager().SendSmsToSubjectByCreateReport(MyExtensions.GetCurrentUserId(), model.ReportYear, "subject", signFio);
            new SendMessageManager().SendSmsToSubjectByCreateReport(MyExtensions.GetCurrentUserId(), model.ReportYear, "manager", signFio);

            return Json(new
            {
                isSuccess,
            }, JsonRequestBehavior.AllowGet);


        }

        //----without ecp 
        public ActionResult SignFormWoEcp(long preambleId)
        {
            string xmlAuditForm = string.Empty;
            var isSuccess = DoOperationHelper(preambleId, ref xmlAuditForm);

            var repository = new SubFormRepository();
            var model = repository.GetById(preambleId);
            if (model == null)
            {
                return RedirectToAction("Index");
            }

            #region  original
            model.StatusId = CodeConstManager.STATUS_SEND_ID;
            model.SendDate = DateTime.Now;
            model.DesignNote = null;

            var history = new SUB_FormHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                FormId = model.Id,
                XmlSign = xmlAuditForm,
                IsSign = true,
                StatusId = CodeConstManager.STATUS_SEND_ID,
                UserId = MyExtensions.GetCurrentUserId(),
                Note = "Отчет предоставлен. Дата отправки:" + DateTime.Now,
                SignFio = ""
            };
            #endregion

            new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SubFormRepository().SaveHistory(history);
            new SubFormRepository().UpdateReestr(model);

            //---- Уведомление об успешной отправке отчета ГЭР
            new SendMessageManager().SendSmsToSubjectByCreateReport(MyExtensions.GetCurrentUserId(), model.ReportYear, "subject", "");
            new SendMessageManager().SendSmsToSubjectByCreateReport(MyExtensions.GetCurrentUserId(), model.ReportYear, "manager", "");

            return Json(new
            {
                isSuccess,
            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult ShowSigned(long id, bool isShowSigned)
        {
            if (isShowSigned)
            {
                var repository = new SubFormRepository();
                var formHistory = repository.GetHistoryBySubFormId(id);
                if (formHistory != null && !string.IsNullOrEmpty(formHistory.XmlSign))
                    Session["SignedValue"] = SerializeHelper.DeserializeDataContract<SUB_Form>(formHistory.XmlSign);
                else
                    Session["SignedValue"] = null;
            }
            else
                Session["SignedValue"] = null;

            return RedirectToAction("ShowDetails", new { id });
        }

        public ActionResult CheckSendWOEcp()
        {
            long id = MyExtensions.GetCurrentUserId().Value;
            var sec_user = new SecUserRepository().GetById(id);
            Dictionary<string, object> item = new Dictionary<string, object>();
            item["ger_wo_ecp"] = sec_user.ger_wo_ecp;
            return Json(item);
        }

        public ActionResult CheckIsHaveGES()
        {
            string ErrorMessage = "";
            bool ishave = false;
            long? roleId = MyExtensions.GetRolesId();

            if (roleId.Value == 4)
            {

                ishave = new SecUserRepository().GetIsHaveGES(MyExtensions.GetCurrentUserId().Value);

            }

            if (ishave == false)
                ErrorMessage = ResourceSetting.IsHaveGESError;

            return Json(new
            {
                ishave,
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);

        }

        #region форма 1
        public ActionResult Form1SaveFile(int ReportYear, IEnumerable<HttpPostedFileBase> files)
        {
            long? currUserId = MyExtensions.GetCurrentUserId();
            string ErrorMessage = "";
            if (files != null)
            {
                try
                {
                    var dirpath = Server.MapPath("~/uploads/form1/" + currUserId.Value + "/" + ReportYear + "/");

                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                    }

                    foreach (var file in files)
                    {
                        if (file == null || file.ContentLength <= 0) continue;

                        Guid nguid = Guid.NewGuid();

                        var uploadFileName = Path.GetFileName(file.FileName);
                        if (uploadFileName == null) continue;

                        var arr = uploadFileName.Split('.');
                        var newFileName = nguid + "_" + arr[0] + "." + arr[arr.Length - 1];

                        var uploadFilePathAndName = Path.Combine(dirpath, newFileName);
                        ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }

            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForm1Files(int ReportYear, long userId)
        {
            string ErrorMessage = "";
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            try
            {
                var dirpath = Server.MapPath("~/uploads/form1/" + userId + "/" + ReportYear + "/");
                if (Directory.Exists(dirpath))
                {
                    string[] filePaths2 = Directory.GetFiles(dirpath, "*");
                    for (int i = 0; i < filePaths2.Length; i++)
                    {
                        string checkFiles = filePaths2[i];
                        if (!string.IsNullOrEmpty(checkFiles))
                        {
                            checkFiles = Replacer(checkFiles);
                            var arr = checkFiles.Split('*');
                            if (arr.Length > 0)
                            {
                                list.Add(new Dictionary<string, object>());
                                list.Last()["filename"] = arr[arr.Length - 1];
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }


            return Json(new
            {
                ErrorMessage,
                list
            }, JsonRequestBehavior.AllowGet);
        }

        //---- загрузить файл
        public ActionResult Form1Upload(int reportYear, long userId, string fname)
        {
            var dirpath = Server.MapPath("~/uploads/form1/" + userId + "/" + reportYear + "/");
            if (Directory.Exists(dirpath))
            {
                string physicalPath = dirpath + "/" + fname;
                return File(physicalPath, System.Net.Mime.MediaTypeNames.Application.Octet, fname);
            }

            return View();
        }

        //---- удалить файл
        public ActionResult DeleteForm1File(int reportYear, long userId, string fname)
        {
            string ErrorMessage = "";
            try
            {
                var dirpath = Server.MapPath("~/uploads/form1/" + userId + "/" + reportYear);
                if (Directory.Exists(dirpath))
                {
                    string physicalPath = dirpath + "/" + fname;

                    string[] filePaths2 = Directory.GetFiles(dirpath, fname);
                    for (int i = 0; i < filePaths2.Length; i++)
                    {
                        System.IO.File.Delete(filePaths2[i]);
                    }
                    //System.IO.File.Delete(physicalPath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region энергоаудит проводился
        //----upload save 
        public ActionResult SaveFile(int ReportYear, IEnumerable<HttpPostedFileBase> files)
        {

            long? currUserId = MyExtensions.GetCurrentUserId();
            string ErrorMessage = "";
            if (files != null)
            {
                try
                {
                    var dirpath = Server.MapPath("~/uploads/form4audit/" + currUserId.Value + "/" + ReportYear + "/");

                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                    }

                    int fcount = 0;
                    foreach (var file in files)
                    {
                        if (file == null || file.ContentLength <= 0) continue;

                        Guid nguid = Guid.NewGuid();

                        var uploadFileName = Path.GetFileName(file.FileName);
                        if (uploadFileName == null) continue;

                        var arr = uploadFileName.Split('.');
                        var newFileName = nguid + "_" + arr[0] + "." + arr[arr.Length - 1];

                        var uploadFilePathAndName = Path.Combine(dirpath, newFileName);
                        ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }

            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        //---
        public ActionResult GetFiles(int ReportYear, long userId)
        {
            string ErrorMessage = "";
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            try
            {
                var dirpath = Server.MapPath("~/uploads/form4audit/" + userId + "/" + ReportYear + "/");
                if (Directory.Exists(dirpath))
                {
                    string[] filePaths2 = Directory.GetFiles(dirpath, "*");
                    for (int i = 0; i < filePaths2.Length; i++)
                    {
                        string checkFiles = filePaths2[i];
                        if (!string.IsNullOrEmpty(checkFiles))
                        {
                            checkFiles = Replacer(checkFiles);
                            var arr = checkFiles.Split('*');
                            if (arr.Length > 0)
                            {
                                list.Add(new Dictionary<string, object>());
                                list.Last()["filename"] = arr[arr.Length - 1];
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }


            return Json(new
            {
                ErrorMessage,
                list
            }, JsonRequestBehavior.AllowGet);
        }

        private string Replacer(string str)
        {
            while (str.IndexOf("\\") != -1)
            {
                str = str.Replace("\\", "*");
            }

            return str;
        }

        //---- загрузить файл
        public ActionResult Upload(int reportYear, long userId, string fname)
        {
            var dirpath = Server.MapPath("~/uploads/form4audit/" + userId + "/" + reportYear + "/");
            if (Directory.Exists(dirpath))
            {
                string physicalPath = dirpath + "/" + fname;
                return File(physicalPath, System.Net.Mime.MediaTypeNames.Application.Octet, fname);
            }

            return View();
        }

        //---- удалить файл
        public ActionResult DeleteFile(int reportYear, long userId, string fname)
        {
            string ErrorMessage = "";
            try
            {
                var dirpath = Server.MapPath("~/uploads/form4audit/" + userId + "/" + reportYear);
                if (Directory.Exists(dirpath))
                {
                    string physicalPath = dirpath + "/" + fname;

                    string[] filePaths2 = Directory.GetFiles(dirpath, fname);
                    for (int i = 0; i < filePaths2.Length; i++)
                    {
                        System.IO.File.Delete(filePaths2[i]);
                    }
                    //System.IO.File.Delete(physicalPath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Json(new
            {
                ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult CheckBeforeSendReport(int subFormId, int reportYear)
        {
            bool isGu = false;
            bool isFillFormGu21 = false;
            bool isFillFormGu22 = false;
            bool isFillFormGu23 = false;
            bool isFillFormGu31 = false;
            bool isFillFormGu32 = false;
            bool isFillFormGu33 = false;
            var _subForm = new SubFormRepository().GetById(subFormId);
            if (MyExtensions.GetCurrentUserFSCode() == 3)
            {
                isGu = true;
                if (_subForm.SUB_Form2Gu != null && _subForm.SUB_Form2Gu.Count != 0)
                {
                    var form2Gu = _subForm.SUB_Form2Gu.FirstOrDefault();

                    if (form2Gu.CountOfEmployees != null)
                        isFillFormGu21 = true;
                    if (form2Gu.CountOfStudents != null)
                        isFillFormGu22 = true;
                    if (form2Gu.CountOfBeds != null)
                        isFillFormGu23 = true;


                }

                if (_subForm.SUB_Form3Gu != null && _subForm.SUB_Form3Gu.Count != 0)
                {
                    var form3Gu = _subForm.SUB_Form3Gu.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(form3Gu.YearOfConstruction))
                        isFillFormGu31 = true;
                    if (form3Gu.TotalAreaOfBuilding != null)
                        isFillFormGu32 = true;
                    if (form3Gu.HeatedAreaOfBuilding != null)
                        isFillFormGu33 = true;
                }

            }

            bool flag = false;
            try
            {
                var dirpath = Server.MapPath("~/uploads/form4audit/" + MyExtensions.GetCurrentUserId() + "/" + reportYear + "/");
                if (Directory.Exists(dirpath))
                {
                    string[] fileArr = Directory.GetFiles(dirpath, "*");
                    if (fileArr.Length > 0)
                        flag = true;
                }

            }
            catch (Exception ex)
            {
                flag = false;
            }

            bool isRent = false;
            try
            {
                if (_subForm.IsRent != null && _subForm.IsRent == true)
                {
                    var dirpath = Server.MapPath("~/uploads/form1/" + MyExtensions.GetCurrentUserId() + "/" + reportYear + "/");
                    if (Directory.Exists(dirpath))
                    {
                        string[] filePaths2 = Directory.GetFiles(dirpath, "*");
                        if (filePaths2.Length > 0)
                            isRent = true;
                    }
                }
                else isRent = true;
            }
            catch (Exception ex)
            {

            }

            return Json(new
            {
                IsExistFile = flag,
                isRent = isRent,
                isGu,
                isFillFormGu21,
                isFillFormGu22,
                isFillFormGu23,
                isFillFormGu31,
                isFillFormGu32,
                isFillFormGu33

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllCommentsByFormId(long formId, bool isGu = false)
        {
            var list = new SubFormRepository().GetAllCommentsByFormId(formId, isGu);
            return Json(list);
        }

        public ActionResult ChangeContactInfo(int reportYear, int subFormId, string fieldName, string val)
        {
            //----капироваться реквизиты
            new SubFormRepository().ChangeContactInfo(reportYear, fieldName, val);

            return Json("ok");
        }

    }
}
