using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.FTSASTaskActions;
using System.Collections.Generic;


namespace OHC.EAMI.AppServiceManager.Test.TaskActions
{
    [TestClass]
    public class ActionEcsFileGenerateTest
    {
        [TestMethod]
        public void CreateEFTFile()
        {

            Dictionary<string, string> execArgs = new Dictionary<string, string>();
            execArgs.Add("PROG_NAME", "MCOD");
            execArgs.Add("SYS_NAME", "CAPMAN");
            execArgs.Add("SMTP_SERVER", "smtpoutbound.dhs.ca.gov");
            execArgs.Add("EMAIL_GROUP", "genady.gidenko@dhcs.ca.gov");
            execArgs.Add("REJECT_PENDING_REQUEST", "True");
            execArgs.Add("REJECT_SENT_TO_SCO_REQUEST", "True");
            execArgs.Add("SENT_TO_SCO_FILE_NAME", "D011623G.MCOD.PRENOTE");
            execArgs.Add("ENTITY_CODE", "0000100409");


            //RejectPrenote pe = new RejectPrenote();
            //pe.Context = new MockTaskActionContext(execArgs);
            //TaskResult result = pe.Execute();

            //Assert.IsNotNull(result);

            ActionEftFileGenerate ecs = new ActionEftFileGenerate();
            ecs.Context = new MockTaskActionContext(execArgs);

            FTSAS.Integration.TaskResult result = ecs.Execute();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateWarrantFile()
        {
            ActionWarrantFileGenerate ecs = new ActionWarrantFileGenerate();
            FTSAS.Integration.TaskResult result = ecs.Execute();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DetermineEFT()
        {
            //"NET - NAMED - PIPE - BINDING"
            //    "SVC_URI" =
            //    "SYS_NAME" = MEDICAL_RX

            //ActionDeterminePEE_EFT eft = new ActionDeterminePEE_EFT();
            //FTSAS.Integration.TaskResult result = eft.Execute();
            //Assert.IsNotNull(result);
        }

    }
}

