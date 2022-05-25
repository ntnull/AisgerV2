using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;

namespace Aisger.Utils
{
    public class RegJurnalManager
    {
        public const string LOGIN_CODE = "LOGIN";
        public const string ADD_CODE = "ADD_OBJ";
        public const string DELETE_CODE = "DELETE_OBJ";
        public const string EDIT_CODE = "UPDATE_OBJ";
        public const string LOGOUT_CODE = "LOGOUT";
        private static readonly RegJurnalManager instance = new RegJurnalManager();
        private IDictionary<string, long> Dictionary = new Dictionary<string, long>();
        private SecJurnalEventRepository JurEventRepository;
        public static RegJurnalManager Instance
        {
            get { return instance; }
        }

        public RegJurnalManager()
        {
            JurEventRepository = new SecJurnalEventRepository();
            var repository = new SecDicEventTypeRepository();
            IOrderedQueryable<SEC_DIC_EventType> eventss = repository.GetAll();
            foreach (var jurDicEventype in eventss)
            {
                if (!Dictionary.ContainsKey(jurDicEventype.Code))
                {
                    Dictionary.Add(jurDicEventype.Code, jurDicEventype.Id);
                }
            }
        }

        public void Login(string name, long id)
        {
            var events = new SEC_JurEvent { Author = name, UserId = id };
            events.Event = name + " авторизован";
            events.RegisterDate = DateTime.Now;
            if (Dictionary.ContainsKey(LOGIN_CODE))
            {
                events.EventTypeId = Dictionary[LOGIN_CODE];
            }
            JurEventRepository.SaveObject(events);
        }

        public void LogOut(long? userId)
        {
            var user = new AccountRepository().GetUserById(userId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            var events = new SEC_JurEvent
            {
                Author = userName,
                UserId = userId,
                Event = userName + " вышел из системы",
                RegisterDate = DateTime.Now
            };
            if (Dictionary.ContainsKey(LOGOUT_CODE))
            {
                events.EventTypeId = Dictionary[LOGOUT_CODE];
            }
            JurEventRepository.SaveObject(events);
        }


        public void AddObject(string typeName, long id, string className, long? userId, string userName)
        {
            RegJur("Добавлен объект типа:'" + typeName + "' с индентификтором=" + id, ADD_CODE, className, userId, userName);
        }

        public void DelObject(string typeName, long id, string className, long? userId, string userName)
        {
            RegJur("Удален объект типа:'" + typeName + "' с индентификтором=" + id, DELETE_CODE, className, userId, userName);
        }

        public void EditObject(string typeName, long id, string className, long? userId, string userName)
        {
            RegJur("Изменен объект типа:'" + typeName + "' с индентификтором=" + id, EDIT_CODE, className, userId, userName);
        }
        public string Encrypt(string input)
        {
            string salt = "My_$@lt_v@1Ue";
            int iteration = 3000;
            int bytes = 32;
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(input, Encoding.UTF8.GetBytes(salt), iteration);
            string my_encrypted_Key = Convert.ToBase64String(rfc2898.GetBytes(bytes));
            return my_encrypted_Key;
        }

        private void RegJur(string desc, string code, string className, long? userId, string userName)
        {
            var events = new SEC_JurEvent { Author = userName, UserId = userId, Event = desc, RegisterDate = DateTime.Now, ClassName = className };
            if (Dictionary.ContainsKey(code))
            {
                events.EventTypeId = Dictionary[code];
            }
            JurEventRepository.SaveObject(events);
        }
    }
}