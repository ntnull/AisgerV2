using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
	public class MAP_EE2Excel : MAP_FormEE2Manager
	{
		public string FormEE2Comments { get; set; }
		public Nullable<double> TotalArea { get; set; }
		public Nullable<double> NumberOfStoreys { get; set; }
		public string ReportYear { get; set; }
		public string EnergySource { get; set; }
		public string ExpenceEnergy { get; set; }
		public string EEDetailsName { get; set; }
		public string EEDetailModelsName { get; set; }
		public string CountOfFixtures { get; set; }
		public string CountOfLamps { get; set; }
		public string Power { get; set; }
		public string CPRA { get; set; }
		public string AggregatePower { get; set; }
		public string AverageTariff { get; set; }
		public string WorkingHours { get; set; }
		public string MaintenanceCosts { get; set; }
		public string Comments { get; set; }
		public int? CNT { get; set; }
	}
}