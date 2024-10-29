using System.Collections.Generic;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{

    [DataContract]
    public class PaymentStatusInquiryRequest : EAMITransaction
    {
        [DataMember(IsRequired = true, Order = 6)]
        public string PaymentRecordCount { get; set; }

        [DataMember(IsRequired = true, Name = "PaymentRecordList", Order = 7)]
        public List<BaseRecord> PaymentRecordList { get; set; }

    }
}
