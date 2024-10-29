using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;
using System.Globalization;

namespace OHC.EAMI.SCO
{
    internal class SCOClaimScheduleBase
    {
        #region Properties

        private SCOClaimHeaderBase _scoClaimHeader;
        private List<Tuple<SCOPaymentDetailBase, SCOSecondaryPaymentDetailBase>> detailPayments;
        //private List<SCODetailPaymentStatement> detailStatementRecords;
        private SCOClaimTotalRecordBase claimTotal;
        private int _claimLineCount;

        public SCOClaimHeaderBase ClaimHeader
        {
            get
            {
                if (_scoClaimHeader == null) _scoClaimHeader = new SCOClaimHeaderBase();
                return _scoClaimHeader;
            }
        }

        public List<Tuple<SCOPaymentDetailBase, SCOSecondaryPaymentDetailBase>> PaymentDetails
        {
            get
            {
                if (detailPayments == null) detailPayments = new List<Tuple<SCOPaymentDetailBase, SCOSecondaryPaymentDetailBase>>();
                return detailPayments;
            }
        }

        //public List<SCODetailPaymentStatement> DetailStatementRecords
        //{
        //    get
        //    {
        //        //if (detailStatementRecords == null) detailStatementRecords = new List<SCODetailPaymentStatement>();
        //        //return detailStatementRecords;
        //        return new List<SCODetailPaymentStatement>();
        //    }
        //}

        public SCOClaimTotalRecordBase ClaimTotalRecord
        {
            get
            {
                if (claimTotal == null) claimTotal = new SCOClaimTotalRecordBase();
                return claimTotal;
            }
        }

        public int GetRecordCount()
        {
            int cnt = 2;

            cnt = cnt + detailPayments.Count() * 2;

            return cnt;
        }

        public int GetDetailedPaymentRecordCount()
        {
            return detailPayments.Select(b => b.Item1).ToList().Where(a => a.TRLR_CODE == "0" && a.DETAIL_CODE == "1" &&
                          a.HEADER_CODE == "0" && a.LINE_NUMBER == "00" && a.DET_AMOUNT_INDICATOR == "1").Count();
        }

        public virtual int GetPaymentStatementRecordCount()
        {
            return detailPayments.Select(b => b.Item1).SelectMany(__ => __.SCODetailPaymentStatementList).Where(a => 
                a.TRLR_CODE == "0" 
                && a.DETAIL_CODE == "1"
                && a.HEADER_CODE == "0" 
                && (Convert.ToInt32(a.LINE_NUMBER) >= 1 
                && Convert.ToInt32(a.LINE_NUMBER) <= 42)).Count();
        }

        public virtual int  GetAuditRecordCount()
        {
            return detailPayments.Select(b => b.Item1).SelectMany(__ => __.SCOAuditDetailList).Where(a =>
                a.TRLR_CODE == "0"
                && a.DETAIL_CODE == "1"
                && a.HEADER_CODE == "0"
                && (a.LINE_NUMBER == "98")).Count();
        }

        public double GetDetailedPaymentSum()
        {
            return detailPayments.Select(b => b.Item1).ToList().Where(a => a.TRLR_CODE == "0" && a.DETAIL_CODE == "1" &&
                          a.HEADER_CODE == "0" && a.LINE_NUMBER == "00" && a.DET_AMOUNT_INDICATOR == "1").Sum(b => b.dblPaymentAmount);
        }

        #endregion

        #region Constructors

        //public SCOClaimScheduleBase(string claimIdentifier, int claimNumber, int claimScheduleNumber, List<IClaimpayment> lstclaimPayments)
        //{
        //    var claimPaymentList = lstclaimPayments.ToList();
        //    _scoClaimHeader = new SCOClaimHeader();
        //    detailPayments = new List<Tuple<SCOPaymentDetailBase, SCOSecondaryPaymentDetailBase>>();
        //    claimTotal = new SCOClaimTotalRecord();

        //    _scoClaimHeader.CLAIM_ID = claimIdentifier;
        //    _scoClaimHeader.CLAIM_NUMBER = (claimNumber.ToString().Length == 1) ? "0" + claimNumber.ToString() : claimNumber.ToString();
        //    _scoClaimHeader.CLAIM_SCHEDULE_NUMBER = lstclaimPayments.First().ECS_NUMBER;
        //    _claimLineCount++;

