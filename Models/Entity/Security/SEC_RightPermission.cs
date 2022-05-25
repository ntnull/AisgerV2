using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    public partial class SEC_RightPermission : IObject
    {
        public DateTime CreateDate { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}