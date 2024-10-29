using OHC.EAMI.WebUI.Test.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.WebUI.Test
{
    public abstract class SeleniumTest : IISServerTest
    {
        private RemoteWebDriver _remoteWebDriver;
        private WebDriverTpe _dType;

        protected IWebDriver WebDriver
        {
            get { return _remoteWebDriver; }
        }

        protected SeleniumTest(string applicationName, string applicationVirtualDirectory, WebDriverTpe driverType) : base(applicationName, applicationVirtualDirectory)
        {
            _dType = driverType;
        }

        public override void Initialize()
        {
            if(_dType==WebDriverTpe.Chrome)
                _remoteWebDriver = new ChromeDriver();
            else if(_dType==WebDriverTpe.IE)
                _remoteWebDriver = new InternetExplorerDriver();
        }

        public override void Cleanup()
        {
            _remoteWebDriver.Dispose();
        }
    }
}
