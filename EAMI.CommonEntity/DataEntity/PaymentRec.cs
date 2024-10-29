
namespace EAMI.CommonEntity
{
    public class PaymentRec : PaymentGroup
    {
        public int TransactionID { get; set; }
        public string PaymentRecNumberFull
        {
            get
            {
                return this.UniqueNumber + "_" + this.PaymentRecNumberExt;
            }
            set { }
        }
        public string PaymentRecNumberExt { get; set; }
        public string IndexCode { get; set; }
        public string ObjDetailCode { get; set; }
        public string ObjAgencyCode { get; set; }
        public string PCACode { get; set; }
        //public string PriorityGroupHash { get; set; }
        public string PaymentSetNumber { get; set; }
        public override decimal Amount { get; set; }

        public bool IsReportableRPI { get; set; }
        public string RPICode { get; set; }


        // payment rec capman extension members


        // composite members and lists
        public Dictionary<string, string> PaymentKvpList { get; set; }
        public List<PaymentFundingDetail> FundingDetailList { get; set; }
    }


    public class PaymentRecTr : PaymentRec
    {
        // trace payment members
        public int TrPaymentRecID { get; set; }
        public string TrPaymentKvpListXML { get; set; }
        public string TrFundingDetialListXML { get; set; }
    }

    public class PaymentRecForUser
    {
        public int Payment_Record_ID { get; set; }
        public int Transaction_ID { get; set; }
        public string PaymentRec_Number { get; set; }
        public string PaymentRec_NumberEx { get; set; }
        public string Payment_Type { get; set; }
        public string Payment_Date { get; set; }
        public decimal Amount { get; set; }
        public string FiscalYear { get; set; }
        public string IndexCode { get; set; }
        public string ObjectDetailCode { get; set; }
        public string ObjectAgencyCode { get; set; }
        public string PCACode { get; set; }
        public string ApprovedBy { get; set; }
        public string PaymentSet_Number { get; set; }
        public string PaymentSet_NumberExt { get; set; }
        public int Payee_Entity_Info_ID { get; set; }
        public string RPICode { get; set; }
        public bool IsReportableRPI { get; set; }
        public int PEE_Address_ID { get; set; }
        public int PEE_EFT_Info_ID { get; set; }
        public int Payment_Method_Type_ID { get; set; }
        public int User_ID { get; set; }
        public string PaymentStatusCode { get; set; }
    }
}
