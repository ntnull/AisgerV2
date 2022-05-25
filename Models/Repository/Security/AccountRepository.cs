using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Aisger.Utils;
using NPOI.SS.Formula.Functions;

namespace Aisger.Models.Repository.Security
{
    public class AccountRepository : SqlRepository
    {

        public virtual List<SEC_User> GetAll()
        {
            return AppContext.Set<SEC_User>().Where(e => !e.IsDeleted).OrderByDescending(e => e.Id).ToList();
        }

        public SEC_User GetUserFromPwd(string login, string pwd)
        {
            string encode = RegJurnalManager.Instance.Encrypt(pwd);
            SEC_User user = GetAll().Where(o => o.Login == login && o.Pwd == encode).SingleOrDefault();
            return user;
        }
        public virtual void UpdatePwd(SEC_User model, long? author)
        {
            string rwd = RegJurnalManager.Instance.Encrypt(model.Pwd);
            var user = AppContext.SEC_User.FirstOrDefault(e => e.Id == model.Id);
            if (user != null)
            {
                user.Pwd = rwd;
                user.ConfirmPwd = rwd;
                AppContext.SaveChanges();
            }
            new SecUserRepository().SaveHistoryUser(user, author);
//            string sql = "update SEC_User set Pwd='" + rwd + "' where ID=" + environmental.Id;
//            AppContext.Database.ExecuteSqlCommand(sql);
            //            EFDbSet.Context.SaveChanges();
        }
        public IList<SEC_User> GetByRightRole(string code)
        {
            var right = AppContext.SEC_RightPermission.FirstOrDefault(e => e.Code == code);
            if (right == null)
            {
                return new List<SEC_User>();
            }
            var rolesId = right.SEC_RolePermission.Select(e => e.RolesId);
            var roles = AppContext.SEC_Roles.Where(e => rolesId.Contains(e.Id)).Select(e=>e.Id);
            return GetAll().Where(e=>!e.IsGuest).Where(e => e.RolesId != null && roles.Contains(e.RolesId.Value)).ToList();
        }
        public virtual List<SEC_User> GetEmployees()
        {
            return AppContext.Set<SEC_User>().Where(e => !e.IsDeleted && e.OrganizationId != null).OrderBy(e => e.LastName).ToList();
        }

        public virtual List<SEC_User> GetEmployeesByOrgId(long orgId)
        {
            return AppContext.Set<SEC_User>().Where(e => !e.IsDeleted && e.OrganizationId == orgId).OrderBy(e => e.LastName).ToList();
        }
        public void RegistrationNewUser(RegisterModel model)
        {

            var user = new SEC_User
            {
                Login = model.UserName,
                Pwd = Encrypt(model.Password)
            };

            AppContext.SEC_User.Add(user);
            AppContext.SaveChanges();
        }

        public SEC_User GetUserByBin(string bin)
        {
            return AppContext.SEC_User.FirstOrDefault(e => e.Login == bin && !e.IsDeleted);

        }


        public string Encrypt(string input)
        {
            const string salt = "My_$@lt_v@1Ue";
            const int iteration = 3000;
            const int bytes = 32;
            var rfc2898 = new Rfc2898DeriveBytes(input, Encoding.UTF8.GetBytes(salt), iteration);
            string myEncryptedKey = Convert.ToBase64String(rfc2898.GetBytes(bytes));
            return myEncryptedKey;
        }


        public bool CheckAccount(string userName, string password)
        {
            var pwd = Encrypt(password);
            return AppContext.SEC_User.Any(e => e.Login == userName && e.Pwd == pwd);
        }


        public SEC_User GetCurrentUser(string userName)
        {
            return AppContext.SEC_User.SingleOrDefault(e => (e.Login == userName || e.Pwd == userName));
        }
        public SEC_User GetUser(long? userId)
        {
            if (userId == null)
            {
                return null;
            }
            var user = AppContext.SEC_User.SingleOrDefault(e => e.Id == userId.Value);
            if (user == null)
            {
                return null;
            }
            /*user.RptObjects = new List<RPT_Object>();
            foreach (var obj in user.RPT_Object)
            {
                user.RptObjects.Add(obj);
            }
            if (user.RptObjects.Count == 0)
            {
                user.RptObjects.Add(new RPT_Object());
            }*/
            return user;
        }

        public SEC_User GetUserById(long? userId)
        {

            if (userId == null)
            {
                return null;

            }
            var user = AppContext.SEC_User.SingleOrDefault(e => e.Id == userId.Value);
            if (user == null)
            {
                return null;
            }

            return user;
        }
        public SEC_User GetUserByLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return null;
            }
            return AppContext.SEC_User.SingleOrDefault(e => e.Login == login);
        }
        public List<DIC_Kato> GetKatos(long? refParent, bool mandatory)
        {
            if (refParent == null)
            {
                return new List<DIC_Kato>();
            }
            var parentId = refParent.Value;
            var list = AppContext.DIC_Kato.Where(e => e.refParent == parentId).ToList();
            if (!mandatory)
            {
                list.Insert(0, new DIC_Kato { Id = 0, NameRu = "" });
            }
            return list;
        }

        public bool IsAcceptReport(long? userId)
        {
            if (userId == null)
            {
                return false;
            }
            var rolesId = AppContext.SEC_RolePermission.Where(e => e.SEC_RightPermission.Code== CodeConstManager.ACCEPT_REPORT).Select(e=>e.SEC_Roles.Id);
          
            return AppContext.SEC_User.Any(e => e.Id == userId && rolesId.Contains(e.RolesId.Value));
        }
    }
}