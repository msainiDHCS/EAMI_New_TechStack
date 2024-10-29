using OHC.EAMI.WebUI.ViewModels;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.Common;
using System.Web.Mvc.Filters;
using MoreLinq;
using OHC.EAMI.WebUI.Filters;
using System.Net;
using OHC.EAMI.WebUI.Controllers;

namespace OHC.EAMI.WebUI.Data
{
    public static class PaymentRecordQueries
    {
        private static readonly string _assignPaymentRecordCacheKey = HttpContext.Current.Session.SessionID + "_AssignPaymentRecord";

        private enum enumPaymentRecordStatus { UNASSIGNED=0, ASSIGNED=1 };

        public static List<PaymentSuperGroup> GetFromOrPutInCache_ListPaymentSuperGroup(string system, string paymentRecordStatusId, bool forceGetFromService)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup_FromService = null;
            List<PaymentSuperGroup> listPaymentSuperGroup_FromCache = null;
            int intPaymentRecordStatusId = Convert.ToInt32(paymentRecordStatusId);
            bool boolPaymentRecordStatusId = Convert.ToBoolean(intPaymentRecordStatusId);

            listPaymentSuperGroup_FromCache = CacheManager<List<PaymentSuperGroup>>
                .Get(_assignPaymentRecordCacheKey, Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(boolPaymentRecordStatusId)));  // get from cache


