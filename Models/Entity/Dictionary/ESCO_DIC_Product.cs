using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    public partial class ESCO_DIC_Product : IObject
    {
        public string CurrentUrlImage { get; set; }
        public IList<string> AttachFiles { get; set; }
    }
}