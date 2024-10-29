using System.Runtime.Serialization;
using EAMI.CommonEntity;

namespace EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SCOProperty : BaseEntity
    {
        [DataMember]
        public int? SCO_Property_ID { get; set; }

        [DataMember]
        public int? Fund_ID { get; set; }

        [DataMember]
        public string Fund_Code { get; set; }

        [DataMember]
        public string SCO_Property_Name { get; set; }

        [DataMember]
        public string SCO_Property_Value { get; set; }

        [DataMember]
        public int SCO_Property_Type_ID { get; set; }
        [DataMember]
        public int SCO_Property_Enum_ID { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? System_ID { get; set; }

        public string System_Code { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int EnvironmentID { get; set; }
        [DataMember]
        public string EnvironmentText { get; set; }
        [DataMember]
        public int PaymentTypeID { get; set; }
        [DataMember]
        public string PaymentTypeText { get; set; }

        [DataMember]
        public List<SCOPropertyTypeLookUp> lstSCOPropertyTypeLookUp = new List<SCOPropertyTypeLookUp>();
        [DataMember]
        public List<SCOPropertyEnumsLookup> lstSCOPropertyEnumsLookUp = new List<SCOPropertyEnumsLookup>();
        [DataMember]
        public List<Fund> lstFunds = new List<Fund>();

        public SCOSetting scoSetting { get; set; }
        public SCOFileSetting SCOFileSetting { get; set; }
    }
}
