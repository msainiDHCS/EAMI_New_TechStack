using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOClaimHeaderBase
    {
        #region Properties

        [FixedLengthFileFieldAttributes(Position = 1, ByteLength = 2)]
        public string RECORD_CODE { get; set; }

        /// <summary>
        /// A Value from '01' to '99', claims within a file are in ascending consecutive order. There is one claim number per claim schedule submitted.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 3, ByteLength = 2)]
        public string CLAIM_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 5, ByteLength = 1)]
        public string TRLR_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 6, ByteLength = 1)]
        public string DETAIL_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 7, ByteLength = 1)]
        public string HEADER_CODE { get; set; }

        [FixedLengthFileFieldAttributes(Position = 8, ByteLength = 3)]
        public string FILLER_1 { get; set; }

        /// <summary>
        /// Number assigned by department's accounting office matching hard copy claim certification submitted by agency; left justified
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 11, ByteLength = 8)]
        public string CLAIM_SCHEDULE_NUMBER { get; set; }

        [FixedLengthFileFieldAttributes(Position = 19, ByteLength = 2)]
        public string FILLER_2 { get; set; }

        /// <summary>
        /// Provided by SCO-Audits after approval of payment system.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 21, ByteLength = 10)]
        public string CLAIM_ID { get; set; }

        //[FixedLengthFileFieldAttributes(Position = 31, ByteLength = (8006 - 31))]
        //public string FILLER_3 { get; set; }
        
        #endregion
    }
}
