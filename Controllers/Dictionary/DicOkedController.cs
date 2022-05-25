using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;

namespace Aisger.Controllers.Dictionary
{
    public class DicOkedController : ACommonController
    {
        public ActionResult Index()
        {
            var repository = new DicOkedRepository();
            var organization = repository.GetAll().Where(o => o.refParent == null).ToList();
            return View(organization);
        }

        [HttpGet]
        public ActionResult ChooseKato(string id)
        {
            long parentId;
            if (long.TryParse(id, out parentId))
            {
                var organization = new DIC_OKED();
                organization.Id = parentId;
                return View(organization);
            }
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public virtual ActionResult Create(long id)
        {
            var ecosystem = new DIC_OKED();

            long idParent = id;
            var builder = new StringBuilder();

            var repository = new DicOkedRepository();
            var parent = repository.GetById(idParent);
            if (parent != null)
            {
                builder.Append("[" + parent.Code+"] - ").Append(parent.NameRu);
            }
            ecosystem.ParentName = builder.ToString();
            ecosystem.refParent = idParent;
            return View(ecosystem);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(DIC_OKED ecosystem)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var repository = new DicOkedRepository();

                var eco = repository.GetAll().SingleOrDefault(e => e.Code == ecosystem.Code);
                if (eco != null)
                {
                    ecosystem.IsCodeIncorect = true;
                    ModelState.AddModelError("ExistNumber", ResourceSetting.existCode);

                    return View(ecosystem);
                }
                ecosystem.Id = 0;
                ecosystem.NameKz = ecosystem.NameRu;
                repository.SaveOrUpdate(ecosystem, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(ecosystem);
        }

        [HttpGet]
        public virtual ActionResult Edit(long id)
        {
            var ecosystem = new DicOkedRepository().GetById(id);
            var builder = new StringBuilder();

            var repository = new DicOkedRepository();
            if (ecosystem.refParent!=null)
            {
             var parent = repository.GetById(ecosystem.refParent.Value);

             builder.Append("[" + parent.Code + "] - ").Append(parent.NameRu);
            }
            ecosystem.ParentName = builder.ToString();
            return View(ecosystem);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(DIC_OKED ecosystem)
        {

            if (ModelState.IsValid)
            {
                var repository = new DicOkedRepository();

                var eco = repository.GetAll().SingleOrDefault(e => e.Code == ecosystem.Code && e.Id != ecosystem.Id);
                if (eco != null)
                {
                    ecosystem.IsCodeIncorect = true;
                    ModelState.AddModelError("ExistNumber", ResourceSetting.existCode);

                    return View(ecosystem);
                }
                ecosystem.NameKz = ecosystem.NameRu;
                repository.SaveOrUpdate(ecosystem, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(ecosystem);
        }

        public virtual ActionResult Delete(long id)
        {
            new DicOkedRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void SetSelectId(string id)
        {
            long idParent;
            if (long.TryParse(id, out idParent))
            {
            }
            //CodeConstManager.SelectParentId = idParent;
        }

        [HttpPost]
        public JsonResult ShowDetails(long id)
        {
            var dicOrganization = new DicOkedRepository().GetById(id);
            if (dicOrganization == null)
                return Json(new { Status = 0, Message = "Not found" });

            return
                Json(
                    new
                    {
                        Status = 1,
                        Message = "Ok",
                        Content = RenderPartialViewToString("ShowDetails", dicOrganization)
                    });
        }
    }
}