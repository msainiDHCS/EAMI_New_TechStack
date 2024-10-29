using System.Collections.Generic;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{

    [DataContract]
    public class PaymentStatusInquiryResponse : PaymentStatusResponse
    {
        [DataMember(IsRequired = true, Name = "PaymentRecordStatuList", Order = 8)]
        public List<PaymentRecordStatusPlus> PaymentRecordStatuList { get; set; }
    }
    
}
