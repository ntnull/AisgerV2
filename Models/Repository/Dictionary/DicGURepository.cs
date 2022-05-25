using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Dictionary
{
    public class DicGURepository: SqlRepository
    {
        public List<DIC_GU> GetDicGuList()
        {
            return AppContext.DIC_GU.Where(e => e.IsDeleted == false).OrderBy(x => x.Id).ToList();
        }

        public string SaveDicGu(DIC_GU model)
        {
            string ErrorMessage = string.Empty;
            try
            {
                AppContext.DIC_GU.Add(model);
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return ErrorMessage;
        }

        public DIC_GU GetById(int id)
        {
            var row = AppContext.DIC_GU.FirstOrDefault(x=>x.Id==id);
            return row;
        }

        public string EditDicGu(DIC_GU model)
        {
            string ErrorMessage = string.Empty;
            try
            {
                var row = AppContext.DIC_GU.FirstOrDefault(x => x.Id == model.Id);
                row.Code = model.Code;
                row.NameKz = model.NameKz;
                row.NameRu = model.NameRu;
                row.IsDeleted = model.IsDeleted;
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return ErrorMessage;
        }

        public void DeleteById(int id)
        {
            var row = AppContext.DIC_GU.FirstOrDefault(x => x.Id == id);
            AppContext.DIC_GU.Remove(row);
            AppContext.SaveChanges();
        }
    }
}