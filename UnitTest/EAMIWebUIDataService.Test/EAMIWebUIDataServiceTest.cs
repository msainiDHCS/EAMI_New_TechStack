using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.DataAccess;
using System.Data;

//using EAMIWebUIDataService.Test;


namespace EAMIWebUIDataService.Test
{

    /**
     * 
     * The EAMIWebUIDataService.svc service must be running in order to sucesfully execute the tests below
     * 
     * */

    [TestClass]
    public class EAMIWebUIDataServiceTest
    {

        [TestMethod]
        public void CreateNewClaimSchedule()
        {
            int userID = 3;
            bool assigned = false;
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            string userLogInName = "unittest";
            string priorityGroupHash = "23010512-S0021";

            string priorityGroupHash1 = "23010512-S0020";
            //string priorityGroupHash2 = "2017.11.01.072";
            //string priorityGroupHash3 = "2017.11.01.073";
            //string priorityGroupHash4 = "2017.11.01.074";
            CommonStatus result2 = new CommonStatus(false);

            List<string> paymentGroupHashList = new List<string>() { priorityGroupHash, priorityGroupHash1 };
            result2 = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.CreateNewClaimSchedule(paymentGroupHashList, userID, userLogInName));

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void AddToClaimSchedule()
        {
            int userID = 3;
            bool assigned = false;
            CommonStatus result2 = new CommonStatus(false);
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            string userLogInName = "unittest";
            int cliamScheduleID = 1149;
            
            List<string> paymentGroupHashList = new List<string>();
            paymentGroupHashList.Add("2016.02.01.043");
            paymentGroupHashList.Add("2016.02.01.049");

            result2 = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.AddPaymentGroupsToClaimSchedule(cliamScheduleID, paymentGroupHashList, userLogInName));

            Assert.IsNotNull(result2);
        }


        [TestMethod]
        public void DeleteClaimSchedule()
        {
            bool assigned = false;
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            int userID = 11;
            string userLogInName = "unittest";
            int claimScheduleID = 2;
            
            CommonStatus result2 = new CommonStatus(false);
            result2 = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.DeleteClaimSchedule(claimScheduleID, userLogInName));

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void Delete_PaymentRecords_From_ClaimSchedule()
        {
            bool assigned = false;
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            int userID = 11;
            string userLogInName = "unittest";
            int claimScheduleID = 31;
            string priorityGroupHash2 = "2017.11.01.072";
            string priorityGroupHash3 = "2017.11.01.073";
            string priorityGroupHash4 = "2017.11.01.074";

            List<string> paymentGroupHashList = new List<string>() { priorityGroupHash2, priorityGroupHash3, priorityGroupHash4 };

            CommonStatus result2 = new CommonStatus(false);
            result2 = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.RemovePaymentGroupsFromClaimSchedule(claimScheduleID, paymentGroupHashList, userLogInName));

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void GetPaymentRecsForAssignmentTest()
        {
            bool assigned = false;
            WcfServiceInvoker wcfService = new WcfServiceInvoker();

            //List<PaymentGroup> result = wcfService.InvokeService<IEAMIWebUIDataService, List<PaymentGroup>>
            //                           (svc => svc.GetPaymentGroupsForAssignment(assigned));

            //List<PaymentSuperGroup> result2 = wcfService.InvokeService<IEAMIWebUIDataService, List<PaymentSuperGroup>>
            //                            (svc => svc.GetPaymentSuperGroupsForAssignment(assigned));
            List<PaymentSuperGroup> result2 = wcfService.InvokeService<IEAMIWebUIDataService, List<PaymentSuperGroup>>
                                        (svc => svc.GetPaymentSuperGroupsForAssignment(assigned).Payload);
            //OHC.EAMI.Common.CommonStatusPayload<>
            Assert.IsNotNull(result2);
        }
        
        [TestMethod]
        public void GetClaimScheduleListByStatusTest()
        {
            string userLogInName = "unittest";
            int statusTypeID_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            int statusTypeID_RETURN_TO_PROCESSOR = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "RETURN_TO_PROCESSOR").FirstOrDefault().ID;

