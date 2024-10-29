using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OHC.EAMI.FTSASTaskActions;
using FTSAS;
using FTSAS.Integration;

namespace OHC.EAMI.AppServiceManager.Test.TaskActions
{
    [TestClass]
    public class ActionNotificationsTest
    {
        [TestMethod]
        public void OnHoldPaymentSetsNotification()
        {
            ActionOnHoldPaymentSetsNotification onHoldPaymentSetsNotification = new ActionOnHoldPaymentSetsNotification();
            TaskResult result = onHoldPaymentSetsNotification.Execute();

            Assert.IsNotNull(result);
        }

        


        [TestMethod]
        public void UnresolvedEcsNotification()
        {
            ActionUnresolvedEcsNotification unresolvedEcsNotification = new ActionUnresolvedEcsNotification();
            ITaskActionContext context = new TaskActionContextTest();
            //TaskResult result = unresolvedEcsNotification.Execute(context);
            TaskResult result = unresolvedEcsNotification.Execute();

            Assert.IsNotNull(result);
        }





        public class TaskActionContextTest : ITaskActionContext
        {

            public TaskActionContextTest()
            {
                this.ExecutionArguments = new Dictionary<string, string>();
                this.ExecutionArguments.Add("DAYS_PASSED", "1");
                //this.ExecutionArguments.Add("UNRECONCILED_ECS_EMAIL_GROUP", "Eugene.Samoylovich@dhcs.ca.gov; ");

                //this.ExecutionArguments.Add("UNRECONCILED_ECS_EMAIL_GROUP", "Eugene.Samoylovich@dhcs.ca.gov; EAMIProdSupport@dhcs.ca.gov;");
                this.ExecutionArguments.Add("UNRECONCILED_ECS_EMAIL_GROUP", "Genady.Gidenko@dhcs.ca.gov; Eugene.Samoylovich@dhcs.ca.gov;");
                
                //this.ExecutionArguments.Add("UNRECONCILED_ECS_EMAIL_GROUP", "Eugene.Samoylovich@dhcs.ca.gov");
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

            /* NOTE: the bottom two methods mimic real context implementation */
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
}
