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

namespace OHC.EAMI.WebUI.Data
{
    public static class PaymentRecordQueries_12_29_17
    {
        private static readonly string _assignPaymentRecordCacheKey = HttpContext.Current.Session.SessionID + "_AssignPaymentRecord";

        private enum enumPaymentRecordStatus { UNASSIGNED=0, ASSIGNED=1 };

        //public static bool PutInCache_ListUnassignedPaymentRecsGroupedByPayeeAndPaymentType(string strCacheKey, string strCacheScope)
        public static List<EntityGroup<RefCode, PaymentRecGroup<string>>> GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType(
                                                            string system, string paymentRecordStatusId)
        {
            //try
            //{
            //    //string _assignPaymentRecordCacheKey = HttpContext.Current.Session.SessionID + "_AssignPaymentRecord";

            //    WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            //    List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType = 
            //        _wcfService.InvokeService<IEAMIWebUIDataService, List<EntityGroup<RefCode, PaymentRecGroup<string>>>>(
            //            svc => svc.GetPaymentRecsForAssignmentGroupedByPayeeAndPaymentType(assigned));

            //    CacheManager<List<EntityGroup<RefCode, PaymentRecGroup<string>>>>.Remove(_assignPaymentRecordCacheKey, 
            //        Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(!assigned)));
            //    return CacheManager<List<EntityGroup<RefCode, PaymentRecGroup<string>>>>.Set(listPaymentRecsGroupedByPayeeAndPaymentType, _assignPaymentRecordCacheKey, 
            //        Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(assigned)), 20);
            //}
            //catch (Exception e)
            //{
            //    //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
            //    throw;
            //}
            try
            {
                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType_FromService = null;
                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType_FromCache = null;
                int intPaymentRecordStatusId = Convert.ToInt32(paymentRecordStatusId);
                bool boolPaymentRecordStatusId = Convert.ToBoolean(intPaymentRecordStatusId);



                listPaymentRecsGroupedByPayeeAndPaymentType_FromCache = CacheManager<List<EntityGroup<RefCode, PaymentRecGroup<string>>>>
                    .Get(_assignPaymentRecordCacheKey, Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(boolPaymentRecordStatusId)));  // get from cache

                if (listPaymentRecsGroupedByPayeeAndPaymentType_FromCache == null)    //if not in cache, repopulate cache from service.
                {
                    if (intPaymentRecordStatusId == 0 || intPaymentRecordStatusId == 1)
                    {
                        CacheManager<List<EntityGroup<RefCode, PaymentRecGroup<string>>>>.Remove(_assignPaymentRecordCacheKey,
                            Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(!boolPaymentRecordStatusId)));

                        WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                        listPaymentRecsGroupedByPayeeAndPaymentType_FromService = _wcfService.InvokeService<IEAMIWebUIDataService, List<EntityGroup<RefCode, PaymentRecGroup<string>>>>(
                                svc => svc.GetPaymentRecsForAssignmentGroupedByPayeeAndPaymentType(boolPaymentRecordStatusId));     //get from db via service

                        if (CacheManager<List<EntityGroup<RefCode, PaymentRecGroup<string>>>>.Set(listPaymentRecsGroupedByPayeeAndPaymentType_FromService, _assignPaymentRecordCacheKey,
                            Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(boolPaymentRecordStatusId)), 20))
                        {
                            listPaymentRecsGroupedByPayeeAndPaymentType_FromCache = CacheManager<List<EntityGroup<RefCode, PaymentRecGroup<string>>>>
                                .Get(_assignPaymentRecordCacheKey, Enum.GetName(typeof(enumPaymentRecordStatus), Convert.ToInt32(boolPaymentRecordStatusId)));  // get from cache
                        }
                    }
                    else
                    {
                        throw new Exception("intPaymentRecordStatusId is not 0 or 1.");
                    }

                }

