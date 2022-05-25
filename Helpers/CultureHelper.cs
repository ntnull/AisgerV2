using System.Collections.Generic;
using System.Threading;
using Aisger.Utils;

namespace Aisger.Helpers
{
    public class CultureHelper
    {
        public const string Ru = "ru";
        public const string Kk = "kk";
        // public const string En = "en";

        public const string FieldRu = "ru";
        public const string FieldKz = "kz";
        // public const string FieldEn = "En";


        public const string CookiesField = "lang";

        public static List<string> Cultures = new List<string>() { 
            Ru
            //, En
            , Kk };

        public static string GetCurrentCulture()
        {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
               return FieldRu;
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                return FieldKz;
            //if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == En)
            //    return FieldEn;
            return string.Empty;
        }

        public static string GetDictionaryName(string code)
        {
            switch (code)
            {
                case "NameRu":
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return "NameRu";
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return "NameKz";
                        break;
                    }
                case "NAME_RU":
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return "NAME_RU";
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return "NAME_KZ";
                        break;
                    }
               
                case CodeConstManager.SUB_DIC_STATUS_NOTGIVED:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.SUB_DIC_STATUS_NOTGIVED;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.SUB_DIC_STATUS_NOTGIVED_KZ;
                        break;
                    }
                case CodeConstManager.SORT_NAME_DATEEDIT:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.SORT_NAME_DATEEDIT;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.SORT_NAME_DATEEDIT_KZ;
                        break;
                    }
                
                case CodeConstManager.SORT_NAME_DATESEND:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.SORT_NAME_DATESEND;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.SORT_NAME_DATESEND_KZ;
                        break;
                    }

                case CodeConstManager.SUB_REASON_SEND:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.SUB_REASON_SEND;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.SUB_REASON_SEND_KZ;
                        break;
                    }
                case CodeConstManager.SUB_REASON_NOTSEND:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.SUB_REASON_NOTSEND;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.SUB_REASON_NOTSEND_KZ;
                        break;
                    }
                case CodeConstManager.SUB_REASON_ALL:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.SUB_REASON_ALL;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.SUB_REASON_ALL_KZ;
                        break;
                    }
                case CodeConstManager.RST_EXCLUDED_NAME:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.RST_EXCLUDED_NAME;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.RST_EXCLUDED_NAME_KZ;
                        break;
                    }
                case CodeConstManager.RST_NOTEXCLUDED_NAME:
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return CodeConstManager.RST_NOTEXCLUDED_NAME;
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return CodeConstManager.RST_NOTEXCLUDED_NAME_KZ;
                        break;
                    }
                      case "nameGive":
                    {
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Ru)
                            return "Не предоставил";
                        if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == Kk)
                            return "Тапсырмаған";
                        break;
                    }
                   
              
            }
            return string.Empty;
        }
    }
}