using System;
using System.Runtime.Serialization;
using OHC.EAMI.CommonEntity.Base;

namespace OHC.EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Fund : BaseEntity
    {
        [DataMember]
        public int? Fund_ID { get; set; }

        [DataMember]
        public string Fund_Code { get; set; }
        [DataMember]
        public string Fund_Name { get; set; }

        [DataMember]
        public string Fund_Description { get; set; }

        [DataMember]
        public string Stat_Year { get; set; }

        [DataMember]
        public int System_ID { get; set; }

        [DataMember]
        public string System_Code { get; set; }
       
        public string Status
        {
            //get
            //{
            //    return (base.IsActive ? "Active" : "Inactive");
            //}
            get; set;
        }
    }
}
