using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Entity.Collector;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.EnergyAudit;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Collector
{
    public class CollectorController : Controller
    {
        private const string CurrentDataConst = "CurrentDataConst";

        [GerNavigateLogger]
        public ActionResult Index()
        {
            var repository = new CollectorRepository();
            var cmdevices =  repository.GetCmdevicesList();
            return View(cmdevices);
        }

        public ActionResult ShowDetails(long id)
        {
            throw new NotImplementedException();
        }

        [GerNavigateLogger]
        public ActionResult ShowIndicatorValues(string code)
        {
            var repository = new CollectorRepository();
            var device = repository.GetCmdevice(code);

            return View(device);
        }

        public ActionResult DrawColumnChart(string cmdCode, DateTime startPeriod, DateTime endPeriod)
        {
            var repository = new CollectorRepository();
            var device = repository.GetCmdevice(cmdCode);

            var valuesPerDay = repository.GetDataPerDay(cmdCode, startPeriod, endPeriod);
            var seriesList = new List<Series>()
            {
                new Series()
                {
                    name = ResourceSetting.Value,
                    colorByPoint = false
                }
            };
            var categories = new List<DateTime>();
            foreach (var pdayView in valuesPerDay.OrderBy(ppd => ppd.ReportDate))
            {
                if (pdayView.ReportDate.HasValue && !categories.Contains(pdayView.ReportDate.Value))
                {
                    categories.Add(pdayView.ReportDate.Value);

                    var view = pdayView;
                    var curDayValues = valuesPerDay.Where(p => p.ReportDate == view.ReportDate);
                    Double value = 0;

                    foreach (var cdv in curDayValues)
                    {
                        value += (cdv.LastValue.HasValue ? cdv.LastValue.Value : 0)
                            - (cdv.FirstValue.HasValue ? cdv.FirstValue.Value : 0);
                    }
                    seriesList.FirstOrDefault(s => s.name == ResourceSetting.Value).data.Add(value);
                }
            }

            var chartModel = new HighchartSettings
            {
                // chartTitle = string.Empty,
                // chartSubtitle = string.Empty,
                yTitle = device.DIC_Unit == null ? ResourceSetting.units : device.DIC_Unit.NameRu,
                categories = categories.Select(cat => cat.ToShortDateString()).ToArray(),
                series = seriesList.ToArray()
            };

            return new JsonResult()
            {
                Data = chartModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult DrawPieChart(string cmdCode, DateTime startPeriod, DateTime endPeriod)
        {
            var repository = new CollectorRepository();
            var device = repository.GetCmdevice(cmdCode);
            var valuesPerDay = repository.GetDataPerDay(cmdCode, startPeriod, endPeriod);

            var seriesList = new List<Series>();

            double value = 0;
            foreach (var pdayView in valuesPerDay.OrderBy(ppd => ppd.ReportDate))
            {
                value += ((pdayView.LastValue.HasValue ? pdayView.LastValue.Value : 0) - (pdayView.FirstValue.HasValue ? pdayView.FirstValue.Value : 0));
            }

            var series = new Series
            {
                name = device.NameRu,
            };
            series.data.Add(new DataObject()
            {
                name = ResourceSetting.Total,
                y = value,
                color = "#90ee7e"
            });
            seriesList.Add(series);



            var chartModel = new PieGraphParameter();
            chartModel.StatData.Add(new StatData()
            {
                Title = ResourceSetting.Total,
                Value = value.ToString("F"),
                ValuePercent = "100%"
            });

            chartModel.HighchartSettings = new HighchartSettings
            {
                // chartTitle = string.Empty,
                // chartSubtitle = string.Empty,
                additionalValue = value.ToString("F"),
                series = seriesList.ToArray(),
            };

            return new JsonResult()
            {
                Data = chartModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// Инициализация графика в режиме реального времени
        /// </summary>
        /// <returns>настройки и данные для графика</returns>
        public ActionResult DrawRealTimeChart(string cmdCode)
        {
            var repository = new CollectorRepository();
            var device = repository.GetCmdevice(cmdCode);

            var valuesPerDay = repository.GetDataForLastPeriod(cmdCode, 30).OrderBy(iv => iv.DatetimeStamp).ToList();

            var parameter = new RealtimeGraphParameter();
            if (valuesPerDay != null)
            {
                var seriesList = new List<Series>();
                seriesList.Add(new Series()
                {
                    name = ResourceSetting.sReadings,
                    data = new List<object>(),
                    unit = device.DIC_Unit == null ? ResourceSetting.unit : device.DIC_Unit.NameRu
                });

                foreach (var value in valuesPerDay)
                {
                    seriesList.FirstOrDefault(ds => ds.name == ResourceSetting.sReadings).data.Add(new object[] { value.DatetimeStamp.ToLongTimeString(), value.Value });
                }

                var chartModel = new HighchartSettings
                {
                    series = seriesList.ToArray(),
                    categories = valuesPerDay.Select(p => p.DatetimeStamp.ToLongTimeString()).Cast<object>().ToArray(),
                };
                parameter.HighchartSettings = chartModel;
            }

            return new JsonResult()
            {
                Data = parameter,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        
        /// <summary>
        /// ПОлучение последних данных для графика и схемы
        /// </summary>
        /// <param name="cmdCode">код линии</param>
        /// <returns>данные для графика</returns>
        public ActionResult GetDataForRealTimeChart(string cmdCode)
        {
            var repository = new CollectorRepository();
            var device = repository.GetCmdevice(cmdCode);
            var values = repository.GetDataForLastPeriod(cmdCode, 1);
            DateTime? currentDataDatetime = null;

            var rlData = new ReatimeGraphData();
            if (values != null && values.Any())
            {
                var value = values.FirstOrDefault();
                currentDataDatetime = value.DatetimeStamp;

                rlData.Datasets.Add(new Dataset()
                {
                    name = ResourceSetting.sReadings,
                    xdata = value.DatetimeStamp.ToLongTimeString(),
                    ydata = value.Value,
                    unit = device.DIC_Unit == null ? ResourceSetting.units : device.DIC_Unit.NameRu
                });
            }

            var jsonResult = new JsonResult()
            {
                Data = rlData,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            // dont send same data
            if (Session[CurrentDataConst] == null)
                Session[CurrentDataConst] = currentDataDatetime;
            else
            {
                var dtStamp = (DateTime?)Session[CurrentDataConst];
                if (dtStamp.Value >= currentDataDatetime)
                    jsonResult.Data = null;
            }

            return jsonResult;
        }

        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult Create()
        {
            var repository = new CollectorRepository();

            var device = new COLLECTOR_Cmdevice();
            device.IndicatorTypeList = repository.GetIndicatorTypeList();

            var unitRepository = new DicUnitRepository();
            device.DicUnitList = unitRepository.GetSelectList();
            return View(device);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            var repository = new CollectorRepository();
            var device = repository.GetById(id);
            device.IndicatorTypeList = repository.GetIndicatorTypeList();
            var unitRepository = new DicUnitRepository();
            device.DicUnitList = unitRepository.GetSelectList();
            return View("Create", device);
        }
  
        public ActionResult Delete(long id)
        {
            var repository = new CollectorRepository();
            repository.Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(COLLECTOR_Cmdevice device)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var repository = new CollectorRepository();
                if (device.Id == 0)
                {
                    var eco = repository.GetQuery().FirstOrDefault(e => e.Code == device.Code);
                    if (eco != null)
                    {
                        ModelState.AddModelError("ExistNumber", ResourceSetting.existCode);

                        return View("Create", device);
                    }  
                }
                repository.SaveOrUpdate(device, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View("Create", device);
        }
    }
}