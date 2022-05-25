using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using Aisger.Utils;

namespace Aisger.Models.Repository.Dictionary
{
    public class DicHolidaysRepository : SqlRepository
    {
        public List<DicHolidayYear> GetList()
        {
            var list= AppContext.DIC_Holidays.OrderBy(e => e.RegDate).Select(e => e.RegDate.Year).Distinct().ToList();
            var holidayes = new List<DicHolidayYear>();
            foreach (var i in list)
            {
                var entity = new DicHolidayYear();
                entity.Year = i;
                entity.HolidaysCount = AppContext.DIC_Holidays.Count(e => e.RegDate.Year == i && !e.IsWorkDay);
                entity.WorkCount = AppContext.DIC_Holidays.Count(e => e.RegDate.Year == i && e.IsWorkDay);
                holidayes.Add(entity);
            }
            return holidayes;
        }
        public int CreateReportYear()
        {

            if (!AppContext.DIC_Holidays.Any())
            {
                return DateTime.Now.Year;
            }
            var max = AppContext.DIC_Holidays.Max(e => e.RegDate.Year);
            
            return max +1;
        }

        public DicHolidayEntity GetByYear(int year)
        {
            var entity = new DicHolidayEntity();
            entity.Year = year;
            entity.DicHolidayses=new List<DIC_Holidays>();
            entity.DicWorkes=new List<DIC_Holidays>();
            var list = AppContext.DIC_Holidays.Where(e => e.RegDate.Year == year).OrderBy(e=>e.RegDate);
            
            foreach (var dicHolidayse in list)
            {
                dicHolidayse.RegDateStr = dicHolidayse.RegDate.ToString("dd/MM", CultureInfo.InvariantCulture);
                if (dicHolidayse.IsWorkDay)
                {
                    entity.DicWorkes.Add(dicHolidayse);
                }
                else
                {
                    entity.DicHolidayses.Add(dicHolidayse);
                }
            }
            return entity;
        }

        public void SaveOrUpdate(DicHolidayEntity model, long? getCurrentUserId)
        {
            if (!AppContext.DIC_Holidays.Any())
            {
                foreach (var dicHolidayse in model.DicHolidayses)
                {
                    var dateStr = dicHolidayse.RegDateStr + "/" + model.Year;
                    var date = DateHelper.GetDate(dateStr);
                    if (date != null)
                    {
                        dicHolidayse.IsWorkDay = false;
                        dicHolidayse.RegDate = date.Value;
                        AppContext.DIC_Holidays.Add(dicHolidayse);
                    }
                }
                foreach (var dicHolidayse in model.DicWorkes)
                {
                    var dateStr = dicHolidayse.RegDateStr + "/" + model.Year;
                    var date = DateHelper.GetDate(dateStr);
                    if (date != null)
                    {
                        dicHolidayse.IsWorkDay = true;
                        dicHolidayse.RegDate = date.Value;
                        AppContext.DIC_Holidays.Add(dicHolidayse);
                    }
                }
                AppContext.SaveChanges();
                return;
            }
            var list = AppContext.DIC_Holidays.Where(e => e.RegDate.Year == model.Year);
            var holidays = list.Where(e => !e.IsWorkDay).Select(e => e.RegDate);
            var daysHoliday = new List<DateTime>();
            var workdays = list.Where(e => e.IsWorkDay).Select(e => e.RegDate);
            var daysWorks = new List<DateTime>();

            foreach (var dicHolidayse in model.DicHolidayses)
            {
                var dateStr = dicHolidayse.RegDateStr + "/" + model.Year;
                var date = DateHelper.GetDate(dateStr);
                if (date != null && !holidays.Contains(date.Value))
                {
                    dicHolidayse.IsWorkDay = false;
                    dicHolidayse.RegDate = date.Value;
                    AppContext.DIC_Holidays.Add(dicHolidayse);
                }
                if (date != null) daysHoliday.Add(date.Value);
            }
            var holidayDeleteList = list.Where(e => !daysHoliday.Contains(e.RegDate) && !e.IsWorkDay);
            foreach (var entity in holidayDeleteList)
            {
                AppContext.DIC_Holidays.Remove(entity);
            }

            foreach (var dicHolidayse in model.DicWorkes)
            {
                var dateStr = dicHolidayse.RegDateStr + "/" + model.Year;
                var date = DateHelper.GetDate(dateStr);
                if (date != null && !workdays.Contains(date.Value))
                {
                    dicHolidayse.IsWorkDay = true;
                    dicHolidayse.RegDate = date.Value;
                    AppContext.DIC_Holidays.Add(dicHolidayse);
                }
                if (date != null) daysWorks.Add(date.Value);
            }
            var workDeleteList = list.Where(e => !daysWorks.Contains(e.RegDate) && e.IsWorkDay);
            foreach (var entity in workDeleteList)
            {
                AppContext.DIC_Holidays.Remove(entity);
            }
            AppContext.SaveChanges();

        }

        public int CountDate(DateTime begin, DateTime end)
        {
            var list =AppContext.DIC_Holidays.Where(e => e.RegDate >= begin && e.RegDate <= end);
            var index = 0;
            foreach (var dicHolidayse in list)
            {
                if (dicHolidayse.IsWorkDay)
                {
                    index--;
                }
                else
                {
                    index++;
                }
            }
            return index;
        }

        public bool GetInfoReportYear(int year)
        {
            if (!AppContext.DIC_Holidays.Any())
            {
                return false;
            }
            return AppContext.DIC_Holidays.Any(e => e.RegDate.Year == year);
        }

        public void Delete(long id, long? getCurrentUserId)
        {
            if (!AppContext.DIC_Holidays.Any())
            {
                return;
            }
            var list = AppContext.DIC_Holidays.Where(e => e.RegDate.Year == id);
            foreach (var dicHolidayse in list)
            {
                AppContext.DIC_Holidays.Remove(dicHolidayse);
            }
            AppContext.SaveChanges();
        }
    }
}