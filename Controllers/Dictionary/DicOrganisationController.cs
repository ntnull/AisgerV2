using System.Linq;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using FlowDoc.Models.Repository.Dictionary;

namespace Aisger.Controllers.Dictionary
{
    public class DicOrganisationController : ACommonController
    {
        public ActionResult Index()
        {
            var repository = new DicOrganisationRepository();
            return View(repository.GetAll());
        }
        public ActionResult Delete(long id)
        {
            new DicOrganisationRepository().Delete(id,MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new DIC_Organization();
            FillViewBag(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DIC_Organization model)
        {
            if (ModelState.IsValid)
            {
                if (model.Region == 0)
                {
                    model.Region = null;
                }
                if (model.Village == 0)
                {
                    model.Village = null;
                }
                if (model.SubRegion == 0)
                {
                    model.SubRegion = null;
                }
                var repository = new DicOrganisationRepository();
                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            FillViewBag(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var model = new DicOrganisationRepository().GetById(id);
            FillViewBag(model);
            return View("Create", model);
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
        private void FillViewBag(DIC_Organization model)
        {
            var listanimal = new KatoRepository().GetKatos(1, true);
            ViewData["OblastList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.Oblast);

            ViewData["RegionList"] = new SelectList(new KatoRepository().GetKatos(model.Oblast, true), "Id",
                                                   "NameRu", model.Region);

            ViewData["SubRegionList"] = new SelectList(new KatoRepository().GetKatos(model.Region, false), "Id",
                                                   "NameRu", model.SubRegion);

            ViewData["VillageList"] = new SelectList(new KatoRepository().GetKatos(model.SubRegion, false), "Id",
                                                   "NameRu", model.Village);

        }
    }
}
