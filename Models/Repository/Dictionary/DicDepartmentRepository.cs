using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Dictionary
{
    public class DicDepartmentRepository : AObjectRepository<DIC_Department>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.DIC_Department; }
        }
    }
}