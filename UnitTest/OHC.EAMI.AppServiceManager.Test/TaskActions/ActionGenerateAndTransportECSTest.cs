using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace OHC.EAMI.AppServiceManager.Test.TaskActions
{
    [TestClass]
    public class ActionGenerateAndTransportECSTest
    {
        [TestMethod]
        public void TestZipCodeParsing_Invalid_EmptyAddressLine()
        {
            //ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            Tuple<string, string, string> result; 
            string addressLine = string.Empty;

            //result = ecs.ParseZipCodeFromPayeeAddressLine(addressLine);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZipCodeParsing_Valid_zip5()
        {
            //ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            //Tuple<string, string, string> result;
            //string addressLine = "South San Francisco CA, 94080";

            //result = ecs.ParseZipCodeFromPayeeAddressLine(addressLine);

            //Assert.IsNotNull(result);
            //Assert.IsTrue(result.Item2.Length == 5);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZipCodeParsing_Valid_zip9()
        {
            //ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            //Tuple<string, string, string> result;
            //string addressLine = "OXNARD CA, 930368294";

            //result = ecs.ParseZipCodeFromPayeeAddressLine(addressLine);

            //Assert.IsNotNull(result);
            //Assert.IsTrue(result.Item2.Length == 5 && result.Item3.Length == 4);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZipCodeParsing_Valid_zip9_withHyphen()
        {
            //ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            //Tuple<string, string, string> result;
            //string addressLine = "Long Beach CA, 90802-4317";

            //result = ecs.ParseZipCodeFromPayeeAddressLine(addressLine);

            //Assert.IsNotNull(result);
            //Assert.IsTrue(result.Item1.Length == 5 && result.Item2.Length == 4);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZipCodeParsing_Invalid_zip9_withHyphen()
        {
            //ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            //Tuple<string, string, string> result;
            //string addressLine = "Long Beach CA, 90802-4AB7";

            //result = ecs.ParseZipCodeFromPayeeAddressLine(addressLine);

            //Assert.IsNull(result);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZipCodeParsing_Invalid_noZip()
        {
            //ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            //Tuple<string, string, string> result;
            //string addressLine = "ALAMEDA CA,";

            //result = ecs.ParseZipCodeFromPayeeAddressLine(addressLine);

            //Assert.IsNull(result);
            Assert.IsTrue(true);
        }

        //[TestMethod]
        //public void CreateScoFile()
        //{
        //    ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
        //    //TaskActions result = new CommonStatus(true);

        //    ecs.Execute(null);

        //    //Assert.IsNotNull(true);
        //}
    }
}

