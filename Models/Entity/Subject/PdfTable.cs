using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Subject
{
    public class PdfTable
    {
        public int Id { get; set; }
        public long FormId { get; set; }
        public bool IsGenerated { get; set; }
        public int Year { get; set; }
        public string KatoCode { get; set; }
    }
}