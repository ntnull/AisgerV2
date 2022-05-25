using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aisger.Models.Constants
{
    public class EAuditOperationConsts
    {
        [Display(Name = "Подписать")]
        public const string Sign = "Sign";

        [Display(Name = "Отправить")]
        public const string Send = "Send";

        [Display(Name = "Вернуть на доработку")]
        public const string Return = "Return";

        [Display(Name = "Одобрить")]
        public const string Approve = "Approve";

        [Display(Name = "Отправить на проверку")]
        public const string Checking = "Checking";

        [Display(Name = "Соотвествует")]
        public const string Match = "Match";

        [Display(Name = "Не соотвествует")]
        public const string NotMatch = "NotMatch";
    }
}