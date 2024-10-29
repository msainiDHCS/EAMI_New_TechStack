using FTSAS.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.AppServiceManager.Test
{

    /// <summary>
    /// this is a mock context to be used when unit testing custom FTSAS.TaskActions 
    /// note - the context will normally be created by FTSAS
    /// and injected into the task action object instance
    /// </summary>
    public class MockTaskActionContext : ITaskActionContext
    {
        public MockTaskActionContext(Dictionary<string, string> executionArgs)
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

