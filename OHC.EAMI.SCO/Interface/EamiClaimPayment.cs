using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    public class EamiClaimPayment : IClaimpayment
    {
        public int ClaimScheduleNumber { get; set; }
        public string ECS_NUMBER { get; set; }
        public int SEQ_NUMBER { get; set; }
        public string VENDOR_NO { get; set; }
        public string VENDOR_NAME { get; set; }
        public string VENDOR_ADDRESS_LINE1 { get; set; }
        public string VENDOR_ADDRESS_LINE2 { get; set; }
        public string VENDOR_ADDRESS_LINE3 { get; set; }
        public string VENDOR_ADDRESS_LINE4 { get; set; }
        public string VENDOR_ZIPCODE_FIRST5 { get; set; }
        public string VENDOR_ZIPCODE_LAST4 { get; set; }

        /// <summary>
        /// For SCO EFT, Credits (22 - checking, 32 - savings) Debits(27 - checking, 37 - savings) Prenotes(23 - checking, 33 - savings)
        /// </summary>
        public string VENDOR_TRANSACTION_CODE { get; set; }
        /// <summary>
        /// 9 digit which includes check digit 
        /// </summary>
        public string VENDOR_TRANSACTION_ROUTING_CODE { get; set; }
        public string VENDOR_CHECK_DIGIT { get; set; }
        public string VENDOR_DFI_ACCOUNT_NUMBER { get; set; }

        public string SECONDARY_VENDOR_NO { get; set; }
        public string SECONDARY_VENDOR_LAST_NAME { get; set; }
        public string SECONDARY_VENDOR_FIRST_NAME { get; set; }
        public string SECONDARY_VENDOR_MIDDLE_INT { get; set; }
        public string SECONDARY_VENDOR_ADDRESS_LINE1 { get; set; }
        public string SECONDARY_VENDOR_ADDRESS_LINE2 { get; set; }
        public string SECONDARY_VENDOR_ADDRESS_LINE3 { get; set; }
        public string SECONDARY_VENDOR_ADDRESS_LINE4 { get; set; }
        public string SECONDARY_VENDOR_ZIPCODE_FIRST5 { get; set; }
        public string SECONDARY_VENDOR_ZIPCODE_LAST4 { get; set; }       
        public string TRN02_Reference_ID { get; set; }
        public string TRN03_Company_ID { get; set; }
        public double PAYMENT_AMOUNT { get; set; }

        public List<EamiClaimRemittanceAdviceRecord> EamiClaimRemittanceAdviceRecordList { get; set; }
        public List<EamiClaimAuditRecord> EamiClaimAuditRecordList { get; set; }

        public EamiClaimPayment()
        {
            EamiClaimRemittanceAdviceRecordList = new List<EamiClaimRemittanceAdviceRecord>();
            EamiClaimAuditRecordList = new List<EamiClaimAuditRecord>();
        }
    }
}
