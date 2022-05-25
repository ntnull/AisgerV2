using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Aisger.Models.Entity.Esco;
using Newtonsoft.Json.Schema;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Aisger.Models.Repository.Dictionary
{
    public class EscoDicProductKindRepository : AObjectRepository<ESCO_DIC_ProductKind>
    {
        public IEnumerable<ESCO_DIC_ProductKind> GetList(long? getCurrentUserId)
        {
            return GetCollectionList().Where(e => e.UserId == getCurrentUserId);
        }

        public ESCO_DIC_Product GetProductById(long id)
        {
            return AppContext.ESCO_DIC_Product.FirstOrDefault(e => e.Id == id);
        }
        public int GetAllCont(EscoEntityDocument model)
        {
            if (string.IsNullOrEmpty(model.Biniin))
            {
                return AppContext.ESCO_DIC_Product.Count(e => !e.IsDeleted);
            }

            return   AppContext.ESCO_DIC_Product.Count(
                        e =>
                            !e.IsDeleted &&
                            (e.NameProduct.Contains(model.Biniin) || e.Note.Contains(model.Biniin) ||
                             e.ESCO_DIC_ProductKind.NameGroup.Contains(model.Biniin) || e.ESCO_DIC_ProductKind.Note.Contains(model.Biniin)));
           
        }
        public SubUpdateField UpdateProductKind(long modelId, long userd, string fieldName, string fieldValue)
        {
            var entity = new SubUpdateField();
            var model = AppContext.ESCO_DIC_ProductKind.FirstOrDefault(e => e.Id == modelId);
            if (model == null)
            {
                model = new ESCO_DIC_ProductKind();
                model.UserId = userd;
                model.CreateDate = DateTime.Now;
                AppContext.ESCO_DIC_ProductKind.Add(model);
                AppContext.SaveChanges();
            }
            model.EditDate = DateTime.Now;
            switch (fieldName)
            {

                case "NameGroup":
                    {
                        model.NameGroup = fieldValue;
                        break;
                    }

                case "Note":
                    {
                        model.Note = fieldValue;
                        break;
                    }
               
                case "Wastes":
                    {
                        var okeds = fieldValue.Split(',');
                        var aquticOblasts = AppContext.Set<ESCO_DIC_ProductKindOked>().Where(e => e.KindId == model.Id);
                        var oblastId = new List<long>();
                        foreach (var country in okeds)
                        {
                            var idolbast = Convert.ToInt64(country);
                            if (aquticOblasts.SingleOrDefault(e => e.OkedId == idolbast) == null)
                            {
                                var way = new ESCO_DIC_ProductKindOked
                                {
                                    KindId = model.Id,
                                    OkedId = idolbast
                                };
                                AppContext.Set<ESCO_DIC_ProductKindOked>().Add(way);
                            }
                            else
                            {
                                oblastId.Add(idolbast);
                            }
                        }
                        var listdelete = aquticOblasts.Where(e => !oblastId.Contains(e.OkedId.Value));
                        foreach (var crRoutesAquticOblast in listdelete)
                        {
                            AppContext.Set<ESCO_DIC_ProductKindOked>().Remove(crRoutesAquticOblast);
                        }
                        break;
                    }
            }

            AppContext.SaveChanges();
            entity.ModelId = model.Id;
            return entity;
        }

        public SubUpdateField UpdateRecord(long modelId, long recordId, string fieldName, string fieldValue)
        {
            var entity = new SubUpdateField();
            var model = AppContext.ESCO_DIC_ProductKind.FirstOrDefault(e => e.Id == modelId);
            if (model == null)
            {
                return entity;
            }
            model.EditDate = DateTime.Now;
            var record = AppContext.ESCO_DIC_Product.FirstOrDefault(e=>e.Id==recordId);

            if (record == null)
            {
                record = new ESCO_DIC_Product();
                record.CreateDate = DateTime.Now;
                record.KindId = modelId;
            }
            else
            {
                record.EditDate = DateTime.Now;
            }
            
            switch (fieldName)
            {

                case "NameProduct":
                    {
                        record.NameProduct = fieldValue;
                        break;
                    }

                case "Note":
                    {
                        record.Note = fieldValue;
                        break;
                    }
            }
            if (record.Id == 0)
            {
                AppContext.ESCO_DIC_Product.Add(record);
            }

            AppContext.SaveChanges();
            entity.ModelId = model.Id;
            entity.RecordId = record.Id;
            return entity;
        }

        public ESCO_DIC_Product UpdatePostProduct(ESCO_DIC_Product product)
        {
            if (!string.IsNullOrEmpty(product.urlSite) &&!product.urlSite.Contains("http") && !product.urlSite.Contains("https"))
            {
                product.urlSite = @"http://"+product.urlSite;
            }
            if (product.Id == 0)
            {
                product.CreateDate = DateTime.Now;
                AppContext.ESCO_DIC_Product.Add(product);
            }
            else
            {
                var entity = AppContext.ESCO_DIC_Product.FirstOrDefault(e => e.Id == product.Id);
                if (entity != null)
                {
                    entity.NameProduct = product.NameProduct;
                    entity.Note = product.Note;
                    entity.urlSite = product.urlSite;
                    entity.EditDate = DateTime.Now;
                }
            }
            AppContext.SaveChanges();
            return product;
        }

        public void DeleteProduct(long id)
        {
            var entity = AppContext.ESCO_DIC_Product.FirstOrDefault(e => e.Id == id);
            if (entity == null) return;
            AppContext.ESCO_DIC_Product.Remove(entity);
            AppContext.SaveChanges();
        }
        public IEnumerable<EscoDicProduct> GetReuslt(EscoEntityDocument model, int page = 1, int pageSize = 20, double? totalPage = null)
        {
            var order = new StringBuilder(" order by o.\"Id\" ");


            var builder = new StringBuilder();
            builder.Append(" o.\"IsDeleted\" ='false' ");
           
            if (!string.IsNullOrEmpty(model.Biniin))
            {
                builder.Append("and (LOWER(\"NameProduct\") like LOWER(N'%" + model.Biniin + "%')");
                builder.Append(" or LOWER(o.\"Note\") like LOWER(N'%" + model.Biniin + "%'))");
            }
            if (builder.Length > 0)
            {
                builder.Insert(0, " WHERE ");
            }
            var start = page * pageSize + 1;

            var where = "WITH ordered_orders AS ( " +
                       " SELECT O.*,r.\"NameGroup\" as \"NameGroup\"," +
                       "(case when u.\"JuridicalName\" is null then u.\"LastName\" ELSE u.\"JuridicalName\" END)  as \"JuridicalName\","+  
"u.\"Address\" as \"Address\",k.\"NameRu\" as \"Oblast\","+
"('эл. адрес:' || u.\"Email\" || ', моб. тел:' || u.\"Mobile\" || ', раб. тел:' || u.\"WorkPhone\") as ContactInfo,"+
                       "row_number() OVER(" + order +
                       ") AS row_num " +
                       "FROM \"ESCO_DIC_Product\" O left join \"ESCO_DIC_ProductKind\" as r on r.\"Id\"=o.\"KindId\" " +
                       "left join \"SEC_User\" as u on r.\"UserId\"=u.\"Id\" "+
                       " left join \"DIC_Kato\" as k on u.\"Oblast\"=k.\"Id\" "+
                        builder +
                       ")SELECT * FROM ordered_orders WHERE row_num BETWEEN " + start + " AND " + (start + pageSize - 1);

            return AppContext.Database.SqlQuery<EscoDicProduct>(where);

        }
    }
}