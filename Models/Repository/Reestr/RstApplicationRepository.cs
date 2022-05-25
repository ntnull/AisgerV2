using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

namespace Aisger.Models.Repository.Reestr
{
     public class RstApplicationRepository : AObjectRepository<RST_Application>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.RST_Application; }
        }
        public virtual List<RST_Application> GetListReestr(int? year)
        {
            if (year == null)
            {
                return GetQueryByDescending(e => !e.IsDeleted, true, e => e.Id).ToList();
            }
            return GetQueryByDescending(e => !e.IsDeleted && e.ReportYear==year.Value, true, e => e.Id).ToList();
            
        }
        public bool ExistYear(int year, long? oblast, long? getCurrentUserId, long id)
         {
             if (id == 0)
             {
                 return GetAll().Exists(e => e.ReportYear == year && e.Oblast == oblast);
             }
             return GetAll().Exists(e => e.ReportYear == year && e.Oblast == oblast && e.Id != id);
         }

         protected override void BeforeSave(RST_Application attachedEntity, RST_Application oldEntity)
         {
             if (!oldEntity.IsDeleted)
             {
                 UpdateReports(attachedEntity, oldEntity);
             }
         }
         private SEC_User RegistredUser(RST_Reestr reestr)
         {
             SEC_User user;
             if (!string.IsNullOrEmpty(reestr.BINIIN))
             {
                  user = AppContext.SEC_User.FirstOrDefault(e => e.BINIIN == reestr.BINIIN);
                 if (user != null)
                 {
                     user.ConfirmPwd = user.Pwd;
                     if (!string.IsNullOrEmpty(reestr.OwnerName) && reestr.OwnerName != user.JuridicalName)
                     {
                         user.JuridicalName = reestr.OwnerName;
                     }
                     if (!string.IsNullOrEmpty(reestr.Address) && reestr.Address != user.Address)
                     {
                         user.Address = reestr.Address;
                     }
					 RegistredSecKindUser(user);
                     return user;
                 }
             }

			 user = new SEC_User
			 {
				 Address = reestr.Address,
				 JuridicalName = reestr.OwnerName,
				 BINIIN = reestr.BINIIN,
				 Pwd = RegJurnalManager.Instance.Encrypt(CodeConstManager.DEFAULT_PWD),
				 IDK = reestr.IDK
			 };

             user.ConfirmPwd = user.Pwd;
             user.CreateDate = DateTime.Now;
             user.IsGuest = true;
             user.TypeApplicationId = CodeConstManager.APP_TYPE_TOO;
             user.Login = reestr.BINIIN;

             var way = new SEC_UserKind()
             {
                 SEC_User = user,
                 KindId = 1
             };
             user.SEC_UserKind.Add(way);

             if (reestr.RST_Application != null)
             {
                 user.Oblast = reestr.RST_Application.Oblast;
             }
             var role = new SecRolesRepository().GetByCode("Guest");
             if (role != null)
             {
                 user.RolesId = role.Id;
             }
             AppContext.SEC_User.Add(user);
			 
			 //----
			 RegistredSecKindUser(user);

             return user;
         }

		 private void RegistredSecKindUser(SEC_User user)
		 {
			 var userKind = AppContext.SEC_UserKind.FirstOrDefault(e => e.UserId == user.Id && e.KindId == 1);
			 if (userKind == null)
			 {
				 var newUserKind = new SEC_UserKind
				 {
					 UserId = user.Id,
					 KindId = 1,
					 DateEdit = DateTime.Now
				 };
				 AppContext.SEC_UserKind.Add(newUserKind);
			 }

		 }

         private void RegistredSubject(RST_Reestr reestr)
         {
             var user = AppContext.SEC_User.FirstOrDefault(e => e.BINIIN == reestr.BINIIN);
             if (user != null)
             {
                 user.ConfirmPwd = user.Pwd;
                 if (!string.IsNullOrEmpty(reestr.OwnerName) && reestr.OwnerName != user.JuridicalName)
                 {
                     user.JuridicalName = reestr.OwnerName;
                 }
                 if (!string.IsNullOrEmpty(reestr.Address) && reestr.Address != user.Address)
                 {
                     user.Address = reestr.Address;
                 }
             return;
             }

             user = new SEC_User
             {
                 Address = reestr.Address,
                 JuridicalName = reestr.OwnerName,
                 BINIIN = reestr.BINIIN,
                 Pwd = RegJurnalManager.Instance.Encrypt(CodeConstManager.DEFAULT_PWD)
             };
             user.ConfirmPwd = user.Pwd;
             user.CreateDate = DateTime.Now;
             user.IsGuest = true;
             user.TypeApplicationId = CodeConstManager.APP_TYPE_TOO;
             user.Login = reestr.BINIIN;
             
             var way = new SEC_UserKind()
             {
                 SEC_User = user,
                 KindId = 1
             };
             user.SEC_UserKind.Add(way);

             if (reestr.RST_Application != null)
             {
                 user.Oblast = reestr.RST_Application.Oblast;
             }
             var role = new SecRolesRepository().GetByCode("Guest");
             if (role != null)
             {
                 user.RolesId = role.Id;
             }
             AppContext.SEC_User.Add(user);
         }

         private void UpdateProducts(RST_Application attachedEntity, RST_Application oldEntity)
        {
            if (oldEntity.Id == 0)
            {
                foreach (var reestr in oldEntity.RstReestrs)
                {
                    var history = new RST_ReestrHistory
                    {
                        StatusId = CodeConstManager.REG_STATUS_REESTR_ID,
                        UserId = oldEntity.UserId,
                        RegDate = DateTime.Now
                    };

                    var dbReestr = AppContext.RST_Reestr.FirstOrDefault(e => e.BINIIN == reestr.BINIIN);
                    if (dbReestr != null)
                    {
                        dbReestr.RST_Application = oldEntity;
                        dbReestr.StatusId = CodeConstManager.REG_STATUS_REESTR_ID;
                        history.RST_Reestr = dbReestr;
                    }
                    else
                    {
                        history.RST_Reestr = reestr;
                        reestr.RST_Application = oldEntity;
                        reestr.StatusId = CodeConstManager.REG_STATUS_REESTR_ID;
                        reestr.CreateDate = DateTime.Now;
                        AppContext.RST_Reestr.Add(reestr);
                        RegistredSubject(reestr);
                    }
                    AppContext.RST_ReestrHistory.Add(history);
                }
                return;
            }

            var bufferzones = AppContext.RST_Reestr.Where(e => e.ApplicationId == attachedEntity.Id);
            var bufferZonsIds = new List<long>();

            foreach (var entity in oldEntity.RstReestrs)
            {
                if (entity.Id == 0)
                {
                    var history = new RST_ReestrHistory
                    {
                      
                        StatusId = CodeConstManager.REG_STATUS_REESTR_ID,
                        UserId = oldEntity.UserId,
                        RegDate = DateTime.Now
                    };
                    var dbReestr = AppContext.RST_Reestr.FirstOrDefault(e => e.BINIIN == entity.BINIIN);
                    if (dbReestr != null)
                    {
                        dbReestr.ApplicationId = oldEntity.Id;
                        dbReestr.StatusId = CodeConstManager.REG_STATUS_REESTR_ID;
                        history.RST_Reestr = dbReestr;
                    }
                    else
                    {
                        history.RST_Reestr = entity;
                        entity.ApplicationId = oldEntity.Id;
                        AppContext.RST_Reestr.Add(entity);
                    }
                    RegistredSubject(entity);
                   
                    AppContext.RST_ReestrHistory.Add(history);
                }
                else
                {
                    var oldaccess = bufferzones.SingleOrDefault(e => e.Id == entity.Id);
                    if (oldaccess != null)
                    {
                        oldaccess.BINIIN = entity.BINIIN;
                        oldaccess.Address = entity.Address;
                        oldaccess.KadastNumber = entity.KadastNumber;
                        oldaccess.OwnerName = entity.OwnerName;
                        oldaccess.ObjectName = entity.ObjectName;
                    }
                    bufferZonsIds.Add(entity.Id);
                }
            }
            var bufferCoordDeleteList = bufferzones.Where(e => !bufferZonsIds.Contains(e.Id));
            foreach (var entity in bufferCoordDeleteList)
            {
                entity.IsDeleted = true;
            }
        }

         private string GenerateIDK(string code, int pos)
         {
             var idStr = pos.ToString(CultureInfo.InvariantCulture);
             code = code + "0000";
             return
                  code.Substring(0, code.Length - idStr.Length) +
                  idStr;
         }

         private void UpdateReports(RST_Application attachedEntity, RST_Application oldEntity)
         {
             var rstDicCodeOblast = AppContext.RST_DIC_CodeOblast.FirstOrDefault(r=>r.OblastId==oldEntity.Oblast);
             if (rstDicCodeOblast == null) return;
             var oblastCode = rstDicCodeOblast.OblastCode;
             var idk = oblastCode +oldEntity.ReportYear.ToString(CultureInfo.InvariantCulture).Substring(2,2);

             var maxcode = AppContext.RST_ReportReestr.Where(e => e.IDK.StartsWith(idk)).Max(e => e.IDK);
             var indexPos = 0;
             if (maxcode != null && maxcode.Length==7)
             {
                 if (!int.TryParse(maxcode.Substring(3, 4), out indexPos))
                 {
                     if (int.TryParse(maxcode.Substring(4, 4), out indexPos))
                     {
                     }
                 }
             }
             indexPos++;

             var bins = new List<string>();
             var report = AppContext.RST_Report.FirstOrDefault(e => !e.IsDeleted && e.ReportYear == oldEntity.ReportYear);
             if (report == null)
             {
                 return;
             }
			 foreach (var reestr in oldEntity.RstReestrs)
			 {
				 if (!string.IsNullOrEmpty(reestr.BINIIN) && bins.Contains(reestr.BINIIN)) continue;
				 if (!string.IsNullOrEmpty(reestr.BINIIN))
				 {
					 bins.Add(reestr.BINIIN);
				 }
				 var history = new RST_ReestrReportHistory
				 {
					 Author = oldEntity.UserId,
					 Oblast = oldEntity.Oblast,
					 ReportYear = report.ReportYear,
					 RegDate = DateTime.Now,
					 RST_Application = oldEntity
				 };


				 //----
				 var dbReestr2 = AppContext.RST_ReportReestr.OrderBy(e => e.RST_Report.ReportYear)
					.FirstOrDefault(
						e =>
							!e.IsDeleted && e.BINIIN!=null && e.BINIIN == reestr.BINIIN &&
							e.RST_Report.ReportYear == oldEntity.ReportYear);

				 if (dbReestr2 != null)
				 {
					 continue;
				 }

				 //----проверить идк 
				 if (reestr.IDK == null || reestr.IDK == "")
					 reestr.IDK = GenerateIDK(idk, indexPos);

                var authorid = MyExtensions.GetCurrentUserId();
                var authorlogin = MyExtensions.GetCurrentUserLogin();
				 var user = RegistredUser(reestr);
                var dbReestr = new RST_ReportReestr
                {
                    Address = reestr.Address,
                    BINIIN = reestr.BINIIN,
                    OwnerName = reestr.OwnerName,
                    Oblast = oldEntity.Oblast,
                    SEC_User1 = user,
                    Editor = oldEntity.UserId,
                    EditDate = DateTime.Now,
                    ReportId = report.Id,
                    StatusId = CodeConstManager.NEW_STATUS_REESTR_ID, //CodeConstManager.REG_STATUS_REESTR_ID,
                    IDK = reestr.IDK,
                    authorid = authorid,
                    authorlogin = authorlogin,
                };

				 indexPos++;
				 history.RST_ReportReestr = dbReestr;
				 history.SEC_User = user;
				 history.StatusId = CodeConstManager.NEW_STATUS_REESTR_ID; //CodeConstManager.REG_STATUS_REESTR_ID,
				 history.Note = "Первичная регистрация в реестр";
				 //                     reestr.RST_Application = oldEntity;
				 //                     reestr.StatusId = CodeConstManager.REG_STATUS_REESTR_ID;
				 //                     reestr.CreateDate = DateTime.Now;
				 AppContext.RST_ReportReestr.Add(dbReestr);
				 AppContext.RST_ReestrReportHistory.Add(history);


				 AppContext.SaveChanges();
				 //                     RegistredSubject(reestr);
			 }
         }

		 private void UpdateReportsOld(RST_Application attachedEntity, RST_Application oldEntity)
		 {
			 var rstDicCodeOblast = AppContext.RST_DIC_CodeOblast.FirstOrDefault(r => r.OblastId == oldEntity.Oblast);
			 if (rstDicCodeOblast == null) return;
			 var oblastCode = rstDicCodeOblast.OblastCode;
			 var idk = oblastCode + oldEntity.ReportYear.ToString(CultureInfo.InvariantCulture).Substring(2, 2);

			 var maxcode = AppContext.RST_ReportReestr.Where(e => e.IDK.StartsWith(idk)).Max(e => e.IDK);
			 var indexPos = 0;
			 if (maxcode != null && maxcode.Length == 7)
			 {
				 if (!int.TryParse(maxcode.Substring(3, 4), out indexPos))
				 {
					 if (int.TryParse(maxcode.Substring(4, 4), out indexPos))
					 {
					 }
				 }
			 }
			 indexPos++;

			 var bins = new List<string>();
			 var report = AppContext.RST_Report.FirstOrDefault(e => !e.IsDeleted && e.ReportYear == oldEntity.ReportYear);
			 if (report == null)
			 {
				 return;
			 }
			 foreach (var reestr in oldEntity.RstReestrs)
			 {
				 if (!string.IsNullOrEmpty(reestr.BINIIN) && bins.Contains(reestr.BINIIN)) continue;
				 if (!string.IsNullOrEmpty(reestr.BINIIN))
				 {
					 bins.Add(reestr.BINIIN);
				 }
				 var history = new RST_ReestrReportHistory
				 {
					 Author = oldEntity.UserId,
					 Oblast = oldEntity.Oblast,
					 ReportYear = report.ReportYear,
					 RegDate = DateTime.Now,
					 RST_Application = oldEntity
				 };

				 //                 var dbReestr = AppContext.RST_ReportReestr.Where(e => e.ReportYear == oldEntity.ReportYear).FirstOrDefault(e => e.BINIIN == reestr.BINIIN);
				 RST_ReportReestr dbReestr = null;
				 if (!string.IsNullOrEmpty(reestr.BINIIN))
				 {
					 dbReestr = AppContext.RST_ReportReestr.OrderBy(e => e.RST_Report.ReportYear)
						 .FirstOrDefault(
							 e =>
								 !e.IsDeleted && e.BINIIN == reestr.BINIIN &&
								 e.RST_Report.ReportYear == (oldEntity.ReportYear - 1));
				 }

				 if (dbReestr != null)
				 {

					 var dbReestr2 = AppContext.RST_ReportReestr.OrderBy(e => e.RST_Report.ReportYear)
						 .FirstOrDefault(
							 e =>
								 !e.IsDeleted && e.BINIIN == reestr.BINIIN &&
								 e.RST_Report.ReportYear == oldEntity.ReportYear);

					 if (dbReestr2 != null)
					 {
						 continue;
					 }

					 if (dbReestr.RST_Report == null)
					 {
						 continue;
					 }
					 if (dbReestr.RST_Report.ReportYear == oldEntity.ReportYear)
					 {
						 if (dbReestr.IsExcluded)
						 {
							 dbReestr.IsExcluded = false;
							 history.RST_ReportReestr = dbReestr;
							 history.Note = "Из исключенных перенесен в общий реестр";
							 history.UserId = dbReestr.UserId;
							 history.StatusId = CodeConstManager.REG_STATUS_REESTR_ID;
							 AppContext.RST_ReestrReportHistory.Add(history);
						 }
					 }
					 else
					 {
						 var dbReestrNew = new RST_ReportReestr
						 {
							 Address = dbReestr.Address,
							 BINIIN = dbReestr.BINIIN,
							 OwnerName = dbReestr.OwnerName,
							 Oblast = oldEntity.Oblast,
							 UserId = dbReestr.UserId,
							 Editor = oldEntity.UserId,
							 EditDate = DateTime.Now,
							 ReportId = report.Id,
							 StatusId = CodeConstManager.OLD_STATUS_REESTR_ID,
							 IDK = dbReestr.IDK
						 };
						 indexPos++;
						 history.RST_ReportReestr = dbReestr;
						 history.UserId = dbReestr.UserId;
						 history.StatusId = CodeConstManager.OLD_STATUS_REESTR_ID;
						 history.Note = "Перенесен с предыдущего года";
						 AppContext.RST_ReportReestr.Add(dbReestrNew);
						 AppContext.RST_ReestrReportHistory.Add(history);
					 }
				 }
				 else
				 {
					 var dbReestr2 = AppContext.RST_ReportReestr.OrderBy(e => e.RST_Report.ReportYear)
						.FirstOrDefault(
							e =>
								!e.IsDeleted && e.BINIIN == reestr.BINIIN &&
								e.RST_Report.ReportYear == oldEntity.ReportYear);

					 if (dbReestr2 != null)
					 {
						 continue;
					 }

					 //----проверить идк 
					 if (reestr.IDK == null || reestr.IDK == "")
						 reestr.IDK = GenerateIDK(idk, indexPos);

					 var user = RegistredUser(reestr);
					 dbReestr = new RST_ReportReestr
					 {
						 Address = reestr.Address,
						 BINIIN = reestr.BINIIN,
						 OwnerName = reestr.OwnerName,
						 Oblast = oldEntity.Oblast,
						 SEC_User1 = user,
						 Editor = oldEntity.UserId,
						 EditDate = DateTime.Now,
						 ReportId = report.Id,
						 StatusId = CodeConstManager.NEW_STATUS_REESTR_ID, //CodeConstManager.REG_STATUS_REESTR_ID,
						 IDK = reestr.IDK
					 };

					 indexPos++;
					 history.RST_ReportReestr = dbReestr;
					 history.SEC_User = user;
					 history.StatusId = CodeConstManager.NEW_STATUS_REESTR_ID; //CodeConstManager.REG_STATUS_REESTR_ID,
					 history.Note = "Первичная регистрация в реестр";
					 //                     reestr.RST_Application = oldEntity;
					 //                     reestr.StatusId = CodeConstManager.REG_STATUS_REESTR_ID;
					 //                     reestr.CreateDate = DateTime.Now;
					 AppContext.RST_ReportReestr.Add(dbReestr);
					 AppContext.RST_ReestrReportHistory.Add(history);


					 AppContext.SaveChanges();
					 //                     RegistredSubject(reestr);
				 }
			 }
		 }
    }
}