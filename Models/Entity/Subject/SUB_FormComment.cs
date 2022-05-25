using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Aisger.Models
{
    public partial class SUB_FormComment
    {
        public long OwnerId { get; set; }

        public List<SUB_FormRecordHistory> SubFormRecordHistories { get; set; }

    }

    public partial class SUB_FormComRecord
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

    public partial class SUB_FormRecordHistory
    {
        public string CreateDateStr
        {
            get
            {
                if (RegDate == null)
                {
                    return null;
                }
                return RegDate.Value.ToString("dd/MM/yyyy HH:mm");
            }
        }
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
    }
}