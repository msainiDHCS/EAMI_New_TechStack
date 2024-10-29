using System;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{
        
    [DataContract]
    public class PingRequest
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string SenderID { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string ReceiverID { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public DateTime ClientTimeStamp { get; set; }


        [IgnoreDataMember]
        public string ASenderID { get; set; }
    }
}
