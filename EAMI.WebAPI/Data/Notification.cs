using Microsoft.AspNetCore.Http.Extensions;
using OHC.EAMI.Common;
using OHC.EAMI.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace EAMI.Data
{
    public class Notification
    {
        private static readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Notification(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        static Notification()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public void SendErrorEmail(Exception exception)
        {
            try
            {
                List<string> recipientEmailList = new List<string>();
                string strEmailTo = Configuration["DHCS_EAMI_Prod_Support_Email"];
                recipientEmailList = strEmailTo.Trim().ToLower().Split(';').ToList();

                if (recipientEmailList.Count == 0)
                {
                    //EAMILogger.Instance.Error(exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace);
                }
                else
                {
                    if (bool.Parse(Configuration["errorEmailNotificationOn"]))
                    {
                        bool isProdEnv = bool.Parse(Configuration["IsProductionEnvironment"]);
                        string serverName = "smtpoutbound.dhs.ca.gov";
                        string email_from = "noReply@dhcs.ca.gov";
                        string subject = "EAMI NOTIFICATION - ERROR OCCURRED " + (isProdEnv ? string.Empty : " (" + Environment.MachineName + ")");

                        int portID = 0;
                        bool useDefaultCredentials = true;

                        string[] email_to = recipientEmailList.ToArray();
                        string[] email_to_cc = new string[] { "" };
                        string[] email_to_bcc = new string[] { "" };

                        StringBuilder sb = new StringBuilder();
                        var context = _httpContextAccessor.HttpContext;
                        sb.AppendLine("The following EAMI error occurred at " + DateTime.Now.ToString() + ":");
                        sb.AppendLine();
                        sb.Append("Error occurred on URL:  " + context.Request.GetDisplayUrl().ToString() + Environment.NewLine);
                        sb.AppendLine();
                        sb.Append("Error triggered by User:  " + context.User.Identity.Name.ToString() + Environment.NewLine);
                        sb.AppendLine();
                        sb.Append("Error Message:  " + Environment.NewLine);
                        sb.AppendLine(exception.ToString());
                        sb.AppendLine();
                        sb.AppendLine("Please do not reply to this email.");

                        EmailAccess ea = EmailAccess.GetInstance(
                            serverName,
                            portID,
                            useDefaultCredentials
                        );

                        bool success = ea.SendMessage(email_to, email_to_cc, email_to_bcc, email_from, subject, sb.ToString(), false);
                        if (!success)
                        {
                            StringBuilder sbError = new StringBuilder();
                            sbError.AppendLine("Error sending notification email of EAMI exception.");
                            sbError.AppendLine("  EmailTo: " + string.Join(",", email_to));
                            sbError.AppendLine("  Subject: " + subject);
                            sbError.AppendLine("  Message: " + sb.ToString());
                            throw new Exception(sbError.ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}