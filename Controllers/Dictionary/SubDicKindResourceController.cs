using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;

namespace Aisger.Controllers.Dictionary
{

    public class SubDicKindResourceController : ACommonController
    {

        public ActionResult Index()
        {
            var repository = new SubDicKindResourceRepository();
            return View(repository.GetCollectionList());
        }
        public ActionResult Delete(long id)
        {
            new SubDicKindResourceRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new SUB_DIC_KindResource();
            FillViewBag(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SUB_DIC_KindResource model)
        {
            if (ModelState.IsValid)
            {
                var repository = new SubDicKindResourceRepository();
                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            FillViewBag(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var model = new SubDicKindResourceRepository().GetById(id);
            FillViewBag(model);
            return View("Create", model);
        }
        [AcceptVerbs(HttpVerbs.Get)]

        private void FillViewBag(SUB_DIC_KindResource model)
        {

            ViewData["Units"] = new SelectList(new DicUnitRepository().GetAll(), "Id",
                                                   "NameRu", model.UnitId);

        }

    }
}
