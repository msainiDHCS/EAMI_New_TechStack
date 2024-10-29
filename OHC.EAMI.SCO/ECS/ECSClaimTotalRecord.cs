using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class ECSClaimTotalRecord : SCOClaimTotalRecordBase
    {
        #region Constructors

        public ECSClaimTotalRecord()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "1";
            DETAIL_CODE = "0";
            HEADER_CODE = "0";
        }

        #endregion
    }
}
