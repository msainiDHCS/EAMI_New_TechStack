using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class ECSClaimHeader : SCOClaimHeaderBase
    {
        [FixedLengthFileFieldAttributes(Position = 31, ByteLength = (8006 - 31))]
        public string FILLER_3 { get; set; }

        #region Constructors

        public ECSClaimHeader()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "0";
            HEADER_CODE = "1";
            FILLER_1 = String.Empty.PadLeft(3);
            FILLER_2 = String.Empty.PadLeft(2);
            FILLER_3 = String.Empty.PadLeft(8006 - 31);
        }

        #endregion
    }
}
