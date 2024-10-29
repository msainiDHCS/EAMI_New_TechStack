using System;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{
    
    [DataContract]
    public class PingResponse
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string SenderID { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string ReceiverID { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public DateTime ServerTimeStamp { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public DateTime ClientTimeStamp { get; set; }

        [DataMember(IsRequired = false, Order = 4)]
        public string ResponseMessage { get; set; }
                                
    }
}
