using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class ECSPaymentDetail : SCOPaymentDetailBase
    {
        public List<SCODetailPaymentStatementBase> ECSDetailPaymentStatementList { get; set; }
        public List<SCOAuditDetailBase> ECSAuditDetailList { get; set; }

        #region Constructors

        public ECSPaymentDetail()
        {
            RECORD_CODE = "05";
            TRLR_CODE = "0";
            DETAIL_CODE = "1";
            HEADER_CODE = "0";
            LINE_NUMBER = "00";
            DET_AMOUNT_INDICATOR = "1";
            REPORTABLE_CODE = "0";
            SCO_INTERNAL_USE = "00";

            ECSDetailPaymentStatementList = new List<SCODetailPaymentStatementBase>();
            ECSAuditDetailList = new List<SCOAuditDetailBase>();
        }

        #endregion
    }
}
;