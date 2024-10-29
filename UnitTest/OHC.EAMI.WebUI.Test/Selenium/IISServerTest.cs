using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace OHC.EAMI.WebUI.Test
{

    [TestClass]
    public abstract class IISServerTest
    {
        private readonly string _applicationBaseUrl;
        private readonly string _applicationVirtualDirectory;
        protected IISServerTest(string applicationBaseUrl, string applicationVirtualDirectory)
        {
            _applicationBaseUrl = applicationBaseUrl;
            _applicationVirtualDirectory = applicationVirtualDirectory;
        }

        protected Uri ApplicationBaseURI
        {
            get { return new Uri(_applicationBaseUrl); }
        }

        protected string ApplicationVirtualDirectory
        {
            get { return _applicationVirtualDirectory; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Cleanup();
        }

        public abstract void Initialize();
        public abstract void Cleanup();
    }
}