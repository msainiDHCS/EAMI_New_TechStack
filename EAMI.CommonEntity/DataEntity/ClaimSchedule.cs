
namespace EAMI.CommonEntity
{
    public class ClaimSchedule : PaymentBase
    {

        public override decimal Amount
        {
            get
            {
                decimal retValue = base.Amount;

                if (this.PaymentGroupList != null && this.PaymentGroupList.Count > 0)
                {
                    retValue = this.PaymentGroupList.Sum(pg => pg.Amount);
                }
                return retValue;
            }
            set
            {
                base.Amount = value;
            }
        }

        public decimal AmountSetAtPgLevel
        {
            get
            {
                decimal retValue = base.Amount;

                if (this.PaymentGroupList != null && this.PaymentGroupList.Count > 0)
                {
                    retValue = this.PaymentGroupList.Sum(pg => pg.AmountSetAtPgLevel);
                }
                return retValue;
            }
            set { base.Amount = value; }
        }

        public bool HasNegativeFundingSource { get; set; }
        public bool IsLinked { get; set; }
        public string LinkedByPGNumber { get; set; }
        public List<string> LinkedCSNumberList { get; set; }
        public List<AggregatedFundingDetail> AggregatedFundingDetailList { get; set; }
        public List<PaymentGroup> PaymentGroupList { get; set; }        
        public bool HasRemittanceAdviceNote { get; set; }
        public string RemittanceAdviceNote { get; set; }
        public int SeqNumber { get; set; }
        public WarrantRec Warrant { get; set; }
    }
}
