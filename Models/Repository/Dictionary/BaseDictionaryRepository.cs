using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using Npgsql;

namespace Aisger.Models.Repository.Dictionary
{
    public abstract class BaseDictionaryRepository<T> : SqlRepository, IRepository<T> where T : class, IEntityDictionary, new()
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
            return AppContext.Set<T>().Where(o => !o.IsDeleted).OrderBy(e => e.Id).ToList();
        }

        public virtual IOrderedEnumerable<T> GetList()
        {
            return GetAll().Where(o => !o.IsDeleted).OrderByDescending(e => e.Id);
        }
        public T Find(int entityId)
        {
            return AppContext.Set<T>().Find(entityId);
        }

        // виртуальный метод. вызывает перед сохранением объектов, может быть определен в дочерних классах

        public T SaveOrUpdate(T entity, long? userId)
        {
            var user = new AccountRepository().GetUserById(userId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            var iDbEntity = entity as IEntityDictionary;
            string className = typeof(T).FullName;
            if (iDbEntity == null)
                throw new ArgumentException("entity should be IObject type", "entity");

            if (iDbEntity.Id == 0)
            {
                iDbEntity.CreateDate = DateTime.Now;
                //                RegJurnalManager.Instance.AddObject(TitleObject, iDbEntity.Id, className, userId, userName);
                return Add(entity);
            }
            iDbEntity.EditDate = DateTime.Now;
            RegJurnalManager.Instance.EditObject(TitleObject, iDbEntity.Id, className, userId, userName);
            return Update(entity);
        }

        public virtual string TitleObject
        {
            get { return ""; }
        }

        public T Add(T entity)
        {
            BeforeSave(null, entity);
            AppContext.MarkAsAdded(entity);
            AppContext.Commit(true);
            return entity;
        }

        public virtual T Update(T entity)
        {
            var iDbEntity = entity as IEntityDictionary;
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

        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> expression, bool order, Expression<Func<T, long>> keySelector)
        {
            return order
                ? AppContext.Set<T>().Where(expression).OrderBy(keySelector)
                : AppContext.Set<T>().Where(expression);
        }

        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> expression)
        {
            return AppContext.Set<T>().Where(expression);
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

        public virtual void Delete(long id, long? userId)
        {
            var obj = GetById(id);
            obj.IsDeleted = true;
            PrepareDelete(obj);
            Update(obj);
            Update(obj);
            var user = new AccountRepository().GetUserById(userId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            RegJurnalManager.Instance.DelObject(TitleObject, id, typeof(T).FullName, userId, userName);
        }

        protected virtual void PrepareDelete(T obj)
        {
        }
    }
    public class RstDicStatusRepository : BaseDictionaryRepository<RST_DIC_Status>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.RstDicStatus; }
        }
    }
    public class DicOkedRepository : BaseDictionaryRepository<DIC_OKED>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.DicOked; }
        }
    }
    public class RstDicReasonRepository : AObjectRepository<RST_DIC_Reason>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.RstDicReason; }
        }
    }
    public class DicUnitRepository : AObjectRepository<DIC_Unit>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.DicUnit; }
        }

        public List<SelectListItem> GetSelectList()
        {
            return GetQuery(u => !u.IsDeleted).Select(u => new SelectListItem()
            {
                Value = u.Id.ToString(),
                Text = u.NameRu
            }).ToList();
        }

    }

    public class SubDucTypeCounterRepository : AObjectRepository<SUB_DIC_TypeCounter>
    {

    }

    public class SubDicEventRepository : SqlRepository
    {
        public List<SUB_DIC_Event> GetAll(long? userId)
        {
            if (userId == null)
            {
                return new List<SUB_DIC_Event>();
            }
            return AppContext.SUB_DIC_Event.Where(e => e.UserId == userId.Value).ToList();
        }
    }
    public class SubDicTypeResourceRepository : AObjectRepository<SUB_DIC_TypeResource>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.SubDicTypeResource; }
        }

        public void SaveObject(SUB_DIC_TypeResource resource)
        {
            resource.Id = GetAll().Max(e => e.Id) + 1;
            resource.CreateDate = DateTime.Now;
            AppContext.SUB_DIC_TypeResource.Add(resource);
            AppContext.SaveChanges();
        }

    }
    public class SubDicKindResourceRepository : AObjectRepository<SUB_DIC_KindResource>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.SubDicKindResource; }
        }
    }
    public class SubDicStatusRepository : BaseDictionaryRepository<SUB_DIC_Status>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.RstDicStatus; }
        }
    }
    public class SubDicKindTabOneRepository : BaseDictionaryRepository<SUB_DIC_KindTabOne>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.SubDicKindTabOne; }
        }
    }
    public class SubDicKindTabTwoRepository : BaseDictionaryRepository<SUB_DIC_KindTabTwo>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.SubDicKindTabTwo; }
        }
    }
    public class DicKindUserRepository : BaseDictionaryRepository<DIC_KindUser>
    {

    }

    public class DicTypeApplicationRepository : BaseDictionaryRepository<DIC_TypeApplication>
    {

    }
    public class MapDicStatusRepository : BaseDictionaryRepository<MAP_DIC_Status>
    {

    }

    public class SubDicNormEnergyRepository : SqlRepository
    {
        public List<SUB_DIC_NormEnergy> GetAll()
        {
            return AppContext.SUB_DIC_NormEnergy.OrderBy(e => e.Id).ToList();
        }
    }

    public class DicGuRepository : SqlRepository
    {
        public List<DIC_GU> GetAll()
        {
            return AppContext.DIC_GU.OrderBy(e => e.Id).ToList();
        }
    }

    public class SubDicEnergyindicator : SqlRepository
    {
        public List<sub_dic_energyindicator> GetAll()
        {
            return AppContext.sub_dic_energyindicator.OrderBy(e => e.id).ToList();
        }
    }

    public class EAuditDicTypeRepository : BaseDictionaryRepository<EAUDIT_DIC_TypeResource>
    {
        
    }

    public class EAuditStatusRepository : BaseDictionaryRepository<EAUDIT_DIC_Statuses>
    {

    }
}