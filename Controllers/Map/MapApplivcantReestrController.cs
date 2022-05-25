using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Controllers.Subject;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

namespace Aisger.Controllers.Map
{
    public class MapApplivcantReestrController : AGestController
    {
        //
        // GET: /EscoReestr/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var list = new SecUserRepository().GetListByKindId(CodeConstManager.KIND_USER_MAP);
            return View(list);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Create(string bin)
        {
            var user = new AccountRepository().GetUserByBin(bin);
            var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_APP);

            FillBagRegistrationGuest(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(SEC_Guest model)
        {
            RemoveManadatoryFields();
            FillBagRegistrationGuest(model);
            /*  var user = new SecUserRepository().GetAll().SingleOrDefault(e => e.Login == model.BINIIN && e.Id != model.Id);
              if (user != null && model.Id != user.Id)
              {
                  model.IsError = true;
                  model.ErrorMessage = "С данным ИИН или БИН пользователь уже зарегистрирован, обратитесь к администратору";
                  return View(model);
              }*/
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
                model.Kinds.Add(CodeConstManager.KIND_USER_MAP.ToString(CultureInfo.InvariantCulture));
                repository.RegisteredUser(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(model);

        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            SEC_User user = new SecUserRepository().GetById(id);
            var model = new AccountController().SecGuest(user, CodeConstManager.CODE_USER_APP);
            //            model.Kinds = user.SEC_UserKind.Select(aquticOblast => aquticOblast.KindId.ToString()).ToList();
            FillBagRegistrationGuest(model);
            return View("Create", model);
        }

    }
}
