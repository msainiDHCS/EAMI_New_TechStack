using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.ServiceManager;
using System.Security.Permissions;
using System.Configuration;
using System.Security.Principal;
using OHC.EAMI.Common;

namespace OHC.EAMI.Service
{

    /* NOTE: Eugene S. 2022-01-27
     * the service needs to implement authorization or role based security when being used
     * original implementation for data service API used wsHttpsbinding 
     * but later switch to BasicHttpsBinding to integrate with AWS Messaging Solution (i.e. client build on .Net Core 3.1 which could not support wsHttpsBinding)
     * see more details on the difference between the two bindings:
     *  https://stackoverflow.com/questions/2106715/basichttpbinding-vs-wshttpbinding
     *  https://www.codeproject.com/Articles/36396/Difference-between-BasicHttpBinding-and-WsHttpBind
     *  
     *  with wsHttpsBinding - we are using service operator/method attribues (see commented out PrincipalPermission attr) 
     *  letting the .Net FW do the checking without custom code
     *  
     *  with basicHttpsBinding - we opted for a custom check of calling principal against a local server group which has 
     *  users that are allowed to access the service methods  
     */
     
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class EAMIServiceOperations : IEAMIServiceOperations
    {
        /// <summary>
        /// an entry point for EAMIPaymentSubmission operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[PrincipalPermission(SecurityAction.Demand, Role = "EAMIAppDataServiceUsers")]
        public virtual PaymentSubmissionResponse EAMIPaymentSubmission(PaymentSubmissionRequest request)
        {
            CheckAuthorization("EAMIPaymentSubmission()");
            return new EAMIServiceManager().EAMIPaymentSubmission(request);
        }

        /// <summary>
        /// an entry point for EAMIPaymentStatusInquiry operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[PrincipalPermission(SecurityAction.Demand, Role = "EAMIAppDataServiceUsers")]
        public virtual PaymentStatusInquiryResponse EAMIPaymentStatusInquiry(PaymentStatusInquiryRequest request)
        {
            CheckAuthorization("EAMIPaymentStatusInquiry()");
            return new EAMIServiceManager().EAMIPaymentStatusInquiry(request);
        }

        /// <summary>
        /// an entry point for EAMIRejectedPaymentInquiry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[PrincipalPermission(SecurityAction.Demand, Role = "EAMIAppDataServiceUsers")]
        public virtual RejectedPaymentInquiryResponse EAMIRejectedPaymentInquiry(RejectedPaymentInquiryRequest request)
        {
            CheckAuthorization("EAMIPaymentStatusInquiry()");
            return new EAMIServiceManager().EAMIRejectedPaymentInquiry(request);
        }

        /// <summary>
        /// an entry point for Ping operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[PrincipalPermission(SecurityAction.Demand, Role = "EAMIAppDataServiceUsers")]
        public virtual PingResponse Ping(PingRequest request)
        {
            CheckAuthorization("Ping()");
            return new EAMIServiceManager().Ping(request);
        }


        #region data service authorization impalementation

        private void CheckAuthorization(string request = "")
         {
             string sscUserName = string.Empty;
             string userName = string.Empty;
             string upnUserName = string.Empty;
             try
             {
                 // check and exit if custom authorization is not enabled or app setting is missing
                 if (!bool.Parse(ConfigurationManager.AppSettings["DataSvcAuthorizationEnabled"].ToString()))
                 {
                     return;
                 }

                 sscUserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                 userName = sscUserName.Contains("\\") ? sscUserName.Substring(9) : sscUserName;
                 upnUserName = userName + "@intra.dhs.ca.gov";

                EAMILogger.Instance.Info("CheckAuthorization() for request coming from=> " + request + ". UserName=> " + upnUserName);
                // check against local group of EAMI Admins (including service account) who's authorized to access web service methods
                if (!IsInEAMIAdminGroup(upnUserName))
                 {
                    EAMILogger.Instance.Error("CheckAuthorization(): User doesn't belong to a specified Windows user group and cannot access the service => " + upnUserName);
                    throw new Exception();
                 }

                #region disabled Authorization against both the local admin group and EAMI app users
                /* 
                // this implementation checks against both local EAMI Admin group and EAMI application users
                // we are not using this for now because web server runs under service account whose identity
                // passed to the web service.
                if (!IsInEAMIAdminGroup(upnUserName) && !IsEAMIUser(userName))
                {
                    throw new Exception();
                }
                */
                #endregion
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("CheckAuthorization() => Access Denied");
                sb.AppendLine("sscUser : " + sscUserName);
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
                EAMILogger.Instance.Error(sb.ToString());
                throw new Exception("Access Denied");
            }
        }

        /// <summary>
        /// You are a member of the Built-in Administrators group, you are assigned two run-time access tokens: a standard user access token and an administrator access token. 
        /// By default, you are in the standard user role.
        /// When you attempt to perform a task that requires administrative privileges, you can dynamically elevate your role by using the Consent dialog box. 
        /// The code that executes the IsInRole method does not display the Consent dialog box. 
        /// The code returns false if you are in the standard user role, even if you are in the Built-in Administrators group.
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="group">Built-in Administrators group</param>
        /// <returns></returns>
        private bool IsInEAMIAdminGroup(string userName, string group = "EAMIAppDataServiceUsers")
        {
            // must use upn name
            using (WindowsIdentity identity = new WindowsIdentity(userName))
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(group);
            }
        }

        //public bool IsEAMIUser(string userName)
        //{
        //    return RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_USER).GetRefCodeByCode(userName) != null;
        //}


        #endregion

    }
}
