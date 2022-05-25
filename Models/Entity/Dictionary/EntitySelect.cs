using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Dictionary
{
    public class EntitySelect
    {
        public EntitySelect()
        {
        }


        public EntitySelect(long id, string nameRu)
        {
            Id = id;
            NameRu = nameRu;
        }

        public long Id { get; set; }
        public string NameRu { get; set; }
    }

    public class TermSearch
    {
        public long Id { get; set; }
        public string Term { get; set; }
    }
}