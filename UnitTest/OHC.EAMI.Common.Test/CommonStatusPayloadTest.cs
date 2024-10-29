using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OHC.EAMI.Common.Test
{
    [TestClass]
    public class CommonStatusPayloadTest
    {
        [TestMethod]
        public void CommonStatusPayloadBasicTest()
        {
            string origName = "SomeName";
            int origNumber = 555;
            CommonStatusPayload<MockPayload> csWithPayload = DoSomething(true, origName, origNumber);
            Assert.IsInstanceOfType(csWithPayload.Payload, typeof(MockPayload), 
                "expected different type for given object instance");
            Assert.IsTrue(csWithPayload.Payload.Name == origName, "expected different name string value");
            Assert.IsTrue(csWithPayload.Payload.Number == origNumber, "expected different number value");
        }

        //test helper mehtod to test return of common status with payload
        CommonStatusPayload<MockPayload> DoSomething(bool status, string name, int number)
        {
            return new CommonStatusPayload<MockPayload>(new MockPayload(name, number), true);
        }
    }

    internal class MockPayload
    {
        public MockPayload(string name, int number) 
        {
            this.Name = name;
            this.Number = number;
        }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
