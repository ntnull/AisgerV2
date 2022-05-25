using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace Aisger.Models.Repository
{
    public interface IRepository<T> where T : class
    {
        aisgerEntities CreateDatabaseContext();

        List<T> GetAll();

        T Find(int entityId);

        T SaveOrUpdate(T entity, long? userId);

        T Add(T entity);

        T Update(T entity);

        void Delete(T entity);

        // возвращает список ошибок
        DbEntityValidationResult Validate(T entity);

        // возвращает строку с ошибками
        string ValidateAndReturnErrorString(T entity, out bool isValid);
    }
}