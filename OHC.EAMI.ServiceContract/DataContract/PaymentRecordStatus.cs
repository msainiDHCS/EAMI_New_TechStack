using System;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{            
    [DataContract]
    //[KnownType(typeof(PaymentRecordStatusPlus))]
    public class PaymentRecordStatus : BaseRecord
    {
        [DataMember(IsRequired = true, Order = 2)]
        public string PaymentRecNumberExt { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public string PaymentSetNumber { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public string PaymentSetNumberExt { get; set; }
      
        [DataMember(IsRequired = true, Order = 5)]
        public string StatusCode { get; set; }

        [DataMember(IsRequired = true, Order = 6)]
        public DateTime StatusDate { get; set; }

        [DataMember(IsRequired = false, Order = 7)]
        public string StatusNote { get; set; }

        


      
    }
}
