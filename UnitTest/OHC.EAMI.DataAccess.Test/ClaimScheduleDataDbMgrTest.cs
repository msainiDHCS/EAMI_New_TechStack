using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common;
using System.Collections.Generic;
using System.Linq;
using OHC.EAMI.WebUIServiceManager;



namespace OHC.EAMI.DataAccess.Test
{
    [TestClass]
    public class ClaimScheduleDataDbMgrTest
    {
        //[TestMethod]
        //public void CreateNewClaimSchedule()
        //{
        //    ClaimSchedule cs = PopulateNewClaimSchedule(); 

        //    CommonStatus status = ClaimScheduleDataDbMgr.InsertClaimScheduleData(cs, "unittest");
        //    Assert.IsTrue(status.Status);
        //}

        //[TestMethod]
        //public void CheckClaimSchedulePaymentRecStatusFlag_REVIEWED()
        //{
        //    int claimScheduleID = 3;
        //    int paymentRecStatusTypeID_Reviewed = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "REVIEWED").FirstOrDefault().ID;

        //    bool reviewedFlag = DataAccess.ClaimScheduleDataDbMgr.CheckClaimSchedulePaymentRecStatus(claimScheduleID, paymentRecStatusTypeID_Reviewed);
        //    Assert.IsTrue(reviewedFlag);
        //}

