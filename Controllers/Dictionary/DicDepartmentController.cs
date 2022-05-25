using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;

namespace Aisger.Controllers.Dictionary
{
   public class DicDepartmentController : ACommonController
    {
        public ActionResult Index()
        {
            var repository = new DicDepartmentRepository();
            return View(repository.GetAll());
        }
        public ActionResult Delete(long id)
        {
            new DicDepartmentRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new DIC_Department();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DIC_Department model)
        {
            if (ModelState.IsValid)
            {

                var repository = new DicDepartmentRepository();
                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var model = new DicDepartmentRepository().GetById(id);
            return View("Create", model);
        }
      
    }
}
