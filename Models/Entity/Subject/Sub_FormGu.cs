using Aisger.Models.Repository;
using Aisger.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Entity.Subject
{
    public class Sub_FormGu
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
        public List<SUB_Form2RecordGu> SUB_Form2RecordGu { get; set; }
        public List<SUB_Form3RecordGu> SUB_Form3RecordGu { get; set; }

        
        public SUB_Form2Gu SUB_Form2Gu { get; set; }
        public SUB_Form3Gu SUB_Form3Gu { get; set; }

        //----справочник
        public List<SUB_DIC_NormEnergy> SubDicNormEnergies { get; set; }
        public SUB_Form SignedSubForm { get; set; }

        public List<sub_dic_energyindicator> SubDicEnergyindicatorList { get; set; }
        public List<SUB_Form4RecordGu> SubForm4RecordGuList { get; set; }
        
        public List<SUB_Form4Record> SubForm4Records { get; set; }
        public List<SUB_Form4Record> SubForm4RecordsOther { get; set; }
        public List<SUB_Form5Record> SubForm5Records { get; set; }
        public List<SUB_Form5Record> SubForm5RecordsOther { get; set; }
        public List<SUB_Form6Record> SubForm6Records { get; set; }
        public List<SUB_FormGuLightingInfo> SUB_FormGuLightingInfo { get; set; }

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
    }

    public class SUB_Form2RecordGu
    {
        //SUB_DIC_TypeResource
        public long Form2RecordId { get; set; }
        public long Form6RecordId { get; set; }
        public Nullable<long> FormId { get; set; }
        public long TypeResourceId { get; set; }
        public string TypeResourceName { get; set; }
        public string TypeResourceUnitName { get; set; }
        public Nullable<double> NotOwnSource { get; set; }
        public Nullable<double> ExpenceEnergy { get; set; }
        public Nullable<int> CountDevice { get; set; }
        public Nullable<float> Equipment { get; set; }
        public virtual SUB_DIC_TypeResource SUB_DIC_TypeResource { get; set; }
    }

    public class SUB_Form3RecordGu
    {
        public long Form3RecordId { get; set; }
        public long Form6RecordId { get; set; }
        public Nullable<long> FormId { get; set; }
        public Nullable<long> KindResourceId { get; set; }
        public string KindResourceName { get; set; }
        public string KindResourceUnitName { get; set; }
        public Nullable<double> ConsumptionVolume { get; set; }
        public Nullable<double> LosTransportVolume { get; set; }
        public string Note { get; set; }
        public Nullable<double> ConsumptionPrice { get; set; }
        public Nullable<double> LosTransportPrice { get; set; }

        
        public Nullable<int> CountDevice { get; set; }
        public Nullable<float> Equipment { get; set; }
    }

    public class SubForm5RecordsGu
    {
        public long Id { get; set; }
        public Nullable<long> FormId { get; set; }
        public string IndicatorName { get; set; }
        public string RegularStandart { get; set; }
        public string UnitMeasure { get; set; }
        public string CalcFormula { get; set; }
        public Nullable<float> EnergyValue { get; set; }
        public string Note { get; set; }
        public Nullable<long> NormEnergyId { get; set; }
        public Nullable<int> KindIndex { get; set; }
        public Nullable<long> energyindicator_id { get; set; }

        public virtual sub_dic_energyindicator sub_dic_energyindicator { get; set; }
    }

    public class SUB_Form4RecordGu
    {
        public long Id { get; set; }
        public Nullable<long> FormId { get; set; }
        public string EventName { get; set; }
        public Nullable<System.DateTime> EmplPeriod { get; set; }
        public Nullable<double> ActualInvest { get; set; }
        public Nullable<long> TypeResourceId { get; set; }
        public Nullable<double> InKind { get; set; }
        public Nullable<double> InMoney { get; set; }
        public string Note { get; set; }
        public Nullable<long> TypeCounterId { get; set; }
        public string PlanExpend { get; set; }
        public Nullable<long> EventId { get; set; }
        public Nullable<int> KindIndex { get; set; }

        public string EmplPeriodStr
        {
            get { return EmplPeriod != null ? EmplPeriod.Value.ToString("MM/yyyy", CultureInfo.InvariantCulture) : null; }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    EmplPeriod = dateTemp.Value;
                }
            }
        }

        public virtual SUB_DIC_Event SUB_DIC_Event { get; set; }
        public virtual SUB_DIC_TypeCounter SUB_DIC_TypeCounter { get; set; }
        public virtual SUB_DIC_TypeResource SUB_DIC_TypeResource { get; set; }
        public virtual SUB_Form SUB_Form { get; set; }
    }
}