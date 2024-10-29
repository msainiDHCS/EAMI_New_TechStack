using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{
    [DataContract]
    public class RejectedPaymentInquiryRequest : EAMITransaction
    {
        [DataMember(IsRequired = true, Order = 6)]
        public DateTime RejectedDateFrom { get; set; }

        [DataMember(IsRequired = true, Order = 7)]
        public DateTime RejectedDateTo { get; set; }    
    }
}
