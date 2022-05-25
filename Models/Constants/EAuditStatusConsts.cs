using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Constants
{
    public class EauditStatusConsts
    {
        public const string Created = "created";
        // отправлен
        public const string Sended = "sended";
        // предоставил
        public const string Provided = "provided";
        //Возвращен
        public const string Returned = "returned";
        // Одобрен
        public const string Approved = "approved";
        // на проверке
        public const string Checking = "checking";
        // соответствует
        public const string Match = "match";
        // не соответствует
        public const string NotMatch = "notmatch";
    }
}