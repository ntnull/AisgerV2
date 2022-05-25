using Aisger.Helpers;
using Aisger.Models.Entity.Reestr;
using Aisger.Models.Repository.Dictionary;
using Aisger.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Repository.Report
{
	public class SourceControllerRepository : SqlRepository
	{
        private string lang = "ru";
        public SourceControllerRepository()
        {
            lang = CultureHelper.GetCurrentCulture();
        }        

		public string getNotOwnSourcePageCount(string columnsVal, int year, ref int allcount, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, long restype_id, double? min, double? max, string name_oked_idk, string fscode, string oked_ids, int isplan, int isem_system)
		{
			string errorMessage = "";
			try
			{
				#region query

				string query = getQueryNew(year, false);
				string where = "";

				//----окэд идк наименование
				if (!string.IsNullOrWhiteSpace(name_oked_idk))
					where += " and ( upper(t.\"IDK\") like '%" + name_oked_idk.ToUpper() + "%'  or upper(o2.\"NameRu\") like '%" + name_oked_idk.ToUpper() + "%' or upper(t.\"OwnerName\") like '%" + name_oked_idk.ToUpper() + "%' or t1.\"BINIIN\" like '%" + name_oked_idk + "%' ) ";

				//----
				if (!oblast_ids.Equals("-1"))
				{
					where += " and t.\"Oblast\" in (" + replaceToComma(oblast_ids) + ")";
				}

				//----
				if (!string.IsNullOrWhiteSpace(reason_ids))
				{
					where += " and t.\"ReasonId\" in (" + replaceToComma(reason_ids) + ")";
				}

				//----
				if (!string.IsNullOrWhiteSpace(expectant_ids))
				{
					if (expectant_ids.IndexOf("null") == -1)
					{
						where = where + " and t.\"Expectant\" in (" + replaceToComma(expectant_ids) + ")";
					}
					else
					{
						var arr = expectant_ids.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids+= arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (t.\"Expectant\" is null) or ( t.\"Expectant\" in (" + ids + ") ))";
						}
						else where = where + " and t.\"Expectant\" is null ";
					}
				}

				//----
				if (excluded_id != 0)
				{
					long l;
					Int64.TryParse(Convert.ToString(excluded_id), out l);
					switch (l)
					{
						case CodeConstManager.RST_EXCLUDED_ID:
							{
								where = where + " and t.\"IsExcluded\"=TRUE ";
								break;
							}
						case CodeConstManager.RST_NOTEXCLUDED_ID:
							{
								where = where + " and t.\"IsExcluded\"=FALSE ";
								break;
							}
					}
				}

				//----fscode
				if (!string.IsNullOrWhiteSpace(fscode))
				{
					if (fscode.IndexOf("null")==-1)
					{
						where = where + " and t.usrfscode in (" + replaceToComma(fscode) + ")";
					}
					else
					{
						var arr = fscode.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
                                    ids+= arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (t.usrfscode is null) or ( t.usrfscode in (" + ids + ") ))";
						}
						else where = where + " and t.usrfscode is null ";
					}

				}

				//----oked
				if (!string.IsNullOrWhiteSpace(oked_ids))
				{
					if (oked_ids.IndexOf("null") == -1)
					{
						where = where + " and o2.\"RootId\" in (" + replaceToComma(oked_ids) + ")";
					}
					else
					{
						var arr = oked_ids.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids+= arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (o2.\"RootId\" is null) or ( o2.\"RootId\" in (" + ids + ") ))";
						}
						else where = where + " and o2.\"RootId\" is null ";
					}
				}


                //----isplan
                if (isplan != -1)
                {

                    where = where + " and tf.\"IsPlan\"=" + ((isplan == 1) ? "true" : "false");
                }

                //----isem_system
                if (isem_system != -1)
                {

                    where = where + " and tf.\"IsEnergyManagementSystem\"=" + ((isem_system == 1) ? "true" : "false");
                }


                if (restype_id != 0 && restype_id != -1)
				{
					string colName = "s" + restype_id;
					where += " and " + colName + ".\"TypeResourceId\"=" + restype_id;

					string notOwnRes = colName + ".\"NotOwnSource\" ";					
					if (min != 0 && max != 0)
						where += " and " + notOwnRes + ">=" + min + " and " + notOwnRes + "<=" + max;
					else if (min != 0)
						where += " and " + notOwnRes + ">=" + min;
					else if (max != 0)
						where += " and " + notOwnRes + "<=" + max;
				}

				query = query + where;
				query = "select count(*) from (" + query + ") f   ";

				if (restype_id == 0)
				{
					#region if else in

					if (min != 0 && max != 0)
						query += " where f.consumption>=" + min + " and f.consumption<=" + max;
					else if (min != 0)
						query += " where f.consumption>=" + min;
					else if (max != 0)
						query += " where f.consumption<=" + max;

					#endregion
				}

				//----checker replacer
				if (!columnsVal.Equals("NotOwnSource"))
				{
					while (query.IndexOf("NotOwnSource") != -1)
					{
						query = query.Replace("NotOwnSource", columnsVal);
					}
				}

				//-----consump
				var cons_query = GetConsumptionQuery();
				query = query.Replace("cons_query", cons_query);

				int count = AppContext.Database.SqlQuery<int>(query).First();
				allcount = count;
				#endregion
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

		public string getNotOwnSource(string columnsVal,int year, int PageNum, ref List<SourceControllerClass> rlist, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, bool istut, long restype_id, double? min, double? max, int orderById, string name_oked_idk,string fscode,string oked_ids,int isplan,int isem_system)
		{			
			string errorMessage = "";
			try
			{
				int offset = 0;
				if (PageNum > 1)
				{
					offset = (PageNum - 1) * 50;
				}

				#region query

				string query = getQueryNew(year, istut);
				string where="";
				
				//----окэд идк наименование
				if (!string.IsNullOrWhiteSpace(name_oked_idk))
					where += " and ( upper(t.\"IDK\") like '%" + name_oked_idk.ToUpper() + "%'  or upper(o2.\"NameRu\") like '%" + name_oked_idk.ToUpper() + "%' or upper(t.\"OwnerName\") like '%" + name_oked_idk.ToUpper() + "%' or t1.\"BINIIN\" like '%"+name_oked_idk+"%' ) ";
				
				//----oblast
				if (!oblast_ids.Equals("-1"))
				{
					where += " and t.\"Oblast\" in (" + replaceToComma(oblast_ids) + ")";
				}

				//----reason
				if (!string.IsNullOrWhiteSpace(reason_ids))
				{				
					where += " and t.\"ReasonId\" in (" + replaceToComma(reason_ids)+")";
				}

				//----expectant
				if (!string.IsNullOrWhiteSpace(expectant_ids))
				{
					if (expectant_ids.IndexOf("null") == -1)
					{
						where = where + " and t.\"Expectant\" in (" + replaceToComma(expectant_ids) + ")";
					}
					else
					{
						var arr = expectant_ids.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids+= arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (t.\"Expectant\" is null) or ( t.\"Expectant\" in (" + ids + ") ))";
						}
						else where = where + " and t.\"Expectant\" is null ";
					}					
				}

				//----
				if (excluded_id != 0)
				{
					long l;
					Int64.TryParse(Convert.ToString(excluded_id), out l);
					switch (l)
					{
						case CodeConstManager.RST_EXCLUDED_ID:
							{
								where = where + " and t.\"IsExcluded\"=TRUE ";
								break;
							}
						case CodeConstManager.RST_NOTEXCLUDED_ID:
							{
								where = where + " and t.\"IsExcluded\"=FALSE ";
								break;
							}
					}
				}

				//----fscode
				 if (!string.IsNullOrWhiteSpace(fscode))
				{
					if (fscode.IndexOf("null")==-1)
					{
						where = where + " and t.usrfscode in (" + replaceToComma(fscode) + ")";
					}
					else
					{
						var arr = fscode.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids+= arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (t.usrfscode is null) or ( t.usrfscode in (" + ids + ") ))";
						}
						else where = where + " and t.usrfscode is null ";
					}

				}

				//----oked
				 if (!string.IsNullOrWhiteSpace(oked_ids))
				 {
					 if (oked_ids.IndexOf("null") == -1)
					 {
						 where = where + " and o2.\"RootId\" in (" + replaceToComma(oked_ids) + ")";
					 }
					 else
					 {
						 var arr = oked_ids.Split('*');
						 if (arr.Length > 1)
						 {
							 var ids = "";
							 for (int i = 0; i < arr.Length; i++)
								 if (arr[i] != "null")
								 {
									 ids+= arr[i] + ",";
								 }

							 ids = ids.TrimEnd(',');
							 where = where + " and ( (o2.\"RootId\" is null) or ( o2.\"RootId\" in (" + ids + ") ))";
						 }
						 else where = where + " and o2.\"RootId\" is null ";
					 }
				 }

                //----isplan
                if (isplan != -1)
                {

                    where = where + " and tf.\"IsPlan\"=" + ((isplan == 1) ? "true" : "false");
                }

                //----isem_system
                if (isem_system != -1)
                {

                    where = where + " and tf.\"IsEnergyManagementSystem\"=" + ((isem_system == 1) ? "true" : "false");
                }

                string orderBy = " order by t.\"Id\" ";
				if (restype_id != 0 && restype_id != -1)
				{

					string colName = "s" + restype_id;
					string colName2 = "r" + restype_id;
					where += " and " + colName + ".\"TypeResourceId\"=" + restype_id;

					string notOwnRes = colName + ".\"NotOwnSource\" ";
					if (istut)
						notOwnRes = " (" + colName2 + ".\"Keof\"*" + colName + ".\"NotOwnSource\") ";


					if (min != 0 && max != 0)
						where += " and " + notOwnRes + ">=" + min + " and " + notOwnRes + "<=" + max;
					else if (min != 0)
						where += " and " + notOwnRes + ">=" + min;
					else if (max != 0)
						where += " and " + notOwnRes + "<=" + max;


					//----check order by
					if (orderById == 1)
						orderBy = "  order by " + notOwnRes;
					else if (orderById == 0)
						orderBy = "  order by " + notOwnRes + " desc ";
				}

				if (restype_id == 0)
				{
					if (orderById != -1)
						orderBy = "";
				}

				query = query + where + orderBy;

				query = "select f.* from (" + query + ") f   ";

				if (restype_id == 0)
				{
					#region if else in

					if (min != 0 && max != 0)
						query += " where f.consumption>=" + min + " and f.consumption<=" + max;
					else if (min != 0)
						query += " where f.consumption>=" + min;
					else if (max != 0)
						query += " where f.consumption<=" + max;

					//----check order by
					if (orderById == 1)
						query += "  order by f.consumption";
					else if (orderById == 0)
						query += "  order by f.consumption desc ";
					#endregion
				}

				string pagenation = " LIMIT 50 OFFSET " + offset;
				if (PageNum != 0)
					query = query + pagenation;

				#endregion
				
				//----checker replacer
				if (!columnsVal.Equals("NotOwnSource"))
				{
					while (query.IndexOf("NotOwnSource") != -1)
					{
						query = query.Replace("NotOwnSource", columnsVal);
					}
				}

				//-----consump
				var cons_query=GetConsumptionQuery();
				query = query.Replace("cons_query", cons_query);
                var dd = query;

				var rows = AppContext.Database.SqlQuery<SourceControllerClass>(query).ToList();
				rlist=rows;
			}
			catch (Exception ex)
			{
				errorMessage += ex.Message;
			}

			return errorMessage;
		}

        public string getNotOwnSourceForExcelExport(string columnsVal, int year, int PageNum, ref List<SourceControllerClass> rlist, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, bool istut, long restype_id, double? min, double? max, int orderById, string fscode, string oked_ids, int isplan,int isem_system,string  name_oked_idk)
		{
			string errorMessage = "";
			try
			{
				#region query
				string query = getQueryNew(year, istut);

				string where = " ";

                //----окэд идк наименование
                if (!string.IsNullOrWhiteSpace(name_oked_idk))
                    where += " and ( upper(t.\"IDK\") like '%" + name_oked_idk.ToUpper() + "%'  or upper(o2.\"NameRu\") like '%" + name_oked_idk.ToUpper() + "%' or upper(t.\"OwnerName\") like '%" + name_oked_idk.ToUpper() + "%' or t1.\"BINIIN\" like '%" + name_oked_idk + "%') ";

  
                //----oblast
                if (!oblast_ids.Equals("-1"))
				{
					where += " and t.\"Oblast\" in (" + replaceToComma(oblast_ids) + ")";
				}

				//----reason
				if (!string.IsNullOrWhiteSpace(reason_ids))
				{
					where += " and t.\"ReasonId\" in (" + replaceToComma(reason_ids) + ")";
				}

				//----expectant
				if (!string.IsNullOrWhiteSpace(expectant_ids))
				{
					if (expectant_ids.IndexOf("null") == -1)
					{
						where = where + " and t.\"Expectant\" in (" + replaceToComma(expectant_ids) + ")";
					}
					else
					{
						var arr = expectant_ids.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids = arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (t.\"Expectant\" is null) or ( t.\"Expectant\" in (" + ids + ") ))";
						}
						else where = where + " and t.\"Expectant\" is null ";
					}
				}

				//----
				if (excluded_id != 0)
				{
					long l;
					Int64.TryParse(Convert.ToString(excluded_id), out l);
					switch (l)
					{
						case CodeConstManager.RST_EXCLUDED_ID:
							{
								where = where + " and t.\"IsExcluded\"=TRUE ";
								break;
							}
						case CodeConstManager.RST_NOTEXCLUDED_ID:
							{
								where = where + " and t.\"IsExcluded\"=FALSE ";
								break;
							}
					}
				}

				//----fscode
				if (!string.IsNullOrWhiteSpace(fscode))
				{
					if (fscode.IndexOf("null")==-1)
					{
						where = where + " and  t.usrfscode in (" + replaceToComma(fscode) + ")";
					}
					else
					{
						var arr = fscode.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids = arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( ( t.usrfscode is null) or (  t.usrfscode in (" + ids + ") ))";
						}
						else where = where + " and  t.usrfscode is null ";
					}

				}

				//----oked
				if (!string.IsNullOrWhiteSpace(oked_ids))
				{
					if (oked_ids.IndexOf("null") == -1)
					{
						where = where + " and o2.\"RootId\" in (" + replaceToComma(oked_ids) + ")";
					}
					else
					{
						var arr = oked_ids.Split('*');
						if (arr.Length > 1)
						{
							var ids = "";
							for (int i = 0; i < arr.Length; i++)
								if (arr[i] != "null")
								{
									ids = arr[i] + ",";
								}

							ids = ids.TrimEnd(',');
							where = where + " and ( (o2.\"RootId\" is null) or ( o2.\"RootId\" in (" + ids + ") ))";
						}
						else where = where + " and o2.\"RootId\" is null ";
					}
				}
                
                //----isplan
                if (isplan != -1)
                {

                    where = where + " and tf.\"IsPlan\"=" + ((isplan == 1) ? "true" : "false");
                }

                //----isem_system
                if (isem_system != -1)
                {

                    where = where + " and tf.\"IsEnergyManagementSystem\"=" + ((isem_system == 1) ? "true" : "false");
                }

                string orderBy = " order by t.\"Id\" ";
                if (restype_id != 0 && restype_id != -1)
                {

                    string colName = "s" + restype_id;
                    string colName2 = "r" + restype_id;
                    where += " and " + colName + ".\"TypeResourceId\"=" + restype_id;

                    string notOwnRes = colName + ".\"NotOwnSource\" ";
                    if (istut)
                        notOwnRes = " (" + colName2 + ".\"Keof\"*" + colName + ".\"NotOwnSource\") ";


                    if (min != 0 && max != 0)
                        where += " and " + notOwnRes + ">=" + min + " and " + notOwnRes + "<=" + max;
                    else if (min != 0)
                        where += " and " + notOwnRes + ">=" + min;
                    else if (max != 0)
                        where += " and " + notOwnRes + "<=" + max;


                    //----check order by
                    if (orderById == 1)
                        orderBy = "  order by " + notOwnRes;
                    else if (orderById == 0)
                        orderBy = "  order by " + notOwnRes + " desc ";
                }

                if (restype_id == 0)
                {
                    if (orderById != -1)
                        orderBy = "";
                }

                query = query + where + orderBy;

                query = "select f.* from (" + query + ") f   ";

                if (restype_id == 0)
                {
                    #region if else in

                    if (min != 0 && max != 0)
                        query += " where f.consumption>=" + min + " and f.consumption<=" + max;
                    else if (min != 0)
                        query += " where f.consumption>=" + min;
                    else if (max != 0)
                        query += " where f.consumption<=" + max;

                    //----check order by
                    if (orderById == 1)
                        query += "  order by f.consumption";
                    else if (orderById == 0)
                        query += "  order by f.consumption desc ";
                    #endregion
                }

                #endregion
                //----checker replacer
                if (!columnsVal.Equals("NotOwnSource"))
				{
					while (query.IndexOf("NotOwnSource") != -1)
					{
						query = query.Replace("NotOwnSource", columnsVal);
					}
				}
                
                //----replace consump
                var cons_query = GetConsumptionQuery();
				query = query.Replace("cons_query", cons_query);

				var rows = AppContext.Database.SqlQuery<SourceControllerClass>(query).ToList();
				rlist = rows;
			}
			catch (Exception ex)
			{
				errorMessage += ex.Message;
			}

			return errorMessage;
		}
		
		public string getResourceType(ref List<ResourceHelper> list)
		{
			string errorMessage = "";
			try
			{
				#region query
				//string old_query = "select t.\"Id\" as resource_id, case '"+lang+"' when 'kz' then t.\"NameKz\" else t.\"NameRu\" end as resource_name,  "
				//			   + " t.\"Code\" as code, case '"+lang+"' when 'kz' then t1.\"NameKz\" else t1.\"NameRu\" end as unit_name  "
				//			   + " from \"SUB_DIC_TypeResource\" t , \"DIC_Unit\" t1 where t.\"UnitId\"=t1.\"Id\" and  t.\"IsDeleted\"=false order by t.\"Id\"    ";



                string query = "  select r.* from (select r1.* from (select t.\"Id\" as resource_id, case 'ru' when 'kz' then t.\"NameKz\" else t.\"NameRu\" end as resource_name,   t.\"Code\" as code, case 'ru' when 'kz' then t1.\"NameKz\" else t1.\"NameRu\" end as unit_name  , 0 as dic_type"
 + "  from \"SUB_DIC_TypeResource\" t , \"DIC_Unit\" t1"
 + "     where t.\"UnitId\"=t1.\"Id\" and  t.\"IsDeleted\"=false order by t.\"Id\" ) r1 "
 + "             union"
 + "             select r2.* from ( select t.\"Id\" as resource_id, case 'ru' when 'kz' then t.\"NameKz\" else t.\"NameRu\" end as resource_name,   t.\"Code\" as " + " code, case 'ru' when 'kz' then t1.\"NameKz\" else t1.\"NameRu\" end as unit_name,1 as dic_type "
 + "               from \"SUB_DIC_KindResource\" t , \"DIC_Unit\" t1"
 + "                where t.\"UnitId\"=t1.\"Id\" and  t.\"IsDeleted\"=false order by t.\"Id\" ) r2 "
 + "              union"
 + "             select 1 as resource_id, case 'ru' when 'kz' then 'количество сотрудников(работников)' else 'количество сотрудников(работников)' end as resource_name,'01' as code, '' as unit_name,2 as dic_type"
  + "              union	"
  + "             select 2 as resource_id, case 'ru' when 'kz' then 'количество учащихся(воспитанников)' else 'количество учащихся(воспитанников)' end as resource_name,'02' as code, '' as unit_name,2 as dic_type"
   + "              union"
   + "             select 3 as resource_id, case 'ru' when 'kz' then 'количество койко-мест(посещений)' else 'количество койко-мест(посещений)' end as resource_name,'03' as code, '' as unit_name,2 as dic_type   "
    + "             ) r order by r.dic_type , r.resource_id  ";
                

                #endregion

                var rows = AppContext.Database.SqlQuery<ResourceHelper>(query).ToList();
				list = rows;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

        private string getQueryNew(int year, bool isTut)
        {
            #region query
            string query = " select t.\"Id\"  as rst_id  , t.\"BINIIN\" as bin "
                            + ", t.\"IDK\" as idk  "
                            + ", case '" + lang + "' when 'kz' then case tf.\"IsPlan\" when true then 'ия' else 'жоқ' end else case tf.\"IsPlan\" when  true then 'да' else 'нет' end end as isplan "
                            + ", case '" + lang + "' when 'kz' then case tf.\"IsEnergyManagementSystem\" when true then 'ия' else 'жоқ' end else case tf.\"IsEnergyManagementSystem\" when  true then 'да' else 'нет' end end as isem_system"
                            + " , case '" +lang+"' when 'kz' then o2.\"NameKz\" else o2.\"NameRu\" end as oked_name  "
                            + " , t1.\"Id\" as user_id , "
                            + " t.\"OwnerName\" as juridical_name "
                            + " , tf.\"Id\" as sub_form_id  "
                            + " , case '"+lang+"' when 'kz' then t3.\"NameKz\" else t3.\"NameRu\" end as oblast_name "
                            + " , t3.\"Id\" as oblast_id  , "
                            + " t.usrfscode as fscode "

                            + " , case '"+lang+"' when 'kz' then  "
                            + "   case t.usrfscode when 1 then 'ЗТ'   "
                            + "   when 2 then 'КВ' "
                            + "     when 3 then 'ММ' when 4 then 'ЖК' else 'м/ж'end   "
                            + "     else  "
                            + "   case t.usrfscode when 1 "
                            + "     then 'ЮР' when 2 then 'КВ' when 3 then 'ГУ' when 4 then 'ИП' else 'н/д' end  "
                            + " end as fscode_name  "
                            + " ,case '"+lang+"' when 'kz' then case t.\"IsExcluded\" when true then 'Шығарылған' else 'Шығарылмаған' end   "
                            + "   else case t.\"IsExcluded\" when true then 'Исключен' else 'Не исключен' end "
                            + "     end as excluded_name "
                            + "   , t.\"IsExcluded\" as isexcluded   ,t4.\"Id\" as expectant_id "
                            + "   , case '"+lang+ "' when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as expectant_name , sumpot.consumption  "
                            + "   ,(COALESCE(r1.\"Keof\",1)*COALESCE(s1.\"NotOwnSource\",0))+   (COALESCE(r2.\"Keof\",1)*COALESCE(s2.\"NotOwnSource\",0))+  (COALESCE(r3.\"Keof\",1)*COALESCE(s3.\"NotOwnSource\",0))+ "
                            + "     (COALESCE(r4.\"Keof\",1)*COALESCE(s4.\"NotOwnSource\",0))+  (COALESCE(r5.\"Keof\",1)*COALESCE(s5.\"NotOwnSource\",0))+  (COALESCE(r6.\"Keof\",1)*COALESCE(s6.\"NotOwnSource\",0))+  "
                            + "     (COALESCE(r7.\"Keof\",1)*COALESCE(s7.\"NotOwnSource\",0))+  (COALESCE(r8.\"Keof\",1)*COALESCE(s8.\"NotOwnSource\",0))+  (COALESCE(r9.\"Keof\",1)*COALESCE(s9.\"NotOwnSource\",0))+  "
                            + "     (COALESCE(r10.\"Keof\",1)*COALESCE(s10.\"NotOwnSource\",0))+  (COALESCE(r11.\"Keof\",1)*COALESCE(s11.\"NotOwnSource\",0))+ "
                            + "     (COALESCE(r12.\"Keof\",1)*COALESCE(s12.\"NotOwnSource\",0))+  (COALESCE(r13.\"Keof\",1)*COALESCE(s13.\"NotOwnSource\",0))+ "
                            + "     (COALESCE(r14.\"Keof\",1)*COALESCE(s14.\"NotOwnSource\",0))+  (COALESCE(r15.\"Keof\",1)*COALESCE(s15.\"NotOwnSource\",0))+ "
                            + "      (COALESCE(r16.\"Keof\",1)*COALESCE(s16.\"NotOwnSource\",0))+  (COALESCE(r17.\"Keof\",1)*COALESCE(s17.\"NotOwnSource\",0))+  "
                            + "      (COALESCE(r18.\"Keof\",1)*COALESCE(s18.\"NotOwnSource\",0))+  (COALESCE(r19.\"Keof\",1)*COALESCE(s19.\"NotOwnSource\",0))+  "
                            + "      (COALESCE(r20.\"Keof\",1)*COALESCE(s20.\"NotOwnSource\",0))+  (COALESCE(r21.\"Keof\",1)*COALESCE(s21.\"NotOwnSource\",0))+   "
                            + "      (COALESCE(r22.\"Keof\",1)*COALESCE(s22.\"NotOwnSource\",0))+   (COALESCE(r23.\"Keof\",1)*COALESCE(s23.\"NotOwnSource\",0))+   "
                            + "      (COALESCE(r24.\"Keof\",1)*COALESCE(s24.\"NotOwnSource\",0))+   (COALESCE(r25.\"Keof\",1)*COALESCE(s25.\"NotOwnSource\",0))+   "
                            + "      (COALESCE(r26.\"Keof\",1)*COALESCE(s26.\"NotOwnSource\",0))+   (COALESCE(r27.\"Keof\",1)*COALESCE(s27.\"NotOwnSource\",0))+   "
                            + "      (COALESCE(r28.\"Keof\",1)*COALESCE(s28.\"NotOwnSource\",0)) as tut    "
                            + "  ,  case "+isTut+" when true then (r1.\"Keof\"*s1.\"NotOwnSource\") else s1.\"NotOwnSource\" end  as noS1  "
                            + "  ,  case " + isTut + " when true then (r2.\"Keof\"*s2.\"NotOwnSource\") else s2.\"NotOwnSource\" end  as noS2  "
                            + "  ,  case " + isTut + " when true then (r3.\"Keof\"*s3.\"NotOwnSource\") else s3.\"NotOwnSource\" end  as noS3  "
                            + "  ,  case " + isTut + " when true then (r4.\"Keof\"*s4.\"NotOwnSource\") else s4.\"NotOwnSource\" end  as noS4  "
                            + "  ,  case " + isTut + " when true then (r5.\"Keof\"*s5.\"NotOwnSource\") else s5.\"NotOwnSource\" end  as noS5  "
                            + "  ,  case " + isTut + " when true then (r6.\"Keof\"*s6.\"NotOwnSource\") else s6.\"NotOwnSource\" end  as noS6  "
                            + "  ,  case " + isTut + " when true then (r7.\"Keof\"*s7.\"NotOwnSource\") else s7.\"NotOwnSource\" end  as noS7  "
                            + "  ,  case " + isTut + " when true then (r8.\"Keof\"*s8.\"NotOwnSource\") else s8.\"NotOwnSource\" end  as noS8  "
                            + "  ,  case " + isTut + " when true then (r9.\"Keof\"*s9.\"NotOwnSource\") else s9.\"NotOwnSource\" end  as noS9  "
                            + "  ,  case " + isTut + " when true then (r10.\"Keof\"*s10.\"NotOwnSource\") else s10.\"NotOwnSource\" end  as noS10  "
                            + "  ,  case " + isTut + " when true then (r11.\"Keof\"*s11.\"NotOwnSource\") else s11.\"NotOwnSource\" end  as noS11  "
                            + "  ,  case " + isTut + " when true then (r12.\"Keof\"*s12.\"NotOwnSource\") else s12.\"NotOwnSource\" end  as noS12  "
                            + "  ,  case " + isTut + " when true then (r13.\"Keof\"*s13.\"NotOwnSource\") else s13.\"NotOwnSource\" end  as noS13  "
                            + "  ,  case " + isTut + " when true then (r14.\"Keof\"*s14.\"NotOwnSource\") else s14.\"NotOwnSource\" end  as noS14  "
                            + "  ,  case " + isTut + " when true then (r15.\"Keof\"*s15.\"NotOwnSource\") else s15.\"NotOwnSource\" end  as noS15  "
                            + "  ,  case " + isTut + " when true then (r16.\"Keof\"*s16.\"NotOwnSource\") else s16.\"NotOwnSource\" end  as noS16  "
                            + "   ,  case " + isTut + " when true then (r17.\"Keof\"*s17.\"NotOwnSource\") else s17.\"NotOwnSource\" end  as noS17  "
                            + "   ,  case " + isTut + " when true then (r18.\"Keof\"*s18.\"NotOwnSource\") else s18.\"NotOwnSource\" end  as noS18  "
                            + "   ,  case " + isTut + " when true then (r19.\"Keof\"*s19.\"NotOwnSource\") else s19.\"NotOwnSource\" end  as noS19  "
                            + "   ,  case " + isTut + " when true then (r20.\"Keof\"*s20.\"NotOwnSource\") else s20.\"NotOwnSource\" end  as noS20  "
                            + "   ,  case " + isTut + " when true then (r21.\"Keof\"*s21.\"NotOwnSource\") else s21.\"NotOwnSource\" end  as noS21  "
                            + "   ,  case " + isTut + " when true then (r22.\"Keof\"*s22.\"NotOwnSource\") else s22.\"NotOwnSource\" end  as noS22  "
                            + "   ,  case " + isTut + " when true then (r23.\"Keof\"*s23.\"NotOwnSource\") else s23.\"NotOwnSource\" end  as noS23  "
                            + "   ,  case " + isTut + " when true then (r24.\"Keof\"*s24.\"NotOwnSource\") else s24.\"NotOwnSource\" end  as noS24  "
                            + "   ,  case " + isTut + " when true then (r25.\"Keof\"*s25.\"NotOwnSource\") else s25.\"NotOwnSource\" end  as noS25  "
                            + "   ,  case " + isTut + " when true then (r26.\"Keof\"*s26.\"NotOwnSource\") else s26.\"NotOwnSource\" end  as noS26  "
                            + "   ,  case " + isTut + " when true then (r27.\"Keof\"*s27.\"NotOwnSource\") else s27.\"NotOwnSource\" end  as noS27  "
                            + "   ,  case " + isTut + " when true then (r28.\"Keof\"*s28.\"NotOwnSource\") else s28.\"NotOwnSource\" end  as noS28   "
                            + " , f2gu.\"CountOfEmployees\" as countOfEmployees,  f2gu.\"CountOfStudents\" as countOfStudents "
                            + " , f2gu.\"CountOfBeds\" as countOfBeds ,f2sum.sum_expenceenergy  from  \"RST_ReportReestr\" t    "
                            + " inner join \"RST_Report\" t5 on t.\"ReportId\"=t5.\"Id\"  "
                            + " inner join \"SEC_User\"  t1 on t1.\"Id\"=t.\"UserId\"  "
                            + " left join \"DIC_OKED\" o1 on t1.\"OkedId\"=o1.\"Id\"  "
                            + " left join \"DIC_OKED\" o2 on o1.\"refParent\"=o2.\"Id\"  "
                            + " left join \"SUB_Form\" tf on t.\"FormId\"=tf.\"Id\" "
                            + " left join \"DIC_Kato\" t3 on t.\"Oblast\"=t3.\"Id\"  "
                            + " left join \"RST_DIC_Reason\" t4 on t.\"Expectant\"=t4.\"Id\"   "
                            + " left join  \"SUB_Form2Record\"  s1  on tf.\"Id\"=s1.\"FormId\"  and s1.\"TypeResourceId\"=1  "
                            + " left join  \"SUB_DIC_TypeResource\" r1 on s1.\"TypeResourceId\"=r1.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s2  on tf.\"Id\"=s2.\"FormId\"  and s2.\"TypeResourceId\"=2  "
                            + " left join  \"SUB_DIC_TypeResource\" r2 on s2.\"TypeResourceId\"=r2.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s3  on tf.\"Id\"=s3.\"FormId\"  and s3.\"TypeResourceId\"=3  left join  \"SUB_DIC_TypeResource\" r3 on s3.\"TypeResourceId\"=r3.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s4  on tf.\"Id\"=s4.\"FormId\"   and s4.\"TypeResourceId\"=4  left join  \"SUB_DIC_TypeResource\" r4 on s4.\"TypeResourceId\"=r4.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s5  on tf.\"Id\"=s5.\"FormId\"   and s5.\"TypeResourceId\"=5  left join  \"SUB_DIC_TypeResource\" r5 on s5.\"TypeResourceId\"=r5.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s6  on tf.\"Id\"=s6.\"FormId\"   and s6.\"TypeResourceId\"=6  left join  \"SUB_DIC_TypeResource\" r6 on s6.\"TypeResourceId\"=r6.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s7  on tf.\"Id\"=s7.\"FormId\"   and s7.\"TypeResourceId\"=7  left join  \"SUB_DIC_TypeResource\" r7 on s7.\"TypeResourceId\"=r7.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s8 on tf.\"Id\"=s8.\"FormId\"   and s8.\"TypeResourceId\"=8  left join  \"SUB_DIC_TypeResource\" r8 on s8.\"TypeResourceId\"=r8.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s9 on tf.\"Id\"=s9.\"FormId\"   and s9.\"TypeResourceId\"=9  left join  \"SUB_DIC_TypeResource\" r9 on s9.\"TypeResourceId\"=r9.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s10 on tf.\"Id\"=s10.\"FormId\"   and s10.\"TypeResourceId\"=10  left join  \"SUB_DIC_TypeResource\" r10 on s10.\"TypeResourceId\"=r10.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s11 on tf.\"Id\"=s11.\"FormId\"   and s11.\"TypeResourceId\"=11  left join  \"SUB_DIC_TypeResource\" r11 on s11.\"TypeResourceId\"=r11.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s12 on tf.\"Id\"=s12.\"FormId\"   and s12.\"TypeResourceId\"=12  left join  \"SUB_DIC_TypeResource\" r12 on s12.\"TypeResourceId\"=r12.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s13 on tf.\"Id\"=s13.\"FormId\"   and s13.\"TypeResourceId\"=13  left join  \"SUB_DIC_TypeResource\" r13 on s13.\"TypeResourceId\"=r13.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s14 on tf.\"Id\"=s14.\"FormId\"   and s14.\"TypeResourceId\"=14  left join  \"SUB_DIC_TypeResource\" r14 on s14.\"TypeResourceId\"=r14.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s15 on tf.\"Id\"=s15.\"FormId\"   and s15.\"TypeResourceId\"=15  left join  \"SUB_DIC_TypeResource\" r15 on s15.\"TypeResourceId\"=r15.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s16 on tf.\"Id\"=s16.\"FormId\"   and s16.\"TypeResourceId\"=16  left join  \"SUB_DIC_TypeResource\" r16 on s16.\"TypeResourceId\"=r16.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s17 on tf.\"Id\"=s17.\"FormId\"   and s17.\"TypeResourceId\"=17  left join  \"SUB_DIC_TypeResource\" r17 on s17.\"TypeResourceId\"=r17.\"Id\"   "
                            + " left join  \"SUB_Form2Record\"  s18 on tf.\"Id\"=s18.\"FormId\"   and s18.\"TypeResourceId\"=18  left join  \"SUB_DIC_TypeResource\" r18 on s18.\"TypeResourceId\"=r18.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s19 on tf.\"Id\"=s19.\"FormId\"   and s19.\"TypeResourceId\"=19  left join  \"SUB_DIC_TypeResource\" r19 on s19.\"TypeResourceId\"=r19.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s20 on tf.\"Id\"=s20.\"FormId\"   and s20.\"TypeResourceId\"=20  left join  \"SUB_DIC_TypeResource\" r20 on s20.\"TypeResourceId\"=r20.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s21 on tf.\"Id\"=s21.\"FormId\"   and s21.\"TypeResourceId\"=21  left join  \"SUB_DIC_TypeResource\" r21 on s21.\"TypeResourceId\"=r21.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s22 on tf.\"Id\"=s22.\"FormId\"   and s22.\"TypeResourceId\"=22  left join  \"SUB_DIC_TypeResource\" r22 on s22.\"TypeResourceId\"=r22.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s23 on tf.\"Id\"=s23.\"FormId\"   and s23.\"TypeResourceId\"=23  left join  \"SUB_DIC_TypeResource\" r23 on s23.\"TypeResourceId\"=r23.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s24 on tf.\"Id\"=s24.\"FormId\"   and s24.\"TypeResourceId\"=24  left join  \"SUB_DIC_TypeResource\" r24 on s24.\"TypeResourceId\"=r24.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s25 on tf.\"Id\"=s25.\"FormId\"   and s25.\"TypeResourceId\"=25  left join  \"SUB_DIC_TypeResource\" r25 on s25.\"TypeResourceId\"=r25.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s26 on tf.\"Id\"=s26.\"FormId\"   and s26.\"TypeResourceId\"=26  left join  \"SUB_DIC_TypeResource\" r26 on s26.\"TypeResourceId\"=r26.\"Id\"    "
                            + " left join  \"SUB_Form2Record\"  s27 on tf.\"Id\"=s27.\"FormId\"   and s27.\"TypeResourceId\"=27  left join  \"SUB_DIC_TypeResource\" r27 on s27.\"TypeResourceId\"=r27.\"Id\"  "
                            + " left join  \"SUB_Form2Record\"  s28 on tf.\"Id\"=s28.\"FormId\"   and s28.\"TypeResourceId\"=28  left join  \"SUB_DIC_TypeResource\" r28 on s28.\"TypeResourceId\"=r28.\"Id\"  "
                            + " left join \"SUB_Form2Gu\" f2gu on f2gu.\"FormId\"=tf.\"Id\" "
                            + " left join (select \"FormId\", sum(\"ExpenceEnergy\") sum_expenceenergy from  \"SUB_Form2Record\" group by  1) as f2sum on tf.\"Id\"=f2sum.\"FormId\" "
                            + "  cons_query  "
                            + " where  t5.\"ReportYear\"="+year
                            + "   and t.\"IsDeleted\"=false   and t1.\"IsDeleted\"=FALSE";
            #endregion

            return query;
        }
        
        private string getQuery(int year, bool isTut)
        {
            #region query
            string query = " select t.\"Id\"  as rst_id , t.\"BINIIN\" as bin , t.\"IDK\" as idk, "
                     + " case '" + lang + "' when 'kz' then case tf.\"IsPlan\" when true then 'ия' else 'жоқ' end else case tf.\"IsPlan\" when  true then 'да' else 'нет' end end as isplan, "
                     + " case '" + lang + "' when 'kz' then case tf.\"IsEnergyManagementSystem\" when true then 'ия' else 'жоқ' end else case tf.\"IsEnergyManagementSystem\" when  true then 'да' else 'нет' end end as isem_system,"
                     + " case '" + lang + "' when 'kz' then o2.\"NameKz\" else o2.\"NameRu\" end as oked_name "
                     + " , t1.\"Id\" as user_id , t.\"OwnerName\" as juridical_name , tf.\"Id\" as sub_form_id "
                     + " , case '" + lang + "' when 'kz' then t3.\"NameKz\" else t3.\"NameRu\" end as oblast_name , t3.\"Id\" as oblast_id "
                     + " , t1.\"FSCode\" as fscode "
                     + " , case '" + lang + "' when 'kz' then  case t1.\"FSCode\" when 1 then 'ЗТ' when 2 then 'КВ' when 3 then 'ММ' when 4 then 'ЖК' else 'м/ж'end  "
                     + " else  case t1.\"FSCode\" when 1 then 'ЮР' when 2 then 'КВ' when 3 then 'ГУ' when 4 then 'ИП' else 'н/д' end  end as fscode_name "
                     + " ,case '" + lang + "' when 'kz' then case t.\"IsExcluded\" when true then 'Шығарылған' else 'Шығарылмаған' end  "
                     + " else case t.\"IsExcluded\" when true then 'Исключен' else 'Не исключен' end end as excluded_name , t.\"IsExcluded\" as isexcluded  "
                     + " ,t4.\"Id\" as expectant_id , case '" + lang + "' when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as expectant_name "
                     + " , sumpot.consumption  "
                    + " ,(COALESCE(r1.\"Keof\",1)*COALESCE(s1.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r2.\"Keof\",1)*COALESCE(s2.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r3.\"Keof\",1)*COALESCE(s3.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r4.\"Keof\",1)*COALESCE(s4.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r5.\"Keof\",1)*COALESCE(s5.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r6.\"Keof\",1)*COALESCE(s6.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r7.\"Keof\",1)*COALESCE(s7.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r8.\"Keof\",1)*COALESCE(s8.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r9.\"Keof\",1)*COALESCE(s9.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r10.\"Keof\",1)*COALESCE(s10.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r11.\"Keof\",1)*COALESCE(s11.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r12.\"Keof\",1)*COALESCE(s12.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r13.\"Keof\",1)*COALESCE(s13.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r14.\"Keof\",1)*COALESCE(s14.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r15.\"Keof\",1)*COALESCE(s15.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r16.\"Keof\",1)*COALESCE(s16.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r17.\"Keof\",1)*COALESCE(s17.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r18.\"Keof\",1)*COALESCE(s18.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r19.\"Keof\",1)*COALESCE(s19.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r20.\"Keof\",1)*COALESCE(s20.\"NotOwnSource\",0))+ "
                    + " (COALESCE(r21.\"Keof\",1)*COALESCE(s21.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r22.\"Keof\",1)*COALESCE(s22.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r23.\"Keof\",1)*COALESCE(s23.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r24.\"Keof\",1)*COALESCE(s24.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r25.\"Keof\",1)*COALESCE(s25.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r26.\"Keof\",1)*COALESCE(s26.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r27.\"Keof\",1)*COALESCE(s27.\"NotOwnSource\",0))+  "
                    + " (COALESCE(r28.\"Keof\",1)*COALESCE(s28.\"NotOwnSource\",0)) as tut "
                    + "  "
                    + " ,  case " + isTut + " when true then (r1.\"Keof\"*s1.\"NotOwnSource\") else s1.\"NotOwnSource\" end  as noS1 "
                    + " ,  case " + isTut + " when true then (r2.\"Keof\"*s2.\"NotOwnSource\") else s2.\"NotOwnSource\" end  as noS2 "
                    + " ,  case " + isTut + " when true then (r3.\"Keof\"*s3.\"NotOwnSource\") else s3.\"NotOwnSource\" end  as noS3 "
                    + " ,  case " + isTut + " when true then (r4.\"Keof\"*s4.\"NotOwnSource\") else s4.\"NotOwnSource\" end  as noS4 "
                    + " ,  case " + isTut + " when true then (r5.\"Keof\"*s5.\"NotOwnSource\") else s5.\"NotOwnSource\" end  as noS5 "
                    + " ,  case " + isTut + " when true then (r6.\"Keof\"*s6.\"NotOwnSource\") else s6.\"NotOwnSource\" end  as noS6 "
                    + " ,  case " + isTut + " when true then (r7.\"Keof\"*s7.\"NotOwnSource\") else s7.\"NotOwnSource\" end  as noS7 "
                    + " ,  case " + isTut + " when true then (r8.\"Keof\"*s8.\"NotOwnSource\") else s8.\"NotOwnSource\" end  as noS8 "
                    + " ,  case " + isTut + " when true then (r9.\"Keof\"*s9.\"NotOwnSource\") else s9.\"NotOwnSource\" end  as noS9 "
                    + " ,  case " + isTut + " when true then (r10.\"Keof\"*s10.\"NotOwnSource\") else s10.\"NotOwnSource\" end  as noS10 "
                    + " ,  case " + isTut + " when true then (r11.\"Keof\"*s11.\"NotOwnSource\") else s11.\"NotOwnSource\" end  as noS11 "
                    + " ,  case " + isTut + " when true then (r12.\"Keof\"*s12.\"NotOwnSource\") else s12.\"NotOwnSource\" end  as noS12 "
                    + " ,  case " + isTut + " when true then (r13.\"Keof\"*s13.\"NotOwnSource\") else s13.\"NotOwnSource\" end  as noS13 "
                    + " ,  case " + isTut + " when true then (r14.\"Keof\"*s14.\"NotOwnSource\") else s14.\"NotOwnSource\" end  as noS14 "
                    + " ,  case " + isTut + " when true then (r15.\"Keof\"*s15.\"NotOwnSource\") else s15.\"NotOwnSource\" end  as noS15 "
                    + " ,  case " + isTut + " when true then (r16.\"Keof\"*s16.\"NotOwnSource\") else s16.\"NotOwnSource\" end  as noS16 "
                    + " ,  case " + isTut + " when true then (r17.\"Keof\"*s17.\"NotOwnSource\") else s17.\"NotOwnSource\" end  as noS17 "
                    + " ,  case " + isTut + " when true then (r18.\"Keof\"*s18.\"NotOwnSource\") else s18.\"NotOwnSource\" end  as noS18 "
                    + " ,  case " + isTut + " when true then (r19.\"Keof\"*s19.\"NotOwnSource\") else s19.\"NotOwnSource\" end  as noS19 "
                    + " ,  case " + isTut + " when true then (r20.\"Keof\"*s20.\"NotOwnSource\") else s20.\"NotOwnSource\" end  as noS20 "
                    + " ,  case " + isTut + " when true then (r21.\"Keof\"*s21.\"NotOwnSource\") else s21.\"NotOwnSource\" end  as noS21 "
                    + " ,  case " + isTut + " when true then (r22.\"Keof\"*s22.\"NotOwnSource\") else s22.\"NotOwnSource\" end  as noS22 "
                    + " ,  case " + isTut + " when true then (r23.\"Keof\"*s23.\"NotOwnSource\") else s23.\"NotOwnSource\" end  as noS23 "
                    + " ,  case " + isTut + " when true then (r24.\"Keof\"*s24.\"NotOwnSource\") else s24.\"NotOwnSource\" end  as noS24 "
                    + " ,  case " + isTut + " when true then (r25.\"Keof\"*s25.\"NotOwnSource\") else s25.\"NotOwnSource\" end  as noS25 "
                    + " ,  case " + isTut + " when true then (r26.\"Keof\"*s26.\"NotOwnSource\") else s26.\"NotOwnSource\" end  as noS26 "
                    + " ,  case " + isTut + " when true then (r27.\"Keof\"*s27.\"NotOwnSource\") else s27.\"NotOwnSource\" end  as noS27 "
                    + " ,  case " + isTut + " when true then (r28.\"Keof\"*s28.\"NotOwnSource\") else s28.\"NotOwnSource\" end  as noS28 "
                    + "  , f2gu.\"CountOfEmployees\" as countOfEmployees,  f2gu.\"CountOfStudents\" as countOfStudents , f2gu.\"CountOfBeds\" as countOfBeds "
                    + "  from  \"RST_ReportReestr\" t "
                    + "  "
                    + " inner join \"RST_Report\" t5 on t.\"ReportId\"=t5.\"Id\" "
                    + " inner join \"SEC_User\"  t1 on t1.\"Id\"=t.\"UserId\" "
                    + " left join \"DIC_OKED\" o1 on t1.\"OkedId\"=o1.\"Id\" "
                    + " left join \"DIC_OKED\" o2 on o1.\"refParent\"=o2.\"Id\" "
                    + " left join \"SUB_Form\" tf on t.\"FormId\"=tf.\"Id\" "
                    + "left join \"DIC_Kato\" t3 on t.\"Oblast\"=t3.\"Id\" "
                    + " left join \"RST_DIC_Reason\" t4 on t.\"Expectant\"=t4.\"Id\"  "

                    + " left join  \"SUB_Form2Record\"  s1  on tf.\"Id\"=s1.\"FormId\"  and s1.\"TypeResourceId\"=1 "
                    + " left join  \"SUB_DIC_TypeResource\" r1 on s1.\"TypeResourceId\"=r1.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s2  on tf.\"Id\"=s2.\"FormId\"  and s2.\"TypeResourceId\"=2 "
                    + " left join  \"SUB_DIC_TypeResource\" r2 on s2.\"TypeResourceId\"=r2.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s3  on tf.\"Id\"=s3.\"FormId\"  and s3.\"TypeResourceId\"=3 "
                    + " left join  \"SUB_DIC_TypeResource\" r3 on s3.\"TypeResourceId\"=r3.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s4  on tf.\"Id\"=s4.\"FormId\"   and s4.\"TypeResourceId\"=4 "
                    + " left join  \"SUB_DIC_TypeResource\" r4 on s4.\"TypeResourceId\"=r4.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s5  on tf.\"Id\"=s5.\"FormId\"   and s5.\"TypeResourceId\"=5 "
                    + " left join  \"SUB_DIC_TypeResource\" r5 on s5.\"TypeResourceId\"=r5.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s6  on tf.\"Id\"=s6.\"FormId\"   and s6.\"TypeResourceId\"=6 "
                    + " left join  \"SUB_DIC_TypeResource\" r6 on s6.\"TypeResourceId\"=r6.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s7  on tf.\"Id\"=s7.\"FormId\"   and s7.\"TypeResourceId\"=7 "
                    + " left join  \"SUB_DIC_TypeResource\" r7 on s7.\"TypeResourceId\"=r7.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s8 on tf.\"Id\"=s8.\"FormId\"   and s8.\"TypeResourceId\"=8 "
                    + " left join  \"SUB_DIC_TypeResource\" r8 on s8.\"TypeResourceId\"=r8.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s9 on tf.\"Id\"=s9.\"FormId\"   and s9.\"TypeResourceId\"=9 "
                    + " left join  \"SUB_DIC_TypeResource\" r9 on s9.\"TypeResourceId\"=r9.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s10 on tf.\"Id\"=s10.\"FormId\"   and s10.\"TypeResourceId\"=10 "
                    + " left join  \"SUB_DIC_TypeResource\" r10 on s10.\"TypeResourceId\"=r10.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s11 on tf.\"Id\"=s11.\"FormId\"   and s11.\"TypeResourceId\"=11 "
                    + " left join  \"SUB_DIC_TypeResource\" r11 on s11.\"TypeResourceId\"=r11.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s12 on tf.\"Id\"=s12.\"FormId\"   and s12.\"TypeResourceId\"=12 "
                    + " left join  \"SUB_DIC_TypeResource\" r12 on s12.\"TypeResourceId\"=r12.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s13 on tf.\"Id\"=s13.\"FormId\"   and s13.\"TypeResourceId\"=13 "
                    + " left join  \"SUB_DIC_TypeResource\" r13 on s13.\"TypeResourceId\"=r13.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s14 on tf.\"Id\"=s14.\"FormId\"   and s14.\"TypeResourceId\"=14 "
                    + " left join  \"SUB_DIC_TypeResource\" r14 on s14.\"TypeResourceId\"=r14.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s15 on tf.\"Id\"=s15.\"FormId\"   and s15.\"TypeResourceId\"=15 "
                    + " left join  \"SUB_DIC_TypeResource\" r15 on s15.\"TypeResourceId\"=r15.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s16 on tf.\"Id\"=s16.\"FormId\"   and s16.\"TypeResourceId\"=16 "
                    + " left join  \"SUB_DIC_TypeResource\" r16 on s16.\"TypeResourceId\"=r16.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s17 on tf.\"Id\"=s17.\"FormId\"   and s17.\"TypeResourceId\"=17 "
                    + " left join  \"SUB_DIC_TypeResource\" r17 on s17.\"TypeResourceId\"=r17.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s18 on tf.\"Id\"=s18.\"FormId\"   and s18.\"TypeResourceId\"=18 "
                    + " left join  \"SUB_DIC_TypeResource\" r18 on s18.\"TypeResourceId\"=r18.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s19 on tf.\"Id\"=s19.\"FormId\"   and s19.\"TypeResourceId\"=19 "
                    + " left join  \"SUB_DIC_TypeResource\" r19 on s19.\"TypeResourceId\"=r19.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s20 on tf.\"Id\"=s20.\"FormId\"   and s20.\"TypeResourceId\"=20 "
                    + " left join  \"SUB_DIC_TypeResource\" r20 on s20.\"TypeResourceId\"=r20.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s21 on tf.\"Id\"=s21.\"FormId\"   and s21.\"TypeResourceId\"=21 "
                    + " left join  \"SUB_DIC_TypeResource\" r21 on s21.\"TypeResourceId\"=r21.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s22 on tf.\"Id\"=s22.\"FormId\"   and s22.\"TypeResourceId\"=22 "
                    + " left join  \"SUB_DIC_TypeResource\" r22 on s22.\"TypeResourceId\"=r22.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s23 on tf.\"Id\"=s23.\"FormId\"   and s23.\"TypeResourceId\"=23 "
                    + " left join  \"SUB_DIC_TypeResource\" r23 on s23.\"TypeResourceId\"=r23.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s24 on tf.\"Id\"=s24.\"FormId\"   and s24.\"TypeResourceId\"=24 "
                    + " left join  \"SUB_DIC_TypeResource\" r24 on s24.\"TypeResourceId\"=r24.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s25 on tf.\"Id\"=s25.\"FormId\"   and s25.\"TypeResourceId\"=25 "
                    + " left join  \"SUB_DIC_TypeResource\" r25 on s25.\"TypeResourceId\"=r25.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s26 on tf.\"Id\"=s26.\"FormId\"   and s26.\"TypeResourceId\"=26 "
                    + " left join  \"SUB_DIC_TypeResource\" r26 on s26.\"TypeResourceId\"=r26.\"Id\" "
                    + "  "
                    + " left join  \"SUB_Form2Record\"  s27 on tf.\"Id\"=s27.\"FormId\"   and s27.\"TypeResourceId\"=27 "
                    + " left join  \"SUB_DIC_TypeResource\" r27 on s27.\"TypeResourceId\"=r27.\"Id\" "
                    + " left join  \"SUB_Form2Record\"  s28 on tf.\"Id\"=s28.\"FormId\"   and s28.\"TypeResourceId\"=28 "
                    + " left join  \"SUB_DIC_TypeResource\" r28 on s28.\"TypeResourceId\"=r28.\"Id\" "
                    + " left join \"SUB_Form2Gu\" f2gu on f2gu.\"FormId\"=tf.\"Id\" "
                      + "  cons_query "
                    + " where  t5.\"ReportYear\"=" + year + " and t.\"IsDeleted\"=false  and t1.\"IsDeleted\"=FALSE";

            #endregion

            return query;
        }

        public string GetConsumptionQuery()
		{
			string query = " left join  (  select  f2.\"FormId\" ,  sum ( case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
					+ " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\",0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce( tr.\"Keof\", 1)  else            "
					+ "  (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\",0))*coalesce(tr.\"Keof\", 1) end  ) as consumption  "
					+ " from  \"SUB_Form2Record\" f2        inner join  \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\"   group by f2.\"FormId\" ) "
					+ " as sumpot on sumpot.\"FormId\"=tf.\"Id\" ";
			return query;
		}

		public string getDicColumns(ref List<SelectListItem> ListItems)
		{
			string errorMessage = "";
			try
			{
				ListItems.Add(new SelectListItem() { Value = "NotOwnSource", Text = (lang == "kz") ? "Меншікті емес көздерден алынған энергетикалық ресурстарын тұтыну(5а)" : "Потребление энергоресурсов полученные НЕ из собственных источников(5а)" });
				ListItems.Add(new SelectListItem() { Value = "ExtractVolume", Text = (lang == "kz") ? "Өндірілген/өңделген ОЭР көлемі(4)" : "Объем добытых(4)" });
				ListItems.Add(new SelectListItem() { Value = "LosEnergy", Text = (lang == "kz") ? "ОЭР тасымалдау кезіндегі кемулер(5б)" : "Потери при транспортировке ТЭР(5б)" });
				ListItems.Add(new SelectListItem() { Value = "OwnSource", Text = (lang == "kz") ? "Меншікті көздерден алынған энергетикалық ресурстарын тұтыну(5а)" : "Потребление энергоресурсов, полученных из собственных источников(6)" });
				ListItems.Add(new SelectListItem() { Value = "TransferOtherLegal", Text = (lang == "kz") ? "Басқа заңды тұлғаларға және жеке тұлғаларға берілген (өткізілген) энергетикалық ресурстары(7)" : "Энергоресурсы, переданные (реализованные) другим юридическим и физическим лицам(7)" });
				ListItems.Add(new SelectListItem() { Value = "ExpenceEnergy", Text = (lang == "kz") ? "Энергетикалық ресурстарын сатып алуға жұмсалған шығындар (5а+6 бағаналар сомасы), мың. теңге (ҚҚС есебімен)(8)" : "Расходы на приобретение энергоресурсов (сумма столбцов 5а+6), тенге (с учетом НДС)(8)" });

			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

		public string EditRst_ReportReestr(long rst_id, bool isexcluded, long? expactant_id,int? fscode)
		{
			string errorMessage = "";
			try
			{
				var item = AppContext.RST_ReportReestr.Find(rst_id);
				item.IsExcluded = isexcluded;
				item.Expectant = expactant_id;
                item.usrfscode = fscode;
				int result = AppContext.SaveChanges();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

		//----Форма собственности
		public List<SelectListItem> GetDicTypeApplication()
		{
			//----
			List<SelectListItem> ListItems = new List<SelectListItem>();
			ListItems.Add(new SelectListItem() { Value = "1", Text = (lang == "kz") ? "ЗТ":"ЮР" });
			ListItems.Add(new SelectListItem() { Value = "2", Text = (lang == "kz") ? "КВ":"КВ"  });
			ListItems.Add(new SelectListItem() { Value = "3", Text = (lang == "kz") ? "ММ":"ГУ" });
			ListItems.Add(new SelectListItem() { Value = "4", Text = (lang == "kz") ? "ЖК":"ИП" });
			ListItems.Add(new SelectListItem() { Value = "", Text =ResourceSetting.sNoData  });			

			return ListItems;
		}

		//----
		public List<SelectListItem> GetDicOked()
		{
			var ListItems = new List<SelectListItem>();
			var okedRepository = new DicOkedRepository();
			var okedList = okedRepository.GetAll().Where(x => x.refParent == null).ToList();
			foreach (var item in okedList)
			{
				ListItems.Add(new SelectListItem() { Value = item.Id.ToString(), Text = (lang.Equals("kz")) ? item.NameKz : item.NameRu });
			}
			ListItems.Add(new SelectListItem() { Value = "", Text =ResourceSetting.sNoData }); 

			return ListItems;
		}

		//----
		public List<SelectListItem> GetDicKato()
		{
			var ListItems = new List<SelectListItem>();
			var repository = new KatoRepository();
			var list = repository.GetKatos(1, true);

			ListItems.Add(new SelectListItem() { Value = "-1", Text =ResourceSetting.sByRepublic});
			foreach (var item in list)
			{
				ListItems.Add(new SelectListItem() { Value = item.Id.ToString(), Text = (lang.Equals("kz")) ? item.NameKz : item.NameRu });
			}

			return ListItems;
		}

		//---- helper function
		public string replaceToComma(string val)
		{

			while (val.IndexOf("*") != -1)
			{
				val = val.Replace("*", ",");
			}
			return val;
		}

        //---- итого
        public string getNotOwnSourceSum(int year, ref SourceControllerClassSum item, string oblast_ids, string reason_ids, int? excluded_id, string expectant_ids, bool istut, long restype_id, double? min, double? max, int orderById, string name_oked_idk, string fscode, string oked_ids, int isplan, int isem_system)
        {
            string ErrorMessage = string.Empty;
            try
            {

               
                string where = "";

                //----окэд идк наименование
                if (!string.IsNullOrWhiteSpace(name_oked_idk))
                    where += " and ( upper(t.\"IDK\") like '%" + name_oked_idk.ToUpper() + "%'  or upper(o2.\"NameRu\") like '%" + name_oked_idk.ToUpper() + "%' or upper(t.\"OwnerName\") like '%" + name_oked_idk.ToUpper() + "%' or t1.\"BINIIN\" like '%" + name_oked_idk + "%' ) ";

                //----oblast
                if (!oblast_ids.Equals("-1"))
                {
                    where += " and t.\"Oblast\" in (" + replaceToComma(oblast_ids) + ")";
                }

                //----reason
                if (!string.IsNullOrWhiteSpace(reason_ids))
                {
                    where += " and t.\"ReasonId\" in (" + replaceToComma(reason_ids) + ")";
                }

                //----expectant
                if (!string.IsNullOrWhiteSpace(expectant_ids))
                {
                    if (expectant_ids.IndexOf("null") == -1)
                    {
                        where = where + " and t.\"Expectant\" in (" + replaceToComma(expectant_ids) + ")";
                    }
                    else
                    {
                        var arr = expectant_ids.Split('*');
                        if (arr.Length > 1)
                        {
                            var ids = "";
                            for (int i = 0; i < arr.Length; i++)
                                if (arr[i] != "null")
                                {
                                    ids = arr[i] + ",";
                                }

                            ids = ids.TrimEnd(',');
                            where = where + " and ( (t.\"Expectant\" is null) or ( t.\"Expectant\" in (" + ids + ") ))";
                        }
                        else where = where + " and t.\"Expectant\" is null ";
                    }
                }

                //----
                if (excluded_id != 0)
                {
                    long l;
                    Int64.TryParse(Convert.ToString(excluded_id), out l);
                    switch (l)
                    {
                        case CodeConstManager.RST_EXCLUDED_ID:
                            {
                                where = where + " and t.\"IsExcluded\"=TRUE ";
                                break;
                            }
                        case CodeConstManager.RST_NOTEXCLUDED_ID:
                            {
                                where = where + " and t.\"IsExcluded\"=FALSE ";
                                break;
                            }
                    }
                }

                //----fscode
                if (!string.IsNullOrWhiteSpace(fscode))
                {
                    if (fscode.IndexOf("null") == -1)
                    {
                        where = where + " and t.usrfscode in (" + replaceToComma(fscode) + ")";
                    }
                    else
                    {
                        var arr = fscode.Split('*');
                        if (arr.Length > 1)
                        {
                            var ids = "";
                            for (int i = 0; i < arr.Length; i++)
                                if (arr[i] != "null")
                                {
                                    ids = arr[i] + ",";
                                }

                            ids = ids.TrimEnd(',');
                            where = where + " and ( (t.usrfscode is null) or ( t.usrfscode in (" + ids + ") ))";
                        }
                        else where = where + " and t.usrfscode is null ";
                    }

                }

                //----oked
                if (!string.IsNullOrWhiteSpace(oked_ids))
                {
                    if (oked_ids.IndexOf("null") == -1)
                    {
                        where = where + " and o2.\"RootId\" in (" + replaceToComma(oked_ids) + ")";
                    }
                    else
                    {
                        var arr = oked_ids.Split('*');
                        if (arr.Length > 1)
                        {
                            var ids = "";
                            for (int i = 0; i < arr.Length; i++)
                                if (arr[i] != "null")
                                {
                                    ids = arr[i] + ",";
                                }

                            ids = ids.TrimEnd(',');
                            where = where + " and ( (o2.\"RootId\" is null) or ( o2.\"RootId\" in (" + ids + ") ))";
                        }
                        else where = where + " and o2.\"RootId\" is null ";
                    }
                }

                //----isplan
                if (isplan != -1)
                {

                    where = where + " and tf.\"IsPlan\"=" + ((isplan == 1) ? "true" : "false");
                }

                //----isem_system
                if (isem_system != -1)
                {

                    where = where + " and tf.\"IsEnergyManagementSystem\"=" + ((isem_system == 1) ? "true" : "false");
                }


                if (restype_id != 0 && restype_id != -1)
                {

                    string colName = "s" + restype_id;
                    string colName2 = "r" + restype_id;
                    where += " and " + colName + ".\"TypeResourceId\"=" + restype_id;

                    string notOwnRes = colName + ".\"NotOwnSource\" ";
                    if (istut)
                        notOwnRes = " (" + colName2 + ".\"Keof\"*" + colName + ".\"NotOwnSource\") ";


                    if (min != 0 && max != 0)
                        where += " and " + notOwnRes + ">=" + min + " and " + notOwnRes + "<=" + max;
                    else if (min != 0)
                        where += " and " + notOwnRes + ">=" + min;
                    else if (max != 0)
                        where += " and " + notOwnRes + "<=" + max;

                }

                #region query
                string query = " select sum( "
                          + " (COALESCE(r1.\"Keof\",1)*COALESCE(s1.\"NotOwnSource\",0))+   (COALESCE(r2.\"Keof\",1)*COALESCE(s2.\"NotOwnSource\",0))+  (COALESCE(r3.\"Keof\",1)*COALESCE(s3.\"NotOwnSource\",0))+  "
                          + " (COALESCE(r4.\"Keof\",1)*COALESCE(s4.\"NotOwnSource\",0))+  (COALESCE(r5.\"Keof\",1)*COALESCE(s5.\"NotOwnSource\",0))+  (COALESCE(r6.\"Keof\",1)*COALESCE(s6.\"NotOwnSource\",0))+   "
                          + " (COALESCE(r7.\"Keof\",1)*COALESCE(s7.\"NotOwnSource\",0))+  (COALESCE(r8.\"Keof\",1)*COALESCE(s8.\"NotOwnSource\",0))+  (COALESCE(r9.\"Keof\",1)*COALESCE(s9.\"NotOwnSource\",0))+   "
                          + " (COALESCE(r10.\"Keof\",1)*COALESCE(s10.\"NotOwnSource\",0))+  (COALESCE(r11.\"Keof\",1)*COALESCE(s11.\"NotOwnSource\",0))+  "
                          + " (COALESCE(r12.\"Keof\",1)*COALESCE(s12.\"NotOwnSource\",0))+  (COALESCE(r13.\"Keof\",1)*COALESCE(s13.\"NotOwnSource\",0))+  "
                          + " (COALESCE(r14.\"Keof\",1)*COALESCE(s14.\"NotOwnSource\",0))+  (COALESCE(r15.\"Keof\",1)*COALESCE(s15.\"NotOwnSource\",0))+  "
                          + " (COALESCE(r16.\"Keof\",1)*COALESCE(s16.\"NotOwnSource\",0))+  (COALESCE(r17.\"Keof\",1)*COALESCE(s17.\"NotOwnSource\",0))+   "
                          + " (COALESCE(r18.\"Keof\",1)*COALESCE(s18.\"NotOwnSource\",0))+  (COALESCE(r19.\"Keof\",1)*COALESCE(s19.\"NotOwnSource\",0))+   "
                          + " (COALESCE(r20.\"Keof\",1)*COALESCE(s20.\"NotOwnSource\",0))+  (COALESCE(r21.\"Keof\",1)*COALESCE(s21.\"NotOwnSource\",0))+    "
                          + " (COALESCE(r22.\"Keof\",1)*COALESCE(s22.\"NotOwnSource\",0))+   (COALESCE(r23.\"Keof\",1)*COALESCE(s23.\"NotOwnSource\",0))+    "
                          + " (COALESCE(r24.\"Keof\",1)*COALESCE(s24.\"NotOwnSource\",0))+   (COALESCE(r25.\"Keof\",1)*COALESCE(s25.\"NotOwnSource\",0))+    "
                          + " (COALESCE(r26.\"Keof\",1)*COALESCE(s26.\"NotOwnSource\",0))+   (COALESCE(r27.\"Keof\",1)*COALESCE(s27.\"NotOwnSource\",0))+    "
                          + " (COALESCE(r28.\"Keof\",1)*COALESCE(s28.\"NotOwnSource\",0))) as tut     "
                          + "  , sum(  case False when true then (r1.\"Keof\"*s1.\"NotOwnSource\") else s1.\"NotOwnSource\" end ) as noS1   "
                          + "  , sum(  case False when true then (r2.\"Keof\"*s2.\"NotOwnSource\") else s2.\"NotOwnSource\" end ) as noS2   "
                          + "  , sum(  case False when true then (r3.\"Keof\"*s3.\"NotOwnSource\") else s3.\"NotOwnSource\" end ) as noS3   "
                          + "  , sum(  case False when true then (r4.\"Keof\"*s4.\"NotOwnSource\") else s4.\"NotOwnSource\" end ) as noS4   "
                          + "  , sum(  case False when true then (r5.\"Keof\"*s5.\"NotOwnSource\") else s5.\"NotOwnSource\" end ) as noS5   "
                          + "  , sum(  case False when true then (r6.\"Keof\"*s6.\"NotOwnSource\") else s6.\"NotOwnSource\" end ) as noS6   "
                          + "  , sum(  case False when true then (r7.\"Keof\"*s7.\"NotOwnSource\") else s7.\"NotOwnSource\" end ) as noS7   "
                          + "  , sum(  case False when true then (r8.\"Keof\"*s8.\"NotOwnSource\") else s8.\"NotOwnSource\" end ) as noS8   "
                          + "  , sum(  case False when true then (r9.\"Keof\"*s9.\"NotOwnSource\") else s9.\"NotOwnSource\" end ) as noS9   "
                          + "  , sum( case False when true then (r10.\"Keof\"*s10.\"NotOwnSource\") else s10.\"NotOwnSource\" end ) as noS10   "
                          + "  , sum( case False when true then (r11.\"Keof\"*s11.\"NotOwnSource\") else s11.\"NotOwnSource\" end ) as noS11   "
                          + "  , sum( case False when true then (r12.\"Keof\"*s12.\"NotOwnSource\") else s12.\"NotOwnSource\" end ) as noS12   "
                          + "  , sum( case False when true then (r13.\"Keof\"*s13.\"NotOwnSource\") else s13.\"NotOwnSource\" end ) as noS13   "
                          + "  , sum( case False when true then (r14.\"Keof\"*s14.\"NotOwnSource\") else s14.\"NotOwnSource\" end ) as noS14   "
                          + "  , sum( case False when true then (r15.\"Keof\"*s15.\"NotOwnSource\") else s15.\"NotOwnSource\" end ) as noS15   "
                          + "  , sum( case False when true then (r16.\"Keof\"*s16.\"NotOwnSource\") else s16.\"NotOwnSource\" end ) as noS16   "
                          + "  , sum( case False when true then (r17.\"Keof\"*s17.\"NotOwnSource\") else s17.\"NotOwnSource\" end ) as noS17   "
                          + "  , sum( case False when true then (r18.\"Keof\"*s18.\"NotOwnSource\") else s18.\"NotOwnSource\" end ) as noS18   "
                          + "  , sum( case False when true then (r19.\"Keof\"*s19.\"NotOwnSource\") else s19.\"NotOwnSource\" end ) as noS19   "
                          + "  , sum( case False when true then (r20.\"Keof\"*s20.\"NotOwnSource\") else s20.\"NotOwnSource\" end ) as noS20   "
                          + "  , sum( case False when true then (r21.\"Keof\"*s21.\"NotOwnSource\") else s21.\"NotOwnSource\" end ) as noS21   "
                          + "  , sum( case False when true then (r22.\"Keof\"*s22.\"NotOwnSource\") else s22.\"NotOwnSource\" end ) as noS22   "
                          + "  , sum( case False when true then (r23.\"Keof\"*s23.\"NotOwnSource\") else s23.\"NotOwnSource\" end ) as noS23   "
                          + "  , sum( case False when true then (r24.\"Keof\"*s24.\"NotOwnSource\") else s24.\"NotOwnSource\" end ) as noS24   "
                          + "  , sum( case False when true then (r25.\"Keof\"*s25.\"NotOwnSource\") else s25.\"NotOwnSource\" end ) as noS25   "
                          + "  , sum( case False when true then (r26.\"Keof\"*s26.\"NotOwnSource\") else s26.\"NotOwnSource\" end ) as noS26   "
                          + "  , sum( case False when true then (r27.\"Keof\"*s27.\"NotOwnSource\") else s27.\"NotOwnSource\" end ) as noS27   "
                          + "  , sum( case False when true then (r28.\"Keof\"*s28.\"NotOwnSource\") else s28.\"NotOwnSource\" end ) as noS28    "
                          + "  , sum(f2gu.\"CountOfEmployees\" ) as countOfEmployees "
                          + "  , sum( f2gu.\"CountOfStudents\" ) as countOfStudents  "
                          + "  , sum(f2gu.\"CountOfBeds\" ) as countOfBeds    "
                          + " from  \"RST_ReportReestr\" t     "
                          + " inner join \"RST_Report\" t5 on t.\"ReportId\"=t5.\"Id\"   "
                          + " inner join \"SEC_User\"  t1 on t1.\"Id\"=t.\"UserId\"   "
                          + " left join \"DIC_OKED\" o1 on t1.\"OkedId\"=o1.\"Id\"   "
                          + " left join \"DIC_OKED\" o2 on o1.\"refParent\"=o2.\"Id\"   "
                          + " left join \"SUB_Form\" tf on t.\"FormId\"=tf.\"Id\"  "
                          + " left join \"DIC_Kato\" t3 on t.\"Oblast\"=t3.\"Id\"   "
                          + " left join \"RST_DIC_Reason\" t4 on t.\"Expectant\"=t4.\"Id\"    "
                          + " left join  \"SUB_Form2Record\"  s1  on tf.\"Id\"=s1.\"FormId\"  and s1.\"TypeResourceId\"=1   "
                          + " left join  \"SUB_DIC_TypeResource\" r1 on s1.\"TypeResourceId\"=r1.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s2  on tf.\"Id\"=s2.\"FormId\"  and s2.\"TypeResourceId\"=2   "
                          + " left join  \"SUB_DIC_TypeResource\" r2 on s2.\"TypeResourceId\"=r2.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s3  on tf.\"Id\"=s3.\"FormId\"  and s3.\"TypeResourceId\"=3  left join  \"SUB_DIC_TypeResource\" r3 on s3.\"TypeResourceId\"=r3.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s4  on tf.\"Id\"=s4.\"FormId\"   and s4.\"TypeResourceId\"=4  left join  \"SUB_DIC_TypeResource\" r4 on s4.\"TypeResourceId\"=r4.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s5  on tf.\"Id\"=s5.\"FormId\"   and s5.\"TypeResourceId\"=5  left join  \"SUB_DIC_TypeResource\" r5 on s5.\"TypeResourceId\"=r5.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s6  on tf.\"Id\"=s6.\"FormId\"   and s6.\"TypeResourceId\"=6  left join  \"SUB_DIC_TypeResource\" r6 on s6.\"TypeResourceId\"=r6.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s7  on tf.\"Id\"=s7.\"FormId\"   and s7.\"TypeResourceId\"=7  left join  \"SUB_DIC_TypeResource\" r7 on s7.\"TypeResourceId\"=r7.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s8 on tf.\"Id\"=s8.\"FormId\"   and s8.\"TypeResourceId\"=8  left join  \"SUB_DIC_TypeResource\" r8 on s8.\"TypeResourceId\"=r8.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s9 on tf.\"Id\"=s9.\"FormId\"   and s9.\"TypeResourceId\"=9  left join  \"SUB_DIC_TypeResource\" r9 on s9.\"TypeResourceId\"=r9.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s10 on tf.\"Id\"=s10.\"FormId\"   and s10.\"TypeResourceId\"=10  left join  \"SUB_DIC_TypeResource\" r10 on s10.\"TypeResourceId\"=r10.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s11 on tf.\"Id\"=s11.\"FormId\"   and s11.\"TypeResourceId\"=11  left join  \"SUB_DIC_TypeResource\" r11 on s11.\"TypeResourceId\"=r11.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s12 on tf.\"Id\"=s12.\"FormId\"   and s12.\"TypeResourceId\"=12  left join  \"SUB_DIC_TypeResource\" r12 on s12.\"TypeResourceId\"=r12.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s13 on tf.\"Id\"=s13.\"FormId\"   and s13.\"TypeResourceId\"=13  left join  \"SUB_DIC_TypeResource\" r13 on s13.\"TypeResourceId\"=r13.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s14 on tf.\"Id\"=s14.\"FormId\"   and s14.\"TypeResourceId\"=14  left join  \"SUB_DIC_TypeResource\" r14 on s14.\"TypeResourceId\"=r14.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s15 on tf.\"Id\"=s15.\"FormId\"   and s15.\"TypeResourceId\"=15  left join  \"SUB_DIC_TypeResource\" r15 on s15.\"TypeResourceId\"=r15.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s16 on tf.\"Id\"=s16.\"FormId\"   and s16.\"TypeResourceId\"=16  left join  \"SUB_DIC_TypeResource\" r16 on s16.\"TypeResourceId\"=r16.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s17 on tf.\"Id\"=s17.\"FormId\"   and s17.\"TypeResourceId\"=17  left join  \"SUB_DIC_TypeResource\" r17 on s17.\"TypeResourceId\"=r17.\"Id\"    "
                          + " left join  \"SUB_Form2Record\"  s18 on tf.\"Id\"=s18.\"FormId\"   and s18.\"TypeResourceId\"=18  left join  \"SUB_DIC_TypeResource\" r18 on s18.\"TypeResourceId\"=r18.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s19 on tf.\"Id\"=s19.\"FormId\"   and s19.\"TypeResourceId\"=19  left join  \"SUB_DIC_TypeResource\" r19 on s19.\"TypeResourceId\"=r19.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s20 on tf.\"Id\"=s20.\"FormId\"   and s20.\"TypeResourceId\"=20  left join  \"SUB_DIC_TypeResource\" r20 on s20.\"TypeResourceId\"=r20.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s21 on tf.\"Id\"=s21.\"FormId\"   and s21.\"TypeResourceId\"=21  left join  \"SUB_DIC_TypeResource\" r21 on s21.\"TypeResourceId\"=r21.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s22 on tf.\"Id\"=s22.\"FormId\"   and s22.\"TypeResourceId\"=22  left join  \"SUB_DIC_TypeResource\" r22 on s22.\"TypeResourceId\"=r22.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s23 on tf.\"Id\"=s23.\"FormId\"   and s23.\"TypeResourceId\"=23  left join  \"SUB_DIC_TypeResource\" r23 on s23.\"TypeResourceId\"=r23.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s24 on tf.\"Id\"=s24.\"FormId\"   and s24.\"TypeResourceId\"=24  left join  \"SUB_DIC_TypeResource\" r24 on s24.\"TypeResourceId\"=r24.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s25 on tf.\"Id\"=s25.\"FormId\"   and s25.\"TypeResourceId\"=25  left join  \"SUB_DIC_TypeResource\" r25 on s25.\"TypeResourceId\"=r25.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s26 on tf.\"Id\"=s26.\"FormId\"   and s26.\"TypeResourceId\"=26  left join  \"SUB_DIC_TypeResource\" r26 on s26.\"TypeResourceId\"=r26.\"Id\"     "
                          + " left join  \"SUB_Form2Record\"  s27 on tf.\"Id\"=s27.\"FormId\"   and s27.\"TypeResourceId\"=27  left join  \"SUB_DIC_TypeResource\" r27 on s27.\"TypeResourceId\"=r27.\"Id\"   "
                          + " left join  \"SUB_Form2Record\"  s28 on tf.\"Id\"=s28.\"FormId\"   and s28.\"TypeResourceId\"=28  left join  \"SUB_DIC_TypeResource\" r28 on s28.\"TypeResourceId\"=r28.\"Id\"   "
                          + " left join \"SUB_Form2Gu\" f2gu on f2gu.\"FormId\"=tf.\"Id\"   "
                          + " where  t5.\"ReportYear\"=" + year + "   and t.\"IsDeleted\"=false  and t1.\"IsDeleted\"=FALSE ";
                query = query + " " + where;
                #endregion


                var row = AppContext.Database.SqlQuery<SourceControllerClassSum>(query).FirstOrDefault();
                item = row;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return ErrorMessage;
        }
    }
	
	//----
	class SourceControllerHelper
	{
		public long rst_reportreestr_id { get; set; } //  sub_form_id 
		public long user_id { get; set; }
		public string idk { get; set; }
        public string bin { get; set; }
		public string oked_name { get; set; }
		public string juridical_name { get; set; }
		public Nullable<double> notownsource { get; set; }
		public long? resource_id { get; set; }
		public string resource_name { get; set; }
	}

	//----
	public class ResourceHelper
	{
		public long resource_id { get; set; }
		public string resource_name { get; set; }
		public string unit_name { get; set; }
        public string dic_type { get; set; }
	}
}