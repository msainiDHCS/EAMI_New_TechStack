namespace EAMI.CommonEntity
{
    public class PaymentSuperGroup
    {
        public string UniqueKey { get; set; }
        public PaymentExcEntityInfo PayeeInfo { get; set; }
        public string PaymentType { get; set; }
        public string ContractNumber { get; set; }
        public string FiscalYear { get; set; }
        public string ExclusivePaymentCode { get; set; }
        public RefCode PaymentMethodType { get; set; }

        public decimal Amount
        {
            get
            {
                return this.HasListItems() ? this.PaymentGroupList.Sum(pr => pr.Amount) : 0;
            }
            set
            { }
        }

        public List<PaymentGroup> PaymentGroupList { get; set; }

        public bool HasExclusivePaymentType()
        {
            return HasListItems() && (PaymentGroupList.FirstOrDefault(item => item.ExclusivePaymentType.Code != "NONE")) != null;
        }
        
        public bool HasItemsOnHold()
        {
            return HasListItems() && (PaymentGroupList.FirstOrDefault(item => (item.OnHoldFlagStatus != null)) != null);
        }

        public bool HasItemsReleaseFromHold()
        {
            return HasListItems() && (PaymentGroupList.FirstOrDefault(item => (item.ReleasedFromHoldFlagStatus != null)) != null);
        }

        public bool HasItemsReleaseFromSup()
        {
            return HasListItems() && (PaymentGroupList.FirstOrDefault(item => (item.ReleaseFromSupFlagStatus != null)) != null);
        }

        private bool HasListItems()
        {
            return this.PaymentGroupList != null && this.PaymentGroupList.Count > 0;
        }

    }
}
