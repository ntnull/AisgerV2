using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aisger.Controllers.Security;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Map;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;

namespace Aisger.Controllers.Subject
{
    public class SubjectInfoController : ACommonController
    {
        // GET: /SubjectInfo/
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MyExtensions.GetRolesId() == 4)
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }
            //base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(long id, int year)
        {
            var model = new SubjectInfo();
            var user = new SecUserRepository().GetById(id);
            if (user.IsGuest)
            {             
                model.SecUser = new SEC_Guest();
                var bufferRstRR = new SubFormRepository().GetRstReportReestrByUserId(user.Id, year);
                if (bufferRstRR != null)
                {
                    model.SecUser.Id = user.Id;
                    model.SecUser.ger_wo_ecp = (user.ger_wo_ecp != null)? user.ger_wo_ecp.Value : false;
                    model.SecUser.BINIIN = user.BINIIN;
                    model.SecUser.IsHaveGES = user.IsHaveGES;
                    model.SecUser.IDK = user.IDK;
                    model.SecUser.LastName = bufferRstRR.usrlastname;
                    model.SecUser.SecondName = bufferRstRR.usrsecondname;
                    model.SecUser.FirstName = bufferRstRR.usrfirstname;
                    model.SecUser.JuridicalName = bufferRstRR.usrjuridicalname;
                    model.SecUser.Post = bufferRstRR.usrpost;
                    model.SecUser.Mobile = bufferRstRR.usrmobile;
                    model.SecUser.WorkPhone = bufferRstRR.usrworkphone;
                    model.SecUser.InternalPhone = bufferRstRR.usrinternalphone;
                    model.SecUser.Address = bufferRstRR.usraddress;

                    model.SecUser.IsCvazy = (bufferRstRR.usriscvazy != null) ? Convert.ToBoolean(bufferRstRR.usriscvazy) : false;
                    model.SecUser.ResponceFIO = bufferRstRR.usrresponcefio;
                    model.SecUser.ResponcePost = bufferRstRR.usrresponcepost;

                    if (bufferRstRR.usroblast != null)
                    {
                        model.SecUser.Oblast = bufferRstRR.usroblast.Value;
                    }

                    if (bufferRstRR.usrregion != null)
                    {
                        model.SecUser.Region = bufferRstRR.usrregion.Value;
                    }

                    model.SecUser.SubRegion = bufferRstRR.usrsubregion;
                    model.SecUser.Village = bufferRstRR.usrvillage;
                    model.SecUser.TypeApplicationId = Convert.ToInt64(bufferRstRR.usrtypeapplicationid);
                    model.SecUser.OkedId = bufferRstRR.usrokedid;
                    model.SecUser.FSCode = bufferRstRR.usrfscode;
                    model.SecUser.IDK = bufferRstRR.usridk;
                    model.SecUser.Email = bufferRstRR.usremail;

                    model.SecUser.JuridicalKato =bufferRstRR.usraddress;
                    model.SecUser.FactKato = bufferRstRR.usraddress;
                }
                else
                {
                    model.SecUser = new PrivateSettingController().GetGestInfo(user);
                    model.SecUser.JuridicalKato = GetKatoString(user);
                    model.SecUser.FactKato = GetFactKatoString(user);
                }

                FillGuestViewBag(model.SecUser);

                foreach (var propertyInfo in typeof(SEC_Guest).GetProperties())
                {
                    object[] attrs = propertyInfo.GetCustomAttributes(true);
                    var value = propertyInfo.GetValue(model.SecUser);
                    foreach (object attr in attrs)
                    {
                        var requiredAttribute = attr as RequiredAttribute;
                        if (requiredAttribute != null)
                        {
                            string propName = propertyInfo.Name;
                            var msg = ResourceSetting.RequiredToFill;
                            if (value == null)
                                ModelState.AddModelError(propName, msg);
                        }
                    }
                }
            }

            model.SubForms = new SubFormRepository().GetListCurrentByUser(id);
            model.RstReportReestrs = new RstReportRepository().GetRstReportReestrsByUserId(id);
            model.RstReestrHistories = new RstReestrRepository().GetReestrReportHistoryByUserId(id);
            model.MapApplications =
                new MapApplicationRepository().GetListCurrentByUser(id).Where(e => e.SendDate != null).ToList();
            foreach (var rstReestrHistory in model.RstReestrHistories)
            {
                if (rstReestrHistory.RST_Application != null)
                {
                    var dir1 = Server.MapPath("~/uploads/application/" + rstReestrHistory.RST_Application.Id + "/");
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

            return View(model);
        }

        public string GetKatoString(SEC_User guest)
        {
            var builder = new StringBuilder();
            if (guest.DIC_Kato != null)
            {
                builder.Append(guest.DIC_Kato.NameRu);
            }
            if (guest.DIC_Kato1 != null)
            {
                builder.Append(", ").Append(guest.DIC_Kato1.NameRu);
            }
            if (guest.DIC_Kato2 != null)
            {
                builder.Append(", ").Append(guest.DIC_Kato2.NameRu);
            }
            if (guest.DIC_Kato3 != null)
            {
                builder.Append(", ").Append(guest.DIC_Kato3.NameRu);
            }
            return builder.ToString();
        }

        public string GetFactKatoString(SEC_User guest)
        {
            var builder = new StringBuilder();
            var repository = new KatoRepository();
            var oblast = repository.GetById(guest.FactOblast);
            var region = repository.GetById(guest.FactRegion);
            var subregion = repository.GetById(guest.FactSubRegion);
            var village = repository.GetById(guest.FactVillage);

            if (oblast != null)
            {
                builder.Append(oblast.NameRu);
            }
            if (region != null)
            {
                builder.Append(", ").Append(region.NameRu);
            }
            if (subregion != null)
            {
                builder.Append(", ").Append(subregion.NameRu);
            }
            if (village != null)
            {
                builder.Append(", ").Append(village.NameRu);
            }
            return builder.ToString();
        }

        private void FillGuestViewBag(SEC_Guest model)
        {

            model.WastList = new AccountController().GetPlants(model.Wastes);
            model.KindList = new AccountController().GetKinds(model.Kinds);
            ViewData["TypeApplicationList"] = new SelectList(new DicTypeApplicationRepository().GetAll(), "Id", "ShortName" + CultureHelper.GetCurrentCulture() , model.TypeApplicationId);
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

        //----sign report without ecp
		public ActionResult SaveSubjectSettingsByManager(long userId, bool ger_wo_ecp)
		{
			string query = "UPDATE \"SEC_User\" SET ger_wo_ecp=" + ger_wo_ecp + " WHERE \"Id\"=" + userId;
			var ErrorMessage = new SecUserRepository().ExecuteSqlCommand(query);
			Dictionary<string, object> dict = new Dictionary<string, object>();
			dict["ErrorMessage"] = ErrorMessage;
			return Json(dict);
		}

        //----change user fscode
        public ActionResult ChangeSubjectFSCodeSettingsByManager(long userId, int? fscode, int? reportYear)
        {
            if (fscode == 0)
                fscode = 1;

            string query = " update \"SEC_User\" SET \"FSCode\"=" + fscode + "  WHERE \"Id\"=" + userId + ";"
                       + " update \"RST_ReportReestr\" set usrfscode=" + fscode + " where \"UserId\"=" + userId + ""
                       + " and \"ReportId\" in (select \"Id\" from \"RST_Report\" where \"ReportYear\"=" + reportYear+ ");";
            
            var ErrorMessage = new SecUserRepository().ExecuteSqlCommand(query);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["ErrorMessage"] = ErrorMessage;
            return Json(dict);
        }

        //----change user idk
        public ActionResult ChangeSubjectIDKSettingsByManager(long userId,int? reportYear,string idk)
        {
            string query = " update \"SEC_User\" SET \"IDK\"='" + idk + "'  WHERE \"Id\"=" + userId + ";"
                       + " update \"RST_ReportReestr\" set \"IDK\"='"+idk+ "' , usridk='"+idk+"' where \"UserId\"=" + userId + ""
                       + " and \"ReportId\" in (select \"Id\" from \"RST_Report\" where \"ReportYear\"=" + reportYear + ");";

            var ErrorMessage = new SecUserRepository().ExecuteSqlCommand(query);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["ErrorMessage"] = ErrorMessage;
            return Json(dict);
        }
    }
}
