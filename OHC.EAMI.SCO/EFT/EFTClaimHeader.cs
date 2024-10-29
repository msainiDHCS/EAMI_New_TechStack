using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTClaimHeader : SCOClaimHeaderBase
    {
        [FixedLengthFileFieldAttributes(Position = 31, ByteLength = 1)]
        public string CDD_INDICATOR { get; set; }
        
        [FixedLengthFileFieldAttributes(Position = 32, ByteLength = 1)]
        public string HIPAA_Indicator { get; set; }

        [FixedLengthFileFieldAttributes(Position = 33, ByteLength = (8006 - 33))]
        public string FILLER_3 { get; set; }


        #region Constructors

        public EFTClaimHeader()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "0";
            HEADER_CODE = "1";
            CDD_INDICATOR = "P";
            HIPAA_Indicator = "";
            FILLER_1 = String.Empty.PadLeft(3);
            FILLER_2 = String.Empty.PadLeft(2);
            FILLER_3 = String.Empty.PadLeft(8006-33);
        }

        #endregion
    }
}
