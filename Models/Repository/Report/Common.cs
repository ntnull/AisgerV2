using Aisger.Models.Entity;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Report
{
    public class Common
    {
        private static string conName = "conName";

        public Dictionary<string, object> getTableList(string query)
        {
            int reportYear = getStaticMainPageReportYear();
            query = query.Replace("#year", reportYear.ToString());
            //---- 
            Dictionary<string, object> item = new Dictionary<string, object>();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string errorMessage = "";

            //---- Connect to a PostgreSQL database
            string conString = ConfigurationManager.AppSettings[conName];
            NpgsqlConnection conn = new NpgsqlConnection(conString);

            try
            {
                conn.Open();

                //---- 
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.CommandTimeout = 100000;
                NpgsqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {

                    foreach (DbDataRecord dbDataRecord in dr)
                    {
                        list.Add(new Dictionary<string, object>());
                        for (int i = 0; i < dbDataRecord.FieldCount; i++)
                        {
                            list.Last()[dbDataRecord.GetName(i)] = dbDataRecord[dbDataRecord.GetName(i)];
                        }
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            item["ListItems"] = list;
            item["ErrorMessage"] = errorMessage;

            return item;
        }

        public List<MainPageInfoModel> GetMainPageInfoList(string query)
        {
            int reportYear = getStaticMainPageReportYear();
            query = query.Replace("#year", reportYear.ToString());
            //---- 
            Dictionary<string, object> item = new Dictionary<string, object>();
            List<MainPageInfoModel> list = new List<MainPageInfoModel>();
            string errorMessage = "";

            //---- Connect to a PostgreSQL database
            string conString = ConfigurationManager.AppSettings[conName];
            NpgsqlConnection conn = new NpgsqlConnection(conString);

            try
            {
                conn.Open();

                //---- 
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.CommandTimeout = 100000;
                NpgsqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    foreach (DbDataRecord dbDataRecord in dr)
                    {
                        var row = new MainPageInfoModel();
                        row.name = dbDataRecord["name"].ToString();
                        row.cn = (dbDataRecord["cn"] != null) ? Convert.ToInt32(dbDataRecord["cn"].ToString()) : 0;
                        list.Add(row);
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return list;
        }
        public int getStaticMainPageReportYear()
        {
            //---- 
            string query = "select \"ReportYear\" from \"RST_Report\" where \"IsStatisticMainPage\"=true  order by \"ReportYear\" desc  limit 1  ";
            string errorMessage = "";
            int reportYear = DateTime.Now.Year - 1;
            //---- Connect to a PostgreSQL database
            string conString = ConfigurationManager.AppSettings[conName];
            NpgsqlConnection conn = new NpgsqlConnection(conString);

            try
            {
                conn.Open();

                //---- 
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.CommandTimeout = 100000;
                reportYear = (int)command.ExecuteScalar();           

                conn.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                reportYear = DateTime.Now.Year - 1;
            }

            return reportYear;
        }

        public string getListByQuery(ref List<Dictionary<string, object>> list, string query)
		{
			string errorMessage = "";

			//---- Connect to a PostgreSQL database
			string conString = ConfigurationManager.AppSettings[conName];
			NpgsqlConnection conn = new NpgsqlConnection(conString);

			try
			{
				conn.Open();

				//---- 
				NpgsqlCommand command = new NpgsqlCommand(query, conn);
				command.CommandTimeout = 100000;
				NpgsqlDataReader dr = command.ExecuteReader();
				if (dr.HasRows)
				{

					foreach (DbDataRecord dbDataRecord in dr)
					{
						list.Add(new Dictionary<string, object>());
						for (int i = 0; i < dbDataRecord.FieldCount; i++)
						{
							list.Last()[dbDataRecord.GetName(i)] = dbDataRecord[dbDataRecord.GetName(i)];
						}
					}
				}

				conn.Close();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

        public string getTableWithList(ref List<Dictionary<string, object>> list, string query)
        {
            //----
            string errorMessage = "";

            //---- Connect to a PostgreSQL database
            string conString = ConfigurationManager.AppSettings[conName];
            NpgsqlConnection conn = new NpgsqlConnection(conString);

            try
            {
                conn.Open();

                //---- 
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {

                    foreach (DbDataRecord dbDataRecord in dr)
                    {
                        list.Add(new Dictionary<string, object>());
                        for (int i = 0; i < dbDataRecord.FieldCount; i++)
                        {
                            list.Last()[dbDataRecord.GetName(i)] = dbDataRecord[dbDataRecord.GetName(i)];
                        }
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
       
    }
}