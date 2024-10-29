using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using FTSAS.Integration;
using EAMI.EFTPreNote.ServiceContract;

using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;
using System.ServiceModel.Channels;
using OHC.EAMI.Common;
using OHC.EAMI.Util;
using System.Configuration;
using System.Data;

namespace OHC.EAMI.FTSASTaskActions
{
    internal enum enEFTPrenoteStatus
    {
        UNKNOWN = 0,
        RESULT_FOUND = 1,
        NO_RESULT_FOUND = 2,
        ERROR = 3
    }

    [TaskAction(ActionName = "DeterminePEE_EFT_withNotification", IsPhantom = false)]
    public class ActionDeterminePEE_EFT_withNotification : TaskActionBase
    {

        private bool isProductionEnvironment = false;
        private string hostName = string.Empty;
        private string progName = string.Empty;
        private string sysName = string.Empty;
        private int sysId = 0;
        private string tranMsgId = string.Empty;
        private List<PaymentGroup> tranPGList = new List<PaymentGroup>();
        private List<RefCode> eftPeeList = null;
        bool systemSetting_ProcesEFTPayments = false;
        bool systemSetting_ProcesWarrantPayments = false;

        public override TaskResult Execute()
        {
            TaskResult tr = new TaskResult(true);
            CommonStatusPayload<List<RefCode>> cs = new CommonStatusPayload<List<RefCode>>(eftPeeList, false);
            try
            {
                EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                progName = Context.GetExecutionArgTextValueByKey("PROG_NAME");
                sysName = Context.GetExecutionArgTextValueByKey("SYS_NAME");

                systemSetting_ProcesEFTPayments = bool.Parse(RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY).GetRefCodeByCode<SCOSetting>("PROCESS_EFT_PAYMENTS").SCOSettingValue);
                systemSetting_ProcesWarrantPayments = bool.Parse(RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY).GetRefCodeByCode<SCOSetting>("PROCESS_WARRANT_PAYMENTS").SCOSettingValue);

