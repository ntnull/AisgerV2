using System.Globalization;
using System.Web;
using NPOI.SS.Formula.Functions;

namespace Aisger.Utils
{
    public class CultureUtils
    {
        public const string CURRENT_CULTURE = "CURRENT_CULTURE";

        public static CultureInfo GetCurrentCulture()
        {
            var culture = new CultureInfo("ru");
            if (HttpContext.Current != null && HttpContext.Current.Session[CURRENT_CULTURE] != null)
            {
                culture = (CultureInfo) HttpContext.Current.Session[CURRENT_CULTURE];
            }
            return culture;
        }

     
    }
}