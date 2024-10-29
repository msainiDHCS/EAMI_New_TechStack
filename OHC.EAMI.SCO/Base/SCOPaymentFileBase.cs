using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class SCOPaymentFileBase
    {
        #region Properties

        private SCOFileHeaderBase header;
        private List<SCOClaimScheduleBase> claimSchedules;
        private SCOFileTotalRecordBase fileTotal;
        private int _fileLineCount = 0;

        public int GetCount()
        {
            int cnt = 2;

            foreach (SCOClaimScheduleBase r in claimSchedules)
            {
                cnt = cnt + r.GetRecordCount();
            }

            return cnt;
        }

        public SCOFileHeaderBase FileHeader
        {
            get
            {
                if (header == null)
                    header = new SCOFileHeaderBase();

                return header;
            }
        }

        //public virtual List<SCOClaimScheduleBase> ClaimSchedules
        //{
        //    get
        //    {
        //        if (claimSchedules == null)
        //            claimSchedules = new List<SCOClaimScheduleBase>();

        //        return claimSchedules;
        //    }
        //}

        public virtual SCOFileTotalRecordBase FileTotal
        {
            get
            {
                if (fileTotal == null)
                    fileTotal = new SCOFileTotalRecordBase();

                return fileTotal;
            }
        }


        public int FileLineCount
        {
            get
            {
                return _fileLineCount;
            }
        }

        #endregion

        //#region Constructors

        //public SCOPaymentFileBase(int agencyID, string claim_Identifier, List<IClaimpayment> lstPaymentRecords)
        //{
        //    int warantLineCount = 0;
        //    int raLineCount = 0;
        //    int auditLineCount = 0;
        //    double sumTotal = 0;

        //    //initialize
        //    header = new SCOFileHeader();
        //    claimSchedules = new List<SCOClaimSchedule>();
        //    fileTotal = new SCOFileTotalRecord();
            

        //    header.AGENCY_ID = agencyID.ToString();

        //    List<int> availableCSNumbers = lstPaymentRecords.Select(a => a.ClaimScheduleNumber).Distinct().ToList();

        //    //loop thru all claim schedule numbers available
        //    int tempClaimNumber = 0;

        //    //foreach (int csn in availableCSNumbers)
        //    //{
        //    //    tempClaimNumber++;
        //    //    claimSchedules.Add(new SCOClaimSchedule(tempClaimNumber, csn, lstPaymentRecords.Where(a => a.ClaimScheduleNumber == csn).ToList()));
        //    //}

        //    string ecs_number = lstPaymentRecords.First().ECS_NUMBER;
        //    int csn = 0;
        //    tempClaimNumber++;
        //    claimSchedules.Add(new SCOClaimSchedule(claim_Identifier, tempClaimNumber, csn, lstPaymentRecords));

        //    foreach (SCOClaimSchedule cs in claimSchedules)
        //    {
        //        warantLineCount = warantLineCount + cs.GetDetailedPaymentRecordCount();
        //        raLineCount = raLineCount + cs.GetPaymentStatementRecordCount();
        //        auditLineCount = auditLineCount + cs.GetAuditRecordCount();
        //        sumTotal = sumTotal + cs.GetDetailedPaymentSum();
        //    }

        //    fileTotal.CLAIM_COUNT = claimSchedules.Count().ToString();
        //    fileTotal.TOTAL_FILE_CREDITDEBIT_RECORD_COUNT = warantLineCount.ToString();
        //    fileTotal.TOTAL_FILE_STATEMENT_RECORD_COUNT = (raLineCount + auditLineCount).ToString();
        //    fileTotal.TOTAL_FILE_CREDIT_AMOUNT = string.Format("{0:#.00}", sumTotal);
            
        //    int fileBodyLineCount =  (claimSchedules.Count() * 2) + warantLineCount + raLineCount + auditLineCount;
        //    int fileHeaderLineCount = 1;
        //    int fileFooterLineCount = 1;

        //    fileTotal.TOTAL_RECORD_COUNT = (fileHeaderLineCount + fileBodyLineCount).ToString();
        //    _fileLineCount = fileHeaderLineCount + fileFooterLineCount + fileBodyLineCount;

        //    #endregion
        }
    }
