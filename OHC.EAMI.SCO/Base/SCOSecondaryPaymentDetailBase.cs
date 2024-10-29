using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOSecondaryPaymentDetailBase
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

        [FixedLengthFileFieldAttributes(Position = 8, ByteLength = 5)]
        public string ZIP_CODE_FIRST_FIVE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 13, ByteLength = 4)]
        public string ZIP_CODE_LAST_FOUR { get; set; }

        [FixedLengthFileFieldAttributes(Position = 17, ByteLength = 10)]
        public string PAYEE_ID { get; set; }

        [FixedLengthFileFieldAttributes(Position = 27, ByteLength = 6)]
        public string SEQ_NUMBER { get; set; }
        
        [FixedLengthFileFieldAttributes(Position = 33, ByteLength = 2)]
        public string LINE_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 35, ByteLength = 1)]
        public string DET_AMOUNT_INDICATOR { get; set; }

        [FixedLengthFileFieldAttributes(Position = 36, ByteLength = 2)]
        public string SCO_INTERNAL_USE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 38, ByteLength = 11)]
        public string PAYMENT_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 49, ByteLength = 4)]
        public string FILLER_1 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 53, ByteLength = 30)]
        public string PAYEE_NAME { get; set; }

        [FixedLengthFileFieldAttributes(Position = 83, ByteLength = 5)]
        public string FILLER_2 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 88, ByteLength = 30)]
        public string ADDRESS_LINE_1 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 118, ByteLength = 30)]
        public string ADDRESS_LINE_2 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 148, ByteLength = 30)]
        public string ADDRESS_LINE_3 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 178, ByteLength = 30)]
        public string ADDRESS_LINE_4 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 208, ByteLength = 50)]
        public string FILLER_3 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 258, ByteLength = (8006 - 258))]
        public string SP_ADUTI_INFO { get; set; }

        #endregion        
    }
}
