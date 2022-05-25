using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity.Map;
using Aisger.Models.Entity.Reestr;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Report;
using Aisger.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Reestr;

namespace Aisger.Controllers.Report
{
	public class SourceControllerController : ACommonController
	{
		private static string lang = CultureHelper.GetCurrentCulture();

        //
        // GET: /SourceController/
        //[GerNavigateLogger]
        public ActionResult Index()
		{
			return View();
		}

		public ActionResult GetNotOwnResources(int year, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, string fscode,string oked_ids, string columnsVal = "NotOwnSource", int pageNum = 1, bool isshowtut = false, long restype_id = 0, double min = 0, double max = 0, int orderBy = -1, string name_oked_idk = "",int isplan=-1, int isem_system=-1)
		{
			string dirpath = Server.MapPath("~/bin/Aisger.dll");
			var date = System.IO.File.GetLastWriteTime(dirpath);
			string strdate = date.ToString("dd-MM-yyyy HH:mm");
			//----
			RObjectClass<SourceControllerClass> item = new RObjectClass<SourceControllerClass>();

			//---- fill data	
			List<SourceControllerClass> model = new List<SourceControllerClass>();
			string errorMessage = new SourceControllerRepository().getNotOwnSource(columnsVal, year, pageNum, ref model, oblast_ids, reason_ids, excluded_id, expectant_ids, isshowtut, restype_id, min, max, orderBy, name_oked_idk, fscode,oked_ids,isplan,isem_system);
			item.ListItems = model;

			//---- fill count
			if (errorMessage == "")
			{
				int allcount = 0;
				errorMessage = new SourceControllerRepository().getNotOwnSourcePageCount(columnsVal, year, ref allcount, "-1", "", 0, "", -1, 0, 0, "", "","",-1,-1);
				item.AllCount = allcount;

				//----
				int count = 0;
				errorMessage = new SourceControllerRepository().getNotOwnSourcePageCount(columnsVal, year, ref count, oblast_ids, reason_ids, excluded_id, expectant_ids, restype_id, min, max, name_oked_idk, fscode,oked_ids,isplan,isem_system);
				item.Count = count;
			}

			item.ErrorMessage = errorMessage;
			var json = Json(item);
			json.MaxJsonLength = 50000000;
			return json;
		}

		public ActionResult getResourceType()
		{
			List<ResourceHelper> list = new List<ResourceHelper>();
			string errorMessage = new SourceControllerRepository().getResourceType(ref  list);

			return Json(list);
		}
       
		//----
		public ActionResult GetSomeDictionary()
		{
			Dictionary<string, object> item = new Dictionary<string, object>();
			string errorMessage = "";
			try
			{
                //----
                var excludeds = new List<UnMappedDictionary>
                {
                new UnMappedDictionary(CodeConstManager.RST_EXCLUDED_ALL_ID, CultureHelper.GetDictionaryName(CodeConstManager.RST_EXCLUDED_ALL)),
                new UnMappedDictionary(CodeConstManager.RST_EXCLUDED_ID, CultureHelper.GetDictionaryName(CodeConstManager.RST_EXCLUDED_NAME)),
                new UnMappedDictionary(CodeConstManager.RST_NOTEXCLUDED_ID, CultureHelper.GetDictionaryName(CodeConstManager.RST_NOTEXCLUDED_NAME)),
                };

                var excludeds2 = new List<UnMappedDictionary>
                {
                    new UnMappedDictionary(CodeConstManager.RST_EXCLUDED_ID, ResourceSetting.yes),
                    new UnMappedDictionary(CodeConstManager.RST_NOTEXCLUDED_ID, ResourceSetting.no),
                };

                //----
                var buffer = new RstDicReasonRepository().GetAll();
				var reasons = new MultiSelectList(buffer, "Id", CultureHelper.GetDictionaryName("NameRu"), null);

				var expectants = buffer.Where(e => e.IsExcluded || e.Id == CodeConstManager.STATUS_EVADERS_ID).ToList();
				List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
				foreach (var o in expectants)
				{
					var dict = new Dictionary<string, object>();
					dict["Id"] = o.Id;
					dict["Name"] = o.Name;
					list.Add(dict);
				}
				list.Add(new Dictionary<string,object>());
				list.Last()["Id"] = null;
				list.Last()["Name"] = ResourceSetting.sNoData;  // нет данных

				item["expectants"] = list;
				item["excludeds"] = excludeds;
                item["excludeds2"] = excludeds2;
                item["reasons"] = reasons;

				//----
				List<SelectListItem> ListItems = new List<SelectListItem>();
				errorMessage = new SourceControllerRepository().getDicColumns(ref ListItems);
				item["diccolumns"] = ListItems;

				//----
				item["dicokeds"] = new SourceControllerRepository().GetDicOked();

				//----
				item["fscodes"] = new SourceControllerRepository().GetDicTypeApplication();

				//----
				item["dickatos"] = new SourceControllerRepository().GetDicKato();

                #region isplan 
                var isPlanList = new List<Dictionary<string, object>>();
                var isPlanItem1 = new Dictionary<string, object>();
                isPlanItem1["Id"] = 1;
                isPlanItem1["Name"] = ResourceSetting.yes;
                isPlanList.Add(isPlanItem1);

                var isPlanItem2 = new Dictionary<string, object>();
                isPlanItem2["Id"] = 0;
                isPlanItem2["Name"] = ResourceSetting.no;
                isPlanList.Add(isPlanItem2);
                #endregion
                item["isplan"] = isPlanList;

            }
            catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			item["ErrorMessage"] = errorMessage;

			return Json(item);
		}

