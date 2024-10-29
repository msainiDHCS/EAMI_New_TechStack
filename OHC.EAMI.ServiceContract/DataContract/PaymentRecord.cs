using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{
        
    [DataContract]
    public class PaymentRecord : BaseRecord
    {

        [DataMember(IsRequired = true, Order = 2)]
        public string PaymentRecNumberExt { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public string PaymentType { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public DateTime PaymentDate { get; set; }

        [DataMember(IsRequired = true, Order = 5)]                       
        public string Amount  { get; set; }

        [DataMember(IsRequired = true, Order = 6)]    
        public string FiscalYear  { get; set; }        

        [DataMember(IsRequired = true, Order = 7)]
        public PaymentExchangeEntity PayeeInfo { get; set; }

        [DataMember(IsRequired = true, Order = 8)]
        public string IndexCode { get; set; }

        [DataMember(IsRequired = true, Order = 9)]
        public string ObjectDetailCode { get; set; }

        [DataMember(IsRequired = false, Order = 10)]
        public string ObjectAgencyCode { get; set; }

        [DataMember(IsRequired = true, Order = 11)]
        public string PCACode { get; set; }

        [DataMember(IsRequired = true, Order = 12)]
        public string ApprovedBy { get; set; }

        [DataMember(IsRequired = true, Order = 13)]
        public string PaymentSetNumber { get; set; }

        [DataMember(IsRequired = true, Order = 14)]
        public string PaymentSetNumberExt { get; set; }

        [DataMember(IsRequired = false, Order = 15)]        
        public byte[] Attachment { get; set; }

        [DataMember(IsRequired = true, Name = "FundingDetailList", Order = 16)]
        public List<FundingDetail> FundingDetailList { get; set; }     
                
    }
}
