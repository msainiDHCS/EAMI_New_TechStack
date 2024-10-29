using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCODetailPaymentStatementBase
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
        
        /// <summary>
        /// Statement line number; vaild values are line 1 to 42; line number 1 is the first line to be machine printed; must be ascending.Skip line are not printed.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 33, ByteLength = 2)]
        public string LINE_NUMBER { get; set; }
        
        /// <summary>
        /// 0' = No statement detail amount present; '1' detail statement amount present therefore adds and compares to payment amount or '3' = EFT that adds and compares to claim schedule
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 35, ByteLength = 1)]
        public string DET_AMOUNT_INDICATOR { get; set; }

        [FixedLengthFileFieldAttributes(Position = 36, ByteLength = 2)]
        public string SCO_INTERNAL_USE { get; set; }

        /// <summary>
        /// Statement line shows payment information (total or subtotal); required if Det-Amt-Ind='1'. Zero fill if Det-Amt-Ind is off = '0'. Right justify, zero fill, no commas, or $. 
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 38, ByteLength = 11, DefaultFiler = '0')]
        public string DETAIL_STATEMENT_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 49, ByteLength = 4)]
        public string FILLER_1 { get; set; }

        /// <summary>
        /// For agency use to describe payment to payee: must include agency name, address, and telephone number for inquiry purposes. Print info is “free form” formatted by agency. Must be in UPPERCASE.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 53, ByteLength = 62)]
        public string STATEMENT_PRINT_INFO { get; set; }

        [FixedLengthFileFieldAttributes(Position = 115, ByteLength = (8006 - 115))]
        public string STATEMENT_AUDIT_INFO { get; set; }

        #endregion

        //#region Constructors

        //public SCODetailPaymentStatementBase()
        //{
        //    RECORD_CODE = "05";
        //    TRLR_CODE = "0";
        //    DETAIL_CODE = "1";
        //    HEADER_CODE = "0";
        //    SCO_INTERNAL_USE = "00";
        //    FILLER_1 = String.Empty.PadLeft(4);
        //}

        //#endregion
    }
}
