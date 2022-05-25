using Aisger.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  Aisger.Models.Repository.Security
{
    //public class RemoveDuplicateRepository<T> : SqlRepository
    //{
    //    public virtual string updateObject(T entity)
    //    {
    //        string ErrorMessage = "";

    //        #region entity update
    //        try
    //        {
    //            var iDbEntity = item as IObjectUser;
    //            if (iDbEntity == null)
    //                throw new ArgumentException("entity should be IObject type", "entity");

    //            var attachedEntity = AppContext.Set<T>().Find(iDbEntity.Id);

    //            AppContext.Entry(attachedEntity).CurrentValues.SetValues(item);
    //            AppContext.Commit(true);
    //            //AppContext.Entry(item).State = EntityState.Modified;
    //            //AppContext.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            ErrorMessage = ex.Message;
    //        }
    //        #endregion

    //        return ErrorMessage;
    //    }

    //    //----
    //    public virtual string removeObject(T entity)
    //    {
    //        string ErrorMessage = "";

    //        #region entity update
    //        try
    //        {
    //            var iDbEntity = item as IObjectUser;
    //            if (iDbEntity == null)
    //                throw new ArgumentException("entity should be IObject type", "entity");
                
    //            var row = AppContext.Set<T>().Find(iDbEntity.Id);
    //            if (row == null)
    //                return "";

                
    //            AppContext.Set<T>().Remove(row);
    //            int cnt = AppContext.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            ErrorMessage = ex.Message;
    //        }
    //        #endregion

    //        return ErrorMessage;
    //    }
    //}
}