        //    foreach (var claimPayment in claimPaymentList.OrderBy(_ => _.SEQ_NUMBER))
        //    {
        //        //WARRANT SECTION IN CSO FILE
        //        SCOPaymentDetailBase scoPaymentDetail = new SCOPaymentDetailBase();
        //        scoPaymentDetail.CLAIM_NUMBER = _scoClaimHeader.CLAIM_NUMBER;
        //        scoPaymentDetail.PAYEE_ID = claimPayment.VENDOR_NO.Trim();
        //        scoPaymentDetail.PAYEE_NAME = claimPayment.VENDOR_NAME.Trim();
        //        scoPaymentDetail.ADDRESS_LINE_1 = claimPayment.VENDOR_ADDRESS_LINE1;
        //        scoPaymentDetail.ADDRESS_LINE_2 = claimPayment.VENDOR_ADDRESS_LINE2;
        //        scoPaymentDetail.ADDRESS_LINE_3 = claimPayment.VENDOR_ADDRESS_LINE3;
        //        scoPaymentDetail.ADDRESS_LINE_4 = claimPayment.VENDOR_ADDRESS_LINE4;
        //        scoPaymentDetail.ZIP_CODE_FIRST_FIVE = claimPayment.VENDOR_ZIPCODE_FIRST5.ToString().Trim();
        //        scoPaymentDetail.ZIP_CODE_LAST_FOUR = claimPayment.VENDOR_ZIPCODE_LAST4;
        //        scoPaymentDetail.PAYMENT_AMOUNT = string.Format("{0:#.00}", claimPayment.PAYMENT_AMOUNT);
        //        scoPaymentDetail.TRANSACTION_CODE = claimPayment.VENDOR_TRANSACTION_CODE;
        //        scoPaymentDetail.TRANSIT_ROUTING_NUMBER = claimPayment.VENDOR_TRANSACTION_ROUTING_CODE;
        //        scoPaymentDetail.CHECK_DIGIT = claimPayment.VENDOR_CHECK_DIGIT;
        //        scoPaymentDetail.DFI_ACCOUNT_NUMBER = claimPayment.VENDOR_DFI_ACCOUNT_NUMBER;
        //        scoPaymentDetail.SEQ_NUMBER = claimPayment.SEQ_NUMBER.ToString().PadLeft(3, '0');
                
        //        SCOSecondaryPaymentDetailBase sscoPaymentDetail = new SCOSecondaryPaymentDetailBase();
        //        sscoPaymentDetail.PAYMENT_AMOUNT = "0";


                
        //        //RA SECTION FOR A GIVEN PAYMENT IN SCO FILE
        //        int scoRALineNumber = 0;

        //        foreach (var ra in claimPayment.EamiClaimRemittanceAdviceRecordList)
        //        {
        //            scoRALineNumber++;
        //            SCODetailPaymentStatementBase scoDetailPaymentStatement = new SCODetailPaymentStatementBase();
        //            scoDetailPaymentStatement.CLAIM_NUMBER = _scoClaimHeader.CLAIM_NUMBER;
        //            scoDetailPaymentStatement.PAYEE_ID = claimPayment.VENDOR_NO;
        //            scoDetailPaymentStatement.ZIP_CODE_FIRST_FIVE = claimPayment.VENDOR_ZIPCODE_FIRST5.ToString();
        //            scoDetailPaymentStatement.ZIP_CODE_LAST_FOUR = claimPayment.VENDOR_ZIPCODE_LAST4;
        //            scoDetailPaymentStatement.DET_AMOUNT_INDICATOR = ra.PRINT_AMOUNT ? "1" : "0";
        //            scoDetailPaymentStatement.DETAIL_STATEMENT_AMOUNT = string.Format("{0:#.00}", ra.AMOUNT);
        //            scoDetailPaymentStatement.LINE_NUMBER = (scoRALineNumber.ToString().Length == 1) ? "0" + scoRALineNumber.ToString() : scoRALineNumber.ToString(); 
        //            scoDetailPaymentStatement.STATEMENT_PRINT_INFO = ra.RA_LINE.ToUpper();
        //            scoDetailPaymentStatement.SEQ_NUMBER = claimPayment.SEQ_NUMBER.ToString().PadLeft(3, '0');

        //            scoPaymentDetail.SCODetailPaymentStatementList.Add(scoDetailPaymentStatement);
        //        }


