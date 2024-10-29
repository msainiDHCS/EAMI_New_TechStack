using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;
using OHC.EAMI.Common;
using System.IO;
using OHC.EAMI.Common.FixedLengthFileParser;

namespace OHC.EAMI.SCO
{
    public class ScoServiceManager
    {
        private int Organization_ID = 0;
        private string _claim_Identifier;

        public ScoServiceManager(int OrgID)
        {
            Organization_ID = OrgID;
        }

        public string CLAIM_IDENTIFIER
        {   get
            {
                return _claim_Identifier;
            }

            set
            {
                _claim_Identifier = value;
            }
        }

    public CommonStatusPayload<Tuple<string, int>> CreateWarrantFile(List<IClaimpayment> inputPaymentData)
        {
            string generatedSCOFile = string.Empty;
            int scoFileLineCount = 0;

            //holder for potential errors
            CommonStatus cStaus = null;

            ECSGeneratedFileValidator sfValidator = new ECSGeneratedFileValidator(Organization_ID, _claim_Identifier);

            //sort the input data based on zip code based on http://www.sco.ca.gov/files-aud/reqts.pdf, page 7 #10
            inputPaymentData.Sort((x, y) => x.VENDOR_ZIPCODE_FIRST5.CompareTo(y.VENDOR_ZIPCODE_FIRST5));

            cStaus = sfValidator.Execute(inputPaymentData);

            if (cStaus.Status)
            {
                //generate the sco file               

                if (inputPaymentData != null && inputPaymentData.Count() > 0)
                {
                    if (Organization_ID > 0)
                    {
                        StringBuilder sb = new StringBuilder();

                        ECSPaymentFile pf = new ECSPaymentFile(Organization_ID, _claim_Identifier, inputPaymentData);

                        if (pf != null && pf.FileHeader != null)
                        {
                            sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSFileHeader>(pf.FileHeader));

                            if (pf.ClaimSchedules != null & pf.ClaimSchedules.Count() > 0)
                            {
                                foreach (ECSClaimSchedule cs in pf.ClaimSchedules)
                                {
                                    if (cs.ClaimHeader != null && cs.PaymentDetails != null && cs.PaymentDetails.Count() > 0 && cs.ClaimTotalRecord != null)
                                    {
                                        sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSClaimHeader>(cs.ClaimHeader));

                                        foreach (Tuple<ECSPaymentDetail, ECSSecondaryPaymentDetail> scoPaymentDetail in cs.PaymentDetails)
                                        {
                                            //PRINT WARRANT LINE
                                            sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSPaymentDetail>(scoPaymentDetail.Item1));

                                            //PRINT RA LINE
                                            foreach (ECSDetailPaymentStatement scoDetailPaymentStatement in scoPaymentDetail.Item1.ECSDetailPaymentStatementList)
                                            {
                                                sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSDetailPaymentStatement>(scoDetailPaymentStatement));
                                            }

                                            //PRINT AUDIT LINE
                                            foreach (ECSAuditDetail wrAuditDetail in scoPaymentDetail.Item1.ECSAuditDetailList)
                                            {
                                                sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSAuditDetail>(wrAuditDetail));
                                            }
                                        }

                                        if (cs.ClaimTotalRecord != null)
                                            sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSClaimTotalRecord>(cs.ClaimTotalRecord));
                                    }
                                }

