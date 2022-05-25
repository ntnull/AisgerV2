using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Report;
using Aisger.Models.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Report
{
    public class ReportAnalyseController : ACommonController
    {
        //
        // GET: /ReportAnalyse/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report1()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report2()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report3()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report4()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report5()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report6()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report7()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report8()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report9()
        {
            return View();
        }

        [GerNavigateLogger]
        public ActionResult Report10()
        {
            return View();
        }

        ReportAnalyseRepository bi = new ReportAnalyseRepository();
        string lang = "ru";

        #region report1
        public JsonResult GetReportTop10ByCons(int oblast_id, int year)
        {
            var item = bi.reportTop10ByCons(oblast_id, year);
            return Json(item);
        }

        public JsonResult GetReportTop10AndOthersByCons(int oblast_id, int year)
        {
            var item = bi.reportTop10AndOthersByCons(oblast_id, year, lang);
            return Json(item);
        }
        #endregion

        #region report2
        public JsonResult GetReportConsByRes7ForPie(int oblast_id = -1, int year = 2015)
        {
            var item = bi.reportConsByRes7ForPie(oblast_id, year, lang);
            return Json(item);
        }

        public JsonResult GetReportConsByRes7(int oblast_id = -1, int year = 2015)
        {
            var item = bi.reportConsByRes7(oblast_id, year, lang);
            return Json(item);
        }

        public JsonResult GetReportConsByRes1(int oblast_id = -1, int year = 2015)
        {
            var item = bi.reportConsByRes1(oblast_id, year, lang);
            return Json(item);
        }

        public JsonResult GetReportTop10ByRes(decimal resource_id, int year, int oblast_id = -1)
        {
            var item = bi.reportTop10ByRes(resource_id, year, oblast_id);
            return Json(item);
        }

        public JsonResult GetReportResByRegion(decimal resource_id, int year, int oblast_id = -1)
        {
            var item = bi.reportResByRegion(resource_id, year, oblast_id, lang);
            return Json(item);
        }
        #endregion

        #region subject form
        //----
        public JsonResult GetSubjectForm(decimal subject_id,int year)
        {
            var item = bi.subjectForm(subject_id, lang, year);
            return Json(item);
        }

        //----
        public JsonResult GetSubjectFormPieChart(decimal subject_id, int year)
        {
            var item = bi.subjectFormPieChart(subject_id, year, lang);
            return Json(item);
        }

        //----
        public JsonResult GetSubForm4Record(decimal subject_id, int year)
        {
            var item = bi.subForm4Record(subject_id, year);
            return Json(item);
        }

        //----
        public JsonResult GetSubFormResValumeByYears(decimal subject_id)
        {
            var item = bi.subFormResValumeByYears(subject_id);
            return Json(item);
        }

        //----
        public JsonResult GetSubFormDynamicsResByYears1(decimal subject_id, int year)
        {
            var item = bi.subFormDynamicsResByYears1(subject_id, year);
            return Json(item);
        }

        //----
        public JsonResult GetSubFormDynamicsResByYears7(decimal subject_id, int year)
        {
            var item = bi.subFormDynamicsResByYears7(subject_id, year, lang);
            return Json(item);
        }

        //----
        public JsonResult GetSubFormResConsumption(decimal subject_id, int year)
        {
            var item = bi.subFormResConsumption(subject_id, year, lang);
            return Json(item);
        }

        //----
        public JsonResult GetSubFormEnergyIndicators(decimal subject_id, int year)
        {
            var item = bi.subFormEnergyIndicators(subject_id, year);
            return Json(item);
        }

        //----
        public JsonResult GetSubFormEAudit(decimal subject_id)
        {
            var item = bi.subFormEAudit(subject_id);
            return Json(item);
        }

        #endregion

        #region report3
        public JsonResult GetReportByJurType(int oblast_id = -1)
        {
            var item = bi.reportByJurType(oblast_id, lang);
            return Json(item);
        }

        public JsonResult GetReportTop10ByJurType(int oblast_id, int year, int jur_type_id)
        {
            var item = bi.reportTop10ByJurType(oblast_id, year, jur_type_id);
            return Json(item);
        }
        #endregion

        #region report 5
        public JsonResult GetReportConsByOked(int year)
        {
            var item = bi.reportConsByOked(year, lang);
            return Json(item);
        }

        #endregion

        #region report 6
        public JsonResult GetReportEffecSecial(int oblast_id = -1, int year = 2015)
        {
            var item = bi.reportEffecSecial(oblast_id, year, lang);
            return Json(item);
        }

        public JsonResult GetReportFlatEffecSecial(int oblast_id = -1, int year = 2015)
        {
            var item = bi.reportFlatEffecSecial(oblast_id, year, lang);
            return Json(item);
        }
        #endregion

        #region report 7
        public JsonResult GetReportMoreThan100(int oblast_id = -1, int year = 2015)
        {
            var item = bi.reportMoreThan100(oblast_id, year);
            return Json(item);
        }

        #endregion

        #region report 10
        public JsonResult GetReportEE2(int oblast_id = -1, int year = 2015)
        {
            var item = new ReportEE2Repository().getReportEE2Analyse(oblast_id, lang);
            return Json(item);
        }

        #endregion

        #region on map
        public JsonResult GetReportSubjectsOnMap(int year = 2015)
        {
            var item = bi.reportSubjectsOnMap(year);
            return Json(item);
        }

        public JsonResult GetReportOblConsOnMap(int year = 2015)
        {
            var item = bi.reportOblConsOnMap(year, lang);
            return Json(item);
        }
        #endregion

		#region view
		public JsonResult GetViewByJurType()
		{
			var item = bi.viewByJurType(lang);
			return Json(item);
		}

		public JsonResult GetViewConsByRes8(int year=2015)
		{
			var item = bi.viewConsByRes8(year,lang);
			return Json(item);
		}

		public JsonResult GetViewByJurTop10(int year)
		{
			var item = bi.viewByJurTop10(year);
			return Json(item);
		}

	
		#endregion
		public JsonResult getDicKato(int refParent = 1)
        {
            Common common = new Common();

            Dictionary<string, object> item = new Dictionary<string, object>();

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string allname = "По республике";
            if (lang.Equals("kz"))
                allname = "Республика бойынша";

            list.Add(new Dictionary<string, object> { { "kato_id", -1 }, { "kato_name", allname } });

            string query = " SELECT \"Id\" as \"kato_id\" , "
                           + " case '" + lang + "' "
                           + " when 'ru' then \"NameRu\" "
                           + " when 'kz' then \"NameKz\" "
                           + " else  \"NameRu\" end as kato_name "
                           + " FROM  \"DIC_Kato\"  where \"refParent\"=" + refParent;

            string errorMessage = common.getTableWithList(ref list, query);
            item["ListItems"] = list;
            item["ErrorMessage"] = errorMessage;

            return Json(item);
        }

        //public JsonResult getDicKato(int refParent = 1)
        //{
        //    Common common = new Common();
        //    Dictionary<string, object> item = new Dictionary<string, object>();

        //    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        //    string allname = "По республике";
        //    if (lang.Equals("kz"))
        //        allname = "Республика бойынша";

        //    list.Add(new Dictionary<string, object> { { "kato_id", -1 }, { "kato_name", allname } });

        //    string query = " SELECT \"Id\" as \"kato_id\" , "
        //                   + " case '" + lang + "' "
        //                   + " when 'ru' then \"NameRu\" "
        //                   + " when 'kz' then \"NameKz\" "
        //                   + " else  \"NameRu\" end as kato_name "
        //                   + " FROM  \"DIC_Kato\"  where \"refParent\"=" + refParent;

        //    string errorMessage = common.getTableWithList(ref list, query);
        //    item["ListItems"] = list;
        //    item["ErrorMessage"] = errorMessage;

        //    return Json(item);
        //}

        public JsonResult getDicKato1(int refParent = 1)
        {
            Common common = new Common();
            Dictionary<string, object> item = new Dictionary<string, object>();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            list.Add(new Dictionary<string, object> { { "Id", -1 }, { "NameRu", "По республике" }, { "NameKz", "Республика бойынша" } });

            string query = " SELECT * FROM  \"DIC_Kato\"  where \"refParent\"=" + refParent+"  order by \"Id\" ";

            string errorMessage = common.getTableWithList(ref list, query);
            item["ListItems"] = list;
            item["ErrorMessage"] = errorMessage;

            return Json(item);
        }

        public JsonResult getDicOked()
        {
            Common common = new Common();
            string query = " select  t.*  from \"DIC_OKED\"  t  ";

            var item = common.getTableList(query);

            var jsonResult = Json(item);
            jsonResult.MaxJsonLength = 2000000000;
            return jsonResult;
        }

        public JsonResult getSDicTypeResource()
        {
            Common common = new Common();
            string query = " select   t2.\"NameKz\" as \"DIC_UNIT_NAMEKZ\" ,t2.\"NameRu\" as \"DIC_UNIT_NAMERU\" , t1.* " +
                           " from  public.\"SUB_DIC_TypeResource\" t1 , public.\"DIC_Unit\" t2 where  t1.\"UnitId\"=t2.\"Id\" and t1.\"Code\"='2' ";

            var item = common.getTableList(query);
            return Json(item);
        }

		public JsonResult getDicRstReport()
		{
			string errorMessage = "";
			var item = new Dictionary<string, object>();
			try
			{
				var years = new RstReportRepository().GetRstReport();
				item["ListItems"] = years;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			item["ErrorMessage"] = errorMessage;

			return Json(item);
		}

    }
}
