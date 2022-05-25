using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger;
using Aisger.Models;
using Aisger.Models.Repository;

namespace FlowDoc.Models.Repository.Dictionary
{
    public class DicOrganisationRepository : AObjectRepository<DIC_Organization>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.Organisation; }
        }
    }
}