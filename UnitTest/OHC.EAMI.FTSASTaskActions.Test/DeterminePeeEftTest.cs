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
    public class DeterminePeeEftTest
    {
        
        [TestMethod]
        public void ActionDeterminePeeEftTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();

            execArgs.Add("DATA_SOURCE_KEY", "EAMI-RX2-Data");
            execArgs.Add("SYS_NAME", "MEDICAL_RX");

            //execArgs.Add("DATA_SOURCE_KEY", "EAMI-Dental-Data");            
            //execArgs.Add("SYS_NAME", "MDSDFFS");

            execArgs.Add("SVC_URI", "net.pipe://localhost/EftPrenoteService");
            execArgs.Add("NET-NAMED-PIPE-BINDING", "True");

            ActionDeterminePEE_EFT action = new ActionDeterminePEE_EFT();
            action.Context = new MockTaskActionContext(execArgs);
            TaskResult result = action.Execute();

            Assert.IsNotNull(result);

        }
    }
    */
}
