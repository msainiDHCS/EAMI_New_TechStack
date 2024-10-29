using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.SCO
{
    public class EamiClaimAuditRecord
    {
        public string AUDIT_LINE { get; set; }
        public string PAYMENT_TYPE { get; set; }
        public string CONTRACT_NUMBER { get; set; }
        public string CONTRACT_DATE_FROM { get; set; }
        public string CONTRACT_DATE_TO { get; set; }
        public string INDEX_CODE { get; set; }
        public string OBJECT_DETAIL_CODE { get; set; }
        public string PCA_CODE { get; set; }
        public string FISCAL_YEAR { get; set; }
        public string EXCLUSIVE_PAYMENT_CODE { get; set; }
        public string PAYMENT_DATE { get; set; }
        public double AMOUNT { get; set; }
        public string PAYMENT_REC_NUMBER { get; set; }
        public string PAYMENT_REC_NUMBER_EXT { get; set; }
        public string PAYMENT_SET_NUMBER { get; set; }
        public string PAYMENT_SET_NUMBER_EXT { get; set; }
        public string CLAIM_SCHEDULE_NUMBER { get; set; }
        public string FUNDING_FISCAL_YEAR_QTR { get; set; }
        public string FUNDING_SOURCE_NAME { get; set; }
        public double FUNDING_FFP_AMOUNT { get; set; }
        public double FUNDING_SGF_AMOUNT { get; set; }
        public double FUNDING_TOTAL_AMOUNT { get; set; }
    }
}
