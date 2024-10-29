using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

//using TSAE.Entity;
using FTSAS.Integration;

using OHC.EAMI.Common;
using OHC.EAMI.DataAccess;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.SCO;
using OHC.EAMI.Util.FileTransfer;
using System.Text.RegularExpressions;
using System.Data;

namespace OHC.EAMI.FTSASTaskActions
{

    [TaskAction(ActionName = "ActionWarrantFileGenerate", IsPhantom = false)]
    public class ActionWarrantFileGenerate : TaskActionBase
    {
        public override TaskResult Execute()
        {
            string dataSourceKey = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
            EAMIDBConnection.EAMIDBContext = dataSourceKey;
            TaskResult result = new TaskResult(true, string.Empty, enProductiveOutcome.NONE);
            string outputFolderPath = Context.GetExecutionArgTextValueByKey("OUTPUT_FOLDER").Trim();
            string archiveFolderPath = Context.GetExecutionArgTextValueByKey("ARCHIVE_FOLDER").Trim();

            if (string.IsNullOrEmpty(outputFolderPath))
            {
                throw new Exception("OUTPUT_FOLDER value is empty.");
            }

            if (string.IsNullOrEmpty(archiveFolderPath))
            {
                throw new Exception("ARCHIVE_FOLDER value is empty.");
            }

            try
            {
                EAMIDBConnection.EAMIDBContext = dataSourceKey;
                outputFolderPath = outputFolderPath.LastOrDefault() == '\\' ? outputFolderPath : string.Format("{0}\\", outputFolderPath);
                archiveFolderPath = archiveFolderPath.LastOrDefault() == '\\' ? archiveFolderPath : string.Format("{0}\\", archiveFolderPath);
                string scheduledTaskNumber = Context == null ? string.Empty : Context.TaskNumber;

                //GET APPROVED CLAIM SCHEDULE LIST
                int paymentMethodTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).Where(_ => _.Code == "WARRANT").First().ID;
                int currentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).Where(_ => _.Code == "APPROVED").First().ID;

                //CREATE AND PACKAGE Electronic Claim Schedule
                List<ElectronicClaimSchedule> ecsList = new List<ElectronicClaimSchedule>();

                //GET A LIST OF APPROVED ECS's
                ecsList = DataAccess.ClaimScheduleDataDbMgr.GetElectronicClaimSchedulesByDateRangeStatusType(currentStatusTypeId, null, null).Where(_ => _.PaymentMethodType.ID == paymentMethodTypeId).ToList();

                //PROCEED IF ECS EXIST
                if (ecsList.Count() > 0)
                {
                    //UPDATE ECS Task Number
                    DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleTaskNumber(ecsList.Select(_ => _.EcsId).ToList(), scheduledTaskNumber, null);

                    //CREATE SCO FILES
                    List<ElectronicClaimSchedule> scoFileList = CreateSCOFile(ecsList, outputFolderPath);

                    //UPDATE ESC status to DB
                    UpdateECSStatus(scoFileList);

                    //ARCHIVE ECS FILE
                    ArchiveECSFiles(scoFileList, outputFolderPath, archiveFolderPath);

                    //SET TASK result
                    result.Status = scoFileList.Where(_ => _.CurrentStatusType.Code == "SENT_TO_SCO").Count() > 0;
                    //by default set outcome to FULL
                    result.Outcome = enProductiveOutcome.FULL;

                    if (result.Status && scoFileList.Where(_ => _.CurrentStatusType.Code == "FAIL").Count() > 0)
                    {
                        //If Status is true - Capture note outcome and set otcome as PARTIAL
                        result.Outcome = enProductiveOutcome.PARTIAL;
                        result.Note = scoFileList.Where(_ => _.CurrentStatusType.Code == "FAIL").First().CurrentStatusNote;
                    }
                    else if (!result.Status)
                    {
                        //If Status is false - Capture note outcome and set otcome as NONE
                        result.Outcome = enProductiveOutcome.NONE;
                        result.Note = scoFileList.Where(_ => _.CurrentStatusType.Code == "FAIL").First().CurrentStatusNote;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Outcome = enProductiveOutcome.NONE;
                result.Note = ex.Message + "; " + ex.StackTrace;
                EAMILogger.Instance.Error(ex);
            }

            //RETURN Status
            return result;
        }

