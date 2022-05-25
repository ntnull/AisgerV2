using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Dictionary
{
    public class DicGuController : Controller
    {
        // GET: DicGu
        private readonly DicGURepository _repo;
        public DicGuController()
        {
            _repo = new DicGURepository();
        }
        public ActionResult Index()
        {
            var rows = _repo.GetDicGuList();
            return View(rows);
        }

        public ActionResult Create()
        {
            var row = new DIC_GU();
            row.IsDeleted = false;
            return View(row);
        }

        [HttpPost]
        public ActionResult Create(DIC_GU model)
        {
            var errorMessage = _repo.SaveDicGu(model);
            if (errorMessage == "")
            {
                return Redirect("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var row = _repo.GetById(id);
            return View("Create",row);
        }

        [HttpPost]
        public ActionResult Edit(DIC_GU model)
        {
            var errorMessage = _repo.EditDicGu(model);
            if (errorMessage == "")
            {
                return Redirect("/DicGu/Index");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            _repo.DeleteById(id);
            return Redirect("/DicGu/Index");
        }
    }
}