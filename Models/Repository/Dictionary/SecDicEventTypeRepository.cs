using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Dictionary
{
    public class SecDicEventTypeRepository : SqlRepository
    {

        public IOrderedQueryable<SEC_DIC_EventType> GetAll()
        {
            return AppContext.Set<SEC_DIC_EventType>().OrderBy(e => e.NameRu);
        }

        public virtual SEC_DIC_EventType GetByCode(object code)
        {
            return GetAll().FirstOrDefault(c => c.Code == (string)code);
        }
        public SEC_DIC_EventType GetById(object id)
        {
            return GetAll().FirstOrDefault(c => c.Id == (long)id);
        }

    }
}