        private List<IClaimpayment> PopulateSCOClaimPayment(ElectronicClaimSchedule ecs)
        {
            List<IClaimpayment> claimPaymentList = new List<IClaimpayment>();

            foreach (ClaimSchedule ecs_cs in ecs.ClaimScheduleList.OrderBy(_ => _.SeqNumber))
            {
                //FETCH CS FROM DB
                ClaimSchedule cs = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { ecs_cs.PrimaryKeyID }).Where(_ => _.PrimaryKeyID == ecs_cs.PrimaryKeyID).First();

                //DETERMINE ZIP CODE
                Tuple<string, string> parsedZip = ParseZipCodePartsFromFullZipCode(cs.PayeeInfo.PEE_Zip);

                //DETERMINE ADDRESS LINES
                Tuple<string, string, string, string> scoAddressLines = DetermineScoAddressLines(cs.PayeeInfo);

                //PAYMENT LINE
                EamiClaimPayment eamiClaimPayment = new EamiClaimPayment()
                {
                    ClaimScheduleNumber = cs.PrimaryKeyID,
                    ECS_NUMBER = ecs.EcsNumber,
                    SEQ_NUMBER = cs.SeqNumber,
                    PAYMENT_AMOUNT = Convert.ToDouble(cs.Amount),
                    VENDOR_NAME = cs.PayeeInfo.PEE_Name.ToUpper(),
                    VENDOR_ADDRESS_LINE1 = scoAddressLines.Item1.ToUpper(),
                    VENDOR_ADDRESS_LINE2 = scoAddressLines.Item2.ToUpper(),
                    VENDOR_ADDRESS_LINE3 = scoAddressLines.Item3.ToUpper(),
                    VENDOR_ADDRESS_LINE4 = scoAddressLines.Item4.ToUpper(),
                    VENDOR_ZIPCODE_FIRST5 = parsedZip.Item1,
                    VENDOR_ZIPCODE_LAST4 = parsedZip.Item2,
                    VENDOR_NO = cs.PayeeInfo.PEE_FullCode,
                    VENDOR_CHECK_DIGIT = "",
                    VENDOR_DFI_ACCOUNT_NUMBER = "",
                    VENDOR_TRANSACTION_CODE = "",
                    VENDOR_TRANSACTION_ROUTING_CODE = ""
                };

                //REMITTANCE ADVICE LINES
                eamiClaimPayment.EamiClaimRemittanceAdviceRecordList.AddRange(PopulateRemittanceAdviceLines(cs, eamiClaimPayment));

                //AUDIT LINES
                eamiClaimPayment.EamiClaimAuditRecordList.AddRange(PopulateClaimAuditRecord(cs));

                claimPaymentList.Add(eamiClaimPayment);
            }

            return claimPaymentList;
        }

