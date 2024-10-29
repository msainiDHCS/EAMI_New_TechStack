using System;
using System.Runtime.Serialization;

namespace OHC.EAMI.ServiceContract
{

    [DataContract]
    [KnownType(typeof(PaymentSubmissionRequest))]
    [KnownType(typeof(PaymentSubmissionResponse))]
    [KnownType(typeof(PaymentStatusInquiryRequest))]
    [KnownType(typeof(PaymentStatusInquiryResponse))]
    [KnownType(typeof(PaymentStatusResponse))]
    public class EAMITransaction
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string SenderID { get; set; }                

        [DataMember(IsRequired = true, Order = 1)]
        public string ReceiverID { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string TransactionID { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public TransactionType TransactionType { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public string TransactionVersion { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public DateTime TimeStamp { get; set; }


        [IgnoreDataMember]
        public int DBTransactionID { get; set; }

        [IgnoreDataMember]
        public string ActualSenderID { get; set; }

        [IgnoreDataMember]
        public TransactionType ActualTransactionType { get; set; }        
    }



    [DataContract]
    public enum TransactionType
    {
        [EnumMember]
        Unknown = 1,
        [EnumMember]
        PaymentSubmissionRequest = 2,
        [EnumMember]
        PaymentSubmissionResponse = 3,
        [EnumMember]
        StatusInquiryRequest = 4,
        [EnumMember]
        StatusInquiryResponse = 5,
        [EnumMember]
        RejectedPaymentInquiryRequest = 6,
        [EnumMember]
        RejectedPaymentInquiryResponse = 7
    }
}


