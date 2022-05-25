using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Dictionary
{

    public class SubDicTypeResourceController : ACommonController
    {

        public ActionResult Index()
        {
            var repository = new SubDicTypeResourceRepository();
            return View(repository.GetCollectionList());
        }
        public ActionResult Delete(long id)
        {
            new SubDicTypeResourceRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new SUB_DIC_TypeResource();
            FillViewBag(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SUB_DIC_TypeResource model)
        {
            if (ModelState.IsValid)
            {
                var repository = new SubDicTypeResourceRepository();
                if (model.Id == 0)
                {
                    repository.SaveObject(model);
                }
                else
                {
//                model.Id = new SubDicTypeResourceRepository().GetAll().Max(e => e.Id) + 1;
                    repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                }
                return RedirectToAction("Index");
            }
            FillViewBag(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var model = new SubDicTypeResourceRepository().GetById(id);
            FillViewBag(model);
            return View("Create", model);
        }
        [AcceptVerbs(HttpVerbs.Get)]

        private void FillViewBag(SUB_DIC_TypeResource model)
        {

            ViewData["Units"] = new SelectList(new DicUnitRepository().GetAll(), "Id",
                                                   "NameRu", model.UnitId);

        }

    }
}
