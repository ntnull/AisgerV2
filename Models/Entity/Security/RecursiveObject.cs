using System;
using System.Collections.Generic;

namespace Aisger.Models.Entity.Security
{
    public class RecursiveObject
    {
        public string data { get; set; }
        public Int64 id { get; set; }
        public FlatTreeAttribute attr { get; set; }
        public List<RecursiveObject> children { get; set; }
    }

    public class FlatTreeAttribute
    {
        public string id;
        public bool selected;
        public bool editable;
    }

    public class JsTreeModel
    {
        public string data;
        public JsTreeAttribute attr;
        public JsTreeModel[] children;
    }

    public class JsTreeAttribute
    {
        public string id;
        public bool selected;
    }

    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<Select2Result> Results { get; set; }
    }

    public class Select2Result
    {
        public string id { get; set; }
        public string text { get; set; }
    }

	public class RecursiveObjectTree
	{
		public string name { get; set; }
		public Int64 id { get; set; }
		public bool isselected { get; set; }
		public List<RecursiveObjectTree> items { get; set; }
	}
}