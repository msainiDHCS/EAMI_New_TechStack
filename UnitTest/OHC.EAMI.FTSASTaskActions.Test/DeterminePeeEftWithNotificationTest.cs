using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

//using OHC.EAMI.CommonEntity;
//using OHC.EAMI.DataAccess;


using OHC.EAMI.FTSASTaskActions;
using FTSAS.Integration;

namespace OHC.EAMI.FTSASTaskActions.Test
{
    [TestClass]
    public class DeterminePeeEftWithNotificationTest
    {
        [TestMethod]
        public void ActionDeterminePeeEftWithNotificationTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();

            // MedRX data source arguments
            execArgs.Add("DATA_SOURCE_KEY", "EAMI-MC-Data");
            execArgs.Add("SYS_NAME", "CAPMAN");
            execArgs.Add("PROG_NAME", "MCOD");

            // DENTAL data source arguments
            //execArgs.Add("DATA_SOURCE_KEY", "EAMI-Dental-Data");
            //execArgs.Add("SYS_NAME", "MDSDFFS");
            //execArgs.Add("PROG_NAME", "MDSDFFS");

            // option to use FTSAS Argus or PRENOTE SVC for EFT check
            execArgs.Add("USE-ARGS-FOR-EFT-CHECK", "false");

            // PRENOTE SVS Arguments
            execArgs.Add("SVC_URI", "net.pipe://localhost/EftPrenoteService");
            execArgs.Add("NET-NAMED-PIPE-BINDING", "True");


            // DENTAL EFT Arguments
            //execArgs.Add("PEE_CODE", "12-34567");
            //execArgs.Add("PEE_EIN", "999999999");
            //execArgs.Add("FI_ROUTING_NBR", "123456789");
            //execArgs.Add("FI_ACCOUNT_TYPE", "Checking");
            //execArgs.Add("ACCOUNT_NBR", "9876543210");
            //execArgs.Add("DATE_PRENOTED", "2021-10-18");

            // MedRX EFT Arguments
            //execArgs.Add("PEE_CODE", "22334455");
            //execArgs.Add("PEE_EIN", "888888888");
            //execArgs.Add("FI_ROUTING_NBR", "353004166");
            //execArgs.Add("FI_ACCOUNT_TYPE", "Checking");
            //execArgs.Add("ACCOUNT_NBR", "4480250465");
            //execArgs.Add("DATE_PRENOTED", "2021-01-22");

            // email recepient argument
            execArgs.Add("NEW_PYMT_SUBMISSION_EMAIL_GRP", "genady.gidenko@dhcs.ca.gov");


            ActionDeterminePEE_EFT_withNotification action = new ActionDeterminePEE_EFT_withNotification();
            action.Context = new MockTaskActionContext(execArgs);
            TaskResult result = action.Execute();
            //TaskResult result2 = action.Execute();
            //TaskResult result3 = action.Execute();

            Assert.IsNotNull(result);

        }
    }
}
