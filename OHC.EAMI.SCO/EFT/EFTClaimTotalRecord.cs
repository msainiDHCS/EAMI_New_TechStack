using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTClaimTotalRecord : SCOClaimTotalRecordBase
    {
        #region Constructors

        public EFTClaimTotalRecord()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "1";
            DETAIL_CODE = "0";
            HEADER_CODE = "0";
        }

        #endregion
    }
}
