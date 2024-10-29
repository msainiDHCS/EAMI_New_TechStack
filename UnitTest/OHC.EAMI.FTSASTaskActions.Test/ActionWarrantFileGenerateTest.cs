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
    public class ActionWarrantFileGenerateTest
    {
        [TestMethod]
        public void WarrantFileGenerateTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();

            // MedRX data source arguments
            //execArgs.Add("DATA_SOURCE_KEY", "EAMI-MC-Data");
            //execArgs.Add("ROOT_FOLDER_LOCATION", @"C:\EAMI");


            execArgs.Add("DATA_SOURCE_KEY", "EAMI-MC-Data");
            execArgs.Add("OUTPUT_FOLDER", @"C:\EAM\TEMP_SFTP\SND_TO_SCO\Warrant");
            execArgs.Add("ARCHIVE_FOLDER", @"C:\EAM\Archive\ECS_TO_SCO");

            ActionWarrantFileGenerate action = new ActionWarrantFileGenerate();
            action.Context = new MockTaskActionContext(execArgs);
            TaskResult result = action.Execute();

            Assert.IsNotNull(result);

        }
    }
}
