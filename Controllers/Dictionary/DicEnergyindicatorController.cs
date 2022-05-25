using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Dictionary
{
    public class DicEnergyindicatorController : Controller
    {
        private readonly SubDicEnergyindicatorRepository repo;
        public DicEnergyindicatorController()
        {
            repo = new SubDicEnergyindicatorRepository();
        }
        // GET: DicEnergyindicator
        public ActionResult Index()
        {
            var model = repo.GetSubDicEnergyindicatorList();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = repo.GetSubDicEnergyindicatorById(id);
            return View("Create", model);
        }

        public ActionResult Create()
        {
            var model = new sub_dic_energyindicator();
            model.forgu = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(sub_dic_energyindicator model)
        {
            var errorMessage = new SubDicEnergyindicatorRepository().SaveSubDicEnergyindicator(model);
            if (errorMessage == "")
            {
                return Redirect("Index");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var errorMessage = repo.DeleteSubDicEnergyindicatorById(id);
            if (errorMessage == "")
            {
                return RedirectToAction("/Index");
            }

            return Redirect("/Index");
        }
    }
}