        //        //AUDIT SECTION (LINE 98)
        //        foreach (var auditLine in claimPayment.EamiClaimAuditRecordList)
        //        {
        //            scoRALineNumber++;
        //            SCOAuditDetailBase scoAuditLine = new SCOAuditDetailBase();
        //            scoAuditLine.CLAIM_NUMBER = _scoClaimHeader.CLAIM_NUMBER;
        //            scoAuditLine.PAYEE_ID = claimPayment.VENDOR_NO;
        //            scoAuditLine.ZIP_CODE_FIRST_FIVE = claimPayment.VENDOR_ZIPCODE_FIRST5.ToString();
        //            scoAuditLine.ZIP_CODE_LAST_FOUR = claimPayment.VENDOR_ZIPCODE_LAST4;
        //            scoAuditLine.DET_AMOUNT_INDICATOR = "1";
        //            scoAuditLine.DETAIL_STATEMENT_AMOUNT = FormatDetailStatementAmount(auditLine.FUNDING_TOTAL_AMOUNT);                    
        //            scoAuditLine.LINE_NUMBER = "98";
        //            scoAuditLine.SEQ_NUMBER = claimPayment.SEQ_NUMBER.ToString().PadLeft(3, '0');
        //            scoAuditLine.PAYMENT_TYPE = auditLine.PAYMENT_TYPE;
        //            scoAuditLine.CONTRACT_NUMBER = auditLine.CONTRACT_NUMBER;
        //            scoAuditLine.CONTRACT_DATE_FROM = auditLine.CONTRACT_DATE_FROM;
        //            scoAuditLine.CONTRACT_DATE_TO = auditLine.CONTRACT_DATE_TO;
        //            scoAuditLine.INDEX_CODE = auditLine.INDEX_CODE;
        //            scoAuditLine.OBJECT_DETAIL_CODE = auditLine.OBJECT_DETAIL_CODE;
        //            scoAuditLine.PCA_CODE = auditLine.PCA_CODE;
        //            scoAuditLine.FISCAL_YEAR = auditLine.FISCAL_YEAR;
        //            scoAuditLine.EXCLUSIVE_PAYMENT_CODE = auditLine.EXCLUSIVE_PAYMENT_CODE;
        //            scoAuditLine.PAYMENT_DATE = auditLine.PAYMENT_DATE;
        //            scoAuditLine.AMOUNT = string.Format("{0:#.00}", auditLine.AMOUNT);
        //            scoAuditLine.PAYMENT_REC_NUMBER = auditLine.PAYMENT_REC_NUMBER;
        //            scoAuditLine.PAYMENT_REC_NUMBER_EXT = auditLine.PAYMENT_REC_NUMBER_EXT;
        //            scoAuditLine.PAYMENT_SET_NUMBER = auditLine.PAYMENT_SET_NUMBER;
        //            scoAuditLine.PAYMENT_SET_NUMBER_EXT = auditLine.PAYMENT_SET_NUMBER_EXT;
        //            scoAuditLine.CLAIM_SCHEDULE_NUMBER = auditLine.CLAIM_SCHEDULE_NUMBER;
        //            scoAuditLine.FUNDING_FISCAL_YEAR_QTR = auditLine.FUNDING_FISCAL_YEAR_QTR;
        //            scoAuditLine.FUNDING_SOURCE_NAME = auditLine.FUNDING_SOURCE_NAME;
        //            scoAuditLine.FUNDING_FFP_AMOUNT = string.Format("{0:#.00}", auditLine.FUNDING_FFP_AMOUNT);
        //            scoAuditLine.FUNDING_SGF_AMOUNT = string.Format("{0:#.00}", auditLine.FUNDING_SGF_AMOUNT);

        //            scoPaymentDetail.SCOAuditDetailList.Add(scoAuditLine);
        //        }

        //        detailPayments.Add(new Tuple<SCOPaymentDetailBase, SCOSecondaryPaymentDetailBase>(scoPaymentDetail, sscoPaymentDetail));
        //        _claimLineCount++;
        //    }
            
        //    //claim schedule total, assuming seconday vendor details will not be used            
        //    claimTotal.CLAIM_NUMBER = _scoClaimHeader.CLAIM_NUMBER;

        //    //WARRANT LINE COUNT
        //    claimTotal.TOTAL_CLAIM_CREDITDEBIT_RECORD_COUNT = GetDetailedPaymentRecordCount().ToString();

        //    //RA LINE COUNT
        //    claimTotal.TOTAL_CLAIM_STATEMENT_RECORD_COUNT = (GetPaymentStatementRecordCount() + GetAuditRecordCount()).ToString();
            
        //    //TOTAL WARRANT AMOUNT
        //    claimTotal.TOTAL_CLAIM_CREDIT_AMOUNT = string.Format("{0:#.00}", GetDetailedPaymentSum());
        //}

        #endregion

        #region Private Methods

        private string FormatDetailStatementAmount(double tot_amnt)
        {
            //This is to place the NEGTAIVE SIGN 
            //as the first character of the 
            //11 character long DetailStatementAmount value
            //EXAMPLES: 
            //   "-0000458.25"
            //   "-8569458.87"
            //   "00000785.58"
            //   "00089785.21"

            string amt_sign = tot_amnt < 0 ? "-" : string.Empty;
            int padLen = string.IsNullOrEmpty(amt_sign) ? 11 : 10;
            char zeroChar = char.Parse("0");
            string amt = string.Format("{0}{1}", amt_sign, string.Format("{0:#.00}", Math.Abs(tot_amnt)).PadLeft(padLen, zeroChar));

            return amt;
        }

        #endregion
    }
}