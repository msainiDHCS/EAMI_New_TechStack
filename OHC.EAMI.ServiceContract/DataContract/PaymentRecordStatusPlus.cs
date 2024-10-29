using System;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{            
    [DataContract]
    public class PaymentRecordStatusPlus : PaymentRecordStatus
    {                        
        [DataMember(IsRequired = false, Order = 8)]
        public string ClaimScheduleNumber { get; set; }

        [DataMember(IsRequired = false, Order = 9)]
        public DateTime ClaimScheduleDate { get; set; }

        [DataMember(IsRequired = false, Order = 10)]
        public string WarrantNumber { get; set; }

        [DataMember(IsRequired = false, Order = 11)]
        public string WarrantAmount { get; set; }

        [DataMember(IsRequired = false, Order = 12)]
        public DateTime WarrantDate { get; set; }
      
    }
}
