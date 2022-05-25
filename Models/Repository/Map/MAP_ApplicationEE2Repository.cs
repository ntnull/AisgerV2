using Aisger.Helpers;
using Aisger.Models.Entity.Map;
using Aisger.Models.Repository.Dictionary;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Repository.Map
{
	public class MAP_ApplicationEE2Repository: SqlRepository
    {
        private static string conName = "conName";
     
        string conString = ConfigurationManager.AppSettings[conName];
        private static string lang = CultureHelper.GetCurrentCulture();

        #region FormEE2
        public string getFormEE2ItemList(ref IEnumerable<MAP_EE2Filters> ListItems)
        {
            string errorMessage = "";
            try
            {
                string query = "select t.*  form \"MAP_FormEE2\" t ";
                var rows = AppContext.Database.SqlQuery<MAP_EE2Filters>(query).ToList();
                ListItems = rows;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public string getFormEE2ItemBySecUserId(ref MAP_EE2Filters item, long id)
        {
            string errorMessage = "";
            try
            {
                string query = "select t.*  from \"MAP_FormEE2\" t where t.\"SecUserId\"=" + id;
                var rows = AppContext.Database.SqlQuery<MAP_EE2Filters>(query).ToList();
                if (rows.Count > 0)
                    item = rows[0];
                else item.Id = -1;

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public string insertMapFormEE2(ref MAP_FormEE2 item)
        {
            string errorMessage = "";
			try
			{
				AppContext.MAP_FormEE2.Add(item);
				AppContext.SaveChanges();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

            return errorMessage;
        }

		public string updateMapFormEE2(MAP_FormEE2 item)
		{
			string errorMessage = "";
			try
			{
				var row = AppContext.MAP_FormEE2.Find(item.Id);
				row.TotalArea = item.TotalArea;
				row.FSCode = item.FSCode;
				row.EditDate = DateTime.Now;
				row.DicEEStatusId = item.DicEEStatusId;
				row.Comments = item.Comments;
				row.NumberOfStoreys = item.NumberOfStoreys;
				int cnt = AppContext.SaveChanges();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

        public string updateMapFormEE2Status(MAP_FormEE2 item)
        {
            string errorMessage = "";
			try
			{
				#region---- Connect to a PostgreSQL database

				var row = AppContext.MAP_FormEE2.Find(item.Id);
				row.EditDate = DateTime.Now;
				row.DicEEStatusId = item.DicEEStatusId;

				int count = AppContext.SaveChanges();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				#endregion

			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

            return errorMessage;
        }

        //---- егер текущий пользователь manager болса
        public string getFormEE2ByManager(ref MAP_mEE2Filters filter)
        {
            string errorMessage = "";
			try
			{
				string query = " Select  t1.\"Id\" as \"SecUserId\" , t1.\"JuridicalName\" , t1.\"BINIIN\" , t1.\"Address\" ,  t1.\"LastName\"||' '||t1.\"FirstName\" as \"User_Name\" ,  t1.\"Oblast\", t1.\"OkedId\" ,  "
								 + " case '" + lang + "' when 'ru' then t2.\"NameRu\" when 'kz' then t2.\"NameKz\" else t2.\"NameRu\" end as \"Oblast_Name\" ,  t3.\"Status_Name\" ,"
								 + " case '" + lang + "' when 'ru' then t4.\"NameRu\" when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as \"Region_Name\" , t5.\"Oked_Name\" "
								 + " FROM \"SEC_UserKind\" t "
								 + " RIGHT OUTER JOIN \"SEC_User\" t1 on t.\"UserId\"=t1.\"Id\" "
								 + " RIGHT OUTER JOIN \"DIC_Kato\" t2 on t1.\"Oblast\"=t2.\"Id\" "
								 + " LEFT OUTER JOIN (select o1.\"Id\" , o1.\"RootId\" , case '" + lang + "' when 'kz' then o2.\"NameKz\" else o2.\"NameRu\" end as \"Oked_Name\"  "
								 + "  from \"DIC_OKED\" o1 , \"DIC_OKED\" o2 where o1.\"RootId\"=o2.\"Id\" ) t5 on t1.\"OkedId\"=t5.\"Id\" "
								 + " LEFT OUTER JOIN \"DIC_Kato\" t4 on t1.\"Region\"=t4.\"Id\" "
								 + " LEFT  OUTER  JOIN   (select tt.\"SecUserId\" , tt.\"DicEEStatusId\" , case 'kz' when 'ru' then tt1.\"NameRu\" when 'kz' then tt1.\"NameKz\" else tt1.\"NameRu\" end as \"Status_Name\"   "
								 + "       from \"MAP_FormEE2\" tt ,   \"MAP_DIC_EEStatus\" tt1 where tt.\"DicEEStatusId\"=tt1.\"Id\" ) t3 on t1.\"Id\"=t3.\"SecUserId\"            "
								 + "  where  t.\"KindId\"=5 and t1.\"IsDeleted\"=false  ";

				if (!string.IsNullOrWhiteSpace(filter.Name))
				{
					query = query + " and   lower(t1.\"LastName\"||t1.\"FirstName\") like lower('%" + filter.Name + "%') ";
				}

				if (!string.IsNullOrWhiteSpace(filter.BINIIN))
				{
					query = query + " and   lower(t1.\"BINIIN\") like lower('%" + filter.BINIIN + "%') ";
				}

				if (filter.Oblasts != null && filter.Oblasts.Count > 0)
				{
					query = query + " and t1.\"Oblast\" IN (" + string.Join(",", filter.Oblasts) + ")";
				}

				if (filter.Statuses != null && filter.Statuses.Count > 0)
				{
					query = query + " and t3.\"DicEEStatusId\" IN (" + string.Join(",", filter.Statuses) + ")";
				}

				if (filter.Okeds != null && filter.Okeds.Count > 0)
				{
					query = query + " and t5.\"RootId\" IN (" + string.Join(",", filter.Okeds) + ")";
				}

				query += " order by t1.\"Id\" desc";

				var rows = AppContext.Database.SqlQuery<MAP_FormEE2Manager>(query).ToList();
				filter.ListItems = rows;
			}
			catch (Exception ex) { errorMessage = ex.Message; }

            return errorMessage;
        }
        #endregion

        #region FormEE2Record
        public string getFormEE2RecordItemBySecUserId(ref List<Map_FormEE2Record> ListItems,long SecUserId)
        {
            string errorMessage = "";
            try
            {
                string query = "select t.*  from \"Map_FormEE2Record\" t ,\"MAP_FormEE2\" t1 where t.\"FormEE2Id\"=t1.\"Id\"  and t1.\"SecUserId\"=" + SecUserId;
				var rows = AppContext.Database.SqlQuery<Map_FormEE2Record>(query).ToList();
                ListItems = rows;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public string insertMapFormEE2Record(Map_FormEE2Record item)
        {
            string errorMessage = "";
            try
			{
				AppContext.Map_FormEE2Record.Add(item);
				AppContext.SaveChanges();				
			}
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public string updateMapFormEE2Record(Map_FormEE2Record item)
        {
            string errorMessage = "";
            try
			{
				var row = AppContext.Map_FormEE2Record.Find(item.Id);
				row.ReportYear = item.ReportYear;
				row.EnergySource = item.EnergySource;
				row.ExpenceEnergy = item.ExpenceEnergy;
				int cnt = AppContext.SaveChanges();				
			}
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
        #endregion

        #region ApplicationEE2
        public string getApplicationEE2ItemList(ref IEnumerable<MAP_ApplicationEE2Info> ListItems,long SecUserId)
        {
            string errorMessage = "";
            try
            {
                string query = "select t.* , t1.\"NameRu\" as \"DicDetails_Name\" , t2.\"NameRu\" as \"DicDetailModels_Name\"  from \"MAP_ApplicationEE2\" t , \"MAP_DIC_EEDetails\" t1 , \"MAP_DIC_EEDetailModels\" t2 where t.\"DicDetailsId\"=t1.\"Id\" and t.\"DicDetailModelsId\"=t2.\"Id\" and  t.\"IsDeleted\"=false and \"SecUserId\"=" + SecUserId + " order by t.\"Id\" desc ";
                var rows = AppContext.Database.SqlQuery<MAP_ApplicationEE2Info>(query).ToList();
                ListItems = rows;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public string getApplicationEE2Item(long id, ref MAP_ApplicationEE2Info item)
        {
            string errorMessage = "";
            try
            {
                string query = "select t.* , t1.\"NameRu\" as \"DicDetails_Name\" , t2.\"NameRu\" as \"DicDetailModels_Name\"   from \"MAP_ApplicationEE2\" t , \"MAP_DIC_EEDetails\" t1 , \"MAP_DIC_EEDetailModels\" t2  where t.\"DicDetailsId\"=t1.\"Id\" and t.\"DicDetailModelsId\"=t2.\"Id\"  and  t.\"IsDeleted\"=false  and  t.\"Id\"=" + id;
                var rows = AppContext.Database.SqlQuery<MAP_ApplicationEE2Info>(query).ToList();
                if (rows.Count > 0)
                    item = rows[0];

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public string insertMapApplicationEE2(ref MAP_ApplicationEE2Info item)
        {
            string errorMessage = "";

            try
			{
				MAP_ApplicationEE2 row = new MAP_ApplicationEE2();
				row.DicDetailsId = item.DicDetailsId;
				row.DicDetailModelsId = item.DicDetailModelsId;
				row.CountOfFixtures = item.CountOfFixtures;
				row.CountOfLamps = item.CountOfLamps;
				row.Power = item.Power;
				row.CPRA = item.CPRA;
				row.AggregatePower = item.AggregatePower;
				row.AverageTariff = item.AverageTariff;
				row.WorkingHours = item.WorkingHours;
				row.MaintenanceCosts = item.MaintenanceCosts;
				row.SecUserId = item.SecUserId;
				row.Comments = item.Comments;
				row.IsDeleted = false;

				AppContext.MAP_ApplicationEE2.Add(row);
				AppContext.SaveChanges();

			}
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public string updateMapApplicationEE2(MAP_ApplicationEE2Info item)
        {
            string errorMessage = "";

			try
			{
				var row = AppContext.MAP_ApplicationEE2.Find(item.Id);
				row.EditDate = DateTime.Now;
				row.DicDetailsId = item.DicDetailsId;
				row.DicDetailModelsId = item.DicDetailModelsId;
				row.CountOfFixtures = item.CountOfFixtures;
				row.CountOfLamps = item.CountOfLamps;
				row.Power = item.Power;
				row.CPRA = item.CPRA;
				row.AggregatePower = item.AggregatePower;
				row.AverageTariff = item.AverageTariff;
				row.WorkingHours = item.WorkingHours;
				row.MaintenanceCosts = item.MaintenanceCosts;
				row.Comments = item.Comments;

				int cnt = AppContext.SaveChanges();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

            return errorMessage;
        }

        public string deleteMapApplicationEE2(long id)
        {
            string errorMessage = "";

			var row = AppContext.MAP_ApplicationEE2.Find(id);
			if (row == null)
				return "Not found this id=" + id; //"TBLCOTTAGES2016 by ID (" + this.ID + ") not found";

			AppContext.MAP_ApplicationEE2.Remove(row);
			int cnt = AppContext.SaveChanges();
			if (cnt != 1)
				errorMessage = "not deleted Id=" + id;

            return errorMessage;
        }

        #endregion


        #region справочники
        public List<SelectListItem> GetDicDetailsList()
        {
            string errorMessage = "";
            List<MAP_DIC_EEDetails> list = new List<MAP_DIC_EEDetails>();

            string query = "select * from \"MAP_DIC_EEDetails\" where \"IsDeleted\"=false  order by \"Id\"  ";     
            var rows = AppContext.Database.SqlQuery<MAP_DicObject>(query);

            var selectList = rows.Where(x => !x.IsDeleted).Select(u => new SelectListItem()
            {
                Value = u.Id.ToString(),
                Text = (lang == "kz") ? u.NameKz : u.NameRu
            }).ToList();

            return selectList;
        }

        public List<SelectListItem> GetDicEEStatusList()
        {
            string query = "select * from \"MAP_DIC_EEStatus\" where \"IsDeleted\"=false  order by \"Id\" ";
            var ListItems = AppContext.Database.SqlQuery<MAP_DicObject>(query);
            var selectList = ListItems.Where(x => !x.IsDeleted).Select(u => new SelectListItem()
            {
                Value = u.Id.ToString(),
                Text = (lang == "kz") ? u.NameKz : u.NameRu
            }).ToList();

            return selectList;
        }
        
        public List<SelectListItem> GetDicDetailModelsList()
        {
            string query = "select * from \"MAP_DIC_EEDetailModels\" where \"IsDeleted\"=false  order by \"Id\"  ";
            var ListItems = AppContext.Database.SqlQuery<MAP_DIC_EEDetailModels>(query);
            var selectList = ListItems.Where(x =>x.IsDeleted!=true).Select(u => new SelectListItem()
            {
                Value = u.Id.ToString(),
                Text = (lang == "kz") ? u.NameKz : u.NameRu
            }).ToList();

            return selectList;
        }

		public string GetDicDetailModels(ref List<MAP_DIC_EEDetailModelsInfo> ListItems)
        {
            string errorMessage = "";
            try
            {
                string query = " select t.\"Id\",t.\"Code\", t.\"DICEEDetailsId\" , "
                               + " case 'ru' when 'kz' then t.\"NameKz\" when 'ru' then t.\"NameRu\" else t.\"NameRu\" end as \"DicName\"  "
                               + " from \"MAP_DIC_EEDetailModels\"  t where t.\"IsDeleted\"=false ";

                var rows = AppContext.Database.SqlQuery<MAP_DIC_EEDetailModelsInfo>(query).ToList();
                ListItems = rows;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public MultiSelectList GetOblasList(IList<string> selectedValues)
        {
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            return new MultiSelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        public MultiSelectList GetDicEEStatusList2(IList<string> selectedValues)
        {
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            string query = "select * from \"MAP_DIC_EEStatus\" where \"IsDeleted\"=false  order by \"Id\" ";
            var listitems = AppContext.Database.SqlQuery<MAP_DicObject>(query).ToList();
            return new MultiSelectList(listitems, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        //----Форма собственности
        public List<SelectListItem> GetDicTypeApplicationList()
        {
            List<SelectListItem> ListItems = new List<SelectListItem>();
            SelectListItem item1 = new SelectListItem();
            item1.Text = "Юридические лица"; item1.Value = "1";
            ListItems.Add(item1);

            SelectListItem item2 = new SelectListItem();
            item2.Text = "Квазигосударственный сектор"; item2.Value = "2";
            ListItems.Add(item2);

            SelectListItem item3 = new SelectListItem();
            item3.Text = "Государственные учреждения"; item3.Value = "3";
            ListItems.Add(item3);

            return ListItems;
        }

		public MultiSelectList GetDicOked(IList<string> selectedValues)
		{
			var okedRepository = new DicOkedRepository();
			var okedList = okedRepository.GetAll().Where(x => x.refParent == null).ToList();
			return new MultiSelectList(okedList, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
		}
        #endregion

		#region excell export
		public string exportExcell(ref List<MAP_EE2Excel> ListItems, string name, string bin, List<string> oblastList, List<string> statusList, List<string> okedList, string Oked_Name, string Oblast_Name, string Region_Name,string orderName,string sorted)
		{

			string errorMessage = "";
			try
			{
				#region

				string bef_query = "Select  t1.\"Id\" as \"SecUserId\" , t1.\"JuridicalName\" , t1.\"BINIIN\" , t1.\"Address\" ,  t1.\"LastName\"||' '||t1.\"FirstName\" as \"User_Name\" ,  t1.\"Oblast\", t1.\"OkedId\" ,  "
					 + " case '"+lang+"' when 'ru' then t2.\"NameRu\" when 'kz' then t2.\"NameKz\" else t2.\"NameRu\" end as \"Oblast_Name\" ,  t3.\"Status_Name\" , "
					 + " case '"+lang+"' when 'ru' then t4.\"NameRu\" when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as \"Region_Name\" , t5.\"Oked_Name\"  "
					 + "  FROM \"SEC_UserKind\" t   "
					 + "  RIGHT OUTER JOIN \"SEC_User\" t1 on t.\"UserId\"=t1.\"Id\"   "
					 + "  RIGHT OUTER JOIN \"DIC_Kato\" t2 on t1.\"Oblast\"=t2.\"Id\"   "
					 + "  LEFT OUTER JOIN (select o1.\"Id\" , o1.\"RootId\" , case '"+lang+"' when 'kz' then o2.\"NameKz\" else o2.\"NameRu\" end as \"Oked_Name\"   "
					 + "    from \"DIC_OKED\" o1 , \"DIC_OKED\" o2 where o1.\"RootId\"=o2.\"Id\" ) t5 on t1.\"OkedId\"=t5.\"Id\"   "
					 + "  LEFT OUTER JOIN \"DIC_Kato\" t4 on t1.\"Region\"=t4.\"Id\"  "
					 + "  LEFT  OUTER  JOIN (select tt.\"SecUserId\" , tt.\"DicEEStatusId\" , "
					 + "   case '"+lang+"' when 'ru' then tt1.\"NameRu\" when 'kz' then tt1.\"NameKz\" else tt1.\"NameRu\" end as \"Status_Name\"          "
					 + "   from \"MAP_FormEE2\" tt ,   \"MAP_DIC_EEStatus\" tt1 where tt.\"DicEEStatusId\"=tt1.\"Id\" ) t3 on t1.\"Id\"=t3.\"SecUserId\"               "
					 + " where  t.\"KindId\"=5 and t1.\"IsDeleted\"=false   ";

				
				if (!string.IsNullOrWhiteSpace(name))
				{
					bef_query = bef_query + " and   lower(t1.\"LastName\"||t1.\"FirstName\") like lower('%" + name + "%') ";
				}

				if (!string.IsNullOrWhiteSpace(bin))
				{
					bef_query = bef_query + " and   lower(t1.\"BINIIN\") like lower('%" + bin + "%') ";
				}

				if (oblastList != null && oblastList.Count > 0)
				{
					bef_query = bef_query + " and t1.\"Oblast\" IN (" + string.Join(",", oblastList) + ")";
				}

				if (statusList != null && statusList.Count > 0)
				{
					bef_query = bef_query + " and t3.\"DicEEStatusId\" IN (" + string.Join(",", statusList) + ")";
				}

				if (okedList != null && okedList.Count > 0)
				{
					bef_query = bef_query + " and t5.\"RootId\" IN (" + string.Join(",", okedList) + ")";
				}


				if (!string.IsNullOrEmpty(Oked_Name)) {
					bef_query = bef_query + " and upper(t5.\"Oked_Name\") like '" + Oked_Name.ToUpper() + "%'";
				}

				if (!string.IsNullOrEmpty(Oblast_Name))
				{
					bef_query = bef_query + " and upper(t2.\"NameRu\") like '%" + Oblast_Name.ToUpper() + "%'";
				}

				if (!string.IsNullOrEmpty(Region_Name))
				{
					bef_query = bef_query + " and upper(t4.\"NameRu\") like '%" + Region_Name.ToUpper() + "%'";
				}

				string query = "select f1.* , f2.\"NumberOfStoreys\", f2.\"TotalArea\" ,f2.\"ReportYear\",f2.\"EnergySource\", f2.\"ExpenceEnergy\" , f2.\"Comments\" as \"FormEE2Comments\" , f3.*  "
					 + " from  (" + bef_query + " ) f1  "
					 + " LEFT OUTER JOIN ( "
					 + " select t1.* , o.* from \"MAP_FormEE2\" t1, "
					 + "  (select t2.\"FormEE2Id\" ,  "
					 + "      string_agg(t2.\"ReportYear\"::text,';'ORDER BY t2.\"Id\") as \"ReportYear\", "
					 + "      string_agg(t2.\"EnergySource\"::text,';'ORDER BY t2.\"Id\") as \"EnergySource\", "
					 + "      string_agg(t2.\"ExpenceEnergy\"::text,';'ORDER BY t2.\"Id\") as \"ExpenceEnergy\" "
					 + "   from \"Map_FormEE2Record\" t2 group by t2.\"FormEE2Id\") o   where o.\"FormEE2Id\"=t1.\"Id\") f2 on f2.\"SecUserId\"=f1.\"SecUserId\" "
					 + " LEFT OUTER JOIN ( "
					 + " select \"SecUserId\" ,  count(*) as \"CNT\" , "
					 + " string_agg(case '"+lang+"' when 'kz' then m1.\"NameKz\" else m1.\"NameRu\" end::text,';'ORDER BY m.\"Id\") as \"EEDetailsName\" , "
					 + " string_agg(case '"+lang+"' when 'kz' then m2.\"NameKz\" else m2.\"NameRu\" end::text,';'ORDER BY m.\"Id\") as \"EEDetailModelsName\" , "
					 + " string_agg(COALESCE(m.\"Id\",0)::text,';'ORDER BY m.\"Id\") as \"ApplicationEE2Id\" , "
					 + " string_agg(COALESCE(m.\"CountOfFixtures\",0)::text,';'ORDER BY m.\"Id\") as \"CountOfFixtures\" , "
					 + " string_agg(COALESCE(m.\"CountOfLamps\",0)::text,';'ORDER BY m.\"Id\") as \"CountOfLamps\" , "
					 + " string_agg(COALESCE(m.\"Power\",0)::text,';'ORDER BY m.\"Id\") as \"Power\" , "
					 + " string_agg(COALESCE(m.\"CPRA\",'null')::text,';'ORDER BY m.\"Id\") as \"CPRA\" , "
					 + " string_agg(COALESCE(m.\"AggregatePower\",0)::text,';'ORDER BY m.\"Id\") as \"AggregatePower\" , "
					 + " string_agg(COALESCE(m.\"AverageTariff\",0)::text,';'ORDER BY m.\"Id\") as \"AverageTariff\" , "
					 + " string_agg(COALESCE(m.\"WorkingHours\",'null')::text,';'ORDER BY m.\"Id\") as \"WorkingHours\" , "
					 + " string_agg(COALESCE(m.\"MaintenanceCosts\",0)::text,';'ORDER BY m.\"Id\") as \"MaintenanceCosts\",   "
					 + " string_agg(COALESCE(m.\"Comments\",'null')::text,';'ORDER BY m.\"Id\") as \"Comments\"  "
					 + " from \"MAP_ApplicationEE2\" m, \"MAP_DIC_EEDetails\" m1 , \"MAP_DIC_EEDetailModels\" m2  "
					 + " where m.\"DicDetailsId\"=m1.\"Id\" and m.\"DicDetailModelsId\"=m2.\"Id\" "
					 + " group by \"SecUserId\") f3 on f3.\"SecUserId\"=f1.\"SecUserId\"  ";

				//----order by
				string orderBy = " order by f1.\"SecUserId\"  desc ";
				if (!string.IsNullOrWhiteSpace(orderName))
				{
					orderBy = "  order by f1.\"" + orderName + "\" " + sorted;
				}

				query = query + orderBy;
				#endregion

				var rows = AppContext.Database.SqlQuery<MAP_EE2Excel>(query).ToList();
				ListItems = rows;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}
		#endregion
	}
}