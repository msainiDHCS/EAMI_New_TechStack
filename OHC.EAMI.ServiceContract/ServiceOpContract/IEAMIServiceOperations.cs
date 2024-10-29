using System.ServiceModel;
namespace OHC.EAMI.ServiceContract
{      
    [ServiceContract(Namespace = "http://dhcsintranet/EAMI/", ConfigurationName="IEAMIServiceOperations")]
    public interface IEAMIServiceOperations
    {       
        [OperationContract]
        PaymentSubmissionResponse EAMIPaymentSubmission(PaymentSubmissionRequest request);

        [OperationContract]
        PaymentStatusInquiryResponse EAMIPaymentStatusInquiry(PaymentStatusInquiryRequest request);

        [OperationContract]
        RejectedPaymentInquiryResponse EAMIRejectedPaymentInquiry(RejectedPaymentInquiryRequest request);

        [OperationContract]
        PingResponse Ping(PingRequest request);
    }
}
