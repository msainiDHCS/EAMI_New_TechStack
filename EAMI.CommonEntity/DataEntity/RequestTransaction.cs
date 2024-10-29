
namespace EAMI.CommonEntity
{
    public class RequestTransaction
    {
        // request/transaction
        public int RequestTransactionID { get; set; }
        public DateTime RequestSentTimeStamp { get; set; }
        public DateTime RequestReceivedTimeStamp { get; set; }
        public string SenderID { get; set; }
        public int SOR_ID { get; set; }
        public string ReqMsgTransactionID { get; set; }
        public string ReqMsgTransactionType { get; set; }
        public int TransactionTypeId { get; set; }
        public string MsgTransactionVersion { get; set; }

        public string MsgTransactionRecCount { get; set; }
        public string MsgTotalRecAmount { get; set; }

        // rejected payment inquiry date range
        public DateTime RejectedPaymentDateFrom { get; set; }
        public DateTime RejectedPaymentDateTo { get; set; }        
        
        // response
        public string RespMsgTransactionID { get; set; }
        public string RespMsgTransactionType { get; set; }
        public DateTime ResponseTimeStamp { get; set; }
        public int RespStatusTypeID { get; set; }
        public string ResponseMessage { get; set; }

        // list of payments
        public List<PaymentRec> PaymentRecList { get; set; }
    }

}
