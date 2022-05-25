using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models
{
    public class EauditAttachment3
    {
        public EauditAttachment3()
        {
            
        }
        public EAUDIT_Preamble Preamble { get; set; }

        public EAUDIT_Preamble SignedEauditPreamble { get; set; }

        public List<EAUDIT_IndustryBuildingForm1> IndustryBuildingForm1Rows { get; set; }
        public bool IsReadOnly { get; set; }
        public List<EAUDIT_FieldComments> FieldComments { get; set; }
    }
}