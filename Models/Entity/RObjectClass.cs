using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
	public class RObjectClass<T>
	{
		public List<T> ListItems { get; set; }
		public string ErrorMessage { get; set; }
		private static int _count = 0;
		public int Count { get { return _count; } set { _count = value; } }

		public int? AllCount { get; set; }
	}
}
