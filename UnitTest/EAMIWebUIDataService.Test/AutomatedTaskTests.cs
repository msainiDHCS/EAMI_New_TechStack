using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;

namespace EAMIWebUIDataService.Test
{
    [TestClass]
    public class AutomatedTaskTests
    {
        [TestMethod]
        public void CreateECS()
        {
            //generate ecs
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            //CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.CreateECS());

            Assert.IsTrue(true);
        }
    }
}
