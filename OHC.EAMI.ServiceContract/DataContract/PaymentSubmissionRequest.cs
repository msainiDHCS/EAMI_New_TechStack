using System.Collections.Generic;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{
    [DataContract]
    public class PaymentSubmissionRequest : EAMITransaction
    {
        [DataMember(IsRequired = true, Order = 6)]
        public string PaymentRecordCount { get; set; }
                        
        [DataMember(IsRequired = true, Order = 7)]
        public string PaymentRecordTotalAmount { get; set; }

        [DataMember(IsRequired = false, Order = 8)]
        public PaymentExchangeEntity PayerInfo { get; set; }

        [DataMember(IsRequired = true, Name = "PaymentRecordList", Order = 9)]
        public virtual List<PaymentRecord> PaymentRecordList { get; set; }        
    }
}
