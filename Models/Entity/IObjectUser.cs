using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity
{

    public interface IEntity2
    {
        long Id { get; set; }

    }

    /// <summary>
    /// интерфейс для объектов
    /// </summary>
    public interface IObjectUser : IEntity2
    {

    }
}