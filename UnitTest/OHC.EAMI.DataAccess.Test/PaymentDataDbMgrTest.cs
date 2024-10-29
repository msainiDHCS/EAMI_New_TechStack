 using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using OHC.EAMI.WebUIServiceManager;

namespace OHC.EAMI.DataAccess.Test
{
    [TestClass]
    public class PaymentDataDbMgrTest
    {
        private DateTime calendarDayToday = DateTime.Now;
        private DateTime calendarDayTomorrow = DateTime.Now.AddDays(1);
        private DateTime calendarDayTodayPlusTwo = DateTime.Now.AddDays(2);

        [TestMethod]
        public void GetPaymentFundingDetailList()
        {
            int id = 259;
            EAMIWebUIDataServiceMgr placeHolder = new EAMIWebUIDataServiceMgr();
            var paymentFundingDetailList =  placeHolder.GetPaymentFundingDetails(id);
            //List<PaymentFundingDetail> paymentFundingDetailList = new EAMIWebUIDataServiceMgr().GetPaymentFundingDetails(id);
            Assert.IsNotNull(paymentFundingDetailList);
        }   

        [TestMethod]
        public void GetYearlyCalendarEntries()
        {
            //string username = "unittest";
            //int currentYear = 2017;

            //CommonStatusPayload<List<Tuple<EAMIDateType, DateTime>>> result = WebUIServiceManager.PaymentDataServiceManager.GetYearlyCalendarEntries(currentYear, username);

            //Assert.IsTrue(result.Payload.Count > 0);
        }

        [TestMethod]
        public void AddPaydateCalendarEntry()
        {
            //string username = "unittest";
            //List<Tuple<EAMIDateType, DateTime>> calendarList = ComposeCalendarDateEntryList(EAMIDateType.PayDate);

            //CommonStatus result = PaymentDataDbMgr.AddYearlyCalendarEntry(calendarList, username);

            //Assert.IsTrue(result.Status);
        }

        [TestMethod]
        public void AddDrawdateCalendarEntry()
        {
            //string username = "unittest";
            //List<Tuple<EAMIDateType, DateTime>> calendarList = ComposeCalendarDateEntryList(EAMIDateType.DrawDate);

            //CommonStatus result = PaymentDataDbMgr.AddYearlyCalendarEntry(calendarList, username);

            //Assert.IsTrue(result.Status);
        }

        [TestMethod]
        public void DeletePaydateCalendarEntry()
        {
            //string username = "unittest";
            //List<Tuple<EAMIDateType, DateTime>> calendarList = ComposeCalendarDateEntryList(EAMIDateType.PayDate);

            //foreach (Tuple<EAMIDateType, DateTime> date in calendarList)
            //{
            //    CommonStatus result = PaymentDataDbMgr.DeleteYearlyCalendarEntry(date.Item1, date.Item2, username);
            //    Assert.IsNotNull(result);
            //}
        }

        [TestMethod]
        public void DeleteDrawdateCalendarEntry()
        {
            //string username = "unittest";
            //List<Tuple<EAMIDateType, DateTime>> calendarList = ComposeCalendarDateEntryList(EAMIDateType.DrawDate);

            //foreach (Tuple<EAMIDateType, DateTime> date in calendarList)
            //{
            //    CommonStatus result = PaymentDataDbMgr.DeleteYearlyCalendarEntry(date.Item1, date.Item2, username);
            //    Assert.IsNotNull(result);
            //}
        }

        [TestMethod]
        public void SetPaymentRecsToHold()
        {
            string username = "unittest";
            //string note = "This is a test by a user who is testing the Hold functionality through the unit tests";
            string note = "This is another test ";
            List<string> priorityGroupHashList = new List<string> { "2017.12.01.029" };

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "HOLD").FirstOrDefault();

            CommonStatus result = PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, username, note);
                        
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentRecsToUnHold()
        {
            string username = "unittest";
            List<string> priorityGroupHashList = new List<string> { "2017.12.01.029" };

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNHOLD").FirstOrDefault();

            CommonStatus result = PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, username, "TEST");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentRecsToReturnToSup()
        {
            string username = "unittest";
            List<string> priorityGroupHashList = new List<string> { "2017.11.01.059" };

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "RETURNED_TO_SUP").FirstOrDefault();

