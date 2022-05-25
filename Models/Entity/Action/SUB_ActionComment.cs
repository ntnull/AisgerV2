using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Utils;

namespace Aisger.Models
{
    public partial class SUB_ActionComment
    {
        public long OwnerId { get; set; }

    }
    public partial class SUB_ActionComRecord
    {
        public string OwnerName
        {
            get
            {
                if (SEC_User == null)
                {
                    return null;
                }
                return SEC_User.FullName;
            }
        }
        public string CreateDateStr
        {
            get
            {
                if (CreateDate == null)
                {
                    return null;
                }
                return CreateDate.Value.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }

    public partial class SUB_ActionTab1
    {
        public string BeginDateStr
        {
            get { return DateHelper.GetDate(BeginDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    BeginDate = dateTemp.Value;
                }
            }
        }

        public string EndDateStr
        {
            get { return DateHelper.GetDate(EndDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    EndDate = dateTemp.Value;
                }
            }
        }
    }
}