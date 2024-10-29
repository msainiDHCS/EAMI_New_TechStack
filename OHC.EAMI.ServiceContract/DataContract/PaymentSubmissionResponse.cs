using System.Collections.Generic;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{

    [DataContract]
    public class PaymentSubmissionResponse : PaymentStatusResponse
    {
        [DataMember(IsRequired = true, Name = "PaymentRecordStatuList", Order = 8)]
        public List<PaymentRecordStatus> PaymentRecordStatuList { get; set; }
    }
}
