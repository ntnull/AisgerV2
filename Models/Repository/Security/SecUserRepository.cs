using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Utils;
using Microsoft.Data.OData.Query.SemanticAst;
using Npgsql;
using NpgsqlTypes;
using NPOI.SS.Formula.Functions;
using Aisger.Models.Entity.Subject;
using Aisger.Models.Repository.Dictionary;

namespace Aisger.Models.Repository.Security
{
    public class SecUserRepository : AObjectRepository<SEC_User>
    {     
        public virtual List<SEC_User> GetEmployeesByOrgId(long? orgId)
        {
            var user = AppContext.SEC_User.FirstOrDefault(e => e.Id == orgId);
            if (user == null)
            {
                return new List<SEC_User>();
            }

            return AppContext.Set<SEC_User>().Where(e => !e.IsDeleted && !e.IsGuest && e.OrganizationId == user.OrganizationId).OrderBy(e => e.LastName).ToList();
        }
		public IEnumerable<SEC_User> GetListByKindId(long kindId)
		{
			string query =
                "select r.\"Id\",  \"Login\",  \"BIN_IDK\" ,  \"BIN_DEL\" ,  \"BIN_Id\" ,  \"Pwd\",   \"FirstName\",   \"LastName\",   \"SecondName\",   \"JuridicalName\",   \"Post\",   \"BINIIN\",   \"Email\",   \"Mobile\",   \"WorkPhone\",   \"InternalPhone\",   \"IsGuest\",     \"OrganizationId\",   \"DeparmentId\",   \"RolesId\",   \"CreateDate\",   \"EditDate\",   \"IsDeleted\",   \"Address\",   \"IsCvazy\",   \"ResponceFIO\",   \"ResponcePost\",   \"Oblast\",   \"Region\",   \"SubRegion\",   \"Village\",   \"TypeApplicationId\",   \"IsHaveGES\",   \"IDK\",   \"OkedId\",   \"Lat\",   \"Lng\",   \"Certificate\",   \"BankRequisites\",   \"FactAddress\",   \"urlSite\",   \"FactOblast\",   \"FactRegion\",   \"FactSubRegion\",   \"FactVillage\",   \"FSCode\",   \"Note\",   is_obl_control ,ger_wo_ecp ," +
				"(case when s.\"IsBlocked\" is null then FALSE else s.\"IsBlocked\" end) as \"IsDisabled\" " +
				" from public.\"SEC_User\" as r " +
				"inner join public.\"SEC_UserKind\" as s on s.\"UserId\"=r.\"Id\" and s.\"KindId\"=" + kindId +
		   " where r.\"IsDeleted\"=FALSE  order by s.\"DateEdit\" desc";

			return AppContext.Database.SqlQuery<SEC_User>(query);
		}

        public IEnumerable<SEC_UserKind> CheckKindUser(long userId,long kindId)
        {
            string query ="select * from \"SEC_UserKind\"  where \"UserId\"="+userId+"  and \"KindId\"="+kindId;
            return AppContext.Database.SqlQuery<SEC_UserKind>(query);
        }

        protected override void BeforeSave(SEC_User attachedEntity, SEC_User oldEntity)
        {
            if (attachedEntity != null)
            {
                if (String.IsNullOrEmpty(oldEntity.ConfirmPwd))
                {
                    oldEntity.ConfirmPwd = "test";
                }
                attachedEntity.ConfirmPwd = oldEntity.ConfirmPwd;
            }
        }
        public override List<SEC_User> GetAll()
        {
            var res = GetQueryByDescending(e => !e.IsDeleted, true, e => e.Id);
            return res.ToList();
        }
        protected override void PrepareDelete(SEC_User obj)
        {
            obj.ConfirmPwd = "test";
        }

        //        public void SaveUpdate(SEC_USER employee)
        //        {
        //            if (employee.ID == 0)
        //            {
        //                employee.DATE_CREATE = DateTime.Now;
        //                Context.SEC_USER.Add(employee);
        //            }
        //            else
        //            {
        //                SEC_USER prodToUpdate = GetById(employee.ID);
        //                employee.DATE_LAST_CHANGE = DateTime.Now;
        //                if (prodToUpdate != null)
        //                {
        //                    Context.Entry(prodToUpdate).CurrentValues.SetValues(employee);
        //                    prodToUpdate.ConfirmPwd = prodToUpdate.PWD;
        //                }
        //            }
        //            Context.SaveChanges();
        //        }

        public bool CheckAccount(string userName, string password)
        {
            var pwd = Encrypt(password);

            var user =
                AppContext.SEC_User.FirstOrDefault(o => o.Pwd == pwd && o.Login.ToUpper() == userName.ToUpper() && !o.IsDeleted);
            if (user == null)
            {
                return false;
            }
            FillSessionUserData(user, false);
            return true;
        }

        public bool CheckAccountByName(string userName, bool byCookie)
        {
            var user = AppContext.SEC_User.FirstOrDefault(o => o.Login.ToUpper() == userName.ToUpper() && !o.IsDeleted);
            if (user == null)
            {
                return false;
            }
            FillSessionUserData(user, byCookie);
            return true;
        }

