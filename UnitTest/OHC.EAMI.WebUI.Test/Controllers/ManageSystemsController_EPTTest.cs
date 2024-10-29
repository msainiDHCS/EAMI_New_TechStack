using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.WebUI.Controllers;
using OHC.EAMI.WebUI.Test.Session;

namespace OHC.EAMI.WebUI.Test.Controllers
{
    [TestClass]
    public class ManageSystemsController_EPTTest
    {
        //Arrange...
        ManageSystemsController controller = new ManageSystemsController();

        public ManageSystemsController_EPTTest()
        {
            HttpContext.Current = MockHelpers.FakeHttpContext();
            HttpContext.Current.Session.Add("ProgramChoiceId", 4);
        }

        /// <summary>
        /// Test whether or not the ManageSystemsController returns the right view for EPT
        /// </summary>
        [TestMethod]
        public void TestViewExclusivePmtType_ShouldReturnValidView()
        {
            //Act...
            var result = controller.ViewExclusivePmtType(1, false);
            var viewName = ((ViewResultBase)result).ViewName;

            //Assert...
            Assert.IsNotNull(result);
            Assert.AreEqual("ExclusivePmtType/ViewExclusivePmtType", viewName);
        }
    }
}