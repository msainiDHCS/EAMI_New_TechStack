using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOClaimTotalRecordBase
    {
        #region Properties

        [FixedLengthFileFieldAttributes(Position = 1, ByteLength = 2)]
        public string RECORD_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 3, ByteLength = 2)]
        public string CLAIM_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 5, ByteLength = 1)]
        public string TRLR_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 6, ByteLength = 1)]
        public string DETAIL_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 7, ByteLength = 1)]
        public string HEADER_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 8, ByteLength = 18)]
        public string FILLER_1 { get; set; }

        /// <summary>
        /// Total number of all detail payment records for claim with "Record-Type 010 Line-No = 00, Det-Amt-Ind = 1". Should agree with number of payments on claim schedule.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 26, ByteLength = 9, DefaultFiler = '0')]
        public string TOTAL_CLAIM_CREDITDEBIT_RECORD_COUNT { get; set; }

        /// <summary>
        /// Total number of all statement records for claim with "Record-Type 010 Line-No-01 Through 42 (statement record)". 
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 35, ByteLength = 11, DefaultFiler = '0')]
        public string TOTAL_CLAIM_STATEMENT_RECORD_COUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 46, ByteLength = 3, DefaultFiler = '0')]
        public string SCO_INTERNAL_USE { get; set; }

        /// <summary>
        /// Total dollar amount of all detail payment records for claim with "Record Type = 010, Line-No = 00, Det-Amt-Ind = 1". Byte 59 must be hard coded with a decimal. Should        
        /// agree with total on claim schedule.Claim Credit amount cannot exceed $9,999,999,999.99. Zero fill.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 49, ByteLength = 13, DefaultFiler = '0')]
        public string TOTAL_CLAIM_CREDIT_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 62, ByteLength = (8006 - 62))]
        public string FILLER_2 { get; set; }

        #endregion
    }
}