            CommonStatus result = PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, username, "TEST");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentRecsToReturnToSOR()
        {
            string username = "unittest";
            List<string> priorityGroupHashList = new List<string> { "2017.12.01.034" };

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "RETURNED_TO_SOR").FirstOrDefault();

            CommonStatus result = PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, username, "TEST");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentRecToReleaseFromSup()
        {
            string username = "unittest";
            List<string> priorityGroupHashList = new List<string> { "2017.12.01.046" };

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "RELEASED_FROM_SUP").FirstOrDefault();

            CommonStatus result = PaymentDataDbMgr.SetPaymentGroupToReleaseFromSup(priorityGroupHashList, username, "TEST ReleaseFromSup");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentRecsToReviewed()
        {
            //string username = "unittest";
            //string priorityGroupHash = "";

            //RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "REVIEWED").FirstOrDefault();

            //CommonStatus result = PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHash, username, "TEST");
            //Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AssignPaymentToUser()
        {
            string username = "unittest";
            int assignToUserID = 3;
            int paydateCalendarID = 1;
            string payDateValue = "2023-02-24";
            string priorityGroupHash = "23912895-S0092";
            
            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();

            List<PaymentGroup> paymentGroupListToAssign = new List<PaymentGroup>();
            paymentGroupListToAssign.Add(new PaymentGroup() { AssignedUser = new EAMIUser() { User_ID = assignToUserID }, PayDate = new RefCode() { Code = payDateValue }, UniqueNumber = priorityGroupHash });

            CommonStatus result = new CommonStatus(false);
            result = PaymentDataDbMgr.AssignPaymentGroup(statusType, paymentGroupListToAssign, username);

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void AssignPaymentGroupListToUser()
        {
            string username = "unittest";
            int assignToUserID = 3;
            string priorityGroupHash = "2017.11.01.002";
            int paydateCalendarID = 10;
            int maxCountToAssign = 1;

            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> PaymentGroupList = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId).Take(maxCountToAssign).ToList();
            //List<PaymentGroup> PaymentGroupList = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId).Where(_ => _.PaymentRecordList.Any(__ => __.PriorityGroupHash == priorityGroupHash)).ToList();

            CommonStatus result2 = new CommonStatus(false);
            if (PaymentGroupList.Count > 0)
            {

                List<PaymentGroup> paymentGroupListToAssign = new List<PaymentGroup>();

                foreach (PaymentGroup pg in PaymentGroupList)
                {
                    paymentGroupListToAssign.Add(new PaymentGroup(){
                      AssignedUser = new EAMIUser() { User_ID = assignToUserID }
                    , PayDate = new RefCode() { ID = paydateCalendarID }
                    , UniqueNumber = pg.UniqueNumber });

                }

                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();

                result2 = PaymentDataDbMgr.AssignPaymentGroup(statusType, paymentGroupListToAssign, username);
            }

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void AssignPaymentListToUser()
        {
            //string username = "unittest";
            //int assignToUserID = 1;
            //int paydateCalendarID = 2;
            //int maxCountToAssign = 1;


            //int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault().ID;
            //List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);

            //foreach (PaymentGroup pgr in result.Take(maxCountToAssign))
            //{
            //    RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();
            //    CommonStatus resultStatus = PaymentDataDbMgr.AssignPaymentGroup(statusType, pgr.UniqueNumber, assignToUserID, paydateCalendarID, username);
            //}

            //Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UnAssignPaymentGroupListToUser()
        {
            string username = "unittest";
            int assignToUserID = 11;
            int paydateCalendarID = 10;
            int maxCountToAssign = 3;

            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> PaymentGroupList = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId).Take(maxCountToAssign).ToList();
            
            CommonStatus result2 = new CommonStatus(false);
            if (PaymentGroupList.Count > 0)
            {
                List<PaymentGroup> paymentGroupListToUnassign = new List<PaymentGroup>();
                foreach (PaymentGroup pg in PaymentGroupList)
                {
                    paymentGroupListToUnassign.Add(new PaymentGroup() { AssignedUser = new EAMIUser() { User_ID = assignToUserID }, PayDate = new RefCode() { ID = paydateCalendarID }, UniqueNumber = pg.UniqueNumber });
                }

                RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault();
                result2 = PaymentDataDbMgr.UnAssignPaymentGroup(statusType, paymentGroupListToUnassign, username);
            }

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void UnAssignAllPaymentGroups()
        {
            //string username = "unittest";
            //int assignToUserID = 1;
            //int paydateCalendarID = 2;
            //int maxCountToAssign = 1;


            //int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            //List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);

            //foreach (PaymentGroup pgr in result)
            //{
            //    RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault();
            //    CommonStatus resultStatus = PaymentDataDbMgr.AssignPaymentGroup(statusType, pgr.UniqueNumber, assignToUserID, paydateCalendarID, username);
            //}

            //Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReAssignPaymentToUser()
        {
            string username = "unittest";
            int assignToUserID = 3;
            int paymentRecID = 1;
            int paydateCalendarID = 1;
            string paydateCalendarDate = "2017-06-13";
            string priorityGroupHash = "2018.02.12.004";

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault();


            List<PaymentGroup> paymentGroupListToReAssign = new List<PaymentGroup>();
            //paymentGroupListToReAssign.Add(new PaymentGroup() { AssignedUser = new EAMIUser() { User_ID = assignToUserID }, PayDate = new RefCode() { ID = paydateCalendarID }, UniqueNumber = priorityGroupHash });
            paymentGroupListToReAssign.Add(new PaymentGroup() { AssignedUser = new EAMIUser() { User_ID = assignToUserID }, PayDate = new RefCode() { Code = paydateCalendarDate }, UniqueNumber = priorityGroupHash });

            CommonStatus result = new CommonStatus(false);
            result = PaymentDataDbMgr.ReAssignPaymentGroup(statusType, paymentGroupListToReAssign, username);

            Assert.IsNotNull(result);
        }

        //[TestMethod]
        //public void GetFundingDetailsByPaymentRecID()
        //{
        //    int priorityGroupHash = 259;

        //    List<PaymentFundingDetail> paymentFundingDetailList = PaymentDataDbMgr.GetFundingDetailsByPaymentRecID(priorityGroupHash);
        //    Assert.IsNotNull(paymentFundingDetailList);
        //}
        
        [TestMethod]
        public void GetPaymentRecordsByStatus_Unassigned()
        {
            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentRecordsByStatus_Assigned()
        {
            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentRecordsByStatus_AddedToClaimSchedule()
        {
            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ADDED_TO_CS").FirstOrDefault().ID;
            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentRecordsAssignedByUser()
        {
            int userID = 3;
            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsAssignedByUser(userID);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentGroupsByStatus_UNASSIGNED()
        {

            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);

            int totalPaymentRecordsInGroup = result.Sum(_ => _.PaymentRecordList.Count);
            decimal paymentGroupAmountRecordsInGroup = result.SelectMany(_ => _.PaymentRecordList).Sum(__ => __.Amount);

            //foreach (PaymentGroup pg in result.OrderBy(_ => _.UniqueNumber))
            //{
            //    //Debug.WriteLine(string.Format("{0}{1}{2}", pg.UniqueNumber, pg.PayeeInfo.PEE.Description, pg.PayeeInfo.PEE_AddressLine));
            //    Debug.WriteLine(string.Format("{0}{1}", pg.UniqueNumber, pg.PayeeInfo.PEE.Description));
            //}

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentGroupsForAssignment_TEST()
        {
            CommonStatusPayload<List<PaymentGroup>> statusPayload = new EAMIWebUIDataServiceMgr().GetPaymentGroupsForAssignment(false);           
            Assert.IsNotNull(statusPayload.Status);
        }

        [TestMethod]
        public void GetPaymentSuperGroupsForAssignment_TEST()
        {
            CommonStatusPayload<List<PaymentSuperGroup>> statusPayload = new EAMIWebUIDataServiceMgr().GetPaymentSuperGroupsForAssignment(false);
            Assert.IsNotNull(statusPayload.Status);
        }
        


        [TestMethod]
        public void GetPaymentGroupsByStatus_ASSIGNED()
        {

            int paymentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId);

            int totalPaymentRecordsInGroup = result.Sum(_ => _.PaymentRecordList.Count);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentGroupsByHashList()
        {
            List<string> hashList = new List<string>();

            hashList.Add("2017.11.01.002");

            List<PaymentGroup> result = PaymentDataDbMgr.GetPaymentGroupsByPaymentSetNumberList(hashList);
            
            Assert.IsNotNull(result);
        }

        
        [TestMethod]
        public void SetPaymentRecsToUnAssigned()
        {
            string username = "unittest";
            List<string> priorityGroupHashList = new List<string> { "2009230001001P", "2010080001003P", "2010080001004P", "2010080001005P", "2010080001007P" };

            /*
             * 2009230001001P
2010080001003P
2010080001004P
2010080001005P
2010080001007P
             * */

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault();

            CommonStatus result = PaymentDataDbMgr.SetPaymentGroupStatus(statusType, priorityGroupHashList, username, "TEST");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPaymentGroupsAssignedByUser()
        {
            string username = "unittest";
            string priorityGroupHash = "";
            int userID = 11;

            RefCode statusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault();

            List<PaymentGroup> paymentGroupList =  PaymentDataDbMgr.GetPaymentGroupsAssignedByUser(userID);
            
            Assert.IsNotNull(paymentGroupList);
        }
        
        [TestMethod]
        public void GetUserListWithAssignedPaymentCount()
        {
            List<Tuple<EAMIUser, int>> result = DataAccess.PaymentDataDbMgr.GetUserListWithAssignedPaymentCount(11);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetEAMICounts()
        {
            //EAMIDBConnection.EAMIDBContext = "";

            //List<Tuple<string, int>> result = DataAccess.PaymentDataDbMgr.GetEAMICounts();
            var result = PaymentDataDbMgr.GetEAMICounts();

            Assert.IsNotNull(result);
        }
                
        [TestMethod]
        public void CreateNewClaimSchedules_MultipleLinked()
        {
            int userID = 3;
            string username = "unittest";
            List<string> hashList = new List<string>();
            CommonStatus cs = new CommonStatus(false);           


            hashList.Add("1909160130046P");
            hashList.Add("1909160130043P");

            cs = new EAMIWebUIDataServiceMgr().CreateNewClaimSchedule(hashList, userID, username);

            //foreach (string pr in hashList)
            //{
            //    List<string> lst = new List<string>();
            //    lst.Add(pr);
            //    cs = new EAMIWebUIDataServiceMgr().CreateNewClaimSchedule(lst, userID, username);
            //    if (!cs.Status) break;
            //}


            Assert.IsNotNull(cs);
        }

        private List<Tuple<EAMIDateType, DateTime>> ComposeCalendarDateEntryList(EAMIDateType dateType)
        {
            List<Tuple<EAMIDateType, DateTime>> calendarList = new List<Tuple<EAMIDateType, DateTime>>();

            calendarList.Add(Tuple.Create(dateType, calendarDayToday));
            calendarList.Add(Tuple.Create(dateType, calendarDayTomorrow));
            calendarList.Add(Tuple.Create(dateType, calendarDayTodayPlusTwo));

            return calendarList;
        }
        
        [TestMethod]
        public void GetPaymentSuperGroupsForSupervisorScreen()
        {
            //List<PaymentSuperGroup> paymentSuperGroupList = new EAMIWebUIDataServiceMgr().GetPaymentSuperGroupsForSupervisorScreen();
            var paymentSuperGroupList = new EAMIWebUIDataServiceMgr().GetPaymentSuperGroupsForSupervisorScreen();
            Assert.IsNotNull(paymentSuperGroupList);
        }

    
    }
}

