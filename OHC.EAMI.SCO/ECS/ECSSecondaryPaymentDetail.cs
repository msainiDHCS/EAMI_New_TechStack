using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class ECSSecondaryPaymentDetail : SCOSecondaryPaymentDetailBase
    {
        #region Constructors

        public ECSSecondaryPaymentDetail()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "1";
            HEADER_CODE = "0";
            LINE_NUMBER = "00";
            DET_AMOUNT_INDICATOR = "2";
        }

        #endregion
    }
}
