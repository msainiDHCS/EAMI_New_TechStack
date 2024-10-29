using OHC.EAMI.Common.FixedLengthFileGenerator;
using OHC.EAMI.Common.FixedLengthFileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.SCO
{
    internal class SCODexRecord : FixedLengthFileParser
    {
        [FixedLengthFileFieldAttributes(Position = 1, ByteLength = 3)]
        public string FILE_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 4, ByteLength = 4)]
        public string AGENCY_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 8, ByteLength = 9)]
        public string FILLER_1 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 17, ByteLength = 8)]
        public string CLAIM_SCHEDULE_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 25, ByteLength = 3)]
        public string FILLER_2 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 28, ByteLength = 2)]
        public string SCHEDULE_TYPE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 30, ByteLength = 4)]
        public string FUND { get; set; }

        [FixedLengthFileFieldAttributes(Position = 34, ByteLength = 3)]
        public string FILLER_3 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 37, ByteLength = 4)]
        public string AGENCY { get; set; }

        [FixedLengthFileFieldAttributes(Position = 41, ByteLength = 4)]
        public string STATUTE_YEAR { get; set; }

        [FixedLengthFileFieldAttributes(Position = 45, ByteLength = 7)]
        public string REFERENCE_NO { get; set; }

        [FixedLengthFileFieldAttributes(Position = 52, ByteLength = 31)]
        public string FILLER_4 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 83, ByteLength = 8)]
        public string ISSUE_DATE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 91, ByteLength = 8)]
        public string WARRANT_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 99, ByteLength = 6)]
        public string FILLER_5 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 105, ByteLength = 13)]
        public string WARRANT_AMOUNT { get; set; }

        [FixedLengthFileFieldAttributes(Position =118, ByteLength = 10)]
        public string PAYEE_ID { get; set; }

        [FixedLengthFileFieldAttributes(Position = 128, ByteLength = 30)]
        public string PAYEE_NAME { get; set; }

        [FixedLengthFileFieldAttributes(Position = 158, ByteLength = 5)]
        public string FILLER_6 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 163, ByteLength = 30)]
        public string ADDRESS_LINE_1 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 193, ByteLength = 30)]
        public string ADDRESS_LINE_2 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 223, ByteLength = 30)]
        public string ADDRESS_LINE_3 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 253, ByteLength = 30)]
        public string ADDRESS_LINE_4 { get; set; }

        [FixedLengthFileFieldAttributes(Position = 283, ByteLength = 1)]
        public string AE_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 284, ByteLength = 2)]
        public string CLAIM_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 286, ByteLength = 9)]
        public string ZIP_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 295, ByteLength = 3)]
        public string SEQ_NUMBER { get; set; }

        //[FixedLengthFileFieldAttributes(Position = 301, ByteLength = 9)]
        //public string FILLER_7 { get; set; }
    }
}
