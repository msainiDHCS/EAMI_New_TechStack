using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class PaymentGroup : PaymentBase
    {
        public string PaymentSuperGroupKey { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ApprovedBy { get; set; }

        public override decimal Amount
        {
            get
            {
                decimal retValue = 0;
                if (this.PaymentRecordList != null && this.PaymentRecordList.Count > 0)
                {
                    retValue = this.PaymentRecordList.Sum(pr => pr.Amount);
                }
                return retValue;
            }
            set
            { }
        }

        public decimal AmountSetAtPgLevel
        {
            get
            { return base.Amount; }
            set
            { base.Amount = value; }
        }

        public string PaymentSetNumberFull
        {
            get
            {
                return this.UniqueNumber + "_" + this.PaymentSetNumberExt;
            }
            set { }
        }

        public string PaymentSetNumberExt { get; set; }

        //public override string PRICode
        //{
        //    get
        //    {
        //        // return 1st PRICode from the list if all list items have same code - else return empty string
        //        string retValue = string.Empty;
        //        if (this.PaymentRecordList != null && this.PaymentRecordList.Count > 0)
        //        {
        //            if (this.PaymentRecordList.GroupBy(item => item.PRICode).Count() == 1)
        //            {
        //                retValue = this.PaymentRecordList[0].PRICode;
        //            }
        //        }
        //        return retValue;
        //    }
        //    set
        //    { }
        //}                

        public EntityStatus OnHoldFlagStatus { get; set; }
        public EntityStatus ReleasedFromHoldFlagStatus { get; set; }
        public EntityStatus ReleaseFromSupFlagStatus { get; set; }

        public List<PaymentRec> PaymentRecordList { get; set; }

        public bool HasReportableRPIItems()
        {
            return HasListItems() && (PaymentRecordList.FirstOrDefault(item => item.IsReportableRPI)) != null;
        }

        public decimal ReportableAmount
        {
            get
            {
                decimal retValue = 0;
                if (this.HasReportableRPIItems())
                {
                    retValue = this.PaymentRecordList.Where(pr => pr.IsReportableRPI == true).Sum(pr => pr.Amount);
                                 
                }
                return retValue;
            }
        }
            
        
        private bool HasListItems()
        {
            return this.PaymentRecordList != null && this.PaymentRecordList.Count > 0;
        }
    }
}