		//----
		public ActionResult EditControlParam(long rst_id, long user_id, bool exclusion, int? fscode, long? expectant_id)
		{
			var sec_user = new SEC_User();
			sec_user.Id = user_id;
			sec_user.FSCode = fscode;

			string errorMessage = new SecUserRepository().UpdateEmployeeForSourceController(sec_user, MyExtensions.GetCurrentUserId());
			if (errorMessage == "")
				errorMessage = new SourceControllerRepository().EditRst_ReportReestr(rst_id, exclusion, expectant_id,fscode);

			Dictionary<string, object> dict = new Dictionary<string, object>();
			dict["ErrorMessage"] = errorMessage;
			return Json(dict);
		}

		//----
		public ActionResult ExportExcel(int year, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, string fscode,string oked_ids, string columnsVal = "NotOwnSource", bool isshowtut = false, long restype_id = -1, double min = 0, double max = 0, int orderBy = -1,int isplan=-1,int isem_system=-1, string name_oked_idk = "")
		{
			#region excell create begin
			ExcelPackage pck = new ExcelPackage();
			var ws = pck.Workbook.Worksheets.Add("Отчет");
			ws.Column(1).Width = 15;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 30;
			ws.Column(4).Width = 30;
			ws.Column(5).Width = 30;
			ws.Column(6).Width = 30;
			ws.Column(7).Width = 30;
			ws.Column(8).Width = 30;
			ws.Column(9).Width = 30;
			ws.Column(10).Width = 30;
            ws.Column(11).Width = 30;
            ws.Column(12).Width = 30;
           // ws.Column(13).Width = 30;
            int rowCount = 2;
			int cellCount = 12; 

			#region fill header
			ws.Cells[1, 1, 2, 1].Merge = true;
			ws.Cells[1, 1, 2, 1].Value = ResourceSetting.IDK;
			ws.Cells[1, 1, 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 2, 2, 2].Merge = true;
            ws.Cells[1, 2, 2, 2].Value = ResourceSetting.BININ;
            ws.Cells[1, 2, 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 3, 2, 3].Merge = true;
			ws.Cells[1, 3, 2, 3].Value = ResourceSetting.DicOked;
			ws.Cells[1, 3, 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 4, 2, 4].Merge = true;
			ws.Cells[1, 4, 2, 4].Value = ResourceSetting.ApplicationName;
			ws.Cells[1, 4, 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 5, 2, 5].Merge = true;
			ws.Cells[1, 5, 2, 5].Value = ResourceSetting.Oblast;
			ws.Cells[1, 5, 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 6, 2, 6].Merge = true;
			ws.Cells[1, 6, 2, 6].Value = ResourceSetting.TypeApplication;
			ws.Cells[1, 6, 2, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 7, 2, 7].Merge = true;
			ws.Cells[1, 7, 2, 7].Value = ResourceSetting.sExclusion;
			ws.Cells[1, 7, 2, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

			ws.Cells[1, 8, 2, 8].Merge = true;
			ws.Cells[1, 8, 2, 8].Value = ResourceSetting.ExpectantName;
			ws.Cells[1, 8, 2, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 9, 2, 9].Merge = true;
            ws.Cells[1, 9, 2, 9].Value = ResourceSetting.EnergyAuditConducted;
            ws.Cells[1, 9, 2, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 10, 2, 10].Merge = true;
            ws.Cells[1, 10, 2, 10].Value = ResourceSetting.EMSystemImplemented;
            ws.Cells[1, 10, 2, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 11, 2, 11].Merge = true;
			ws.Cells[1, 11, 2, 11].Value = ResourceSetting.showTutSum;
			ws.Cells[1, 11, 2, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			ws.Cells[1, 11, 2, 11].Style.WrapText = true;

			ws.Cells[1, 12, 2, 12].Merge = true;
			ws.Cells[1, 12, 2, 12].Value = ResourceSetting.ExpensesTengeIncludingVAT;
			ws.Cells[1, 12, 2, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			ws.Cells[1, 12, 2, 12].Style.WrapText = true;

            //ws.Cells[1, 13, 2, 13].Merge = true;
            //ws.Cells[1, 13, 2, 13].Value = ResourceSetting.sTutTotal;
            //ws.Cells[1, 13, 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //ws.Cells[1, 13, 2, 13].Style.WrapText = true;

            #region fill type resources
            List<ResourceHelper> dicList = new List<ResourceHelper>();
			string errorMessage = new SourceControllerRepository().getResourceType(ref dicList);
			cellCount = cellCount + dicList.Count;

			int colIndex = 1;
			foreach (var item in dicList)
			{
				ws.Cells[1, 12 + colIndex].Value = item.resource_name;
				ws.Cells[1, 12 + colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

				ws.Cells[2, 12 + colIndex].Value = item.unit_name;
				colIndex++;
			}

			//----
			ws.Row(1).Height = 30;
			for (int i = 1; i <= dicList.Count; i++)
			{
				ws.Column(12+i).Width = 15;
				ws.Cells[1, 12+i].Style.WrapText = true;
			}
			ws.Cells[2, 1, 2, cellCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			ws.Cells[1, 1, 1, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells[2, 1, 2, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			ws.Cells[1, 1, 1, cellCount].Style.Font.Bold = true;
			#endregion

			#endregion

			#region fill body
			if (errorMessage == "")
			{
				int rowIndex = 0;
				var list = new List<SourceControllerClass>();
				errorMessage = new SourceControllerRepository().getNotOwnSourceForExcelExport(columnsVal, year, 0, ref list, oblast_ids, reason_ids, excluded_id, expectant_ids, isshowtut, restype_id, min, max, orderBy, fscode,oked_ids,isplan,isem_system,name_oked_idk);
                if (!string.IsNullOrWhiteSpace(errorMessage)) { throw new Exception(errorMessage); }
                rowCount = rowCount + list.Count;

				foreach (var item in list)
				{
					ws.Cells[3 + rowIndex, 1].Value = item.idk;
                    ws.Cells[3 + rowIndex, 2].Value = item.bin;
                    ws.Cells[3 + rowIndex, 3].Value = item.oked_name;
					ws.Cells[3 + rowIndex, 4].Value = item.juridical_name;
					ws.Cells[3 + rowIndex, 5].Value = item.oblast_name;
					ws.Cells[3 + rowIndex, 6].Value = item.fscode_name;
					ws.Cells[3 + rowIndex, 7].Value = item.excluded_name;
					ws.Cells[3 + rowIndex, 8].Value = item.expectant_name;
                    ws.Cells[3 + rowIndex, 9].Value = item.isplan;
                    ws.Cells[3 + rowIndex, 10].Value = item.isem_system;
                    ws.Cells[3 + rowIndex, 11].Value = (item.consumption == null) ? item.consumption : Math.Round(Convert.ToDouble(item.consumption), 3); ;
                    ws.Cells[3 + rowIndex, 12].Value = (item.sum_expenceenergy == null) ? item.sum_expenceenergy : Math.Round(Convert.ToDouble(item.sum_expenceenergy), 3); ;
                   // ws.Cells[3 + rowIndex, 13].Value = (item.tut == null) ? item.tut : Math.Round(Convert.ToDouble(item.tut), 3); ;
					int cIndex = 0;
					foreach (var dItem in dicList)
					{
                        if (dItem.dic_type == "0")
                        {
                            var dict = item.GetType().GetProperty("noS" + dItem.resource_id);
                            object val = dict.GetValue(item, null);

                            if (val != null)
                                val = (object)Math.Round(Convert.ToDouble(val), 3);

                            ws.Cells[3 + rowIndex, 13 + cIndex].Value = val;
                        }
                        else if (dItem.dic_type == "1")
                        {
                            if (dItem.resource_id == 1)
                            {
                                ws.Cells[3 + rowIndex, 13 + cIndex].Value = (item.coV1 != null) ? (object)Math.Round(Convert.ToDouble(item.coV1), 3) : "";
                            }

                            if (dItem.resource_id == 2)
                            {
                                ws.Cells[3 + rowIndex, 13 + cIndex].Value = (item.coV2 != null) ? (object)Math.Round(Convert.ToDouble(item.coV2), 3) : "";
                            }

                            if (dItem.resource_id == 3)
                            {
                                ws.Cells[3 + rowIndex, 13 + cIndex].Value = (item.coV3 != null) ? (object)Math.Round(Convert.ToDouble(item.coV3), 3) : "";
                            }
                        }
                        else if (dItem.dic_type == "2")
                        {
                            if (dItem.resource_id == 1)
                                ws.Cells[3 + rowIndex, 13 + cIndex].Value = item.countOfEmployees;

                            if (dItem.resource_id == 2)
                                ws.Cells[3 + rowIndex, 13 + cIndex].Value = item.countOfStudents;

                            if (dItem.resource_id == 3)
                                ws.Cells[3 + rowIndex, 13 + cIndex].Value = item.countOfBeds;
                        }
						cIndex++;
					}

					rowIndex++;
				}
			}
			#endregion

			#region fill border
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < cellCount; j++)
				{
					ws.Cells[1 + i, 1 + j].Style.Border.BorderAround(
						ExcelBorderStyle.Thin);
				}
			}
			#endregion

			#endregion

			FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.FileDownloadName = "Потребление энергоресурсов, полученные не из собственных источников.xlsx";
			return result;
		}

        //----
        public ActionResult ExportSumExcel(int year, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, string fscode, string oked_ids, bool isshowtut = false, long restype_id = -1, double min = 0, double max = 0, int orderBy = -1, int isplan = -1, int isem_system = -1, string name_oked_idk = "")
        {
            #region excell create begin
            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Отчет");
            ws.Column(1).Width = 15;
            ws.Column(2).Width = 30;
            ws.Column(3).Width = 30;
            int rowCount = 2;
            int cellCount = 3;

            #region fill header
            ws.Cells[1, 1, 2, 1].Merge = true;
            ws.Cells[1, 1, 2, 1].Value = "№";
            ws.Cells[1, 1, 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 2, 2, 2].Merge = true;
            ws.Cells[1, 2, 2, 2].Value = "Наименование колонки";
            ws.Cells[1, 2, 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[1, 3, 2, 3].Merge = true;
            ws.Cells[1, 3, 2, 3].Value = "Итого";
            ws.Cells[1, 3, 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            #region fill type resources
            List<ResourceHelper> dicList = new List<ResourceHelper>();
            string errorMessage = new SourceControllerRepository().getResourceType(ref dicList);

            int colIndex = 1;

            //----
            ws.Row(1).Height = 30;
            for (int i = 1; i <= 3; i++)
            {
                // ws.Column(12 + i).Width = 15;
                ws.Cells[1, i].Style.WrapText = true;
            }
            ws.Cells[1, 1, 1, cellCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[1, 1, 1, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1, 2, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1, 1, cellCount].Style.Font.Bold = true;
            #endregion

            #endregion

            var sumItem = new SourceControllerClassSum();
            errorMessage = new SourceControllerRepository().getNotOwnSourceSum(year, ref sumItem, oblast_ids, reason_ids, excluded_id, expectant_ids, isshowtut, restype_id, min, max, orderBy, name_oked_idk, fscode, oked_ids, isplan, isem_system);

            #region fill body
            int rowIndex = 4;
            if (errorMessage == "")
            {
                //  rowCount = rowCount + list.Count;
                ws.Cells[3, 1].Value = 1;
                ws.Cells[3, 2].Value = ResourceSetting.sTutTotal;
                ws.Cells[3, 3].Value = (sumItem.tut == null) ? sumItem.tut : Math.Round(Convert.ToDouble(sumItem.tut), 3);
                             
                foreach (var dItem in dicList)
                {
                    ws.Cells[rowIndex, 1].Value = rowIndex-2;
                    ws.Cells[rowIndex, 2].Value = dItem.resource_name;

                    if (dItem.dic_type == "0")
                    {
                        var dict = sumItem.GetType().GetProperty("noS" + dItem.resource_id);
                        object val = dict.GetValue(sumItem, null);

                        if (val != null)
                            val = (object)Math.Round(Convert.ToDouble(val), 3);

                        ws.Cells[rowIndex, 3].Value = val;
                    }
                    else if (dItem.dic_type == "1")
                    {
                        if (dItem.resource_id == 1)
                        {
                            ws.Cells[rowIndex, 3].Value = (sumItem.coV1 != null) ? (object)Math.Round(Convert.ToDouble(sumItem.coV1), 3) : "";
                        }

                        if (dItem.resource_id == 2)
                        {
                            ws.Cells[rowIndex, 3].Value = (sumItem.coV2 != null) ? (object)Math.Round(Convert.ToDouble(sumItem.coV2), 3) : "";
                        }

                        if (dItem.resource_id == 3)
                        {
                            ws.Cells[rowIndex, 3].Value = (sumItem.coV3 != null) ? (object)Math.Round(Convert.ToDouble(sumItem.coV3), 3) : "";
                        }
                    }
                    else if (dItem.dic_type == "2")
                    {
                        if (dItem.resource_id == 1)
                            ws.Cells[rowIndex, 3].Value = sumItem.countOfEmployees;

                        if (dItem.resource_id == 2)
                            ws.Cells[rowIndex, 3].Value = sumItem.countOfStudents;

                        if (dItem.resource_id == 3)
                            ws.Cells[rowIndex, 3].Value = sumItem.countOfBeds;
                    }
                    
                    rowIndex++;
                }
            }

            #endregion

            #region fill border
            rowCount = rowIndex;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < cellCount; j++)
                {
                    ws.Cells[1 + i, 1 + j].Style.Border.BorderAround(
                        ExcelBorderStyle.Thin);
                }
            }
            #endregion

            #endregion

            FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = "Итоговые значения(" + year + ").xlsx";
            return result;
        }
        
        //----
        public ActionResult GetYears()
		{
			var years = new RstReportRepository().GetRstReport();
			return Json(years);
		}

        public ActionResult getNotOwnSourceSum(int year, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, string fscode, string oked_ids, int pageNum = 1, bool isshowtut = false, long restype_id = 0, double min = 0, double max = 0, int orderBy = -1, string name_oked_idk = "", int isplan = -1, int isem_system = -1)
        {
            var item = new SourceControllerClassSum();
            string errorMessage = new SourceControllerRepository().getNotOwnSourceSum(year, ref item, oblast_ids, reason_ids, excluded_id, expectant_ids, isshowtut, restype_id, min, max, orderBy, name_oked_idk, fscode, oked_ids, isplan, isem_system);
            return Json(new { ErrorMessage = errorMessage, item = item });
        }
    }
}