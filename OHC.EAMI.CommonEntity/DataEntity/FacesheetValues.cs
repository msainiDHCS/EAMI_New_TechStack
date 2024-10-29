using System;
using System.Runtime.Serialization;
using OHC.EAMI.CommonEntity.Base;

namespace OHC.EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class FacesheetValues : BaseEntity
    {
        [DataMember]
        public int? Facesheet_ID { get; set; }

        [DataMember]
        public int? Fund_ID { get; set; }

        [DataMember]
        public string Fund_Code { get; set; }

        [DataMember]
        public string Fund_Name { get; set; }

        [DataMember]
        public string Chapter { get; set; }

        [DataMember]
        public string Reference_Item { get; set; }
        
        [DataMember]
        public string Program { get; set; }

        [DataMember]
        public int? System_ID { get; set; }

        [DataMember]
        public string System_Code {  get; set; }

        [DataMember]
        public string Agency_Number { get; set; }

        [DataMember]
        public string Agency_Name { get; set; }

        [DataMember]
        public string Fiscal_Year {  get; set; }

        [DataMember]
        public string Element { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Stat_Year { get; set; }

        public string Status
        {
            get
            {
                return (base.IsActive ? "Active" : "Inactive");
            }
        }
    }
}
