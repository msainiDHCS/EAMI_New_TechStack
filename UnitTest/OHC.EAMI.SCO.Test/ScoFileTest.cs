using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.SCO;
using System.Collections.Generic;
using OHC.EAMI.Common;
using System.IO;
using System.Text;

namespace OHC.EAMI.SCO.Test
{
    [TestClass]
    public class ScoFileTest
    {
        [TestMethod]
        public void SCO_ValidFileCreationTest()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(3);

            //assert
            ScoServiceManager manager = new ScoServiceManager(4260);            
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile != null, scofile.GetCombinedMessage());
            Assert.IsTrue(scofile.Payload != null && scofile.Payload.Length > 0);

            if (scofile != null && scofile.Payload != null)
                System.IO.File.WriteAllText(@"ScoFile.txt", scofile.Payload);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithInvalidOrgID()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithNoClaimScheduleNumber()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);
            emaiClaimPaymentList.ForEach(a => a.ClaimScheduleNumber = 0);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithNoPaymentData()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);
            emaiClaimPaymentList.ForEach(a => a.PAYMENT_AMOUNT = 0);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;

            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithNoVendorID()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);
            emaiClaimPaymentList.ForEach(a => a.VENDOR_NO = string.Empty);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithNoVendorZip()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);
            emaiClaimPaymentList.ForEach(a => a.SECONDARY_VENDOR_ZIPCODE_FIRST5 = string.Empty);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestMaxingOutClaimSchedulesPerEFTFile()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(101);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithVendorPaymentCrossingThreshold()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);
            emaiClaimPaymentList.ForEach(a => a.PAYMENT_AMOUNT = 100000000);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_NoFileCreationTestWithClaimFilePaymentTotalCrossingThreshold()
        {
            //arrange
            List<IClaimpayment> emaiClaimPaymentList = new List<IClaimpayment>();

            //act
            emaiClaimPaymentList = GetSCOSourceData(5);
            emaiClaimPaymentList.ForEach(a => a.PAYMENT_AMOUNT = 500000000);

            //assert
            ScoServiceManager manager = new ScoServiceManager(0);
            var payload = manager.CreateWarrantFile(emaiClaimPaymentList).Payload.Item1;
            var status = manager.CreateWarrantFile(emaiClaimPaymentList).Status;
            //CommonStatusPayload<string> scofile = manager.CreateWarrantFile(emaiClaimPaymentList);
            CommonStatusPayload<string> scofile = new CommonStatusPayload<string>(payload, status);

            Assert.IsTrue(scofile.Payload.Length == 0);
        }

        [TestMethod]
        public void SCO_Parse_DEX_File()
        {
            //string dexFilePath = @"C:\A1\EAMI\SCO\DEX\EAMI.DEX.ByGENADY.TXT";
            string dexFilePath = @"C:\Users\JStark\Documents\test.TXT";

            ScoServiceManager manager = new ScoServiceManager(4260);
            List<string> dexLines = new List<string>();

            using (StreamReader streamReader = new StreamReader(dexFilePath, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    dexLines.Add(line);
                }
            };
            
            //act
            CommonStatusPayload<List<EamiDexRecord>> result = manager.ReadDEXFile(dexLines);

            //assert
            Assert.IsTrue(result.Payload.Count == 0);
        }

        internal static List<IClaimpayment> GetSCOSourceData(int claimSchdlCount)
        {
            List<IClaimpayment> eamiClaimSchedulePayments = new List<IClaimpayment>();

            int maxVendorsPerClaimSchedule = 1;

            int cc = 0; int vl = 0, tempCSNo = 5164729;

            Random rng = new Random();
            

            while (cc++ < claimSchdlCount)
            {
                tempCSNo = tempCSNo + cc + 10;
                vl = 0;

                while (vl++ < maxVendorsPerClaimSchedule)
                {
                    //one vendor entry per max number allowed
                    string vendorNo = rng.Next(888888, 9999999).ToString();                   

                    eamiClaimSchedulePayments.Add(new EamiClaimPayment()
                    {
                        ClaimScheduleNumber = tempCSNo,
                        PAYMENT_AMOUNT = rng.Next(10000, 999999),
                        VENDOR_NO = vendorNo,
                        VENDOR_NAME = "Test Vendor",
                        VENDOR_ADDRESS_LINE1 = "920 Folsom Street",
                        VENDOR_ADDRESS_LINE2 = "",
                        VENDOR_ADDRESS_LINE3 = "",
                        VENDOR_ADDRESS_LINE4 = "Sacramento, CA",
                        VENDOR_ZIPCODE_FIRST5 = "95818",
                        VENDOR_ZIPCODE_LAST4 = "9160"
                    });
                };

            };

            return eamiClaimSchedulePayments;
        }
        
    }
}
