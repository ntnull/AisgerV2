using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Models.Constants;
using Aisger.Models.Repository.Security;

namespace Aisger.Helpers
{
    public static class EauditHelper
    {
        public static string GetUserKindCode(long userId)
        {
            string code = null;
            var userRepository = new SecUserRepository();
            var user = userRepository.GetQuery(u => u.Id == userId).FirstOrDefault();
            if (user != null)
            {
                if (user.DIC_Organization != null &&  user.DIC_Organization.Code == "01")
                {
                    code = user.SEC_Roles.Code;
                }
                else
                {
                    var userKind = user.SEC_UserKind.FirstOrDefault();
                    if (userKind != null)
                        code = userKind.DIC_KindUser.Code;    
                }
            }
            return code;
        } 
    }
}