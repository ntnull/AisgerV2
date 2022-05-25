using Aisger.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Repository.Security
{
    public class UA
    {
        //private static string lang = CultureHelper.GetCurrentCulture();
        static string ConnectionString = ConfigurationManager.AppSettings["conName"];

        public int Id { get; set; }
        public DateTime? RealDate { get; set; }
        public string RealUser { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorLogin { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public int? OriginalId { get; set; }
        public string TableName { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        
        public string RealDateHtml
        {
            get
            {
                string result = "";
                if (RealDate.HasValue)
                {
                    result = RealDate.Value.ToString("yyyy.MM.dd HH:mm:ss");
                }
                return result;
            }
        }
        public string EventTypeName { get; set; }
        public string EventTypeGlyphIcon { get; set; }
        public string OriginalIdHtml
        {
            get
            {
                string result = "";
                if (OriginalId.HasValue)
                {
                    result = OriginalId.Value.ToString();
                }
                return result;
            }
        }
        public bool IsForCRUD { get; set; }

        public string RealTableName { get; set; }
        public string User { get; set; }
                
        public string ExtData { get; set; }


        public static List<SelectListItem> EventTypeList(string lang)
        {
            List<SelectListItem> eventTypeList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = (lang == "kz") ? "Барлық әрекеттер": "Все виды действий ", Value = "0" } };

                using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
                {
                    connect.Open();
                    using (NpgsqlCommand command = connect.CreateCommand())
                    {
                        command.CommandText = @"select * from public.""SEC_UA_EVENTTYPE"" order by ""Id""";
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                eventTypeList.Add(new SelectListItem { Selected = false, Value = reader["Id"].ToString(), Text = (lang == "kz") ? reader["NameKz"].ToString() : reader["Name"].ToString() });
                            }
                            reader.Close();
                        }
                    }
                    connect.Close();
                }
            

            return eventTypeList;
        }


        public static List<SelectListItem> UserList(string lang)
        {
            List<SelectListItem> userList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = (lang == "kz") ? "Барлық пайдаланушылар" : "Все пользователи", Value = "all" } };

                //using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
                //{
                //    connect.Open();
                //    using (NpgsqlCommand command = connect.CreateCommand())
                //    {
                //        command.CommandText = @"select distinct ""AuthorLogin"", ""AuthorId"" from public.""SEC_UA_EVENTS""";
                //        using (NpgsqlDataReader reader = command.ExecuteReader())
                //        {
                //            while (reader.Read())
                //            {
                //                string author = reader[0].ToString();
                //                if (reader["AuthorId"].Equals(DBNull.Value))
                //                {
                //                    author = reader[0].ToString();
                //                }
                //                else
                //                {
                //                    SEC_User usr = new SecUserRepository().GetById(Convert.ToInt64(reader["AuthorId"]));
                //                    if (usr != null)
                //                    {
                //                        author = usr.FullName + " (" + usr.Login + ")";
                //                    }
                //                    else
                //                    {
                //                        author = reader[0].ToString();
                //                    }
                //                }


                //                userList.Add(new SelectListItem { Selected = false, Value = reader[0].ToString(), Text = author });
                //            }
                //            reader.Close();
                //        }
                //    }
                //    connect.Close();
                //}
            
            return userList;
        }

        public static List<SelectListGroup> tableGroupList;
        public static List<SelectListGroup> TableGroupList
        {
            get
            {
                if (tableGroupList == null)
                {
                    tableGroupList = new List<SelectListGroup>();

                    using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
                    {
                        connect.Open();
                        using (NpgsqlCommand command = connect.CreateCommand())
                        {
                            command.CommandText = @"SELECT distinct split_part(""TABLE_NAME"", '_', 1) from public.""SEC_UA_TABLES""";
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    tableGroupList.Add(new SelectListGroup { Name = reader[0].ToString() });
                                }
                                reader.Close();
                            }
                        }
                        connect.Close();
                    }
                }
                return tableGroupList;
            }
        }


        public static List<SelectListItem> TableList(string lang)
        {
            List<SelectListItem> tableList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = (lang == "kz") ? "Барлық кестелер" : "Все таблицы", Value = "all" } };

            using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
            {
                connect.Open();
                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = @"select * from public.""SEC_UA_TABLES"" order by ""DESCR"", ""TABLE_NAME""";
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string val = reader["TABLE_NAME"].ToString();
                            string txt = val;
                            if (!reader["DESCR"].Equals(DBNull.Value))
                            {
                                txt = reader["DESCR"].ToString();
                            }

                            tableList.Add(new SelectListItem { Selected = false, Value = val, Text = txt, Group = TableGroupList.Find(g => g.Name == val.Split('_')[0]) });
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }


            return tableList;
        }


        public static List<SelectListItem> ControllerList(string lang)
        {
            List<SelectListItem> controllerList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = (lang == "kz") ? "Барлық бөлімдер" : "Все разделы", Value = "all" } };

                using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
                {
                    connect.Open();
                    using (NpgsqlCommand command = connect.CreateCommand())
                    {
                        command.CommandText = @"SELECT ""CONTROLLER_NAME"", ""DESCR"" FROM public.""SEC_UA_CONTROLLERS"" order by ""DESCR""";
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                controllerList.Add(new SelectListItem { Value = reader[0].ToString(), Text = reader[1].ToString() });
                            }
                            reader.Close();
                        }
                    }
                    connect.Close();
                }            

            return controllerList;
        }

        public UA() { }

        public UA(NpgsqlDataReader reader)
        {
            Id = Convert.ToInt32(reader["id"]);
            RealDate = Convert.ToDateTime(reader["RealDate"]);
            RealUser = reader["RealUser"].ToString();
            EventTypeName = reader["EventType"].ToString();
            EventTypeGlyphIcon = "glyphicon glyphicon-" + reader["GlyphIcon"].ToString();

            if (reader["AuthorId"].Equals(DBNull.Value))
            {
                User = RealUser;
            }
            else
            {
                SEC_User usr = new SecUserRepository().GetById(Convert.ToInt64(reader["AuthorId"]));
                if (usr != null)
                {
                    User = usr.FullName + " (" + usr.Login + ")";
                }
                else
                {
                    User = reader["AuthorLogin"].ToString();
                }
            }

            IsForCRUD = Convert.ToBoolean(reader["IsForCRUD"]);

            if (IsForCRUD)
            {
                TableName = reader["TableName"].ToString();

                RealTableName = reader["Table_Name"].ToString();

                if (!reader["OriginalId"].Equals(DBNull.Value))
                {
                    OriginalId = Convert.ToInt32(reader["OriginalId"]);
                }
            }
            else
            {
                Controller = reader["controller"].ToString();
                Action = reader["act"].ToString();
            }
        }
        
        public static Dictionary<string,KeyValuePair<string, bool>> GetColumns(string tableName)
        {
            Dictionary<string, KeyValuePair<string, bool>> result = new Dictionary<string, KeyValuePair<string, bool>>();
            using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
            {
                connect.Open();
                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM public.""SEC_UA_COLUMNS"" where ""TABLE_NAME"" = '" + tableName + "'";
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader["column_name"].ToString(), new KeyValuePair<string, bool>(reader["descr"].ToString(), Convert.ToBoolean(reader["ishidden"])));
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return result;
        }

        public static string GetData(string id, bool old)
        {
            string result = null;
            string field = "OldData";
            if (!old)
            {
                field = "NewData";
            }

            using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
            {
                connect.Open();
                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = @"select """+ field + @""" from public.""SEC_UA_EVENTS"" where ""Id"" = " + id;
                    result = command.ExecuteScalar().ToString();
                }
                connect.Close();
            }
            return result;
        }
        
        public static List<SelectListItem> GetActionList(string controller,string lang)
        {
            List<SelectListItem> actionList = new List<SelectListItem> { new SelectListItem { Selected = true, Text =(lang=="kz")?"Барлық беттер":"Все страницы", Value = "all" } };

            if (controller==null)
            {
                return actionList;
            }
            using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
            {
                connect.Open();
                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = @"
select *
from public.""SEC_UA_ACTIONS""
where ""SEC_UA_CONTROLLER"" = '" + controller + "'";
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string val = reader["action_name"].ToString();
                            string txt = reader["descr"].ToString();

                            actionList.Add(new SelectListItem { Selected = false, Value = val, Text = txt });
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return actionList;
        }

        public static IList<UA> GetList(int limit, int offset, string eType, string user, string period, string from, string to, string table, string control, string page, string sort, out int count)
        {
            IList<UA> result = new List<UA>();

            string sql = @"
select count(*) from
(select
  ev.""Id"", ev.""RealDate"", ev.""RealUser"", et.""Name"" EventType, et.""GlyphIcon"", ev.""AuthorId"", ev.""AuthorLogin"",
  coalesce(tt.""DESCR"", tt.""TABLE_NAME"") TableName, ev.""OriginalId"", ev.""OldData"", ev.""NewData"", et.""IsForCRUD"",
  ct.""DESCR"" controller, ac.""DESCR"" act, coalesce(tt.""LOGLEVEL"", 10) ll, et.""Id"" eType, tt.""TABLE_NAME"", ct.""CONTROLLER_NAME"", ac.""ACTION_NAME"", tt.""EXTSQL""
from public.""SEC_UA_EVENTTYPE"" et
  LEFT JOIN public.""SEC_UA_EVENTS"" ev on ev.""SEC_UA_EVENTTYPE"" = et.""Id""
  LEFT OUTER JOIN public.""SEC_UA_TABLES"" tt ON ev.""SEC_UA_TABLES"" = tt.""TABLE_NAME""
  LEFT OUTER JOIN public.""SEC_UA_CONTROLLERS"" ct ON ev.""Controller"" = ct.""CONTROLLER_NAME""
  LEFT OUTER JOIN public.""SEC_UA_ACTIONS"" ac ON ev.""Action"" = ac.""ACTION_NAME"" and ct.""CONTROLLER_NAME"" = ac.""SEC_UA_CONTROLLER""
order by SORTER) req
where ""Id"" is not null
  and ""ll"" > 0";
            if (user!="")
            {
                sql += @"
  and ""AuthorLogin"" = '" + user + "'";
            }

            if (eType != "0")
            {
                sql += @"
  and ""etype"" = '" + eType + "'";
            }

            switch (period)
            {
                default:break;
                case "1":
                    sql += @"
  and ""RealDate"" > current_date";
                    break;
                case "2":
                    sql += @"
  and date_trunc('day', ""RealDate"") = (current_date - INTERVAL '1 DAY')";
                    break;
                case "10":
                    sql += @"
  and ""RealDate"" >= '" + from.Replace("_", " ") + @"'
  and ""RealDate"" <= '" + to.Replace("_", " ") + @"'";                    
                    break;
            }

            if (table != "all")
            {
                sql += @"
  and ""TABLE_NAME"" = '" + table + "'";
            }

            if (control != "all")
            {
                sql += @"
  and ""CONTROLLER_NAME"" = '" + control + "'";
            }

            if (page != "all")
            {
                sql += @"
  and ""ACTION_NAME"" = '" + page + "'";
            }

            switch (sort)
            {
                case "date_desc":
                    sql = sql.Replace("SORTER", @"""RealDate"" desc");
                    break;
                case "date_asc":
                    sql = sql.Replace("SORTER", @"""RealDate"" asc");
                    break;
            }
            using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
            {
                connect.Open();
                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = sql;
                    count = Convert.ToInt32(command.ExecuteScalar());
                }

                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = sql.Replace("count(*)", "*") + "\nlimit " + limit.ToString() + " offset " + offset;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new UA(reader));

                            if (!reader["EXTSQL"].Equals(DBNull.Value))
                            {
                                if (!string.IsNullOrEmpty(reader["EXTSQL"].ToString()))
                                {
                                    result.Last().ExtData = GetExtData(reader["OriginalId"].ToString(), reader["EXTSQL"].ToString());
                                }
                            }
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }

            return result;
        }

        public static string GetExtData(string id, string sql)
        {
            string result = null;

            using (NpgsqlConnection connect = new NpgsqlConnection(ConnectionString))
            {
                connect.Open();
                using (NpgsqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = sql.Replace("UAUID", id);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            for (int i = 0; i <reader.FieldCount; i++)
                            {
                                result += reader.GetName(i) + ": " + reader[i].ToString() + ", ";
                            }                            
                        }
                        reader.Close();
                    }                        
                }
                connect.Close();
            }
            return result;
        }
    }
}