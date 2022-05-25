using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Subject
{
    public abstract class AGestController : ACommonController
    {
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
        }

 
        [HttpPost]
        public virtual ActionResult CheckBin(string bin)
        {

            var user = new AccountRepository().GetUserByBin(bin);
            var isNew = user == null;
            return Json(new { Success = isNew });

        }
    }
}