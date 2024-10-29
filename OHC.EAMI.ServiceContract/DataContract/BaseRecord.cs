using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{
           
    [DataContract]
    //[KnownType(typeof(PaymentRecord))]
    //[KnownType(typeof(PaymentRecordStatus))]
    //[KnownType(typeof(PaymentRecordStatusPlus))]
    public class BaseRecord
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string PaymentRecNumber { get; set; }

        [DataMember(IsRequired = false, Order = 1)]
        public Dictionary<string, string> GenericNameValueList { get; set; }


        

        [IgnoreDataMember]
        public bool IsValid { get; set; }

        [IgnoreDataMember]
        public string ValidationMessage { get; set; } 
        
    }
}
