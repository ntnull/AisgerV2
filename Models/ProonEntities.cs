using System;
//using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Aisger.Models;
using Aisger.Models.Entity;
using Aisger.Models.Repository;

namespace Aisger.Models
{
    public partial class aisgerEntities : IDbContext
    {
        //        private ILogDbAction _logger;


        //        public SEC_USER CurrentUser { get; set; }
        //        public DbSet<EntityChange> EntityChanges { get; set; }


        public aisgerEntities(bool isProxy) : this()
        {
            this.Configuration.ProxyCreationEnabled = isProxy;
        }

        public void MarkAsAdded<T>(T entity) where T : class
        {
            Entry(entity).State = System.Data.Entity.EntityState.Added;
            Set<T>().Add(entity);
        }

        public void MarkAsDeleted<T>(T entity) where T : class
        {
            Attach(entity);
            Entry(entity).State = System.Data.Entity.EntityState.Deleted;
            Set<T>().Remove(entity);
        }

        public void MarkAsModified<T>(T entity) where T : class
        {
            Attach(entity);
            Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public void Commit(bool withLogging)
        {
            BeforeCommit();
            ILogDbAction logger = null;
            if (withLogging)
            {
                logger = new LogDbAction(this);
                logger.Run();
            }
            try
            {
                SaveChanges();
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
            if (withLogging)
            {

                logger.SaveEvents();
                SaveChanges();
            }
        }

        // откат всех изменений в объектах
        public void Rollback()
        {
            ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }

        public void EnableTracking(bool isEnable)
        {
            Configuration.AutoDetectChangesEnabled = isEnable;
        }

        public void SetEntityState<T>(T entity, System.Data.Entity.EntityState state) where T : class
        {
            Entry(entity).State = state;
        }

        public DbChangeTracker GetChangeTracker()
        {
            return ChangeTracker;
        }

        public System.Data.Entity.EntityState GetEntityState<T>(T entity) where T : class
        {
            return Entry(entity).State;
        }

        public IQueryable<T> Find<T>() where T : class
        {
            return Set<T>();
        }

        public DbEntityEntry GetDbEntry<T>(T entity) where T : class
        {
            return Entry(entity);
        }

        public void Attach<T>(T entity) where T : class
        {
            if (Entry(entity).State == System.Data.Entity.EntityState.Detached)
            {
                Set<T>().Attach(entity);
            }
        }

        private void BeforeCommit()
        {
            UndoExistAddedEntitys();
        }

        //исправление ситуации, когда есть объекты помеченные как  новые, но при этом существующие в базе данных
        private void UndoExistAddedEntitys()
        {
            var dbEntityEntries =
                GetChangeTracker().Entries().Where(x => x.State == EntityState.Added);
            foreach (DbEntityEntry dbEntityEntry in dbEntityEntries)
            {
                if (GetKeyValue(dbEntityEntry.Entity) > 0)
                {
                    SetEntityState(dbEntityEntry.Entity, EntityState.Unchanged);
                }
            }
        }

        public static long GetKeyValue<T>(T entity) where T : class
        {
            var dbEntity = entity as IEntity;
            if (dbEntity == null)
                throw new ArgumentException("Entity should be IEntity type - " + entity.GetType().Name);

            return dbEntity.Id;
        }

      
    }
}