        [TestMethod]
        public void SetClaimSchedulesStatusToReturnToProcessor()
        {
            int claimScheduleID = 3;
            string note = "TEST";
            //string user = "unitTestUser";
            string user = "jstark";
            List<int> claimScheduleIDList = new List<int>();
            claimScheduleIDList.Add(claimScheduleID);

            CommonStatus status = DataAccess.ClaimScheduleDataDbMgr.SetClaimSchedulesStatusToReturnToProcessor(claimScheduleIDList, note, user);
            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void SetClaimSchedulesStatusToReturnTo_SUBMIT_FOR_APPROVAL()
        {
            int claimScheduleID = 3;
            string note = "TEST";
            string user = "unitTestUser";
            List<int> claimScheduleIDList = new List<int>();
            
            //claimScheduleIDList.Add(claimScheduleID);


            claimScheduleIDList.Add(107);
            claimScheduleIDList.Add(109);
            claimScheduleIDList.Add(110);
            claimScheduleIDList.Add(111);
            claimScheduleIDList.Add(112);
            claimScheduleIDList.Add(113);
            //claimScheduleIDList.Add(6);
            //claimScheduleIDList.Add(7);
            //claimScheduleIDList.Add(4);
            //claimScheduleIDList.Add(9);
            //claimScheduleIDList.Add(10);
            //claimScheduleIDList.Add(11);
            //claimScheduleIDList.Add(2);
            //claimScheduleIDList.Add(13);
            //claimScheduleIDList.Add(14);
            //claimScheduleIDList.Add(15);
            //claimScheduleIDList.Add(16);

            RefCode csStatus_SUBMIT_FOR_APPROVAL = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "SUBMIT_FOR_APPROVAL").FirstOrDefault();

            CommonStatus status = DataAccess.ClaimScheduleDataDbMgr.SetClaimScheduleStatus(csStatus_SUBMIT_FOR_APPROVAL, claimScheduleIDList, note, user);
            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void SetClaimSchedulesStatusToReturnTo_RETURN_TO_PROCESSOR()
        {
            int claimScheduleID = 3;
            string note = "TEST";
            string user = "unitTestUser";
            List<int> claimScheduleIDList = new List<int>();
            claimScheduleIDList.Add(claimScheduleID);

            RefCode csStatus_RETURN_TO_PROCESSOR = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "RETURN_TO_PROCESSOR").FirstOrDefault();

            CommonStatus status = DataAccess.ClaimScheduleDataDbMgr.SetClaimScheduleStatus(csStatus_RETURN_TO_PROCESSOR, claimScheduleIDList, note, user);
            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void SetClaimSchedulesStatusTo_APPROVED()
        {
            int claimScheduleID = 3;
            string note = "TEST";
            string user = "unitTestUser";
            List<int> claimScheduleIDList = new List<int>();

            //claimScheduleIDList.Add(claimScheduleID);

            claimScheduleIDList.Add(107);
            claimScheduleIDList.Add(109);
            claimScheduleIDList.Add(110);
            claimScheduleIDList.Add(111);
            claimScheduleIDList.Add(112);
            claimScheduleIDList.Add(113);

            RefCode csStatus_APPROVED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "APPROVED").FirstOrDefault();

            CommonStatus status = DataAccess.ClaimScheduleDataDbMgr.SetClaimScheduleStatus(csStatus_APPROVED, claimScheduleIDList, note, user);
            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void AddPaymentGroupsToClaimSchedule()
        {
            int paymentRecCount = 2;
            int claimScheduleID = 54;
            string note = "TEST";
            string user = "jstark";
            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            List<string> hashList = new List<string>();

            //foreach (PaymentGroup pgr in PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId).Take(paymentRecCount))
            //{
            //    hashList.Add(pgr.UniqueNumber);
            //}

            hashList.Add("2019.05.01.006");

            CommonStatus status = new EAMIWebUIDataServiceMgr().AddPaymentGroupsToClaimSchedule(claimScheduleID, hashList, user); 
            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void GetSPGetClaimSchedulesByID()
        {
            int claimScheduleID = 10;

            List<ClaimSchedule> claimSchedule = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { claimScheduleID });
            
            Assert.IsNotNull(claimSchedule);
        }

        [TestMethod]
        public void ValidateIfCanMofdifyClaimSchedule()
        {
            int claimScheduleID = 4;

            List<ClaimSchedule> claimScheduleList = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { claimScheduleID });

            CommonStatus cs = ValidateIfCanMofdifyClaimSchedule(claimScheduleList);

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void GetElectronicClaimSchedulesByDateRange()
        {
            DateTime dateFrom = DateTime.Now.AddDays(-15);
            DateTime dateTo = DateTime.Now.AddDays(2);
            int statusTypeID = 1;

            List<ElectronicClaimSchedule> ecsList = DataAccess.ClaimScheduleDataDbMgr.GetElectronicClaimSchedulesByDateRangeStatusType(statusTypeID, dateFrom, dateTo);

            Assert.IsNotNull(ecsList);
        }

        [TestMethod]
        public void Delete_ClaimSchedule()
        {
            string userLogInName = "unittest";
            string note = "test";
            CommonStatus cs = new CommonStatus(true);
            int csID = 23;

            cs = new EAMIWebUIDataServiceMgr().DeleteClaimSchedule(csID, userLogInName);
           
            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void Remove_PaymentRecords_From_ClaimSchedule()
        {
            string userLogInName = "unittest";
            string note = "test";
            List<string> hasList = new List<string>();
            CommonStatus cs = new CommonStatus(true);
            int csID = 1;

            hasList.Add("2017.11.01.019");

            cs = new EAMIWebUIDataServiceMgr().RemovePaymentGroupsFromClaimSchedule(csID, hasList, userLogInName);

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void Delete_PaymentRecords_From_ClaimSchedule()
        {
            bool assigned = false;
            int userID = 11;
            //string userLogInName = "unittest";
            string userLogInName = "jstark";
            int claimScheduleID = 54;

            string priorityGroupHash = "2019.05.01.006";
            //string priorityGroupHash1 = "2017.11.01.066";
            //string priorityGroupHash2 = "2017.11.01.072";
            //string priorityGroupHash3 = "2017.11.01.073";
            //string priorityGroupHash4 = "2017.11.01.074";

            //string priorityGroupHash2 = "2017.11.01.072";
            //string priorityGroupHash3 = "2017.11.01.073";
            //string priorityGroupHash4 = "2017.11.01.074";

            //List<string> paymentGroupHashList = new List<string>() { priorityGroupHash, priorityGroupHash1, priorityGroupHash2, priorityGroupHash3, priorityGroupHash4 };
            List<string> paymentGroupHashList = new List<string>() { priorityGroupHash};

            ClaimSchedule cs = DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByID(new List<int>() { claimScheduleID }).FirstOrDefault();

            //if (cs.PaymentGroupList.Select(_ => paymentGroupHashList.Contains(_.UniqueNumber)).Count() == cs.PaymentGroupList.Count())
            if (cs.PaymentGroupList.Where(_ => paymentGroupHashList.Contains(_.UniqueNumber)).Count() == cs.PaymentGroupList.Count())
            {
                DataAccess.ClaimScheduleDataDbMgr.RemovePaymentGroupsFromClaimSchedule(cs, paymentGroupHashList, userLogInName);
            }

            Assert.IsNotNull(cs);
        } 
        
        //[TestMethod]
        //public void GetClaimScheduleIDsByPriorityGroupHash()
        //{
        //    string priorityGroupHash = "2017.11.01.007";

        //    List<int> csIDList = DataAccess.ClaimScheduleDataDbMgr.GetClaimScheduleIDsByPriorityGroupHash(priorityGroupHash);

        //    Assert.IsNotNull(csIDList);
        //}

        [TestMethod]
        public void SetClaimSchedulesStatusTo_SUBMIT_FOR_APPROVAL()
        {
            string note = "TEST";
            string user = "unitTestUser";
            List<int> csIDList = new List<int>();

            //csIDList.Add(2);
            csIDList.Add(107);
            csIDList.Add(109);
            csIDList.Add(110);
            csIDList.Add(111);
            csIDList.Add(112);
            csIDList.Add(113);
            csIDList.Add(114);
            csIDList.Add(115);
            csIDList.Add(116);
            csIDList.Add(117);
            csIDList.Add(119);

            CommonStatus status = new EAMIWebUIDataServiceMgr().SetClaimSchedulesStatusToSubmitForApproval(csIDList, note, user);
            
            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void SetClaimSchedulesStatusTo_RETURN_TO_PROCESSOR()
        {
            string note = "TEST";
            string user = "unitTestUser";
            List<int> csIDList = new List<int>();

            csIDList.Add(2);

            CommonStatus status = new EAMIWebUIDataServiceMgr().SetClaimSchedulesStatusToReturnToProcessor(csIDList, note, user);

            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void SetClaimSchedulesStatus_To_APPROVED()
        {
            string note = "TEST";
            //string user = "unitTestUser";
            string user = "unittest";
            List<int> csIDList = new List<int>();

            //csIDList.Add(2);

            csIDList.Add(107);
            csIDList.Add(109);
            csIDList.Add(110);
            csIDList.Add(111);
            csIDList.Add(112);
            csIDList.Add(113);
            csIDList.Add(114);
            csIDList.Add(115);
            csIDList.Add(116);
            csIDList.Add(117);
            csIDList.Add(119);

            CommonStatus status = new EAMIWebUIDataServiceMgr().SetClaimSchedulesStatusToApproved(csIDList, note, user);

            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void SetECSStatus_To_APPROVED()
        {
            string note = "TEST";
            string user = "unittest";
            int ecsID = 1;

            CommonStatus status = new EAMIWebUIDataServiceMgr().SetECSStatusToApproved(ecsID, note, user);

            Assert.IsTrue(status.Status);
        }

        [TestMethod]
        public void Delete_PaymentRecords_From_ClaimSchedule_Service()
        {
            bool assigned = false;
            int userID = 11;
            string userLogInName = "unittest";
            int claimScheduleID = 4;

            List<string> hashList = new List<string>() { "2017.11.01.041" };

            CommonStatus status = new EAMIWebUIDataServiceMgr().RemovePaymentGroupsFromClaimSchedule(claimScheduleID, hashList, userLogInName);

            Assert.IsNotNull(status);
        }

        //[TestMethod]
        //public void GetECSFundingSummary()
        //{
        //    List<int> claimScheduleIDList = new List<int>();
            
        //    claimScheduleIDList.Add(5);
        //    claimScheduleIDList.Add(4);
        //    claimScheduleIDList.Add(8);
        //    claimScheduleIDList.Add(3);

        //    CommonStatusPayload<List<PaymentFundingDetail>> statusPayload = new EAMIWebUIDataServiceMgr().GetECSFundingSummary(claimScheduleIDList);

        //    Assert.IsNotNull(statusPayload);
        //}


        [TestMethod]
        public void GetECSFileNameincrementor()
        {
            string fileName = "PS.DEVL.DISB.FTP.D{0}.MCMW.WARRANT";
            DateTime dateStamp = DateTime.Parse("10/23/2018");

            string incrementorValue = DataAccess.ClaimScheduleDataDbMgr.GetECSFileNameIncrement(fileName, dateStamp);

            Assert.IsNotNull(incrementorValue);
        } 

        private CommonStatus ValidateIfCanMofdifyClaimSchedule(List<ClaimSchedule> claimSCheduleList)
        {
            CommonStatus comonStatus = new CommonStatus(true);

            RefCode statusType_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();
            RefCode statusType_RETURN_TO_PROCESSOR = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "RETURN_TO_PROCESSOR").FirstOrDefault();

            if (!claimSCheduleList.Select(_ => _.CurrentStatus.StatusType.ID == statusType_ASSIGNED.ID).Any()
                    && !claimSCheduleList.Select(_ => _.CurrentStatus.StatusType.ID == statusType_RETURN_TO_PROCESSOR.ID).Any())
            {
                comonStatus.Status = false;
                comonStatus.AddMessageDetail("Cannot Modify Claim Schedule. Status is not ASSIGNED and is not RETURN_TO_PROCESSOR");
            }

            return comonStatus;
        }
        
        private ClaimSchedule PopulateNewClaimSchedule()
        {
            int paymentRecCount = 5;
            TimeSpan diff = DateTime.Now  - DateTime.Now.Date;
            string claimScheduleNumber = string.Format("{0:MMdd}{1}", DateTime.Now, Math.Floor(diff.TotalSeconds).ToString().PadLeft(5,'0'));
            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            RefCode paydate = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYDATE_CALENDAR).Where(_ => _.Code == "2017-11-11").FirstOrDefault();
            List<PaymentGroup> prList = new List<PaymentGroup>();

            foreach (PaymentGroup pgr in PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId).Take(paymentRecCount))
            {
                prList.Add(pgr);
            }

            ClaimSchedule cs = null;
            if (prList.Count > 0)
            {
                if (prList != null)
                {
                    cs = new ClaimSchedule();
                    cs.Amount = prList.Sum(_ => _.Amount);
                    cs.AssignedUser = new EAMIUser { User_ID = 1 };
                    cs.UniqueNumber = claimScheduleNumber;
                    cs.ContractNumber = "TEST01";
                    cs.FiscalYear = "2017";
                    cs.PaymentType = "TEST01";
                    cs.PayDate = paydate;
                    cs.PayeeInfo = new PaymentExcEntityInfo { PEEInfo_PK_ID = 1 };

                    cs.PaymentGroupList = prList;
                }
            }
            return cs;
        }
    }
}
