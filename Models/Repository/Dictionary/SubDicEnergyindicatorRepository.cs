using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Dictionary
{
    public class SubDicEnergyindicatorRepository: SqlRepository
    {
        public List<sub_dic_energyindicator> GetSubDicEnergyindicatorList()
        {          
            return AppContext.sub_dic_energyindicator.Where(e => e.isdeleted == false).OrderBy(x => x.id).ToList();
        }

        public sub_dic_energyindicator GetSubDicEnergyindicatorById(int id)
        {
            return AppContext.sub_dic_energyindicator.FirstOrDefault(e => e.id == id);
        }

        public string SaveSubDicEnergyindicator(sub_dic_energyindicator model)
        {
            string errorMessage = "";
            try
            {
                if (model.id != null && model.id != 0)
                {
                    var entity = AppContext.sub_dic_energyindicator.FirstOrDefault(x => x.id == model.id);
                    entity.namekz = model.namekz;
                    entity.nameru = model.nameru;
                    entity.unitnamekz = model.unitnamekz;
                    entity.unitnameru = model.unitnameru;
                    entity.forgu = model.forgu;
                }
                else
                {
                    if (model.forgu == null)
                        model.forgu = false;
                    AppContext.sub_dic_energyindicator.Add(model);
                }

                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        public string DeleteSubDicEnergyindicatorById(int id)
        {
            string errorMessage = "";
            try
            {
                var row = AppContext.sub_dic_energyindicator.FirstOrDefault(x=>x.id==id);
                AppContext.sub_dic_energyindicator.Remove(row);
                AppContext.SaveChanges();
            }catch(Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }
    }
}