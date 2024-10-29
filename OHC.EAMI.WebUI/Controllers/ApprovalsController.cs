using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using OHC.EAMI.WebUI.Data;
using Microsoft.Security.Application;


namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.SUPERVISOR)]
    public class ApprovalsController : BaseController
    {
        private readonly WcfServiceInvoker _wcfService;

        public ApprovalsController()
        {
            _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<List<RefCodeList>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>(
                    svc => svc.GetReferenceCodeList(enRefTables.TB_SCO_PROPERTY));

            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            SCOSetting scoSetting = new SCOSetting();
            scoSetting = cs.Payload[0].GetRefCodeByCode<SCOSetting>("PROCESS_WARRANT_PAYMENTS");
            bool processWarrants = bool.Parse(scoSetting.SCOSettingValue);
            ViewBag.ProcessWarrants = processWarrants;
            ViewBag.ExclusivePaymentTypesRefCodeList = PaymentRecordQueries.GetExclusivePaymentTypesList();
        }

        #region Public Methods
        public ActionResult Index()
        {
            return View();
        }

        #region Pending Return Methods

        [AjaxOnly]
        public ActionResult PendingReturnIndex()
        {
            //Search Criteria Reference Code
            PaymentProcessingFilters filter = new PaymentProcessingFilters();

            filter.Systems = new List<SelectListItem>();
            foreach (RefCode x in base.GetReferenceCodes(enRefTables.TB_SYSTEM_OF_RECORD))
                filter.Systems.Add(new SelectListItem() { Text = x.Code, Value = x.ID.ToString() });


            filter.PayeeNames = new List<SelectListItem>();
            filter.PaymentTypes = new List<SelectListItem>();
            filter.ContractNumbers = new List<SelectListItem>();
            GetCounters();
            return PartialView("~/Views/Approvals/PendingReturns/PendingReturnIndex.cshtml", filter);
        }

        [HttpPost]
        public JsonResult GetPendingReturnFilterValues(string type, string parentValue, string childValue)
        {
            type = Sanitizer.GetSafeHtmlFragment(type);
            parentValue = Sanitizer.GetSafeHtmlFragment(parentValue);
            childValue = Sanitizer.GetSafeHtmlFragment(childValue);

            List<PaymentSuperGroup> psgs = GetPendingReturns();
            List<Tuple<string, string>> lst = new List<Tuple<string, string>>();
            var parentValueStrArray = parentValue.Split(',');
            var childValueStrArray = childValue.Split(',');

            if (psgs != null && psgs.Count() > 0)
            {
                if (type == "PAYEE")
                {
                    var payeelist = psgs.Select(a => a.PayeeInfo).Distinct().Select
                        (a =>
                        new Tuple<string, string>(a.PEE_Name, a.PEE.ID.ToString())
                    ).OrderByDescending(a => a.Item1);

                    string previousItem1 = "";
                    string previousItem2 = "";
                    string newItem2 = "";
                    var uniquePayeeList = new List<Tuple<string, string>>();
                    foreach (var tuple in payeelist)
                    {
                        if (!tuple.Item1.Equals(previousItem1))
                        {
                            uniquePayeeList.Add(tuple);
                            previousItem1 = tuple.Item1;
                            previousItem2 = tuple.Item2;
                        }
                        else
                        {
                            uniquePayeeList.RemoveAll(t => t.Item1 == previousItem1);
                            newItem2 = previousItem2 + "_" + tuple.Item2;
                            uniquePayeeList.Add(new Tuple<string, string>(tuple.Item1, newItem2));
                            previousItem1 = tuple.Item1;
                            previousItem2 = newItem2;
                        }
                    }

                    lst.AddRange(uniquePayeeList);
                }
                else if (type == "PAYMENTTYPE")
                {
                    List<PaymentSuperGroup> psgsList = psgs.Where(t => parentValueStrArray.Select(int.Parse).ToList().Contains(t.PayeeInfo.PEE.ID)).ToList();
                    if (psgsList.Count > 0)
                    {
                        var paymentTypeList = psgsList.Select(a => a.PaymentType).Distinct().Select
                            (a =>
                            new Tuple<string, string>(a, a)
                        ).OrderByDescending(a => a.Item1);

                        lst.AddRange(paymentTypeList);
                    }
                }

                else if (type == "CONTRACTNUMBER")
                {
                    List<PaymentSuperGroup> psgsList = psgs.Where(t => parentValueStrArray.Select(int.Parse).ToList().Contains(t.PayeeInfo.PEE.ID)).ToList();
                    if (psgsList.Count > 0)
                    {
                        List<PaymentSuperGroup> innerPSGSList = psgsList.Where(a => childValueStrArray.ToList().Contains(a.PaymentType)).ToList();
                        if (innerPSGSList.Count > 0)
                        {
                            var contractNumberList = innerPSGSList.Select(a => a.ContractNumber).Distinct().Select
                                        (a => new Tuple<string, string>(a, a)).OrderByDescending(a => a.Item1);

                            lst.AddRange(contractNumberList);
                        }
                    }
                }
            }

            var tlistItems = lst.Select(a =>
              new
              {
                  text = a.Item1,
                  value = a.Item2
              }
          );

            return Json(tlistItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PendingReturnSuperGroup(string system, int[] payeeNameIDs, string[] paymentTypeValues, string[] contractNumberValues)
        {
            PaymentProcessingSearchResults cls = new PaymentProcessingSearchResults();
            if (!string.IsNullOrEmpty(system))
            {
                List<PaymentProcessingRecordMaster> lst = new List<PaymentProcessingRecordMaster>();
                List<PaymentSuperGroup> psgs = GetPendingReturns();

                if (psgs != null && psgs.Count > 0)
                {

                    //Apply Search Criteria                
                    if (payeeNameIDs != null && payeeNameIDs.Count() > 0)
                        psgs = (from p in psgs where payeeNameIDs.Contains(p.PayeeInfo.PEE.ID) select p).ToList<PaymentSuperGroup>();

                    if (paymentTypeValues != null && paymentTypeValues.Count() > 0)
                        psgs = (from p in psgs where paymentTypeValues.Contains(p.PaymentType) select p).ToList<PaymentSuperGroup>();

                    if (contractNumberValues != null && contractNumberValues.Count() > 0)
                        psgs = (from p in psgs where contractNumberValues.Contains(p.ContractNumber) select p).ToList<PaymentSuperGroup>();

                    foreach (PaymentSuperGroup g in psgs)
                    {
                        PaymentProcessingRecordMaster m = new PaymentProcessingRecordMaster();

                        m.GroupIdentifier = g.UniqueKey;
                        m.PayeeName = g.PayeeInfo.PEE_Name;
                        m.PayeeCode = g.PayeeInfo.PEE.Code;
                        m.PayeeSuffix = g.PayeeInfo.PEE_IdSfx;
                        m.PaymentType = g.PaymentType;
                        m.ContractNumber = g.ContractNumber;
                        m.FiscalYear = g.FiscalYear;
                        m.TotalPaymentDollars = Double.Parse(g.Amount.ToString());
                        m.HasExclusivePaymentType = g.HasExclusivePaymentType();
                        lst.Add(m);
                    }

                    cls.masterDataList = lst;
                }
            }
            return PartialView("~/Views/Approvals/PendingReturns/PendingReturnSuperGroup.cshtml", cls);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PendingReturnPaymentGroup(string id, string paymentGroupName)
        {
            List<PaymentProcessingRecordChild> lst = new List<PaymentProcessingRecordChild>();
            List<PaymentSuperGroup> psgs = GetPendingReturns();

            if (psgs != null)
            {
                var pgList = psgs.Where(a => a.UniqueKey == id).FirstOrDefault().PaymentGroupList;

                if (pgList != null)
                {
                    foreach (PaymentGroup pg in pgList)
                    {
                        PaymentProcessingRecordChild prsetinstance = new PaymentProcessingRecordChild();

                        prsetinstance.PaymentRecNumberSetNumber = pg.UniqueNumber;
                        prsetinstance.AssignedPaymentDate = Convert.ToDateTime(pg.PayDate.Code);
                        prsetinstance.AssignedUser = pg.AssignedUser.Display_Name;
                        prsetinstance.PaymentSetTotalAmount = Convert.ToDouble(pg.Amount);
                        prsetinstance.Return_StatusNote = pg.CurrentStatus.StatusNote;
                        prsetinstance.Return_CreatedBy = pg.CurrentStatus.CreatedBy;
                        prsetinstance.Return_StatusDate = pg.CurrentStatus.StatusDate;
                        prsetinstance.ExclusivePaymentType_Code = pg.ExclusivePaymentType.Code;
                        prsetinstance.Payment_Method_Type = pg.PaymentMethodType.Code;
                        lst.Add(prsetinstance);
                    }

                }
            }

            ViewBag.ID = id;
            ViewBag.PaymentGroupName = paymentGroupName;

            return PartialView("~/Views/Approvals/PendingReturns/PendingReturnPaymentGroup.cshtml", lst);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PendingReturnPaymentRecord(string paymentRecordSetNumber, string topGroupID, string paymentGroupName)
        {
            List<PaymentProcessingRecord> lst = new List<PaymentProcessingRecord>();
            List<PaymentSuperGroup> psgs = GetPendingReturns();

            if (psgs != null)
            {
                var pgList = psgs.Where(a => a.UniqueKey == topGroupID).FirstOrDefault().PaymentGroupList;
                if (pgList != null)
                {
                    var prset = pgList.Where(a => a.UniqueNumber == paymentRecordSetNumber).FirstOrDefault();

                    if (prset != null && prset.PaymentRecordList != null)
                    {
                        foreach (PaymentRec pr in prset.PaymentRecordList)
                        {
                            PaymentProcessingRecord ppr = new PaymentProcessingRecord()
                            {
                                PaymentRecID = pr.PrimaryKeyID,
                                PaymentRecNumber = pr.UniqueNumber,
                                PaymentRecordDate = pr.PaymentDate,
                                PaymentSetNumber = prset.UniqueNumber,
                                Amount = Convert.ToDouble(pr.Amount)
                            };

                            lst.Add(ppr);
                        }
                    }
                }
            }
            string cleanPaymentRecordSetNumber = paymentRecordSetNumber.Replace('.', '_');
            ViewBag.gridPrefix = topGroupID + "_" + cleanPaymentRecordSetNumber;
            ViewBag.PaymentGroupName = paymentGroupName;
            ViewBag.TopGroupID = topGroupID;
            ViewBag.ParentPaymentRecordSetNumber = paymentRecordSetNumber;

            return PartialView("~/Views/Approvals/PendingReturns/PendingReturnPaymentRecord.cshtml", lst);
        }

        [HttpPost]
        public ActionResult PendingReturnFundingDetails(int paymentRecordID, string topGroupID, string parentPaymentRecordSetNumber)
        {
            List<PaymentSuperGroup> psgs = GetPendingReturns();
            return base.DisplayGroupFundingDetails(psgs, paymentRecordID, topGroupID, parentPaymentRecordSetNumber);
        }

        [HttpPost]
        public JsonResult PendingReturnApprovePaymentGroup(string paymentRecordSet, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            if ((!string.IsNullOrEmpty(paymentRecordSet) && !string.IsNullOrEmpty(paymentRecordSet.Trim())) && (!string.IsNullOrEmpty(note) && !string.IsNullOrEmpty(note.Trim())))
            {
                note = HttpUtility.HtmlEncode(note.Trim());

                List<string> priorityHashGroupList = new List<string>();
                priorityHashGroupList.Add(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.ReturnPaymentGroupsToSystemOfRecord(priorityHashGroupList, User.Identity.Name, note));

                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (paymentRecordSet == null || paymentRecordSet.Count() <= 0)
                    errorMessageList.Add("Please select at least one Payment Set");
                if (string.IsNullOrEmpty(note))
                    errorMessageList.Add("Please enter a comment");
                jsonReturnStatus.returnedData = errorMessageList;
            }

            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PendingReturnDenyPaymentGroup(string paymentRecordSet, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            if ((!string.IsNullOrEmpty(paymentRecordSet) && !string.IsNullOrEmpty(paymentRecordSet.Trim())) && (!string.IsNullOrEmpty(note) && !string.IsNullOrEmpty(note.Trim())))
            {
                note = HttpUtility.HtmlEncode(note.Trim());

                List<string> priorityHashGroupList = new List<string>();
                priorityHashGroupList.Add(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetPaymentGroupsToReleaseFromSup(priorityHashGroupList, User.Identity.Name, note));

                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (paymentRecordSet == null || paymentRecordSet.Count() <= 0)
                    errorMessageList.Add("Please select at least one Payment Set");
                if (string.IsNullOrEmpty(note))
                    errorMessageList.Add("Please enter a comment");
                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pending Claim Schedules Methods

        [AjaxOnly]
        public ActionResult PCSIndex()
        {
            PaymentProcessingFilters filter = new PaymentProcessingFilters();

            filter.Systems = new List<SelectListItem>();
            foreach (RefCode x in base.GetReferenceCodes(enRefTables.TB_SYSTEM_OF_RECORD))
                filter.Systems.Add(new SelectListItem() { Text = x.Code, Value = x.ID.ToString() });


            filter.PayeeNames = new List<SelectListItem>();
            filter.PaymentTypes = new List<SelectListItem>();
            filter.ContractNumbers = new List<SelectListItem>();
            filter.PayDates = new List<SelectListItem>();

            GetCounters();
            return PartialView("~/Views/Approvals/PendingClaimSchedules/PCSIndex.cshtml", filter);
        }

        [HttpPost]
        public JsonResult GetPendingClaimScheduleFilterValues(string type, string parentValue, string childValue)
        {
            type = Sanitizer.GetSafeHtmlFragment(type);
            parentValue = Sanitizer.GetSafeHtmlFragment(parentValue);
            childValue = Sanitizer.GetSafeHtmlFragment(childValue);

            List<ClaimSchedule> psgs = GetPendingClaimSchedules();
            List<Tuple<string, string>> lst = new List<Tuple<string, string>>();
            var parentValueStrArray = parentValue.Split(',');
            var childValueStrArray = childValue.Split(',');

            if (psgs != null && psgs.Count() > 0)
            {
                if (type == "PAYEE")
                {
                    var payeelist = psgs.Select(a => a.PayeeInfo).Distinct().Select
                        (a =>
                        new Tuple<string, string>(a.PEE_Name, a.PEE.ID.ToString())
                    ).OrderByDescending(a => a.Item1);

                    string previousItem1 = "";
                    string previousItem2 = "";
                    string newItem2 = "";
                    var uniquePayeeList = new List<Tuple<string, string>>();
                    foreach (var tuple in payeelist)
                    {
                        if (!tuple.Item1.Equals(previousItem1))
                        {
                            uniquePayeeList.Add(tuple);
                            previousItem1 = tuple.Item1;
                            previousItem2 = tuple.Item2;
                        }
                        else
                        {
                            uniquePayeeList.RemoveAll(t => t.Item1 == previousItem1);
                            newItem2 = previousItem2 + "_" + tuple.Item2;
                            uniquePayeeList.Add(new Tuple<string, string>(tuple.Item1, newItem2));
                            previousItem1 = tuple.Item1;
                            previousItem2 = newItem2;
                        }
                    }

                    lst.AddRange(uniquePayeeList);
                }
                else if (type == "PAYMENTTYPE")
                {
                    List<ClaimSchedule> psgsList = psgs.Where(t => parentValueStrArray.Select(int.Parse).ToList().Contains(t.PayeeInfo.PEE.ID)).ToList();
                    if (psgsList.Count > 0)
                    {
                        var paymentTypeList = psgsList.Select(a => a.PaymentType).Distinct().Select
                            (a =>
                            new Tuple<string, string>(a, a)
                        ).OrderByDescending(a => a.Item1);

                        lst.AddRange(paymentTypeList);
                    }
                }

                else if (type == "CONTRACTNUMBER")
                {
                    List<ClaimSchedule> psgsList = psgs.Where(t => parentValueStrArray.Select(int.Parse).ToList().Contains(t.PayeeInfo.PEE.ID)).ToList();
                    if (psgsList.Count > 0)
                    {
                        List<ClaimSchedule> innerPSGSList = psgsList.Where(a => childValueStrArray.Contains(a.PaymentType)).ToList();
                        if (innerPSGSList.Count > 0)
                        {
                            var contractNumberList = innerPSGSList.Select(a => a.ContractNumber).Distinct().Select
                                        (a => new Tuple<string, string>(a, a)).OrderByDescending(a => a.Item1);

                            lst.AddRange(contractNumberList);
                        }
                    }
                }

                else if (type == "PAYDATE")
                {
                    // we order by descending because of the way they're put into the list so that they show up Ascending
                    var payDateList = psgs.Select(a => a.PayDate.Code).Distinct().Select
                         (a =>
                         new Tuple<string, string>(a, a)
                     ).OrderByDescending(a => a.Item1);

                    lst.AddRange(payDateList);
                }
            }

            var tlistItems = lst.Select(a =>
              new
              {
                  text = a.Item1,
                  value = a.Item2
              }
          );

            return Json(tlistItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PCSSuperGroup(string system, int[] payeeNameIDs, string[] paymentTypeValues, string[] contractNumberValues, string payDateValue)
        {
            ClaimScheduleResults csResult = new ClaimScheduleResults();
            List<ClaimScheduleRecordMaster> csMasterRecordList = new List<ClaimScheduleRecordMaster>();
            List<ClaimSchedule> csList = GetPendingClaimSchedules();
            DateTime currentDate = DateTime.Now;
            if (csList != null && csList.Count > 0)
            {
                //Apply Search Criteria                
                if (payeeNameIDs != null && payeeNameIDs.Count() > 0)
                    csList = (from p in csList where payeeNameIDs.Contains(p.PayeeInfo.PEE.ID) select p).ToList();

                if (paymentTypeValues != null && paymentTypeValues.Count() > 0)
                    csList = (from p in csList where paymentTypeValues.Contains(p.PaymentType) select p).ToList();

                if (contractNumberValues != null && contractNumberValues.Count() > 0)
                    csList = (from p in csList where contractNumberValues.Contains(p.ContractNumber) select p).ToList();

                if (!string.IsNullOrEmpty(payDateValue))
                {
                    //Pending Claim Schedules Approval screen => paydate dropdown sends mm/dd/yyyy format from UI pendingcs-index.js.
                    //this is also used on Approve Claim Schedule(s) Pop-up.
                    string formattedDate = Convert.ToDateTime(payDateValue).ToString("yyyy-MM-dd");
                    csList = (from p in csList where formattedDate.Contains(p.PayDate.Code) select p).ToList();
                }

                if (csList != null && csList.Count > 0)
                {
                    foreach (ClaimSchedule cs in csList)
                    {
                        ClaimScheduleRecordMaster csMasterRecord = new ClaimScheduleRecordMaster();
                        csMasterRecord.CSPrimaryKeyId = cs.PrimaryKeyID;
                        csMasterRecord.UniqueNumber = cs.UniqueNumber;
                        csMasterRecord.ExclusivePaymentType_Code = cs.ExclusivePaymentType.Code;
                        csMasterRecord.PayeeName = cs.PayeeInfo.PEE_Name;
                        csMasterRecord.PayeeCode = cs.PayeeInfo.PEE.Code;
                        csMasterRecord.PayeeSuffix = cs.PayeeInfo.PEE_IdSfx;
                        csMasterRecord.PaymentType = cs.PaymentType;
                        csMasterRecord.ContractNumber = cs.ContractNumber;
                        csMasterRecord.FiscalYear = cs.FiscalYear;
                        csMasterRecord.TotalPaymentDollars = Double.Parse(cs.Amount.ToString());
                        csMasterRecord.GroupIdentifier = String.Join("|", cs.PaymentGroupList.Select(t => t.UniqueNumber).ToArray());
                        csMasterRecord.CurrentStatus = cs.CurrentStatus;
                        csMasterRecord.PayDate = Convert.ToDateTime(cs.PayDate.Code);
                        csMasterRecord.CurrentDate = currentDate;
                        csMasterRecord.IsLinked = cs.IsLinked;
                        csMasterRecord.hasNegativeFundingSource = cs.HasNegativeFundingSource;
                        csMasterRecord.PaymentMethodType = cs.PaymentMethodType;
                        if (csMasterRecord.IsLinked)
                        {
                            csMasterRecord.LinkedCSSets = String.Join(",", cs.LinkedCSNumberList.Where(t => t != csMasterRecord.UniqueNumber).ToArray());
                        }
                        csMasterRecordList.Add(csMasterRecord);
                    }
                    csResult.csRecordMasterList = csMasterRecordList;
                }
            }

            return PartialView("~/Views/Approvals/PendingClaimSchedules/PCSSuperGroup.cshtml", csResult);
        }

        [HttpPost]
        public JsonResult PCSApprove(string[] claimScheduleIds)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            bool conversionError = false;
            int csId = 0;

            if (claimScheduleIds.Length > 0)
            {
                List<int> csIdList = new List<int>();
                foreach (string c in claimScheduleIds)
                {
                    if (Int32.TryParse(c, out csId))
                    {
                        if (csId > 0)
                        {
                            csIdList.Add(csId);
                            csId = 0;
                        }
                        else
                        {
                            conversionError = true;
                            break;
                        }
                    }
                    else
                    {
                        conversionError = true;
                        break;
                    }
                }

                if (!conversionError)
                {
                    CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                           (svc => svc.SetClaimSchedulesStatusToApproved(csIdList, string.Empty, User.Identity.Name));

                    if (!cs.Status)
                    {
                        List<string> errorMessageList = new List<string>();
                        foreach (string message in cs.MessageDetailList)
                        {
                            errorMessageList.Add(message);
                        }

                        jsonReturnStatus.returnedData = errorMessageList;
                    }
                    else
                    {
                        jsonReturnStatus.status = true;
                    }
                    ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
                }
                else
                {
                    List<string> errorMessageList = new List<string>();
                    if (claimScheduleIds.Length <= 0)
                        errorMessageList.Add("Invalid Claim Schedule Numbers");

                    jsonReturnStatus.returnedData = errorMessageList;
                }
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                errorMessageList.Add("Invalid Claim Schedule Numbers");
                jsonReturnStatus.returnedData = errorMessageList;
            }

            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PCSDeny(string claimScheduleId, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int csId = 0;

            if ((!string.IsNullOrEmpty(claimScheduleId) && Int32.TryParse(claimScheduleId, out csId)) && (!string.IsNullOrEmpty(note) && !string.IsNullOrEmpty(note.Trim())))
            {
                note = HttpUtility.HtmlEncode(note.Trim());
                List<int> csIdList = new List<int>();
                csIdList.Add(csId);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetClaimSchedulesStatusToReturnToProcessor(csIdList, note, User.Identity.Name));

                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (string.IsNullOrEmpty(claimScheduleId))
                    errorMessageList.Add("Invalid Claim Schedule Number");
                if (string.IsNullOrEmpty(note))
                    errorMessageList.Add("Comment is required");

                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PCSRemittanceAdvice(int csID, int systemID = 1)
        {
            string strPEE_Name;

            RemittanceCSModel remittanceCSModel = ApprovalsQueries.GetRemittanceAdviceDataByCSID(csID, systemID, out  strPEE_Name);

            ViewBag.PEE_Name = strPEE_Name;
            return PartialView("../PaymentProcessing/IPCSRemittanceAdvice", remittanceCSModel);
        }

        [HttpPost]
        public JsonResult PCSSaveRemittanceNote(string claimScheduleId, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int csId = 0;

            if (!string.IsNullOrEmpty(claimScheduleId) && Int32.TryParse(claimScheduleId, out csId))
            {
                if (!string.IsNullOrEmpty(note))
                    note = note.Trim();

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                      (svc => svc.SetRemittanceAdviceNote(csId, note, User.Identity.Name));
                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (string.IsNullOrEmpty(claimScheduleId))
                    errorMessageList.Add("Invalid Claim Schedule Number");

                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PCSPaymentGroup(string csUniqueNumber, string paymentGroupName)
        {
            int csPID = 0;
            List<PaymentProcessingRecordChild> lst = new List<PaymentProcessingRecordChild>();

            if (!string.IsNullOrEmpty(csUniqueNumber))
            {
                List<ClaimSchedule> csList = GetPendingClaimSchedules();

                if (csList != null)
                {
                    var pgList = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().PaymentGroupList;

                    if (pgList != null)
                    {
                        csPID = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().PrimaryKeyID;
                        foreach (PaymentGroup pg in pgList)
                        {
                            PaymentProcessingRecordChild prsetinstance = new PaymentProcessingRecordChild();
                            prsetinstance.PaymentRecNumberSetNumber = pg.UniqueNumber;
                            prsetinstance.AssignedPaymentDate = Convert.ToDateTime(pg.PayDate.Code);
                            prsetinstance.AssignedUser = pg.AssignedUser.Display_Name;
                            prsetinstance.PaymentSetTotalAmount = Convert.ToDouble(pg.Amount);

                            //prsetinstance.IsIHSS = pg.IsIHSS;
                            //prsetinstance.IsSCHIP = pg.IsSCHIP;
                            //prsetinstance.IsHQAF = pg.IsHQAF;
                            prsetinstance.PaymentSuperGroupKey = pg.PaymentSuperGroupKey;

                            lst.Add(prsetinstance);
                        }
                    }
                }
            }

            ViewBag.ID = csUniqueNumber;
            ViewBag.CSPID = csPID;
            ViewBag.PaymentGroupName = paymentGroupName;

            return PartialView("~/Views/Approvals/PendingClaimSchedules/PCSPaymentGroup.cshtml", lst);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PCSPaymentRecord(string csUniqueNumber, string paymentRecordSetNumber, string payemntGroupId, string paymentGroupName)
        {
            List<PaymentProcessingRecord> lst = new List<PaymentProcessingRecord>();

            List<ClaimSchedule> csList = GetPendingClaimSchedules();


            if (csList != null)
            {
                var pgList = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().PaymentGroupList;
                if (pgList != null)
                {
                    var prset = pgList.Where(t => t.UniqueNumber == paymentRecordSetNumber).FirstOrDefault();

                    if (prset != null && prset.PaymentRecordList != null)
                    {
                        foreach (PaymentRec pr in prset.PaymentRecordList)
                        {
                            PaymentProcessingRecord ppr = new PaymentProcessingRecord()
                            {
                                PaymentRecID = pr.PrimaryKeyID,
                                PaymentRecNumber = pr.UniqueNumber,
                                PaymentRecordDate = pr.PaymentDate,
                                PaymentSetNumber = prset.UniqueNumber,
                                Amount = Convert.ToDouble(pr.Amount)
                            };

                            lst.Add(ppr);
                        }
                    }
                }
            }
            string cleanPaymentRecordSetNumber = paymentRecordSetNumber.Replace('.', '_');
            ViewBag.ID = csUniqueNumber;
            ViewBag.gridPrefix = payemntGroupId + "_" + csUniqueNumber + "_" + cleanPaymentRecordSetNumber;
            ViewBag.PaymentGroupName = paymentGroupName;
            ViewBag.payemntGroupId = payemntGroupId;
            ViewBag.parentPaymentRecordSetNumber = paymentRecordSetNumber;

            return PartialView("~/Views/Approvals/PendingClaimSchedules/PCSPaymentRecord.cshtml", lst);
        }

        [HttpPost]
        public ActionResult PCSFundingDetails(string csUniqueNumber, int paymentRecordID, string parentPaymentRecordSetNumber)
        {
            List<ClaimSchedule> csList = GetPendingClaimSchedules();
            return base.DisplayCSFundingDetails(csList, csUniqueNumber, paymentRecordID, parentPaymentRecordSetNumber);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PCSFundingSummary(string[] claimScheduleIds)
        {
            IPFundingDetails fds = new IPFundingDetails();
            List<int> csIdList = new List<int>();
            int csId = 0;
            bool conversionError = false;
            if (claimScheduleIds != null && claimScheduleIds.Length > 0)
            {
                foreach (string cs in claimScheduleIds)
                {
                    if (Int32.TryParse(cs, out csId))
                    {
                        csIdList.Add(csId);
                    }
                    else
                    {
                        conversionError = true;
                        break;
                    }
                }

                if (!conversionError)
                {
                    Common.CommonStatusPayload<List<AggregatedFundingDetail>> fundingStatusPayload = _wcfService.InvokeService<IEAMIWebUIDataService, Common.CommonStatusPayload<List<AggregatedFundingDetail>>>
                                           (svc => svc.GetECSFundingSummary(csIdList));
                    ErrorHandlerController.CheckFatalException(fundingStatusPayload.Status, fundingStatusPayload.IsFatal, fundingStatusPayload.GetCombinedMessage());

                    if (fundingStatusPayload != null && fundingStatusPayload.Payload != null && fundingStatusPayload.Payload.Count > 0)
                    {
                        fds.WarningMessages = fundingStatusPayload.MessageDetailList;
                        fds.FundingDataGroups = new List<IPFundingListGroup>();

                        List<AggregatedFundingDetail> fundingList = new List<AggregatedFundingDetail>();
                        fundingList = fundingStatusPayload.Payload;

                        IPFundingListGroup fdgInstance = new IPFundingListGroup();
                        double totalFFPAmount = 0, totalSGFAmount = 0, totalTotalAmount = 0;

                        fdgInstance.FundingData = new List<IPFundingDetail>();
                        foreach (AggregatedFundingDetail d in fundingList)
                        {
                            IPFundingDetail fdInstance = new IPFundingDetail();
                            fdInstance.FundingSource = d.FundingSourceName;
                            fdInstance.FFPAmount = Convert.ToDouble(d.FFPAmount);
                            totalFFPAmount += fdInstance.FFPAmount;
                            fdInstance.SGFAmount = Convert.ToDouble(d.SGFAmount);
                            totalSGFAmount += fdInstance.SGFAmount;
                            fdInstance.TotalAmount = Convert.ToDouble(d.TotalAmount);
                            totalTotalAmount += fdInstance.TotalAmount;
                            fdgInstance.FundingData.Add(fdInstance);
                        }

                        fdgInstance.FundingData.Add(new IPFundingDetail
                        {
                            FundingSource = "Total Amount",
                            FFPAmount = totalFFPAmount,
                            SGFAmount = totalSGFAmount,
                            TotalAmount = totalTotalAmount
                        });

                        fds.FundingDataGroups.Add(fdgInstance);
                    }
                }
            }

            return PartialView("~/Views/Approvals/PendingClaimSchedules/PCSFundingSummary.cshtml", fds);

        }
        #endregion

        #region E-Claim Schedules Methods

        [AjaxOnly]
        public ActionResult ECSIndex()
        {
            PaymentProcessingFilters filter = new PaymentProcessingFilters();

            filter.Systems = new List<SelectListItem>();
            foreach (RefCode x in base.GetReferenceCodes(enRefTables.TB_SYSTEM_OF_RECORD))
                filter.Systems.Add(new SelectListItem() { Text = x.Code, Value = x.ID.ToString() });

            filter.ECSStatusTypes = new List<SelectListItem>();
            foreach (RefCode x in base.GetReferenceCodes(enRefTables.TB_ECS_STATUS_TYPE))
            {
                if (x.ID != 5 && x.ID != 6) //Don't display FAIL or REJECTED Statuses.
                {
                    filter.ECSStatusTypes.Add(new SelectListItem() { Text = x.Code, Value = x.ID.ToString() });
                }
            }

            GetCounters();
            return PartialView("~/Views/Approvals/EClaimSchedules/ECSIndex.cshtml", filter);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ECSSearchView(string system, DateTime fromDate, DateTime toDate, string statusTypeId)
        {
            ECSSearchResults cls = new ECSSearchResults();
            List<EClaimScheduleMasterRecord> ecsMasterList = new List<EClaimScheduleMasterRecord>();
            if (!string.IsNullOrEmpty(system))
            {
                //if (searchDateRange != DateTime.MinValue && searchDateRange != DateTime.MaxValue)
                //{                    
                List<ElectronicClaimSchedule> ecsList = GetElectronicClaimSchedulesByDate(fromDate, toDate, Int32.Parse(statusTypeId));
                if (ecsList != null && ecsList.Count > 0)
                {
                    foreach (ElectronicClaimSchedule ecs in ecsList)
                    {
                        EClaimScheduleMasterRecord record = new EClaimScheduleMasterRecord();
                        record.EcsId = ecs.EcsId;
                        record.EcsNumber = ecs.EcsNumber;
                        record.HasExclusivePaymentType = ecs.HasExclusivePaymentType();
                        record.TransferDate = ecs.CurrentStatusDate;
                        record.CreatedBy = ecs.CreatedBy;
                        record.ApprovedBy = ecs.ApprovedBy;
                        record.TotalPayment = ecs.Amount;
                        record.PaymentMethodType = ecs.PaymentMethodType;

                        if (ecs.CurrentStatusType.ID != 1) //PENDING STATUS ID
                            record.canApproveOrDelete = false;

                        if (ecs.CurrentStatusType.ID == 3) //SENT TO SCO STATUS ID
                            record.canSenttoSCO = true;
                        else
                            record.canSenttoSCO = false;

                        ecsMasterList.Add(record);
                    }
                }
            }
            cls.masterDataList = ecsMasterList;
            return PartialView("~/Views/Approvals/EClaimSchedules/ECSSearchView.cshtml", cls);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ECSChildRecord(string ecsId, DateTime fromDate, DateTime toDate, int statusTypeId)
        {
            List<ClaimScheduleRecordMaster> csList = new List<ClaimScheduleRecordMaster>();

            List<ElectronicClaimSchedule> ecsList = GetElectronicClaimSchedulesByDate(fromDate, toDate, statusTypeId);
            if (ecsList != null && ecsList.Count > 0)
            {
                ElectronicClaimSchedule ecs = ecsList.Where(t => t.EcsId == Int32.Parse(ecsId)).FirstOrDefault();
                if (ecs != null)
                {
                    foreach (ClaimSchedule cs in ecs.ClaimScheduleList)
                    {
                        ClaimScheduleRecordMaster record = new ClaimScheduleRecordMaster();
                        if (cs.Warrant != null)
                        {
                            record.WR_ECS_NUMBER = cs.Warrant.ECS_NUMBER;
                            record.WR_ISSUE_DATE = cs.Warrant.ISSUE_DATE;
                            record.WR_WARRANT_NUMBER = cs.Warrant.WARRANT_NUMBER;
                            record.WR_WARRANT_AMOUNT = cs.Warrant.WARRANT_AMOUNT;
                            record.WR_SEQ_NUMBER = cs.Warrant.SEQ_NUMBER;
                        }
                        record.ExclusivePaymentType_Code = cs.ExclusivePaymentType.Code;
                        record.UniqueNumber = cs.UniqueNumber;
                        record.CSPrimaryKeyId = cs.PrimaryKeyID;
                        record.UniqueNumber = cs.UniqueNumber;
                        record.PayeeName = cs.PayeeInfo.PEE_Name;
                        record.PayeeCode = cs.PayeeInfo.PEE.Code;
                        record.PayeeSuffix = cs.PayeeInfo.PEE_IdSfx;
                        record.PaymentType = cs.PaymentType;
                        record.ContractNumber = cs.ContractNumber;
                        record.FiscalYear = cs.FiscalYear;
                        record.TotalPaymentDollars = Double.Parse(cs.Amount.ToString());
                        record.GroupIdentifier = String.Join("|", cs.PaymentGroupList.Select(t => t.UniqueNumber).ToArray());
                        record.CurrentStatus = cs.CurrentStatus;
                        record.PayDate = Convert.ToDateTime(cs.PayDate.Code);
                        record.AssignedUser = cs.AssignedUser.Display_Name;
                        //record.hasIHSS = cs.IsIHSS;
                        //record.hasSCHIP = cs.IsSCHIP;
                        //record.hasHQAF = cs.IsHQAF;
                        csList.Add(record);

                    }
                }
            }

            ViewBag.ID = ecsId;
            return PartialView("~/Views/Approvals/EClaimSchedules/ECSChildRecord.cshtml", csList);
        }

        [HttpPost]
        public JsonResult ApproveECS(string ecsId, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int correctECSId = 0;

            correctECSId = Int32.Parse(ecsId);
            if (correctECSId > 0)
            {
                note = HttpUtility.HtmlEncode(note.Trim());
                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetECSStatusToApproved(correctECSId, note, User.Identity.Name));

                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (correctECSId <= 0)
                    errorMessageList.Add("Invalid ECS Id passed");
                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult PendingECS(string ecsId, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int correctECSId = 0;

            correctECSId = Int32.Parse(ecsId);
            if (correctECSId > 0)
            {
                note = HttpUtility.HtmlEncode(note.Trim());
                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetECSStatusToPending(correctECSId, note, User.Identity.Name));

                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (correctECSId <= 0)
                    errorMessageList.Add("Invalid ECS Id passed");
                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteECS(string ecsId, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int correctECSId = 0;

            if (string.IsNullOrEmpty(note))
            { note = ""; }
            else { note = HttpUtility.HtmlEncode(note.Trim()); }

            correctECSId = Int32.Parse(ecsId);
            if (correctECSId > 0)
            {
                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.DeleteECS(correctECSId, note, User.Identity.Name));

                if (!cs.Status)
                {
                    List<string> errorMessageList = new List<string>();
                    foreach (string message in cs.MessageDetailList)
                    {
                        errorMessageList.Add(message);
                    }

                    jsonReturnStatus.returnedData = errorMessageList;
                }
                else
                {
                    jsonReturnStatus.status = true;
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }
            else
            {
                List<string> errorMessageList = new List<string>();
                if (correctECSId <= 0)
                    errorMessageList.Add("Invalid ECS Id passed");
                jsonReturnStatus.returnedData = errorMessageList;
            }

            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Private Methods

        private List<PaymentSuperGroup> GetPendingReturns()
        {
            CommonStatusPayload<List<PaymentSuperGroup>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<PaymentSuperGroup>>>
                                    (svc => svc.GetPaymentSuperGroupsForSupervisorScreen());
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }

        private List<ClaimSchedule> GetPendingClaimSchedules()
        {
            CommonStatusPayload<List<ClaimSchedule>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<ClaimSchedule>>>
                                    (svc => svc.GetClaimSchedulesSubmettedForApproval());
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }


        private List<ElectronicClaimSchedule> GetElectronicClaimSchedulesByDate(DateTime dateFrom, DateTime dateTo, int statusTypeId)
        {
            //PENDING = 1, APPROVED = 2, SENT_TO_SCO = 3, WARRANT_RECEIVED = 4, FAIL = 5, REJECTED = 6                
            if (statusTypeId != 4 && statusTypeId != 6) //If Search is not WARRANT_RECEIVED OR REJECTED, PASS MIN/MAX Values for date range
            {
                dateFrom = new DateTime(1900, 01, 01);
                dateTo = DateTime.Now.AddYears(10);

            }
            CommonStatusPayload<List<ElectronicClaimSchedule>> cs = _wcfService.InvokeService<IEAMIWebUIDataService,
                CommonStatusPayload<List<ElectronicClaimSchedule>>>(svc => svc.GetElectronicClaimSchedulesByDateRangeStatusType(dateFrom, dateTo, statusTypeId));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }

        [AjaxOnly]
        public ActionResult ECSRemittanceAdvice(int csID, int systemID = 1)
        {
            string strPEE_Name;
            RemittanceCSModel remittanceCSModel = ApprovalsQueries.GetRemittanceAdviceDataByCSID(csID, systemID, out strPEE_Name);
            ViewBag.PEE_Name = strPEE_Name;
            return PartialView("../PaymentProcessing/IPCSRemittanceAdvice", remittanceCSModel);
        }

        private void GetCounters()
        {
            List<PaymentSuperGroup> rList = GetPendingReturns();
            List<ClaimSchedule> cList = GetPendingClaimSchedules();
            ViewBag.PendingReturnCount = (rList != null) ? rList.Sum(t => t.PaymentGroupList.Count).ToString() : "";
            ViewBag.PendingClaimScheduleCount = (cList != null) ? cList.Count().ToString() : "";
        }
        #endregion

    }
}