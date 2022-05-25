using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Dictionary
{
    public class KatoRepository : SqlRepository
    {
        public DIC_Kato GetById(long? id)
        {
            return AppContext.DIC_Kato.FirstOrDefault(e => e.Id == id);
        }

        public List<DIC_Kato> GetKatos(long? refParent, bool mandatory)
        {
            if (refParent == null)
            {
                return new List<DIC_Kato>();
            }
            var parentId = refParent.Value;
            var list = AppContext.DIC_Kato.Where(e => e.refParent == parentId).OrderBy(e=> new {e.type,e.NameRu}).ToList();
            if (!mandatory)
            {
                list.Insert(0, new DIC_Kato { Id = 0, NameRu = "" });
            }
            return list;
        }

		public string GetKatoByCuture(ref List<DicObjectClass> list, long? refParent, string lang)
		{
			string ErrorMessage = "";
			try
			{
				string query = "select \"Id\" , case '" + lang + "' when 'kz' then \"NameKz\" else \"NameRu\" end as \"Name\"   from \"DIC_Kato\"  ";
				if (refParent != null)
				{
					query = query + "  where \"refParent\"=" + refParent.Value;
				}

				var rows = AppContext.Database.SqlQuery<DicObjectClass>(query).ToList();
				if (rows != null)
					list = rows;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		
    }
}