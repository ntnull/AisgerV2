using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
    public class Map_FormEE2Recordold
    {
        public long Id { get; set; }
        public long FormEE2Id { get; set; }
        public int ReportYear { get; set; }
        public Nullable<double> EnergySource { get; set; }
        public Nullable<double> ExpenceEnergy { get; set; }
    }

}