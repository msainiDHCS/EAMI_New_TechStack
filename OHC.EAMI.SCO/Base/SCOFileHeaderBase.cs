using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOFileHeaderBase
    {
        #region Properties

        [FixedLengthFileFieldAttributes(Position = 1, ByteLength = 5)]
        public string RECORD_ID { get; set; }

        [FixedLengthFileFieldAttributes(Position = 6, ByteLength = 5)]
        public string FILLER_1 { get; set; }

        /// <summary>
        /// 4 digit-left justify: Zero fill uniform agency code.Source : Uniform Codes Manual, Department of Finance.
        /// </summary>
        [FixedLengthFileFieldAttributes(Position = 11, ByteLength = 4)]
        public string AGENCY_ID { get; set; }

        [FixedLengthFileFieldAttributes(Position = 15, ByteLength = 5)]
        public string SYSTEM_IDENTIFICATION { get; set; }

        [FixedLengthFileFieldAttributes(Position = 20, ByteLength = (8006 - 20))]
        public string FILLER_2 { get; set; }
        
        #endregion
    }
}
