using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aisger.Models.Entity
{
    /// <summary>
    /// интерфейс для объектов БД
    /// </summary>
    public interface IEntity
    {
       long Id { get; set; }
        
    }
    /// <summary>
    /// интерфейс для объектов
    /// </summary>
    public interface IObject : IEntity
    {
         System.DateTime CreateDate { get; set; }
         Nullable<System.DateTime> EditDate { get; set; }
         bool IsDeleted { get; set; }
       
    }
}