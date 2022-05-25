using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
    public partial class DIC_Holidays :IEntity
    {
        public const string DATE_FORMAT = "dd/mm";
        public const string DATE_FORMAT_FULL = "dd/MM/yyyy";

        [Display(Name = "RegDate", ResourceType = typeof (ResourceSetting))]
        public string RegDateStr { get; set; }

        public static DateTime? GetDate(string date)
        {
            if (String.IsNullOrEmpty(date))
            {
                return null;
            }
            return GetDateFormat(date);
        }
        private static DateTime? GetDateFormat(string val)
        {
            DateTime date;
            if (DateTime.TryParseExact(val, DATE_FORMAT_FULL, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            //                DateTime.TryParse(val, out date))
            {
                return date;
            }
            return null;
        }
        public static string GetDate(DateTime? date)
        {
            if (date != null && date.Value.Year < 2000)
            {
                return null;
            }
            return date != null ? date.Value.ToString(DATE_FORMAT, CultureInfo.InvariantCulture) : null;
        }
    }

    public class DicHolidayYear
    {
        public int Year { get; set; }
        public int HolidaysCount { get; set; }
        public int WorkCount { get; set; }
    }

    public class DicHolidayEntity
    {   
        public int Year { get; set; }
        public List<DIC_Holidays> DicHolidayses { get; set; }
        public List<DIC_Holidays> DicWorkes { get; set; }
    }
}