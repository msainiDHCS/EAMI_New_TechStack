using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTSecondaryPaymentDetail : SCOSecondaryPaymentDetailBase
    {
        #region Constructors

        public EFTSecondaryPaymentDetail()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "1";
            HEADER_CODE = "0";
            LINE_NUMBER = "00";
            DET_AMOUNT_INDICATOR = "2";
            FILLER_1 = String.Empty.PadLeft(4);
            FILLER_2 = String.Empty.PadLeft(5);
            //FILLER_3 = String.Empty.PadLeft(50);
        }

        #endregion
    }
}
