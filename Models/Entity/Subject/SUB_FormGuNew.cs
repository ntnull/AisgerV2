using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Entity.Subject
{
    public class SUB_FormGuNew
    {
        public long Id { get; set; }
        public long? StatusId { get; set; }
        public long? UserId { get; set; }
        public int? ReportYear { get; set; }
        public long? Editor { get; set; }
        public SEC_User SEC_User1 { get; set; }
        public SEC_User SEC_User { get; set; }
        public int? BeginPlanYear { get; set; }
        public int? EndPlanYear { get; set; }
        public bool IsNotEvents { get; set; }
        public string DesignNote { get; set; }
        public System.DateTime CreateDate { get; set; }
        public List<SUB_FormComment> SUB_FormComment { get; set; }
        public string Note { get; set; }
        public bool IsPlan { get; set; }
        public bool? IsRent { get; set; }

        public bool? IsEnergyManagementSystem { get; set; }
        public List<SubFornMessage> SubFornMessages { get; set; }

        // форма 2
        public List<SUB_Form2Record> SubForm2Records { get; set; }

        // форма 3
        public List<SUB_Form3GuRecord> SUB_Form3GuRecords { get; set; }
        public List<SUB_Form4Record> SubForm4Records { get; set; }
        public List<SUB_Form4Record> SubForm4RecordsOther { get; set; }

        // форма 3а
        public List<SUB_Form5Record> SubForm5Records { get; set; }
        public List<SUB_Form5Record> SubForm5RecordsOther { get; set; }
        //----
        public List<SUB_Form3aGuRecord1> SUB_Form3aGuRecord1s { get; set; }
        public List<SUB_Form3aGuRecord1> SUB_Form3aGuRecord1sOther { get; set; }
        //----
        public List<SUB_Form3aGuRecord2> SUB_Form3aGuRecord2s { get; set; }
        public List<SUB_Form3aGuRecord2> SUB_Form3aGuRecord2sOther { get; set; }
        //----
        public List<SUB_Form3aGuRecord3> SUB_Form3aGuRecord3s { get; set; }
        public List<SUB_Form3aGuRecord3> SUB_Form3aGuRecord3sOther { get; set; }

        //----справочник
        public List<SUB_DIC_NormEnergy> SubDicNormEnergies { get; set; }
        public SUB_Form SignedSubForm { get; set; }

        public List<sub_dic_energyindicator> SubDicEnergyindicatorList { get; set; }
        
        //----
        public IList<string> AttachFiles { get; set; }
        public List<SUB_DIC_TypeResource> SubDicTypeResources { get; set; }
        public bool IsSigned { get; set; }
        public string PreviousUrl { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<System.DateTime> DesignDate { get; set; }
        public string DesignDateStr { get; set; }

        public ICollection<SUB_FormHistory> SUB_FormHistory { get; set; }

        public List<string> Wastes { get; set; }
        public MultiSelectList WastList { get; set; }
        public ICollection<SUB_FormKadastr> SUB_FormKadastr { get; set; }
        public List<SUB_FormKadastr> SubFormKadastrs { get; set; }
    }
}