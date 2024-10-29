using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;
using OHC.EAMI.FTSASTaskActions;
using FTSAS.Integration;


namespace OHC.EAMI.FTSASTaskActions.Test
{
    [TestClass]
    public class UnresolvedEcsNotificationTest
    {
        [TestMethod]
        public void ActionUnresolvedEcsNotificationTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();
            // MedRX data source arguments
            execArgs.Add("DATA_SOURCE_KEY", "EAMI-RX-Data");
            execArgs.Add("PROG_NAME", "MEDICAL_RX");
            execArgs.Add("DAYS_PASSED", "1");
            execArgs.Add("UNRECONCILED_ECS_EMAIL_GROUP", "eugene.samoylovich@dhcs.ca.gov;");

            ActionUnresolvedEcsNotification action = new ActionUnresolvedEcsNotification();
            action.Context = new MockTaskActionContext(execArgs);
            TaskResult result = action.Execute();

            Assert.IsNotNull(result);
        }
    }
}
