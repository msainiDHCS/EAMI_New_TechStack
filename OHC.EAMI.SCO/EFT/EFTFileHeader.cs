using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTFileHeader : SCOFileHeaderBase
    {
        #region Constructors

        public EFTFileHeader()
        {
            RECORD_ID = "00HDR";
            SYSTEM_IDENTIFICATION = "EFTTC";
            FILLER_1 = String.Empty.PadLeft(5);
            FILLER_2 = String.Empty.PadLeft(8006 - 20);
        }

        #endregion
    }
}
