using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Aisger.Models
{
    public interface IDbContext : IDisposable
    {
        IQueryable<T> Find<T>() where T : class;

        void MarkAsAdded<T>(T entity) where T : class;

        void MarkAsDeleted<T>(T entity) where T : class;

        void MarkAsModified<T>(T entity) where T : class;

        void Commit(bool withLogging);

        //откатывает изменения во всех модифицированных объектах
        void Rollback();

        // включает или отключает отслеживание изменений объектов
        void EnableTracking(bool isEnable);

        System.Data.Entity.EntityState GetEntityState<T>(T entity) where T : class;

        void SetEntityState<T>(T entity, System.Data.Entity.EntityState state) where T : class;

        // возвращает объект содержащий список объектов с их состоянием
        DbChangeTracker GetChangeTracker();

        DbEntityEntry GetDbEntry<T>(T entity) where T : class;
    }

}