        private void FillSessionUserData(SEC_User user, bool byCookie)
        {
            HttpContext.Current.Session.Add(CodeConstManager.SESSION_USER, user);

            var token = AppContext.SEC_UserToken.FirstOrDefault(e => e.UserId == user.Id);
            if (token != null)
            {
                token.Token = Guid.NewGuid().ToString();
                token.EditDate = DateTime.Now;
            }
            else
            {
                token = new SEC_UserToken
                {
                    UserId = user.Id,
                    Token = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    EditDate = DateTime.Now
                };
                AppContext.SEC_UserToken.Add(token);
            }

            AppContext.SaveChanges();
            DIC_Organization organisation = null;

            //            HttpContext.Current.Session.Add(CodeConstManager.SESSION_CURRENT_REESTR, 0);
            if (user.DIC_Organization != null)
            {
                HttpContext.Current.Session.Add(CodeConstManager.SESSION_USER_ORG_ID, user.DIC_Organization.Id);
                organisation = AppContext.DIC_Organization.SingleOrDefault(e => e.Id == user.DIC_Organization.Id);
            }
            var rigths = new Dictionary<string, bool>();
            var rootList = new List<string>();
            foreach (var rolespermissions in user.SEC_Roles.SEC_RolePermission)
            {
                if (!rigths.ContainsKey(rolespermissions.SEC_RightPermission.Code))
                {
                    if (rolespermissions.SEC_RightPermission.SEC_RightPermission2 != null && !rootList.Contains(rolespermissions.SEC_RightPermission.SEC_RightPermission2.Code))
                    {
                        rootList.Add(rolespermissions.SEC_RightPermission.SEC_RightPermission2.Code);
                    }
                    rigths.Add(rolespermissions.SEC_RightPermission.Code, rolespermissions.IS_EDIT);
                }
            }
            foreach (var key in rootList)
            {
                if (!rigths.ContainsKey(key))
                {
                    rigths.Add(key, false);
                }
            }
            if (!rigths.ContainsKey(CodeConstManager.EscoPage))
            {
                rigths.Add(CodeConstManager.EscoPage, false);
            }
            if (!rigths.ContainsKey(CodeConstManager.EscoSearch))
            {
                rigths.Add(CodeConstManager.EscoSearch, false);
            }
            if (user.SEC_UserKind.Count == 1)
            {
                if (!rigths.ContainsKey(CodeConstManager.MapApplication))
                {
                    rigths.Add(CodeConstManager.MapApplication, false);
                }
            }
           /* if (user.SEC_UserKind.Any(e => e.DIC_KindUser.Code == CodeConstManager.CODE_USER_APP))
            {
                if (!rigths.ContainsKey(CodeConstManager.MapApplication))
                {
                    rigths.Add(CodeConstManager.MapApplication, false);
                }
               /* #1#
            }*/
            if (user.SEC_UserKind.Any(e => e.DIC_KindUser.Code == CodeConstManager.CODE_USER_SUBJECT))
            {
                if (!rigths.ContainsKey(CodeConstManager.RegisterForm)) 
                {
                    rigths.Add(CodeConstManager.RegisterForm, false);
                }
                if (!rigths.ContainsKey(CodeConstManager.SubActionPlan))
                {
                    rigths.Add(CodeConstManager.SubActionPlan, false);
                }
                if (!rigths.ContainsKey(CodeConstManager.SubjectPage))
                {
                    rigths.Add(CodeConstManager.SubjectPage, false);
                }
            }
            if (user.SEC_UserKind.Any(e => e.DIC_KindUser.Code == CodeConstManager.CODE_USER_AUDIT))
            {
                if (!rigths.ContainsKey(CodeConstManager.EnergyAudit))
                {
                    rigths.Add(CodeConstManager.EnergyAudit, false);
                }
                if (!rigths.ContainsKey(CodeConstManager.AuditPage))
                {
                    rigths.Add(CodeConstManager.AuditPage, false);
                }
            }
            if (user.SEC_UserKind.Any(e => e.DIC_KindUser.Code == CodeConstManager.CODE_USER_ESCO))
            {
                if (!rigths.ContainsKey(CodeConstManager.EscoDicProductKind))
                {
                    rigths.Add(CodeConstManager.EscoDicProductKind, false);
                }
            }

            if (user.SEC_UserKind.Any(e => e.DIC_KindUser.Code == CodeConstManager.CODE_USER_MAPEE2))
            {
                if (!rigths.ContainsKey("MapEnergy"))
                {
                    rigths.Add("MapEnergy", false);
                }

                if (!rigths.ContainsKey(CodeConstManager.MapApplicationEE2))
                {
                    rigths.Add(CodeConstManager.MapApplicationEE2, false);
                }

				if (rigths.ContainsKey(CodeConstManager.MapApplicationEE2))
				{

					if (!rigths.ContainsKey(CodeConstManager.MapApplicationEE2Add))
						rigths.Add(CodeConstManager.MapApplicationEE2Add, false);

					if (!rigths.ContainsKey(CodeConstManager.MapApplicationEE2Edit))
						rigths.Add(CodeConstManager.MapApplicationEE2Edit, false);

					if (!rigths.ContainsKey(CodeConstManager.MapApplicationEE2Operation))
						rigths.Add(CodeConstManager.MapApplicationEE2Operation, false);

					if (!rigths.ContainsKey(CodeConstManager.MapApplicationEE2Send))
						rigths.Add(CodeConstManager.MapApplicationEE2Send, false);

					if (!rigths.ContainsKey(CodeConstManager.MapApplicationEE2Save))
						rigths.Add(CodeConstManager.MapApplicationEE2Save, false);

				}
            }
          
            HttpContext.Current.Session.Add(CodeConstManager.SESSION_USER_ROLES, rigths);
           
        }

        //        private void FindParent(SEC_RIGHTPERMISSIONS role, Dictionary<string, bool> rights)
        //        {
        //            if (role.PARENT_ID != null)
        //            {
        //                rights.Add(role.SEC_RIGHTPERMISSIONS2.CODE);
        //                FindParent(role.SEC_RIGHTPERMISSIONS2, rights);
        //            }
        //        }

        public string Encrypt(string input)
        {
            const string salt = "My_$@lt_v@1Ue";
            const int iteration = 3000;
            const int bytes = 32;
            var rfc2898 = new Rfc2898DeriveBytes(input, Encoding.UTF8.GetBytes(salt), iteration);
            string myEncryptedKey = Convert.ToBase64String(rfc2898.GetBytes(bytes));
            return myEncryptedKey;
        }

