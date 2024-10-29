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


namespace EAMIWebUIDataService.Test
{
    [TestClass]
    public class EAMIAdvancePaymentRecordsTests
    {
        //STEP 1
        [TestMethod]
        public void ASSIGN_PaymentGroups_FROM_UNASSIGNED_LIST()
        {
            //return;
            List<Tuple<string, List<PaymentGroup>>> errorList = new List<Tuple<string, List<PaymentGroup>>>();
            List<PaymentGroup> pmtGroupList = new List<PaymentGroup>();
            string userLogInName = "system";
            int assignmentSplitCount = 4;
            int userID = 2;

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

            //EXCLUDE THESE GROUPS
            //string excludePaymentRecord = "2017.11.01";
            //paymentGroupList.RemoveAll(_ => excludePaymentRecord.Contains(_.UniqueNumber.ToString().Substring(0, 10)));
            paymentGroupList.RemoveRange(7,6);

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
                    Tuple<string, List<PaymentGroup>> error = new Tuple<string, List<PaymentGroup>>(
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

        //STEP 2
        [TestMethod]
        public void CREATE_NewClaimSchedule_FROM_ASSIGNED_LIST()
        {
            //return;
            List<Tuple<string, string, string, string, int, string, List<string>>> errorList = new List<Tuple<string, string, string, string, int, string, List<string>>>();

            //GET Payment Group List
            int paymentStatusTypeId_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            List<PaymentGroup> assignedPaymentgrpList = OHC.EAMI.DataAccess.PaymentDataDbMgr.GetPaymentGroupsByStatus(paymentStatusTypeId_ASSIGNED);
            
            //EXCLUDE THESE GROUPS
            //string excludePaymentRecord = "2017.11.01";
            //assignedPaymentgrpList.RemoveAll(_ => excludePaymentRecord.Contains(_.UniqueNumber.ToString().Substring(0, 10)));
            //assignedPaymentgrpList.RemoveRange(0, 1);

            if (assignedPaymentgrpList.Count > 0)
            {
                //GROUP ASSIGNED PAYMENT RECORDS
                var grpList = assignedPaymentgrpList.GroupBy(_ => new
                {
                    _.PayDate.ID,
                    _.AssignedUser.User_ID,
                    _.ContractNumber,
                    _.FiscalYear,
                    _.PaymentType,
                    _.PayeeInfo.PEEInfo_PK_ID,
                    _.ExclusivePaymentType.Code
                }).ToList();

                //GO THROUGH EACH GROUP
                foreach (var grp in grpList)
                {
                    List<PaymentGroup> pgList = assignedPaymentgrpList.Where(_ =>
                        _.PayDate.ID == grp.Key.ID
                        && _.AssignedUser.User_ID == grp.Key.User_ID
                        && _.ContractNumber == grp.Key.ContractNumber
                        && _.FiscalYear == grp.Key.FiscalYear
                        && _.PaymentType == grp.Key.PaymentType
                        && _.PayeeInfo.PEEInfo_PK_ID == grp.Key.PEEInfo_PK_ID
                        && _.ExclusivePaymentType.Code == grp.Key.Code)
                    .ToList();

                    double maxTotalAmountAllowed = 999999999999 / 100;
                    while (pgList.Count > 0)
                    {
                        //KEEP Claim Schedule under MAX amount of 99 Million
                        List<PaymentGroup> lst = RemoveRecordsToStayUnderMaxAllowedAmount(pgList, maxTotalAmountAllowed);

                        //CREATE CLAIM SCHEDULE HERE
                        WcfServiceInvoker wcfService = new WcfServiceInvoker();
                        CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.CreateNewClaimSchedule(lst.Select(_ => _.UniqueNumber).ToList(), (int)grp.Key.User_ID, "system"));

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
                                grp.Key.Code,
                                new List<string>(lst.Select(_ => _.UniqueNumber).ToList()));

                            errorList.Add(errorDetail);
                        }
                        //REMOVE PROESSED PAYMENT RECORD GROUPS 
                        pgList.RemoveAll(_ => lst.Select(__ => __.UniqueNumber).Contains(_.UniqueNumber));
                        System.Threading.Thread.Sleep(1000);
                    }
                };
            }

            Assert.IsNotNull(errorList.Count > 0);
        }

        //STEP 3
        [TestMethod]
        public void SUBMIT_FOR_APPROVAL_ClaimSchedule()
        {
           // return;
            int assignmentSplitCount = 10;
            string datetimestamp = DateTime.Now.ToString();

            //GET Payment Group List
            int csStatusTypeId_ASSIGNED = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "ASSIGNED").FirstOrDefault().ID;
            //List<ClaimSchedule> csList = OHC.EAMI.DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_ASSIGNED });
            WcfServiceInvoker csListService = new WcfServiceInvoker();
            //List<ClaimSchedule> csList = csListService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>(svc => svc.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_ASSIGNED }, "system").ToList());
            List<ClaimSchedule> csList = csListService.InvokeService<IEAMIWebUIDataService, List<ClaimSchedule>>(svc => svc.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_ASSIGNED }, "system").Payload);

            //csList.RemoveRange(5, 3);

            while (csList.Count > 0)
            {
                List<int> lst = csList.Take(assignmentSplitCount).Select(_ => _.PrimaryKeyID).ToList();

                //SUBMIT_FOR_APPROVAL HERE
                WcfServiceInvoker wcfService = new WcfServiceInvoker();
                CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetClaimSchedulesStatusToSubmitForApproval(lst, string.Format("SUBMIT_FOR_APPROVAL. Load Test on {0}", datetimestamp), "system"));

                //REMOVE PROCESSED
                csList.RemoveAll(_ => lst.Select(__ => __).Contains(_.PrimaryKeyID));
            }
            ;
            Assert.IsNotNull(csList);
        }

        //STEP 4
        [TestMethod]
        public void APPROVE_ClaimSchedule()
        {
            //return;
            int assignmentSplitCount = 15;
            string datetimestamp = DateTime.Now.ToString();


            //GET Payment Group List
            int csStatusTypeId_SUBMIT_FOR_APPROVAL = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_CLAIM_SCHEDULE_STATUS_TYPE).Where(_ => _.Code == "SUBMIT_FOR_APPROVAL").FirstOrDefault().ID;
            List<ClaimSchedule> csList = OHC.EAMI.DataAccess.ClaimScheduleDataDbMgr.GetClaimSchedulesByStatusType(new List<int>() { csStatusTypeId_SUBMIT_FOR_APPROVAL });


            //csList.RemoveRange(5, 2);

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

        //STEP 5
        [TestMethod]
        public void APPROVE_ECS()
        {
            int  ecsID = 3;

            WcfServiceInvoker wcfService = new WcfServiceInvoker();
            CommonStatus status = wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(svc => svc.SetECSStatusToApproved(ecsID, string.Format("Approve ECS on {0}", DateTime.Now), "unittest"));

            Assert.IsNotNull(status);
        }

        /* Eugene S 2018-12-07 this unit test is broken and needs some fixing
        //STEP 6
        [TestMethod]
        public void CREATE_ScoFile()
        {
            ActionGenerateAndTransportECS ecs = new ActionGenerateAndTransportECS();
            CommonStatus result = new CommonStatus(true);

            //result = ecs.Execute(null);

            Assert.IsNotNull(result);
        }
         */

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
    }
}
