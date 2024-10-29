using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OHC.EAMI.SCO
{
    public interface IClaimpayment 
    {
        int ClaimScheduleNumber { get; set; }
        string ECS_NUMBER { get; set; }
        int SEQ_NUMBER { get; set; }
        string VENDOR_NO { get; set; }
        string VENDOR_NAME { get; set; }
        string VENDOR_ADDRESS_LINE1 { get; set; }
        string VENDOR_ADDRESS_LINE2 { get; set; }
        string VENDOR_ADDRESS_LINE3 { get; set; }
        string VENDOR_ADDRESS_LINE4 { get; set; }
        string VENDOR_ZIPCODE_FIRST5 { get; set; }
        string VENDOR_ZIPCODE_LAST4 { get; set; }
        /// <summary>
        /// For SCO EFT, Credits (22 - checking, 32 - savings) Debits(27 - checking, 37 - savings) Prenotes(23 - checking, 33 - savings)
        /// </summary>
        string VENDOR_TRANSACTION_CODE { get; set; }
        /// <summary>
        /// 9 digit which includes check digit 
        /// </summary>
        string VENDOR_TRANSACTION_ROUTING_CODE { get; set; }
        string VENDOR_CHECK_DIGIT { get; set; }
        string VENDOR_DFI_ACCOUNT_NUMBER { get; set; }
        
        string SECONDARY_VENDOR_NO { get; set; }
        string SECONDARY_VENDOR_LAST_NAME { get; set; }
        string SECONDARY_VENDOR_FIRST_NAME { get; set; }
        string SECONDARY_VENDOR_MIDDLE_INT { get; set; }
        string SECONDARY_VENDOR_ADDRESS_LINE1 { get; set; }
        string SECONDARY_VENDOR_ADDRESS_LINE2 { get; set; }
        string SECONDARY_VENDOR_ADDRESS_LINE3 { get; set; }
        string SECONDARY_VENDOR_ADDRESS_LINE4 { get; set; }
        string SECONDARY_VENDOR_ZIPCODE_FIRST5 { get; set; }
        string SECONDARY_VENDOR_ZIPCODE_LAST4 { get; set; }
        string TRN02_Reference_ID { get; set; }
        string TRN03_Company_ID { get; set; }
        double PAYMENT_AMOUNT { get; set; }

        List<EamiClaimRemittanceAdviceRecord> EamiClaimRemittanceAdviceRecordList { get; set; }
        List<EamiClaimAuditRecord> EamiClaimAuditRecordList { get; set; }
    }
}
