

namespace Aisger.Models
{
	using System;
	using System.Collections.Generic;

	public partial class VIEW_COLL_IndicatorValuesPerDay
	{
		public long Id { get; set; }
		public Nullable<long> refCmdevice { get; set; }
		public Nullable<System.DateTime> FirstDatetime { get; set; }
		public Nullable<double> FirstValue { get; set; }
		public Nullable<System.DateTime> LastDatetime { get; set; }
		public Nullable<double> LastValue { get; set; }
		public Nullable<System.DateTime> ReportDate { get; set; }
	}
}