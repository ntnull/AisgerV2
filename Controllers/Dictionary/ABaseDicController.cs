#region

using System;
using System.Collections.Generic;
using System.Resources;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository;

#endregion

namespace Aisger.Controllers.Dictionary
{
    public class ABaseDicController<T, TM> : ACommonController
        where T : class, IBaseDictionary, new()
        where TM : AObjectRepository<T>, new()
    {
        protected virtual string GetCodeView
        {
            get { return new T().ToString().Replace("Aisger.Models.", ""); }
        }


        public virtual ActionResult Index()
        {
            var list = new TM().GetAll();
            return ShowGrid(list);
        }
        public ActionResult IndexGrid()
        {
            var list = new TM().GetAll();
            return PartialView("../Dictionary/GridDictionary", list);
          /*  return PartialView("_IndexGrid",
                PeopleRepository.GetPeople());*/
        }
        [HttpGet]
        public virtual ActionResult Create()
        {
            IBaseDictionary environmental = new T();
            return ShowEdit(environmental);
        }
    /*    [HttpGet]
        public virtual ActionResult ExportExcel()
        {
            var list = new TM().GetAll();
//            return new ExportExcelManager<T>(list, null, null); 
        }*/

        [HttpGet]
        public virtual ActionResult Edit(long id)
        {
            IBaseDictionary environmental = new TM().GetById(id);
            return ShowEdit(environmental);
        }

        public virtual ActionResult Delete(long id)
        {
            new TM().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        protected ActionResult ShowGrid(IEnumerable<T> posts)
        {
            ViewBag.DATA_CODE = GetCodeView;
            if (GetCodeView.Equals("DIC_Unit"))
            {
                ViewBag.Title = ResourceSetting.DicUnit;
            }
            else
            {
                ViewBag.Title = new ResourceManager(typeof(ResourceSetting)).GetString(GetCodeView);
            }

            ICollection<T> is2 = posts as ICollection<T>;
            ViewBag.Count = is2.Count;
            return View("../Dictionary/GridDictionary", posts);
        }

        protected ActionResult ShowEdit(IBaseDictionary environmental)
        {
            ViewBag.DATA_CODE = GetCodeView;
            ViewBag.Title = new ResourceManager(typeof(ResourceSetting)).GetString(GetCodeView);
            return View("../Dictionary/EditDictionary", environmental);
        }

        protected ActionResult GetPostHolder(IBaseDictionary environmental)
        {
            if (ModelState.IsValid)
            {
                environmental.CreateDate = DateTime.Now;
                new TM().SaveOrUpdate((T)environmental, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return ShowEdit(environmental);
        }
    }
}