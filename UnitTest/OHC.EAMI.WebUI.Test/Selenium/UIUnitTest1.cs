using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.WebUI.Test
{
    [TestClass]
    public class UnitTest1 : SeleniumTest
    {
        public UnitTest1() : base("http://localhost", "OHC.EAMI.WebUI", Common.WebDriverTpe.IE)
        {
        }

        [TestMethod]
        public void TestMethod1()
        {
            //string path = base.ApplicationVirtualDirectory + @"/home/SampleFormWithSubmit";
            string path = base.ApplicationVirtualDirectory + @"/home";
            Uri fullURI = new Uri(base.ApplicationBaseURI, path);

            WebDriver.Navigate().GoToUrl(fullURI);

            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(15));
            
            IWebElement FirstName = WebDriver.FindElement(new ByIdOrName("FirstName"));
            FirstName.SendKeys("DHCS");
            
            IWebElement btneditmyprofilesave = WebDriver.FindElement(new ByIdOrName("btneditmyprofilesave"));
            btneditmyprofilesave.Click();

            //wait.Until(d => d.Title.StartsWith("Sample", StringComparison.OrdinalIgnoreCase));
        }
    }
}
