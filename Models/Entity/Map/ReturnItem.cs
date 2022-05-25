using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Map
{
	public class ReturnItem<T>
	{
		public T Item { get; set; }
		public string ErrorMessage { get; set; }
	}
}