            WcfServiceInvoker wcfService = new WcfServiceInvoker();

            //List<ClaimSchedule> result = wcfService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>
            //                            (svc => svc.GetClaimSchedulesByStatusType(new List<int>() { statusTypeID_ASSIGNED, statusTypeID_RETURN_TO_PROCESSOR }, userLogInName));
            List<ClaimSchedule> result = wcfService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>
                                        (svc => svc.GetClaimSchedulesByStatusType(new List<int>() { statusTypeID_ASSIGNED, statusTypeID_RETURN_TO_PROCESSOR }, userLogInName).Payload);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetClaimScheduleListByUser()
        {
            int userID = 4;
            string userLogInName = "unittest";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            //List<ClaimSchedule> result = wcfService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>
            //                            (svc => svc.GetClaimSchedulesForProcessorByUser(userID, userLogInName));
            List<ClaimSchedule> result = wcfService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>
                                        (svc => svc.GetClaimSchedulesForProcessorByUser(userID, userLogInName).Payload);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddNewRemittanceAdviceNote()
        {
            int claimScheduleID = 1;
            string userLogInName = "unittest";
            string note = "TEST ADD NEW REMITTANCE ADVICE NOTE";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.SetRemittanceAdviceNote(claimScheduleID, note, userLogInName));
            Assert.IsNotNull(result);
             
        }

