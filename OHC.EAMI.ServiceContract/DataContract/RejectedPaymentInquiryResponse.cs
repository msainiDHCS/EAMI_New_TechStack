using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{
    [DataContract]
    public class RejectedPaymentInquiryResponse : PaymentStatusResponse
    {
        [DataMember(IsRequired = false, Name = "PaymentRecordStatuList", Order = 8)]
        public List<PaymentRecordStatus> PaymentRecordStatuList { get; set; }
    }    
}
