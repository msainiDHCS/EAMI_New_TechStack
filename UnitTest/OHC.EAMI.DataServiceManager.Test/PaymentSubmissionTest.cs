using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;


using OHC.EAMI.ServiceContract;
using OHC.EAMI.ServiceManager;

namespace OHC.EAMI.DataServiceManager.Test
{
    [TestClass]
    public class PaymentSubmissionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // load payment submission xml file
            string xml = File.ReadAllText(@"C:\temp\jwang_PaymentSubmissionRequest_20220310-113430_EFT.xml");
            PaymentSubmissionRequest request = Helper.Deserialize(xml, typeof(PaymentSubmissionRequest)) as PaymentSubmissionRequest;

            PaymentSubmissionResponse response = new EAMIServiceManager().EAMIPaymentSubmission(request);
        }
    }
}
