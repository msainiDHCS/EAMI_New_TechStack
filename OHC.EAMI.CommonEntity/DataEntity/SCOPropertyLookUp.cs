using System.Collections.Generic;
using System.Runtime.Serialization;
using OHC.EAMI.CommonEntity.Base;

namespace OHC.EAMI.CommonEntity.DataEntity
{
    //public class SCOPropertiesLookUp
    //{
    //    public List<SCOPropertyTypeLookUp> lstSCOPropertyTypeLookUp = new List<SCOPropertyTypeLookUp>();
    //    public List<SCOPropertyEnumsLookup> lstSCOPropertyEnumsLookUp = new List<SCOPropertyEnumsLookup>();
    //}

    public class SCOPropertyEnumsLookup : BaseEntity
    {
        [DataMember]
        public int? SCO_Property_Enum_ID { get; set; }
        [DataMember]
        public int? SCO_Property_Type_ID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Status { get; set; }
    }
    public class SCOPropertyTypeLookUp : BaseEntity
    {
        [DataMember]
        public int? SCO_Property_Type_ID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Status { get; set; }
    }
}
