using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using Npgsql;

namespace Aisger.Controllers.Subject
{
    public class SubjectPersonController : ACommonController
    {
        //
        // GET: /SubjectPerson/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var list = new SecUserRepository().GetListByKindId(CodeConstManager.KIND_USER_SUBJECT);
            return View(list);
        }
        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Create()
        {
            var model = new SEC_Guest { TypeApplicationId = 1, Pwd = CodeConstManager.DEFAULT_PWD, ConfirmPwd = CodeConstManager.DEFAULT_PWD};
            FillViewBag(model);
            return View(model);
        }
       
        private void FillViewBag(SEC_Guest model)
        {
            model.WastList = new AccountController().GetPlants(model.Wastes);
            ViewData["TypeApplicationList"] = new SelectList(new DicTypeApplicationRepository().GetAll(), "Id", "ShortNameRu", model.TypeApplicationId);
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.OkedId);
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            ViewData["OblastList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.Oblast);

            ViewData["RegionList"] = new SelectList(repository.GetKatos(model.Oblast, true), "Id",
                                                   "NameRu", model.Region);

            ViewData["SubRegionList"] = new SelectList(repository.GetKatos(model.Region, false), "Id",
                                                   "NameRu", model.SubRegion);

            ViewData["VillageList"] = new SelectList(repository.GetKatos(model.SubRegion, false), "Id",
                                                   "NameRu", model.Village);

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetKatos(long parentId, bool mandatory)
        {
            var founder = new KatoRepository().GetKatos(parentId, mandatory).Select(q => new
            {
                q.Id,
                q.NameRu
            });
            return Json(founder.ToArray(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Create(SEC_Guest model)
        {
            ModelState.Remove("Pwd");
            ModelState.Remove("ConfirmPwd");
            ModelState.Remove("OkedId");
            ModelState.Remove("ResponcePost");
            ModelState.Remove("ResponceFIO");
            FillViewBag(model);
            var user = new SecUserRepository().GetAll().SingleOrDefault(e => e.Login == model.BINIIN && e.Id!=model.Id);
            if (user != null && model.Id!=user.Id)
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
                model.Kinds = new List<string>();
                model.Kinds.Add("1");
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
            var model = new SEC_Guest
            {
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
                TypeApplicationId = user.TypeApplicationId

            };

            model.TypeApplicationId = user.TypeApplicationId;
          
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
            FillViewBag(model);
            return View("Create", model);
        }
    }
}
