using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using FTSAS.Integration;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.ServiceManager;
using System.IO;
using OHC.EAMI.Util;
using OHC.EAMI.DataAccess;

namespace OHC.EAMI.FTSASTaskActions
{
    [TaskActionAttribute(ActionName = "ImportPaymentFile", IsSingleTaskDefinition = false)]
    public class ActionImportPaymentFile : FileActionBase
    {
        private bool isProductionEnvironment = false;
        private string hostName = string.Empty;
        private string progName = string.Empty;
        private string sysName = string.Empty;

        public override TaskResult Execute()
        {            
            progName = Context.GetExecutionArgTextValueByKey("PROG_NAME");
            sysName = Context.GetExecutionArgTextValueByKey("SYS_NAME");
            EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
            isProductionEnvironment = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
            hostName = System.Environment.MachineName;

            TaskResult tr = new TaskResult(true);
            tr.Outcome = enProductiveOutcome.FULL;

            string fileName = Context.ReceiveFilePath + "\\" + Context.ReceiveFileName;
            if (!File.Exists(fileName))
            {
                tr.Status = false;
                tr.Outcome = enProductiveOutcome.NONE;
                tr.Note = "File does not exist to execute this task.  " + fileName;
                return tr;
            }

            string fileNameToBe = Context.ReceiveFileNameOriginal;

            try
            {
                string xml = File.ReadAllText(fileName);
                PaymentSubmissionRequest request = EAMIServiceManager.Deserialize(xml, typeof(PaymentSubmissionRequest)) as PaymentSubmissionRequest;
                EAMIServiceManager sm = new EAMIServiceManager();
                PaymentSubmissionResponse response = sm.EAMIPaymentSubmission(request);

                string message = this.CreateMessageContent(Context, response);
                bool success = this.SendEmail(message, Context);

                // loop through each distenation item and fire push-file event              
                //foreach (KeyValuePair<string, string> kvpDest in Context.DestinationPathList)
                //{
                //    PushDestinationFileArgs arg = new PushDestinationFileArgs(
                //                                    kvpDest.Key,
                //                                    fileNameToBe,
                //                                    string.Concat(Context.ReceiveFilePath, "\\", Context.ReceiveFileName));
                //    this.OnPushDestinationFile(arg);
                //}

                return tr;
            }
            catch (Exception ex)
            {
                tr.Status = false;
                tr.Note = ex.Message;
                tr.Outcome = enProductiveOutcome.NONE;
                return tr;
            }
        }


        private bool SendEmail(string msg, IFileActionContext context)
        {
            string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            string recipientAddr = context.GetExecutionArgTextValueByKey("PYMT_SUBMISSION_RESP_EMAIL_GRP");

            string[] email_to = recipientAddr.Replace(" ", string.Empty).TrimEnd(';').Split(';'); // new string[] { recipientAddr };
            string[] email_to_cc = new string[] { "" };
            string[] email_to_bcc = new string[] { "" };

            string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
            string subject = "EAMI(" + progName + ") NOTIFICATION - PAYMENT SUBMISSION RESPONSE " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")");
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


        private string CreateMessageContent(IFileActionContext context, PaymentSubmissionResponse response)
        {
            StringBuilder sb = new StringBuilder();

            string acceptedRecCount = response.PaymentRecordStatuList.FindAll(item => item.StatusCode == "Accepted").Count().ToString();
            string rejectedRecCount = response.PaymentRecordStatuList.FindAll(item => item.StatusCode == "RejectedFD").Count().ToString();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - PAYMENT SUBMISSION RESPONSE" + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 12;\" width = \"100%\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PROGRAM :</td>");
            sb.AppendLine("<td>" + sysName + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PAYMENT FILE:</td>");
            sb.AppendLine("<td>" + Context.ReceiveFileName + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> TRANSACTION NBR:</td>");
            sb.AppendLine("<td>" + response.TransactionID + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> SUBMISSION TIME:</td>");
            sb.AppendLine("<td>" + response.TimeStamp.ToString() + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> SUBMISSION STATUS:</td>");
            sb.AppendLine("<td>" + response.ResponseStatusCode + "</td>");
            sb.AppendLine("</tr>");

            if (!string.IsNullOrWhiteSpace(response.ResponseMessage))
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td width = \"22 %\"> STATUS MSG:</td>");
                sb.AppendLine("<td>" + response.ResponseMessage + "</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> TOTAL PAYMENT REC COUNT:</td>");
            sb.AppendLine("<td>" + response.PaymentRecordStatuList.Count.ToString() + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> ACCEPTED/REJETED REC COUNT:</td>");
            sb.AppendLine("<td>" + acceptedRecCount + " / " + rejectedRecCount + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 13px;\" width = \"550\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">PAYMENT REC #:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"7 %\">STATUS:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"7 %\">STATUS NOTE:</td>");
            //sb.AppendLine("<td style=\"background-color: wheat;\" width = \"4 %\">EFT:</td>");
            sb.AppendLine("</tr>");

            foreach (PaymentRecordStatus prs in response.PaymentRecordStatuList)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td width = \"5 %\">" + prs.PaymentRecNumber + "</td>");
                sb.AppendLine("<td width = \"7 %\">" + prs.StatusCode + "</td>");
                sb.AppendLine("<td width = \"7 %\">" + prs.StatusNote + "</td>");
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

