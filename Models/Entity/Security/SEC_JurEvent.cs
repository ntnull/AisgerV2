using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
//  [MetadataType(typeof(SecJurEventMetaData))]
    public partial class SEC_JurEvent : IObject
    {
        public IObject EntityObj { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class SecJurEventMetaData
    {
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}