using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity
{
   
    public interface IRepositoryUser<T> where T : class
    {
        aisgerEntities CreateDatabaseContext();

        string updateObject(T entity);

        string removeObject(T entity);

    }
}