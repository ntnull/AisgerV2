using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models
{
    public partial class EAUDIT_BuildingForm1
    {
        //public long RefDicUnit { get; set; }

        public List<DIC_Unit> DIC_Units { get; set; }

        public bool IsCommand { get; set; }

        public int InnerOrder { get; set; }

        public int RowSpan { get; set; }

        /// <summary>
        /// Дополнительное поле. Пока не используется
        /// </summary>
        public bool? IsAdditionalRow { get; set; }
    }
}