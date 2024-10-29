using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using OHC.EAMI.Common;

namespace OHC.EAMI.Util
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EmailAccess
    {
        #region | Variables |
        private static volatile EmailAccess instance;
        private static object syncRoot = new object();
        private static SmtpClient smtp = null;

        #endregion

        #region | Methods |
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="useDefaultCredentials"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        private EmailAccess(string serverName, bool useDefaultCredentials = true, string userName = "",
            string password = "", int port = 0)
        {
            if (!string.IsNullOrEmpty(serverName))
            {
                smtp = new SmtpClient(serverName);

                if (port > 0)
                    smtp.Port = port;

                smtp.UseDefaultCredentials = useDefaultCredentials;

                if (!useDefaultCredentials)
                {
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                }
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            }
        }

        private SmtpClient SmtpClientDeepCopy(SmtpClient smtp)
        {
            SmtpClient smtp2 = new SmtpClient();
            smtp2.Host = smtp.Host;
            smtp2.Port = smtp.Port;
            smtp2.UseDefaultCredentials = smtp.UseDefaultCredentials;
            smtp2.Credentials = smtp.Credentials;
            smtp2.DeliveryMethod = smtp.DeliveryMethod;

            return smtp2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="port"></param>
        /// <param name="useDefaultCredentials"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static EmailAccess GetInstance(string serverName, int port, bool useDefaultCredentials = true, string userName = "",
            string password = "")
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new EmailAccess(serverName, useDefaultCredentials, userName, password, port);
                }
            }

            return instance;
        }

        /// <summary>
        /// Checks to see if all of the emails in an array are avalid while list can not be empty
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public void IsEmailListValid(string[] strIn, out string strGoodEmails, out string strBadEmails)
        {
            StringBuilder strValidEmails = new StringBuilder();
            StringBuilder strInValidEmails = new StringBuilder();

            strGoodEmails = String.Empty;
            strBadEmails = String.Empty;  

            if (strIn != null && strIn.Count() > 0)
            {
                foreach (string email in strIn)
                {
                    if (ValidateEmail.Instance.IsValidEmail(email))
                    {
                        strValidEmails.Append(email + ";");
                    }
                    else
                    {
                        strInValidEmails.Append(email + ";");
                    }
                }
                strGoodEmails = strValidEmails.ToString();
                strBadEmails = strInValidEmails.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmails">This list must contain all valid email(s)</param>
        /// <param name="cc">This list could contain invalid email and will not break</param>
        /// <param name="bcc">This list could contain invalid email and will not break</param>
        /// <param name="fromEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendMessage(string[] toEmails, string[] cc, string[] bcc, string fromEmail, string subject, string body, bool isBodyHTML = true)
        {
            bool wasEmailSendSuccessful = false;
            string strGoodEmails, strBadEmails = string.Empty;
            IsEmailListValid(toEmails, out strGoodEmails, out strBadEmails);

            if (!string.IsNullOrEmpty(strBadEmails))
            {
                //log message that email was not sent to specific invalid email addresses...
                EAMILogger.Instance.Error("ERROR: Cannot send notification to recipent email => " + strBadEmails + ". This is due to one or more invalid email address(s).");
            }
            if (!string.IsNullOrEmpty(strGoodEmails) 
                && ValidateEmail.Instance.IsValidEmail(fromEmail))
            {
                try
                {
                    MailMessage message = new MailMessage();

                    strGoodEmails.Replace(" ", string.Empty).TrimEnd(';').Split(';').ToList().ForEach(toEmailAddress => message.To.Add(new MailAddress(toEmailAddress)));
                    message.From = new MailAddress(fromEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isBodyHTML;

                    if (cc != null && cc.Any())
                    {
                        foreach (string email in cc)
                        {
                            if (ValidateEmail.Instance.IsValidEmail(email))
                                message.CC.Add(new MailAddress(email));
                        }
                    }

                    if (bcc != null && bcc.Any())
                    {
                        foreach (string email in bcc)
                        {
                            if (ValidateEmail.Instance.IsValidEmail(email))
                                message.Bcc.Add(new MailAddress(email));
                        }
                    }

                    // Smtp server needs a different copy of SmtpClient for each Send to for concurrent email sends.
                    // Otherwise, subsequent emails will not be sent until prior email send completes.
                    SmtpClient smtpDC = SmtpClientDeepCopy(smtp);

                    if (smtpDC != null)
                    {
                        smtpDC.Send(message);
                    }
                    else
                    {
                        EAMILogger.Instance.Error("SmtpClient is Null");
                    }

                    wasEmailSendSuccessful = true;
                }
                catch (Exception ex)
                {
                    EAMILogger.Instance.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                    wasEmailSendSuccessful = false;
                }
            }

            return wasEmailSendSuccessful;
        }

        #endregion
    }
}


