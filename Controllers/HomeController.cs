using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Report;
using Aisger.Utils;

namespace Aisger.Controllers
{
    public class HomeController : ACommonController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        public ActionResult StartPage()
        {
            var model = new List<MainPageInfoModel>();
            try
            {
                var cm = new Common();
                #region query
                string query = " select 'гу' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year "
                                    + "   and usr.\"IsDeleted\"=FALSE   "
                                    + "   and r.usrfscode=3  "
                                    + "   and r.\"IsExcluded\"=false  "
                                    + " union "
                                    + " select 'юл' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE "
                                    + "   and r.usrfscode=1  "
                                    + "   and r.\"IsExcluded\"=false  "
                                    + "  union "
                                    + " select 'кв' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE "
                                    + "   and r.usrfscode=2  "
                                    + "   and r.\"IsExcluded\"=false  "
                                    + "   union "
                                    + " select 'ип' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE "
                                    + "   and r.usrfscode=4  "
                                    + "   and r.\"IsExcluded\"=false  "
                                    + "   "
                                    + "   union "
                                    + " select 'субъекты гэр' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE   "
                                    + "   union "
                                    + " select 'исключено' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE   "
                                    + "   and r.\"IsExcluded\"=true "
                                    + "  union "
                                    + " select 'уклонились' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE "
                                    + "   and r.\"ReasonId\"=6 "
                                    + "   union "
                                    + " select 'в реестре' as name , count(*) as cn   from  public.\"RST_ReportReestr\"  as r   "
                                    + "  inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\"   "
                                    + "  inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\"  "
                                    + "  where   r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"=#year"
                                    + "   and usr.\"IsDeleted\"=FALSE "
                                    + "   and r.\"IsExcluded\"=false ";
                #endregion
                string strCn = string.Empty;
                string strName = string.Empty;

                model = cm.GetMainPageInfoList(query);
            }
            catch (Exception ex)
            {
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public JsonResult GetOblastName()
        {
            var repository = new KatoRepository();
            var allItems = repository.GetKatos(1, true);

            var list = new MultiSelectList(allItems, "Id", CultureHelper.GetDictionaryName("NameRu"));
            var oblast = new List<string>();
            oblast.Add("");
            foreach (var allItem in list)
            {
                oblast.Add(allItem.Text);
            }
            return Json(new { Items = oblast });
        }

        [HttpPost]
        public JsonResult GetStatus(string code)
        {
            var oblast = new List<string>();
            oblast.Add("");
            if (code == CodeConstManager.AppForm)
            {
                var repository = new SubDicStatusRepository();
                var allItems = repository.GetAll();
                var list = new MultiSelectList(allItems, "Id", CultureHelper.GetDictionaryName("NameRu"));
                foreach (var allItem in list)
                {
                    oblast.Add(allItem.Text);
                }
            }
            if (code == CodeConstManager.MapApp)
            {
                var repository = new MapDicStatusRepository();
                var allItems = repository.GetAll();
                var list = new MultiSelectList(allItems, "Id", CultureHelper.GetDictionaryName("NameRu"));
                foreach (var allItem in list)
                {
                    oblast.Add(allItem.Text);
                }
            }
            return Json(new { Items = oblast });
        }
        [HttpPost]
        public JsonResult ExportExcelFile(string url)
        {
            return Json(new { Success = true });
        }

        [AllowAnonymous]
        public ActionResult QuestionAnswers()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult VideoInstructions()
        {
            return View();
        }
    }
}
