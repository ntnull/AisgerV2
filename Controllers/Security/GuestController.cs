using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

namespace Aisger.Controllers.Security
{
    public class GuestController : ACommonController
    {
    
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var list = new SecUserRepository().GetAll().Where(e => e.IsGuest);
            return View(list);
        }
        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Create()
        {
            var model = new SEC_Guest { TypeApplicationId = 1, Pwd = CodeConstManager.DEFAULT_PWD, ConfirmPwd = CodeConstManager.DEFAULT_PWD };
            FillBagRegistrationGuest(model);
            return View(model);
        }
        public void FillBagRegistrationGuest(SEC_Guest model)
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
        }
        [HttpPost]
        public ActionResult Create(SEC_Guest model)
        {
            ModelState.Remove("Pwd");
            ModelState.Remove("ConfirmPwd");
            ModelState.Remove("ResponcePost");
            ModelState.Remove("ResponceFIO");

            FillBagRegistrationGuest(model);
            var user = new SecUserRepository().GetAll().SingleOrDefault(e => e.Login == model.BINIIN && e.Id != model.Id);
            if (user != null && model.Id != user.Id)
            {
                model.IsError = true;
                model.ErrorMessage = "С данным ИИН или БИН пользователь уже зарегистрирован, обратитесь к администратору";
                return View(model);
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
                repository.RegisteredUser(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(model);

        }
        public ActionResult Delete(long id)
        {
            new SecUserRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            SEC_User user = new SecUserRepository().GetById(id);
            var model = SecGuest(user);
            FillBagRegistrationGuest(model);
            return View("Create", model);
        }

        public SEC_Guest SecGuest(SEC_User user)
        {
            var model = new SEC_Guest
            {
                Id = user.Id,
                Address = user.Address,
                FactAddress = user.FactAddress,
                Certificate = user.Certificate,
                BINIIN = user.BINIIN,
                FirstName = user.FirstName,
                Email = user.Email,
                InternalPhone = user.InternalPhone,
                IsCvazy = user.IsCvazy,
                IsHaveGES = user.IsHaveGES,
                JuridicalName = user.JuridicalName,
                LastName = user.LastName,
                Mobile = user.Mobile,
                SecondName = user.SecondName,
                ResponceFIO = user.ResponceFIO,
                ResponcePost = user.ResponcePost,
                Post = user.Post,
                WorkPhone = user.WorkPhone,
                OkedId = user.OkedId,
                Wastes = user.SEC_UserOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList(),
                Kinds = user.SEC_UserKind.Select(aquticOblast => aquticOblast.KindId.ToString()).ToList(),
                TypeApplicationId = user.TypeApplicationId,
                ger_wo_ecp = (user.ger_wo_ecp == null) ? false : user.ger_wo_ecp.Value,
                FSCode = user.FSCode
            };          

            if (user.Oblast != null)
            {
                model.Oblast = user.Oblast.Value;
            }
            if (user.Region != null)
            {
                model.Region = user.Region.Value;
            }

            model.SubRegion = user.SubRegion;
            model.Village = user.Village;
            model.FactAddress = user.FactAddress;
            model.FactOblast = user.FactOblast;
            model.FactRegion = user.FactRegion;
            model.FactSubRegion = user.FactSubRegion;
            model.FactVillage = user.FactVillage;
            model.JuridicalKato = GetKatoString(user);
            model.FactKato = GetFactKatoString(user);
            return model;
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

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult ChangePwd(long id)
        {
            SEC_User model = new SecUserRepository().GetById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePwd(SEC_User model)
        {
            if (model.Id == 0 && model.Pwd != model.ConfirmPwd)
            {
                model.IsConfirm = true;
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var pwd = RegJurnalManager.Instance.Encrypt(model.Pwd);
                new SecUserRepository().ChangePwd(model.Id, pwd);
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
}
