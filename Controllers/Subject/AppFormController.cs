using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Entity.Subject;
using System.Web.UI.WebControls;

namespace Aisger.Controllers.Subject
{
    public class AppFormController : ASubjectController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string _actionName = filterContext.ActionDescriptor.ActionName;
            if (MyExtensions.GetRolesId() == 4 && _actionName.IndexOf("ShowDetails")==-1 && _actionName.IndexOf("ApplicationEditForRstRR") == -1)
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }
            //base.OnActionExecuting(filterContext);
        }

        //
        // GET: /AppForm/
        [GerNavigateLogger]
        public ActionResult CommonView(int? year, long? sendId, string idk, string biniin, string owner, string status, string oblast)
        {
            var filter = new SUB_FormFilter();
            filter.ReportYear = year;
            filter.SendId = sendId;
            if (!string.IsNullOrEmpty(biniin))
            {
                filter.BINIIN = biniin;
            }
            if (!string.IsNullOrEmpty(idk))
            {
                filter.IDK = idk;
            }
           
            if (!string.IsNullOrEmpty(owner))
            {
                filter.JuridicalName = owner;
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
          
            if (!string.IsNullOrEmpty(oblast))
            {
                filter.Oblasts = new List<string>();
                var statuslList = oblast.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Oblasts.Add(s);
                }
            }
          
            FillCommonViewBag(filter);
            filter.SubFormRecords = new SubFormRepository().GetCommonReestrsByFilter(filter);

            // FillViewFiltertBag(filter);
            return View(filter);
        }

        [HttpGet]
        public ActionResult ApplicationEdit(long Id)
        {
            var user = new AccountRepository().GetUserById(Id);
            var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_SUBJECT);
            if (Request.UrlReferrer != null) model.PreviousUrl = Request.UrlReferrer.ToString();
          
            FillBagRegistrationGuest(model);
            return View(model);
        }

		#region update rst_report_reestr users data

		[HttpGet]
		public ActionResult ApplicationEditForRstRR(long Id, int ReportYear)
		{
			var user = new AccountRepository().GetUserById(Id);
			var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_SUBJECT);
			if (Request.UrlReferrer != null) model.PreviousUrl = Request.UrlReferrer.ToString();

			var rstrr_item = new RstReportRepository().GetRecordByUserIdVSReportYear(Id, ReportYear);
			if (rstrr_item != null)
			{
				model.LastName = rstrr_item.usrlastname;
				model.SecondName = rstrr_item.usrsecondname;
				model.FirstName = rstrr_item.usrfirstname;
				model.JuridicalName = rstrr_item.usrjuridicalname;
				model.Post = rstrr_item.usrpost;
				model.Mobile = rstrr_item.usrmobile;
				model.WorkPhone = rstrr_item.usrworkphone;
				model.InternalPhone = rstrr_item.usrinternalphone;
				model.Address = rstrr_item.usraddress;

				model.IsCvazy = (rstrr_item.usriscvazy != null) ? Convert.ToBoolean(rstrr_item.usriscvazy) : false;
				model.ResponceFIO = rstrr_item.usrresponcefio;
				model.ResponcePost = rstrr_item.usrresponcepost;
				model.Oblast = Convert.ToInt64(rstrr_item.usroblast);
				model.Region = (rstrr_item.usrregion != null) ? Convert.ToInt64(rstrr_item.usrregion) : 0;
				model.SubRegion = rstrr_item.usrsubregion;
				model.Village = rstrr_item.usrvillage;
				model.TypeApplicationId = Convert.ToInt64(rstrr_item.usrtypeapplicationid);
				model.OkedId = rstrr_item.usrokedid;	
				model.IDK = rstrr_item.usridk;
				model.ReportYear = ReportYear;
				model.Wastes = new SubFormRepository().GetRstReportReestrOked(model.Id, ReportYear);
                model.Email = rstrr_item.usremail;
                model.FSCode = rstrr_item.usrfscode;
			}

			FillBagRegistrationGuest(model);

			return View(model);
		}

		[HttpPost]
		public ActionResult ApplicationEditForRstRR(SEC_Guest model)
		{
			FillBagRegistrationGuest(model);

			if (model.Region == 0)
			{
				ModelState.AddModelError("JuridicalKato", ResourceSetting.RegionNotEmpty);
			}

			if (string.IsNullOrEmpty(model.Post))
			{
				ModelState.AddModelError("Post", ResourceSetting.NotEmpty);
			}

            if (model.FSCode == null)
            {
                ModelState.AddModelError("FSCode", ResourceSetting.NotEmpty);
                return View(model);
            }
            
            if (model.Village == 0)
            {
                model.Village = null;
            }

			if (model.SubRegion == 0)
			{
				model.SubRegion = null;
			}

			var rstrr_item = new RstReportRepository().GetRecordByUserIdVSReportYear(model.Id, model.ReportYear);
			if (rstrr_item != null)
			{
				rstrr_item.usrlastname = model.LastName;
				rstrr_item.usrsecondname = model.SecondName;
				rstrr_item.usrfirstname = model.FirstName;
				rstrr_item.usrjuridicalname = model.JuridicalName;
				rstrr_item.usrpost = model.Post;
				rstrr_item.usrmobile = model.Mobile;
				rstrr_item.usrworkphone = model.WorkPhone;
				rstrr_item.usrinternalphone = model.InternalPhone;
				rstrr_item.usraddress = model.Address;

				rstrr_item.usriscvazy = model.IsCvazy;
				rstrr_item.usrresponcefio = model.ResponceFIO;
				rstrr_item.usrresponcepost = model.ResponcePost;
				rstrr_item.usroblast = model.Oblast;
				rstrr_item.usrregion = model.Region;
				rstrr_item.usrsubregion = model.SubRegion;
				rstrr_item.usrvillage = model.Village;
				rstrr_item.usrtypeapplicationid = model.TypeApplicationId;
				rstrr_item.usrokedid = model.OkedId;
				rstrr_item.usridk = model.IDK;
                rstrr_item.usremail = model.Email;
                rstrr_item.usrfscode = model.FSCode;
				new RstReportRepository().UpdateReportReestrByUserIdVSReportYear(rstrr_item);
                //new SecUserRepository().UpdateSECUser(model);

                new SubFormRepository().InsertRstReportReestrOked(rstrr_item.Id,model.Wastes);

			}

			if (!string.IsNullOrEmpty(model.PreviousUrl))
			{
				return Redirect(model.PreviousUrl);
			}
			//return RedirectToAction("Index");


			return View(model);
		}
		#endregion

		protected void RemoveManadatoryFields()
        {
            ModelState.Remove("Pwd");
            ModelState.Remove("ConfirmPwd");
            ModelState.Remove("OkedId");
            ModelState.Remove("ResponcePost");
            ModelState.Remove("ResponceFIO");
            ModelState.Remove("Mobile");
            ModelState.Remove("Email");
        }
        [HttpPost]
        public ActionResult ApplicationEdit(SEC_Guest model)
        {
            RemoveManadatoryFields();
            FillBagRegistrationGuest(model);
           
            if (model.Region == 0)
            {
                ModelState.AddModelError("JuridicalKato", ResourceSetting.RegionNotEmpty);
            }
            if (string.IsNullOrEmpty(model.Post))
            {
                ModelState.AddModelError("Post", ResourceSetting.NotEmpty);
            }
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
                model.Kinds.Add(CodeConstManager.KIND_USER_ESCO.ToString(CultureInfo.InvariantCulture));
                repository.RegisteredUser(model, MyExtensions.GetCurrentUserId());
                if (!string.IsNullOrEmpty(model.PreviousUrl))
                {
                    return Redirect(model.PreviousUrl);
                }
                return RedirectToAction("Index");
            }
            return View(model);

        }

        protected virtual void FillBagRegistrationGuest(SEC_Guest model)
        {

            model.WastList = new AccountController().GetPlants(model.Wastes);
            model.KindList = new AccountController().GetKinds(model.Kinds);
            ViewData["TypeApplicationList"] = new SelectList(new DicTypeApplicationRepository().GetAll(), "Id", "ShortName" + CultureHelper.GetCurrentCulture(), model.TypeApplicationId);
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.OkedId);
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            ViewData["OblastList"] = new SelectList(listanimal, "Id",
                                                 CultureHelper.GetDictionaryName("NameRu"), model.Oblast);

            ViewData["RegionList"] = new SelectList(repository.GetKatos(model.Oblast, true), "Id",
                                                   CultureHelper.GetDictionaryName("NameRu"), model.Region);

            ViewData["SubRegionList"] = new SelectList(repository.GetKatos(model.Region, false), "Id",
                                                   CultureHelper.GetDictionaryName("NameRu"), model.SubRegion);

            ViewData["VillageList"] = new SelectList(repository.GetKatos(model.SubRegion, false), "Id",
                                                   CultureHelper.GetDictionaryName("NameRu"), model.Village);

            var list = new List<FsCode>();
            list.Add(new FsCode() { Id = null, NameRu = "" });
            list.Add(new FsCode() { Id = 1, NameRu = "юр" });
            list.Add(new FsCode() { Id = 2, NameRu = "кв" });
            list.Add(new FsCode() { Id = 3, NameRu = "гу" });
            list.Add(new FsCode() { Id = 4, NameRu = "ип" });
            ViewData["FsCodeList"] = new SelectList(list, "Id", "NameRu", model.FSCode);
        }
    
        public ActionResult ExportExcel(int? year, long? sendId, string idk, string biniin, string owner, string status, string oblast)
        {

            var filter = new SUB_FormFilter();
            filter.ReportYear = year;
            filter.SendId = sendId;
            if (!string.IsNullOrEmpty(biniin))
            {
                filter.BINIIN = biniin;
            }
            if (!string.IsNullOrEmpty(idk))
            {
                filter.IDK = idk;
            }

            if (!string.IsNullOrEmpty(owner))
            {
                filter.JuridicalName = owner;
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

            if (!string.IsNullOrEmpty(oblast))
            {
                filter.Oblasts = new List<string>();
                var statuslList = oblast.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Oblasts.Add(s);
                }
            }
            FillCommonViewBag(filter);
            filter.SubFormRecords = new SubFormRepository().GetCommonReestrsByFilter(filter);

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
            ws.Cells["E1"].Value = ResourceSetting.Oblast;
            ws.Cells["E1"].Style.Font.Bold = true;
            ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["F1"].Value = ResourceSetting.RstDicStatus;
            ws.Cells["F1"].Style.Font.Bold = true;
            ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            ws.Cells["G1"].Value = ResourceSetting.ReportReason;
            ws.Cells["G1"].Style.Font.Bold = true;
            ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
         

            var reportReestrFilters = filter.SubFormRecords as SUB_FormRecord[] ??
                                      filter.SubFormRecords.ToArray();
            for (var i = 0; i < reportReestrFilters.Count(); i++)
            {
                var index = i + 2;
                var reestr = reportReestrFilters[i];
                var reasonName = "";
                if (reestr.IsBack)
                {
                    reasonName = CodeConstManager.SUB_REASON_SEND;
                }
                ws.Cells["A" + index].Value = reestr.BINIIN;
                ws.Cells["B" + index].Value = reestr.IDK;
                ws.Cells["B" + index].Style.WrapText = true;
                ws.Cells["C" + index].Value = reestr.JuridicalName;
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

                ws.Cells["E" + index].Value = reestr.OblastName;
                ws.Cells["E" + index].Style.WrapText = true;
                ws.Cells["F" + index].Value = reestr.StatusName;
                ws.Cells["F" + index].Style.WrapText = true;
                ws.Cells["G" + index].Value = reasonName;
                ws.Cells["G" + index].Style.WrapText = true;
              
            }

            FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = "report.xlsx";
            return result;
        }
        
        private void FillCommonViewBag(SUB_FormFilter filter)
        {
            filter.StatusList = GetStatus(filter.Statuses);
            filter.OblastList = GetOblasList(filter.Oblasts);
            var listanimal = new SubFormRepository().GetYears();
            if (filter.ReportYear == null)
            {
                filter.ReportYear = (int?)listanimal.Max(e => e.ID);
            }
            ViewData["Years"] = new SelectList(listanimal, "ID",
                                                 "NAME_RU", filter.ReportYear);

            var reason = new List<UnMappedDictionary>
            {
                new UnMappedDictionary(CodeConstManager.SUB_REASON_SEND_ID, CodeConstManager.SUB_REASON_SEND),
                new UnMappedDictionary(CodeConstManager.SUB_REASON_NOTSEND_ID, CodeConstManager.SUB_REASON_NOTSEND),
                new UnMappedDictionary(CodeConstManager.SUB_REASON_ALL_ID, CodeConstManager.SUB_REASON_ALL)
            };

            ViewData["Reasons"] = new SelectList(reason, "ID",
                                               "NAME_RU", filter.SendId);
            ViewBag.IsAcceptReport = IsAcceptReport();
        }
        public MultiSelectList GetOblasList(IList<string> selectedValues)
        {
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            return new MultiSelectList(listanimal, "Id", "NameRu", selectedValues);
        }
        public MultiSelectList GetStatus(IList<string> selectedValues)
        {
            var plants = new SubDicStatusRepository().GetAll();
            var list = new List<SUB_DIC_Status>();
            foreach (var subDicStatuse in plants)
            {
                if (subDicStatuse.Id != 1)
                {
                    list.Add(subDicStatuse);
                }
            }
            return new MultiSelectList(list, "Id", "NameRu", selectedValues);
        }

        [GerNavigateLogger]
        public ActionResult Index(int? id)
        {
            var listanimal = new SubFormRepository().GetYears();
            var reportYear = id;
            if (reportYear == null)
            {
                reportYear = 1;
            }
            ViewData["Years"] = new SelectList(listanimal, "ID",
                                                 "NAME_RU", reportYear);
            ViewBag.ReportYear = reportYear;

            var oblasts = new List<string>();
            foreach (var code in CodeConstManager.OBLAST_CODES)
            {
                if (MyExtensions.CheckRight(code))
                {
                    oblasts.Add(code);
                }
            }
            if (oblasts.Count>0)
            {
                var list =
                    new SubFormRepository().GetListByOblast(oblasts, reportYear);
                return View(list);

            }

            var currentList = new List<SUB_Form>();
//                new SubFormRepository().GetCurrentEmployee(MyExtensions.GetCurrentUserId());
          

            return View(currentList);
        }
        
        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Design(long id)
        {
            ViewBag.SubReadonly = true;

            var repository = new SubFormRepository();
            var model = repository.GetById(id);

            if (repository.GetFSCode(model.UserId.Value,model.ReportYear) == 3)
            {
                if (model.ReportYear >= 2018 && model.ReportYear < 2019)
                {
                    return RedirectToAction("DesignGu", new { id = id });
                }
                else
                {
                    return RedirectToAction("DesignGuNew", new { id = id });
                }
            }

            model.AttachFiles = new List<string>();
            model.DesignDate = DateTime.Now;
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2")).OrderBy(e => e.PosIndex).ToList();
            
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
            
			//model.SubForm4Records = new List<SUB_Form4Record>();
			//var forms4 = model.SUB_Form4Record.OrderBy(e => e.Id);
			//foreach (var record in forms4)
			//{
			//	model.SubForm4Records.Add(record);
			//}
			//----form4
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

			//----form5
            model.SubForm5Records = new List<SUB_Form5Record>();
			model.SubForm5RecordsOther = new List<SUB_Form5Record>();
            var forms5 = model.SUB_Form5Record.OrderBy(e => e.Id);
            foreach (var record in forms5)
            {
				if (record.energyindicator_id == 0 || record.energyindicator_id == null)
				{
					model.SubForm5RecordsOther.Add(record);
				}
				else
				{
					model.SubForm5Records.Add(record);
				}
            }

			//----form6
            model.SubForm6Records = new List<SUB_Form6Record>();
            var forms6 = model.SUB_Form6Record.OrderBy(e => e.Id);
            foreach (var record in forms6)
            {
                model.SubForm6Records.Add(record);
            }

			var rst_reportreestr = model.RST_ReportReestr.FirstOrDefault();
			if (rst_reportreestr != null)
			{
				model.Wastes = repository.GetRstReportReestrOked(model.UserId.Value, model.ReportYear);
			}

            var bufferRstRR = model.RST_ReportReestr.Where(x => x.RST_Report.ReportYear == model.ReportYear).FirstOrDefault();
			if (bufferRstRR != null)
			{
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
            FillHistory(model);
            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll(); ;
            if (Request.UrlReferrer != null) model.PreviousUrl = Request.UrlReferrer.ToString();
            return View(model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult DesignGu(long id)
        {
            int? kindIndex;
            int num;       

            SUB_Form _subForm = new SubFormRepository().GetById(id);
            _subForm.DesignDate = DateTime.Now;
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
                SendDate = _subForm.SendDate,
                DesignDateStr = _subForm.DesignDateStr,
                IsRent=_subForm.IsRent
            };

            model.AttachFiles = new List<string>();
            model.DesignDate = DateTime.Now;

            if (!model.Editor.HasValue)
            {
                model.Editor = MyExtensions.GetCurrentUserId();
            }

            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll();
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            
            var years = new SubFormRepository().GetPastYears(model.UserId).Where(e => e.ReportYear != model.ReportYear.Value);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            var listanimal = IsAcceptReport() ? new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) :new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));   //(e.Id != CodeConstManager.STATUS_ACCEPT_ID)
            ViewData["statusList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.StatusId);


            if (Session["SignedValue"] is SUB_Form)
            {
                model.SignedSubForm = (SUB_Form)Session["SignedValue"];
            }
            else model.SignedSubForm = null;


            #region form2
            //----resource 1
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e => e.Code != null && e.Code.Contains("2") && e.IsGu == true).OrderBy(e => e.PosIndex);
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

            //----resource 2
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

            #region gu
            model.SUB_Form2Gu = (_subForm.SUB_Form2Gu.Count != 0) ? _subForm.SUB_Form2Gu.FirstOrDefault<SUB_Form2Gu>() : new SUB_Form2Gu();
            model.SUB_Form3Gu = (_subForm.SUB_Form3Gu.Count != 0) ? _subForm.SUB_Form3Gu.FirstOrDefault<SUB_Form3Gu>() : new SUB_Form3Gu();
            model.SUB_FormGuLightingInfo = _subForm.SUB_FormGuLightingInfo.ToList<SUB_FormGuLightingInfo>();
            if (model.SUB_FormGuLightingInfo.Count == 0)
            {
                model.SUB_FormGuLightingInfo = new List<SUB_FormGuLightingInfo>();
                SUB_FormGuLightingInfo info = new SUB_FormGuLightingInfo();
                model.SUB_FormGuLightingInfo.Add(info);
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

            #region form4         
            model.SubForm4Records = new List<SUB_Form4Record>();
            model.SubForm4RecordsOther = new List<SUB_Form4Record>();

            foreach (SUB_Form4Record record5 in _subForm.SUB_Form4Record)
            {
                kindIndex = record5.KindIndex;
                num = 2;
                if ((kindIndex.GetValueOrDefault() == num) ? kindIndex.HasValue : false)
                {
                    model.SubForm4RecordsOther.Add(record5);
                }
                else
                {
                    model.SubForm4Records.Add(record5);
                }
            }

            if ((model.SubForm4Records == null) || (model.SubForm4Records.Count == 0))
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                SUB_Form4Record record6 = new SUB_Form4Record();
                model.SubForm4Records.Add(record6);
            }

            if ((model.SubForm4RecordsOther == null) || (model.SubForm4RecordsOther.Count == 0))
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                SUB_Form4Record record7 = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(record7);
            }

            List<SUB_DIC_Event> list2 = new SubDicEventRepository().GetAll(model.UserId);
            List<SUB_DIC_TypeCounter> typeCounter = new SubDucTypeCounterRepository().GetAll();
            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                ViewData["TypeCounters" + i]= new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
                ViewData["DicEvents" + i]= new SelectList(list2, "Id", "NameRu", model.SubForm4Records[i].EventId);
                long? typeCounterId = model.SubForm4Records[i].TypeCounterId;
                long num3 = 0L;
                if ((typeCounterId.GetValueOrDefault() == num3) ? typeCounterId.HasValue : false)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }

                typeCounterId = model.SubForm4Records[i].EventId;
                num3 = 0L;

                if ((typeCounterId.GetValueOrDefault() == num3) ? typeCounterId.HasValue : false)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }
            }

            for (int j = 0; j < model.SubForm4RecordsOther.Count; j++)
            {
                ViewData["TypeCountersOther" + j] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[j].TypeCounterId);
                ViewData["DicEventsOther" + j]= new SelectList(list2, "Id", "NameRu", model.SubForm4RecordsOther[j].EventId);
            }
            #endregion

            #region form5
            model.SubForm5Records = new List<SUB_Form5Record>();
            model.SubForm5RecordsOther = new List<SUB_Form5Record>();
            foreach (SUB_Form5Record record8 in  _subForm.SUB_Form5Record)
            {
                kindIndex = record8.KindIndex;
                num = 2;
                if ((kindIndex.GetValueOrDefault() == num) ? kindIndex.HasValue : false)
                {
                    model.SubForm5RecordsOther.Add(record8);
                }
                else
                {
                    model.SubForm5Records.Add(record8);
                }
            }

            if ((model.SubForm5Records == null) || (model.SubForm5Records.Count == 0))
            {
                model.SubForm5Records = new List<SUB_Form5Record>();
                SUB_Form5Record record9 = new SUB_Form5Record();
                model.SubForm5Records.Add(record9);
            }

            if ((model.SubForm5RecordsOther == null) || (model.SubForm5RecordsOther.Count == 0))
            {
                model.SubForm5RecordsOther = new List<SUB_Form5Record>();
                SUB_Form5Record record10 = new SUB_Form5Record();
                model.SubForm5RecordsOther.Add(record10);
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

            #region user info
       
            RST_ReportReestr rstReportReestr= new SubFormRepository().GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear.Value);
            if (rstReportReestr != null)
            {
                model.SEC_User1 = _subForm.SEC_User1;

                model.Wastes = _subForm.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();
                model.SEC_User1.Id = _subForm.SEC_User1.Id;
                model.SEC_User1.LastName = rstReportReestr.usrlastname;
                model.SEC_User1.SecondName = rstReportReestr.usrsecondname;
                model.SEC_User1.FirstName = rstReportReestr.usrfirstname;
                model.SEC_User1.JuridicalName = rstReportReestr.usrjuridicalname;
                model.SEC_User1.Post = rstReportReestr.usrpost;
                model.SEC_User1.Mobile = rstReportReestr.usrmobile;
                model.SEC_User1.WorkPhone = rstReportReestr.usrworkphone;
                model.SEC_User1.InternalPhone = rstReportReestr.usrinternalphone;
                model.SEC_User1.Address = rstReportReestr.usraddress;
                model.SEC_User1.BINIIN = rstReportReestr.BINIIN;
                model.SEC_User1.IsCvazy = rstReportReestr.usriscvazy.HasValue ? Convert.ToBoolean(rstReportReestr.usriscvazy) : false;
                model.SEC_User1.ResponceFIO = rstReportReestr.usrresponcefio;
                model.SEC_User1.ResponcePost = rstReportReestr.usrresponcepost;
                model.SEC_User1.Oblast = rstReportReestr.usroblast;
                model.SEC_User1.Region = rstReportReestr.usrregion;
                model.SEC_User1.SubRegion = rstReportReestr.usrsubregion;
                model.SEC_User1.Village = rstReportReestr.usrvillage;
                model.SEC_User1.TypeApplicationId = Convert.ToInt64(rstReportReestr.usrtypeapplicationid);
                model.SEC_User1.OkedId = rstReportReestr.usrokedid;
                model.SEC_User1.FSCode = rstReportReestr.usrfscode;
                model.SEC_User1.IDK = rstReportReestr.usridk;
                model.SEC_User1.IsGuest = _subForm.SEC_User1.IsGuest;
                model.SEC_User1.Email = rstReportReestr.usremail;

                if (model.SEC_User1.OkedId != null)
                    model.SEC_User1.DIC_OKED = new DicOkedRepository().GetAll().FirstOrDefault(x => x.Id == model.SEC_User1.OkedId.Value);
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
            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().ToList();

            ViewData["DicEvents"] = new SelectList(list2, "Id", "NameRu", 0);

            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);

            model.SubDicEnergyindicatorList = new SubDicEnergyindicator().GetAll().Where(x=>x.forgu==true).ToList();

            if (model.SEC_User1.OkedId != null)
                ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);
            else ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", null);

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;           
            #endregion

            return base.View(model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult DesignGuNew(long id)
        {
            int? kindIndex;
            int num;

            SUB_Form _subForm = new SubFormRepository().GetById(id);
            _subForm.DesignDate = DateTime.Now;

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
                SendDate = _subForm.SendDate,
                DesignDateStr = _subForm.DesignDateStr,
                IsRent = _subForm.IsRent
            };

            model.AttachFiles = new List<string>();
            model.DesignDate = DateTime.Now;

            if (!model.Editor.HasValue)
            {
                model.Editor = MyExtensions.GetCurrentUserId();
            }

            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll();
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }

            var years = new SubFormRepository().GetPastYears(model.UserId).Where(e => e.ReportYear != model.ReportYear.Value);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            var listanimal = IsAcceptReport() ? new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));   //(e.Id != CodeConstManager.STATUS_ACCEPT_ID)
            ViewData["statusList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.StatusId);


            if (Session["SignedValue"] is SUB_Form)
            {
                model.SignedSubForm = (SUB_Form)Session["SignedValue"];
            }
            else model.SignedSubForm = null;
            
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

            #region dics
            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().ToList();
            model.SubDicEnergyindicatorList = new SubDicEnergyindicator().GetAll().Where(x => x.forgu == true).ToList();
            
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

            #region user info

            RST_ReportReestr rstReportReestr = new SubFormRepository().GetRstReportReestrByUserId(model.UserId.Value, model.ReportYear.Value);
            if (rstReportReestr != null)
            {
                model.SEC_User1 = _subForm.SEC_User1;

                model.Wastes = _subForm.SEC_User1.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();
                model.SEC_User1.Id = _subForm.SEC_User1.Id;
                model.SEC_User1.LastName = rstReportReestr.usrlastname;
                model.SEC_User1.SecondName = rstReportReestr.usrsecondname;
                model.SEC_User1.FirstName = rstReportReestr.usrfirstname;
                model.SEC_User1.JuridicalName = rstReportReestr.usrjuridicalname;
                model.SEC_User1.Post = rstReportReestr.usrpost;
                model.SEC_User1.Mobile = rstReportReestr.usrmobile;
                model.SEC_User1.WorkPhone = rstReportReestr.usrworkphone;
                model.SEC_User1.InternalPhone = rstReportReestr.usrinternalphone;
                model.SEC_User1.Address = rstReportReestr.usraddress;
                model.SEC_User1.BINIIN = rstReportReestr.BINIIN;
                model.SEC_User1.IsCvazy = rstReportReestr.usriscvazy.HasValue ? Convert.ToBoolean(rstReportReestr.usriscvazy) : false;
                model.SEC_User1.ResponceFIO = rstReportReestr.usrresponcefio;
                model.SEC_User1.ResponcePost = rstReportReestr.usrresponcepost;
                model.SEC_User1.Oblast = rstReportReestr.usroblast;
                model.SEC_User1.Region = rstReportReestr.usrregion;
                model.SEC_User1.SubRegion = rstReportReestr.usrsubregion;
                model.SEC_User1.Village = rstReportReestr.usrvillage;
                model.SEC_User1.TypeApplicationId = Convert.ToInt64(rstReportReestr.usrtypeapplicationid);
                model.SEC_User1.OkedId = rstReportReestr.usrokedid;
                model.SEC_User1.FSCode = rstReportReestr.usrfscode;
                model.SEC_User1.IDK = rstReportReestr.usridk;
                model.SEC_User1.IsGuest = _subForm.SEC_User1.IsGuest;
                model.SEC_User1.Email = rstReportReestr.usremail;

                if (model.SEC_User1.OkedId != null)
                    model.SEC_User1.DIC_OKED = new DicOkedRepository().GetAll().FirstOrDefault(x => x.Id == model.SEC_User1.OkedId.Value);
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
            if (model.SEC_User1.OkedId != null)
                ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);
            else ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", null);

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
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;
            #endregion

            return base.View(model);
        }

        [HttpPost]
        public ActionResult Create(SUB_Form model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
         
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
            
            if (model.Id == 0)
            {
                new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            }
            var history = new SUB_FormHistory
            {
                FormId = model.Id,
                StatusId = model.StatusId,
                UserId = MyExtensions.GetCurrentUserId(),
                RegDate = DateTime.Now
            };
            new SubFormRepository().SaveHistory(history);

                var dirpath = Server.MapPath("~/uploads/subhsitory/" + history.Id);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }

               
                foreach (var file in httpPostedFileBases)
                {
                    if (file == null || file.ContentLength <= 0) continue;
                    var uploadFileName = Path.GetFileName(file.FileName);
                    if (uploadFileName == null) continue;
                    var uploadFilePathAndName = Path.Combine(dirpath, uploadFileName);
                    ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                }
                if (!string.IsNullOrEmpty(model.PreviousUrl))
                {
                    return Redirect(model.PreviousUrl);
                }

                return RedirectToAction("CommonView","RstReport");

        }

        [HttpPost]
		public ActionResult Design(SUB_Form model, IEnumerable<HttpPostedFileBase> files, bool IsSendMessage=false)
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

                #region kazirwe 
                var history = new SUB_FormHistory
                {
                    FormId = model.Id,
                    StatusId = model.StatusId,
                    UserId = MyExtensions.GetCurrentUserId(),
                    RegDate =(model.SendDate!=null)?model.SendDate.Value.AddDays(1):DateTime.Now,
                    Note = model.DesignNote
                };
                #endregion

                #region origin
                //var history = new SUB_FormHistory
                //{
                //    FormId = model.Id,
                //    StatusId = model.StatusId,
                //    UserId = MyExtensions.GetCurrentUserId(),
                //    RegDate = DateTime.Now,
                //    Note=model.DesignNote
                //};
                #endregion

                //----send notify
                if (IsSendMessage == true) //&& (model.StatusId == 4 || model.StatusId == 5 || model.StatusId == 6))
				{
					new SendMessageManager().SendSmsToSubjectByChangeStatus(model.UserId.Value, model.ReportYear, model.StatusId.Value);
					//                    model.IsBack = true;
					//new SendMessageManager().SendSubForm(model);
				}

                if (!IsAcceptReport())
                {
                    model.Editor = MyExtensions.GetCurrentUserId();
                }

				//----- check isPlan
				if (model.IsPlan == null)
					model.IsPlan = false;

                //new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
				new SubFormRepository().ChangeStatusByManager(model);
                new SubFormRepository().SaveHistory(history);

				//----send notify
				//	if (IsSendMessage)
				//	new SendMessageManager().SendSmsToSubjectByChangeStatus(model.UserId, model.ReportYear, model.StatusId.Value);

                var dirpath = Server.MapPath("~/uploads/subhsitory/" + history.Id);
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
          
                if (!string.IsNullOrEmpty(model.PreviousUrl))
                {
                    return Redirect(model.PreviousUrl);
                }
                return RedirectToAction("CommonView", "RstReport");
            }
            FillHistory(model);
            ViewBag.SubReadonly = true;
            FillViewBag(model);
            
            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll();

            return View(model);

        }

        [HttpPost]
        public ActionResult DesignGu(Sub_FormGu model, IEnumerable<HttpPostedFileBase> files, bool IsSendMessage = false)
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
                               
                #region  origin
                var history = new SUB_FormHistory
                {
                    FormId = model.Id,
                    StatusId = model.StatusId,
                    UserId = MyExtensions.GetCurrentUserId(),
                    RegDate = DateTime.Now
                };
                #endregion

                //----send notify
                if (IsSendMessage == true) //&& (model.StatusId == 4 || model.StatusId == 5 || model.StatusId == 6))
                {
                    new SendMessageManager().SendSmsToSubjectByChangeStatus(model.UserId.Value, model.ReportYear.Value, model.StatusId.Value);
                    //                    model.IsBack = true;
                    //new SendMessageManager().SendSubForm(model);
                }

                if (!IsAcceptReport())
                {
                    model.Editor = MyExtensions.GetCurrentUserId();
                }

                //----- check isPlan
                if (model.IsPlan == null)
                    model.IsPlan = false;

                var _subForm = new SubFormRepository().GetById(model.Id);
                _subForm.StatusId = model.StatusId;
                _subForm.DesignDate = model.DesignDate;
                _subForm.DesignNote = model.DesignNote;
                _subForm.Editor = model.Editor;
                new SubFormRepository().ChangeStatusByManager(_subForm);
                new SubFormRepository().SaveHistory(history);

                //----send notify
                //	if (IsSendMessage)
                //	new SendMessageManager().SendSmsToSubjectByChangeStatus(model.UserId, model.ReportYear, model.StatusId.Value);

                var dirpath = Server.MapPath("~/uploads/subhsitory/" + history.Id);
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

                if (!string.IsNullOrEmpty(model.PreviousUrl))
                {
                    return Redirect(model.PreviousUrl);
                }
                return RedirectToAction("CommonView", "RstReport");
            }
      
            ViewBag.SubReadonly = true;

            #region gu
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
            #region form4
            if ((model.SubForm4Records == null) || (model.SubForm4Records.Count == 0))
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                SUB_Form4Record record6 = new SUB_Form4Record();
                model.SubForm4Records.Add(record6);
            }

            if ((model.SubForm4RecordsOther == null) || (model.SubForm4RecordsOther.Count == 0))
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                SUB_Form4Record record7 = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(record7);
            }

            List<SUB_DIC_Event> list2 = new SubDicEventRepository().GetAll(model.UserId);
            List<SUB_DIC_TypeCounter> typeCounter = new SubDucTypeCounterRepository().GetAll();
            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                ViewData["TypeCounters" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
                ViewData["DicEvents" + i] = new SelectList(list2, "Id", "NameRu", model.SubForm4Records[i].EventId);
                long? typeCounterId = model.SubForm4Records[i].TypeCounterId;
                long num3 = 0L;
                if ((typeCounterId.GetValueOrDefault() == num3) ? typeCounterId.HasValue : false)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }

                typeCounterId = model.SubForm4Records[i].EventId;
                num3 = 0L;

                if ((typeCounterId.GetValueOrDefault() == num3) ? typeCounterId.HasValue : false)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }
            }

            for (int j = 0; j < model.SubForm4RecordsOther.Count; j++)
            {
                ViewData["TypeCountersOther" + j] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[j].TypeCounterId);
                ViewData["DicEventsOther" + j] = new SelectList(list2, "Id", "NameRu", model.SubForm4RecordsOther[j].EventId);
            }
            #endregion

            #region form5
            if ((model.SubForm5Records == null) || (model.SubForm5Records.Count == 0))
            {
                model.SubForm5Records = new List<SUB_Form5Record>();
                SUB_Form5Record record9 = new SUB_Form5Record();
                model.SubForm5Records.Add(record9);
            }

            if ((model.SubForm5RecordsOther == null) || (model.SubForm5RecordsOther.Count == 0))
            {
                model.SubForm5RecordsOther = new List<SUB_Form5Record>();
                SUB_Form5Record record10 = new SUB_Form5Record();
                model.SubForm5RecordsOther.Add(record10);
            }
            #endregion

            #region form6
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

            #region history
            model.SUB_FormHistory = model.SUB_FormHistory.ToList();
            foreach (var rstReestrHistory in model.SUB_FormHistory)
            {
                var dir1 = Server.MapPath("~/uploads/subhsitory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files2 = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files2)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }
            #endregion

            #region dics       
            var years = new SubFormRepository().GetPastYears(model.UserId).Where(e => e.ReportYear != model.ReportYear);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);
            
            var listanimal = IsAcceptReport() ? new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));   //(e.Id != CodeConstManager.STATUS_ACCEPT_ID)
            ViewData["statusList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.StatusId);


            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().ToList();

            ViewData["DicEvents"] = new SelectList(list2, "Id", "NameRu", 0);

            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);

            model.SubDicEnergyindicatorList = new SubDicEnergyindicator().GetAll().Where(x => x.forgu == true).ToList();
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.SEC_User1.OkedId);

            string oblastName = "";
            string ErrorMessage = new SubFormRepository().GetOblastName(MyExtensions.GetCurrentUserId().Value, model.ReportYear.Value, ref oblastName, CultureHelper.GetCurrentCulture());
            ViewData["Oblast"] = oblastName;
            #endregion

            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll();

            return View(model);
        }

        [HttpPost]
        public ActionResult DesignGuNew(SUB_FormGuNew model, IEnumerable<HttpPostedFileBase> files, bool IsSendMessage = false)
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

                #region  origin
                var history = new SUB_FormHistory
                {
                    FormId = model.Id,
                    StatusId = model.StatusId,
                    UserId = MyExtensions.GetCurrentUserId(),
                    RegDate = DateTime.Now
                };
                #endregion

                //----send notify
                if (IsSendMessage == true) //&& (model.StatusId == 4 || model.StatusId == 5 || model.StatusId == 6))
                {
                    new SendMessageManager().SendSmsToSubjectByChangeStatus(model.UserId.Value, model.ReportYear.Value, model.StatusId.Value);                 
                }

                if (!IsAcceptReport())
                {
                    model.Editor = MyExtensions.GetCurrentUserId();
                }

                //----- check isPlan
                if (model.IsPlan == null)
                    model.IsPlan = false;

                var _subForm = new SubFormRepository().GetById(model.Id);
                _subForm.StatusId = model.StatusId;
                _subForm.DesignDate = model.DesignDate;
                _subForm.DesignNote = model.DesignNote;
                _subForm.Editor = model.Editor;
                new SubFormRepository().ChangeStatusByManager(_subForm);
                new SubFormRepository().SaveHistory(history);

                //----send notify
                //	if (IsSendMessage)
                //	new SendMessageManager().SendSmsToSubjectByChangeStatus(model.UserId, model.ReportYear, model.StatusId.Value);

                var dirpath = Server.MapPath("~/uploads/subhsitory/" + history.Id);
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

                if (!string.IsNullOrEmpty(model.PreviousUrl))
                {
                    return Redirect(model.PreviousUrl);
                }
                return RedirectToAction("CommonView", "RstReport");
            }

            ViewBag.SubReadonly = true;

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
            //----form3
            if (model.SUB_Form3GuRecords == null || model.SUB_Form3GuRecords.Count == 0)
            {
                model.SUB_Form3GuRecords = new List<SUB_Form3GuRecord>();
                var item = new SUB_Form3GuRecord() { AutomateItem = 1 };
                model.SUB_Form3GuRecords.Add(item);
            }

            //----form4
            if ((model.SubForm4Records == null) || (model.SubForm4Records.Count == 0))
            {
                model.SubForm4Records = new List<SUB_Form4Record>();
                SUB_Form4Record record6 = new SUB_Form4Record();
                model.SubForm4Records.Add(record6);
            }

            if ((model.SubForm4RecordsOther == null) || (model.SubForm4RecordsOther.Count == 0))
            {
                model.SubForm4RecordsOther = new List<SUB_Form4Record>();
                SUB_Form4Record record7 = new SUB_Form4Record();
                model.SubForm4RecordsOther.Add(record7);
            }

            for (int i = 0; i < model.SubForm4Records.Count; i++)
            {
                ViewData["TypeCounters" + i] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4Records[i].TypeCounterId);
                ViewData["DicEvents" + i] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4Records[i].EventId);
                long? typeCounterId = model.SubForm4Records[i].TypeCounterId;
                long num3 = 0L;
                if ((typeCounterId.GetValueOrDefault() == num3) ? typeCounterId.HasValue : false)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].TypeCounterId", ResourceSetting.NotEmpty);
                }

                typeCounterId = model.SubForm4Records[i].EventId;
                num3 = 0L;

                if ((typeCounterId.GetValueOrDefault() == num3) ? typeCounterId.HasValue : false)
                {
                    ModelState.AddModelError("model.SubForm4Records[" + i + "].EventId", ResourceSetting.NotEmpty);
                }
            }

            for (int j = 0; j < model.SubForm4RecordsOther.Count; j++)
            {
                ViewData["TypeCountersOther" + j] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), model.SubForm4RecordsOther[j].TypeCounterId);
                ViewData["DicEventsOther" + j] = new SelectList(dicevents, "Id", "NameRu", model.SubForm4RecordsOther[j].EventId);
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

            if (model.SUB_Form3aGuRecord1sOther == null || model.SUB_Form3aGuRecord1sOther.Count == 0)
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

            #region history
            model.SUB_FormHistory = model.SUB_FormHistory.ToList();
            foreach (var rstReestrHistory in model.SUB_FormHistory)
            {
                var dir1 = Server.MapPath("~/uploads/subhsitory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files2 = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files2)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }
            #endregion

            #region dics       
            var years = new SubFormRepository().GetPastYears(model.UserId).Where(e => e.ReportYear != model.ReportYear);
            ViewData["Years"] = new SelectList(years, "Id", "ReportYear", model.Id);

            var listanimal = IsAcceptReport() ? new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID)) : new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));   //(e.Id != CodeConstManager.STATUS_ACCEPT_ID)
            ViewData["statusList"] = new SelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), model.StatusId);
            
            model.SubDicNormEnergies = new SubDicNormEnergyRepository().GetAll().ToList();
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

            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll();

            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [HttpGet]
        public ActionResult SelectApplication(string searchTerm, int pageSize, int pageNum)
        {

            var founder = new SubFormRepository().GetTerm(searchTerm);

            Select2PagedResult pagedAttendees = AttendeesToSelect2Format(founder, founder.Count);

            return new JsonpResult
            {
                Data = pagedAttendees,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        [HttpGet]
        public ActionResult Edit(long id, string returnUrl, string activeTab)
        {
            var repository = new SubFormRepository();
            var model = repository.GetById(id);
            model.ActiveTab = activeTab;
            model.PreviousUrl = returnUrl;
            model.AttachFiles = new List<string>();

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
            FillViewBag(model);
            FillHistory(model);

            model.SubDicTypeResources = new SubDicTypeResourceRepository().GetAll(); 
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


         
            return View("Create", model);
        }
        [HttpPost]
        public ActionResult FileUpload(long id, IEnumerable<HttpPostedFileBase> files)
        {
            string path = Server.MapPath("~/uploads/appform/" + id + "/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var repository = new SubFormRepository();
            var preamble = repository.GetById(id);


            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(Path.Combine(path, file.FileName));
                }
            }
            repository.Update(preamble);
            var eauditModel = repository.GetById(id);
            return RedirectToAction("Edit", "AppForm", new { id = id });
        }
   /*     public FileResult FileDownload(long id, long refPreamble)
        {
            string path = Server.MapPath("~/uploads/appform/" + refPreamble + "/");
            var repository = new SubFormRepository();
            var preamble = repository.GetById(refPreamble);

            var fileInfo = preamble.EAUDIT_AttachedFiles.FirstOrDefault(af => af.Id == id);
            if (fileInfo == null)
            {
                return null;
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(path, fileInfo.FileName));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileInfo.FileName);
        }*/
        private static Select2PagedResult AttendeesToSelect2Format(IEnumerable<TermSearch> attendees, int totalAttendees)
        {
            var jsonAttendees = new Select2PagedResult { Results = new List<Select2Result>() };

            foreach (var a in attendees)
            {
                jsonAttendees.Results.Add(new Select2Result { id = a.Id.ToString(CultureInfo.InvariantCulture), text = a.Term });
            }
            jsonAttendees.Total = totalAttendees;

            return jsonAttendees;
        }
        public ActionResult GetFileUploader(long preambleId)
        {
            var auditRepository = new SubFormRepository();
            var model = auditRepository.GetById(preambleId);
            if (model == null)
            {
                return null;
            }
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
            return PartialView("_UploadFilesView", model);
        }

        public JsonResult FileRemove(long id, string filename)
        {
            string path = Server.MapPath("~/uploads/appform/" + id + "/");
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');

                    var name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    var exist = name == filename;
                    if (exist) continue;
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }


            }

            return Json(new
            {
                IsSuccess = true,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadFile(string id, string filename)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            var dir = Server.MapPath("~/uploads/appform/" + id);
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
      
        public ActionResult Create(int? year, long? userId)
        {
            var reportYear = year;
            if (reportYear == null)
            {
                reportYear = DateTime.Now.Year - 1;
            }

            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();
            var model = new SUB_Form
            {
                StatusId = 1,
                Editor = currentUserId,
                ReportYear = reportYear.Value,
                BeginPlanYear = reportYear.Value,
                EndPlanYear = reportYear.Value+4,
                UserId = userId
            };
            if (userId != null)
            {
                model.SEC_User1 = new SecUserRepository().GetById(userId.Value);
            }
            var dics = new SubDicTypeResourceRepository().GetCollectionList().Where(e =>e.Code!=null && e.Code.Contains("2")).OrderBy(e => e.PosIndex);
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
            FillViewBag(model);
            if (Request.UrlReferrer != null) model.PreviousUrl = Request.UrlReferrer.ToString();
            return View(model);
        }
        [HttpPost]
        public override ActionResult UpdateModel(string code, long modelId, long userId, long editorId, long recordId, int year, string fieldName, string fieldValue, long typeId)
        {
            var filter = new SubFormRepository().UpdateModel(code, modelId, userId, recordId, year, fieldName, fieldValue, editorId, typeId, status: CodeConstManager.STATUS_WORK_ID);
            if (filter != null && filter.IsNew)
            {
                new SubFormRepository().RegistredInReport(filter.ModelId, year, userId);
            }
            return Json(new { Success = true, formId = filter.ModelId, fromRecordId = filter.RecordId, unique = filter.Unique });
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
            var dir = Server.MapPath("~/uploads/subhsitory/" + path);
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
        [HttpPost]
        public  ActionResult SendApplication(long id)
        {
          var repository = new SubFormRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return Json(new { Success = false });
            }
            model.IsBack = true;
            model.DesignDate = DateTime.Now;
            model.StatusId = CodeConstManager.STATUS_ACCEPT_ID;
            var history = new SUB_FormHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                FormId = model.Id,
                StatusId = model.StatusId,
                UserId = MyExtensions.GetCurrentUserId()
            };
            model.SUB_FormHistory.Add(history);
            new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SendMessageManager().SendSubForm(model);
            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult BackSend(long id)
        {
            var repository = new SubFormRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.IsBack = true;
            model.DesignDate = DateTime.Now;
            var history = new SUB_FormHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                FormId = model.Id,
                StatusId = model.StatusId,
                UserId = MyExtensions.GetCurrentUserId()
            };
            model.SUB_FormHistory.Add(history);
            new SubFormRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SendMessageManager().SendSubForm(model);
            return RedirectToAction("Index");
        }
      
     
   
    }
}
