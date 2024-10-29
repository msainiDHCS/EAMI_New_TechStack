using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.WebUI.Controllers;
using OHC.EAMI.WebUI.Models;
using OHC.EAMI.WebUI.Test.Fixtures.ManageSystems;
using OHC.EAMI.WebUI.Test.Session;
using Moq;
using System.Linq;

namespace OHC.EAMI.WebUI.Test.Controllers
{
    [TestClass]
    public class ManageSystemsController_FundTest
    {
        ManageSystemsController controller = new ManageSystemsController();

        #region Fund..
        public ManageSystemsController_FundTest() 
        {
            HttpContext.Current = MockHelpers.FakeHttpContext();
            HttpContext.Current.Session.Add("ProgramChoiceId", 4);
        }

        /// <summary>
        /// Test whether or not the ManageSystemsController returns the right view for Fund
        /// </summary>
        [TestMethod]
        public void TestViewFunds_ShouldReturnValidView()
        {
            //Act...
            var result = controller.ViewFunds(1, false);
            var viewName = ((ViewResultBase)result).ViewName;

            //Assert...
            Assert.IsNotNull(result);
            Assert.AreEqual("Fund/ViewFunds", viewName);
        }

        /// <summary>
        /// Test whether or not the ManageSystemsController returns the correct correct data.
        /// </summary>
        [TestMethod]
        public void TestViewFunds_ShouldReturnUniqueFundCodes()
        {
            ////Arrange...
            
            //EAMIFundModel expectedFund = MockFundData.MockGetFund();
            List<EAMIFundModel> expectedFundList = MockFundData.MockGetAllFunds();

            //Act...
            var result = controller.ViewFunds(1, false) as PartialViewResult;
            var lstFunds = result.Model as List<EAMIFundModel>;
            //var actualResult = lstFunds.FirstOrDefault().GetType().GetProperty("Fund_Code").GetValue(lstFunds.FirstOrDefault(), null);

            //Assert...

            //Check if all fund codes in the returned data are unique. Fund codes cannot be duplicate...
            CollectionAssert.AllItemsAreUnique(lstFunds.Select(fc => fc.Fund_Code).ToList());
        }

        #endregion
    }
}
