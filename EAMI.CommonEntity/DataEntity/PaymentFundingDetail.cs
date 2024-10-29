
namespace EAMI.CommonEntity
{
    public class PaymentFundingDetail : AggregatedFundingDetail
    {
        public int FundingDetailID { get; set; }
        public int PaymentRecID { get; set; }
        public string FiscalYear { get; set; }
        public string FiscalQuarter { get; set; }
        public string Title { get; set; }
        public string FedFundCode { get; set; }
        public string StateFundCode { get; set; }
        public Dictionary<string, string> FundingKvpList { get; set; }
    }

    public class AggregatedFundingDetail
    {
        public string FundingSourceName { get; set; }
        public decimal FFPAmount { get; set; }
        public decimal SGFAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
