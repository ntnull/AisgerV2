using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Utils;

namespace Aisger.Controllers.Action
{
    public abstract class AActionController : ACommonController
    {
        protected void FillHistory(SUB_ActionPlan model)
        {
            foreach (var rstReestrHistory in model.SUB_ActionHistory)
            {
                var dir1 = Server.MapPath("~/uploads/actionhsitory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }
        }
        public virtual void FillViewBag(SUB_ActionPlan model)
        {
            var repository = new SubDicStatusRepository();
            var listanimal = repository.GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));
            ViewData["statusList"] = new SelectList(listanimal, "Id",
                                                 CultureHelper.GetDictionaryName("NameRu"), model.StatusId);
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            var dics = new SubDicTypeResourceRepository().GetAll();
            var wrapResource = new List<SUB_DIC_TypeResource>();
            foreach (var subDicTypeResource in dics)
            {
                if (subDicTypeResource.DIC_Unit != null)
                {
                    subDicTypeResource.NameRu += " (" + subDicTypeResource.DIC_Unit.NameRu + ")";
                }
                wrapResource.Add(subDicTypeResource);
            }
            //            model.SubDicTypeResources = dics;
            ViewData["Types"] = new SelectList(wrapResource, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);


            var typeCounter = new SubDucTypeCounterRepository().GetAll();
            ViewData["TypeCounters"] = new SelectList(typeCounter, "Id", CultureHelper.GetDictionaryName("NameRu"), 0);


            model.SubDicKindTabOnes = new SubDicKindTabOneRepository().GetAll();
            model.SubFormTab1s = new List<SUB_ActionTab1>();
            foreach (var tabOne in model.SubDicKindTabOnes)
            {
                var list = model.SUB_ActionTab1.Where(e => e.KindId == tabOne.Id).ToList();
                var codes = new List<string>();
                for (var t = 1; t < 3; t++)
                {
                    var codeIndex = tabOne.IndexCode + ".0" + t;
                    codes.Add(codeIndex);
                    if (list.Any(e => e.Code == codeIndex))
                    {
                        model.SubFormTab1s.Add(list.First(e => e.Code == codeIndex));
                    }
                    else
                    {
                        model.SubFormTab1s.Add(new SUB_ActionTab1() { Code = codeIndex, KindId = tabOne.Id });
                    }
                }
                var notIn = list.Where(e => !codes.Contains(e.Code));
                foreach (var subFormTab1 in notIn)
                {
                    model.SubFormTab1s.Add(subFormTab1);
                }
            }
            var dicsTwo = new SubDicKindTabTwoRepository().GetAll();
            var listTwo = new List<SUB_ActionTab2>();
            foreach (var rptDicKindWaste in dicsTwo)
            {
                var report = model.SUB_ActionTab2.FirstOrDefault(e => e.KindId == rptDicKindWaste.Id);
                if (report != null)
                {
                    listTwo.Add(report);
                }
                else
                {
                    var kind = new SUB_ActionTab2 { KindId = rptDicKindWaste.Id, SUB_DIC_KindTabTwo = rptDicKindWaste };
                    listTwo.Add(kind);
                }
            }
            model.SubFormTab2s = listTwo;
            model.SubFormTab3s = new List<SUB_ActionTab3>();
            var forms4 = model.SUB_ActionTab3.OrderBy(e => e.Id);
            foreach (var record in forms4)
            {
                model.SubFormTab3s.Add(record);
            }
            if (model.SubFormTab3s.Count == 0)
            {
                model.SubFormTab3s.Add(new SUB_ActionTab3());
            }


        }
    }
}