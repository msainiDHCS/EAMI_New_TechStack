using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace OHC.EAMI.Common.Test
{

    [TestClass]
    public class CacheManagerFixture
    {
        [TestMethod]
        public void TestBasicCaching()
        {
            // set object in memory cache store
            string key = "MyKey";
            string itemToCache = "ABC";
            CacheManager<string>.Set(itemToCache, key);

            string key2 = "MyKey2";
            int itemtoCache2 = 46546;
            CacheManager<int>.Set(itemtoCache2, key2);

            string key3 = "MyKey3";
            DateTime itemtoCache3 = DateTime.Parse("2017-07-07");
            CacheManager<DateTime>.Set(itemtoCache3, key3);

            // get object from cache store
            string itemFromCache = CacheManager<string>.Get(key);
            Assert.IsTrue(itemToCache == itemFromCache, "Item from retreived from memory catch does not match the original");

            // remove object from memory cache
            CacheManager<object>.Remove(key);
            Assert.IsNull(CacheManager<object>.Get(key), "Item was not removed from cache store as expected");

            // remove all items
            CacheManager<object>.ClearCache();
            Assert.IsNull(CacheManager<object>.Get(key2), "Item2 was not removed from cache store as expected");
            Assert.IsNull(CacheManager<object>.Get(key3), "ITEM3 was not removed from cache store as expected");            
        }


        [TestMethod]
        public void TestImplementedInstanceCaching()
        {
            string key = "MyKey2";
            // here we create prepopulated instance that implements 
            // abstract, generic CacheManager base class
            TestContext itemToCatche = GetPopulatedTestContextInstance(key);
            itemToCatche.Set();
            DisplayContext(itemToCatche, "ORIGINAL");

            // get item from cache and assert it maches the original
            TestContext itemFromCache = CacheManager<TestContext>.Get(key);
            DisplayContext(itemFromCache, "RETREIVED_1");
            AssertTestContextInstanceMatch(itemToCatche, itemFromCache);
        }

        /// <summary>
        /// asserts the matching of two instances of TestContext classes
        /// </summary>
        /// <param name="tcOriginal"></param>
        /// <param name="tcNew"></param>
        private void AssertTestContextInstanceMatch(TestContext tcOriginal, TestContext tcNew)
        {
            Assert.IsTrue(tcOriginal.CacheKey == tcNew.CacheKey, "CacheKey of retreived item does not match the original");
            Assert.IsTrue(tcOriginal.CacheScope == tcNew.CacheScope, "CacheScope of retreived item does not match the original");
            Assert.IsTrue(tcOriginal.Field1 == tcNew.Field1, "Field1 of retreived item does not match the original");
            Assert.IsTrue(tcOriginal.Field2 == tcNew.Field2, "Field2 of retreived item does not match the original");
            Assert.IsTrue(tcOriginal.Field3.Tables[0].Rows.Count == tcNew.Field3.Tables[0].Rows.Count,
                "Field3 count of retreived item does not match the count of orignal");
            Assert.IsTrue(tcOriginal.Field3.Tables[0].Rows[1][1].ToString() == tcNew.Field3.Tables[0].Rows[1][1].ToString(),
                "Field3 of retreived item does not match the original");
        }

        /// <summary>
        /// builds and returns TestContext instance
        /// </summary>
        /// <returns></returns>
        private TestContext GetPopulatedTestContextInstance(string key)
        {
            TestContext tc = new TestContext(key);

            // do coustom construction logic here                        
            tc.Field1 = "SomeText";
            tc.Field2 = 55;
            tc.Field3 = new DataSet();

            // create table and columns
            DataTable dt = new DataTable("MYTABLE");
            dt.Columns.Add("Column1", typeof(string));
            dt.Columns.Add("Column2", typeof(int));
            dt.Columns.Add("Column3", typeof(DateTime));

            // add rows
            DataRow dr1 = dt.NewRow();
            dr1["Column1"] = "Boom";
            dr1["Column2"] = 2445;
            dr1["Column3"] = DateTime.Now.AddYears(1);
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2["Column1"] = "Baaaaaam";
            dr2["Column2"] = 3;
            dr2["Column3"] = DateTime.Now.AddYears(-5);
            dt.Rows.Add(dr2);

            // add table to dataset
            tc.Field3.Tables.Add(dt);

            return tc;
        }

        /// <summary>
        /// displays test context instance in the console
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="header"></param>
        private void DisplayContext(TestContext tc, string header)
        {            
            Console.WriteLine(header);
            Console.WriteLine("-----------------------");
            Console.WriteLine("CacheKey: " + tc.CacheKey);
            Console.WriteLine("CacheScope: " + tc.CacheScope);
            Console.WriteLine("Field1: " + tc.Field1);
            Console.WriteLine("Field2: " + tc.Field2);
            Console.WriteLine("Field3: ");
            Console.WriteLine("  R1C1:" + tc.Field3.Tables[0].Rows[0]["Column1"].ToString());
            Console.WriteLine("  R1C2:" + tc.Field3.Tables[0].Rows[0]["Column2"].ToString());
            Console.WriteLine("  R1C3:" + tc.Field3.Tables[0].Rows[0]["Column3"].ToString());
            Console.WriteLine("  R2C1:" + tc.Field3.Tables[0].Rows[1]["Column1"].ToString());
            Console.WriteLine("  R2C2:" + tc.Field3.Tables[0].Rows[1]["Column2"].ToString());
            Console.WriteLine("  R2C3:" + tc.Field3.Tables[0].Rows[1]["Column3"].ToString());
            Console.WriteLine("------------------------");
            Console.WriteLine();
        }
    }


    /// <summary>
    /// this class serves as example on how to create data context class that implements caching
    /// </summary>
    internal class TestContext : CacheManager<TestContext>
    {
        //public static string TestContextScope = "TestContextScope";
        public TestContext(string key)
            : base(key)
        { 
            // do whatever logic here
        }

        public TestContext(string key, string scope)
            : base(key, scope)
        {
            // do coustom construction logic here                                   
        }

        public string Field1 { get; set; }
        public int Field2 { get; set; }
        public DataSet Field3 { get; set; }

        protected override TestContext GetInstance()
        {
            return this;
        }
    }
}
