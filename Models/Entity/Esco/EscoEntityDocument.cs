using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Esco
{
    public class EscoEntityDocument
    {
        public IEnumerable<EscoDicProduct> EscoDicProducts { get; set; }

        public string Biniin { get; set; }
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public double AllRecord { get; set; }

        public string FilesId { get; set; }

        public int AllCount { get; set; }
    }
    public class JsonModel
    {
        public string HTMLString { get; set; }
        public bool NoMoreData { get; set; }
    }

    public class EscoDicProduct : ESCO_DIC_Product
    {
        public string NameGroup { get; set; }

        public string ShortNote
        {
            get
            {
                if (!string.IsNullOrEmpty(Note) && Note.Length > 200)
                {
                    return Note.Substring(0, 200) + "....";
                }
                return Note;
            }
        }
        public string JuridicalName { get; set; }
        public string Address { get; set; }
        public string Oblast { get; set; }
        public string ContactInfo { get; set; }
    }
}