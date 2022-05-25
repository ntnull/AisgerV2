using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
    public partial class RST_ReportReestr : IEntity
    {
        [Display(Name = "RegDate", ResourceType = typeof(ResourceSetting))]
        public string EditDateStr
        {
            get { return DateHelper.GetDate(EditDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    EditDate = dateTemp.Value;
                }
            }
        }

        public string OblastName {
			
            get
            {
                if (DIC_Kato != null)
                {
                    return DIC_Kato.NameRu;
                }
                return null;
            }
        }
        public string StatusName
        {
            get
            {
                if (RST_DIC_Status != null)
                {
                    return RST_DIC_Status.NameRu;
                }
                return null;
            }
        }
        public string ReasonName
        {
            get
            {
                if (RST_DIC_Reason != null)
                {
                    return RST_DIC_Reason.NameRu;
                }
                return null;
            }
        }

		

        [NotMapped]
        public int? ReportYear { get; set; }
        public IList<string> AttachFiles { get; set; }

	
    }

	public partial class RST_ReportReestrClass
	{
		public long Id { get; set; }
		public Nullable<long> ReportId { get; set; }
		public Nullable<long> ReestrId { get; set; }
		public string Address { get; set; }
		public string BINIIN { get; set; }
		public string OwnerName { get; set; }
		public Nullable<long> StatusId { get; set; }
		public Nullable<long> UserId { get; set; }
		public Nullable<long> Oblast { get; set; }
		public string IDK { get; set; }
	
		public string usrfirstname { get; set; }
		public string usrlastname { get; set; }
		public string usrsecondname { get; set; }
		public string usrjuridicalname { get; set; }
		public string usrpost { get; set; }
		public string usrmobile { get; set; }
		public string usrworkphone { get; set; }
		public string usrinternalphone { get; set; }
		public string usraddress { get; set; }
		public Nullable<bool> usriscvazy { get; set; }
		public string usrresponcefio { get; set; }
		public string usrresponcepost { get; set; }
		public Nullable<long> usroblast { get; set; }
		public Nullable<long> usrregion { get; set; }
		public Nullable<long> usrsubregion { get; set; }
		public Nullable<long> usrvillage { get; set; }
		public Nullable<long> usrtypeapplicationid { get; set; }
		public Nullable<long> usrokedid { get; set; }
		public Nullable<int> usrfscode { get; set; }
		public string usridk { get; set; }

		public string OblastName { get; set; }
	}
	
    public class RST_TEMP_ReportReestr
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string BINIIN { get; set; }
        public string OwnerName { get; set; }
        public string OblastName { get; set; }
        public string IDK { get; set; }
        public long StatusId { get; set; }
        public string StatusName { get; set; }

        public long? UserId { get; set; }
        public long? ReasonId { get; set; }
        public long? Expectant { get; set; }

        public Nullable<long> Oblast { get; set; }

    }

}