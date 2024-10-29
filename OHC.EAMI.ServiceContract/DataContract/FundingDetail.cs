
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{
    [DataContract]
    public class FundingDetail
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string FundingSourceName { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string FFPAmount { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string SGFAmount { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public string FiscalYear { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public string FiscalQuarter { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public string Title { get; set; }

        [DataMember(IsRequired = false, Order = 6)]
        public Dictionary<string, string> GenericNameValueList { get; set; }
        
    }
}