                return listPaymentRecsGroupedByPayeeAndPaymentType_FromCache;
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }

        //public static bool GetListAssignPaymentRecordsViewModel(string strCacheKey, string strCacheScope)
        public static List<AssignPaymentRecordsViewModel> GetListSearchResults(string system, string paymentRecordStatusId,
                                                            string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            try
            {

                //string[] arrayPayeeName = { "Access Dental Plan" };

                //var cars = ["Saab", "Volvo", "BMW"];



                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType = 
                    GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType(system, paymentRecordStatusId);

                //var listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees =
                //        listPaymentRecsGroupedByPayeeAndPaymentType.Where(preg => arrayPayeeName == null || arrayPayeeName.Contains(preg.ParentItem.Code));
                var listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees =
                        listPaymentRecsGroupedByPayeeAndPaymentType.Where(preg => arrayPayeeName == null || arrayPayeeName.Contains(preg.ParentItem.Description));



                List<AssignPaymentRecordsViewModel> retList = new List<AssignPaymentRecordsViewModel>();

                foreach (EntityGroup<RefCode, PaymentRecGroup<string>> paymentRecEntityGroup in listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees)
                {
                    foreach (var paymentRecGroup in paymentRecEntityGroup.ListItems)
                    {
                        if ((arrayPaymentTypeName == null || arrayPaymentTypeName.Contains(paymentRecGroup.ParentItem)) &&
                            (arrayContractNumber == null ||
                            arrayContractNumber.Where(arraycnum => paymentRecGroup.ListItems.Select(x => x.ContractNumber).Any(prglcnum => arraycnum.Contains(prglcnum))).Any()))
                        {
                            AssignPaymentRecordsViewModel assignPaymentRecordsViewModel = new AssignPaymentRecordsViewModel();
                            assignPaymentRecordsViewModel.PayeeDisplayId = listPaymentRecsGroupedByPayeeAndPaymentType.IndexOf(paymentRecEntityGroup).ToString() + "00" +
                                                                           paymentRecEntityGroup.ListItems.IndexOf(paymentRecGroup).ToString();
                            assignPaymentRecordsViewModel.PayeeRefCode = paymentRecEntityGroup.ParentItem.Code;    //This is Payee Code (is hidden field in browser).
                            assignPaymentRecordsViewModel.PayeeName = paymentRecEntityGroup.ParentItem.Description;
                            assignPaymentRecordsViewModel.PaymentTypeName = paymentRecGroup.ParentItem;
                            assignPaymentRecordsViewModel.TotalAmount = paymentRecGroup.ListItems.Sum(x => x.Amount);

                            retList.Add(assignPaymentRecordsViewModel);
                        }
                    }
                }
                return retList;
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }

        public static List<RefCode> GetListSearchCriteriaPayees(string system, string paymentRecordStatusId,
                                                            string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            try
            {
                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType = 
                    GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType(system, paymentRecordStatusId);
                return listPaymentRecsGroupedByPayeeAndPaymentType.Select(preg => preg.ParentItem)
                        .DistinctBy(Payee => Payee.Description).OrderBy(Payee => Payee.Description).ToList();
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }

        public static List<string> GetListSearchCriteriaPaymentTypes(string system, string paymentRecordStatusId,
                                                            string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            try
            {
                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType =
                    GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType(system, paymentRecordStatusId);

                var listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees =
                        listPaymentRecsGroupedByPayeeAndPaymentType.Where(preg => arrayPayeeName == null || arrayPayeeName.Contains(preg.ParentItem.Description));

                var enumerablePaymentRecGroup = listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees.SelectMany(
                                                            preg => preg.ListItems);

                return enumerablePaymentRecGroup.Select(prg => prg.ParentItem)
                                .Distinct().OrderBy(ParentItem => ParentItem).ToList();
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }

        public static List<string> GetListSearchCriteriaContractNumbers(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            try
            {
                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType =
                    GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType(system, paymentRecordStatusId);

                var listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees =
                        listPaymentRecsGroupedByPayeeAndPaymentType.Where(preg => arrayPayeeName == null || arrayPayeeName.Contains(preg.ParentItem.Description));

                var enumerablePaymentRecGroup = listPaymentRecsGroupedByPayeeAndPaymentType_FilteredBySelectedPayees.SelectMany(
                                                            preg => preg.ListItems);

                var enumerablePaymentRecGroup_FilteredBySelectedPaymentTypes =
                        enumerablePaymentRecGroup.Where(prg => arrayPaymentTypeName == null || arrayPaymentTypeName.Contains(prg.ParentItem));

                var enumerablePaymentRec = enumerablePaymentRecGroup_FilteredBySelectedPaymentTypes.SelectMany(prg => prg.ListItems);

                return enumerablePaymentRec.Select(pr => pr.ContractNumber)
                                .Distinct().OrderBy(ContractNumber => ContractNumber).ToList();
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }

        public static AssignPaymentRecordsViewModel GetPaymentRecords(string payeeDisplayId, string payeeRefCode, string paymentTypeName)
        {
            try
            {




                //List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType = GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType(
                //                                            system, paymentRecordStatusId);
                List<EntityGroup<RefCode, PaymentRecGroup<string>>> listPaymentRecsGroupedByPayeeAndPaymentType = GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType("CAPMAN", "0");



                List<AssignPaymentRecordsViewModel> retList = new List<AssignPaymentRecordsViewModel>();

                var listPayeeSpecificPaymentRecsGroupedByPayeeAndPaymentType =
                    listPaymentRecsGroupedByPayeeAndPaymentType.Where(preg => preg.ParentItem.Code.Equals(payeeRefCode));

                var enumerablePaymentRecGroup = listPayeeSpecificPaymentRecsGroupedByPayeeAndPaymentType.SelectMany(
                                                            paymentRecEntityGroup => paymentRecEntityGroup.ListItems
                                                            .Where(listItem => listItem.ParentItem.Equals(paymentTypeName)));

                AssignPaymentRecordsViewModel assignPaymentRecordsViewModel = new AssignPaymentRecordsViewModel();
                assignPaymentRecordsViewModel.PayeeDisplayId = payeeDisplayId;
                assignPaymentRecordsViewModel.PayeeRefCode = payeeRefCode;
                assignPaymentRecordsViewModel.PaymentTypeName = paymentTypeName;
                List<PaymentRec> listPaymentRecs = null;

                foreach (var PaymentRecGroup in enumerablePaymentRecGroup)
                {
                    // This loop only runs once since enumerablePaymentRecGroup has only 1 item due to
                    // payeeRefCode and paymentTypeName filtering to only 1 list of PaymentRec's.
                    // foreach loop needed to trigger linq query above.
                    listPaymentRecs = PaymentRecGroup.ListItems;
                }

                assignPaymentRecordsViewModel.PaymentRecords = listPaymentRecs.AsQueryable();
                return assignPaymentRecordsViewModel;
            }
            catch (Exception e)
            {
                //TraceSourceWriter.TraceError(e.TargetSite.DeclaringType.Namespace, e.ToString());
                throw;
            }
        }
        


    }
}

