using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.SCO
{
    public class EamiDexRecord
    {
        public string FILE_CODE { get; set; }
        public string AGENCY_CODE { get; set; }
        public string CLAIM_SCHEDULE_NUMBER { get; set; }
        public string SCHEDULE_TYPE { get; set; }
        public string FUND { get; set; }
        public string AGENCY { get; set; }
        public string STATUTE_YEAR { get; set; }
        public string REFERENCE_NO { get; set; }
        public string ISSUE_DATE { get; set; }
        public string WARRANT_NUMBER { get; set; }
        public string WARRANT_AMOUNT { get; set; }
        public string PAYEE_ID { get; set; }
        public string PAYEE_NAME { get; set; }
        public string ADDRESS_LINE_1 { get; set; }
        public string ADDRESS_LINE_2 { get; set; }
        public string ADDRESS_LINE_3 { get; set; }
        public string ADDRESS_LINE_4 { get; set; }
        public string AE_CODE { get; set; }
        public string CLAIM_NUMBER { get; set; }
        public string ZIP_CODE { get; set; }
        public string SEQ_NUMBER { get; set; }
    }
}
