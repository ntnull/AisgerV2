using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using Aisger.Utils;

namespace Aisger.Models.Repository.Security
{
    public class SecJurnalEventRepository : SqlRepository
    {
        public const long ADD_EVENT_TYPE_ID = 1L;
        public const long MODIFY_EVENT_TYPE_ID = 2L;
        public const long DELETE_EVENT_TYPE_ID = 3L;
        public const long USER_LOGIN_EVENT_TYPE_ID = 4L;
        public const long USER_LOGOF_EVENT_TYPE_ID = 5L;
        public IOrderedQueryable<SEC_JurEvent> GetUserEvents(long? userId)
        {
            return AppContext.SEC_JurEvent.Where(o => o.UserId.Equals(userId)).OrderBy(o => o.RegisterDate);
        }

        public IOrderedQueryable<SEC_JurEvent> GetAll()
        {
            return AppContext.SEC_JurEvent.OrderBy(o => o.Id);
        }
        public SEC_JurEvent GetById(long id)
        {
            return AppContext.SEC_JurEvent.FirstOrDefault(o => o.Id==id);
        }

        public virtual void SaveObject(SEC_JurEvent environmental)
        {
            if (environmental.UserId == 0)
            {
                environmental.UserId = null;
            }
            AppContext.SEC_JurEvent.Add(environmental);
            try
            {
                AppContext.SaveChanges();

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

        public void SaveNewEvent(SEC_JurEvent newEvent)
        {
                newEvent.RegisterDate = DateTime.Now;

                var secUser = (SEC_User) HttpContext.Current.Session[CodeConstManager.SESSION_USER];
                newEvent.UserId = secUser.Id;
                newEvent.Author = secUser.Login;
                AppContext.SEC_JurEvent.Add(newEvent);
                AppContext.SaveChanges();
        }
    }

  
}