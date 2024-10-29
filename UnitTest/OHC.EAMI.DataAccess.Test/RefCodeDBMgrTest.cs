using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;
using OHC.EAMI.WebUIServiceManager;

namespace OHC.EAMI.DataAccess.Test
{
    /// <summary>
    /// Summary description for RefCodeDBMgrTest
    /// </summary>
    [TestClass]
    public class RefCodeDBMgrTest
    {
        

        [TestMethod]
        public void ReferenceCodeTableLoadTest()
        {
            // Eugene S
            //NOTE: this unit test performs data access - please make sure you point to correct database

            //// check to make sure the RefCodeTableList is not cached
            //Assert.IsNull(CacheManager<DataSet>.Get(RefCodeDBMgr.REF_CODE_TABLE_CACHE_KEY));

            //// load RefCodeTableList from database 
            //DataSet ds = RefCodeDBMgr.GetRefCodeTableList();
            //Assert.IsNotNull(ds);            

            //// assert the list is cached
            //Assert.IsNotNull(CacheManager<DataSet>.Get(RefCodeDBMgr.REF_CODE_TABLE_CACHE_KEY));

            //// assert corect obj instance type is cached
            //Assert.IsInstanceOfType(CacheManager<DataSet>.Get(RefCodeDBMgr.REF_CODE_TABLE_CACHE_KEY), typeof(DataSet));                      
        }


        [TestMethod]
        public void ReferenceCodeTableListHydrateTest()
        {
            // Eugene S
            //NOTE: this unit test performs data access - please make sure you point to correct database

            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
            Assert.IsNotNull(rctl);
            Assert.IsTrue(rctl.Count > 0);
        }

        [TestMethod]
        public void AccessSimpleRefCodeDataTest()
        {
            // get a root table list (top level list that holds other lists)
            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
            Assert.IsNotNull(rctl);
            Assert.IsTrue(rctl.Count > 0);

            // get specific ref code list by table name
            RefCodeList rcl = rctl.GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE);
            Assert.IsNotNull(rcl);
            Assert.IsTrue(rcl.Count > 0);

            // get specific ref code value by code or id
            RefCode rc = rcl.GetRefCodeByCode("RECEIVED");
            Assert.IsNotNull(rc);

            RefCode rc2 = rcl.GetRefCodeByID(1);
            Assert.IsNotNull(rc2);
        }

        [TestMethod]
        public void AccessComplexRefCodeDataTest()
        {
            // get a root table list (top level list that holds other lists)
            RefCodeTableList rctl = RefCodeDBMgr.GetRefCodeTableList();
            Assert.IsNotNull(rctl);
            Assert.IsTrue(rctl.Count > 0);

            // get specific ref code list by table name
            DBSettingList dsl = rctl.GetRefCodeListByTableName(enRefTables.TB_DB_SETTING) as DBSettingList;
            Assert.IsNotNull(dsl);
            Assert.IsTrue(dsl.Count > 0);

            // get ref code with value
            DBSetting ds = dsl.GetDBSettingByKeyAndType("TRACE_INCOMING_PAYMENT_DATA", "GENERAL_SINGLE_KVP");
            Assert.IsNotNull(ds);
            Assert.IsTrue(bool.Parse(ds.DBSettingValue));

            // get another list and the ref code
            RefCodeList rcl = rctl.GetRefCodeListByTableName(enRefTables.TB_SOR_KVP_KEY);
            Assert.IsNotNull(rcl);

            KvpDefinition kvpd = rcl.GetRefCodeByCode("CONTRACT_NUMBER") as KvpDefinition;
            Assert.IsNotNull(kvpd);

        }
       
        [TestMethod]
        public void GetRefCodeTableListTest()
        {
            CommonStatusPayload<RefCodeTableList> cspl = new EAMIWebUIDataServiceMgr().GetRefCodeTableList();

            RefCodeTableList rctl = cspl.Payload;
            RefCodeList rcl = rctl.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE);

            Assert.IsNotNull(rctl); 
        }
        
    }
}
