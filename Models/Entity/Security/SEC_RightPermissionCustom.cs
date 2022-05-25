using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Security
{
    public class SEC_RightPermissionCustom
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public Nullable<long> ParentId { get; set; }
    }
}