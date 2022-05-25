using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Subject
{
	public class DuplicateBin
	{
		public string biniin { get; set; }
		public int cnt { get; set; }
	}

	public class DuplicateUser
	{
		public long user_id { get; set; }
		public string login { get; set; }
		public string juridicalname { get; set; }
		public string user_idk { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string secondname { get; set; }
		public DateTime createdate { get; set; }
		public long? oblast_id { get; set; }
		public string oblast_name { get; set; }
		public string address { get; set; }
		public string rst_id { get; set; }
		public string rst_biniin { get; set; }
		public string rst_year { get; set; }
		public string rst_ownername { get; set; }
		public string rst_idk { get; set; }
		public string sub_id { get; set; }   //sub_form id
		public string sub_year { get; set; }
		public string sub_status { get; set; }
		public string kind_id { get; set; }
		public string kind_name { get; set; }
	}


	public class ChangeBin {
		public long user_id { get; set; }
		public string biniin { get; set; }
		public string idk { get; set; }
		public string juridicalname { get; set; }
		public long? oblast_id{get;set;}
		public string oblast_name { get; set; }
		public string region_name { get; set; }
		public string address { get; set; }
		public string rst_biniin { get; set; }
		public string rst_id { get; set; }
		public string rst_idk { get; set; }
		public string rst_ownername { get; set; }
		public string rst_year { get; set; }
		public bool IsHaveGES { get; set; }
	}
}