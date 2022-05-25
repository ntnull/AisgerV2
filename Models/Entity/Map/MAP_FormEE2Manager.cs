using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
    public partial class MAP_FormEE2Manager
    {
        public long SecUserId { get; set; }
        public string User_Name { get; set; }
        public long Oblast { get; set; }
        public string Oblast_Name { get; set; }
        public string Region_Name { get; set; }
        public string Status_Name { get; set; }
        public string BINIIN { get; set; }
        public string JuridicalName { get; set; }
        public long? OkedId { get; set; }
        public string Oked_Name { get; set; }
    }
}