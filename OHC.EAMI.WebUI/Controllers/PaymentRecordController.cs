using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Data;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;
using OHC.EAMI.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;


namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.SUPERVISOR)]
    public class PaymentRecordController : BaseController
    {
        private readonly WcfServiceInvoker _wcfService;
        public PaymentRecordController()
        {
            _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<RefCodeList>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>(
                    svc => svc.GetReferenceCodeList(enRefTables.TB_SCO_PROPERTY));

            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            SCOSetting scoSetting = new SCOSetting();
            scoSetting = cs.Payload[0].GetRefCodeByCode<SCOSetting>("PROCESS_WARRANT_PAYMENTS");
            bool processWarrants = bool.Parse(scoSetting.SCOSettingValue);
            ViewBag.ProcessWarrants = processWarrants;
        }
        public ActionResult AssignPaymentRecords_GetAssignees()
        {
            return Json(AdministrationQueries.GetAssignees(base.GetLoggedinUserID()));
        }

        [AjaxOnly]
        public ActionResult AssignPaymentRecords_GetPaymentGroups(AssignPaymentRecordsViewModel aprvm)
        {
            return PartialView("_PaymentGroupsPartial", PaymentRecordQueries.GetPaymentGroups(aprvm));
        }

        [AjaxOnly]
        public ActionResult AssignPaymentRecords_GetPaymentRecs(AssignPaymentRecordsViewModel aprvm, string paymentGroup_UniqueNumber)
        {
            return PartialView("_PaymentRecsPartial", PaymentRecordQueries.GetPaymentRecs(aprvm, paymentGroup_UniqueNumber));
        }

        public JsonResult AssignPaymentRecords_AssignPaymentGroups(List<PaymentGroup> pgList, bool isUnAssign)
        {
            return Json(PaymentRecordQueries.AssignPaymentGroups(pgList, User.Identity.Name, isUnAssign));
        }

        public JsonResult AssignPaymentRecords_ReturnPaymentGroupsToSystemOfRecord(string note, List<PaymentGroup> pgList)
        {
            return Json(PaymentRecordQueries.ReturnPaymentGroupsToSystemOfRecord(note, pgList, User.Identity.Name));
        }

        public ActionResult AssignPaymentRecords()
        {
            ViewBag.SystemsRefCodeList = base.GetReferenceCodes(enRefTables.TB_SYSTEM_OF_RECORD).OrderBy(s => s.ID);
            ViewBag.ExclusivePaymentTypesRefCodeList = PaymentRecordQueries.GetExclusivePaymentTypesList();

            List<AssignPaymentRecordsViewModel> listAssignPaymentRecordsViewModel = new List<AssignPaymentRecordsViewModel>();
            return View(listAssignPaymentRecordsViewModel.AsQueryable<AssignPaymentRecordsViewModel>());
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult AssignPaymentRecords_Search(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            Session["system"] = system;
            Session["paymentRecordStatusId"] = paymentRecordStatusId;
            Session["arrayPayeeName"] = arrayPayeeName;
            Session["arrayPaymentTypeName"] = arrayPaymentTypeName;
            Session["arrayContractNumber"] = arrayContractNumber;
            return PartialView("_AssignPaymentRecords_SearchPartial", PaymentRecordQueries.GetListSearchResults(system, paymentRecordStatusId,
                                        arrayPayeeName, arrayPaymentTypeName, arrayContractNumber).AsQueryable<AssignPaymentRecordsViewModel>());
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_ForceCacheRefresh(string system, string paymentRecordStatusId, bool forceGetFromService)
        {
            return Json(PaymentRecordQueries.ForceCacheRefresh(system, paymentRecordStatusId, forceGetFromService), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_GetSearchCriteriaPayees(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            return Json(PaymentRecordQueries.GetListSearchCriteriaPayees(system, paymentRecordStatusId,
                                                        arrayPayeeName, arrayPaymentTypeName, arrayContractNumber), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_GetSearchCriteriaContracts(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            return Json(PaymentRecordQueries.GetListSearchCriteriaContractNumbers(system, paymentRecordStatusId,
                                                        arrayPayeeName, arrayPaymentTypeName, arrayContractNumber), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_GetSearchCriteriaPaymentTypes(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            return Json(PaymentRecordQueries.GetListSearchCriteriaPaymentTypes(system, paymentRecordStatusId,
                                                        arrayPayeeName, arrayPaymentTypeName, arrayContractNumber), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_GetPayDates()
        {
            return Json(PaymentRecordQueries.GetDates(enRefTables.TB_PAYDATE_CALENDAR), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_GetDrawDates()
        {
            return Json(PaymentRecordQueries.GetDates(enRefTables.TB_DRAWDATE_CALENDAR), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignPaymentRecords_HoldUnholdPaymentGroups(string holdStatusToSet, string note, List<PaymentGroup> pgList)
        {
            return Json(PaymentRecordQueries.HoldUnholdPaymentGroups(holdStatusToSet, note, pgList, User.Identity.Name));
        }
    }
}