                                if (pf.FileTotal != null)
                                {
                                    sb.AppendLine(FixedLengthRecord.ConvertFixedLength<ECSFileTotalRecord>(pf.FileTotal));

                                    //CAPTURE total file line COUNT
                                    scoFileLineCount = pf.FileLineCount;
                                }
                            }
                        }

                        generatedSCOFile = sb.ToString();
                    }
                }
            }
            
            if (cStaus.HasDetails())
                return new CommonStatusPayload<Tuple<string, int>>(new Tuple<string, int>(string.Empty, scoFileLineCount), false, cStaus.MessageDetailList);
            else
                return new CommonStatusPayload<Tuple<string, int>>(new Tuple<string, int>(generatedSCOFile, scoFileLineCount), true);
        }

        public CommonStatusPayload<Tuple<string, int>> CreateEFTFile(List<IClaimpayment> inputPaymentData, Boolean IsHIPAA)
        {
            string generatedSCOFile = string.Empty;
            int scoFileLineCount = 0;

            //holder for potential errors
            CommonStatus cStaus = null;

            EFTGeneratedFileValidator sfValidator = new EFTGeneratedFileValidator(Organization_ID, _claim_Identifier);

            //sort the input data based on zip code based on http://www.sco.ca.gov/files-aud/reqts.pdf, page 7 #10
            //inputPaymentData.Sort((x, y) => x.VENDOR_ZIPCODE_FIRST5.CompareTo(y.VENDOR_ZIPCODE_FIRST5));
            //EAMI-RX EFT payments need to be sorted by the Sequence Number

            cStaus = sfValidator.Execute(inputPaymentData);

            if (cStaus.Status)
            {       
                if (inputPaymentData != null && inputPaymentData.Count() > 0)
                {
                    if (Organization_ID > 0)
                    {
                        StringBuilder sb = new StringBuilder();

                        EFTPaymentFile pf = new EFTPaymentFile(Organization_ID, _claim_Identifier, inputPaymentData, IsHIPAA);

                        if (pf != null && pf.FileHeader != null)
                        {
                            sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTFileHeader>(pf.FileHeader));

                            if (pf.ClaimSchedules != null & pf.ClaimSchedules.Count() > 0)
                            {
                                foreach (EFTClaimSchedule cs in pf.ClaimSchedules)
                                {
                                    if (cs.ClaimHeader != null && cs.PaymentDetails != null && cs.PaymentDetails.Count() > 0 && cs.ClaimTotalRecord != null)
                                    {
                                        sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTClaimHeader>(cs.ClaimHeader));

                                        foreach (Tuple<EFTPaymentDetail, EFTSecondaryPaymentDetail> scoPaymentDetail in cs.PaymentDetails)
                                        {
                                            //PRINT WARRANT LINE
                                            sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTPaymentDetail>(scoPaymentDetail.Item1));

                                            //PRINT RA LINE
                                            foreach (EFTDetailPaymentStatement scoDetailPaymentStatement in scoPaymentDetail.Item1.ECSDetailPaymentStatementList)
                                            {
                                                sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTDetailPaymentStatement>(scoDetailPaymentStatement));
                                            }

                                            //PRINT AUDIT LINE
                                            foreach (EFTAuditDetail wrAuditDetail in scoPaymentDetail.Item1.ECSAuditDetailList)
                                            {
                                                sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTAuditDetail>(wrAuditDetail));
                                            }
                                        }

                                        if (cs.ClaimTotalRecord != null)
                                            sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTClaimTotalRecord>(cs.ClaimTotalRecord));
                                    }
                                }

                                if (pf.FileTotal != null)
                                {
                                    sb.AppendLine(FixedLengthRecord.ConvertFixedLength<EFTFileTotalRecord>(pf.FileTotal));

                                    //CAPTURE total file line COUNT
                                    scoFileLineCount = pf.FileLineCount;
                                }
                            }
                        }

                        generatedSCOFile = sb.ToString();
                    }
                }
            }

            if (cStaus.HasDetails())
                return new CommonStatusPayload<Tuple<string, int>>(new Tuple<string, int>(string.Empty, scoFileLineCount), false, cStaus.MessageDetailList);
            else
                return new CommonStatusPayload<Tuple<string, int>>(new Tuple<string, int>(generatedSCOFile, scoFileLineCount), true);
        }

        public CommonStatusPayload<List<EamiDexRecord>> ReadDEXFile(List<string> dexLines)
        {
            List<EamiDexRecord> scoDexRecordList = new List<EamiDexRecord>();

            foreach (string line in dexLines)
            {
                SCODexRecord dexParsedRecord = new SCODexRecord();
                dexParsedRecord.Parse(line);
                scoDexRecordList.Add(PopulateDexRecord(dexParsedRecord));
            }

            return new CommonStatusPayload<List<EamiDexRecord>>(scoDexRecordList, true); 
        }

        private EamiDexRecord PopulateDexRecord(SCODexRecord dexParsedRecord)
        {
            return new EamiDexRecord()
            {
                ADDRESS_LINE_1 = dexParsedRecord.ADDRESS_LINE_1,
                ADDRESS_LINE_2 = dexParsedRecord.ADDRESS_LINE_2,
                ADDRESS_LINE_3 = dexParsedRecord.ADDRESS_LINE_3,
                ADDRESS_LINE_4 = dexParsedRecord.ADDRESS_LINE_4,
                AE_CODE = dexParsedRecord.AE_CODE,
                AGENCY = dexParsedRecord.AGENCY,
                AGENCY_CODE = dexParsedRecord.AGENCY_CODE,
                CLAIM_NUMBER = dexParsedRecord.CLAIM_NUMBER,
                CLAIM_SCHEDULE_NUMBER = dexParsedRecord.CLAIM_SCHEDULE_NUMBER,
                FILE_CODE = dexParsedRecord.FILE_CODE,
                FUND = dexParsedRecord.FUND,
                ISSUE_DATE = dexParsedRecord.ISSUE_DATE,
                PAYEE_ID = dexParsedRecord.PAYEE_ID,
                PAYEE_NAME = dexParsedRecord.PAYEE_NAME,
                REFERENCE_NO = dexParsedRecord.REFERENCE_NO,
                SCHEDULE_TYPE = dexParsedRecord.SCHEDULE_TYPE,
                SEQ_NUMBER = dexParsedRecord.SEQ_NUMBER,
                STATUTE_YEAR = dexParsedRecord.STATUTE_YEAR,
                WARRANT_AMOUNT = dexParsedRecord.WARRANT_AMOUNT,
                WARRANT_NUMBER = dexParsedRecord.WARRANT_NUMBER,
                ZIP_CODE = dexParsedRecord.ZIP_CODE                
            };
        }
    }
}