                sysId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).GetRefCodeByCode(sysName).ID;
                isProductionEnvironment = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                hostName = System.Environment.MachineName;
                DataSet ds = PaymentDataSubmissionDBMgr.GetPymtSubmissionTransForNotification();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (systemSetting_ProcesEFTPayments || systemSetting_ProcesWarrantPayments)
                    {
                        //PROCESS A SINGLE TRANSACTION RECORD
                        tranMsgId = ds.Tables[0].Rows[0]["Msg_Transaction_ID"].ToString();

                        int tranId = int.Parse(ds.Tables[0].Rows[0]["Transaction_ID"].ToString());
                        int receivedStatusId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RECEIVED").ID;

                        //FETCH PAYMENTS FOR A GIVEN TRANSACTION_ID AND 'RECEIVED' status
                        tranPGList = PaymentDataDbMgr.GetPaymentGroupsByStatusAndTransaction(receivedStatusId, tranMsgId);

                        //DETERMINE PAYMENT METHOD HERE
                        if (systemSetting_ProcesEFTPayments)
                        {
                            cs = DetermineEFTStatusWithPrenote();
                        }
                        else
                        {
                            cs = SetPaymentsToWarrantPaymentMethodType();
                        }

                        eftPeeList = cs.Payload;

                        //SEND NOTIFICATION
                        bool email_success = false;
                        if (cs.Status)
                        {
                            string message = this.CreateMessageContent(ds.Tables[0].Rows[0]);
                            email_success = this.SendEmail(message, Context);

                            if (!email_success)
                            {
                                cs.AddMessageDetail("Failed to send notification.");
                            }
                        }

                        //HANDLE TASK STATUS
                        if (cs.Status && email_success)
                        {
                            PaymentDataSubmissionDBMgr.UpdatePymtSubmissionTransNonificationFlag(new List<int>() { tranId });
                            tr.Note = cs.GetCombinedMessage();
                            tr.Status = true;
                            tr.Outcome = enProductiveOutcome.FULL;
                        }
                        else
                        {
                            tr.Status = false;
                            tr.Note = cs.GetCombinedMessage();
                            tr.Outcome = enProductiveOutcome.NONE;
                        }
                    }
                    else
                    {
                        tr.Status = false;
                        tr.Outcome = enProductiveOutcome.PARTIAL;
                        tr.Note = "System configuration settings prevent processing EFT and WARRANT payments";
                    }
                }
                else
                {
                    tr.Outcome = enProductiveOutcome.NONE;
                }
            }
            catch (Exception ex)
            {
                tr.Status = false;
                tr.Outcome = enProductiveOutcome.NONE;
                tr.Note = ex.Message + "; " + ex.StackTrace;
                EAMILogger.Instance.Error(ex);
            }
            return tr;
        }

        private CommonStatusPayload<List<RefCode>> DetermineEFTStatusWithPrenote()
        {
            bool isUseArgsForEftCheck = Context.GetExecutionArgValueByKey<bool>("USE-ARGS-FOR-EFT-CHECK");
            if (isUseArgsForEftCheck)
                return DetermineEFTStatusWithFTSASStaticArgs();
            else
                return DetermineEFTStatusWithPrenoteSvc();
        }


        private CommonStatusPayload<List<RefCode>> DetermineEFTStatusWithPrenoteSvc()
        {
            List<RefCode> retValue = new List<RefCode>();
            CommonStatusPayload<List<RefCode>> cs = new CommonStatusPayload<List<RefCode>>(retValue, false);
            RefCodeDBMgr.GetRefCodeTableList(true);
            RefCode paymentMethodType_EFT = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByCode("EFT");
            RefCode paymentMethodType_WARRANT = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByCode("WARRANT");

            //Logging list
            List<string> logging_EFTEntityCodeList = new List<string>();
            List<string> logging_EFTPaymentSetList = new List<string>();
            List<string> logging_WarrantEntityCodeList = new List<string>();
            List<string> logging_WarrantPaymentSetList = new List<string>();

            try
            {
                string svcUri = Context.GetExecutionArgTextValueByKey("SVC_URI");
                bool isNetNamedPipeBinding = Context.GetExecutionArgValueByKey<bool>("NET-NAMED-PIPE-BINDING");

                //GROUP PAYMENT SETS BY ENTITY CODE (VENDOR NUMBER)
                List<EFTEntityBankInfoRequest> request = new List<EFTEntityBankInfoRequest>();
                List<PaymentGroup> groupedPymtSetEntityList = tranPGList.GroupBy(t => t.PayeeInfo.PEE.Code).Select(t2 => t2.First()).ToList();

                cs.AddMessageDetail(string.Format("Transaction ID: {0}", tranMsgId));

                //CREATE PRENOTE REQUESTS HERE (a single request for each PAYEMNT GROUP)
                // here go through payment groups/sets with distinct vendor entities
                foreach (PaymentGroup pg in groupedPymtSetEntityList)
                {
                    EFTEntityBankInfoRequest req = new EFTEntityBankInfoRequest()
                    {
                        SysNameCode = sysName,
                        EntityCode = pg.PayeeInfo.PEE.Code,
                        EntityEIN = pg.PayeeInfo.PEE.EntityEIN
                    };

                    cs.AddMessageDetail(string.Format("PRENOTE: {0};{1}", sysName, pg.PayeeInfo.PEE.Code));
                    request.Add(req);
                }

                Binding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                EndpointAddress address = new EndpointAddress(svcUri);
                EftPrenoteSvcClient cli = new EftPrenoteSvcClient(binding, address);

                //CHECK PRENOTE HERE
                List<EFTEntityBankInfoResponse> response = cli.GetEftEntityBankInfoList(request);
                List<EFTEntityBankInfoResponse> errorResponse = new List<EFTEntityBankInfoResponse>();

                //CAPTURE ERROR RECORDS
                errorResponse = response.Where(t => t.Status == enEFTPrenoteStatus.ERROR.ToString()).ToList();
                if (errorResponse.Count == 0)
                {
                    //HANDLE RECORDS WITH EFT MATCH FOUND
                    List<EFTEntityBankInfoResponse> eftMatchFoundList = response.Where(t => t.Status == enEFTPrenoteStatus.RESULT_FOUND.ToString()).ToList();

                    cs.AddMessageDetail(string.Format("EFT Vendor Count: {0}", eftMatchFoundList.Count().ToString()));

                    if (eftMatchFoundList.Count > 0)
                    {
                        List<string> paymentSetList_EFT = new List<string>();
                        foreach (EFTEntityBankInfoResponse resp in eftMatchFoundList)
                        {
                            PayeeEntity pee = groupedPymtSetEntityList.First(t => t.PayeeInfo.PEE.Code.ToUpper() == resp.EntityCode.ToUpper()).PayeeInfo.PEE;
                            PaymentDataDbMgr.InsertEFTInfo(sysId, pee.ID, tranMsgId, resp.FIRoutingNumber, resp.FIAccountType, resp.PrvAccountNo, DateTime.Parse(resp.DatePrenoted));

                            //CAPTURE EFT PAYMENT SET NUMBERS (UniqueNumber)
                            List<PaymentGroup> pgList = tranPGList.Where(t => t.PayeeInfo.PEE.Code.ToUpper() == resp.EntityCode.ToUpper()).ToList();

                            //logging
                            logging_EFTEntityCodeList.Add(string.Format("EFT-{0}; PMT_SET Count={1}", resp.EntityCode.ToUpper(), pgList.Count()));

                            foreach (PaymentGroup pg in pgList)
                            {
                                paymentSetList_EFT.Add(pg.UniqueNumber);
                                //logging
                                logging_EFTPaymentSetList.Add(string.Format("EFT-{0};{1}", resp.EntityCode.ToUpper(), pg.UniqueNumber));
                            }

                            retValue.Add(pee);
                        }

                        //SET PAYMENT METHOD TYPE TO 'EFT'
                        DataAccess.PaymentDataDbMgr.UpdatePaymentRecordPmtMethodType(paymentSetList_EFT, paymentMethodType_EFT.ID);
                    }

                    //HANDLE RECORDS WITH NO EFT MATCH FOUND
                    bool processWarrants = bool.Parse(RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY).GetRefCodeByCode<SCOSetting>("PROCESS_WARRANT_PAYMENTS").SCOSettingValue);
                    if (processWarrants)
                    {
                        List<EFTEntityBankInfoResponse> eftMatchNotFoundList = response.Where(t => t.Status == enEFTPrenoteStatus.NO_RESULT_FOUND.ToString()).ToList();

                        cs.AddMessageDetail(string.Format("NON-EFT Vendor Count: {0}", eftMatchNotFoundList.Count()));

                        if (eftMatchNotFoundList.Count > 0)
                        {
                            List<string> paymentSetList_NonEFT = new List<string>();

                            //CAPTURE PAYMENT SET NUMBERS (UniqueNumber)
                            foreach (EFTEntityBankInfoResponse resp in eftMatchNotFoundList)
                            {
                                PayeeEntity pee = groupedPymtSetEntityList.First(t => t.PayeeInfo.PEE.Code.ToUpper() == resp.EntityCode.ToUpper()).PayeeInfo.PEE;
                                //paymentSetList_NonEFT.AddRange(groupedPymtSetEntityList.Where(t => t.PayeeInfo.PEE.Code.ToUpper() == resp.EntityCode.ToUpper()).Select(_ => _.UniqueNumber));

                                //CAPTURE EFT PAYMENT SET NUMBERS (UniqueNumber)
                                List<PaymentGroup> pgList = tranPGList.Where(t => t.PayeeInfo.PEE.Code.ToUpper() == resp.EntityCode.ToUpper()).ToList();

                                logging_WarrantEntityCodeList.Add(string.Format("WRT-{0}; PMT_SET Count={1}", resp.EntityCode.ToUpper(), pgList.Count()));

                                foreach (PaymentGroup pg in pgList)
                                {
                                    paymentSetList_NonEFT.Add(pg.UniqueNumber);
                                    //logging
                                    logging_WarrantPaymentSetList.Add(string.Format("WRT-{0};{1}", resp.EntityCode.ToUpper(), pg.UniqueNumber));
                                }
                            }

                            //SET PAYMENT METHOD TYPE TO 'WARRANT'
                            DataAccess.PaymentDataDbMgr.UpdatePaymentRecordPmtMethodType(paymentSetList_NonEFT, paymentMethodType_WARRANT.ID);

                            //SET 'UNASSIGNED' STATUS to ALL NON-EFT payment records in this transaction
                            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("UNASSIGNED");
                            CommonStatus outcome = DataAccess.PaymentDataDbMgr.SetPaymentGroupStatus(statusType, paymentSetList_NonEFT, "system", string.Empty);

                            if (!outcome.Status)
                            {
                                throw new Exception(outcome.GetFirstDetailMessage() + "; " + String.Join(",", paymentSetList_NonEFT));
                            }
                        }
                    }
                    cs.Status = true;

                    //CAPTURE LOGGING FOR FTSAS 
                    if (logging_EFTEntityCodeList.Count() > 0) cs.AddMessageDetails(logging_EFTEntityCodeList);
                    if (logging_WarrantEntityCodeList.Count() > 0) cs.AddMessageDetails(logging_WarrantEntityCodeList);
                    if (logging_EFTPaymentSetList.Count() > 0) cs.AddMessageDetails(logging_EFTPaymentSetList);
                    if (logging_WarrantPaymentSetList.Count() > 0) cs.AddMessageDetails(logging_WarrantPaymentSetList);

                }
                else
                {
                    foreach (EFTEntityBankInfoResponse err_resp in errorResponse)
                    {
                        cs.AddMessageDetail(err_resp.StatusNote);
                        cs.AddMessageDetail(string.Format("{0};{1};{2}", err_resp.SysNameCode, err_resp.EntityCode, err_resp.EntityEIN));
                    }
                }
            }
            catch (Exception ex)
            {
                cs.AddMessageDetail(ex.Message);
                cs.AddMessageDetail(ex.StackTrace);
                EAMILogger.Instance.Error(ex);
            }

            return cs;
        }

        private CommonStatusPayload<List<RefCode>> DetermineEFTStatusWithFTSASStaticArgs()
        {
            List<RefCode> retValue = new List<RefCode>();
            CommonStatusPayload<List<RefCode>> cs = new CommonStatusPayload<List<RefCode>>(retValue, false);

            string peeCode = Context.GetExecutionArgTextValueByKey("PEE_CODE");
            string peeEin = Context.GetExecutionArgTextValueByKey("PEE_EIN");
            string fiRoutingNbr = Context.GetExecutionArgTextValueByKey("FI_ROUTING_NBR");
            string fiAccountType = Context.GetExecutionArgTextValueByKey("FI_ACCOUNT_TYPE");
            string accountNbr = Context.GetExecutionArgTextValueByKey("ACCOUNT_NBR");
            string prenotedDate = Context.GetExecutionArgTextValueByKey("DATE_PRENOTED");

            List<PaymentGroup> groupedPymtSetEntityList = tranPGList.GroupBy(t => t.PayeeInfo.PEE.Code).Select(t2 => t2.First()).ToList();
            // here go through payment groups/sets with distinct vendor entities
            foreach (PaymentGroup pg in groupedPymtSetEntityList)
            {
                if (pg.PayeeInfo.PEE.EntityEIN == peeEin && pg.PayeeInfo.PEE.Code == peeCode)
                {
                    PaymentDataDbMgr.InsertEFTInfo(sysId, pg.PayeeInfo.PEE.ID, tranMsgId, fiRoutingNbr, fiAccountType, accountNbr, DateTime.Parse(prenotedDate));
                    retValue.Add(pg.PayeeInfo.PEE);
                }
            }
            return cs;
        }

        private CommonStatusPayload<List<RefCode>> SetPaymentsToWarrantPaymentMethodType()
        {
            List<RefCode> retValue = new List<RefCode>();
            CommonStatusPayload<List<RefCode>> cs = new CommonStatusPayload<List<RefCode>>(retValue, false);
            List<string> paymentSetList_Warrant = new List<string>();

            try
            {
                cs.AddMessageDetail(string.Format("Transaction ID: {0}", tranMsgId));
                cs.AddMessageDetail("PROCESS_EFT_PAYMENTS=False");
                cs.AddMessageDetail("PROCESS_WARRANT_PAYMENTS=True");

                RefCode paymentMethodType_WARRANT = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_METHOD_TYPE).GetRefCodeByCode("WARRANT");

                //CAPTURE PAYMENT SET NUMBERS (UniqueNumber)
                List<string> groupedPymtSetNumberList = tranPGList.Select(_ => _.UniqueNumber).ToList();

                cs.AddMessageDetail(string.Format("NON-EFT Vendor Count: {0}", groupedPymtSetNumberList.Count()));

                //SET PAYMENT METHOD TYPE TO 'WARRANT'
                DataAccess.PaymentDataDbMgr.UpdatePaymentRecordPmtMethodType(groupedPymtSetNumberList, paymentMethodType_WARRANT.ID);

                //SET 'UNASSIGNED' STATUS to ALL WARRANT payment records in this transaction
                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("UNASSIGNED");
                CommonStatus outcome = DataAccess.PaymentDataDbMgr.SetPaymentGroupStatus(statusType, groupedPymtSetNumberList, "system", string.Empty);

                if (!outcome.Status)
                {
                    throw new Exception(outcome.GetFirstDetailMessage() + "; " + String.Join(",", groupedPymtSetNumberList));
                }

                cs.Status = true;

            }
            catch (Exception ex)
            {
                cs.AddMessageDetail(ex.Message);
                cs.AddMessageDetail(ex.StackTrace);
                EAMILogger.Instance.Error(ex);
            }

            return cs;
        }

        private bool SendEmail(string msg, ITaskActionContext context)
        {
            string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            string recipientAddr = context.GetExecutionArgTextValueByKey("NEW_PYMT_SUBMISSION_EMAIL_GRP");

            string[] email_to = recipientAddr.Replace(" ", string.Empty).TrimEnd(';').Split(';'); // new string[] { recipientAddr };
            string[] email_to_cc = new string[] { "" };
            string[] email_to_bcc = new string[] { "" };

            string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
            string subject = "EAMI(" + progName + ") NOTIFICATION - NEW PAYMENT SUBMISSION RECEIVED " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")");
            string message = msg; // "EAMI email Test";

            int portID = 0;
            bool useDefaultCredentials = true;

            EmailAccess ea = EmailAccess.GetInstance(
                    serverName,
                    portID,
                    useDefaultCredentials
                );
            return ea.SendMessage(email_to, email_to_cc, email_to_bcc, email_from, subject, message);
        }


        private string CreateMessageContent(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();

            int tranId = int.Parse(dr["Transaction_ID"].ToString());
            string tranMsgId = dr["Msg_Transaction_ID"].ToString();
            DateTime submissionDate = DateTime.Parse(dr["SubmissionDateTime"].ToString());
            string statusCode = dr["StatusCode"].ToString();
            string systemCode = dr["SystemCode"].ToString();
            string tranTypeCode = dr["TransactionType"].ToString();
            int prCount = int.Parse(dr["PymtRecCount"].ToString());
            int psCount = int.Parse(dr["PymtSetCount"].ToString());

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - NEW PAYMENT SUBMISSION RECEIVED " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 12;\" width = \"100%\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PROGRAM :</td>");
            sb.AppendLine("<td>" + systemCode + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> TRANSACTION NBR:</td>");
            sb.AppendLine("<td>" + tranMsgId + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> SUBMISSION TIME:</td>");
            sb.AppendLine("<td>" + submissionDate.ToString() + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PAYMENT SET/REC CNT:</td>");
            sb.AppendLine("<td>" + psCount.ToString() + "/" + prCount.ToString() + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 13px;\" width = \"550\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">PAYMENT SET #:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"7 %\">AMOUNT:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">VENDOR CODE:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"4 %\">EFT:</td>");
            sb.AppendLine("</tr>");

            foreach (PaymentGroup pg in tranPGList)
            {
                bool isEft = eftPeeList.FirstOrDefault(t => t.Code == pg.PayeeInfo.PEE.Code) != null;
                sb.AppendLine("<tr>");
                sb.AppendLine("<td width = \"5 %\">" + pg.UniqueNumber + "</td>");
                sb.AppendLine("<td align=\"right\" style=\"padding-right: 30px\" width = \"7 %\">$ " + string.Format("{0:n}", pg.Amount) + "</td>");
                sb.AppendLine("<td width = \"5 %\">" + pg.PayeeInfo.PEE.Code + "</td>");
                sb.AppendLine("<td width = \"4 %\">" + (isEft ? "Yes" : "No") + "</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("PLEASE DO NOT REPLY TO THIS EMAIL");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

    }
}
