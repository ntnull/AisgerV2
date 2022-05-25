using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Aisger.Controllers.Security
{
    public class JurEventController : ACommonController
    {
       private string lang = CultureHelper.GetCurrentCulture();

        //[GerNavigateLogger]
        public ActionResult Index()
        {
            var repository = new SecJurnalEventRepository();
            return View(repository.GetAll());
        }

        [HttpPost]
        public JsonResult GetEvenTypeNames()
        {
            var repository = new SecDicEventTypeRepository();
            var allItems = repository.GetAll().Select(c => c.NameRu);
            return Json(new { Items = allItems });
        }

        [HttpGet]
        public ActionResult ShowJurEvent(long id)
        {
            var repository = new SecJurnalEventRepository();
            SEC_JurEvent order = repository.GetById(id);
            return View(order);
        }

        //[GerNavigateLogger]
        public ActionResult UserAuditIndex(
            int limit = 10,
            int offset = 0,
            string eventType = "0",
            string user = "",
            string period = "1",
            string from = "_",
            string to = "_",
            string table = "all",
            string control = "all",
            string page = "all",
            string sort = "date_desc")
        {
            if (offset < 0)
            {
                offset = 0;
            }

            Dictionary<string, double> laps = new Dictionary<string, double>();
            DateTime tick = DateTime.Now;
            laps.Add("start", 0);

            int count = 0;
            IList<UA> data = UA.GetList(limit, offset, eventType, user, period, from, to, table, control, page, sort, out count);
            laps.Add("UA.GetList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            var li = UA.EventTypeList(lang);
            List<SelectListItem> etList = new List<SelectListItem>();
            foreach (SelectListItem item in li)
            {
                item.Selected = (item.Value == eventType);
                etList.Add(item);
            }
            SelectList etSList = new SelectList(etList, "Value", "Text", eventType);
            ViewBag.EventTypeList = etSList;
            laps.Add("UA.EventTypeList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            List<SelectListItem> userList = new List<SelectListItem>();
            foreach (SelectListItem item in UA.UserList(lang))
            {
                item.Selected = (item.Value == user);
                userList.Add(item);
            }
            SelectList userSList = new SelectList(userList, "Value", "Text", user);
            ViewBag.UserList = userSList;
            laps.Add("UA.UserList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            ViewBag.PeriodList = new SelectList(new List<SelectListItem> {
                new SelectListItem {Value = "0", Text = (lang=="kz")?"Барлық уақытта":"За всё время", Selected = (period == "0")},
                new SelectListItem {Value = "1", Text = (lang=="kz")?"Бүгін":"За сегодня", Selected = (period == "1")},
                new SelectListItem {Value = "2", Text = (lang=="kz")?"Кеше":"За вчера", Selected = (period == "2")},
                new SelectListItem {Value = "3", Text = (lang=="kz")?"7 күнде":"За 7 дней", Selected = (period == "3")},
                new SelectListItem {Value = "4", Text = (lang=="kz")?"Айда":"За месяц", Selected = (period == "4") },
                new SelectListItem {Value = "10", Text = (lang=="kz")?"Кездейсоқ аралық":"Произвольный интервал", Selected = (period == "10")}
            }, "Value", "Text", period);
            laps.Add("PeriodList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            List<SelectListItem> tableList = new List<SelectListItem>();
            foreach (SelectListItem item in UA.TableList(lang))
            {
                item.Selected = (item.Value == table);
                tableList.Add(item);
            }
            SelectList tableSList = new SelectList(tableList, "Value", "Text", table);
            ViewBag.TableList = tableSList;
            ViewBag.TableGroupList = UA.TableGroupList;
            laps.Add("UA.TableList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            List<SelectListItem> controllerList = new List<SelectListItem>();
            foreach (SelectListItem item in UA.ControllerList(lang))
            {
                item.Selected = (item.Value == control);
                controllerList.Add(item);
            }
            SelectList controllerSList = new SelectList(controllerList, "Value", "Text", control);
            ViewBag.ControllerList = controllerSList;
            laps.Add("UA.ControllerList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            List<SelectListItem> actionList = new List<SelectListItem>();
            foreach (SelectListItem item in UA.GetActionList(control,lang))
            {
                item.Selected = (item.Value == page);
                actionList.Add(item);
            }
            SelectList actionSList = new SelectList(actionList, "Value", "Text", page);
            ViewBag.ActionList = actionSList;
            laps.Add("UA.GetActionList", (DateTime.Now - tick).TotalSeconds);
            tick = DateTime.Now;

            ViewBag.SortList = new SelectList(new List<SelectListItem> {
                new SelectListItem {Value = "date_desc", Text =(lang=="kz")?"Бірінші жаңалары":"Сначала новые", Selected = (sort == "date_desc")},
                new SelectListItem {Value = "date_asc", Text = (lang=="kz")?"Бірінші ескілері":"Сначала старые", Selected = (sort == "date_asc")}
            }, "Value", "Text", sort);

            ViewBag.Limit = limit;
            ViewBag.OffSet = offset;
            ViewBag.Сount = count;
            ViewBag.CurrentLog = (lang == "kz") ? "Көрсетілгені " + (offset + 1).ToString("N0") + " - " + (offset + limit).ToString("N0") + " барлығы " + count.ToString("N0")
                : "Показано с " + (offset + 1).ToString("N0") + " по " + (offset + limit).ToString("N0") + " из " + count.ToString("N0");
            ViewBag.EventType = eventType;
            ViewBag.User = user;
            ViewBag.Period = period;
            ViewBag.FromDate = from.Split('_')[0];
            ViewBag.FromTime = from.Split('_')[1];
            ViewBag.ToDate = to.Split('_')[0];
            ViewBag.ToTime = to.Split('_')[1];
            ViewBag.Table = table;
            ViewBag.Control = control;
            ViewBag.Page = page;
            ViewBag.Sort = sort;

            string urlPrev = "/JurEvent/UserAuditIndex/?offset=";
            string urlNext = urlPrev;

            if ((offset - limit) < 0)
            {
                urlPrev += "0";
                urlNext += limit.ToString();
            }
            else
            {
                urlPrev += (offset - limit).ToString();
                urlNext += (offset + limit).ToString();
            }
            if (limit != 10)
            {
                urlPrev += "&limit=" + limit;
                urlNext += "&limit=" + limit;
            }
            if (user != "all")
            {
                urlPrev += "&user=" + user;
                urlNext += "&user=" + user;
            }
            if (period != "0")
            {
                urlPrev += "&period=" + period;
                urlNext += "&period=" + period;
                if (period == "10")
                {
                    urlPrev += "&from=" + from + "&to=" + to;
                    urlNext += "&from=" + from + "&to=" + to;
                }
            }
            if (eventType != "0")
            {
                urlPrev += "&eventType=" + eventType;
                urlNext += "&eventType=" + eventType;
                if (eventType != "10")
                {
                    urlPrev += "&table=" + table;
                    urlNext += "&table=" + table;
                }
                else
                {
                    if (control != "all")
                    {
                        urlPrev += "&control=" + control;
                        urlNext += "&control=" + control;
                        if (page != "all")
                        {
                            urlPrev += "&page=" + page;
                            urlNext += "&page=" + page;
                        }
                    }
                }
            }

            urlPrev += "&sort=" + sort;

            ViewBag.UrlPrev = urlPrev;
            ViewBag.UrlNext = urlNext;

            laps.Add("finish", (DateTime.Now - tick).TotalSeconds);
            ViewBag.TimeLine = laps;

            return View(data);
        }

        public JsonResult GetData(string id, string tableName)
        {
            string result = "";

            var serializer = new JavaScriptSerializer();
            Dictionary<string, string> oldData = null;
            Dictionary<string, string> newData = null;
            Dictionary<string, string> colData = null;

            string val = UA.GetData(id, true);
            if (val != "")
            {
                oldData = serializer.Deserialize<Dictionary<string, string>>(val);
                colData = serializer.Deserialize<Dictionary<string, string>>(val);
            }

            val = UA.GetData(id, false);
            if (val != "")
            {
                newData = serializer.Deserialize<Dictionary<string, string>>(val);
                colData = serializer.Deserialize<Dictionary<string, string>>(val);
            }

            Dictionary<string, KeyValuePair<string, bool>> columns = UA.GetColumns(tableName);
            for (int i = 0; i < colData.Count; i++)
            {
                string style = "";
                if (oldData != null)
                {
                    if (newData != null)
                    {
                        if (oldData.Values.ElementAt(i) != newData.Values.ElementAt(i))
                        {
                            style = @" style=""background-color:lightpink""";
                        }
                    }
                }

                string columnName = colData.Keys.ElementAt(i);

                if (columns.Count != 0)
                {
                    columnName = columns.First(p => p.Key == columnName.ToUpper()).Value.Key;
                }
                result += "<tr" + style + ">";
                result += "<td>" + columnName + "</td>";
                if (oldData != null)
                {
                    result += @"<td style=""word-wrap: break-word"">" + oldData.Values.ElementAt(i) + "</td>";
                }
                else
                {
                    result += "<td></td>";
                }
                if (newData != null)
                {
                    result += @"<td style=""word-wrap: break-word"">" + newData.Values.ElementAt(i) + "</td>";
                }
                else
                {
                    result += "<td></td>";
                }
                result += "</tr>";
            }

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetActionList(string controller)
        {
            return new JsonResult { Data = UA.GetActionList(controller,lang), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}