        private List<EamiClaimRemittanceAdviceRecord> PopulateRemittanceAdviceLines(ClaimSchedule cs, EamiClaimPayment cp)
        {
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            int maxremittanceAdviceLines = 42;

            List<EamiClaimRemittanceAdviceRecord> eamiClaimRemittanceAdviceRecordList = new List<EamiClaimRemittanceAdviceRecord>();
            EAMIMasterData eamiMasterData = new EAMIMasterData();
            eamiMasterData.SystemProperty = rcTableList.GetRefCodeListByTableName(enRefTables.TB_System).GetRefCodeByCode<SystemProperty>(rcTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code);

            string DeptName = eamiMasterData.SystemProperty.DepartmentName;
            string AddLine = eamiMasterData.SystemProperty.RADepartmentAddrLine;
            string AddCSZ = eamiMasterData.SystemProperty.RADepartmentAddrCSZ;
            string InquiryPhone = eamiMasterData.SystemProperty.RAInquiryPhNo;

            //ADD REQUIRED RA LINES                
            cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Format("{0}", DeptName).ToUpper(), Convert.ToDouble(cs.Amount)));
            cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Format("{0}", AddLine).ToUpper(), 0));
            cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Format("{0}", AddCSZ).ToUpper(), 0));
            cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Format("PAYMENT INQUIRIES: {0}", InquiryPhone), 0));
            cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Format("CONTRACT NO. {0}", cs.ContractNumber), 0));
            cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Empty, 0));

            //NOTE LINES
            List<string> noteLines = DetermineRemittanceAdviseNoteLines(cs.RemittanceAdviceNote);
            int noteLineCount = noteLines.Count > 0 ? noteLines.Count + 1 : noteLines.Count;

            //REMITTANCE ADVICE LINES
            foreach (PaymentGroup pg in cs.PaymentGroupList)
            {
                if (maxremittanceAdviceLines <= (cp.EamiClaimRemittanceAdviceRecordList.Count + noteLines.Count))
                {
                    break;
                }

                EamiClaimRemittanceAdviceRecord cra = new EamiClaimRemittanceAdviceRecord()
                {
                    AMOUNT = 0,
                    PRINT_AMOUNT = false,
                    RA_LINE = string.Format("DATE: {0:MMM dd, yyyy} PYMT_SET: {1}",
                    pg.PaymentDate,
                    pg.PaymentSetNumberExt)
                };

                cp.EamiClaimRemittanceAdviceRecordList.Add(cra);
            }

            //INSERT AN EMPTY LINE BEFORE THE NOTE PARAGRAPH - IF NOTE IS PRESENT
            if (noteLines.Count > 0)
            {
                cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(string.Empty, 0));
            }

            //REMITTANCE ADVICE NOTE
            foreach (string noteLine in noteLines)
            {
                cp.EamiClaimRemittanceAdviceRecordList.Add(AddSingleRimittanceAdviceLine(noteLine.TrimEnd(), 0));
            }

            return eamiClaimRemittanceAdviceRecordList;
        }

        private List<EamiClaimAuditRecord> PopulateClaimAuditRecord(ClaimSchedule cs)
        {
            List<EamiClaimAuditRecord> eamiClaimAuditRecordList = new List<EamiClaimAuditRecord>();

            foreach (PaymentGroup pg in cs.PaymentGroupList.OrderBy(_ => _.Amount))
            {
                foreach (PaymentRec pr in pg.PaymentRecordList)
                {
                    List<PaymentFundingDetail> paymentFundingDetail = DataAccess.PaymentDataDbMgr.GetFundingDetailsByPaymentRecID(pr.PrimaryKeyID);

                    foreach (PaymentFundingDetail fd in paymentFundingDetail.OrderBy(_ => _.FundingSourceName))
                    {
                        eamiClaimAuditRecordList.AddRange(ComposeEamiClaimAuditLines(pg, pr, cs, fd));
                    }
                }
            }

            return eamiClaimAuditRecordList;
        }

        private List<EamiClaimAuditRecord> ComposeEamiClaimAuditLines(PaymentGroup pg, PaymentRec pr, ClaimSchedule cs, PaymentFundingDetail fd)
        {
            List<EamiClaimAuditRecord> eamiClaimAuditRecordList = new List<EamiClaimAuditRecord>();

            decimal negative_Max = 9999999.99M;
            decimal positive_Max = 99999999.99M;

            decimal ffp_amt = fd.FFPAmount;
            decimal ssg_amt = fd.SGFAmount;
            decimal tot_amt = fd.FFPAmount + fd.SGFAmount;
            decimal max_amt = tot_amt > 0 ? positive_Max : negative_Max;

            int splitCount = DetermineAuditLineSplit(max_amt, tot_amt);

            for (var i = 1; i <= splitCount; i++)
            {
                //DETERMINE AUDIT LINE AMOUNT
                decimal splt_amt = Math.Abs(tot_amt) > max_amt ? max_amt : Math.Abs(tot_amt);
                decimal new_tot_amt = Math.Abs(tot_amt) - splt_amt;

                //NEGATE AMOUNTS
                splt_amt = tot_amt >= 0 ? splt_amt : splt_amt * -1;
                tot_amt = tot_amt >= 0 ? new_tot_amt : new_tot_amt * -1;

                //SET FFP and SSG AMOUNT on the first AUDIT REC only
                ffp_amt = i == 1 ? ffp_amt : 0;
                ssg_amt = i == 1 ? ssg_amt : 0;

                //POPULATE AUDIT RECORD
                eamiClaimAuditRecordList.Add(PopulateEamiClaimAuditRecord(pg, pr, cs, fd, splt_amt, ffp_amt, ssg_amt));
            };

            return eamiClaimAuditRecordList;
        }

        private EamiClaimAuditRecord PopulateEamiClaimAuditRecord(PaymentGroup pg, PaymentRec pr, ClaimSchedule cs, PaymentFundingDetail fd, decimal total_fund_amt, decimal ffp_amt, decimal ssg_amt)
        {
            return new EamiClaimAuditRecord()
            {
                PAYMENT_TYPE = pg.PaymentType.ToUpper(),
                CONTRACT_NUMBER = pg.ContractNumber,
                CONTRACT_DATE_FROM = pg.ContractDateFrom.ToString("MMddyyyy"),
                CONTRACT_DATE_TO = pg.ContractDateTo.ToString("MMddyyyy"),
                INDEX_CODE = pr.IndexCode.ToUpper(),
                OBJECT_DETAIL_CODE = pr.ObjDetailCode.ToUpper(),
                PCA_CODE = pr.PCACode.ToUpper(),
                FISCAL_YEAR = pg.FiscalYear,
                EXCLUSIVE_PAYMENT_CODE = pg.ExclusivePaymentType.Code.ToUpper(),
                PAYMENT_DATE = pr.PaymentDate.ToString("MMddyyyy"),
                AMOUNT = Convert.ToDouble(pr.Amount),
                PAYMENT_REC_NUMBER = pr.UniqueNumber,
                PAYMENT_REC_NUMBER_EXT = pr.PaymentRecNumberExt,
                PAYMENT_SET_NUMBER = pg.UniqueNumber,
                PAYMENT_SET_NUMBER_EXT = pg.PaymentSetNumberExt,
                CLAIM_SCHEDULE_NUMBER = cs.UniqueNumber,
                FUNDING_FISCAL_YEAR_QTR = string.Format("{0}{1}", fd.FiscalYear, fd.FiscalQuarter),
                FUNDING_SOURCE_NAME = fd.FundingSourceName.ToUpper(),
                FUNDING_FFP_AMOUNT = Convert.ToDouble(ffp_amt),
                FUNDING_SGF_AMOUNT = Convert.ToDouble(ssg_amt),
                FUNDING_TOTAL_AMOUNT = Convert.ToDouble(total_fund_amt)
            };
        }

        private int DetermineAuditLineSplit(decimal max_amt, decimal tot_amt)
        {
            int splitCount = 1;

            if (Math.Abs(tot_amt) > max_amt)
            {
                decimal add_splits = Math.Abs(tot_amt) / max_amt;
                splitCount = splitCount + Convert.ToInt32(Math.Floor(add_splits));
            }

            return splitCount;
        }

        private List<string> DetermineRemittanceAdviseNoteLines(string note)
        {
            List<string> noteLines = new List<string>();
            char delimetter = Convert.ToChar(";");

            if (!string.IsNullOrEmpty(note))
            {
                //REMOVE NEW LINE CHARACTERS. MAKE NOTE UPPER CASE
                note = Regex.Replace(note, @"(?<!\r)\n+", "").ToUpper();

                if (!string.IsNullOrEmpty(note.Trim(delimetter).Trim()))
                {
                    noteLines = note.Split(delimetter).ToList();
                }
            }

            return noteLines;
        }

        private EamiClaimRemittanceAdviceRecord AddSingleRimittanceAdviceLine(string printLine, double amount)
        {
            return new EamiClaimRemittanceAdviceRecord()
            {
                AMOUNT = amount,
                PRINT_AMOUNT = false,
                RA_LINE = printLine
            };
        }

        private Tuple<string, string> ParseZipCodePartsFromFullZipCode(string fullZipCode)
        {
            Tuple<string, string> result = new Tuple<string, string>(string.Empty, string.Empty);

            /* *
             * EXAMPLE:
             * 94080
             * 930368294
             * */

            try
            {
                string zip5 = string.Empty;
                string zip4 = string.Empty;

                //PARSE ZIP CODE

                //REPLACE: Remove spaces and hyphens
                string line = fullZipCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                //CUT: keep 9 right characters
                line = line.Substring(Math.Max(0, line.Length - 9));

                //GET zip5+4
                if (line.All(Char.IsDigit) && line.Length == 9)
                {
                    zip5 = line.Substring(0, 5);
                    zip4 = line.Substring(5, 4);
                }
                else
                {
                    //CUT: keep 5 right characters
                    line = line.Substring(Math.Max(0, line.Length - 5));

                    //GET zip5 only
                    if (line.All(Char.IsDigit) && line.Length == 5)
                    {
                        zip5 = line;
                    }
                }

                //Resilt
                result = new Tuple<string, string>(zip5, zip4);
            }
            catch { }

            return result;
        }


        private Tuple<string, string, string, string> DetermineScoAddressLines(PaymentExcEntityInfo peei)
        {
            //ARRANGE SCO ADDRESS LINES USING CAPMAN ADDRESS LINES HERE:
            //NOTE: ZIP is not handled in this procedure

            /*
             * EAMI ADDRESS LINES:
             * eami_AddressLine_1 - is required and is assumed to always hold the mailing STREET or PO BOX ADDRESS info
             * eami_AddressLine_2 - is optional
             * eami_AddressLine_3 - is optional
             * eami_City - is required
             * eami_State - is required
             * eami_Zip - is not handled in this procedure
             */

            /*
             * SCO ADDRESS LINES:
             * sco_AddressLine_1 - old - cannot be empty, can hold the Addressee name or Additional Address information, like C/O, ATTENTION, etc.
             *                   - new - change 9/16/2019 per Reuben: cannot be empty, can hold the Addressee name or Additional Address information, like C/O, ATTENTION, etc. or Street Address 
             * sco_AddressLine_2 - is optional and can be empty
             * sco_AddressLine_3 - old - cannot be empty and should always hold the mailing STREET or PO BOX ADDRESS
             *                   - new - change 9/16/2019 per Reuben: this can be either Street or City State
             * sco_AddressLine_4 - old - cannot be empty, should always hold City and State. Zip Code CANNOT! be included in this line
             *                   - new - change 9/16/2019 per Reuben:can be empty when sco_AddressLine_3 holds City State
             */

            //EAMI
            string eami_VendorName = peei.PEE_Name;
            string eami_AddressLine_1 = peei.PEE_AddressLine1;
            string eami_AddressLine_2 = peei.PEE_AddressLine2;
            string eami_AddressLine_3 = peei.PEE_AddressLine3;
            string eami_City = peei.PEE_City;
            string eami_State = peei.PEE_State;

            //SCO
            string sco_AddressLine_1 = string.Empty;
            string sco_AddressLine_2 = string.Empty;
            string sco_AddressLine_3 = string.Empty;
            string sco_AddressLine_4 = string.Empty;

            //eami_AddressLine_1 is assumed to always hold the mailing STREET or PO BOX ADDRESS
            //SET sco_Street_Address with eami_AddressLine_1
            string sco_Street_Address = eami_AddressLine_1;

            //CITY AND STATE
            string sco_City_State = string.Format("{0}  {1}", eami_City, eami_State);

            if (string.IsNullOrEmpty(eami_AddressLine_2.Trim()) && string.IsNullOrEmpty(eami_AddressLine_3.Trim()))
            {
                //SCENARIO: WHEN BOTH EAMI OPTIONAL LINES ARE EMPTY
                //Set sco_AddressLine_1 with STREET ADDRESS
                //KEEP sco_AddressLine_2 empty
                //Set sco_AddressLine_3 with CITY STATE
                //KEEP sco_AddressLine_4 empty
                sco_AddressLine_1 = sco_Street_Address; // STREET ADDRESS
                sco_AddressLine_2 = string.Empty;
                sco_AddressLine_3 = sco_City_State;  // CITY STATE
                sco_AddressLine_4 = string.Empty;
            }
            else if (string.IsNullOrEmpty(eami_AddressLine_2.Trim()))
            {
                //SCENARIO: When optional eami_AddressLine_2 is empty
                //SET sco_AddressLine_1 with optional eami_AddressLine_3
                //KEEP sco_AddressLine_2 empty
                sco_AddressLine_1 = eami_AddressLine_3;
                sco_AddressLine_2 = string.Empty;
                sco_AddressLine_3 = sco_Street_Address;
                sco_AddressLine_4 = sco_City_State;
            }
            else if (string.IsNullOrEmpty(eami_AddressLine_3.Trim()))
            {
                //SCENARIO: When optional eami_AddressLine_3 is empty
                //SET sco_AddressLine_1 with optional eami_AddressLine_2
                //KEEP sco_AddressLine_2 empty
                sco_AddressLine_1 = eami_AddressLine_2;
                sco_AddressLine_2 = string.Empty;
                sco_AddressLine_3 = sco_Street_Address;
                sco_AddressLine_4 = sco_City_State;
            }
            else
            {
                //SCENARIO: WHEN BOTH EAMI OPTIONAL LINES ARE NOT EMPTY
                //Populate sco address lines accordingly
                sco_AddressLine_1 = eami_AddressLine_2;
                sco_AddressLine_2 = eami_AddressLine_3;
                sco_AddressLine_3 = sco_Street_Address;
                sco_AddressLine_4 = sco_City_State;
            }

            //Resilt
            return new Tuple<string, string, string, string>(sco_AddressLine_1, sco_AddressLine_2, sco_AddressLine_3, sco_AddressLine_4);
        }

        private List<ElectronicClaimSchedule> CreateSCOFile(List<ElectronicClaimSchedule> ecsList, string outputFolderPath)
        {
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            
            EAMIMasterData eamiMasterData = new EAMIMasterData();
            eamiMasterData.SystemProperty = rcTableList.GetRefCodeListByTableName(enRefTables.TB_System).GetRefCodeByCode<SystemProperty>(rcTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code);
            int orgID = int.Parse(eamiMasterData.SystemProperty.OrganizationCode);

            RefCode claimScheduleStatusType_FAIL = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).Where(_ => _.Code == "FAIL").First();
            RefCode claimScheduleStatusType_SENT_TO_SCO = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).Where(_ => _.Code == "SENT_TO_SCO").First();
            DataSet dataSet = null;

            foreach (ElectronicClaimSchedule ecs in ecsList)
            {
                DateTime statusDate = DateTime.Now;
                ScoServiceManager ScoServiceManager = new ScoServiceManager(orgID);
                string claimIdentifier = string.Empty;
                string scoFileName = string.Empty;
                //string scoDexFilePath = string.Empty;

                dataSet = DetermineSCOClaimIdentifier(ecs, rcTableList);

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    if (dataRow["SCO_File_Property_Name"].ToString() == "SCO_CLAIM_ID")
                    {
                        claimIdentifier = dataRow["SCO_File_Property_Value"].ToString();
                    }
                    if (dataRow["SCO_File_Property_Name"].ToString() == "SCO_FILE_NAME")
                    {
                        scoFileName = dataRow["SCO_File_Property_Value"].ToString();
                    }
                }

                ScoServiceManager.CLAIM_IDENTIFIER = claimIdentifier;

                //CREATE CSO FILE STRING
                CommonStatusPayload<Tuple<string, int>> status = ScoServiceManager.CreateWarrantFile(PopulateSCOClaimPayment(ecs));

                //CREATE CSO FILE
                if (status.Status)
                {
                    ecs.EcsFileName = ComposeElectronicClaimScheduleFileName(ecs, scoFileName);

                    CommonStatus cs = CreateLocalFile(outputFolderPath, ecs.EcsFileName, status.Payload.Item1);

                    if (!cs.Status)
                    {
                        ecs.CurrentStatusDate = statusDate;
                        ecs.CurrentStatusType = claimScheduleStatusType_FAIL;
                        ecs.CurrentStatusNote = cs.GetFirstDetailMessage();
                    }
                    else
                    {
                        //UPDATE File Name and File line count
                        UpdateECSFileName(ecs, status.Payload.Item2);

                        //SET STATUS
                        ecs.CurrentStatusType = claimScheduleStatusType_SENT_TO_SCO;
                    }
                }
                else
                {
                    ecs.CurrentStatusDate = statusDate;
                    ecs.CurrentStatusType = claimScheduleStatusType_FAIL;
                    ecs.CurrentStatusNote = status.GetCombinedMessage();
                }
            }
            return ecsList;
        }

        private CommonStatus CreateLocalFile(string outputFolderPath, string fileName, string fileString)
        {
            CommonStatus cs = new CommonStatus(true);

            try
            {
                //Create test file for upload   
                using (StreamWriter sw = new StreamWriter(string.Format("{0}{1}", outputFolderPath, fileName)))
                {
                    //write string
                    sw.Write(fileString);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(string.Format("Cannot create SCO file. File name: {0}{1}; {2}", outputFolderPath, fileName, ex.Message));
                EAMILogger.Instance.Error(ex);
                cs.Status = false;
                cs.AddMessageDetail("Cannot create SCO file");
            }

            return cs;
        }

        private DataSet DetermineSCOClaimIdentifier(ElectronicClaimSchedule ecs, RefCodeTableList rcTableList)
        {
            string claimIdentifier = string.Empty;
            DataSet dataSet = null;
            ExclusivePmtType exclusivePmtType = new ExclusivePmtType();
            ExclPmtType exclPmt = new ExclPmtType();

            exclusivePmtType.EPT = rcTableList.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).GetRefCodeByCode<ExclPmtType>(ecs.ExclusivePaymentType.Code);

            int ecsFundId = exclusivePmtType.EPT.Fund_ID;
            int ecsSystemId = exclusivePmtType.EPT.System_ID;
            string ecsPmtType = ecs.PaymentMethodType.Code;
            string scoEnv = string.Empty;

            if (bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString()))
            {
                scoEnv = SCOPRopertiesEnvironments.PROD.ToString();
            }
            else
            {
                scoEnv = SCOPRopertiesEnvironments.TEST.ToString();
            }

            try
            {
                dataSet = DataAccess.ClaimScheduleDataDbMgr.GetSCOFileProperty(ecsFundId, ecsSystemId, ecsPmtType, scoEnv, ecs.ExclusivePaymentType.IsActive = true);

                //foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                //{
                //    if (dataRow["SCO_File_Property_Name"].ToString() == "SCO_CLAIM_ID_FUND_0912_WARRANT")
                //    {
                //        claimIdentifier = dataRow["SCO_File_Property_Value"].ToString();
                //    }
                //}
                //if (ecs.ExclusivePaymentType.Code == "SCHIP")
                //{
                //    claimIdentifier = DBSetting.GetDBSettingValue<string>(rcTableList, "SCO_CLAIM_ID_FUND_0555_WARRANT");
                //}
                //else
                //{
                //    claimIdentifier = DBSetting.GetDBSettingValue<string>(rcTableList, "SCO_CLAIM_ID_FUND_0912_WARRANT");
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not determine SCO Claim Identifier. {0}", ex.Message));
            }

            return dataSet;
        }

        private string ComposeElectronicClaimScheduleFileName(ElectronicClaimSchedule ecs, string scoFileName)
        {
            string resultFileName = string.Empty;
            DateTime dateStamp = DateTime.Now;
            //bool isProductionEnvironment;
            string fileNameIncrementValue = string.Empty;
            // Boolean.TryParse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString(), out isProductionEnvironment);
            //string dbSettingName = string.Empty;

            try
            {
                //DETERMINE FILE SETTING
                //if (ecs.ExclusivePaymentType.Code == "SCHIP")
                //{
                //    dbSettingName = isProductionEnvironment ? "WRNT_FILE_NAME_FUND_0555_PROD" : "WRNT_FILE_NAME_FUND_0555_TEST";
                //}
                //else
                //{
                //    dbSettingName = isProductionEnvironment ? "WRNT_FILE_NAME_FUND_0912_PROD" : "WRNT_FILE_NAME_FUND_0912_TEST";
                //}

                //string fileNameSettingValue = DBSetting.GetDBSettingValue<string>(RefCodeDBMgr.GetRefCodeTableList(), dbSettingName);
                string fileNameSettingValue = scoFileName;

                //GET FILE NAME INCREMENT
                fileNameIncrementValue = DataAccess.ClaimScheduleDataDbMgr.GetECSFileNameIncrement(fileNameSettingValue, dateStamp);

                if (!string.IsNullOrEmpty(fileNameIncrementValue))
                {
                    //Date Time Stamp + Incrementer
                    string fileDateTimeStamp = string.Format("{0:MMddyy}{1}", dateStamp, fileNameIncrementValue);

                    //Compose File Name
                    resultFileName = string.Format(fileNameSettingValue, fileDateTimeStamp);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not compose ECS File Name. {0}", ex.Message));
            }

            return resultFileName;
        }

        private CommonStatus UpdateECSStatus(List<ElectronicClaimSchedule> ecsList)
        {
            CommonStatus cs = new CommonStatus(true);

            foreach (ElectronicClaimSchedule ecs in ecsList)
            {
                DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleStatus(ecs, string.Empty);
            }

            return cs;
        }

        private CommonStatus UpdateECSFileName(ElectronicClaimSchedule ecs, int fileLineCount)
        {
            CommonStatus cs = new CommonStatus(true);
            cs = DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleFileName(ecs.EcsId, ecs.EcsFileName, fileLineCount);
            return cs;
        }

        private void ArchiveECSFiles(List<ElectronicClaimSchedule> scoFileList, string outputFolderPath, string archiveFolderPath)
        {
            foreach (ElectronicClaimSchedule scoFile in scoFileList)
            {
                string sourceFilePath = string.Format(@"{0}{1}", outputFolderPath, scoFile.EcsFileName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
                string fileExtenssion = Path.GetExtension(sourceFilePath).Replace(".", string.Empty);

                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, string.Format(@"{0}{1}.{2}.{3}", archiveFolderPath, fileNameWithoutExtension, Guid.NewGuid(), fileExtenssion));
                }
            }
        }

    }
}

