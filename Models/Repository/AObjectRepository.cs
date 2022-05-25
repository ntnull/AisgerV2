#region

using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using Aisger.Models.Entity;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

#endregion

namespace Aisger.Models.Repository
{
    /// <summary>
    ///     абстрактрый класс работа с бд
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AObjectRepository<T> : SqlRepository, IRepository<T> where T : class, IObject, new()
    {
        public DbEntityValidationResult Validate(T entity)
        {
            return AppContext.Entry(entity).GetValidationResult();
        }

        public string ValidateAndReturnErrorString(T entity, out bool isValid)
        {
            DbEntityValidationResult dbEntityValidationResult = AppContext.Entry(entity).GetValidationResult();
            isValid = dbEntityValidationResult.IsValid;
            return !dbEntityValidationResult.IsValid
                ? DbValidationMessageParser.GetErrorMessage(dbEntityValidationResult)
                : string.Empty;
        }

        public virtual List<T> GetAll()
        {
            return AppContext.Set<T>().Where(o => !o.IsDeleted).ToList();
        }

        public virtual List<T> GetCollectionList()
        {
            return GetQueryByDescending(e => !e.IsDeleted, true, e => e.Id).ToList();
        }
        public T Find(int entityId)
        {
            return AppContext.Set<T>().Find(entityId);
        }

        // виртуальный метод. вызывает перед сохранением объектов, может быть определен в дочерних классах

        public virtual T SaveOrUpdate(T entity, long? userId)
        {
            var user = new AccountRepository().GetUserById(userId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            var iDbEntity = entity as IObject;
            string className = typeof(T).FullName;
            if (iDbEntity == null)
                throw new ArgumentException("entity should be IObject type", "entity");

            if (iDbEntity.Id == 0)
            {
                try
                {
                    iDbEntity.CreateDate = DateTime.Now;
//                    RegJurnalManager.Instance.AddObject(TitleObject, iDbEntity.Id, className, userId, userName);
                    return Add(entity);
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

            // original
            iDbEntity.EditDate = DateTime.Now;


            RegJurnalManager.Instance.EditObject(TitleObject, iDbEntity.Id, className, userId, userName);
            return Update(entity);
        }
        public virtual string TitleObject
        {
            get { return ""; }
        }

        public virtual T Add(T entity)
        {
            BeforeSave(null, entity);
            AppContext.MarkAsAdded(entity);
            AppContext.Commit(true);
            return entity;
        }

		public virtual T Update(T entity)
		{
			var iDbEntity = entity as IObject;
			if (iDbEntity == null)
				throw new ArgumentException("entity should be IObject type", "entity");

			var attachedEntity = AppContext.Set<T>().Find(iDbEntity.Id);
			AppContext.Entry(attachedEntity).CurrentValues.SetValues(entity);

			BeforeSave(attachedEntity, entity);
			AppContext.Commit(true);

			return entity;
		}

        public void Delete(T entity)
        {
            AppContext.MarkAsDeleted(entity);
            AppContext.Commit(true);

        }

        /// <summary>
        ///     Получить объект по ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(long id)
        {
            return AppContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> expression, bool order,
            Expression<Func<T, long>> keySelector)
        {
            return order
                ? AppContext.Set<T>().Where(expression).OrderBy(keySelector)
                : AppContext.Set<T>().Where(expression);
        }

        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> expression = null)
        {
            return expression == null
                ? AppContext.Set<T>()
                : AppContext.Set<T>().Where(expression);
        }

        public virtual IQueryable<T> GetQueryByDescending(Expression<Func<T, bool>> expression, bool order,
           Expression<Func<T, long>> keySelector)
        {
            return order
                ? AppContext.Set<T>().Where(expression).OrderByDescending(keySelector)
                : AppContext.Set<T>().Where(expression);
        }

        protected virtual void BeforeSave(T attachedEntity, T oldEntity)
        {
        }

        /// <summary>
        ///     Удалить объект
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(long id, long? userId)
        {
            var obj = GetById(id);
            obj.IsDeleted = true;
            PrepareDelete(obj);
            Update(obj);
            var user = new AccountRepository().GetUserById(userId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            RegJurnalManager.Instance.DelObject(TitleObject, id, typeof(T).FullName, userId, userName);
            //                MarkAs
        }

        protected virtual void PrepareDelete(T obj)
        {
        }

        public virtual void Delete<TT>(long id, long? userId)
        {
            var obj = GetById(id);
            obj.IsDeleted = true;
            PrepareDelete(obj);
            Update(obj);
            var user = new AccountRepository().GetUserById(userId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            RegJurnalManager.Instance.DelObject(TitleObject, id, typeof(T).FullName, userId, userName);
            //                MarkAs
        }

    }
}