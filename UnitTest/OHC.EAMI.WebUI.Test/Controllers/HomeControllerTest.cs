using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.WebUI;
using OHC.EAMI.WebUI.Controllers;
using OHC.EAMI.CommonEntity;

namespace OHC.EAMI.WebUI.Test.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void TestIndexViewIsNotEmpty()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestViewReturnsValidModel()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.TestView() as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result.Model,typeof(EAMIElement));
        }

    }
}
