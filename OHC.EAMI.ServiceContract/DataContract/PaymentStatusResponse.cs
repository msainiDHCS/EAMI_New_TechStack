using System.Collections.Generic;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{

    [DataContract]
    [KnownType(typeof(PaymentSubmissionResponse))]
    [KnownType(typeof(PaymentStatusInquiryResponse))]
    public class PaymentStatusResponse : EAMITransaction
    {
        [DataMember(IsRequired = true, Order = 6)]
        public string ResponseStatusCode { get; set; }

        [DataMember(IsRequired = false, Order = 7)]
        public string ResponseMessage { get; set; }        
    }
}