        [TestMethod]
        public void EditExistingRemittanceAdviceNote()
        {
            int claimScheduleID = 1;
            string userLogInName = "unittest";
            string note = "TEST EDIT EXISTING REMITTANCE ADVICE NOTE";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.SetRemittanceAdviceNote(claimScheduleID, note, userLogInName));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteExistingRemittanceAdviceNote()
        {
            int claimScheduleID = 1;
            string userLogInName = "unittest";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.DeleteRemittanceAdviceNote(claimScheduleID));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeleteNonExistingRemittanceAdviceNote()
        {
            int claimScheduleID = 1;
            string userLogInName = "unittest";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.DeleteRemittanceAdviceNote(claimScheduleID));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentGroupsToHold()
        {
            int claimScheduleID = 1;
            string userLogInName = "unittest";
            string note = "WEBUI UNIT TEST : HOLD";
            List<string> priorityGroupHashList = new List<string>();
            priorityGroupHashList.Add("2017.12.01.061");
            //priorityGroupHashList.Add("2018.01.31.006");
            //priorityGroupHashList.Add("2018.01.31.005");

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.SetPaymentGroupsToHold(priorityGroupHashList, userLogInName, note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPaymentGroupsToUnHold()
        {
            int claimScheduleID = 1;
            string userLogInName = "unittest";
            string note = "WEBUI UNIT TEST : UNHOLD" ;
            List<string> priorityGroupHashList = new List<string>();
            priorityGroupHashList.Add("2018.01.31.011");
            priorityGroupHashList.Add("2018.01.31.006");
            priorityGroupHashList.Add("2018.01.31.005");

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.SetPaymentGroupsToUnHold(priorityGroupHashList, userLogInName, note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetProcessorUserListWithAssignedPaymentCounts()
        {
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            //List<Tuple<EAMIUser, int>> result = wcfService.InvokeService<IEAMIWebUIDataService, List<Tuple<EAMIUser, int>>>
            //                            (svc => svc.GetProcessorUserListWithAssignedPaymentCounts(11));
            List<Tuple<EAMIUser, int>> result = wcfService.InvokeService<IEAMIWebUIDataService, List<Tuple<EAMIUser, int>>>
                                       (svc => svc.GetProcessorUserListWithAssignedPaymentCounts(11).Payload);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetEAMICounts()
        {
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            //List<Tuple<string, int>> result = wcfService.InvokeService<IEAMIWebUIDataService, List<Tuple<string, int>>>
            //                            (svc => svc.GetEAMICounts());
            List<Tuple<string, int>> result = wcfService.InvokeService<IEAMIWebUIDataService, List<Tuple<string, int>>>
                                        (svc => svc.GetEAMICounts().Payload);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AssignPaymentGroups()
        {
            List<PaymentGroup> pmtGroupList = new List<PaymentGroup>();
            string userLogInName = "unittest";

            PaymentGroup paymentGroup = new PaymentGroup
            {
                UniqueNumber = "2017.11.01.057",
                AssignedUser = new EAMIUser { User_ID = 3, User_Name = "esamoylo" },
                PayDate = new RefCode { Code = "2018-12-24" }
            };

            PaymentGroup paymentGroup2 = new PaymentGroup
            {
                UniqueNumber = "2017.11.01.077",
                AssignedUser = new EAMIUser { User_ID = 3, User_Name = "esamoylo" },
                PayDate = new RefCode { Code = "2018-12-24" }
            };

            pmtGroupList.Add(paymentGroup);
            pmtGroupList.Add(paymentGroup2);

            //List<PaymentGroup> paymentGroupList, string user
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AssignPaymentGroups(pmtGroupList, userLogInName));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReAssignPaymentGroups()
        {
            List<PaymentGroup> pmtGroupList = new List<PaymentGroup>();
            string userLogInName = "unittest";

            PaymentGroup paymentGroup = new PaymentGroup
            {
                UniqueNumber = "2017.11.01.014",
                AssignedUser = new EAMIUser { User_ID = 9, User_Name = "jstark" },
                PayDate = new RefCode { Code = "2018-12-20" }
            };

            pmtGroupList.Add(paymentGroup);

            //List<PaymentGroup> paymentGroupList, string user
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus result = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.ReAssignPaymentGroups(pmtGroupList, userLogInName));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ASSIGN_PaymentGroups_FROM_UNASSIGNED_LIST()
        {
            return;
            List<Tuple<string, List<PaymentGroup>>> errorList = new List<Tuple<string, List<PaymentGroup>>>();
            List<PaymentGroup> pmtGroupList = new List<PaymentGroup>();
            string userLogInName = "system";
            int assignmentSplitCount = 10;
            int userID = 1;

            //PAYDATE
            List<RefCode> calendarList = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYDATE_CALENDAR);
            int paydateCounter = 0;
            int paydateCount = calendarList.Count();

            //GET Payment Group List
            int paymentStatusTypeId_UNASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "UNASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> paymentGroupList = OHC.EAMI.DataAccess.PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId_UNASSIGNED)
                .OrderBy(_ => _.FiscalYear)
                .ThenBy(_ => _.PayeeInfo.PEEInfo_PK_ID)
                .ThenBy(_ => _.ContractNumber).ToList();

            while (paymentGroupList.Count > 0)
            {
                //CHOOSE PAYDATE
                paydateCounter = paydateCounter == paydateCount ? 0 : paydateCounter;
                string paydate = calendarList.ElementAt(paydateCounter).Code;

                //CHOOSE USER 
                //SPREAD between all users
                userID = userID >= 10 ? 2 : userID;

                //COMPOSE A LIST TO ASSIGN
                List<PaymentGroup> listToAssign = new List<PaymentGroup>();
                foreach (PaymentGroup pg in paymentGroupList.Take(assignmentSplitCount).ToList())
                {

                    listToAssign.Add(new PaymentGroup
                    {
                        UniqueNumber = pg.UniqueNumber,
                        AssignedUser = new EAMIUser { User_ID = userID },
                        PayDate = new RefCode { Code = paydate }
                    });
                }

                //ASSIGN HERE
                WcfServiceInvoker wcfService = new WcfServiceInvoker();
                CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                            (svc => svc.AssignPaymentGroups(listToAssign, userLogInName));
                //CAPTURE ERRORS
                if (!status.Status)
                {
                    Tuple<string, List<PaymentGroup>> error = new Tuple<string,List<PaymentGroup>>(                       
                    status.GetCombinedMessage(),
                    new List<PaymentGroup>(listToAssign));
                    errorList.Add(error);
                }

                //REMOVE PROESSED PAYMENT RECORD GROUPS 
                paymentGroupList.RemoveAll(_ => listToAssign.Select(__ => __.UniqueNumber).Contains(_.UniqueNumber));

                paydateCounter++;
                userID++;
            }

            Assert.IsNotNull(errorList);
        }

        /* Eugene S 2018-12-07 this unit test is broken and needs update
         
        [TestMethod]
        public void CREATE_NewClaimSchedule_FROM_ASSIGNED_LIST()
        {
            return;
            int userID = 1;
            List<Tuple<string, string, string, string, int, string, List<string>>> errorList = new List<Tuple<string, string, string, string, int, string, List<string>>>();

            //GET Payment Group List
            int paymentStatusTypeId_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> assignedPaymentgrpList = OHC.EAMI.DataAccess.PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId_ASSIGNED);

            if (assignedPaymentgrpList.Count > 0)
            {
                var grpList = assignedPaymentgrpList.GroupBy(_ => new
                {
                    _.PayDate.ID,
                    _.AssignedUser.User_ID,
                    _.ContractNumber,
                    _.FiscalYear,
                    _.PaymentType,
                    _.PayeeInfo.PEEInfo_PK_ID,
                    _.IsIHSS,
                    _.IsSCHIP
                }).ToList();


                foreach(var grp in grpList)
                {
                    List<PaymentGroup> pgList = assignedPaymentgrpList.Where(_ =>
                        _.PayDate.ID == grp.Key.ID
                        && _.AssignedUser.User_ID == grp.Key.User_ID
                        && _.ContractNumber == grp.Key.ContractNumber
                        && _.FiscalYear == grp.Key.FiscalYear
                        && _.PaymentType == grp.Key.PaymentType
                        && _.PayeeInfo.PEEInfo_PK_ID == grp.Key.PEEInfo_PK_ID
                        && _.IsIHSS == grp.Key.IsIHSS
                        && _.IsSCHIP == grp.Key.IsSCHIP)
                    .ToList();
                    
                    double maxTotalAmountAllowed = 9999999999/100;
                    while (pgList.Count > 0)
                    {
                        //SPREAD CLaim Schedules between all users
                        userID = userID >= 10 ? 2 : userID;

                        //KEEP Claim Schedule under MAX amount of 99 Million
                        List<PaymentGroup> lst = RemoveRecordsToStayUnderMaxAllowedAmount(pgList, maxTotalAmountAllowed);

                        //CREATE CLAIM SCHEDULE HERE
                        WcfServiceInvoker wcfService = new WcfServiceInvoker();
                        CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.CreateNewClaimSchedule(lst.Select(_ => _.UniqueNumber).ToList()  , userID, "system"));

                        //Capture errors
                        if (!status.Status)
                        {
                            string errorMessage = status.GetCombinedMessage();
                            Tuple<string, string, string, string, int, string, List<string>> errorDetail = new Tuple<string, string, string, string, int, string, List<string>>
                            (
                                errorMessage,
                                grp.Key.ID.ToString(),
                                grp.Key.ContractNumber,
                                grp.Key.FiscalYear + "; " + grp.Key.PaymentType,
                                grp.Key.PEEInfo_PK_ID,
                                string.Format("{0}{1}", grp.Key.IsIHSS ? "IsIHSS" : string.Empty, grp.Key.IsSCHIP ? " IsSCHIP" : string.Empty).Trim(),
                                new List<string>(lst.Select(_ => _.UniqueNumber).ToList()));

                            errorList.Add(errorDetail);
                        }
                        //REMOVE PROESSED PAYMENT RECORD GROUPS 
                        pgList.RemoveAll(_ => lst.Select(__ => __.UniqueNumber).Contains(_.UniqueNumber));
                        System.Threading.Thread.Sleep(1000);
                    }

                    userID++;
                };
            }

            Assert.IsNotNull(errorList.Count > 0);
        }
         */

        [TestMethod]
        public void SUBMIT_FOR_APPROVAL_ClaimSchedule()
        {
            return;
            int assignmentSplitCount = 5;
            string datetimestamp = DateTime.Now.ToString();


            //GET Payment Group List
            int csStatusTypeId_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            //List<ClaimSchedule> csList = OHC.EAMI.DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_ASSIGNED });
            WcfServiceInvoker csListService= new WcfServiceInvoker();
            //List<ClaimSchedule> csList = csListService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>(svc => svc.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_ASSIGNED }, "system"));
            List<ClaimSchedule> csList = csListService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>(svc => svc.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_ASSIGNED }, "system").Payload);

            while (csList.Count > 0)
            {
                List<int> lst = csList.Take(assignmentSplitCount).Select(_ => _.PrimaryKeyID).ToList();

                //SUBMIT_FOR_APPROVAL HERE
                WcfServiceInvoker wcfService = new WcfServiceInvoker();
                CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetClaimSchedulesStatusToSubmitForApproval(lst, string.Format("SUBMIT_FOR_APPROVAL. Load Test on {0}", datetimestamp), "system"));

                //REMOVE PROCESSED
                csList.RemoveAll(_ => lst.Select(__ => __).Contains(_.PrimaryKeyID));
            }

            Assert.IsNotNull(csList);
        }

        [TestMethod]
        public void APPROVE_ClaimSchedule()
        {
            return;
            
            int assignmentSplitCount = 5;
            string datetimestamp = DateTime.Now.ToString();


            //GET Payment Group List
            int csStatusTypeId_SUBMIT_FOR_APPROVAL = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "SUBMIT_FOR_APPROVAL").FirstOrDefault().ID;
            List<ClaimSchedule> csList = OHC.EAMI.DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_SUBMIT_FOR_APPROVAL });

