using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace OHC.EAMI.Common.Test
{
    [TestClass]
    public class CommonStatusTest
    {
        [TestMethod]
        public void CommonStatusCTorsTest()
        {
            //default CTor taking true value
            CommonStatus cs = new CommonStatus(true);
            TestObjInstanceNoDetails(cs, true);

            //default CTor taking false value
            cs = new CommonStatus(false);
            TestObjInstanceNoDetails(cs, false);

            List<string> origMsgList = GetMockMessageList();

            //CTor with string message arg
            cs = new CommonStatus(true, origMsgList[0]);
            TestObjInstanceWithDetails(cs, true, new List<string>() { origMsgList[0] });

            //CTor with List<string>
            cs = new CommonStatus(false, origMsgList);
            TestObjInstanceWithDetails(cs, false, origMsgList);         
        }

        [TestMethod]
        public void CommonStatusAddMsgMethodTest()
        {
            List<string> origMsgList = GetMockMessageList();

            //test add msg detail method
            CommonStatus cs = new CommonStatus(true, origMsgList[0]);
            cs.AddMessageDetail(origMsgList[1]);
            cs.AddMessageDetail(origMsgList[2]);
            TestObjInstanceWithDetails(cs, true, origMsgList);

            // test add/append msg detail list
            cs = new CommonStatus(false, origMsgList[0]);
            cs.AddMessageDetails(new List<string>() { origMsgList[1], origMsgList[2] });
            TestObjInstanceWithDetails(cs, false, origMsgList);            
        }


        [TestMethod]
        public void CommonStatusAddOrChangeStatusMethodTest()
        {
            List<string> origList = GetMockMessageList();
            
            // test simple status change
            CommonStatus cs = new CommonStatus(true, origList[0]);
            cs.Status = false;
            TestObjInstanceWithDetails(cs, false, new List<string>() { origList[0] });

            //test status change/merge from another instance
            CommonStatus csToBeChanged = new CommonStatus(true, origList[1]);            
            csToBeChanged.AddCommonStatus(cs);
            csToBeChanged.AddMessageDetail(origList[2]);
            // csToBeChanged expected to be set to false, 
            // also note the expected order of message lines          
            TestObjInstanceWithDetails(csToBeChanged, false, new List<string>() { origList[1], origList[0], origList[2]});
        }

        // test helper method to test obj instance wo details
        public void TestObjInstanceNoDetails(CommonStatus cs, bool expectedStatus)
        {
            Assert.IsTrue(cs.Status == expectedStatus, "expected " + expectedStatus.ToString() + " value");
            Assert.IsNotNull(cs.MessageDetailList, "object is null, expected valid instance");
            Assert.IsFalse(cs.HasDetails(), "expected false value");
            Assert.IsTrue(cs.GetFirstDetailMessage() == string.Empty, "expected empty string value");
            Assert.IsTrue(cs.GetCombinedMessage() == string.Empty, "expected empty string value");            
            Assert.IsTrue(cs.MessageDetailList.Count == 0, "expected 0 count");
        }

        // test helper method to test obj instance w.Details
        public void TestObjInstanceWithDetails(CommonStatus cs, bool expectedStatus, List<string> expectedMsgList)
        {
            Assert.IsTrue(cs.Status == expectedStatus, "expected " + expectedStatus.ToString() + " value");
            Assert.IsNotNull(cs.MessageDetailList, "object is null, expected valid instance");
            Assert.IsTrue(cs.HasDetails(), "expected true value");
            Assert.IsTrue(cs.MessageDetailList.Count == expectedMsgList.Count, "expected different msg count");
            Assert.IsTrue(cs.GetFirstDetailMessage() == expectedMsgList[0], "expected different message string value");

            // msg-for-msg test between what IS and what is expected,
            // test combined msg containing every expected msg line
            for (int i = 0; i < expectedMsgList.Count; i++)
            {
                Assert.IsTrue(cs.MessageDetailList[i] == expectedMsgList[i], "expected different msg in the list in that sequence position");
                Assert.IsTrue(cs.GetCombinedMessage().Contains(expectedMsgList[i]), "did not find expected msg line in the combined message");
            }                        
        }

        internal List<string> GetMockMessageList()
        {
            return new List<string>() {
                "My test message1",
                "My test message2",
                "My test message3"
            };
        }
    }
}
