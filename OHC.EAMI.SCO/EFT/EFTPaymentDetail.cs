using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTPaymentDetail : SCOPaymentDetailBase
    {       

        public List<SCODetailPaymentStatementBase> ECSDetailPaymentStatementList { get; set; }
        public List<SCOAuditDetailBase> ECSAuditDetailList { get; set; }

        #region Constructors

        public EFTPaymentDetail()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "1";
            HEADER_CODE = "0";
            LINE_NUMBER = "00";
            DET_AMOUNT_INDICATOR = "1";
            REPORTABLE_CODE = "0";
            SCO_INTERNAL_USE = "00";
            SCO_INTERNAL_USE_2 = String.Empty.PadLeft(23);
            RA_PRINT_SUPPRESS_IND = "";
            //FILLER_3 = String.Empty.PadLeft(3);

            ECSDetailPaymentStatementList = new List<SCODetailPaymentStatementBase>();
            ECSAuditDetailList = new List<SCOAuditDetailBase>();
        }

        #endregion
    }
}
;