            while (csList.Count > 0)
            {
                List<int> lst = csList.Take(assignmentSplitCount).Select(_ => _.PrimaryKeyID).ToList();

                //APPROVE HERE
                WcfServiceInvoker wcfService = new WcfServiceInvoker();
                CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetClaimSchedulesStatusToApproved(lst, string.Format("APPROVE. Load Test on {0}", datetimestamp), "system"));

                //REMOVE PROCESSED
                csList.RemoveAll(_ => lst.Select(__ => __).Contains(_.PrimaryKeyID));
            }

            Assert.IsNotNull(csList);
        }

        [TestMethod]
        public void GetElectronicClaimSchedulesByDateRange()
        {
            //APPROVE HERE
            DateTime fromDate = DateTime.Now.AddDays(-5);
            DateTime toDate = DateTime.Now.AddDays(1);
            int statusTypeID = 1;

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            //List<ElectronicClaimSchedule> ecsList = wcfService.InvokeService<IEAMIWebUIDataService, List<ElectronicClaimSchedule>>(svc => svc.GetElectronicClaimSchedulesByDateRangeStatusType(fromDate, toDate, statusTypeID));
            List<ElectronicClaimSchedule> ecsList = wcfService.InvokeService<IEAMIWebUIDataService, List<ElectronicClaimSchedule>>(svc => svc.GetElectronicClaimSchedulesByDateRangeStatusType(fromDate, toDate, statusTypeID).Payload);

            Assert.IsNotNull(ecsList);
        }

        private List<PaymentGroup> RemoveRecordsToStayUnderMaxAllowedAmount(List<PaymentGroup> pgList, double maxTotalAmountAllowed)
        {
            //order list by descending AMOUNT 
            List<PaymentGroup> lst = new List<PaymentGroup>(pgList.OrderByDescending(_ => _.Amount));

            //Ensure list is under MAX amount
            while (lst.Count > 0 && Convert.ToDouble(lst.Sum(_ => _.Amount)) > maxTotalAmountAllowed)
            {
                //if SUM(AMOUNT) is more than MAX allowed, 
                //then continue to REMOVE records from top of the list
                //do this until amount is less than MAX allowed
                lst.RemoveAt(lst.Count - 1);
            }

            return lst;
        }
        
        [TestMethod]
        public void RemovePaymentGroupsFromClaimScheduleTest()
        {
            string userLogInName = "system";
            int csID = 9;
            List<String> priorityGroupHash = new List<string>();
            priorityGroupHash.Add("2017.12.01.009");

            bool assigned = false;
            WcfServiceInvoker wcfService = new WcfServiceInvoker();

            CommonStatus result2 = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.RemovePaymentGroupsFromClaimSchedule(csID, priorityGroupHash, userLogInName));

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void GetFaceSheetDataByECSID()
        {
            int ecsID = 4;

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> faceSheetData = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetFaceSheetDataByECSID(ecsID));

            Assert.IsNotNull(faceSheetData);
        }

        //[TestMethod]
        //public void GetECSFundingSummary()
        //{
        //    List<int> claimScheduleIDList = new List<int>();

        //    claimScheduleIDList.Add(5);
        //    claimScheduleIDList.Add(4);
        //    claimScheduleIDList.Add(8);
        //    claimScheduleIDList.Add(3);

        //    WcfServiceInvoker wcfService = new WcfServiceInvoker();
        //    CommonStatusPayload<List<PaymentFundingDetail>> faceSheetData = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<PaymentFundingDetail>>>(svc => svc.GetECSFundingSummary(claimScheduleIDList));

        //    Assert.IsNotNull(faceSheetData);
        //}


        [TestMethod]
        public void GetECSFaceSheetData()
        {
            int ecsID = 1122;

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> faceSheetData = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetFaceSheetDataByECSID(ecsID));

            Assert.IsNotNull(faceSheetData);
        }

        [TestMethod]
        public void GetECSTransferLetter()
        {
            int ecsID = 6;
            string user = "unitTest";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> transferLetterData = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetTransferLetterDataByECSID(ecsID, user));

            Assert.IsNotNull(transferLetterData);
        }

        [TestMethod]
        public void SetECSStatusToApproved()
        {
            int ecsID = 7;
            string user = "unitTest";
            string note = "test";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus cs = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetECSStatusToApproved(ecsID, note, user));

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void SubmitForApprovalCS()
        {
            string user = "unitTest";
            string note = "test";

            List<int> csIDList = new List<int>();
            csIDList.Add(3);

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus cs = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetClaimSchedulesStatusToSubmitForApproval(csIDList, note, user));

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void ApproveCS()
        {
            string user = "unitTest";
            string note = "test";

            List<int> csIDList = new List<int>();
            csIDList.Add(1);            

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus cs = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetClaimSchedulesStatusToApproved(csIDList, note, user));

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void DeleteECS()
        {
            int ecsID = 8;
            string user = "unitTest";
            string note = "test";

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus cs = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.DeleteECS(ecsID, note, user));

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void GetPayDateFromClaimScheduleList()
        {
            string userLogInName = "unittest";
            int cs_statusTypeID_APPROVED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "APPROVED").FirstOrDefault().ID;              

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            //List<ClaimSchedule> csList = wcfService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>
            //                            (svc => svc.GetClaimSchedulesByStatusType(new List<int>() { cs_statusTypeID_APPROVED }, userLogInName));
            List<ClaimSchedule> csList = wcfService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>
                                        (svc => svc.GetClaimSchedulesByStatusType(new List<int>() { cs_statusTypeID_APPROVED }, userLogInName).Payload);

            //GET PAYDATE ID's from a CLAIM SCHEDULE LIST
            var payDateList = csList.GroupBy(_ => new { _.PayDate.ID }).ToList();


            Assert.IsNotNull(payDateList);
        }


        [TestMethod]
        public void GetHoldReportData()
        {

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cs = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetHoldReportData());

            Assert.IsNotNull(cs);
        }

        [TestMethod]
        public void GetRefCodeTypes()
        {
            WcfServiceInvoker wcfService = new WcfServiceInvoker();

            CommonStatusPayload<RefCodeTableList> cspl = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<RefCodeTableList>>
                                        (svc => svc.GetRefCodeTableList(), true);

            RefCodeTableList rctl = cspl.Payload;
            RefCodeList rcl = rctl.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE);

            Assert.IsNotNull(rctl);

        }

        [TestMethod]
        public void GetReferenceCodeList()
        {
            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            enRefTables tbName = enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE;

            CommonStatusPayload<List<RefCodeList>> cspl = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>
                                        (svc => svc.GetReferenceCodeList(tbName));

            List<RefCodeList> rctl = cspl.Payload;
            //RefCodeList rcl = rctl[0].;

            Assert.IsNotNull(rctl);

        }
    }
}



