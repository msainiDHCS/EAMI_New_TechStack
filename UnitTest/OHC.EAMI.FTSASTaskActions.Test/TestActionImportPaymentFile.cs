using System;
using System.Collections.Generic;
using FTSAS.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.FTSASTaskActions;

namespace OHC.EAMI.FTSASTaskActions.Test
{
    [TestClass]
    public class TestActionImportPaymentFile
    { 
        [TestMethod]
        public void DownloadEnrollmentFileTest()
        {
            Dictionary<string, string> execArgs = new Dictionary<string, string>();

           
            execArgs.Add("DATA_SOURCE_KEY", "EAMI-MC-Data");
            execArgs.Add("SYS_NAME", "CAPMAN");
            execArgs.Add("PROG_NAME", "MCOD");
            execArgs.Add("PYMT_SUBMISSION_RESP_EMAIL_GRP", "gopalkrishna.pandey@dhcs.ca.gov");

            

            ActionImportPaymentFile action = new ActionImportPaymentFile();
            action.Context = new MockFIleActionContext(execArgs);
            TaskResult result = action.Execute();
            Assert.IsNotNull(result);
           
        }
    }

    public class MockFIleActionContext : IFileActionContext
    {
        public MockFIleActionContext(Dictionary<string, string> executionArgs)
        {
            this.ExecutionArguments = executionArgs;
            this.TaskNumber = string.Format("UNIT-TEST_{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
        }


        public string ApplicationName { get; private set; }
        public string AssemblyFileVersion { get; private set; }
        public string AssemblyName { get; private set; }
        public string ClassName { get; private set; }
        public int CurrentAttempt { get; private set; }
        public int ExecutionGroup { get; private set; }
        public int ExpirationTimeInMin { get; private set; }
        public bool IsAppendExecArgsToLinkedAction { get; private set; }
        public bool IsAppendOutputDetailsToLinkedAction { get; private set; }
        public bool IsLinkedToAnotherAction { get; private set; }
        public bool IsPhantom { get; private set; }
        public string LinkedActionName { get; private set; }
        public int MaxAttempts { get; private set; }
        public int MaxExecutionTimeInMin { get; private set; }
        public int Priority { get; private set; }
        public string ServiceInstanceHostName { get; private set; }
        public string ServiceInstanceName { get; private set; }
        public string TaskActionName { get; private set; }
        public string TaskNumber { get; private set; }
        public DateTime TimerTickTime { get; private set; }


        public Dictionary<string, string> ExecutionArguments { get; private set; }

        string IFileActionContext.ReceiveFileNameOriginal => "Vbethapu_PaymentSubmissionRequest_20231218-150519_Test2.xml";

        string IFileActionContext.ReceiveFileName => "Vbethapu_PaymentSubmissionRequest_20231218-150519_Test2.xml";

        string IFileActionContext.ReceiveFilePath => "C:\\Projects";

        long IFileActionContext.ReceiveFileSizeBytes => 0;

        List<string> IFileActionContext.DestinationPathList => null;


        /* NOTE: the bottom two methods mimic real context implementation for exec arguments */
        public string GetExecutionArgTextValueByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            string retValue = string.Empty;
            if (this.ExecutionArguments != null && this.ExecutionArguments.Count > 0)
            {
                retValue = this.ExecutionArguments[key];
            }

            return retValue;
        }

        public T GetExecutionArgValueByKey<T>(string key)
        {
            return (T)Convert.ChangeType(GetExecutionArgTextValueByKey(key), typeof(T));
        }
    }
}
