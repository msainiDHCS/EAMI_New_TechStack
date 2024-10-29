using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common;
using System.Collections.Generic;
using System.Linq;
using OHC.EAMI.WebUIServiceManager;
using System.Data;

namespace OHC.EAMI.DataAccess.Test
{
    [TestClass]
    public class ReportDataDbMgrTest
    {
        [TestMethod]
        public void GetFaceSheetDataByECSID_Test()
        {
            int ecsID = 1;

            CommonStatusPayload<DataSet> result = ReportDataDbMgr.GetFaceSheetDataByECSID(ecsID);

            Assert.IsTrue(result.Status);
        }

        //[TestMethod]
        //public void DeleteECSByID()
        //{
        //    CommonStatus result = ClaimScheduleDataDbMgr.DeleteECS(3, "delete ecs note", "boom");
        //    Assert.IsTrue(result.Status);
        //}
    }
}
