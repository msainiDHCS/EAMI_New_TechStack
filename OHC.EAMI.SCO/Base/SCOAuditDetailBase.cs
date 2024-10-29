using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOAuditDetailBase
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

        [FixedLengthFileFieldAttributes(Position = 38, ByteLength = 11, DefaultFiler = '0')]
        public string DETAIL_STATEMENT_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 49, ByteLength = 4)]
        public string FILLER_1 { get; set; }
        
        [FixedLengthFileFieldAttributes(Position = 53, ByteLength = 50)]
        public string PAYMENT_TYPE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 103, ByteLength = 20)]
        public string CONTRACT_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 123, ByteLength = 8)]
        public string CONTRACT_DATE_FROM { get; set; }

        [FixedLengthFileFieldAttributes(Position = 131, ByteLength = 8)]
        public string CONTRACT_DATE_TO { get; set; }
        
        [FixedLengthFileFieldAttributes(Position = 139, ByteLength = 10)]
        public string INDEX_CODE{ get; set; }
        
        [FixedLengthFileFieldAttributes(Position = 149, ByteLength = 10)]
        public string OBJECT_DETAIL_CODE{ get; set; }

        [FixedLengthFileFieldAttributes(Position = 159, ByteLength = 10)]
        public string PCA_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 169, ByteLength = 5)]
        public string FISCAL_YEAR { get; set; }

        [FixedLengthFileFieldAttributes(Position = 174, ByteLength = 10)]
        public string EXCLUSIVE_PAYMENT_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 184, ByteLength = 8)]
        public string PAYMENT_DATE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 192, ByteLength = 11, DefaultFiler = '0')]
        public string AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 203, ByteLength = 14)]
        public string PAYMENT_REC_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 217, ByteLength = 28)]
        public string PAYMENT_REC_NUMBER_EXT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 245, ByteLength = 14)]
        public string PAYMENT_SET_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 259, ByteLength = 28)]
        public string PAYMENT_SET_NUMBER_EXT { get; set; }
        
        [FixedLengthFileFieldAttributes(Position = 287, ByteLength = 20)]
        public string CLAIM_SCHEDULE_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 307, ByteLength = 11)]
        public string FUNDING_FISCAL_YEAR_QTR { get; set; }

        [FixedLengthFileFieldAttributes(Position = 318, ByteLength = 50)]
        public string FUNDING_SOURCE_NAME { get; set; }

        [FixedLengthFileFieldAttributes(Position = 368, ByteLength = 11, DefaultFiler = '0')]
        public string FUNDING_FFP_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 379, ByteLength = 11, DefaultFiler = '0')]
        public string FUNDING_SGF_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 390, ByteLength = 7616)]
        public string FILLER_2 { get; set; }

        #endregion
        
        #region Constructors

        //public SCOAuditDetailBase()
        //{
        //    RECORD_CODE = "05";
        //    TRLR_CODE = "0";
        //    DETAIL_CODE = "1";
        //    HEADER_CODE = "0";
        //    SCO_INTERNAL_USE = "00";
        //    LINE_NUMBER = "98";
        //    DET_AMOUNT_INDICATOR = "1";
        //    FILLER_1 = String.Empty.PadLeft(4);
        //}

        #endregion
    }
}
