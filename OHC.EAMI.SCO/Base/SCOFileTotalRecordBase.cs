using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOFileTotalRecordBase
    {
        #region Properties

        [FixedLengthFileFieldAttributes(Position = 1, ByteLength = 5)]
        public string RECORD_ID { get; set; }

        [FixedLengthFileFieldAttributes(Position = 6, ByteLength = 2)]
        public string FILLER_1 { get; set; }

        /// <summary>
        /// Total number of all records except file total record.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 8, ByteLength = 13, DefaultFiler = '0')]
        public string TOTAL_RECORD_COUNT { get; set; }

        /// <summary>
        /// Total number of all claim header records with Record Type = 001. 
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 21, ByteLength = 5, DefaultFiler = '0')]
        public string CLAIM_COUNT { get; set; }

        /// <summary>
        /// Total number of all detail payment records with Record-Type = 010 Line-No = 00 DetAmt-Ind = 1. 
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 26, ByteLength = 9, DefaultFiler = '0')]
        public string TOTAL_FILE_CREDITDEBIT_RECORD_COUNT { get; set; }

        /// <summary>
        /// Total number of all statement records with Record-Type = 010 Line-No = 01 to 42.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 35, ByteLength = 11, DefaultFiler = '0')]
        public string TOTAL_FILE_STATEMENT_RECORD_COUNT { get; set; }

        //[FixedLengthFileFieldAttributes(Position = 46, ByteLength = 3)]
        //public string SCO_INTERNAL_USE { get; set; }

        /// <summary>
        /// Total dollar amount of all detail payment records for claim with "RecordType=010, Line-No=00, Det-Amt-Ind=1". Byte 59 must be hard coded with a
        /// decimal. Should agree with total on claim schedule.File Credit amount cannot exceed $9,999,999,999.99. Zero fill.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 46, ByteLength = 16, DefaultFiler = '0')]
        public string TOTAL_FILE_CREDIT_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position = 62, ByteLength = (8006 - 62))]
        public string FILLER_2 { get; set; }

        #endregion

        #region Constructors

        public SCOFileTotalRecordBase()
        {
            RECORD_ID = "99EOF";
        }

        #endregion
    }
}
