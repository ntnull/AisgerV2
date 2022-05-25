using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    public partial class EAUDIT_FieldComments : IEntity
    {
        
    }

    public class EauditFieldCommentViewModel
    {
        public EauditFieldCommentViewModel()
        {
            FieldComment = new EAUDIT_FieldComments();
            FieldCommentHistory = new List<EAUDIT_FieldComments>();
        }

        public EAUDIT_FieldComments FieldComment { get; set; }
        public List<EAUDIT_FieldComments> FieldCommentHistory { get; set; }
    }
}