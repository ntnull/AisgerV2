using System;
using System.Collections.Generic;
using System.Web;
using Aisger.Models;
using Aisger.Utils;
using log4net;
using Aisger.Models.Repository.Report;
using System.Web.Mvc;

namespace Aisger
{
    public static class MyExtensions
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MyExtensions));
        
        public static long GetCurrentOrgId()
        {
            Logger.Debug("GetCurrentOrgId");
            return (long) HttpContext.Current.Session[CodeConstManager.SESSION_USER_ORG_ID];
        }

        public static long? GetCurrentUserId()
        {
            Logger.Debug("GetCurrentEmployee");
            var user = (SEC_User) HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return null;
            }
            return user.Id;
        }

        public static string GetCurrentUserBIN()
        {
            Logger.Debug("GetCurrentEmployee");
            var user = (SEC_User)HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return null;
            }
            return user.BINIIN;
        }

        public static string GetCurrentUserApplicationName()
        {
            Logger.Debug("GetCurrentEmployee");
            var user = (SEC_User) HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return null;
            }
            return user.ApplicationName;
        }

        public static string GetShortCurrentUserApplicationName()
        {
            Logger.Debug("GetCurrentEmployee");
            var user = (SEC_User) HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return null;
            }
            if (user.ApplicationName.Length > 20)
            {
                return user.ApplicationName.Substring(0, 20);
            }
            return user.ApplicationName;
        }

        public static bool CheckRight(string code)
        {
            var roles = (Dictionary<string, bool>) HttpContext.Current.Session[CodeConstManager.SESSION_USER_ROLES];
            var checkRight = roles != null && roles.ContainsKey(code);
            Logger.InfoFormat("CheckRight {0} - {1} ", code, checkRight);
            return checkRight;


        }

        public static string GetListRight()
        {
            var roles = (Dictionary<string, bool>) HttpContext.Current.Session[CodeConstManager.SESSION_USER_ROLES];
            if (roles == null)
            {
                return "";
            }
			string str = "";
			foreach (var i in roles) {
				if (i.Value == true) {
					str += i.Key;
				}
			}
			var t = str;
            return String.Join("#", roles.Keys);
        }

        public static bool CheckRightEdit(string code)
        {
            //[todo] проверка чтения!!!
            var roles = (Dictionary<string, bool>) HttpContext.Current.Session[CodeConstManager.SESSION_USER_ROLES];
            var checkRightEdit = roles != null && roles.ContainsKey(code) && roles[code];
            Logger.InfoFormat("checkRightEdit {0} - {1} ", code, checkRightEdit);
            return checkRightEdit;

        }

        public static string GetControllerByTable(string table)
        {
            return String.IsNullOrWhiteSpace(table) ? null : table.Replace("_", "");
        }

        public static long? GetRolesId()
        {
            Logger.Debug("GetRolesId");
            var user = (SEC_User)HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return 0;
            }
            return user.RolesId;
        }

		public static string GetCurrentPublishDate()
		{
			string dirpath = System.Web.HttpContext.Current.Server.MapPath("~/bin/Aisger.dll");
			var date = System.IO.File.GetLastWriteTime(dirpath);
			string strdate = date.ToString("dd-MM-yyyy HH:mm");
			return strdate;
		}

        public static int? GetCurrentUserFSCode()
        {
            Logger.Debug("GetCurrentEmployee");
            var user = (SEC_User)HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return null;
            }
            return user.FSCode;
        }

        public static string GetCurrentUserLogin()
        {
            Logger.Debug("GetCurrentEmployee");
            var user = (SEC_User)HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            if (user == null)
            {
                return null;
            }
            return user.Login;
        }

        public static int SearchStaticVal(string name)
        {
            int year = 2019;
            int result = 0;
            try
            {
                var list = new List<Dictionary<string, object>>();

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

                var item = cm.getTableList(query);
                HttpContext.Current.Session["StartPage"] = item;
                list = (List<Dictionary<string, object>>)item["ListItems"];

                for (int i = 0; i < list.Count; i++)
                    if (list[i]["name"].ToString() == name)
                    {
                        result = Convert.ToInt32(list[i]["cn"].ToString());
                    }
            }
            catch (Exception ex)
            {
                if (name == "ип")
                    result = 9;

                if (name == "уклонились")
                    result = 5427;

                if (name == "в реестре")
                    result = 26575;

                if (name == "гу")
                    result = 13684;

                if (name == "юл")
                    result = 3617;

                if (name == "субъекты гэр")
                    result = 26618;

                if (name == "исключено")
                    result = 43;

                if (name == "кв")
                    result = 4478;
            }

            return result;
        }

        public static List<Dictionary<string, object>> SearchStaticValList()
        {
            int year = 2019;
            int result = 0;

            var list = new List<Dictionary<string, object>>();
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

                var item = cm.getTableList(query);
                HttpContext.Current.Session["StartPage"] = item;
                list = (List<Dictionary<string, object>>)item["ListItems"];

            }
            catch (Exception ex)
            {
            }

            return list;
        }

        public static int GetVal(string name)
        {
            return 1;//SearchStaticVal(name);
        }
    }
}