        public void RegisteredUser(SEC_Guest model, long? author)
        {
            var secUser = AppContext.SEC_User.FirstOrDefault(e=>e.Id==model.Id);
            if (secUser == null)
            {
                secUser = new SEC_User();
                secUser.CreateDate = DateTime.Now;
                secUser.Pwd = model.Pwd;
                secUser.ConfirmPwd = model.ConfirmPwd;
                if (string.IsNullOrEmpty(secUser.Pwd))
                {
                    secUser.Pwd = CodeConstManager.DEFAULT_PWD;
                    secUser.ConfirmPwd = CodeConstManager.DEFAULT_PWD;
                }
                secUser.Pwd = RegJurnalManager.Instance.Encrypt(secUser.Pwd);
                var role = new SecRolesRepository().GetByCode("Guest");
                if (role != null)
                {                    
                    secUser.RolesId = role.Id;
                }
                secUser.SEC_UserOked = new Collection<SEC_UserOked>();
                if (model.Wastes == null)
                {
                    model.Wastes = new List<string>();
                }
                foreach (var country in model.Wastes)
                {
                    var way = new SEC_UserOked()
                    {
                        SEC_User = secUser,
                        OkedId = Convert.ToInt64(country)
                    };
                    AppContext.SEC_UserOked.Add(way);
                }
                secUser.SEC_UserKind = new Collection<SEC_UserKind>();
                foreach (var country in model.Kinds)
                {
                    var way = new SEC_UserKind()
                    {
                        SEC_User = secUser,
                        DateEdit = DateTime.Now,
                        IsBlocked = model.IsBlokced,
                        KindId = Convert.ToInt64(country)
                    };
                    AppContext.SEC_UserKind.Add(way);
                }
            }
            else
            {
                var kinds = AppContext.Set<SEC_UserKind>().Where(e => e.UserId == secUser.Id);
                var oblastId = new List<long>();
                if (model.Kinds == null)
                {
                    model.Kinds = new List<string>();
                }
                foreach (var country in model.Kinds)
                {
                    var idolbast = Convert.ToInt64(country);
                    var kind = kinds.FirstOrDefault(e => e.KindId == idolbast);
                    if (kind == null)
                    {
                        var way = new SEC_UserKind
                        {
                            UserId = model.Id,
                            KindId = idolbast,
                            DateEdit = DateTime.Now,
                            IsBlocked = model.IsBlokced,
                            ReasonBlocked = model.ReasonBlocked
                        };
                        AppContext.Set<SEC_UserKind>().Add(way);
                    }
                    else
                    {
                        kind.IsBlocked = model.IsBlokced;
                        if (kind.IsBlocked!=null && kind.IsBlocked.Value)
                        {
                            kind.ReasonBlocked = model.ReasonBlocked;
                        }
                        oblastId.Add(idolbast);
                    }
                }
              /*  var listKinddelete = kinds.Where(e => !oblastId.Contains(e.KindId.Value));
                foreach (var crRoutesAquticOblast in listKinddelete)
                {
                    AppContext.Set<SEC_UserKind>().Remove(crRoutesAquticOblast);
                }*/

                var aquticOblasts = AppContext.Set<SEC_UserOked>().Where(e => e.UserId == secUser.Id);
                if (model.Wastes == null)
                {
                   model.Wastes = new List<string>(); 
                }
                foreach (var country in model.Wastes)
                {
                    var idolbast = Convert.ToInt64(country);
                    if (aquticOblasts.SingleOrDefault(e => e.OkedId == idolbast) == null)
                    {
                        var way = new SEC_UserOked
                        {
                            UserId = model.Id,
                            OkedId = idolbast
                        };
                        AppContext.Set<SEC_UserOked>().Add(way);
                    }
                    else
                    {
                        oblastId.Add(idolbast);
                    }
                }
                var listdelete = aquticOblasts.Where(e => !oblastId.Contains(e.OkedId.Value));
                foreach (var crRoutesAquticOblast in listdelete)
                {
                    AppContext.Set<SEC_UserOked>().Remove(crRoutesAquticOblast);
                }
                secUser.ConfirmPwd = CodeConstManager.DEFAULT_PWD;
            }
            secUser.Login = model.BINIIN;
            secUser.BINIIN = model.BINIIN;
            secUser.JuridicalName = model.JuridicalName;
            secUser.Address = model.Address;
            if (!string.IsNullOrEmpty(model.FactAddress))
            {
                secUser.FactAddress = model.FactAddress;
            }
            if (!string.IsNullOrEmpty(model.Certificate))
            {
                secUser.Certificate = model.Certificate;
            }

            secUser.Email = model.Email;
            secUser.FirstName = model.FirstName;
            secUser.LastName = model.LastName;
            secUser.SecondName = model.SecondName;
            if (!string.IsNullOrEmpty(model.urlSite) && (!model.urlSite.Contains("http") || !model.urlSite.Contains("https")))
            {
                model.urlSite = @"http:\\" + model.urlSite;
            }

            secUser.urlSite = model.urlSite;

            secUser.WorkPhone = model.WorkPhone;
            secUser.Mobile = model.Mobile;
            secUser.InternalPhone = model.InternalPhone;
            secUser.ResponceFIO = model.ResponceFIO;
            secUser.ResponcePost = model.ResponcePost;
            secUser.IsConfirm = true;
            secUser.IsCvazy = model.IsCvazy;
            secUser.IsHaveGES = model.IsHaveGES;
            secUser.IsGuest = true;
            secUser.TypeApplicationId = model.TypeApplicationId;
            //2 6 11 12 15 26 27
         
            var _dicTypeApplication = AppContext.DIC_TypeApplication.FirstOrDefault(x => x.Id == model.TypeApplicationId);
            if (_dicTypeApplication.Code.Equals("юр"))
                secUser.FSCode = 1;
            else if (_dicTypeApplication.Code.Equals("кв"))
                secUser.FSCode = 2;
            else if (_dicTypeApplication.Code.Equals("гу"))
                secUser.FSCode = 3;
            else secUser.FSCode = null;

            if (model.IsCvazy == true)
                secUser.FSCode = 2;

            secUser.Oblast = model.Oblast;
            secUser.Region = (model.Region == 0) ? (long?)null : model.Region;
            secUser.Post = model.Post;
            secUser.SubRegion = model.SubRegion;
            secUser.Village = model.Village;
            secUser.OkedId = model.OkedId;
            secUser.FactOblast = model.FactOblast;
            secUser.FactRegion = model.FactRegion;
            secUser.FactSubRegion = model.FactSubRegion;
            secUser.FactVillage = model.FactVillage;
            if (secUser.Id == 0)
            {
                AppContext.SEC_User.Add(secUser);
            }
            try
            {
                AppContext.SaveChanges();
                //----
                HttpContext.Current.Session.Add(CodeConstManager.SESSION_USER, secUser);
                SaveHistoryUser(secUser, MyExtensions.GetCurrentUserId());
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        //-----
        public void AddNewUserKind(long? userId,long? kindId)
        {
            try
            {
				bool flag=false;
				int checkCount = AppContext.SEC_UserKind.Where(x => x.UserId == userId && x.KindId==5).Count();
				if (checkCount == 0)
				{

					var way = new SEC_UserKind
					{
						UserId = userId,
						KindId = kindId
					};

					AppContext.Set<SEC_UserKind>().Add(way);
					AppContext.SaveChanges();
				}
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ChangePwd(long id, string pwd)
        {
            var user = GetById(id);
            user.Pwd = pwd;
            user.ConfirmPwd = pwd;
            AppContext.SaveChanges();
            SaveHistoryUser(user, user.Id);
        }

        public void UpdateEmployee(SEC_User model, long? author)
        {
            var user = AppContext.SEC_User.FirstOrDefault(e=>e.Id==model.Id);
            if (user == null)
            {
                return;
            }
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.InternalPhone = model.InternalPhone;
            user.LastName = model.LastName;
            user.Mobile = model.Mobile;
            user.Post = model.Post;
            user.SecondName = model.SecondName;
            user.WorkPhone = model.WorkPhone;
            user.ConfirmPwd = CodeConstManager.DEFAULT_PWD;
            AppContext.SaveChanges();
            SaveHistoryUser(user, author);
        }

        public void UpdateMark(long userId, string lat, string lng, long? author)
        {
            var user = AppContext.SEC_User.FirstOrDefault(e => e.Id == userId);
            if (user == null)
            {
                return;
            }
            user.ConfirmPwd = CodeConstManager.DEFAULT_PWD;
            user.Lat = lat;
            user.Lng = lng;
            AppContext.SaveChanges();
            SaveHistoryUser(user, author);
        }
        public List<TermSearch> GetSecUserByUserKind(string term, int pageSize, int pageNum, ref int generalCount, string kindUserCode = null)
        {
            var query = AppContext.SEC_User.AsQueryable();
            if (kindUserCode != null)
            {
                query = query.Where(su => su.SEC_UserKind.Any(uk => uk.DIC_KindUser.Code == kindUserCode));
            }
            query = query.Where(e => (e.BINIIN.ToLower().Contains(term.ToLower()) || e.JuridicalName.ToLower().Contains(term.ToLower())) && e.IsGuest);
            generalCount = query.Count();

            query = query.OrderBy(su => su.JuridicalName).Skip(pageSize*(pageNum - 1))
                .Take(pageSize);
            var termList = query.Select(x => new TermSearch() { Id = x.Id, Term = x.BINIIN + " - " + x.JuridicalName }).Distinct();
            return termList.ToList();
        }

        private Select2PagedResult TermSearchToSelect2Format(IEnumerable<TermSearch> termSearches, int totalCount)
        {
            var pagedResult = new Select2PagedResult { Results = new List<Select2Result>() };

            foreach (var a in termSearches)
            {
                pagedResult.Results.Add(new Select2Result { id = a.Id.ToString(CultureInfo.InvariantCulture), text = a.Term });
            }
            pagedResult.Total = totalCount;

            return pagedResult;
        }

        public Select2PagedResult GetPageSecUserByUserKind(string term, int pageSize, int pageNum, string kindUserCode = null)
        {
            int generalCount = 0;
            var termSearchList = GetSecUserByUserKind(term, pageSize, pageNum, ref generalCount, kindUserCode);
            return TermSearchToSelect2Format(termSearchList, generalCount);
        }

        public Select2PagedResult GetPageSecUserByRoleCode(string term, int pageSize, int pageNum, string roleCode = null)
        {
            int generalCount = 0;
            var termSearchList = GetSecUserByRoleCode(term, pageSize, pageNum, ref generalCount, roleCode);
            return TermSearchToSelect2Format(termSearchList, generalCount);
        }
        public List<TermSearch> GetSecUserByRoleCode(string term, int pageSize, int pageNum, ref int generalCount, string roleCode = null)
        {
            var query = AppContext.SEC_User.AsQueryable();
            if (roleCode != null)
            {
                query = query.Where(su => su.SEC_Roles.Code == roleCode);
            }
            query = query.Where(e => (e.FirstName.ToLower().Contains(term.ToLower()) || e.LastName.ToLower().Contains(term.ToLower())) && !e.IsDeleted);
            generalCount = query.Count();

            query = query.OrderBy(su => su.LastName).Skip(pageSize * (pageNum - 1))
                .Take(pageSize);
            var termList = query.Select(x => new TermSearch() { Id = x.Id, Term = x.LastName + " " + x.FirstName }).Distinct();
            return termList.ToList();
        }
        
        public void SaveHistoryUser(SEC_User user, long? author)
        {
            var history = new SEC_UserHistory()
            {
                Author = author,
                BINIIN = user.BINIIN,
                Address = user.Address,
                UserId = user.Id,
                BankRequisites = user.BankRequisites,
                Certificate = user.Certificate,
                CreateDate = user.CreateDate,
                DeparmentId = user.DeparmentId,
                EditDate = user.EditDate,
                Email = user.Email,
                FSCode = user.FSCode,
                FactAddress = user.FactAddress,
                FactOblast = user.FactOblast,
                FactRegion = user.FactRegion,
                FactSubRegion = user.FactSubRegion,
                FactVillage = user.FactVillage,
                FirstName = user.FirstName,
                IDK = user.IDK,
                InternalPhone = user.InternalPhone,
                IsCvazy = user.IsCvazy,
                IsDeleted = user.IsDeleted,
                IsDisabled = user.IsDisabled,
                IsGuest = user.IsGuest,
                IsHaveGES = user.IsHaveGES,
                JuridicalName = user.JuridicalName,
                LastName = user.LastName,
                Lat = user.Lat,
                Lng = user.Lng,
                Login = user.Login,
                Mobile = user.Mobile,
                Note = user.Note,
                Oblast = user.Oblast,
                OkedId = user.OkedId,
                OrganizationId = user.OrganizationId,
                Post = user.Post,
                Pwd = user.Pwd,
                Region = user.Region,
                ResponceFIO = user.ResponceFIO,
                ResponcePost = user.ResponcePost,
                RolesId = user.RolesId,
                SecondName = user.SecondName,
                SubRegion = user.SubRegion,
                TypeApplicationId = user.TypeApplicationId,
                Village = user.Village,
                WorkPhone = user.WorkPhone,
                urlSite = user.urlSite

            };
            AppContext.SEC_UserHistory.Add(history);
            AppContext.SaveChanges();
        }

        public void SaveAuditorStatus(EAUDIT_AuditorReestr auditor)
        {
            AppContext.EAUDIT_AuditorReestr.Add(auditor);
            AppContext.SaveChanges();
        }

		public string UpdateEmployeeForSourceController(SEC_User model, long? author)
		{
			string errorMessage = "";
			try
			{
				AppContext.Database.ExecuteSqlCommand("UPDATE \"SEC_User\" SET \"FSCode\"=" + model.FSCode + " WHERE \"Id\"=" + model.Id);
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
			return errorMessage;
		}

		//---
		public string GetGerNotifyManagers(ref List<SEC_User> list)
		{
			string ErrorMessage = "";
			try
			{
				string query = " select * from \"SEC_User\" where \"RolesId\" in (  "
							   + " select t.\"RolesId\" from \"SEC_RolePermission\" t, \"SEC_RightPermission\" t2  where  t.\"RightPermissionId\"=t2.\"Id\" and t.\"RolesId\"=5 and t2.\"Code\"='" + CodeConstManager.GERNOTIFY + "') ";

				var userList = AppContext.Database.SqlQuery<SEC_User>(query).ToList();
				if (userList != null)
					list = userList;

			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		//=============================================================additonal Method By S.B.
		//----
		public string SaveOrUpdate(string[] arr, long userId)
		{
			string ErrorMessage = "";
			try
			{
				#region
				var list = AppContext.rst_obl_control.Where(x => x.sec_user_id == userId).ToList();
				if (list != null && list.Count > 0)
				{
					AppContext.Database.ExecuteSqlCommand("update rst_obl_control set isdeleted=true where sec_user_id=" + userId);
					for (int i = 0; i < arr.Length; i++)
					{
						var kato_id = Convert.ToInt64(arr[i]);
						var existItem = list.Where(x => x.dic_kato_id == kato_id).FirstOrDefault();
						if (existItem == null)
						{
							var addItem = new rst_obl_control
							{
								dic_kato_id = kato_id,
								sec_user_id = userId,
								isdeleted = false,
								editdate = DateTime.Now
							};
							AppContext.rst_obl_control.Add(addItem);
						}
						else
						{
							var editItem = AppContext.rst_obl_control.Find(existItem.id);
							editItem.editdate = DateTime.Now;
							editItem.isdeleted = false;
						}

						AppContext.SaveChanges();
					}
				}
				else
				{
					for (int i = 0; i < arr.Length; i++)
					{
						var kato_id = Convert.ToInt64(arr[i]);

						var addItem = new rst_obl_control
						{
							dic_kato_id = kato_id,
							sec_user_id = userId,
							isdeleted = false,
							editdate = DateTime.Now
						};
						AppContext.rst_obl_control.Add(addItem);
						AppContext.SaveChanges();
					}
				}
				#endregion
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		//----
		public string GetManagerListForNotifyGer(ref List<SEC_User> list,long? currUserId) 
		{
			string ErrorMessage = "";
			try
			{
				var currUser=AppContext.SEC_User.Find(currUserId.Value);

				string query = "select * from \"SEC_User\" where is_obl_control=true and \"IsDeleted\"=false ";
				var rows = AppContext.Database.SqlQuery<SEC_User>(query).ToList();
				if (rows != null && rows.Count > 0)
				{
					string userIds = string.Join(",", rows.Select(x => x.Id.ToString()));
					var all = AppContext.Database.SqlQuery<rst_obl_control>("select * from rst_obl_control where sec_user_id in ("+userIds+")").ToList();
					if (all != null && all.Count > 0)
					{
						foreach (var row in rows)
						{
							var isexist = all.Where(x => x.sec_user_id == row.Id && x.dic_kato_id == currUser.Oblast).FirstOrDefault();
							if (isexist != null)
								list.Add(row);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		//-----
		public string ExecuteSqlCommand(string query)
		{
			string errorMessage = "";
			try
			{
				AppContext.Database.ExecuteSqlCommand(query);
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
			return errorMessage;
		}

		//----
		public int CheckUserByBiniin(string biniin)
		{
			int result = 1;
			var row = AppContext.Database.SqlQuery<SEC_User>("select * from \"SEC_User\" where  \"BINIIN\"='" + biniin + "' and \"IsDeleted\"=false ").FirstOrDefault();
			if (row == null)
				result = 0;

			return result;
		}

		public bool GetIsHaveGES(long userId)
		{
			int result = 1;
			var user = AppContext.Database.SqlQuery<SEC_User>("select * from \"SEC_User\" where  \"Id\"=" + userId).FirstOrDefault();
			if (user == null)
			{
				return false;
			}
			return user.IsHaveGES;
		}

        public void UpdateSECUser(SEC_Guest model)
        {
            try
            {
                var row = AppContext.SEC_User.FirstOrDefault(x => x.Id == model.Id);
                row.ConfirmPwd = row.Pwd;
                row.LastName = model.LastName;
                row.SecondName = model.SecondName;
                row.FirstName = model.FirstName;
                row.JuridicalName = model.JuridicalName;
                row.Post = model.Post;
                row.Mobile = model.Mobile;
                row.WorkPhone = model.WorkPhone;
                row.InternalPhone = model.InternalPhone;
                row.Address = model.Address;

                row.IsCvazy = model.IsCvazy;
                row.ResponceFIO = model.ResponceFIO;
                row.ResponcePost = model.ResponcePost;
                row.Oblast = model.Oblast;
                row.Region = model.Region;
                row.SubRegion = model.SubRegion;
                row.Village = model.Village;
                row.TypeApplicationId = model.TypeApplicationId;
                row.OkedId = model.OkedId;
                row.IDK = model.IDK;
                row.Email = model.Email;

                var _dicTypeApplication = AppContext.DIC_TypeApplication.FirstOrDefault(x => x.Id == model.TypeApplicationId);
                if (_dicTypeApplication.Code.Equals("юр"))
                    row.FSCode = 1;
                else if (_dicTypeApplication.Code.Equals("кв"))
                    row.FSCode = 2;
                else if (_dicTypeApplication.Code.Equals("гу"))
                    row.FSCode = 3;
                else row.FSCode = null;

                if (model.IsCvazy == true)
                    row.FSCode = 2;

                AppContext.SaveChanges();
            }
            catch (Exception ex) { }
        }

		#region remove duplicate biniin
		public string GetDuplicate(ref List<DuplicateBin> ListItems,string biniin)
		{
			string ErrorMessage = "";
			try
			{
				string query = " select t.\"BINIIN\" as biniin , t.cnt from (select \"BINIIN\" , count(*) as cnt from \"SEC_User\"  where \"Login\" is not null and \"BINIIN\" is not null and \"Login\"!='0' and \"BINIIN\"!='0' group by \"BINIIN\" ) t where t.cnt>1 ";
				
				if (!string.IsNullOrWhiteSpace(biniin)) {
					query += " and t.\"BINIIN\" like '%" + biniin + "%'";
				}

				var rows = AppContext.Database.SqlQuery<DuplicateBin>(query).ToList();
				if (rows != null && rows.Count > 0)
					ListItems = rows;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string GetDuplicateByBiniin(string biniin,ref List<DuplicateUser> ListItems)
		{
			string ErrorMessage = "";
			try
			{

				#region query
				string query = " select u.\"Id\" as user_id ,u.\"Login\" as login , u.\"CreateDate\" as createdate , u.\"JuridicalName\" as juridicalname ,  "
								+ "            u.\"IDK\" as user_idk ,  "
								+ "  u.\"FirstName\" as firstname, u.\"LastName\" as lastname, u.\"SecondName\" as secondname ,d.\"Id\" as oblast_id , d.\"NameRu\" as oblast_name  ,  "
								+ "  u.\"Address\" as address , r.*, sb.* , kind.* from \"SEC_User\" u "
								+ "  left join \"DIC_Kato\" d on d.\"Id\"=u.\"Oblast\"  "
								+ "  left join (select r1.\"UserId\" , "
								+ "              STRING_AGG(r1.\"Id\"::text, '*' order by r1.\"Id\") as rst_id ,  "
								+ "              STRING_AGG(case r1.\"BINIIN\" when '' then 'нет' when null then 'нет' else r1.\"BINIIN\" end ::text, '*' order by r1.\"Id\") as rst_biniin,  "
								+ "              STRING_AGG(case r2.\"ReportYear\" when null then 0 else r2.\"ReportYear\" end ::text, '*' order by r1.\"Id\") as rst_year, "
								+ "              STRING_AGG(case r1.\"OwnerName\" when '' then 'нет' when null then 'нет' else r1.\"OwnerName\" end ::text, '*' order by r1.\"Id\") as rst_ownername ,                "
								+ "             STRING_AGG(case r1.\"IDK\" when '' then 'нет' when null then 'нет' else r1.\"IDK\" end ::text, '*' order by r1.\"Id\") as rst_idk          "
								+ "   from \"RST_ReportReestr\" r1, \"RST_Report\"  r2 where r1.\"ReportId\"=r2.\"Id\" group by  r1.\"UserId\" ) as r on r.\"UserId\"=u.\"Id\"  "
								+ "              "
								+ "  left join (select s.\"UserId\",                                                "
								+ "              STRING_AGG(s.\"Id\"::text, '*' order by s.\"Id\") as sub_id ,    "
								+ "              STRING_AGG(case s.\"ReportYear\" when null then 0 else s.\"ReportYear\" end ::text, '*' order by s.\"Id\") as sub_year,  "
								+ "              STRING_AGG(case sd.\"NameRu\" when '' then 'нет' when null then 'нет' else sd.\"NameRu\" end ::text, '*' order by s.\"Id\") as sub_status           "
								+ "  from \"SUB_Form\" s , \"SUB_DIC_Status\" sd where s.\"StatusId\"=sd.\"Id\"  group by s.\"UserId\") as sb on sb.\"UserId\"=u.\"Id\"          "
								+ "  left join (select k.\"UserId\" ,    "
								+ "            STRING_AGG(k.\"Id\"::text, '*' order by k.\"Id\") as kind_id,  "
								+ "            STRING_AGG(case dk.\"NameRu\" when '' then 'нет' when null then 'нет' else dk.\"NameRu\" end ::text, '*' order by k.\"Id\") as kind_name               "
								+ "  from \"SEC_UserKind\" k, \"DIC_KindUser\" dk where  k.\"KindId\"=dk.\"Id\" group by k.\"UserId\" ) as kind on kind.\"UserId\"=u.\"Id\"   "
								+ "  where u.\"BINIIN\"='" + biniin + "'  order by u.\"Id\"  ";
				#endregion

				var rows = AppContext.Database.SqlQuery<DuplicateUser>(query).ToList();
				if (rows != null && rows.Count > 0)
				{
					ListItems = rows;
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string ExecuteDuplicate(long userId, string removedIds,string biniin)
		{
			string ErrorMessage = "";
			string EqualYear = "";

			try
			{
				var arr = removedIds.Split(',');
				var rst_reestrs = AppContext.RST_ReportReestr.Where(x => x.UserId == userId).ToList();
				var sub_forms = AppContext.SUB_Form.Where(x => x.UserId == userId).ToList();

				for (int i = 0; i < arr.Length; i++)
				{
					long usId = Convert.ToInt64(arr[i]);

					//----rst_reportreestr	
					var reestrs = AppContext.RST_ReportReestr.Where(x => x.UserId.Value == usId).ToList();
					foreach (var item in reestrs)
					{
						ErrorMessage += CheckRstReportReestr(userId, item, rst_reestrs, ref EqualYear);
						if (ErrorMessage != "")
							break;
					}

					if (ErrorMessage == "")
					{
						//----sub_form
						var subs = AppContext.SUB_Form.Where(x => x.UserId.Value == usId).ToList();
						foreach (var item in subs)
						{
							ErrorMessage += CheckSubForm(userId, item, sub_forms, EqualYear);
							if (ErrorMessage != "")
								break;
						}
					}

					//----
					if (ErrorMessage == "")
					{
						ErrorMessage += CheckMapApplication(userId, usId);
					}

					//----
					if (ErrorMessage == "")
					{
						ErrorMessage += CheckEAUDIT_Preamble(userId, usId);
					}

					//----
					if (ErrorMessage == "")
					{
						ErrorMessage += RemoveKindUser(userId, usId);
					}

					if (ErrorMessage != "")
						break;

				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			if (EqualYear != "")
				ErrorMessage = ResourceSetting.EqualBinInfo + " " + biniin + "00";

			return ErrorMessage;
		}

		public string CheckSubForm(long userId, SUB_Form item, List<SUB_Form> list, string EqualYear)
		{
			string ErrorMessage = "";
			try
			{
				var row = (list != null && list.Count > 0) ? list.FirstOrDefault(x => x.ReportYear == item.ReportYear) : null;
				if (row != null)
				{
					#region  //----remove all  sub_form and  records
					//string query = " DO $$ "
					//				+ "  DECLARE formid BIGINT; "
					//				+ "  BEGIN "
					//				+ "     SELECT " + item.Id + " INTO formid; "
					//				+ "     delete from \"SUB_Form2Record\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_Form3Record\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_Form4Record\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_Form5Record\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_Form6Record\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_FormKadastr\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_FormHistory\" where \"FormId\"=formid; "
					//				+ "     delete from \"SUB_Form\" where \"Id\"=formid; "
					//				+ " END $$; ";

					string query = " DO $$ "
								 + "  BEGIN "
								 + "     update \"SUB_Form\" set \"UserId\"=87294  where \"UserId\"=" + item.UserId + " and \"ReportYear\"=" + item.ReportYear + " ; "
								 + " END $$; ";

					int result = AppContext.Database.ExecuteSqlCommand(query);
					if (EqualYear == "")
						EqualYear = "equal";
					#endregion
				}
				else
				{
					//----
					string query = " DO $$ "
								+ "  DECLARE userid BIGINT; "
								+ "  BEGIN "
								+ "     SELECT " + userId + " INTO userid; "
								+ "     update \"SUB_FormHistory\"  set \"UserId\"=userid where \"UserId\"=" + item.UserId.Value + ";"
								+ "     update \"SUB_Form\"  set \"UserId\"=userid where \"UserId\"=" + item.UserId.Value + ";"
								+ " END $$; ";
					int result = AppContext.Database.ExecuteSqlCommand(query);
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string CheckRstReportReestr(long userId, RST_ReportReestr item, List<RST_ReportReestr> list, ref string EqualYear)
		{
			string ErrorMessage = "";
			try
			{
				bool flag = false;
				var row = (list != null && list.Count > 0) ? list.FirstOrDefault(x => x.ReportId == item.ReportId) : null;
				if (row != null)
				{
					#region  //----remove all  sub_form and  records
					var new_biniin = row.BINIIN + "00";
					string query = " DO $$ "
									+ "  DECLARE userid BIGINT; "
									+ "  BEGIN "
									+ "     SELECT " + item.UserId + " INTO userid; "
									+ "     delete from \"RST_ReestrReportHistory\" where \"UserId\"=userid; "
									+ "     update \"RST_ReportReestr\" set \"BINIIN\"=" + new_biniin + " ,\"UserId\"=87294  where \"UserId\"=userid; "
									+ " END $$; ";

					int result = AppContext.Database.ExecuteSqlCommand(query);
					if (EqualYear == "")
						EqualYear = "equal";

					#endregion
				}
				else
				{
					//----
					string query = " DO $$ "
								+ "  DECLARE userid BIGINT; "
								+ "  BEGIN "
								+ "     SELECT " + userId + " INTO userid; "
								+ "     update \"RST_ReestrReportHistory\"  set \"UserId\"=userid where \"UserId\"=" + item.UserId.Value + ";"
								+ "     update \"RST_ReportReestr\"  set \"UserId\"=userid where \"UserId\"=" + item.UserId.Value + ";"
								+ " END $$; ";
					int result = AppContext.Database.ExecuteSqlCommand(query);
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string CheckMapApplication(long userId,long delUserId)
		{
			string ErrorMessage = "";
			try
			{

				#region  //----remove all  sub_form and  records
				string query = " DO $$ "
								+ "  DECLARE userid BIGINT; "
								+ "  BEGIN "
								+ "     SELECT " + userId + " INTO userid; "
								+ "     update \"MAP_ApplicationEE2\"  set \"SecUserId\"=userid where \"SecUserId\"=" + delUserId + ";"
								+ "     update \"MAP_FormEE2\"  set \"SecUserId\"=userid where \"SecUserId\"=" + delUserId + ";"
								+ "     update \"MAP_ApplicationHistory\"  set \"UserId\"=userid where \"UserId\"=" + delUserId + ";"
								+ "     update \"MAP_Application\"  set \"UserId\"=userid where \"UserId\"=" + delUserId + ";"
								+ " END $$; ";

				int result = AppContext.Database.ExecuteSqlCommand(query);
				#endregion

			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string CheckEAUDIT_Preamble(long userId, long delUserId)
		{
			string ErrorMessage = "";
			try
			{

				#region  //----remove all  sub_form and  records
				string query = " DO $$ "
								+ "  DECLARE userid BIGINT; "
								+ "  BEGIN "
								+ "     SELECT " + userId + " INTO userid; "
								+ "     update \"EAUDIT_Preamble\"  set \"refEauditObject\"=userid where \"refEauditObject\"=" + delUserId + ";"
								+ " END $$; ";

				int result = AppContext.Database.ExecuteSqlCommand(query);
				#endregion

			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string RemoveKindUser(long userId, long delUserId)
		{
			string ErrorMessage = "";
			try
			{
				string query = "update \"SEC_JurEvent\" set \"UserId\"=" + userId + "  where \"UserId\"=" + delUserId + ";"
							   + " delete from \"SEC_UserHistory\" where \"UserId\"=" + delUserId + ";"
							   + " delete from \"SEC_UserKind\" where \"UserId\"=" + delUserId + ";"
								+ " delete from \"SEC_User\" where \"Id\"=" + delUserId + ";";

				int result = AppContext.Database.ExecuteSqlCommand(query);

				/*
				 *  select * from "SEC_JurEvent" where "UserId"=79200 
					select * from "SUB_FormKadastr" where "FormId" in (select "Id" from "SUB_Form" where "UserId"=79200)
					select * from "SUB_FormHistory" where "FormId" in (select "Id" from "SUB_Form" where "UserId"=79200)
					select * from "SUB_Form6Record" where "FormId" in (select "Id" from "SUB_Form" where "UserId"=79200)
				 */
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}
		#endregion

		#region chane biniin
		public string GetAllSubject(ref List<ChangeBin> ListItems, string biniin, int oblast_id)
		{
			string ErrorMessage = "";
			try
			{
				#region query
				string query = " select u.\"Id\" as user_id, u.\"BINIIN\" as biniin , u.\"IDK\"  as idk, u.\"JuridicalName\" as juridicalname, u.\"IsHaveGES\" as IsHaveGES, "
                                + " o.\"NameRu\"  as oblast_name,  r.\"NameRu\" as region_name , u.\"Address\" as address, rst.rst_biniin ,"
								+ "  rst.rst_id, rst.rst_idk, rst.rst_ownername, rst.rst_year     from \"SEC_User\" u    "
								+ "  left join \"DIC_Kato\" o on o.\"Id\"=u.\"Oblast\"    left join \"DIC_Kato\" r on r.\"Id\"=u.\"Region\"        "
								+ "  left join (select r1.\"UserId\" , STRING_AGG(r1.\"Id\"::text, '*' order by r1.\"Id\") as rst_id ,              "
								+ "   STRING_AGG(case r1.\"BINIIN\" when '' then 'нет' when null then 'нет' else r1.\"BINIIN\" end ::text, '*' order by r1.\"Id\") as rst_biniin,             "
								+ "   STRING_AGG(case r2.\"ReportYear\" when null then 0 else r2.\"ReportYear\" end ::text, '*' order by r1.\"Id\") as rst_year,               "
								+ "   STRING_AGG(case r1.\"OwnerName\" when '' then 'нет' when null then 'нет' else r1.\"OwnerName\" end ::text, '*' order by r1.\"Id\") as rst_ownername ,               "
								+ "   STRING_AGG(case r1.\"IDK\" when '' then 'нет' when null then 'нет' else r1.\"IDK\" end ::text, '*' order by r1.\"Id\") as rst_idk                "
								+ "   from \"RST_ReportReestr\" r1, \"RST_Report\"  r2 where r1.\"ReportId\"=r2.\"Id\"  group by  r1.\"UserId\" ) rst on rst.\"UserId\"=u.\"Id\"     "
								+ "   where  \"RolesId\"=4  ";
				
				//----
				if (!string.IsNullOrWhiteSpace(biniin))
				{
					query += "  and \"BINIIN\"='" + biniin + "'";
				}

				//----
				if (oblast_id != -1)
				{
					query += "  and u.\"Oblast\"=" + oblast_id;
				}

				query += "   order by u.\"Id\" OFFSET 0 LIMIT 50 ";
				#endregion

				var rows = AppContext.Database.SqlQuery<ChangeBin>(query).ToList();
				if (rows != null)
					ListItems = rows;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string SaveBin(long userId,string biniin, string idk, string juridicalname,string rst_id, bool isHaveGes)
		{
			string ErrorMessage = "";
			try
			{
				#region sql
				string query = " update \"SEC_User\" set \"BINIIN\"='" + biniin + "' , \"Login\"='" + biniin + "' , \"IDK\"='" + idk + "' , \"JuridicalName\"='" + juridicalname + "' , \"IsHaveGES\"='" + isHaveGes.ToString() + "'  where \"Id\"=" + userId + ";"
							   + " update \"RST_ReportReestr\" set \"BINIIN\"='" + biniin + "' , \"OwnerName\"='" + juridicalname + "' ,\"IDK\"='" + idk + "', usrjuridicalname='" + juridicalname + "' , usridk='" + idk + "' where \"UserId\"=" + userId + "; ";


				int result = AppContext.Database.ExecuteSqlCommand(query);
				if (!string.IsNullOrWhiteSpace(rst_id))
				{
					//var arr = rst_id.Split('*');
					//for (int i = 0; i < arr.Length; i++)
					//{
					//	var item = AppContext.RST_ReportReestr.FirstOrDefault(x => x.Id == Convert.ToInt64(arr[i]));
					//	item.BINIIN = biniin;
					//	item.IDK = idk;
					//	item.OwnerName = juridicalname;
					//	item.usrjuridicalname = juridicalname;
					//	item.usridk = idk;
					//	AppContext.SaveChanges();
					//}

					
				}
				#endregion

			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}	
		#endregion

		#region get removed subjects
		public string GetRemovedSubjects(ref List<ChangeBin> ListItems, string biniin)
		{
			string ErrorMessage = "";
			try
			{
				#region query
				string query = " select u.\"Id\" as user_id, u.\"BINIIN\" as biniin , u.\"IDK\"  as idk, u.\"JuridicalName\" as juridicalname , "
								+ " o.\"NameRu\"  as oblast_name,  r.\"NameRu\" as region_name , u.\"Address\" as address, rst.rst_biniin ,"
								+ "  rst.rst_id, rst.rst_idk, rst.rst_ownername, rst.rst_year     from \"SEC_User\" u    "
								+ "  left join \"DIC_Kato\" o on o.\"Id\"=u.\"Oblast\"    left join \"DIC_Kato\" r on r.\"Id\"=u.\"Region\"        "
								+ "  left join (select r1.\"UserId\" , STRING_AGG(r1.\"Id\"::text, '*' order by r1.\"Id\") as rst_id ,              "
								+ "   STRING_AGG(case r1.\"BINIIN\" when '' then 'нет' when null then 'нет' else r1.\"BINIIN\" end ::text, '*' order by r1.\"Id\") as rst_biniin,             "
								+ "   STRING_AGG(case r2.\"ReportYear\" when null then 0 else r2.\"ReportYear\" end ::text, '*' order by r1.\"Id\") as rst_year,               "
								+ "   STRING_AGG(case r1.\"OwnerName\" when '' then 'нет' when null then 'нет' else r1.\"OwnerName\" end ::text, '*' order by r1.\"Id\") as rst_ownername ,               "
								+ "   STRING_AGG(case r1.\"IDK\" when '' then 'нет' when null then 'нет' else r1.\"IDK\" end ::text, '*' order by r1.\"Id\") as rst_idk                "
								+ "   from \"RST_ReportReestr\" r1, \"RST_Report\"  r2 where r1.\"ReportId\"=r2.\"Id\"  group by  r1.\"UserId\" ) rst on rst.\"UserId\"=u.\"Id\"     "
								+ "   where  \"RolesId\"=4 and u.\"IsDeleted\"=true ";

				//----
				if (!string.IsNullOrWhiteSpace(biniin))
				{
					query += "  and \"BINIIN\"='" + biniin + "'";
				}			

				query += "   order by u.\"Id\" ";  // OFFSET 0 LIMIT 50
				#endregion

				var rows = AppContext.Database.SqlQuery<ChangeBin>(query).ToList();
				if (rows != null)
					ListItems = rows;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string RestoreSubject(long userId)
		{
			string ErrorMessage = "";
			try
			{

				string query = " update \"SEC_User\" set \"IsDeleted\"='" + false + "'  where \"Id\"=" + userId + ";";		  
				int result = AppContext.Database.ExecuteSqlCommand(query);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}
		#endregion
	}
}