using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Subject
{
    public class SubCommentsEntity
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateDateStr { get; set; }
        public bool? IsError { get; set; }
        public string TableName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }       
        public int? RowIndex { get; set; }
        public int? ColumnIndex { get; set; }

        public string FIO
        {
            get
            {
                return ((!string.IsNullOrWhiteSpace(LastName)) ? LastName + " " : "")+ ((!string.IsNullOrWhiteSpace(FirstName)) ? FirstName + " " : "") + ((!string.IsNullOrWhiteSpace(SecondName)) ? SecondName : "");
            }
        }
    }
}