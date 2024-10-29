using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTAuditDetail : SCOAuditDetailBase
    {        
        #region Constructors

        public EFTAuditDetail()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "1";
            HEADER_CODE = "0";
            SCO_INTERNAL_USE = "00";
            LINE_NUMBER = "98";
            DET_AMOUNT_INDICATOR = "1";
            FILLER_1 = String.Empty.PadLeft(4);
        }

        #endregion
    }
}
