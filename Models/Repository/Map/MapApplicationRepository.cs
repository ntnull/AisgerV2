using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Aisger.Utils;

namespace Aisger.Models.Repository.Map
{
    public class MapApplicationRepository : AObjectRepository<MAP_Application>
    {
        public List<MAP_Application> GetListCurrentByUser(long? idUser)
        {
            if (idUser == null)
            {
                return GetAll();
            }
            return GetQueryByDescending(e => !e.IsDeleted && e.UserId == idUser, true, e => e.Id).ToList();
        }
        public override string TitleObject
        {
            get { return ResourceSetting.MAP_Application; }
        }
        protected override void BeforeSave(MAP_Application attachedEntity, MAP_Application oldEntity)
        {
            if (!oldEntity.IsDeleted && oldEntity.IsCollectionEdit)
            {
                UpdateEvent(attachedEntity, oldEntity);
                UpdateProduct(attachedEntity, oldEntity);
            }
        }
     private void UpdateEvent(MAP_Application attachedEntity, MAP_Application oldEntity)
        {
            if (oldEntity.Id == 0)
            {
                foreach (var crFish in oldEntity.MapApplicationEvents)
                {
                    crFish.MAP_Application = oldEntity;
                    AppContext.MAP_ApplicationEvent.Add(crFish);
                }
                return;
            }

            var bufferzones = AppContext.MAP_ApplicationEvent.Where(e => e.ApplcationId == attachedEntity.Id);
            var bufferZonsIds = new List<long>();

            foreach (var entity in oldEntity.MapApplicationEvents)
            {
                if (entity.Id == 0)
                {
                    entity.ApplcationId = oldEntity.Id;
                    AppContext.MAP_ApplicationEvent.Add(entity);
                }
                else
                {
                    var oldaccess = bufferzones.SingleOrDefault(e => e.Id == entity.Id);
                    if (oldaccess != null)
                    {
                        oldaccess.EventName = entity.EventName;
                        oldaccess.Note = entity.Note;
                        oldaccess.PaybackPeriod = entity.PaybackPeriod;
                        oldaccess.PlanExpend = entity.PlanExpend;
                        oldaccess.SavedCost = entity.SavedCost;
                        oldaccess.SavedEnergy = entity.SavedEnergy;
                    }
                    bufferZonsIds.Add(entity.Id);
                }
            }
            var bufferCoordDeleteList = bufferzones.Where(e => !bufferZonsIds.Contains(e.Id));
            foreach (var entity in bufferCoordDeleteList)
            {
                AppContext.MAP_ApplicationEvent.Remove(entity);
            }
        }
     private void UpdateProduct(MAP_Application attachedEntity, MAP_Application oldEntity)
     {
         if (oldEntity.Id == 0)
         {
             foreach (var crFish in oldEntity.MapApplicationProducts)
             {
                 crFish.MAP_Application = oldEntity;
                 crFish.Disrciminator = CodeConstManager.DISC_PRODUCT;
                 AppContext.MAP_ApplicationProduct.Add(crFish);
             }
             foreach (var crFish in oldEntity.ProjectPowers)
             {
                 crFish.MAP_Application = oldEntity;
                 crFish.Disrciminator = CodeConstManager.DISC_POWER;
                 AppContext.MAP_ApplicationProduct.Add(crFish);
             }
             foreach (var crFish in oldEntity.InKinds)
             {
                 crFish.MAP_Application = oldEntity;
                 crFish.Disrciminator = CodeConstManager.DISC_IN_KIND;
                 AppContext.MAP_ApplicationProduct.Add(crFish);
             }
             foreach (var crFish in oldEntity.InValueTerms)
             {
                 crFish.MAP_Application = oldEntity;
                 crFish.Disrciminator = CodeConstManager.DISC_IN_VALUE_TERM;
                 AppContext.MAP_ApplicationProduct.Add(crFish);
             }
             return;
         }

         var bufferzones = AppContext.MAP_ApplicationProduct.Where(e => e.ApplcationId == attachedEntity.Id);
         var bufferZonsIds = new List<long>();

         foreach (var entity in oldEntity.MapApplicationProducts)
         {
             if (entity.Id == 0)
             {
                 entity.ApplcationId = oldEntity.Id;
                 entity.Disrciminator = CodeConstManager.DISC_PRODUCT;
                 AppContext.MAP_ApplicationProduct.Add(entity);
             }
             else
             {
                 var oldaccess = bufferzones.SingleOrDefault(e => e.Id == entity.Id);
                 if (oldaccess != null)
                 {
                     oldaccess.ProductName = entity.ProductName;
                     oldaccess.ProductUnit = entity.ProductUnit;
                     oldaccess.ProductVolume = entity.ProductVolume;
                 }
                 bufferZonsIds.Add(entity.Id);
             }
         }
         foreach (var entity in oldEntity.ProjectPowers)
         {
             if (entity.Id == 0)
             {
                 entity.ApplcationId = oldEntity.Id;
                 entity.Disrciminator = CodeConstManager.DISC_POWER;
                 AppContext.MAP_ApplicationProduct.Add(entity);
             }
             else
             {
                 var oldaccess = bufferzones.SingleOrDefault(e => e.Id == entity.Id);
                 if (oldaccess != null)
                 {
                     oldaccess.ProductName = entity.ProductName;
                     oldaccess.ProductUnit = entity.ProductUnit;
                     oldaccess.ProductVolume = entity.ProductVolume;
                 }
                 bufferZonsIds.Add(entity.Id);
             }
         }
         foreach (var entity in oldEntity.InKinds)
         {
             if (entity.Id == 0)
             {
                 entity.ApplcationId = oldEntity.Id;
                 entity.Disrciminator = CodeConstManager.DISC_IN_KIND;
                 AppContext.MAP_ApplicationProduct.Add(entity);
             }
             else
             {
                 var oldaccess = bufferzones.SingleOrDefault(e => e.Id == entity.Id);
                 if (oldaccess != null)
                 {
                     oldaccess.ProductName = entity.ProductName;
                     oldaccess.ProductUnit = entity.ProductUnit;
                     oldaccess.ProductVolume = entity.ProductVolume;
                 }
                 bufferZonsIds.Add(entity.Id);
             }
         }
         foreach (var entity in oldEntity.InValueTerms)
         {
             if (entity.Id == 0)
             {
                 entity.ApplcationId = oldEntity.Id;
                 entity.Disrciminator = CodeConstManager.DISC_IN_VALUE_TERM;
                 AppContext.MAP_ApplicationProduct.Add(entity);
             }
             else
             {
                 var oldaccess = bufferzones.SingleOrDefault(e => e.Id == entity.Id);
                 if (oldaccess != null)
                 {
                     oldaccess.ProductName = entity.ProductName;
                     oldaccess.ProductUnit = entity.ProductUnit;
                     oldaccess.ProductVolume = entity.ProductVolume;
                 }
                 bufferZonsIds.Add(entity.Id);
             }
         }
         var bufferCoordDeleteList = bufferzones.Where(e => !bufferZonsIds.Contains(e.Id));
         foreach (var entity in bufferCoordDeleteList)
         {
             AppContext.MAP_ApplicationProduct.Remove(entity);
         }
     }

       /* public override MAP_Application Add(MAP_Application entity)
        {
            BeforeSave(null, entity);
            AppContext.MarkAsAdded(entity);
            AppContext.Commit(true);
            entity.AppNumber = GetApplicationId(entity.Id);
            AppContext.Commit(true);
            return entity;
        }*/
        public string GetApplicationId(long id)
        {

            var idStr = id.ToString(CultureInfo.InvariantCulture);
            return
                 CodeConstManager.TEMPLATE_APPLID.Substring(0, CodeConstManager.TEMPLATE_APPLID.Length - idStr.Length) +
                 idStr;
        }

        public MAP_ApplicationHistory SaveHistory(MAP_ApplicationHistory history)
        {
            AppContext.MAP_ApplicationHistory.Add(history);
            AppContext.SaveChanges();
            return history;
        }
    }
}