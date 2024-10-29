using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OHC.EAMI.FTSASTaskActions;
//using FTSAS;
using FTSAS.Integration;

namespace OHC.EAMI.AppServiceManager.Test.TaskActions
{
    [TestClass]
    public class ActionProcessDexFileTest
    {
        [TestMethod]
        public void ProcessDexFile()
        {
            ActionProcessDexFile dex = new ActionProcessDexFile();
            //TaskResult result = dex.Execute(null);
            TaskResult result = dex.Execute();

            Assert.IsNotNull(result);
        }

    }
}
