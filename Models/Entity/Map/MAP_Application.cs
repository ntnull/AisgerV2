using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Aisger.Helpers;
using Aisger.Models.Entity;
using Aisger.Utils;
using Aisger.Validation;
using NPOI.SS.Formula.Functions;

namespace Aisger.Models
{
      [MetadataType(typeof(MapApplicationMetaData))]
    public partial class MAP_Application : IObject
    {
          [Display(Name = "Expied", ResourceType = typeof(ResourceSetting))]
          public virtual string Expied
          {
              get
              {
                  if (Deadline == null || SendDate == null)
                  {
                      return null;
                  }
                  var endDate = FinishDate != null ? FinishDate.Value : DateTime.Now;
                  var diff1 = Deadline.Value.Subtract(endDate);
                  if (diff1.Days > 0)
                  {
                      var d = diff1.Days + 1;
                      return d.ToString(CultureInfo.InvariantCulture);
                  }
                  return ResourceSetting.Expired;
              }
          }
          public virtual int ExpiedSign
          {
              get
              {
                  if (Deadline == null || SendDate == null)
                  {
                      return 0;
                  }
                  if (StatusId == 1 || StatusId == 4 || StatusId == 5 || StatusId == 6 || StatusId == 8)
                  {
                      return 0;
                  }
                  var endDate = FinishDate != null ? FinishDate.Value : DateTime.Now;
                  var diff1 = Deadline.Value.Subtract(endDate);
                  if (diff1.Days > 3)
                  {
                      return 0;
                  }
                  if (diff1.Days < 3 && diff1.Days>0)
                  {
                      return 1;
                  }
                  if (diff1.Days < 0)
                  {
                      return 2;
                  }
                  return 0;
              }
          }
          public bool IsCopy { get; set; }
          public bool IsSaveSend { get; set; }

          public string SubjectName
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.ApplicationName;
                }
                return null;
            }
        }

        public string SubjectOblast
        {
            get
            {
                if (SEC_User1 != null && SEC_User1.DIC_Kato != null)
                {
                    return SEC_User1.DIC_Kato.NameRu;
                }
                return null;
            }
        }

        public string SubjectBin
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.BINIIN;
                }
                return null;
            }
        }

          public bool IsCollectionEdit { get; set; }

          public string SubjectAddress
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.Address;
                }
                return null;
            }
        }
        public string SubjectBoss
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.FullName;
                }
                return null;
            }
        }
        public string SubjectPost
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.Post;
                }
                return null;
            }
        }

        public string Executor
        {
            get
            {
                if (SEC_User != null)
                {
                    return SEC_User.FullName;
                }
                return null;
            }
        }
      
        public string StatusName
        {
            get
            {
                if (MAP_DIC_Status != null)
                {
                    if (CultureHelper.GetCurrentCulture() == CultureHelper.FieldKz)
                        return MAP_DIC_Status.NameKz;
                    return MAP_DIC_Status.NameRu;
                }
                return null;
            }
        }
        [Display(Name = "RegDate", ResourceType = typeof(ResourceSetting))]
        public string DesignDateStr
        {
            get { return DateHelper.GetDate(DesignDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    DesignDate = dateTemp.Value;
                }
            }
        }
        [Display(Name = "SendDate", ResourceType = typeof(ResourceSetting))]
        public string SendDateStr
        {
            get { return DateHelper.GetDate(SendDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    SendDate = dateTemp.Value;
                }
            }
        }
        [Display(Name = "FinishDate", ResourceType = typeof(ResourceSetting))]
        public string FinishDateStr
        {
            get { return DateHelper.GetDate(FinishDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    FinishDate = dateTemp.Value;
                }
            }
        }
        [Display(Name = "Deadline", ResourceType = typeof(ResourceSetting))]
        public string DeadlineStr
        {
            get { return DateHelper.GetDate(Deadline); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    Deadline = dateTemp.Value;
                }
            }
        }
        public IList<MAP_ApplicationEvent> MapApplicationEvents { get; set; }
        public IList<MAP_ApplicationProduct> MapApplicationProducts { get; set; }
        public IList<MAP_ApplicationProduct> ProjectPowers { get; set; }
        public IList<MAP_ApplicationProduct> InKinds { get; set; }
        public IList<MAP_ApplicationProduct> InValueTerms { get; set; }
        public IList<string> AttachFiles { get; set; }
        public IList<string> DesignAttachFiles { get; set; }

        public string TempPathFile { get; set; }

        public string OkedString { get; set; }

        public bool IsBlocked { get; set; }

         /* public string TotalCostStr
          {
              get
              {
                  if (TotalCost == null)
                  {
                      return null;
                  }
                  return TotalCost.ToString();
              }
              set
              {
                  if (value == null)
                  {
                      TotalCost = null;
                  }
                  else
                  {
                      TotalCost = value.Replace(' ', '');
                  }

              }
          }*/
    }

    public class MapApplicationMetaData
    {
        [Display(Name = "AppNumber", ResourceType = typeof(ResourceSetting))]
        public string AppNumber { get; set; }


        [Display(Name = "ProjectName", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string ProjectName { get; set; }

        [Display(Name = "ProjectObjective", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string ProjectObjective { get; set; }

        [Display(Name = "ProjectLocation", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string ProjectLocation { get; set; }

        [Display(Name = "EstimatedPeriod", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string EstimatedPeriod { get; set; }

        [Display(Name = "ExpectedResult", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string ExpectedResult { get; set; }

        [Display(Name = "CurrentState", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string CurrentState { get; set; }

        [Display(Name = "TotalCost", ResourceType = typeof(ResourceSetting))]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "SetNumber")]
        [ValidDecimal(ErrorMessage = "Укажите число")]
        public Nullable<double> TotalCost { get; set; }

       [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "RstDicStatus", ResourceType = typeof(ResourceSetting))]
        public Nullable<long> StatusId { get; set; }

    }
}