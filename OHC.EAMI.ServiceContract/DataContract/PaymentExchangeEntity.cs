using System;
using System.Runtime.Serialization;
namespace OHC.EAMI.ServiceContract
{
    [DataContract]
    public class PaymentExchangeEntity
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string EntityID { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string EntityIDSuffix { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string EntityIDType { get; set; }
        
        [DataMember(IsRequired = true, Order = 3)]
        public string Name { get; set; }        

        [DataMember(IsRequired = true, Order = 4)]
        public string AddressLine1 { get; set; }

        [DataMember(IsRequired = false, Order = 5)]
        public string AddressLine2 { get; set; }

        [DataMember(IsRequired = false, Order = 6)]
        public string AddressLine3 { get; set; }

        [DataMember(IsRequired = true, Order = 7)]
        public string AddressCity { get; set; }

        [DataMember(IsRequired = true, Order = 8)]
        public string AddressState { get; set; }

        [DataMember(IsRequired = true, Order = 9)]
        public string AddressZip { get; set; }

        [DataMember(IsRequired = true, Order = 10)]
        public string EIN { get; set; }

        [DataMember(IsRequired = true, Order = 11)]
        public string VendorTypeCode { get; set; }
  
    }
}

