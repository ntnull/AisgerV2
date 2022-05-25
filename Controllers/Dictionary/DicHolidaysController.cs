using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using FlowDoc.Models.Repository.Dictionary;

namespace Aisger.Controllers.Dictionary
{
    public class DicHolidaysController : ACommonController
    {
        //
        // GET: /DicHolidays/

        public ActionResult Index()
        {
            return View(new DicHolidaysRepository().GetList());
        }
        [HttpGet]
        public ActionResult Copy(int id)
        {
            var repository = new DicHolidaysRepository();
            var entity = repository.GetByYear(id);
            var model = new DicHolidayEntity { Year = repository.CreateReportYear(), DicHolidayses = entity.DicHolidayses, DicWorkes = entity.DicWorkes };
            return View("Create", model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = new DicHolidaysRepository().GetByYear(id);
            return View("Create", model);
        }

        public ActionResult Delete(long id)
        {
            new DicHolidaysRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }
         [HttpGet]
        public ActionResult Create()
        {
            var repository = new DicHolidaysRepository();
            var list = new List<DIC_Holidays>();
            list.Add(new DIC_Holidays());
            var model = new DicHolidayEntity { Year = repository.CreateReportYear(), DicHolidayses = list ,DicWorkes = list};
            return View(model);
        }
         [HttpPost]
         public ActionResult Create(DicHolidayEntity model)
         {
             if (ModelState.IsValid)
             {
                 var repository = new DicHolidaysRepository();
                 repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                 return RedirectToAction("Index");
             }
             return View(model);
         }
         [HttpPost]
         public virtual ActionResult GetInfoReportYear(int year)
         {
             var status = new DicHolidaysRepository().GetInfoReportYear(year);
             return Json(new { Success = status });

         }
    }
}