            if (listPaymentSuperGroup_FromCache == null || forceGetFromService)    //if not in cache or forceGetFromService, repopulate cache from service.
            {
                if (intPaymentRecordStatusId == 0 || intPaymentRecordStatusId == 1)
                {
                    CacheManager<List<PaymentSuperGroup>>.Remove(_assignPaymentRecordCacheKey,
                        Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(!boolPaymentRecordStatusId)));

                    WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                    CommonStatusPayload < List < PaymentSuperGroup >> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<PaymentSuperGroup>>>(
                            svc => svc.GetPaymentSuperGroupsForAssignment(boolPaymentRecordStatusId));     //get from db via service
                    ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
                    listPaymentSuperGroup_FromService = cs.Payload;

                    if (CacheManager<List<PaymentSuperGroup>>.Set(listPaymentSuperGroup_FromService, _assignPaymentRecordCacheKey,
                        Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(boolPaymentRecordStatusId)), 20))
                    {
                        listPaymentSuperGroup_FromCache = CacheManager<List<PaymentSuperGroup>>
                            .Get(_assignPaymentRecordCacheKey, Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(boolPaymentRecordStatusId)));  // get from cache
                    }
                }
                else
                {
                    throw new Exception("intPaymentRecordStatusId is not 0 or 1.");
                }

            }

            return listPaymentSuperGroup_FromCache;
        }

        public static List<RefCode> GetExclusivePaymentTypesList()
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<RefCodeList>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>(
                    svc => svc.GetReferenceCodeList(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            RefCodeList retRefCodeList = cs.Payload[0];
            retRefCodeList.RemoveAll(rc => rc.Code == "NONE" || !rc.IsActive);
            // if straight apostrophe is desired, use "&apos;" or "&#39;"
            retRefCodeList.ForEach(rc => rc.Description = rc.Description.Replace("'", "&#146;")); //curly apostrophe
            return retRefCodeList.OrderBy(rc => rc.Code).ToList();
        }

        public static List<AssignPaymentRecordsViewModel> GetListSearchResults(string system, string paymentRecordStatusId,
                                                            string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(system, paymentRecordStatusId, false);

            List<PaymentSuperGroup> listPaymentSuperGroup_FilteredBySelectedPayees =
                    listPaymentSuperGroup.Where(psg => (arrayPayeeName) == null || (arrayPayeeName).Contains(psg.PayeeInfo.PEE_Name)).ToList<PaymentSuperGroup>();

            List<AssignPaymentRecordsViewModel> retList = new List<AssignPaymentRecordsViewModel>();

            foreach (var paymentSuperGroup in listPaymentSuperGroup_FilteredBySelectedPayees)
            {
                if ((arrayPaymentTypeName == null || arrayPaymentTypeName.Contains(paymentSuperGroup.PaymentType)) &&
                    (arrayContractNumber == null || arrayContractNumber.Contains(paymentSuperGroup.ContractNumber)))
                {
                    AssignPaymentRecordsViewModel assignPaymentRecordsViewModel = new AssignPaymentRecordsViewModel();
                    assignPaymentRecordsViewModel.PaymentSuperGroup_UniqueKey = paymentSuperGroup.UniqueKey;
                    assignPaymentRecordsViewModel.HasItemsOnHold = paymentSuperGroup.HasItemsOnHold();
                    assignPaymentRecordsViewModel.HasExclusivePaymentType = paymentSuperGroup.HasExclusivePaymentType();
                    assignPaymentRecordsViewModel.PayeeName = paymentSuperGroup.PayeeInfo.PEE_Name;
                    assignPaymentRecordsViewModel.PayeeFullCode = paymentSuperGroup.PayeeInfo.PEE_FullCode;
                    assignPaymentRecordsViewModel.PaymentTypeName = paymentSuperGroup.PaymentType;
                    assignPaymentRecordsViewModel.ContractNumber = paymentSuperGroup.ContractNumber;
                    assignPaymentRecordsViewModel.SFY = paymentSuperGroup.FiscalYear;
                    assignPaymentRecordsViewModel.TotalAmount = paymentSuperGroup.PaymentGroupList.Sum(pg => pg.Amount);
                    retList.Add(assignPaymentRecordsViewModel);
                }
            }
            return retList;
        }

        public static bool ForceCacheRefresh(string system, string paymentRecordStatusId, bool forceGetFromService)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup_FromCache = null;
            listPaymentSuperGroup_FromCache = GetFromOrPutInCache_ListPaymentSuperGroup(system, paymentRecordStatusId, forceGetFromService);
            return listPaymentSuperGroup_FromCache != null;
        }

        public static List<PayeeEntity> GetListSearchCriteriaPayees(string system, string paymentRecordStatusId,
                                                            string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(system, paymentRecordStatusId, false);
            return listPaymentSuperGroup.Select(psg => psg.PayeeInfo).DistinctBy(PayeeInfo => PayeeInfo.PEE_Name).OrderBy(PayeeInfo => PayeeInfo.PEE_Name)
                                        .Select(PayeeInfo => PayeeInfo.PEE).ToList();
        }

        public static List<string> GetListSearchCriteriaPaymentTypes(string system, string paymentRecordStatusId,
                                                            string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(system, paymentRecordStatusId, false);
            List<PaymentSuperGroup> listPaymentSuperGroup_FilteredBySelectedPayees =
                    listPaymentSuperGroup.Where(psg => arrayPayeeName == null || arrayPayeeName.Contains(psg.PayeeInfo.PEE_Name)).ToList();

            return listPaymentSuperGroup_FilteredBySelectedPayees.Select(psg => psg.PaymentType)
                            .Distinct().OrderBy(PaymentType => PaymentType).ToList();
        }

        public static List<string> GetListSearchCriteriaContractNumbers(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(system, paymentRecordStatusId, false);

            List<PaymentSuperGroup> listPaymentSuperGroup_FilteredBySelectedPayeesAndPaymentTypes =
                    listPaymentSuperGroup.Where(
                        psg => (arrayPayeeName == null || arrayPayeeName.Contains(psg.PayeeInfo.PEE_Name)) &&
                                (arrayPaymentTypeName == null || arrayPaymentTypeName.Contains(psg.PaymentType))).ToList();
            return listPaymentSuperGroup_FilteredBySelectedPayeesAndPaymentTypes.Select(psg => psg.ContractNumber)
                            .Distinct().OrderBy(ContractNumber => ContractNumber).ToList();
        }

        public static AssignPaymentRecordsViewModel GetPaymentGroups(AssignPaymentRecordsViewModel aprvm)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(
                HttpContext.Current.Session["system"].ToString(), HttpContext.Current.Session["paymentRecordStatusId"].ToString(), false);

            PaymentSuperGroup paymentSuperGroup = listPaymentSuperGroup.FirstOrDefault(psg => aprvm.PaymentSuperGroup_UniqueKey == psg.UniqueKey &&
                                                                                                aprvm.PayeeFullCode == psg.PayeeInfo.PEE_FullCode &&
                                                                                                aprvm.PaymentTypeName == psg.PaymentType &&
                                                                                                aprvm.ContractNumber == psg.ContractNumber &&
                                                                                                aprvm.SFY == psg.FiscalYear);
            if (paymentSuperGroup != null)
            {
                aprvm.PaymentGroups = paymentSuperGroup.PaymentGroupList.AsQueryable();
            }
            return aprvm;
        }

        public static PaymentGroup GetPaymentRecs(AssignPaymentRecordsViewModel aprvm, string paymentGroup_UniqueNumber)
        {
            List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(
                HttpContext.Current.Session["system"].ToString(), HttpContext.Current.Session["paymentRecordStatusId"].ToString(), false);

            PaymentSuperGroup paymentSuperGroup = listPaymentSuperGroup.FirstOrDefault(psg => aprvm.PaymentSuperGroup_UniqueKey == psg.UniqueKey &&
                                                                                                aprvm.PayeeFullCode == psg.PayeeInfo.PEE_FullCode &&
                                                                                                aprvm.PaymentTypeName == psg.PaymentType &&
                                                                                                aprvm.ContractNumber == psg.ContractNumber &&
                                                                                                aprvm.SFY == psg.FiscalYear);
            PaymentGroup retPG = null;
            if (paymentSuperGroup != null)
            {
                retPG = paymentSuperGroup.PaymentGroupList.FirstOrDefault(pg => paymentGroup_UniqueNumber == pg.UniqueNumber);
            }
            return retPG;
        }

        public static JSONReturnStatus AssignPaymentGroups(List<PaymentGroup> listPaymentGroup, string user, bool isUnAssign)
        {
            CommonStatus commonStatus_FromService = null;
            int intPaymentRecordStatusId = Convert.ToInt32(HttpContext.Current.Session["paymentRecordStatusId"].ToString());
            bool boolPaymentRecordStatusId = Convert.ToBoolean(intPaymentRecordStatusId);
            JSONReturnStatus jSONReturnStatus = null;

            if (intPaymentRecordStatusId == 0 || intPaymentRecordStatusId == 1)
            {
                CacheManager<List<PaymentSuperGroup>>.Remove(_assignPaymentRecordCacheKey,
                    Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(!boolPaymentRecordStatusId)));

                WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                switch (intPaymentRecordStatusId)
                {
                    case 0:
                        commonStatus_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(
                                                        svc => svc.AssignPaymentGroups(listPaymentGroup, user));     // get from db via service
                        break;
                    case 1:
                        if (isUnAssign)
                        {
                            commonStatus_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(
                                                        svc => svc.UnAssignPaymentGroups(listPaymentGroup, user));     // get from db via service
                        }
                        else
                        {
                            commonStatus_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(
                                                        svc => svc.ReAssignPaymentGroups(listPaymentGroup, user));     // get from db via service
                        }
                        break;
                    default:
                        break;
                }
                ErrorHandlerController.CheckFatalException(commonStatus_FromService.Status, commonStatus_FromService.IsFatal, commonStatus_FromService.GetCombinedMessage());

                jSONReturnStatus = JSONReturnStatus.GetStatus(commonStatus_FromService);
                if (jSONReturnStatus.status)
                {
                    // force set to cache
                    List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(
                        HttpContext.Current.Session["system"].ToString(), HttpContext.Current.Session["paymentRecordStatusId"].ToString(), true);
                }
            }
            return jSONReturnStatus;
        }

        public static JSONReturnStatus ReturnPaymentGroupsToSystemOfRecord(string note, List<PaymentGroup> listPaymentGroup, string user)
        {
            CommonStatus commonStatus_FromService = null;
            int intPaymentRecordStatusId = Convert.ToInt32(HttpContext.Current.Session["paymentRecordStatusId"].ToString());
            bool boolPaymentRecordStatusId = Convert.ToBoolean(intPaymentRecordStatusId);
            List<string> priorityGroupHashList = listPaymentGroup.Select(pg => pg.UniqueNumber).ToList<string>();
            JSONReturnStatus jSONReturnStatus = null;

            if (intPaymentRecordStatusId == 0 || intPaymentRecordStatusId == 1)
            {
                CacheManager<List<PaymentSuperGroup>>.Remove(_assignPaymentRecordCacheKey,
                    Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(!boolPaymentRecordStatusId)));

                WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                commonStatus_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(
                                        svc => svc.ReturnPaymentGroupsToSystemOfRecord(priorityGroupHashList, user, note));     //get from db via service
                ErrorHandlerController.CheckFatalException(commonStatus_FromService.Status, commonStatus_FromService.IsFatal, commonStatus_FromService.GetCombinedMessage());

                jSONReturnStatus = JSONReturnStatus.GetStatus(commonStatus_FromService);
                if (jSONReturnStatus.status)
                {
                    // force set to cache
                    List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(
                        HttpContext.Current.Session["system"].ToString(), HttpContext.Current.Session["paymentRecordStatusId"].ToString(), true);
                }
            }
            return jSONReturnStatus;
        }

        public static List<string> GetDates(enRefTables chosenEnumValue)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<RefCodeList>> listCommonStatusPayload_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>(
                svc => svc.GetReferenceCodeList(chosenEnumValue));     //get from db via service
            ErrorHandlerController.CheckFatalException(listCommonStatusPayload_FromService.Status, listCommonStatusPayload_FromService.IsFatal, listCommonStatusPayload_FromService.GetCombinedMessage());

            return listCommonStatusPayload_FromService.Payload.FirstOrDefault().Select(x => x.Code).ToList();
        }
        
        public static CommonStatus HoldUnholdPaymentGroups(string holdStatusToSet, string note, List<PaymentGroup> listPaymentGroup, string user)
        {
            CommonStatus commonStatus_FromService = null;
            int intPaymentRecordStatusId = Convert.ToInt32(HttpContext.Current.Session["paymentRecordStatusId"].ToString());
            bool boolPaymentRecordStatusId = Convert.ToBoolean(intPaymentRecordStatusId);
            bool boolHoldStatusToSet = Convert.ToBoolean(holdStatusToSet);
            List<string> priorityGroupHashList = listPaymentGroup.Select(pg => pg.UniqueNumber).ToList<string>();

            if (intPaymentRecordStatusId == 0 || intPaymentRecordStatusId == 1)
            {
                CacheManager<List<PaymentSuperGroup>>.Remove(_assignPaymentRecordCacheKey,
                    Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(!boolPaymentRecordStatusId)));

                if (boolHoldStatusToSet)
                {
                    WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                    commonStatus_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(
                                            svc => svc.SetPaymentGroupsToHold(priorityGroupHashList, user, note));     //get from db via service
                }
                else
                {
                    WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                    commonStatus_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>(
                                            svc => svc.SetPaymentGroupsToUnHold(priorityGroupHashList, user, note));     //get from db via service
                }

                if (commonStatus_FromService.Status)
                {
                    // force set to cache
                    List<PaymentSuperGroup> listPaymentSuperGroup = GetFromOrPutInCache_ListPaymentSuperGroup(
                        HttpContext.Current.Session["system"].ToString(), HttpContext.Current.Session["paymentRecordStatusId"].ToString(), true);

                }
            }
            ErrorHandlerController.CheckFatalException(commonStatus_FromService.Status, commonStatus_FromService.IsFatal, commonStatus_FromService.GetCombinedMessage());

            return commonStatus_FromService;
        }
    }
}

