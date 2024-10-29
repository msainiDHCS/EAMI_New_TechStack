using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

//using OHC.EAMI.CommonEntity;
//using OHC.EAMI.DataAccess;
//using OHC.EAMI.FTSASTaskActions;
//using FTSAS.Integration;

namespace OHC.EAMI.FTSASTaskActions.Test
{
    /* deprecated test code
    [TestClass]
    public class DeterminePeeEftSimTest
    {
        [TestMethod]
        public void ActionDeterminePeeEftSimTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();
            //execArgs.Add("DATA_SOURCE_KEY", "EAMI-Dental-Data");            
            //execArgs.Add("SYS_NAME", "MDSDFFS");
            //execArgs.Add("PEE_CODE", "12-34567");
            //execArgs.Add("PEE_EIN", "999999999");
            //execArgs.Add("FI_ROUTING_NBR", "123456789");
            //execArgs.Add("FI_ACCOUNT_TYPE", "Checking");
            //execArgs.Add("ACCOUNT_NBR", "9876543210");
            //execArgs.Add("DATE_PRENOTED", "2021-10-18");

            execArgs.Add("DATA_SOURCE_KEY", "EAMI-RX2-Data");
            execArgs.Add("SYS_NAME", "MEDICAL_RX");
            execArgs.Add("PEE_CODE", "22334455");
            execArgs.Add("PEE_EIN", "888888888");
            execArgs.Add("FI_ROUTING_NBR", "353004166");
            execArgs.Add("FI_ACCOUNT_TYPE", "Checking");
            execArgs.Add("ACCOUNT_NBR", "4480250465");
            execArgs.Add("DATE_PRENOTED", "2021-01-22");

            ActionDeterminePEE_EFT_SIM action = new ActionDeterminePEE_EFT_SIM();
            action.Context = new MockTaskActionContext(execArgs);
            TaskResult result = action.Execute();

            Assert.IsNotNull(result);

        }
    }
    */
}
