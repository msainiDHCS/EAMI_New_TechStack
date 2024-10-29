using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OHC.EAMI.FTSASTaskActions;
using FTSAS.Integration;

namespace OHC.EAMI.FTSASTaskActions.Test
{
    [TestClass]
    public class ActionEftFileGenerateTest
    {
        [TestMethod]
        public void EftFileGenerateTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();

            // MedRX data source arguments
            execArgs.Add("DATA_SOURCE_KEY", "EAMI-MC-Data");
            execArgs.Add("OUTPUT_FOLDER", @"C:\EAM\TEMP_SFTP\SND_TO_SCO\EFT");
            execArgs.Add("ARCHIVE_FOLDER", @"C:\EAM\Archive\ECS_TO_SCO");

            ActionEftFileGenerate action = new ActionEftFileGenerate();
            action.Context = new MockTaskActionContext(execArgs);
            TaskResult result = action.Execute();

            Assert.IsNotNull(result);

        }
    }
}
