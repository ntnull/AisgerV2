using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository
{
	public class DicObjectClass
	{
		public long Id { get; set; }
		public string Name { get; set; }
		private Boolean _ischecked = false;
		public Boolean IsChecked { get { return _ischecked; } set { _ischecked = value; } }
	}
}