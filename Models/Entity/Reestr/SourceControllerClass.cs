using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Reestr
{
	public class SourceControllerClass
	{
		public long rst_id { get; set; }
		public long user_id { get; set; }
		public string idk { get; set; }
        public string bin { get; set; }
		public string oked_name { get; set; }
		public string juridical_name { get; set; }
		public string oblast_name { get; set; }
		public string oblast_id { get; set; }
		public Nullable<int> fscode { get; set; }
		public string fscode_name { get; set; }
		public bool isexcluded { get; set; }
		public string excluded_name { get; set; }
		public Nullable<double> consumption { get; set; }

		public Nullable<long> expectant_id { get; set; }
		public string expectant_name { get; set; }
        public string isplan { get; set; }
        public string isem_system { get; set; }

        public Nullable<double> sum_expenceenergy { get; set; }
        public Nullable<double> sum_expenceenergy2
        {
          
            get { return (sum_expenceenergy != null) ? Math.Round(sum_expenceenergy.Value, 2) : 0; }
        }

        public Nullable<long> sub_form_id { get; set; }
		public Nullable<double> tut { get; set; }
		public Nullable<double> noS1 { get; set; }
		public Nullable<double> noS2 { get; set; }
		public Nullable<double> noS3 { get; set; }
		public Nullable<double> noS4 { get; set; }
		public Nullable<double> noS5 { get; set; }
		public Nullable<double> noS6 { get; set; }
		public Nullable<double> noS7 { get; set; }
		public Nullable<double> noS8 { get; set; }
		public Nullable<double> noS9 { get; set; }
		public Nullable<double> noS10 { get; set; }
		public Nullable<double> noS11 { get; set; }
		public Nullable<double> noS12 { get; set; }
		public Nullable<double> noS13 { get; set; }
		public Nullable<double> noS14 { get; set; }
		public Nullable<double> noS15 { get; set; }
		public Nullable<double> noS16 { get; set; }
		public Nullable<double> noS17 { get; set; }
		public Nullable<double> noS18 { get; set; }
		public Nullable<double> noS19 { get; set; }
		public Nullable<double> noS20 { get; set; }
		public Nullable<double> noS21 { get; set; }
		public Nullable<double> noS22 { get; set; }
		public Nullable<double> noS23 { get; set; }
		public Nullable<double> noS24 { get; set; }
		public Nullable<double> noS25 { get; set; }
		public Nullable<double> noS26 { get; set; }
		public Nullable<double> noS27 { get; set; }
		public Nullable<double> noS28 { get; set; }
        public Nullable<double> coV1 { get; set; }
        public Nullable<double> coV2 { get; set; }
        public Nullable<double> coV3 { get; set; }
        public Nullable<int> countOfEmployees { get; set; }
        public Nullable<int> countOfStudents { get; set; }
        public Nullable<int> countOfBeds { get; set; }

    }

    public class SourceControllerClassSum
    {     
        public Nullable<double> tut { get; set; }
        public Nullable<double> noS1 { get; set; }
        public Nullable<double> noS2 { get; set; }
        public Nullable<double> noS3 { get; set; }
        public Nullable<double> noS4 { get; set; }
        public Nullable<double> noS5 { get; set; }
        public Nullable<double> noS6 { get; set; }
        public Nullable<double> noS7 { get; set; }
        public Nullable<double> noS8 { get; set; }
        public Nullable<double> noS9 { get; set; }
        public Nullable<double> noS10 { get; set; }
        public Nullable<double> noS11 { get; set; }
        public Nullable<double> noS12 { get; set; }
        public Nullable<double> noS13 { get; set; }
        public Nullable<double> noS14 { get; set; }
        public Nullable<double> noS15 { get; set; }
        public Nullable<double> noS16 { get; set; }
        public Nullable<double> noS17 { get; set; }
        public Nullable<double> noS18 { get; set; }
        public Nullable<double> noS19 { get; set; }
        public Nullable<double> noS20 { get; set; }
        public Nullable<double> noS21 { get; set; }
        public Nullable<double> noS22 { get; set; }
        public Nullable<double> noS23 { get; set; }
        public Nullable<double> noS24 { get; set; }
        public Nullable<double> noS25 { get; set; }
        public Nullable<double> noS26 { get; set; }
        public Nullable<double> noS27 { get; set; }
        public Nullable<double> noS28 { get; set; }
        public Nullable<double> coV1 { get; set; }
        public Nullable<double> coV2 { get; set; }
        public Nullable<double> coV3 { get; set; }
        public Nullable<int> countOfEmployees { get; set; }
        public Nullable<int> countOfStudents { get; set; }
        public Nullable<int> countOfBeds { get; set; }

    }
}