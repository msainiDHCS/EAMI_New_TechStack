using System;
using System.Runtime.Serialization;
using OHC.EAMI.CommonEntity.Base;

namespace OHC.EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class FundingSource : BaseEntity
    {
        [DataMember]
        public int? Funding_Source_ID { get; set; }

        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }

        //[DataMember]
        //public string Title { get; set; }

        [DataMember]
        public int System_ID { get; set; }

        [DataMember]
        public string System_Code { get; set; }

        [DataMember]
        public string DeactivatedBy { get; set; }
        [DataMember]

        public DateTime? DeactivatedDate { get; set; }
        public string Status
        {
            get
            {
                return (base.IsActive ? "Active" : "Inactive");
            }
        }
    }
}
