using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web;
using Aisger.Models.Entity;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

namespace Aisger.Models.Repository
{
    public class LogDbAction : ILogDbAction
    {
        private readonly aisgerEntities _dbContext;
        private Dictionary<EntityState, string> _operationTypes;

        private IList<SEC_JurEvent> _events;
        public LogDbAction(aisgerEntities context)
        {
            _dbContext = context;
            InitOperationTypes();
        }

        public void Run()
        {
            _events = new List<SEC_JurEvent>();
            LogChangedEntities(EntityState.Added);
            LogChangedEntities(EntityState.Modified);
            LogChangedEntities(EntityState.Deleted);
        }

        public void SaveEvents()
        {
            foreach (var secJurnalEvent in _events)
            {
                if (secJurnalEvent.EventTypeId == SecJurnalEventRepository.ADD_EVENT_TYPE_ID)
                {
                    secJurnalEvent.ObjId = secJurnalEvent.EntityObj.Id;
                }
                _dbContext.SEC_JurEvent.Add(secJurnalEvent);
            }

        }

        private void InitOperationTypes()
        {
            _operationTypes = new Dictionary<EntityState, string>
            {
                {EntityState.Added, "Добавление"},
                {EntityState.Deleted, "Удаление"},
                {EntityState.Modified, "Изменение"}
            };
        }

        private string GetOperationName(EntityState entityState)
        {
            return _operationTypes[entityState];
        }

        private void LogChangedEntities(EntityState entityState)
        {
            IEnumerable<DbEntityEntry> dbEntityEntries =
                _dbContext.ChangeTracker.Entries().Where(x => x.State == entityState);
            foreach (DbEntityEntry dbEntityEntry in dbEntityEntries)
            {
                LogChangedEntitie(dbEntityEntry, entityState);
            }
        }

        private void LogChangedEntitie(DbEntityEntry dbEntityEntry, EntityState entityState)
        {
//            string operationHash = HashGenerator.GenerateHash(10);
            var enitityId = aisgerEntities.GetKeyValue(dbEntityEntry.Entity);
            var dbEntity = dbEntityEntry.Entity as IObject;

            var type1 = dbEntityEntry.Entity.GetType();
            var entityType = ObjectContext.GetObjectType(type1);
            if (entityType == typeof(SEC_JurEvent) || dbEntity == null)
            {
                return;
            }

            var propertyNames = entityState == EntityState.Deleted
                ? dbEntityEntry.OriginalValues.PropertyNames
                : dbEntityEntry.CurrentValues.PropertyNames;
            var secUser = (SEC_User) HttpContext.Current.Session[CodeConstManager.SESSION_USER];
            var secJurnalEvent = new SEC_JurEvent
            {
                UserId = secUser.Id,
                Author = secUser.Login,
                RegisterDate = DateTime.Now,
                ObjId = enitityId,
                ClassName = GetTypeName(entityType),
            };
            if (entityState == EntityState.Added)
            {
                secJurnalEvent.EventTypeId = SecJurnalEventRepository.ADD_EVENT_TYPE_ID;
                secJurnalEvent.EntityObj = dbEntity;
                secJurnalEvent.Event = "Добавлен объект";
            }
            else if (entityState == EntityState.Deleted ||
                     (entityState == EntityState.Modified && dbEntity.IsDeleted))
            {
                secJurnalEvent.EventTypeId = SecJurnalEventRepository.DELETE_EVENT_TYPE_ID;
                secJurnalEvent.Event = "Удален объект";
            }
            else
            {
                secJurnalEvent.EventTypeId = SecJurnalEventRepository.MODIFY_EVENT_TYPE_ID;

                var eventBuilder = new StringBuilder();
                foreach (var propertyName in propertyNames)
                {
                    var property = dbEntityEntry.Property(propertyName);

                    if (entityState == EntityState.Modified && !property.IsModified)
                        continue;

                    var originalValue =
                        entityState != EntityState.Added && property.OriginalValue != null
                            ? property.OriginalValue.ToString()
                            : string.Empty;
                    var modifyValue =
                        entityState != EntityState.Deleted && property.CurrentValue != null
                            ? property.CurrentValue.ToString()
                            : string.Empty;
                    var operationType = GetOperationName(entityState);
                    eventBuilder.AppendLine(GetEventDecription(entityType, enitityId, propertyName, originalValue,
                                modifyValue, operationType));
                }
                secJurnalEvent.Event = eventBuilder.ToString();
            }
            _events.Add(secJurnalEvent);
        }

        private string GetEventDecription(Type typeObj, long id, string propertyName, string originalValue,
            string modifyValue, string operationType)
        {
            var s = new StringBuilder();
            s.Append("Произведена операция ").Append(operationType)
                .Append(" над объектом ").Append(GetTypeName(typeObj)).Append(" id = ").Append(id)
                .Append(", изменены значение (")
                .Append(propertyName)
                .Append(") ''")
                .Append(originalValue)
                .Append("'' на ''")
                .Append(modifyValue).Append("''.");
            return s.ToString();
        }

        private string GetTypeName(Type typeObj)
        {
            var typeName =  ResourceSetting.ResourceManager.GetString(typeObj.Name);
            return typeName ?? typeObj.Name;
        }
    }
}