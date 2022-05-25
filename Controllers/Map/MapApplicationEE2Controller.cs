using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Entity.Map;
using Aisger.Models.Repository.Map;
using System.IO;
using Aisger.Utils;
using System.Configuration;
using System.Web.Configuration;
using Aisger.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Map
{
    public class MapApplicationEE2Controller : Controller
    {
        // GET: /MapApplicationEE2/

        MAP_ApplicationEE2Repository repository = new MAP_ApplicationEE2Repository();
        long? currUserId = MyExtensions.GetCurrentUserId();
        long? roless = MyExtensions.GetRolesId();

        //---- get files
        string fileFolderPath = WebConfigurationManager.AppSettings["FileFolderPath"];
        string fileServerLogin = WebConfigurationManager.AppSettings["FileServerLogin"];
        string fileServerPassword = WebConfigurationManager.AppSettings["FileServerPassword"];

        [GerNavigateLogger]
        public ActionResult Index()
        {
            if (roless == 5 || roless==1)
                return RedirectToAction("mIndex");
            else return RedirectToAction("uIndex", new { uId = currUserId , uType="user" });
        }

        [GerNavigateLogger]
        public ActionResult mIndex(string name, string biniin, string oblast, string status,string okeds)
		{
			var filter = getModelHelperByManager(name, biniin, oblast, status,okeds);
			return View(filter);
			//return new HttpUnauthorizedResult();
		}

		public MAP_mEE2Filters getModelHelperByManager(string name, string biniin, string oblast, string status,string okeds)
		{
			string errorMessage = "";
			MAP_mEE2Filters filter = new MAP_mEE2Filters();
			filter.Name = name;
			filter.BINIIN = biniin;

			//---- область
			filter.Oblasts = new List<string>();
			if (!string.IsNullOrEmpty(oblast))
			{
				var objectList = oblast.Split(',');
				foreach (var s in objectList)
				{
					filter.Oblasts.Add(s);
				}
			}
			filter.OblastList = repository.GetOblasList(filter.Oblasts);

			//---- статус
			filter.Statuses = new List<string>();
			if (!string.IsNullOrEmpty(status))
			{
				var objectList = status.Split(',');
				foreach (var s in objectList)
				{
					filter.Statuses.Add(s);
				}
			}
			filter.StatusList = repository.GetDicEEStatusList2(filter.Statuses);

			//---- окэд
			filter.Okeds = new List<string>();
			if (!string.IsNullOrEmpty(okeds))
			{
				var objectList = okeds.Split(',');
				foreach (var s in objectList)
				{
					filter.Okeds.Add(s);
				}
			}
			filter.OkedList = repository.GetDicOked(filter.Okeds);

			filter.ListItems = new List<MAP_FormEE2Manager>();
			errorMessage += repository.getFormEE2ByManager(ref filter);

			return filter;
		}

        [GerNavigateLogger]
        public ActionResult uIndex(long uId, string uType = "manager")
		{
			//---- id -> secuserid
			var model = getModelHelper(uId, uType);
			return View(model);
		}

		public MAP_EE2Filters getModelHelper(long uId, string uType = "manager")
		{
			string errorMessage = "";

			MAP_EE2Filters model = new MAP_EE2Filters();
			errorMessage = repository.getFormEE2ItemBySecUserId(ref model, uId);
			model.SecUserId = uId;

			IEnumerable<MAP_ApplicationEE2Info> ItemLists = new List<MAP_ApplicationEE2Info>();
			errorMessage += repository.getApplicationEE2ItemList(ref ItemLists, uId);


			var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId + "/");

			if (Directory.Exists(dirpath))
			{
				foreach (var fItem in ItemLists)
				{
					string filesStr = "";
					errorMessage += getFiles(ref filesStr, fItem.Id);
					if (errorMessage != "")
						break;
					else fItem.files = filesStr;
				}
			}

			//----
			model.MapApplicationEE2List = ItemLists;
			model.DicTypeApplicationList = repository.GetDicTypeApplicationList();
			model.DicEEStatusList = repository.GetDicEEStatusList();

			ViewData["DicTypeApplicationList"] = new SelectList(model.DicTypeApplicationList, "Value",
										   "Text", model.FSCode);

			ViewData["DicEEStatusList"] = new SelectList(model.DicEEStatusList, "Value",
										   "Text", model.DicEEStatusId);

			if (uType.Equals("manager"))
			{
				ViewData["IsRole"] = "5";
				ViewBag.IsRole = "5";
			}
			else
			{
				ViewBag.IsRole = "0";
				ViewData["IsRole"] = "0";
			}

			if (errorMessage != "")
			{
				model.ErrorMessage = errorMessage;
				model.IsError = true;
			}

			return model;
		}

        [GerNavigateLogger]
        public ActionResult Create(long uId)
        {
            MAP_ApplicationEE2Info model = new MAP_ApplicationEE2Info();
            model.SecUserId = uId;

            model.DicDetailsList = repository.GetDicDetailsList();
            model.DicDetailModelsList = repository.GetDicDetailModelsList();

            ViewData["DicDetailsList"] = new SelectList(model.DicDetailsList, "Value",
                                       "Text", model.DicDetailsId);
            ViewBag.vDicDetailModelsId = 0;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(MAP_ApplicationEE2Info model, IEnumerable<HttpPostedFileBase> files)
        {
            ModelState.Remove("Id");

            string errorMessage = "";

            if (ModelState.IsValid)
            {

                if (model.Id == 0)
                {
                    errorMessage = repository.insertMapApplicationEE2(ref model);
                }
                else
                {
                    errorMessage = repository.updateMapApplicationEE2(model);
                }

                ViewBag.Message = errorMessage;

                if (files != null)
                {
                    try
                    {
                        var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId + "/");

                        if (!Directory.Exists(dirpath))
                        {
                            Directory.CreateDirectory(dirpath);
                        }

                        int fcount = 0;
                        foreach (var file in files)
                        {
                            if (file == null || file.ContentLength <= 0) continue;

                            Guid nguid = Guid.NewGuid();

                            var uploadFileName = Path.GetFileName(file.FileName);
                            if (uploadFileName == null) continue;

                            var arr = uploadFileName.Split('.');
                            var newFileName = model.Id + "-" + nguid + "." + arr[arr.Length - 1];

                            var uploadFilePathAndName = Path.Combine(dirpath, newFileName);
                            ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }

                long uId = model.SecUserId;
                if (errorMessage == "")
                    return RedirectToAction("uIndex", new { uId = uId });  //, new { uId=model.SecUserId;}

            }
            else errorMessage = "Non Valid";

            //----
            model.IsError = true;
            ViewBag.Message = errorMessage;

            model.DicDetailsList = repository.GetDicDetailsList();
            model.DicDetailModelsList = repository.GetDicDetailModelsList();

            ViewData["DicDetailsList"] = new SelectList(model.DicDetailsList, "Value",
                              "Text", model.DicDetailsId);

            return View("Create", model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            MAP_ApplicationEE2Info item = new MAP_ApplicationEE2Info();
            string errorMessage = repository.getApplicationEE2Item(id, ref item);

            //---- get files
            var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId + "/");
            if (Directory.Exists(dirpath))
            {
                string filesStr = "";
                errorMessage += getFiles(ref filesStr, item.Id, "edit");
                item.files = filesStr;
            }


            item.DicDetailsList = repository.GetDicDetailsList();
            item.DicDetailModelsList = repository.GetDicDetailModelsList();

            ViewData["DicDetailsList"] = new SelectList(item.DicDetailsList, "Value",
                                       "Text", item.DicDetailsId);

            ViewBag.vDicDetailModelsId = item.DicDetailModelsId;

            return View("Create", item);
        }

        public ActionResult Delete(long id)
        {
            string errorMessage = "";

            if (ModelState.IsValid)
            {
                errorMessage = repository.deleteMapApplicationEE2(id);

                if (errorMessage == "")
                {  //---- удалить всех файлы
                    try
                    {
                        var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId);
                        if (Directory.Exists(dirpath))
                        {
                            var fileName = dirpath + "/" + id + "-*";

                            string[] filePaths2 = Directory.GetFiles(dirpath, id + "-*");
                            for (int i = 0; i < filePaths2.Length; i++)
                            {
                                System.IO.File.Delete(filePaths2[i]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }

                }
            }
            else errorMessage = "Non Valid";

            ViewBag.Message = errorMessage;

			RObjectClass<MAP_ApplicationEE2Info> item = new RObjectClass<MAP_ApplicationEE2Info>();
            item.ErrorMessage = errorMessage;

            return Json(item);
        }

        public JsonResult GetDicDetailModels()
        {
            string errorMessage = "";

            List<MAP_DIC_EEDetailModelsInfo> list = new List<MAP_DIC_EEDetailModelsInfo>();
            errorMessage = repository.GetDicDetailModels(ref list);
			RObjectClass<MAP_DIC_EEDetailModelsInfo> item = new RObjectClass<MAP_DIC_EEDetailModelsInfo>();
            item.ListItems = list;
            item.ErrorMessage = errorMessage;

            return Json(item);
        }

        //----Map_FormEE2
		public JsonResult InsertFormEE2(long secUserId, int FSCode, string TotalArea, string NumberOfStoreys, int statusId, string Comments)
		{
			ReturnItem<MAP_FormEE2> rItem = new ReturnItem<MAP_FormEE2>();
			try
			{
				MAP_FormEE2 newItem = new MAP_FormEE2();
				newItem.SecUserId = secUserId;
				newItem.FSCode = FSCode;
				newItem.Comments = Comments;

				if (!string.IsNullOrWhiteSpace(TotalArea))
					newItem.TotalArea = Convert.ToDouble(TotalArea.Replace('.', ','));

				if (!string.IsNullOrWhiteSpace(NumberOfStoreys))
					newItem.NumberOfStoreys = Convert.ToInt32(NumberOfStoreys.Replace('.', ','));

				newItem.DicEEStatusId = statusId;

				rItem.ErrorMessage = repository.insertMapFormEE2(ref newItem);
				rItem.Item = newItem;
			}
			catch (Exception ex)
			{
				rItem.Item = new MAP_FormEE2();
				rItem.Item.Id = -1;
				rItem.ErrorMessage = ex.Message;
			}

			return Json(rItem);
		}

        public JsonResult UpdateFormEE2(long secUserId,long Id, int FSCode, string TotalArea, string NumberOfStoreys, int statusId, string Comments)
        {
            string errorMessages = "";
			RObjectClass<MAP_FormEE2> item = new RObjectClass<MAP_FormEE2>();
            try
            {
                MAP_FormEE2 uItem = new MAP_FormEE2();
                uItem.Id = Id;
                uItem.FSCode = FSCode;
                uItem.Comments = Comments;
                if (!string.IsNullOrWhiteSpace(TotalArea))
                    uItem.TotalArea = Convert.ToDouble(TotalArea.Replace('.', ','));

                if (!string.IsNullOrWhiteSpace(NumberOfStoreys))
                    uItem.NumberOfStoreys = Convert.ToInt32(NumberOfStoreys.Replace('.', ','));

                uItem.DicEEStatusId = statusId;

                errorMessages += repository.updateMapFormEE2(uItem);

                item.ListItems = new List<MAP_FormEE2>();
                item.ListItems.Add(uItem);

                //----send massage to email
                if (statusId == 2)
                    new SendMessageManager().SendMapApplicationEE2(secUserId);
            }
            catch (Exception ex)
            {
                errorMessages += ex.Message;
            }

            item.ErrorMessage = errorMessages;

            return Json(item);
        }

		public JsonResult UpdateFormEE2Status(long Id, int statusId)
		{
			ReturnItem<MAP_FormEE2> rItem = new ReturnItem<MAP_FormEE2>();
			try
			{
				MAP_FormEE2 uItem = new MAP_FormEE2();
				uItem.Id = Id;
				uItem.DicEEStatusId = statusId;

				rItem.ErrorMessage=repository.updateMapFormEE2Status(uItem);
				rItem.Item = uItem;
			}
			catch (Exception ex)
			{
				rItem.ErrorMessage = ex.Message;
			}

			return Json(rItem);
		}

        //---- Map_FormEE2Record
        public JsonResult GetFormEE2Records(long secUserId)
        {
            List<Map_FormEE2Record> list = new List<Map_FormEE2Record>();

            string errorMessages = repository.getFormEE2RecordItemBySecUserId(ref list,secUserId);

			RObjectClass<Map_FormEE2Record> item = new RObjectClass<Map_FormEE2Record>();
            item.ListItems = list;
            item.ErrorMessage = errorMessages;

            return Json(item);
        }

		public JsonResult InsertFormEE2Records(long FormEE2Id, string yearStr, string sourcesStr, string expencesStr)
		{
			ReturnItem<Map_FormEE2Record> rItem = new ReturnItem<Map_FormEE2Record>();
			string errorMessages = "";
			try
			{
				var yearArr = yearStr.Split(',').Select(Int32.Parse).ToList();
				var sourceArr = sourcesStr.Split(',').ToList();
				var expencesArr = expencesStr.Split(',').ToList();

				for (int i = 0; i < yearArr.Count; i++)
				{

					Map_FormEE2Record newItem = new Map_FormEE2Record();
					newItem.FormEE2Id = FormEE2Id;
					newItem.ReportYear = yearArr[i];
					newItem.EnergySource = Convert.ToDouble((sourceArr[i].Replace('.', ',')));
					newItem.ExpenceEnergy = Convert.ToDouble((expencesArr[i].Replace('.', ',')));
					errorMessages += repository.insertMapFormEE2Record(newItem);
				}
			}
			catch (Exception ex)
			{
				errorMessages += ex.Message;
			}


			rItem.ErrorMessage = errorMessages;
			return Json(rItem);
		}

		public JsonResult UpdateFormEE2Records(string idStr, string yearStr, string sourcesStr, string expencesStr)
		{
			ReturnItem<Map_FormEE2Record> rItem = new ReturnItem<Map_FormEE2Record>();
			string errorMessages = "";
			try
			{
				var idsArr = idStr.Split(',').Select(long.Parse).ToList();
				var yearArr = yearStr.Split(',').Select(Int32.Parse).ToList();
				var sourceArr = sourcesStr.Split(',').ToList();
				var expencesArr = expencesStr.Split(',').ToList();


				for (int i = 0; i < yearArr.Count; i++)
				{
					Map_FormEE2Record newItem = new Map_FormEE2Record();
					newItem.Id = idsArr[i];
					newItem.ReportYear = yearArr[i];
					newItem.EnergySource = Convert.ToDouble((sourceArr[i].Replace('.', ',')));
					newItem.ExpenceEnergy = Convert.ToDouble((expencesArr[i].Replace('.', ',')));
					errorMessages += repository.updateMapFormEE2Record(newItem);
				}

			}
			catch (Exception ex)
			{
				errorMessages = ex.Message;
			}

			rItem.ErrorMessage = errorMessages;
			return Json(rItem);
		}


        //---- загрузить файл
        public ActionResult Upload(string fname)
        {
            var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId);
            if (Directory.Exists(dirpath))
            {
                string physicalPath = dirpath + "/" + fname;
                return File(physicalPath, System.Net.Mime.MediaTypeNames.Application.Octet, fname);
            }

            return RedirectToAction("Index");
        }

        //---- удалить файл
        public ActionResult DeleteFile(string fname, long id)
        {
            var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId);
            if (Directory.Exists(dirpath))
            {
                string physicalPath = dirpath + "/" + fname;
                System.IO.File.Delete(physicalPath);
            }

            return RedirectToAction("Edit", new { id = id });
        }

        //---- helper func
        public string getFiles(ref string files, long id, string editMode = "view")
        {
            string errorMessage = "";
            try
            {
                files = "";
                var dirpath = Server.MapPath("~/uploads/applicationee2/" + currUserId);
                var fileName = dirpath + "/" + id + "-*";

                string[] filePaths2 = Directory.GetFiles(dirpath, id + "-*");
                for (int i = 0; i < filePaths2.Length; i++)
                {
                    string checkFiles = filePaths2[i];
                    if (!string.IsNullOrEmpty(checkFiles))
                    {
                        checkFiles = Replacer(checkFiles);
                        var arr = checkFiles.Split('*');
                        if (editMode.Equals("view"))
                            files += "<a href='/MapApplicationEE2/Upload?fname=" + arr[arr.Length - 1] + "' >" + arr[arr.Length - 1] + "</a><br>";
                        else files += "<a href='/MapApplicationEE2/Upload?fname=" + arr[arr.Length - 1] + "' >" + arr[arr.Length - 1] + "</a><span class='glyphicon glyphicon-remove remove-file' filename='" + arr[arr.Length - 1] + "' o-id='" + id + "'></span><br>";
                    }
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        private string Replacer(string str)
        {
            while (str.IndexOf("\\") != -1)
            {
                str = str.Replace("\\", "*");
            }

            return str;
        }

		public ActionResult ExportExcel(long uId)
		{
			MAP_EE2Filters model = getModelHelper(uId);

			#region excell create begin
			ExcelPackage pck = new ExcelPackage();
			var ws = pck.Workbook.Worksheets.Add("Отчет");
			ws.Column(1).Width = 30;
			ws.Column(2).Width = 30;
			ws.Column(3).Width = 30;
			ws.Column(4).Width = 30;
			ws.Column(5).Width = 30;
			ws.Column(6).Width = 30;
			//ws.Column(6).Width = 60;

			ws.Cells[2, 1, 2, 5].Merge = true;
			ws.Cells[2, 1, 2, 5].Value = "Заявление в программу энергоэффективность 2.0";
			ws.Cells[2, 1, 2, 5].Style.Font.Bold = true;
			ws.Cells[2, 1, 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


			#region форма собс
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					ws.Cells[5 + i, 1 + j].Style.Border.BorderAround(
						ExcelBorderStyle.Thin);
				}
			}
			
			ws.Cells["A5"].Value = "Форма собственности";
			ws.Cells["A5"].Style.Font.Bold = true;

			ws.Cells["A6"].Value = "Общая площадь (кв.м)";
			ws.Cells["A6"].Style.Font.Bold = true;

			ws.Cells["A7"].Value = "Этажность";
			ws.Cells["A7"].Style.Font.Bold = true;

			ws.Cells["A8"].Value = "Статус";
			ws.Cells["A8"].Style.Font.Bold = true;
			if (model.FSCode != null)
			{
				var buffer = model.DicTypeApplicationList.Where(x => x.Value.Equals(model.FSCode.ToString())).FirstOrDefault();
				ws.Cells["B5"].Value = buffer.Text;
			}
			ws.Cells["B6"].Value = model.TotalArea;
			ws.Cells["B7"].Value = model.NumberOfStoreys;
			if (model.DicEEStatusId != null)
			{
				var buffer = model.DicEEStatusList.Where(x => x.Value.Equals(model.DicEEStatusId.ToString())).FirstOrDefault();
				ws.Cells["B8"].Value = buffer.Text;
			}
			#endregion

		   #region год
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					ws.Cells[4 + i, 4 + j].Style.Border.BorderAround(
						ExcelBorderStyle.Thin);
				}
			}

			ws.Cells["E4"].Value = "кВт*ч";
			ws.Cells["E4"].Style.Font.Bold = true;
			ws.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			ws.Cells["F4"].Value = "тыс.тг.";
			ws.Cells["F4"].Style.Font.Bold = true;
			ws.Cells["F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			ws.Cells["D5"].Value = "2012 г.";
			ws.Cells["D5"].Style.Font.Bold = true;

			ws.Cells["D6"].Value = "2013 г.";
			ws.Cells["D6"].Style.Font.Bold = true;

			ws.Cells["D7"].Value = "2014 г.";
			ws.Cells["D7"].Style.Font.Bold = true;

			ws.Cells["D8"].Value = "2015 г.";
			ws.Cells["D8"].Style.Font.Bold = true;

			ws.Cells["D9"].Value = "2016 г.";
			ws.Cells["D9"].Style.Font.Bold = true;

			List<Map_FormEE2Record> list = new List<Map_FormEE2Record>();
			string errorMessages = repository.getFormEE2RecordItemBySecUserId(ref list, uId);
			if (errorMessages == "")
			{
				for (int i = 0; i < 5; i++)
				{
					int year = 2012 + i;
					var item = list.Where(x => x.ReportYear == year).FirstOrDefault();
					ws.Cells[5 + i, 5].Value = item.EnergySource;
					ws.Cells[5 + i, 6].Value = item.ExpenceEnergy;
				}
			}
		

		   #endregion

			#region знач
		    List<MAP_ApplicationEE2Info>  inItem=model.MapApplicationEE2List.ToList();
			for (int i = 0; i<inItem.Count()+1; i++) {
				
				for (int j = 0; j < 12; j++) {
					ws.Cells[13 + i, 1 + j].Style.Border.BorderAround(
						ExcelBorderStyle.Thin);
				}

				if (i < inItem.Count())
				{
					
					ws.Cells[14 + i, 1].Value = inItem[i].DicDetails_Name;
					ws.Cells[14 + i, 2].Value = inItem[i].DicDetailModels_Name;
					ws.Cells[14 + i, 3].Value = inItem[i].CountOfFixtures;
					ws.Cells[14 + i, 4].Value = inItem[i].CountOfLamps;
					ws.Cells[14 + i, 5].Value = inItem[i].Power;
					ws.Cells[14 + i, 6].Value = inItem[i].CPRA;
					ws.Cells[14 + i, 7].Value = inItem[i].AggregatePower;
					ws.Cells[14 + i, 8].Value = inItem[i].AverageTariff;
					ws.Cells[14 + i, 9].Value = inItem[i].WorkingHours;
					ws.Cells[14 + i, 10].Value = inItem[i].MaintenanceCosts;
					ws.Cells[14 + i, 11].Value = inItem[i].Comments;
					ws.Cells[14 + i, 12].Value = inItem[i].files;
				}
			}

			ws.Cells["A13"].Value = ResourceSetting.EE2DetailsName;
			ws.Cells["A13"].Style.Font.Bold = true;

			ws.Cells["B13"].Value = ResourceSetting.EE2DetailsParameter;
			ws.Cells["B13"].Style.Font.Bold = true;

			ws.Cells["C13"].Value = ResourceSetting.CountOfLamps;
			ws.Cells["C13"].Style.Font.Bold = true;

			ws.Cells["D13"].Value = ResourceSetting.CountOfFixtures;
			ws.Cells["D13"].Style.Font.Bold = true;
			
			ws.Cells["E13"].Value = ResourceSetting.Power;
			ws.Cells["E13"].Style.Font.Bold = true;

			ws.Cells["F13"].Value = ResourceSetting.CPRA;
			ws.Cells["F13"].Style.Font.Bold = true;

			ws.Cells["G13"].Value = ResourceSetting.AggregatePower;
			ws.Cells["G13"].Style.Font.Bold = true;

			ws.Cells["H13"].Value = ResourceSetting.AverageTariff;
			ws.Cells["H13"].Style.Font.Bold = true;

			ws.Cells["I13"].Value = ResourceSetting.WorkingHours;
			ws.Cells["I13"].Style.Font.Bold = true;

			ws.Cells["J13"].Value = ResourceSetting.MaintenanceCosts;
			ws.Cells["J13"].Style.Font.Bold = true;

			ws.Cells["K13"].Value = ResourceSetting.sComment;
			ws.Cells["K13"].Style.Font.Bold = true;

			ws.Cells["L13"].Value = ResourceSetting.Files;
			ws.Cells["L13"].Style.Font.Bold = true;


			#endregion
			//columns.Add(o => o.DicDetails_Name).Titled(ResourceSetting.EE2DetailsName).SetWidth(40);
			//columns.Add(o => o.DicDetailModels_Name).Titled(ResourceSetting.EE2DetailsParameter).SetWidth(40);
			//columns.Add(o => o.CountOfFixtures).Titled(ResourceSetting.CountOfFixtures).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.CountOfLamps).Titled(ResourceSetting.CountOfLamps).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.Power).Titled(ResourceSetting.Power).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.CPRA).Titled(ResourceSetting.CPRA).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.AggregatePower).Titled(ResourceSetting.AggregatePower).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.AverageTariff).Titled(ResourceSetting.AverageTariff).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.WorkingHours).Titled(ResourceSetting.WorkingHours).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.MaintenanceCosts).Titled(ResourceSetting.MaintenanceCosts).SetWidth(40).Css("cl-right");
			//columns.Add(o => o.Comments).Titled(ResourceSetting.sComment).SetWidth(40);
			//columns.Add().Titled(ResourceSetting.Files).Filterable(true).Sanitized(false).Encoded(false).RenderValueAs(model => Html.Raw(model.files)).SetWidth(40);

			//ws.Column(1).Width = 20;
			//ws.Column(2).Width = 50;
			//ws.Column(3).Width = 70;
			//ws.Column(4).Width = 40;
			//ws.Column(5).Width = 20;
			//ws.Column(6).Width = 60;
			//ws.Cells["A1"].Value = ResourceSetting.biniinSubject;
			//ws.Cells["A1"].Style.Font.Bold = true;
			//ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["B1"].Value = ResourceSetting.IDK;
			//ws.Cells["B1"].Style.Font.Bold = true;
			//ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["C1"].Value = ResourceSetting.SubPerson;
			//ws.Cells["C1"].Style.Font.Bold = true;
			//ws.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["D1"].Value = ResourceSetting.SendDate;
			//ws.Cells["D1"].Style.Font.Bold = true;
			//ws.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["E1"].Value = ResourceSetting.RegisterForm;
			//ws.Cells["E1"].Style.Font.Bold = true;
			//ws.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["F1"].Value = ResourceSetting.ExpectantName;
			//ws.Cells["F1"].Style.Font.Bold = true;
			//ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["G1"].Value = ResourceSetting.Oblast;
			//ws.Cells["G1"].Style.Font.Bold = true;
			//ws.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["H1"].Value = ResourceSetting.RstDicStatus;
			//ws.Cells["H1"].Style.Font.Bold = true;
			//ws.Cells["H1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);
			//ws.Cells["I1"].Value = ResourceSetting.ReportReason;
			//ws.Cells["I1"].Style.Font.Bold = true;
			//ws.Cells["I1"].Style.Border.BorderAround(ExcelBorderStyle.Thick);

			//var reportReestrFilters = filter.RstReportReestrs as RST_ReportReestrFilter[] ??
			//						  filter.RstReportReestrs.ToArray();
			//for (var i = 0; i < reportReestrFilters.Count(); i++)
			//{
			//	var index = i + 2;
			//	var reestr = reportReestrFilters[i];
			//	ws.Cells["A" + index].Value = reestr.BINIIN;
			//	ws.Cells["B" + index].Value = reestr.IDK;
			//	ws.Cells["B" + index].Style.WrapText = true;
			//	ws.Cells["C" + index].Value = reestr.OwnerName + '/' + reestr.Address;
			//	ws.Cells["C" + index].Style.WrapText = true;
			//	if (reestr.SendDate != null)
			//	{
			//		ws.Cells["D" + index].Value = reestr.SendDate.Value.ToShortDateString();
			//	}
			//	else
			//	{
			//		ws.Cells["D" + index].Value = "";
			//	}
			//	ws.Cells["D" + index].Style.WrapText = true;

			//	ws.Cells["E" + index].Value = reestr.FormStatus;
			//	ws.Cells["E" + index].Style.WrapText = true;
			//	ws.Cells["F" + index].Value = reestr.ExpectantName;
			//	ws.Cells["F" + index].Style.WrapText = true;
			//	ws.Cells["G" + index].Value = reestr.Oblast;
			//	ws.Cells["G" + index].Style.WrapText = true;
			//	ws.Cells["H" + index].Value = reestr.StatusName;
			//	ws.Cells["H" + index].Style.WrapText = true;
			//	ws.Cells["I" + index].Value = reestr.ReasonName;
			//	ws.Cells["I" + index].Style.WrapText = true;
			//}

			#endregion

			FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.FileDownloadName = "report.xlsx";
			return result;
		}

		public ActionResult ExportExcelByManager(string name, string biniin, string oblast, string status, string okeds, string sBINIIN, string Oked_Name, string Oblast_Name, string Region_Name, string orderName,string sorted)
		{
			ExcelPackage pck = new ExcelPackage();
			var ws = pck.Workbook.Worksheets.Add("Отчет");

			var oblastList = new List<string>();
			if (!string.IsNullOrEmpty(oblast))
			{
				var objectList = oblast.Split(',');
				foreach (var s in objectList)
				{
					oblastList.Add(s);
				}
			}

			//---- статус
			var statuseList = new List<string>();
			if (!string.IsNullOrEmpty(status))
			{
				var objectList = status.Split(',');
				foreach (var s in objectList)
				{
					statuseList.Add(s);
				}
			}

			//---- окэд
			var okedList = new List<string>();
			if (!string.IsNullOrEmpty(okeds))
			{
				var objectList = okeds.Split(',');
				foreach (var s in objectList)
				{
					okedList.Add(s);
				}
			}
			
			//----
			if (!string.IsNullOrEmpty(sBINIIN)) {
				biniin = sBINIIN;
			}

			List<MAP_EE2Excel> list = new List<MAP_EE2Excel>();
			string errorMessager = repository.exportExcell(ref list, name, biniin, oblastList, statuseList, okedList, Oked_Name, Oblast_Name, Region_Name,orderName,sorted);

			if (errorMessager == "")
			{
				#region excell create begin
				
				ws.Row(3).Height = 30;
				for (int i = 1; i <= 26; i++)
				{
					ws.Column(i).Width = 15;
					ws.Cells[3, i].Style.WrapText = true;
					ws.Cells[3, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					ws.Cells[3, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				}
				
				//----
				ws.Cells[2, 5, 2, 26].Style.Fill.PatternType = ExcelFillStyle.Solid;
				var color = System.Drawing.ColorTranslator.FromHtml("#BFBFBF");
				ws.Cells[2, 5, 2, 26].Style.Fill.BackgroundColor.SetColor(color);

				ws.Cells[3, 1, 3, 26].Style.Fill.PatternType = ExcelFillStyle.Solid;
				ws.Cells[3, 1, 3, 26].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

				for (int i = 0; i < 1; i++)
				{
					for (int j = 0; j < 26; j++)
					{
						ws.Cells[3 + i, 1 + j].Style.Border.BorderAround(
							ExcelBorderStyle.Thin);
					}
				}

				ws.Cells[3, 1, 3, 26].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				
				ws.Column(1).Width = 10;
				ws.Column(2).Width = 60;
				ws.Column(16).Width = 50;
				ws.Column(17).Width = 50;

				ws.Cells[2, 5, 2, 9].Merge = true;
				ws.Cells[2, 5, 2, 9].Value = "Объем потребления";
				ws.Cells[2, 5, 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				ws.Cells[2, 10, 2, 14].Merge = true;
				ws.Cells[2, 10, 2, 14].Value = "Расходы, тыс. тнг";
				ws.Cells[2, 10, 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				ws.Cells[2, 15, 2, 26].Merge = true;
				ws.Cells[2, 15, 2, 26].Value = "Данные по осветительным приборам";
				ws.Cells[2, 15, 2, 26].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				ws.Cells[2, 5, 2, 26].Style.Font.Bold = true;

				ws.Cells["G2"].Value = "Объем потребления";
				ws.Cells["L2"].Value = "Расходы, тыс. тнг";

				//----
				ws.Cells["A3"].Value = "№";

				ws.Cells["B3"].Value = "Наименование образовательного учреждения";
				ws.Cells["C3"].Value = ResourceSetting.sBIN;
				ws.Cells["D3"].Value = "Общая площадь";
				ws.Cells["E3"].Value = "Этажность";

				ws.Cells["F3"].Value = "2012 г.";

				ws.Cells["G3"].Value = "2013 г.";

				ws.Cells["H3"].Value = "2014 г.";

				ws.Cells["I3"].Value = "2015 г.";

				ws.Cells["J3"].Value = "2016 г.";

				ws.Cells["K3"].Value = "2012 г.";

				ws.Cells["L3"].Value = "2013 г.";

				ws.Cells["M3"].Value = "2014 г.";

				ws.Cells["N3"].Value = "2015 г.";

				ws.Cells["O3"].Value = "2016 г.";

				//----
				ws.Cells["P3"].Value = ResourceSetting.EE2DetailsName;
				ws.Cells["Q3"].Value = ResourceSetting.EE2DetailsParameter;
				ws.Cells["R3"].Value = ResourceSetting.CountOfLamps;
				ws.Cells["S3"].Value = ResourceSetting.CountOfFixtures;
				ws.Cells["T3"].Value = ResourceSetting.Power;
				ws.Cells["U3"].Value = ResourceSetting.CPRA;
				ws.Cells["V3"].Value = ResourceSetting.AggregatePower;
				ws.Cells["W3"].Value = ResourceSetting.AverageTariff;
				ws.Cells["X3"].Value = ResourceSetting.WorkingHours;
				ws.Cells["Y3"].Value = ResourceSetting.MaintenanceCosts;
				ws.Cells["Z3"].Value = ResourceSetting.sComment;

				#region fill excell
				var allcount = 3;
				int index = 0;
				foreach (var item in list) {
					index++;
					allcount = allcount + 1;
							
					for (int j = 0; j < 26; j++)
					{
						ws.Cells[allcount, 1 + j].Style.Border.BorderAround(
							ExcelBorderStyle.Thin);
					}

					ws.Cells[allcount, 1, allcount, 26].Style.Border.Top.Style = ExcelBorderStyle.Medium;

					ws.Cells[allcount, 1].Value = index;
					ws.Cells[allcount, 2].Value = item.JuridicalName;
					ws.Cells[allcount, 3].Value = item.BINIIN;

					//----
					ws.Cells[allcount, 4].Value = item.TotalArea;
					ws.Cells[allcount, 5].Value = item.NumberOfStoreys;

					//----
					if (item.TotalArea != null)
					{
						var yearArr = item.ReportYear.Split(';');
						var energyArr = item.EnergySource.Split(';');
						var expenceArr = item.ExpenceEnergy.Split(';');

						for (int i = 0; i < 5; i++)
						{
							ws.Cells[allcount, 6 + i].Value = energyArr[i];// recordItem.EnergySource;
							ws.Cells[allcount, 11 + i].Value = expenceArr[i];//recordItem.ExpenceEnergy;
						}
					}

					//----
					if (item.CNT != null) {

						int? cnt = item.CNT;
						var DetailsArr=item.EEDetailsName.Split(';');
						var DetailModelsArr=item.EEDetailModelsName.Split(';');
						var cFixturesArr = item.CountOfFixtures.Split(';');
						var cOfLampsArr = item.CountOfLamps.Split(';');
						var PowerArr = item.Power.Split(';');
						var CpraArr = item.CPRA.Split(';');
						var AggPowerArr = item.AggregatePower.Split(';');
						var AveTariffArr = item.AverageTariff.Split(';');
						var WorkHoursArr = item.WorkingHours.Split(';');
						var MainCostsArr = item.MaintenanceCosts.Split(';');
						var CommentsArr = item.Comments.Split(';');

						Array.Reverse(DetailsArr);
						Array.Reverse(DetailModelsArr);
						Array.Reverse(cFixturesArr);
						Array.Reverse(cOfLampsArr);
						Array.Reverse(PowerArr);
						Array.Reverse(CpraArr);
						Array.Reverse(AggPowerArr);
						Array.Reverse(AveTariffArr);
						Array.Reverse(WorkHoursArr);
						Array.Reverse(MainCostsArr);
						Array.Reverse(CommentsArr);

						for (int i = 0; i < cnt; i++)
						{
							try
							{
								for (int j = 0; j < 26; j++)
								{
									ws.Cells[allcount, 1 + j].Style.Border.BorderAround(
										ExcelBorderStyle.Thin);
								}

								if (i == 0)
								{
									ws.Cells[allcount, 1, allcount, 26].Style.Border.Top.Style = ExcelBorderStyle.Medium;
								}

								ws.Cells[allcount, 2].Value = item.JuridicalName;
								ws.Cells[allcount, 16].Value = DetailsArr[i];
								ws.Cells[allcount, 17].Value = DetailModelsArr[i];
								ws.Cells[allcount, 18].Value = (cFixturesArr[i] != "0") ? Convert.ToDouble(cFixturesArr[i].Replace('.', ',')) : (double?)null;

								ws.Cells[allcount, 19].Value = (cOfLampsArr[i] != "0") ? Convert.ToDouble(cOfLampsArr[i].Replace('.', ',')) : (double?)null;// inItem[i].CountOfLamps;
								ws.Cells[allcount, 20].Value = (PowerArr[i] != "0") ? Convert.ToDouble(PowerArr[i].Replace('.', ',')) : (double?)null; //inItem[i].Power;
								ws.Cells[allcount, 21].Value = (CpraArr[i] != "null") ? CpraArr[i] : ""; //inItem[i].CPRA;
								ws.Cells[allcount, 22].Value = (AggPowerArr[i] != "0") ? Convert.ToDouble(AggPowerArr[i].Replace('.', ',')) : (double?)null;// inItem[i].AggregatePower;
								ws.Cells[allcount, 23].Value = (AveTariffArr[i] != "0") ? Convert.ToDouble(AveTariffArr[i].Replace('.', ',')) : (double?)null; //inItem[i].AverageTariff;

								try
								{
									ws.Cells[allcount, 24].Value = (WorkHoursArr[i] != "null") ? Convert.ToDouble(WorkHoursArr[i].Replace('.', ',')) : (double?)null; //inItem[i].WorkingHours;
								}
								catch (Exception ex)
								{
									ws.Cells[allcount, 24].Value = WorkHoursArr[i];
								}

								ws.Cells[allcount, 25].Value = (MainCostsArr[i] != "null") ? Convert.ToDouble(MainCostsArr[i].Replace('.', ',')) : (double?)null; //inItem[i].MaintenanceCosts;
								ws.Cells[allcount, 26].Value = (CommentsArr[i] != "null") ? CommentsArr[i] : "";
							}
							catch (Exception ex)
							{
								ws.Cells[allcount, 16].Value = ex.Message;
							}

							allcount++;
						}

						allcount=allcount - 1;
					}
					
				}
				
				


				//if (model != null && model.ListItems.Count() > 0)
				//{
				//	int index = 0;
				//	foreach (var item in model.ListItems)
				//	{

				//		index++;

				//		allcount = allcount + 1;
				//		//ws.Cells[allcount, 1, allcount, 24].Style.Border.Top.Style = ExcelBorderStyle.Thick;
				//		ws.Cells[allcount, 1].Value = index;
				//		ws.Cells[allcount, 2].Value = item.JuridicalName;

				//		MAP_EE2Filters inModel = getModelHelper(item.SecUserId);
				//		if (inModel.Id != -1)
				//		{

				//			ws.Cells[allcount, 3].Value = inModel.TotalArea;
				//			ws.Cells[allcount, 4].Value = inModel.NumberOfStoreys;

				//			#region----2012 ... 2016 г
				//			List<Map_FormEE2Record> recordList = new List<Map_FormEE2Record>();
				//			string errorMessages = repository.getFormEE2RecordItemBySecUserId(ref recordList, item.SecUserId);
				//			if (errorMessages == "")
				//			{
				//				for (int i = 0; i < 5; i++)
				//				{
				//					int year = 2012 + i;
				//					var recordItem = recordList.Where(x => x.ReportYear == year).FirstOrDefault();
				//					ws.Cells[allcount, 5 + i].Value = recordItem.EnergySource;
				//					ws.Cells[allcount, 10 + i].Value = recordItem.ExpenceEnergy;
				//				}
				//			}
				//			#endregion

				//			List<MAP_ApplicationEE2Info> inItem = inModel.MapApplicationEE2List.ToList();

				//			for (int i = 0; i < inItem.Count; i++)
				//			{

				//				for (int j = 0; j < 24; j++)
				//				{
				//					ws.Cells[allcount, 1 + j].Style.Border.BorderAround(
				//						ExcelBorderStyle.Thin);
				//				}

				//				ws.Cells[allcount, 2].Value = item.JuridicalName;
				//				ws.Cells[allcount, 15].Value = inItem[i].DicDetails_Name;
				//				ws.Cells[allcount, 16].Value = inItem[i].DicDetailModels_Name;
				//				ws.Cells[allcount, 17].Value = inItem[i].CountOfFixtures;
				//				ws.Cells[allcount, 18].Value = inItem[i].CountOfLamps;
				//				ws.Cells[allcount, 19].Value = inItem[i].Power;
				//				ws.Cells[allcount, 20].Value = inItem[i].CPRA;
				//				ws.Cells[allcount, 21].Value = inItem[i].AggregatePower;
				//				ws.Cells[allcount, 22].Value = inItem[i].AverageTariff;
				//				ws.Cells[allcount, 23].Value = inItem[i].WorkingHours;
				//				ws.Cells[allcount, 24].Value = inItem[i].MaintenanceCosts;

				//				allcount++;
				//			}

				//			ws.Cells[(allcount - inItem.Count), 1, (allcount - inItem.Count), 24].Style.Border.Top.Style = ExcelBorderStyle.Medium;

				//			allcount = allcount - 1;
				//		}
				//		else
				//		{
				//			for (int j = 0; j < 24; j++)
				//			{
				//				ws.Cells[allcount, 1 + j].Style.Border.BorderAround(
				//					ExcelBorderStyle.Thin);
				//			}
				//			ws.Cells[allcount, 1, allcount, 24].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				//		}

				//	}
				//}



				#endregion

				#endregion
			
			}
			FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.FileDownloadName = "report.xlsx";
			return result;
		}

		public ActionResult ExportExcelByManager1(string name, string biniin, string oblast, string status, string okeds)
		{
			MAP_mEE2Filters model = getModelHelperByManager(name, biniin, oblast, status, okeds);

			#region excell create begin
			ExcelPackage pck = new ExcelPackage();
			var ws = pck.Workbook.Worksheets.Add("Отчет");

			ws.Row(3).Height = 30;


			for (int i = 1; i <= 24; i++)
			{
				ws.Column(i).Width = 15;
				ws.Cells[3, i].Style.WrapText = true;
				ws.Cells[3, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				ws.Cells[3, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			}


			//----
			ws.Cells[2, 5, 2, 24].Style.Fill.PatternType = ExcelFillStyle.Solid;
			var color = System.Drawing.ColorTranslator.FromHtml("#BFBFBF");
			ws.Cells[2, 5, 2, 24].Style.Fill.BackgroundColor.SetColor(color);

			ws.Cells[3, 1, 3, 24].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[3, 1, 3, 24].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

			for (int i = 0; i < 1; i++)
			{
				for (int j = 0; j < 24; j++)
				{
					ws.Cells[3 + i, 1 + j].Style.Border.BorderAround(
						ExcelBorderStyle.Thin);
				}
			}

			ws.Cells[3, 1, 3, 24].Style.Border.Top.Style = ExcelBorderStyle.Medium;


			ws.Column(1).Width = 10;
			ws.Column(2).Width = 60;
			ws.Column(15).Width = 50;
			ws.Column(16).Width = 50;

			ws.Cells[2, 5, 2, 9].Merge = true;
			ws.Cells[2, 5, 2, 9].Value = "Объем потребления";
			ws.Cells[2, 5, 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			ws.Cells[2, 10, 2, 14].Merge = true;
			ws.Cells[2, 10, 2, 14].Value = "Расходы, тыс. тнг";
			ws.Cells[2, 10, 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			ws.Cells[2, 15, 2, 24].Merge = true;
			ws.Cells[2, 15, 2, 24].Value = "Данные по осветительным приборам";
			ws.Cells[2, 15, 2, 24].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			ws.Cells[2, 5, 2, 24].Style.Font.Bold = true;

			ws.Cells["G2"].Value = "Объем потребления";
			ws.Cells["L2"].Value = "Расходы, тыс. тнг";

			//----
			ws.Cells["A3"].Value = "№";

			ws.Cells["B3"].Value = "Наименование образовательного учреждения";

			ws.Cells["C3"].Value = "Общая площадь";

			ws.Cells["D3"].Value = "Этажность";

			ws.Cells["E3"].Value = "2012 г.";

			ws.Cells["F3"].Value = "2013 г.";

			ws.Cells["G3"].Value = "2014 г.";

			ws.Cells["H3"].Value = "2015 г.";

			ws.Cells["I3"].Value = "2016 г.";

			ws.Cells["J3"].Value = "2012 г.";

			ws.Cells["K3"].Value = "2013 г.";

			ws.Cells["L3"].Value = "2014 г.";

			ws.Cells["M3"].Value = "2015 г.";

			ws.Cells["N3"].Value = "2016 г.";

			//----
			ws.Cells["O3"].Value = ResourceSetting.EE2DetailsName;

			ws.Cells["P3"].Value = ResourceSetting.EE2DetailsParameter;

			ws.Cells["Q3"].Value = ResourceSetting.CountOfLamps;
			ws.Cells["R3"].Value = ResourceSetting.CountOfFixtures;

			ws.Cells["S3"].Value = ResourceSetting.Power;

			ws.Cells["T3"].Value = ResourceSetting.CPRA;

			ws.Cells["U3"].Value = ResourceSetting.AggregatePower;

			ws.Cells["V3"].Value = ResourceSetting.AverageTariff;

			ws.Cells["W3"].Value = ResourceSetting.WorkingHours;

			ws.Cells["X3"].Value = ResourceSetting.MaintenanceCosts;



			#region fill excell
			var allcount = 3;
			if (model != null && model.ListItems.Count() > 0)
			{
				int index = 0;
				foreach (var item in model.ListItems)
				{

					index++;

					allcount = allcount + 1;
					//ws.Cells[allcount, 1, allcount, 24].Style.Border.Top.Style = ExcelBorderStyle.Thick;
					ws.Cells[allcount, 1].Value = index;
					ws.Cells[allcount, 2].Value = item.JuridicalName;

					MAP_EE2Filters inModel = getModelHelper(item.SecUserId);
					if (inModel.Id != -1)
					{

						ws.Cells[allcount, 3].Value = inModel.TotalArea;
						ws.Cells[allcount, 4].Value = inModel.NumberOfStoreys;

						#region----2012 ... 2016 г
						List<Map_FormEE2Record> recordList = new List<Map_FormEE2Record>();
						string errorMessages = repository.getFormEE2RecordItemBySecUserId(ref recordList, item.SecUserId);
						if (errorMessages == "")
						{
							for (int i = 0; i < 5; i++)
							{
								int year = 2012 + i;
								var recordItem = recordList.Where(x => x.ReportYear == year).FirstOrDefault();
								ws.Cells[allcount, 5 + i].Value = recordItem.EnergySource;
								ws.Cells[allcount, 10 + i].Value = recordItem.ExpenceEnergy;
							}
						}
						#endregion

						List<MAP_ApplicationEE2Info> inItem = inModel.MapApplicationEE2List.ToList();

						for (int i = 0; i < inItem.Count; i++)
						{

							for (int j = 0; j < 24; j++)
							{
								ws.Cells[allcount, 1 + j].Style.Border.BorderAround(
									ExcelBorderStyle.Thin);
							}

							ws.Cells[allcount, 2].Value = item.JuridicalName;
							ws.Cells[allcount, 15].Value = inItem[i].DicDetails_Name;
							ws.Cells[allcount, 16].Value = inItem[i].DicDetailModels_Name;
							ws.Cells[allcount, 17].Value = inItem[i].CountOfFixtures;
							ws.Cells[allcount, 18].Value = inItem[i].CountOfLamps;
							ws.Cells[allcount, 19].Value = inItem[i].Power;
							ws.Cells[allcount, 20].Value = inItem[i].CPRA;
							ws.Cells[allcount, 21].Value = inItem[i].AggregatePower;
							ws.Cells[allcount, 22].Value = inItem[i].AverageTariff;
							ws.Cells[allcount, 23].Value = inItem[i].WorkingHours;
							ws.Cells[allcount, 24].Value = inItem[i].MaintenanceCosts;

							allcount++;
						}

						ws.Cells[(allcount - inItem.Count), 1, (allcount - inItem.Count), 24].Style.Border.Top.Style = ExcelBorderStyle.Medium;

						allcount = allcount - 1;
					}
					else
					{
						for (int j = 0; j < 24; j++)
						{
							ws.Cells[allcount, 1 + j].Style.Border.BorderAround(
								ExcelBorderStyle.Thin);
						}
						ws.Cells[allcount, 1, allcount, 24].Style.Border.Top.Style = ExcelBorderStyle.Medium;
					}

				}
			}



			#endregion

			#endregion

			FileContentResult result = new FileContentResult(pck.GetAsByteArray(),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.FileDownloadName = "report.xlsx";
			return result;
		}
    }
}
