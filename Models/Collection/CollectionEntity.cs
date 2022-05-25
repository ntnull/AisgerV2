using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{

    public class CollectionEntity
    {
    }
    public partial class SEC_UserOked : IEntity
    {
    }
    public partial class SUB_Form2Record : IEntity
    {
    }

    public partial class SEC_RolePermission : IEntity
    {
    }

    public partial class SEC_RightPermission : IEntity
    {
    }
    public partial class SUB_Form4Record : IEntity
    {
        public string EmplPeriodStr
        {
            get { return EmplPeriod != null ? EmplPeriod.Value.ToString("MM/yyyy", CultureInfo.InvariantCulture) : null; }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    EmplPeriod = dateTemp.Value;
                }
            }
        }
    }
    public partial class SEC_UserOked : IEntity
    {
    }
    public partial class MAP_ApplicationEvent : IEntity
    {
    }
    public partial class MAP_ApplicationProduct : IEntity
    {
    }
    public partial class MAP_ApplicationHistory : IEntity
    {
    }
    
}