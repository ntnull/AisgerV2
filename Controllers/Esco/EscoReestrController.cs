using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Controllers.Subject;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

namespace Aisger.Controllers.Esco
{
    public class EscoReestrController : AGestController
    {
        //
        // GET: /EscoReestr/

        public ActionResult Index()
        {
            var list = new SecUserRepository().GetListByKindId(CodeConstManager.KIND_USER_ESCO);
            return View(list);
        }
        [HttpGet]
        public ActionResult Create(string bin)
        {
            var user = new AccountRepository().GetUserByBin(bin);
            var model = new AccountController().SecGuest(user,CodeConstManager.CODE_USER_ESCO);
            
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
                model.Kinds.Add(CodeConstManager.KIND_USER_ESCO.ToString(CultureInfo.InvariantCulture));
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
        public ActionResult Edit(long id)
        {
            SEC_User user = new SecUserRepository().GetById(id);
            var model = new AccountController().SecGuest(user,CodeConstManager.CODE_USER_ESCO);
//            model.Kinds = user.SEC_UserKind.Select(aquticOblast => aquticOblast.KindId.ToString()).ToList();
            FillBagRegistrationGuest(model);
            return View("Create", model);
        }

      
    }
}
