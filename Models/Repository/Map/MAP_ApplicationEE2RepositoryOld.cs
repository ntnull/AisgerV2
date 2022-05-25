using Aisger.Helpers;
using Aisger.Models.Entity.Map;
using Aisger.Models.Repository.Dictionary;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Repository.Map
{
	public class MAP_ApplicationEE2RepositoryOld
	{

		private static string conName = "conName";
		aisgerEntities dbContext = new aisgerEntities();

		string conString = ConfigurationManager.AppSettings[conName];
		private static string lang = CultureHelper.GetCurrentCulture();

		#region FormEE2
		public string getFormEE2ItemList(ref IEnumerable<MAP_EE2Filters> ListItems)
		{
			string errorMessage = "";
			try
			{
				string query = "select t.*  form \"MAP_FormEE2\" t ";
				var rows = dbContext.Database.SqlQuery<MAP_EE2Filters>(query).ToList();
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
				var rows = dbContext.Database.SqlQuery<MAP_EE2Filters>(query).ToList();
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

		public string insertMapFormEE2(MAP_FormEE2 item)
		{
			string errorMessage = "";
			try
			{
				//---- Connect to a PostgreSQL database

				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();

				NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO \"MAP_FormEE2\"(\"SecUserId\",\"FSCode\",\"TotalArea\",\"NumberOfStoreys\", \"DicEEStatusId\",\"Comments\" ) "
															   + "  values(@SecUserId,@FSCode,@TotalArea,@NumberOfStoreys,@DicEEStatusId,@Comments) ", conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
				var param0 = new NpgsqlParameter("@SecUserId", NpgsqlTypes.NpgsqlDbType.Bigint);
				param0.Value = item.SecUserId; parameters.Add(param0);

				var param1 = new NpgsqlParameter("@FSCode", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.FSCode; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@TotalArea", NpgsqlTypes.NpgsqlDbType.Double);
				param2.Value = item.TotalArea ?? (object)DBNull.Value; ; parameters.Add(param2);

				var param3 = new NpgsqlParameter("@NumberOfStoreys", NpgsqlTypes.NpgsqlDbType.Integer);
				param3.Value = item.NumberOfStoreys ?? (object)DBNull.Value; ; parameters.Add(param3);

				var param4 = new NpgsqlParameter("@DicEEStatusId", NpgsqlTypes.NpgsqlDbType.Integer);
				param4.Value = item.DicEEStatusId ?? (object)DBNull.Value; ; parameters.Add(param4);

				var param5 = new NpgsqlParameter("@Comments", NpgsqlTypes.NpgsqlDbType.Varchar);
				param5.Value = item.Comments ?? (object)DBNull.Value; ; parameters.Add(param5);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				conn.Close();
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
				//---- Connect to a PostgreSQL database
				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();

				NpgsqlCommand npgSqlCommand = new NpgsqlCommand("update \"MAP_FormEE2\" set \"EditDate\"=now() ,\"FSCode\"=@FSCode , \"TotalArea\"=@TotalArea , \"NumberOfStoreys\"=@NumberOfStoreys  , \"DicEEStatusId\"=@DicEEStatusId , \"Comments\"=@Comments  where \"Id\"=" + item.Id, conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				var param1 = new NpgsqlParameter("@FSCode", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.FSCode; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@TotalArea", NpgsqlTypes.NpgsqlDbType.Double);
				param2.Value = item.TotalArea ?? (object)DBNull.Value; ; parameters.Add(param2);

				var param3 = new NpgsqlParameter("@NumberOfStoreys", NpgsqlTypes.NpgsqlDbType.Integer);
				param3.Value = item.NumberOfStoreys ?? (object)DBNull.Value; ; parameters.Add(param3);

				var param4 = new NpgsqlParameter("@DicEEStatusId", NpgsqlTypes.NpgsqlDbType.Integer);
				param4.Value = item.DicEEStatusId ?? (object)DBNull.Value; ; parameters.Add(param4);

				var param5 = new NpgsqlParameter("@Comments", NpgsqlTypes.NpgsqlDbType.Varchar);
				param5.Value = item.Comments ?? (object)DBNull.Value; ; parameters.Add(param5);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				conn.Close();
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
				//---- Connect to a PostgreSQL database
				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();

				NpgsqlCommand npgSqlCommand = new NpgsqlCommand("update \"MAP_FormEE2\" set \"EditDate\"=now() , \"DicEEStatusId\"=@DicEEStatusId  where \"Id\"=" + item.Id, conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				var param4 = new NpgsqlParameter("@DicEEStatusId", NpgsqlTypes.NpgsqlDbType.Integer);
				param4.Value = item.DicEEStatusId ?? (object)DBNull.Value; ; parameters.Add(param4);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				conn.Close();
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

				var rows = dbContext.Database.SqlQuery<MAP_FormEE2Manager>(query).ToList();
				filter.ListItems = rows;
			}
			catch (Exception ex) { errorMessage = ex.Message; }

			return errorMessage;
		}
		#endregion

		#region FormEE2Record
		public string getFormEE2RecordItemBySecUserId(ref List<Map_FormEE2Record> ListItems, long SecUserId)
		{
			string errorMessage = "";
			try
			{
				string query = "select t.*  from \"Map_FormEE2Record\" t ,\"MAP_FormEE2\" t1 where t.\"FormEE2Id\"=t1.\"Id\"  and t1.\"SecUserId\"=" + SecUserId;
				var rows = dbContext.Database.SqlQuery<Map_FormEE2Record>(query).ToList();
				ListItems = rows;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
			return errorMessage;
		}

		public string insertMapFormEE2Record(Map_FormEE2Record item, long SecUserId)
		{
			string errorMessage = "";
			try
			{
				MAP_EE2Filters form22Item = new MAP_EE2Filters();
				errorMessage = getFormEE2ItemBySecUserId(ref form22Item, SecUserId);
				if (errorMessage != "")
					return errorMessage;

				item.FormEE2Id = form22Item.Id;

				//---- Connect to a PostgreSQL database
				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();

				NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO \"Map_FormEE2Record\"(\"FormEE2Id\",\"ReportYear\",\"EnergySource\",\"ExpenceEnergy\") "
															   + "  values(@FormEE2Id,@ReportYear,@EnergySource,@ExpenceEnergy) ", conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
				var param0 = new NpgsqlParameter("@FormEE2Id", NpgsqlTypes.NpgsqlDbType.Integer);
				param0.Value = item.FormEE2Id; parameters.Add(param0);

				var param1 = new NpgsqlParameter("@ReportYear", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.ReportYear; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@EnergySource", NpgsqlTypes.NpgsqlDbType.Double);
				param2.Value = item.EnergySource ?? (object)DBNull.Value; ; parameters.Add(param2);

				var param3 = new NpgsqlParameter("@ExpenceEnergy", NpgsqlTypes.NpgsqlDbType.Double);
				param3.Value = item.ExpenceEnergy ?? (object)DBNull.Value; ; parameters.Add(param3);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				conn.Close();
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
				//---- Connect to a PostgreSQL database
				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();

				NpgsqlCommand npgSqlCommand = new NpgsqlCommand("update \"Map_FormEE2Record\" set  \"ReportYear\"=@ReportYear , \"EnergySource\"=@EnergySource , \"ExpenceEnergy\"=@ExpenceEnergy  where \"Id\"=" + item.Id, conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				var param1 = new NpgsqlParameter("@ReportYear", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.ReportYear; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@EnergySource", NpgsqlTypes.NpgsqlDbType.Double);
				param2.Value = item.EnergySource ?? (object)DBNull.Value; ; parameters.Add(param2);

				var param3 = new NpgsqlParameter("@ExpenceEnergy", NpgsqlTypes.NpgsqlDbType.Double);
				param3.Value = item.ExpenceEnergy ?? (object)DBNull.Value; ; parameters.Add(param3);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				conn.Close();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}
		#endregion

		#region ApplicationEE2
		public string getApplicationEE2ItemList(ref IEnumerable<MAP_ApplicationEE2Info> ListItems, long SecUserId)
		{
			string errorMessage = "";
			try
			{
				string query = "select t.* , t1.\"NameRu\" as \"DicDetails_Name\" , t2.\"NameRu\" as \"DicDetailModels_Name\"  from \"MAP_ApplicationEE2Info\" t , \"MAP_DIC_EEDetails\" t1 , \"MAP_DIC_EEDetailModels\" t2 where t.\"DicDetailsId\"=t1.\"Id\" and t.\"DicDetailModelsId\"=t2.\"Id\" and  t.\"IsDeleted\"=false and \"SecUserId\"=" + SecUserId + " order by t.\"Id\" desc ";
				var rows = dbContext.Database.SqlQuery<MAP_ApplicationEE2Info>(query).ToList();
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
				var rows = dbContext.Database.SqlQuery<MAP_ApplicationEE2Info>(query).ToList();
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
				//---- Connect to a PostgreSQL database
				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();
				long newId = getMapApplicationNewId();
				NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO \"MAP_ApplicationEE2\"(\"Id\",\"DicDetailsId\",\"DicDetailModelsId\",\"CountOfFixtures\",\"CountOfLamps\",\"Power\",\"CPRA\",\"AggregatePower\",\"AverageTariff\",\"WorkingHours\",\"MaintenanceCosts\",\"SecUserId\",\"Comments\") "
															   + "  values(" + newId + ",@DicDetailsId,@DicDetailModelsId,@CountOfFixtures,@CountOfLamps,@Power,@CPRA,@AggregatePower,@AverageTariff,@WorkingHours,@MaintenanceCosts,@SecUserId,@Comments) ", conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
				var param0 = new NpgsqlParameter("@DicDetailsId", NpgsqlTypes.NpgsqlDbType.Integer);
				param0.Value = item.DicDetailsId; parameters.Add(param0);

				var param1 = new NpgsqlParameter("@DicDetailModelsId", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.DicDetailModelsId; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@CountOfFixtures", NpgsqlTypes.NpgsqlDbType.Double);
				param2.Value = item.CountOfFixtures ?? (object)DBNull.Value; ; parameters.Add(param2);

				var param3 = new NpgsqlParameter("@CountOfLamps", NpgsqlTypes.NpgsqlDbType.Double);
				param3.Value = item.CountOfLamps ?? (object)DBNull.Value; ; parameters.Add(param3);

				var param4 = new NpgsqlParameter("@Power", NpgsqlTypes.NpgsqlDbType.Double);
				param4.Value = item.Power ?? (object)DBNull.Value; parameters.Add(param4);

				var param5 = new NpgsqlParameter("@CPRA", NpgsqlTypes.NpgsqlDbType.Varchar);
				param5.Value = item.CPRA ?? (object)DBNull.Value; parameters.Add(param5);

				var param6 = new NpgsqlParameter("@AggregatePower", NpgsqlTypes.NpgsqlDbType.Double);
				param6.Value = item.AggregatePower ?? (object)DBNull.Value; parameters.Add(param6);

				var param7 = new NpgsqlParameter("@AverageTariff", NpgsqlTypes.NpgsqlDbType.Double);
				param7.Value = (item.AverageTariff != null) ? item.AverageTariff : (object)DBNull.Value; ; parameters.Add(param7);

				var param8 = new NpgsqlParameter("@WorkingHours", NpgsqlTypes.NpgsqlDbType.Varchar);
				param8.Value = (item.WorkingHours != null) ? item.WorkingHours : (object)DBNull.Value; ; parameters.Add(param8);

				var param9 = new NpgsqlParameter("@MaintenanceCosts", NpgsqlTypes.NpgsqlDbType.Double);
				param9.Value = (item.MaintenanceCosts != null) ? item.MaintenanceCosts : (object)DBNull.Value; ; parameters.Add(param9);

				var param10 = new NpgsqlParameter("@SecUserId", NpgsqlTypes.NpgsqlDbType.Bigint);
				param10.Value = item.SecUserId; parameters.Add(param10);

				var param11 = new NpgsqlParameter("@Comments", NpgsqlTypes.NpgsqlDbType.Varchar);
				param11.Value = (item.Comments != null) ? item.Comments : (object)DBNull.Value; ; parameters.Add(param11);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}

				item.Id = newId;

				conn.Close();
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
				string query = " update \"MAP_ApplicationEE2\"  set \"DicDetailsId\"=@DicDetailsId , \"DicDetailModelsId\"=@DicDetailModelsId ,  \"CountOfFixtures\"=@CountOfFixtures , \"CountOfLamps\"=@CountOfLamps"
							  + " , \"Power\"=@Power ,  \"CPRA\"=@CPRA ,  \"AggregatePower\"=@AggregatePower ,  \"AverageTariff\"=@AverageTariff ,  \"WorkingHours\"=@WorkingHours"
							  + " ,\"MaintenanceCosts\"=@MaintenanceCosts , \"Comments\"=@Comments  where \"Id\"=" + item.Id;

				//---- Connect to a PostgreSQL database
				string conString = ConfigurationManager.AppSettings[conName];
				NpgsqlConnection conn = new NpgsqlConnection(conString);

				conn.Open();
				NpgsqlCommand npgSqlCommand = new NpgsqlCommand(query, conn);

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
				var param0 = new NpgsqlParameter("@DicDetailsId", NpgsqlTypes.NpgsqlDbType.Integer);
				param0.Value = item.DicDetailsId; parameters.Add(param0);

				var param1 = new NpgsqlParameter("@DicDetailModelsId", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.DicDetailModelsId; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@CountOfFixtures", NpgsqlTypes.NpgsqlDbType.Double);
				param2.Value = item.CountOfFixtures ?? (object)DBNull.Value; parameters.Add(param2);

				var param3 = new NpgsqlParameter("@CountOfLamps", NpgsqlTypes.NpgsqlDbType.Double);
				param3.Value = item.CountOfLamps ?? (object)DBNull.Value; parameters.Add(param3);

				var param4 = new NpgsqlParameter("@Power", NpgsqlTypes.NpgsqlDbType.Double);
				param4.Value = item.Power ?? (object)DBNull.Value; parameters.Add(param4);

				var param5 = new NpgsqlParameter("@CPRA", NpgsqlTypes.NpgsqlDbType.Varchar);
				param5.Value = item.CPRA ?? (object)DBNull.Value; parameters.Add(param5);

				var param6 = new NpgsqlParameter("@AggregatePower", NpgsqlTypes.NpgsqlDbType.Double);
				param6.Value = item.AggregatePower ?? (object)DBNull.Value; parameters.Add(param6);

				var param7 = new NpgsqlParameter("@AverageTariff", NpgsqlTypes.NpgsqlDbType.Double);
				param7.Value = (item.AverageTariff != null) ? item.AverageTariff : (object)DBNull.Value; ; parameters.Add(param7);

				var param8 = new NpgsqlParameter("@WorkingHours", NpgsqlTypes.NpgsqlDbType.Varchar);
				param8.Value = (item.WorkingHours != null) ? item.WorkingHours : (object)DBNull.Value; ; parameters.Add(param8);

				var param9 = new NpgsqlParameter("@MaintenanceCosts", NpgsqlTypes.NpgsqlDbType.Double);
				param9.Value = (item.MaintenanceCosts != null) ? item.MaintenanceCosts : (object)DBNull.Value; ; parameters.Add(param9);

				var param10 = new NpgsqlParameter("@Comments", NpgsqlTypes.NpgsqlDbType.Varchar);
				param10.Value = (item.Comments != null) ? item.Comments : (object)DBNull.Value; ; parameters.Add(param10);

				for (int i = 0; i < parameters.Count; i++)
				{
					npgSqlCommand.Parameters.Add(parameters[i]);
				}

				int count = npgSqlCommand.ExecuteNonQuery();
				if (count != 1)
				{
					errorMessage = "Error";
				}
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
			string query = "delete from \"MAP_ApplicationEE2\" where \"Id\"=" + id;
			int isResult = dbContext.Database.ExecuteSqlCommand(query);
			if (isResult != 1)
				errorMessage = "not deleted Id=" + id;

			return errorMessage;
		}

		//---- helper 
		public long getMapApplicationNewId()
		{
			string query = " select  nextval('public.MAP_ApplicationEE2_Id_seq') ";
			var maxId = dbContext.Database.SqlQuery<long>(query).SingleOrDefault();
			return maxId;
		}

		#endregion
        
		#region справочники
		public List<SelectListItem> GetDicDetailsList()
		{
			string errorMessage = "";
			List<MAP_DIC_EEDetails> list = new List<MAP_DIC_EEDetails>();

			string query = "select * from \"MAP_DIC_EEDetails\" where \"IsDeleted\"=false  order by \"Id\"  ";
			var rows = dbContext.Database.SqlQuery<MAP_DicObject>(query);

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
			var ListItems = dbContext.Database.SqlQuery<MAP_DicObject>(query);
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
			var ListItems = dbContext.Database.SqlQuery<MAP_DIC_EEDetailModels>(query);
			var selectList = ListItems.Where(x => x.IsDeleted != true).Select(u => new SelectListItem()
			{
				Value = u.Id.ToString(),
				Text = (lang == "kz") ? u.NameKz : u.NameRu
			}).ToList();

			return selectList;
		}

		public string GetDicDetailModels(ref List<MAP_DIC_EEDetailModels> ListItems)
		{
			string errorMessage = "";
			try
			{
				string query = " select t.\"Id\",t.\"Code\", t.\"DICEEDetailsId\" , "
							   + " case 'ru' when 'kz' then t.\"NameKz\" when 'ru' then t.\"NameRu\" else t.\"NameRu\" end as \"DicName\"  "
							   + " from \"MAP_DIC_EEDetailModels\"  t where t.\"IsDeleted\"=false ";

				var rows = dbContext.Database.SqlQuery<MAP_DIC_EEDetailModels>(query).ToList();
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
			var listitems = dbContext.Database.SqlQuery<MAP_DicObject>(query).ToList();
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

		#region not used
		public Dictionary<string, object> getTableList(string query)
		{
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

		public string getObjectData(string query, out NpgsqlDataReader dr)
		{
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
				dr = command.ExecuteReader();
				conn.Close();
			}
			catch (Exception ex)
			{
				dr = null;
				errorMessage = ex.Message;
			}

			return errorMessage;
		}

		public string insertMapApplicationAtSchool11(MAP_ApplicationEE2Info item)
		{
			string errorMessage = "";

			try
			{

				//---- Connect to a PostgreSQL database
				string query = "INSERT INTO \"MAP_ApplicationEE2\"(\"DicDetailsId\",\"DicDetailModelsId\") "
															   + "  values(@\"DicDetailsId\",@\"DicDetailModelsId\") ";

				//System.Data.SqlClient.SqlParameter param1 = new System.Data.SqlClient.SqlParameter("@DicDetailsId", item.DicDetailsId);
				//System.Data.SqlClient.SqlParameter param2 = new System.Data.SqlClient.SqlParameter("@DicDetailModelsId", item.DicDetailsId);
				//List<System.Data.SqlClient.SqlParameter> paramList = new List<System.Data.SqlClient.SqlParameter>();
				//paramList.Add(param1);
				//paramList.Add(param2);
				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
				var param1 = new NpgsqlParameter("@DicDetailsId", NpgsqlTypes.NpgsqlDbType.Integer);
				param1.Value = item.DicDetailsId; parameters.Add(param1);

				var param2 = new NpgsqlParameter("@DicDetailModelsId", NpgsqlTypes.NpgsqlDbType.Integer);
				param2.Value = item.DicDetailsId; parameters.Add(param2);


				List<object> parameterList = new List<object>();
				parameterList.Add(item.DicDetailsId);
				parameterList.Add(item.DicDetailModelsId);

				object[] parameters1 = parameters.ToArray();
				int numberOfRowInserted = dbContext.Database.ExecuteSqlCommand(query, parameters1);
				if (numberOfRowInserted != 1)
				{
					errorMessage = "Error";
				}


			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}
		#endregion

		#region excell export
		public string exportExcell(ref List<MAP_EE2Excel> ListItems, string name, string bin, List<string> oblastList, List<string> statusList, List<string> okedList, string Oked_Name, string Oblast_Name, string Region_Name, string orderName, string sorted)
		{

			string errorMessage = "";
			try
			{
				#region

				string bef_query = "Select  t1.\"Id\" as \"SecUserId\" , t1.\"JuridicalName\" , t1.\"BINIIN\" , t1.\"Address\" ,  t1.\"LastName\"||' '||t1.\"FirstName\" as \"User_Name\" ,  t1.\"Oblast\", t1.\"OkedId\" ,  "
					 + " case '" + lang + "' when 'ru' then t2.\"NameRu\" when 'kz' then t2.\"NameKz\" else t2.\"NameRu\" end as \"Oblast_Name\" ,  t3.\"Status_Name\" , "
					 + " case '" + lang + "' when 'ru' then t4.\"NameRu\" when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as \"Region_Name\" , t5.\"Oked_Name\"  "
					 + "  FROM \"SEC_UserKind\" t   "
					 + "  RIGHT OUTER JOIN \"SEC_User\" t1 on t.\"UserId\"=t1.\"Id\"   "
					 + "  RIGHT OUTER JOIN \"DIC_Kato\" t2 on t1.\"Oblast\"=t2.\"Id\"   "
					 + "  LEFT OUTER JOIN (select o1.\"Id\" , o1.\"RootId\" , case '" + lang + "' when 'kz' then o2.\"NameKz\" else o2.\"NameRu\" end as \"Oked_Name\"   "
					 + "    from \"DIC_OKED\" o1 , \"DIC_OKED\" o2 where o1.\"RootId\"=o2.\"Id\" ) t5 on t1.\"OkedId\"=t5.\"Id\"   "
					 + "  LEFT OUTER JOIN \"DIC_Kato\" t4 on t1.\"Region\"=t4.\"Id\"  "
					 + "  LEFT  OUTER  JOIN (select tt.\"SecUserId\" , tt.\"DicEEStatusId\" , "
					 + "   case '" + lang + "' when 'ru' then tt1.\"NameRu\" when 'kz' then tt1.\"NameKz\" else tt1.\"NameRu\" end as \"Status_Name\"          "
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


				if (!string.IsNullOrEmpty(Oked_Name))
				{
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
					 + " string_agg(case '" + lang + "' when 'kz' then m1.\"NameKz\" else m1.\"NameRu\" end::text,';'ORDER BY m.\"Id\") as \"EEDetailsName\" , "
					 + " string_agg(case '" + lang + "' when 'kz' then m2.\"NameKz\" else m2.\"NameRu\" end::text,';'ORDER BY m.\"Id\") as \"EEDetailModelsName\" , "
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

				var rows = dbContext.Database.SqlQuery<MAP_EE2Excel>(query).ToList();
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