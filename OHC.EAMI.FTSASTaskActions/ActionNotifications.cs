using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using TSAE.Entity;
using FTSAS.Integration;

using OHC.EAMI.Common;
using OHC.EAMI.DataAccess;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Util;
using System.Configuration;
using System.Data;

namespace OHC.EAMI.FTSASTaskActions
{

    #region ActionNewPaymentSubmissionNotification
    [TaskActionAttribute(ActionName="NewPaymentSubmissionNotification", IsPhantom = true)]
    public class ActionNewPaymentSubmissionNotification : TaskActionBase
    {
        private bool isProductionEnvironment = false;
        private string hostName = string.Empty;
        private string progName = string.Empty;
        private List<int> tranIdList = new List<int>();

        public override TaskResult Execute()
        {            
            TaskResult tr = new TaskResult(true);
            try
            {
                EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                progName = Context.GetExecutionArgTextValueByKey("PROG_NAME");
                isProductionEnvironment = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                hostName = System.Environment.MachineName;
                DataSet ds = PaymentDataSubmissionDBMgr.GetPymtSubmissionTransForNotification();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string message = this.CreateMessageContent(ds.Tables[0]);
                    bool success = this.SendEmail(message, Context);
                    if (success)
                    {
                        PaymentDataSubmissionDBMgr.UpdatePymtSubmissionTransNonificationFlag(tranIdList);
                        tr.Outcome = enProductiveOutcome.FULL;
                    }
                    else
                    {
                        tr.Status = false;
                        tr.Outcome = enProductiveOutcome.NONE;
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


        private string CreateMessageContent(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - NEW PAYMENT SUBMISSION RECEIVED " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 13px;\" width = \"500\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">TRANSACTION:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"7 %\">SUBMISSION DATE:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"7 %\">PYMT SET/REC COUNT:</td>");
            sb.AppendLine("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
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


        private string GetDetailHTML(DataRow dr)
        {            
            int tranId = int.Parse(dr["Transaction_ID"].ToString());
            tranIdList.Add(tranId);
            string tranMsgId = dr["Msg_Transaction_ID"].ToString();
            DateTime submissionDate = DateTime.Parse(dr["SubmissionDateTime"].ToString());
            string statusCode = dr["StatusCode"].ToString();
            string systemCode = dr["SystemCode"].ToString();
            string tranTypeCode = dr["TransactionType"].ToString();
            int prCount = int.Parse(dr["PymtRecCount"].ToString());
            int psCount = int.Parse(dr["PymtSetCount"].ToString());

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"5 %\">" + tranId + tranMsgId.Substring(23) + "</td>");
            sb.AppendLine("<td width = \"7 %\">" + submissionDate.ToString() + "</td>");
            //sb.AppendLine("<td align=\"right\" style=\"padding-right: 30px\" width = \"8 %\">$ " + amount + "</td>");
            sb.AppendLine("<td width = \"7 %\">" + psCount.ToString() + "/" + prCount.ToString() + "</td>");
            sb.AppendLine("</tr>");


            return sb.ToString();
        }

        /*
        private string CreateMessageContent(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - NEW PAYMENT SUBMISSION RECEIVED " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
            }

            sb.AppendLine("<br/>");
            sb.AppendLine("PLEASE DO NOT REPLY TO THIS EMAIL");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string GetDetailHTML(DataRow dr)
        {  
            int tranId = int.Parse(dr["Transaction_ID"].ToString());
            tranIdList.Add(tranId);
            string tranMsgId = dr["Msg_Transaction_ID"].ToString();
            DateTime submissionDate = DateTime.Parse(dr["SubmissionDateTime"].ToString());
            string statusCode = dr["StatusCode"].ToString();
            string systemCode = dr["SystemCode"].ToString();
            string tranTypeCode = dr["TransactionType"].ToString();
            int prCount = int.Parse(dr["PymtRecCount"].ToString());
            int psCount = int.Parse(dr["PymtSetCount"].ToString());

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 12;\" width = \"100%\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PYMT SYSTEM:</td>");
            sb.AppendLine("<td>" + systemCode + "</td>");
            sb.AppendLine("</tr>");
            
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> SUBMISSION TIME:</td>");
            sb.AppendLine("<td>" + submissionDate.ToString() + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PYMT SET COUNT:</td>");
            sb.AppendLine("<td>" + psCount.ToString() + "</td>");
            sb.AppendLine("</tr>");
                       
            sb.AppendLine("</table>");
            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<br/>");
            
            return sb.ToString();
        }
        */

    }
    #endregion


    #region ActionOnHoldPaymentSetsNotification
    [TaskActionAttribute(ActionName="OnHoldPaymentSetsNotification", IsPhantom = true)]
    public class ActionOnHoldPaymentSetsNotification : TaskActionBase
    {

        private bool isProductionEnvironment = false;
        private string hostName = string.Empty;
        private string progName = string.Empty;

        public override TaskResult Execute()
        {                        
            TaskResult tr = new TaskResult(true);
            try
            {
                EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                progName = Context.GetExecutionArgTextValueByKey("PROG_NAME");

                isProductionEnvironment = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                int daysPassed = Convert.ToInt32(Context.GetExecutionArgTextValueByKey("DAYS_PASSED"));
                //Test
                //int daysPassed = 1;

                hostName = System.Environment.MachineName;
                DataSet ds = PaymentDataDbMgr.GetPymtOnHoldForNotification(daysPassed);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string message = this.CreateMessageContent(ds.Tables[0], daysPassed);
                    bool success = this.SendEmail(message, Context, daysPassed);
                    if (success)
                    {
                        tr.Outcome = enProductiveOutcome.FULL;
                    }
                    else
                    {
                        tr.Status = false;
                        tr.Outcome = enProductiveOutcome.NONE;
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


        private bool SendEmail(string msg, ITaskActionContext context, int daysPassed)
        {
            string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            string recipientAddr = context.GetExecutionArgTextValueByKey("PYMT_ONHOLD_EMAIL_GROUP");
            //Test
            //string recipientAddr = "alex.hoang@dhcs.ca.gov";

            string[] email_to = recipientAddr.Replace(" ", string.Empty).TrimEnd(';').Split(';'); // new string[] { recipientAddr };
            string[] email_to_cc = new string[] { "" };
            string[] email_to_bcc = new string[] { "" };

            string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
            string subject = "EAMI(" + progName + ") NOTIFICATION - PAYMENTS ON HOLD OVER " + daysPassed + " DAYS " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")");
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


        private string CreateMessageContent(DataTable dt, int daysPassed)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - PAYMENTS ON HOLD OVER " + daysPassed + " DAYS " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 13px;\" width = \"900\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">PYMT SET NUMBER:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">CONTRACT NUMBER:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"6 %\">DATE PLACED ON HOLD:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">PLACED ON HOLD BY:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"10 %\">USER NOTE:</td>");
            sb.AppendLine("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
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


        private string GetDetailHTML(DataRow dr)
        {
            string pymtSetNum = dr["PaymentSet_Number"].ToString();
            string datePlacedOnHold = DateTime.Parse(dr["Status_Date"].ToString()).ToString();
            string note = dr["Status_Note"].ToString();
            string holdUser = dr["CreatedBy"].ToString();
            string contractNumber = dr["Entity_ContractNumber"].ToString();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"5 %\">" + pymtSetNum + "</td>");
            sb.AppendLine("<td width = \"5 %\">" + contractNumber + "</td>");
            sb.AppendLine("<td width = \"6 %\">" + datePlacedOnHold.ToString() + "</td>");
            sb.AppendLine("<td width = \"5 %\">" + holdUser + "</td>");
            sb.AppendLine("<td width = \"10 %\">" + note + "</td>");
            //sb.AppendLine("<td align=\"right\" style=\"padding-right: 30px\" width = \"8 %\">$ " + amount + "</td>");
            //sb.AppendLine("<td width = \"8 %\">" + psCount.ToString() + "/" + prCount.ToString() + "</td>");
            sb.AppendLine("</tr>");

            return sb.ToString();
        }



        /*
        private string CreateMessageContent(DataTable dt, int daysPassed)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - PAYMENTS ON HOLD OVER " + daysPassed + " DAYS " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
            }

            sb.AppendLine("<br/>");
            sb.AppendLine("PLEASE DO NOT REPLY TO THIS EMAIL");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }        


        private string GetDetailHTML(DataRow dr)
        {
            string pymtSetNum = dr["PaymentSet_Number"].ToString();
            string datePlacedOnHold = DateTime.Parse(dr["Status_Date"].ToString()).ToString();
            string note = dr["Status_Note"].ToString();
            string holdUser = dr["CreatedBy"].ToString();
            string contractNumber = dr["Entity_ContractNumber"].ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 12;\" width = \"100%\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PYMT SET NUMBER:</td>");
            sb.AppendLine("<td>" + pymtSetNum + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> CONTRACT NUMBER:</td>");
            sb.AppendLine("<td>" + contractNumber + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> DATE PLACED ON HOLD:</td>");
            sb.AppendLine("<td>" + datePlacedOnHold + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PLACED ON HOLD BY:</td>");
            sb.AppendLine("<td>" + holdUser + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> USER NOTE:</td>");
            sb.AppendLine("<td>" + note + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<br/>");

            return sb.ToString();
        }
        */
    }
    #endregion


    #region ActionUnresolvedEcsNotification
    [TaskActionAttribute(ActionName = "UnresolvedEcsNotification", IsPhantom = true)]
    public class ActionUnresolvedEcsNotification : TaskActionBase
    {
        private bool isProductionEnvironment = false;
        private string hostName = string.Empty;
        private string progName = string.Empty;

        public override TaskResult Execute()
        {


            TaskResult tr = new TaskResult(true);
            try
            {
                EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                progName = Context.GetExecutionArgTextValueByKey("PROG_NAME");

                isProductionEnvironment = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                int daysPassed = Convert.ToInt32(Context.GetExecutionArgTextValueByKey("DAYS_PASSED"));
                //Test
                //int daysPassed = 1;

                hostName = System.Environment.MachineName;
                DataSet ds = PaymentDataDbMgr.GetUnresolvedEcsForNotification(daysPassed);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string message = this.CreateMessageContent(ds.Tables[0], daysPassed);
                    bool success = this.SendEmail(message, Context, daysPassed);
                    if (success)
                    {
                        tr.Outcome = enProductiveOutcome.FULL;
                    }
                    else
                    {
                        tr.Status = false;
                        tr.Outcome = enProductiveOutcome.NONE;
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


        private bool SendEmail(string msg, ITaskActionContext context, int daysPassed)
        {
            string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            string recipientAddr = context.GetExecutionArgTextValueByKey("UNRECONCILED_ECS_EMAIL_GROUP");

            string[] email_to = recipientAddr.Replace(" ", string.Empty).TrimEnd(';').Split(';'); // new string[] { recipientAddr };
            string[] email_to_cc = new string[] { "" };
            string[] email_to_bcc = new string[] { "" };

            string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
            string subject = "EAMI(" + progName + ") NOTIFICATION - UNRECONCILED E-CS OVER " + daysPassed + " DAYS " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")");
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



        private string CreateMessageContent(DataTable dt, int daysPassed)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - UNRECONCILED E-CS OVER " + daysPassed + " DAYS " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 13px;\" width = \"550\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">E-CS NUMBER:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"8 %\">AMOUNT:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"8 %\">DATE SENT-TO-SCO:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"8 %\">PAY DATE:</td>");
            sb.AppendLine("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
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


        private string GetDetailHTML(DataRow dr)
        {
            string ecsNum = dr["ECS_Number"].ToString();
            //string amount = decimal.Parse(dr["Amount"].ToString()).ToString("0.00");
            string amount = string.Format("{0:n}", decimal.Parse(dr["Amount"].ToString()));
            string ecsFile = dr["ECS_File_Name"].ToString();
            DateTime payDate = DateTime.Parse(dr["PayDate"].ToString());
            DateTime approveDate = DateTime.Parse(dr["ApproveDate"].ToString());
            string approvedBy = dr["ApprovedBy"].ToString();
            DateTime sentToScoDate = DateTime.Parse(dr["SentToScoDate"].ToString());

            StringBuilder sb = new StringBuilder();


            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"5 %\">" + ecsNum + "</td>");
            sb.AppendLine("<td align=\"right\" style=\"padding-right: 30px\" width = \"8 %\">$ " + amount + "</td>");
            sb.AppendLine("<td width = \"8 %\">" + sentToScoDate.ToString() + "</td>");
            sb.AppendLine("<td width = \"6 %\">" + payDate.ToString() + "</td>");
            sb.AppendLine("</tr>");
           

            return sb.ToString();
        }



        /*
        private string CreateMessageContent(DataTable dt, int daysPassed)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI NOTIFICATION - UNRECONCILED E-CS OVER " + daysPassed + " DAYS " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
            }

            sb.AppendLine("<br/>");
            sb.AppendLine("PLEASE DO NOT REPLY TO THIS EMAIL");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }


        private string GetDetailHTML(DataRow dr)
        {
            string ecsNum = dr["ECS_Number"].ToString();
            //string amount = decimal.Parse(dr["Amount"].ToString()).ToString("0.00");
            string amount = string.Format("{0:n}", decimal.Parse(dr["Amount"].ToString()));
            string ecsFile = dr["ECS_File_Name"].ToString();
            DateTime payDate = DateTime.Parse(dr["PayDate"].ToString());
            DateTime approveDate = DateTime.Parse(dr["ApproveDate"].ToString());
            string approvedBy = dr["ApprovedBy"].ToString();
            DateTime sentToScoDate = DateTime.Parse(dr["SentToScoDate"].ToString());

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 12;\" width = \"100%\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> E-CS NUMBER:</td>");
            sb.AppendLine("<td>" + ecsNum + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> E-CS AMOUNT:</td>");
            sb.AppendLine("<td> $" + amount + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> DATE SENT-TO-SCO:</td>");
            sb.AppendLine("<td>" + sentToScoDate.ToString() + "</td>");
            sb.AppendLine("</tr>");            

            sb.AppendLine("</table>");
            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<br/>");

            return sb.ToString();
        }
        */
    }
    #endregion


    #region SCO File Transfer Notification
    [TaskActionAttribute(ActionName = "ScoFileTransferNotification", IsPhantom = true)]
    public class ActionScoFileTransferNotification : TaskActionBase
    {
        private bool isProductionEnvironment = false;
        private string hostName = string.Empty;
        private string progName = string.Empty;
        private List<int> tranIdList = new List<int>();

        public override TaskResult Execute()
        {
            TaskResult tr = new TaskResult(true);
            try
            {
                EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                progName = Context.GetExecutionArgTextValueByKey("PROG_NAME");
                isProductionEnvironment = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                hostName = System.Environment.MachineName;

                DataSet ds = PaymentDataSubmissionDBMgr.GetPymtSubmissionTransForNotification();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string message = this.CreateMessageContent(ds.Tables[0]);
                    bool success = this.SendEmail(message, Context);
                    if (success)
                    {
                        PaymentDataSubmissionDBMgr.UpdatePymtSubmissionTransNonificationFlag(tranIdList);
                        tr.Outcome = enProductiveOutcome.FULL;
                    }
                    else
                    {
                        tr.Status = false;
                        tr.Outcome = enProductiveOutcome.NONE;
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


        private bool SendEmail(string msg, ITaskActionContext context)
        {
            string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            string recipientAddr = context.GetExecutionArgTextValueByKey("SCO_FILE_TRANSFER_NOTIFICATION_EMAIL_GRP");

            string[] email_to = recipientAddr.Replace(" ", string.Empty).TrimEnd(';').Split(';'); // new string[] { recipientAddr };
            string[] email_to_cc = new string[] { "" };
            string[] email_to_bcc = new string[] { "" };

            string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
            string subject = "EAMI(" + progName + ") NOTIFICATION - FILE TRANSFERED TO SCO " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")");
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

        /* table content
        private string CreateMessageContent(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - NEW PAYMENT SUBMISSION RECEIVED " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 13px;\" width = \"550\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"5 %\">TRANSACTION:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"8 %\">SUBMISSION DATE:</td>");
            sb.AppendLine("<td style=\"background-color: wheat;\" width = \"8 %\">PYMT SET/REC COUNT:</td>");
            sb.AppendLine("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
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


        private string GetDetailHTML(DataRow dr)
        {
            int tranId = int.Parse(dr["Transaction_ID"].ToString());
            tranIdList.Add(tranId);
            string tranMsgId = dr["Msg_Transaction_ID"].ToString();
            DateTime submissionDate = DateTime.Parse(dr["SubmissionDateTime"].ToString());
            string statusCode = dr["StatusCode"].ToString();
            string systemCode = dr["SystemCode"].ToString();
            string tranTypeCode = dr["TransactionType"].ToString();
            int prCount = int.Parse(dr["PymtRecCount"].ToString());
            int psCount = int.Parse(dr["PymtSetCount"].ToString());

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"5 %\">" + tranMsgId + "</td>");
            sb.AppendLine("<td width = \"8 %\">" + submissionDate.ToString() + "</td>");
            //sb.AppendLine("<td align=\"right\" style=\"padding-right: 30px\" width = \"8 %\">$ " + amount + "</td>");
            sb.AppendLine("<td width = \"8 %\">" + psCount.ToString() + "/" + prCount.ToString() + "</td>");
            sb.AppendLine("</tr>");

            return sb.ToString();
        }
        */

        private string CreateMessageContent(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http - equiv = \"Content -Type\" content = \"text/html; charset=windows-1252\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("EAMI(" + progName + ") NOTIFICATION - FILE TRANSFERED TO SCO " + (isProductionEnvironment ? string.Empty : " (" + hostName + ")"));
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>");

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine(GetDetailHTML(dr));
            }

            sb.AppendLine("<br/>");
            sb.AppendLine("PLEASE DO NOT REPLY TO THIS EMAIL");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string GetDetailHTML(DataRow dr)
        {  
            /*
            int tranId = int.Parse(dr["Transaction_ID"].ToString());
            tranIdList.Add(tranId);
            string tranMsgId = dr["Msg_Transaction_ID"].ToString();
            DateTime submissionDate = DateTime.Parse(dr["SubmissionDateTime"].ToString());
            string statusCode = dr["StatusCode"].ToString();
            string systemCode = dr["SystemCode"].ToString();
            string tranTypeCode = dr["TransactionType"].ToString();
            int prCount = int.Parse(dr["PymtRecCount"].ToString());
            int psCount = int.Parse(dr["PymtSetCount"].ToString());
            */

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border = \"0\" style=\"color: darkslategray; font-size: 12;\" width = \"100%\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PYMT SYSTEM:</td>");
            sb.AppendLine("<td>" + "SYSTEM/PROGRAM NAME" + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PAYMENT FILES:</td>");
            sb.AppendLine("<td>" + "payment-file-1" + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"></td>");
            sb.AppendLine("<td>" + "payment-file-2" + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"></td>");
            sb.AppendLine("<td>" + "payment-file-3" + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> AUDIT FILES:</td>");
            sb.AppendLine("<td>" + "audit-file-1" + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"></td>");
            sb.AppendLine("<td>" + "audit-file-2" + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"></td>");
            sb.AppendLine("<td>" + "audit-file-3" + "</td>");
            sb.AppendLine("</tr>");


            /*
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PYMT SYSTEM:</td>");
            sb.AppendLine("<td>" + systemCode + "</td>");
            sb.AppendLine("</tr>");
            
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> SUBMISSION TIME:</td>");
            sb.AppendLine("<td>" + submissionDate.ToString() + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td width = \"22 %\"> PYMT SET COUNT:</td>");
            sb.AppendLine("<td>" + psCount.ToString() + "</td>");
            sb.AppendLine("</tr>");
            */

            sb.AppendLine("</table>");
            sb.AppendLine("<hr style = \"display: block; margin-top: 0.5em; margin-bottom: 0.5em; margin-left: auto; margin-right: auto; border-width: 0.5px; border-style: solid; border-color: lightgray;\">");
            sb.AppendLine("<br/>");
            
            return sb.ToString();
        }
        

    }
    #endregion




}
