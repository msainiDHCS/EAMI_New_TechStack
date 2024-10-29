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
using OHC.EAMI.WebUI.Data;
using Microsoft.Security.Application;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class PaymentProcessingController : BaseController
    {
        private readonly WcfServiceInvoker _wcfService;

        #region Public Methods

        public PaymentProcessingController()
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

        public ActionResult Index()
        {
            //get the grid data and cache it
            //GetAssignedPaymentRecordHierarchyNotInCS();   
            GetCounters();
            return View();
        }

        #region Invoice Processing Methods 

        public JsonResult GetFilterValues(string type, string parentValue, string childValue)
        {
            type = Sanitizer.GetSafeHtmlFragment(type);
            parentValue = Sanitizer.GetSafeHtmlFragment(parentValue);
            childValue = Sanitizer.GetSafeHtmlFragment(childValue);

            List<PaymentSuperGroup> psgs = GetAssignedPaymentRecordHierarchyNotInCS();
            List<Tuple<string, string>> lst = new List<Tuple<string, string>>();

            //string _processorCacheKey = HttpContext.Session.SessionID + "_ProcessorCS";
            //List<PaymentSuperGroup> psgs = CacheManager<List<PaymentSuperGroup>>.Get(_processorCacheKey, "PROC");
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

        [AjaxOnly]
        public ActionResult InvoiceProcessingAssignment()
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
            return PartialView(filter);
        }

        [AjaxOnly]
        public ActionResult InvoiceProcessingAssignmentGrid(string system, int[] payeeNameIDs, string[] paymentTypeValues, string[] contractNumberValues)
        {
            PaymentProcessingSearchResults cls = new PaymentProcessingSearchResults();
            List<PaymentProcessingRecordMaster> lst = new List<PaymentProcessingRecordMaster>();
            List<PaymentSuperGroup> psgs = GetAssignedPaymentRecordHierarchyNotInCS();

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
                    m.hasHold = g.HasItemsOnHold();
                    m.HasExclusivePaymentType = g.HasExclusivePaymentType();
                    m.hasReleaseFromSup = g.HasItemsReleaseFromSup();
                    //m.hasIHSS = g.HasIHSSItems();
                    //m.hasSCHIP = g.HasSCHIPItems();
                    //m.hasHQAF = g.HasHQAFItems();

                    lst.Add(m);
                }

                cls.masterDataList = lst;
            }
            return PartialView(cls);
        }

        [AjaxOnly]
        public ActionResult InvoiceProcessingAssignmentChild(string id, string paymentGroupName)
        {
            List<PaymentProcessingRecordChild> lst = new List<PaymentProcessingRecordChild>();
            List<PaymentSuperGroup> psgs = GetAssignedPaymentRecordHierarchyNotInCS();

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

                        if (pg.OnHoldFlagStatus != null)
                        {
                            prsetinstance.IsOnHold = true;
                            prsetinstance.OnHold_StatusNote = pg.OnHoldFlagStatus.StatusNote;
                            prsetinstance.OnHold_CreatedBy = pg.OnHoldFlagStatus.CreatedBy;
                            prsetinstance.OnHold_StatusDate = pg.OnHoldFlagStatus.StatusDate;
                        }
                        if (pg.ReleaseFromSupFlagStatus != null)
                        {
                            prsetinstance.IsReleaseFromSupervisor = true;
                            prsetinstance.Denied_StatusNote = pg.ReleaseFromSupFlagStatus.StatusNote;
                            prsetinstance.Denied_CreatedBy = pg.ReleaseFromSupFlagStatus.CreatedBy;
                            prsetinstance.Denied_StatusDate = pg.ReleaseFromSupFlagStatus.StatusDate;
                        }

                        prsetinstance.ExclusivePaymentType_Code = pg.ExclusivePaymentType.Code;
                        prsetinstance.Payment_Method_Type = pg.PaymentMethodType.Code;

                        lst.Add(prsetinstance);
                    }

                }
            }

            ViewBag.ID = id;
            ViewBag.PaymentGroupName = paymentGroupName;

            return PartialView(lst);
        }

        [AjaxOnly]
        public ActionResult InvoiceProcessingPaymentSetDetails(string paymentRecordSetNumber, string topGroupID, string paymentGroupName)
        {
            List<PaymentProcessingRecord> lst = new List<PaymentProcessingRecord>();
            List<PaymentSuperGroup> psgs = GetAssignedPaymentRecordHierarchyNotInCS();

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

            return PartialView(lst);
        }

        public ActionResult InvoiceProcessingFundingDetails(int paymentRecordID, string topGroupID, string ParentPaymentRecordSetNumber)
        {
            List<PaymentSuperGroup> psgs = GetAssignedPaymentRecordHierarchyNotInCS();
            return base.DisplayGroupFundingDetails(psgs, paymentRecordID, topGroupID, ParentPaymentRecordSetNumber);
        }

        [AjaxOnly]
        public ActionResult InvoiceProcessingAddToClaimSchedule()
        {
            AddToClaimSchedule cls = new AddToClaimSchedule();

            cls.ClaimSchedules = new List<SelectListItem>();

            List<ClaimSchedule> csList = new List<ClaimSchedule>();

            CommonStatusPayload<List<ClaimSchedule>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<ClaimSchedule>>>
                                       (svc => svc.GetClaimSchedulesForProcessorByUser(base.GetLoggedinUserID(), User.Identity.Name));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            csList = cs.Payload;

            if (csList != null && csList.Count > 0)
            {
                List<ClaimSchedule> uniqueCSList = csList.GroupBy(t => t.UniqueNumber).Select(g => g.First()).ToList();

                if (uniqueCSList != null && uniqueCSList.Count > 0)
                {
                    foreach (ClaimSchedule ucs in uniqueCSList)
                    {
                        StringBuilder selectOptionText = new StringBuilder();
                        selectOptionText.Append("(" + ucs.UniqueNumber + ")-(");
                        selectOptionText.Append(ucs.ContractNumber + ")-(");
                        selectOptionText.Append(ucs.PayeeInfo.PEE.Code + "-" + ucs.PayeeInfo.PEE_IdSfx + ")-(");
                        selectOptionText.Append(ucs.Amount.ToString("C2") + ")");
                        cls.ClaimSchedules.Add(new SelectListItem { Text = selectOptionText.ToString(), Value = ucs.PrimaryKeyID.ToString() });
                    }

                    return PartialView(cls);
                }
            }


            return PartialView(cls);
        }

        public JsonResult AssignNewClaimSchedule(string paymentRecordSet)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            if (paymentRecordSet != null && paymentRecordSet.Count() > 0)
            {
                List<string> priorityHashGroupList = JsonConvert.DeserializeObject<List<string>>(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.CreateNewClaimSchedule(priorityHashGroupList, base.GetLoggedinUserID(), User.Identity.Name));

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
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPaymentGroupToClaimSchedule(string paymentRecordSet, string claimScheduleId)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int csId = 0;
            if ((paymentRecordSet != null && paymentRecordSet.Count() > 0) && (!string.IsNullOrEmpty(claimScheduleId) && Int32.TryParse(claimScheduleId, out csId)))
            {
                List<string> priorityHashGroupList = JsonConvert.DeserializeObject<List<string>>(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.AddPaymentGroupsToClaimSchedule(csId, priorityHashGroupList, User.Identity.Name));

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
                    errorMessageList.Add("Please select a Claim Schedule Number");
                if (paymentRecordSet == null || paymentRecordSet.Count() <= 0)
                    errorMessageList.Add("Please select at least one Payment Set");
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceProcessingReturnPaymentGroup(string paymentRecordSet, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            if ((paymentRecordSet != null && paymentRecordSet.Count() > 0) && (!string.IsNullOrEmpty(note) && !string.IsNullOrEmpty(note.Trim())))
            {
                note = HttpUtility.HtmlEncode(note.Trim());

                List<string> priorityHashGroupList = JsonConvert.DeserializeObject<List<string>>(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetPaymentGroupsToReturnToSup(priorityHashGroupList, User.Identity.Name, note));

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
                    errorMessageList.Add("Please enter a note");
                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceProcessingHoldPaymentGroup(string paymentRecordSet, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            if ((paymentRecordSet != null && paymentRecordSet.Count() > 0) && (!string.IsNullOrEmpty(note) && !string.IsNullOrEmpty(note.Trim())))
            {
                note = HttpUtility.HtmlEncode(note.Trim());

                List<string> priorityHashGroupList = JsonConvert.DeserializeObject<List<string>>(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetPaymentGroupsToHold(priorityHashGroupList, User.Identity.Name, note));

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
                    errorMessageList.Add("Please enter a note");
                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceProcessingUnholdPaymentGroup(string paymentRecordSet, string note)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            if ((paymentRecordSet != null && paymentRecordSet.Count() > 0))
            {
                if (!string.IsNullOrEmpty(note))
                {
                    note = note.Trim();
                    note = HttpUtility.HtmlEncode(note);
                }
                List<string> priorityHashGroupList = JsonConvert.DeserializeObject<List<string>>(paymentRecordSet);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetPaymentGroupsToUnHold(priorityHashGroupList, User.Identity.Name, note));

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

                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Claim Schedule Methods

        [AjaxOnly]
        public ActionResult IPClaimSchedules()
        {
            ClaimScheduleResults csResult = new ClaimScheduleResults();

            List<ClaimScheduleRecordMaster> csMasterRecordList = new List<ClaimScheduleRecordMaster>();

            List<ClaimSchedule> csList = GetClaimSchedulesForProcessorByUser();

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
                    csMasterRecord.IsLinked = cs.IsLinked;

                    csMasterRecord.PaymentMethodType = cs.PaymentMethodType;
                    if (csMasterRecord.IsLinked)
                    {

                        csMasterRecord.LinkedCSSets = String.Join(",", cs.LinkedCSNumberList.Where(t => t != csMasterRecord.UniqueNumber).ToArray());
                    }

                    csMasterRecordList.Add(csMasterRecord);
                }
                csResult.csRecordMasterList = csMasterRecordList;
            }
            GetCounters();
            return PartialView(csResult);
        }

        public JsonResult DeleteClaimSchedule(string claimScheduleId)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int csId = 0;

            if (!string.IsNullOrEmpty(claimScheduleId) && Int32.TryParse(claimScheduleId, out csId))
            {
                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.DeleteClaimSchedule(csId, User.Identity.Name));

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

        public JsonResult SubmitClaimSchedule(string claimScheduleId)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int csId = 0;

            if (!string.IsNullOrEmpty(claimScheduleId) && Int32.TryParse(claimScheduleId, out csId))
            {
                List<int> csIdList = new List<int>();
                csIdList.Add(csId);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.SetClaimSchedulesStatusToSubmitForApproval(csIdList, string.Empty, User.Identity.Name));

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

        public JsonResult RemovePaymentRecordFromClaimSchedule(string claimScheduleId, string paymentRecordSetNumber)
        {
            JSONReturnStatus jsonReturnStatus = new JSONReturnStatus();
            int csId = 0;
            List<string> paymentRecordSetList = new List<string>();

            if ((!string.IsNullOrEmpty(claimScheduleId) && Int32.TryParse(claimScheduleId, out csId)) &&
                (!string.IsNullOrEmpty(paymentRecordSetNumber)))
            {
                paymentRecordSetList.Add(paymentRecordSetNumber);

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                       (svc => svc.RemovePaymentGroupsFromClaimSchedule(csId, paymentRecordSetList, User.Identity.Name));

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
                if (string.IsNullOrEmpty(paymentRecordSetNumber))
                    errorMessageList.Add("Invalid Payment Set#");

                jsonReturnStatus.returnedData = errorMessageList;
            }
            return Json(jsonReturnStatus, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        public ActionResult IPClaimSchedulePaymentGroup(string csUniqueNumber, string paymentGroupName)
        {
            int csPID = 0;
            List<PaymentProcessingRecordChild> lst = new List<PaymentProcessingRecordChild>();

            if (!string.IsNullOrEmpty(csUniqueNumber))
            {
                List<ClaimSchedule> csList = GetClaimSchedulesForProcessorByUser();

                if (csList != null)
                {
                    var pgList = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().PaymentGroupList;
                    bool selectedCSIsLinked = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().IsLinked;
                    List<string> selectedCSLinkedSets = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().LinkedCSNumberList;
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
                            prsetinstance.IsLinked = selectedCSIsLinked;
                            if (prsetinstance.IsLinked)
                            {
                                prsetinstance.LinkedSets = String.Join(",", selectedCSLinkedSets.Where(t => t != csUniqueNumber).ToArray());
                            }

                            lst.Add(prsetinstance);
                        }
                    }
                }
            }

            ViewBag.ID = csUniqueNumber;
            ViewBag.CSPID = csPID;
            ViewBag.PaymentGroupName = paymentGroupName;

            return PartialView(lst);
        }

        [AjaxOnly]
        public ActionResult IPClaimSchedulePaymentSetDetails(string csUniqueNumber, string paymentRecordSetNumber, string payemntGroupId, string paymentGroupName)
        {
            List<PaymentProcessingRecord> lst = new List<PaymentProcessingRecord>();

            List<ClaimSchedule> csList = GetClaimSchedulesForProcessorByUser();


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
            ViewBag.gridPrefix = payemntGroupId + "_" + cleanPaymentRecordSetNumber;
            ViewBag.PaymentGroupName = paymentGroupName;
            ViewBag.payemntGroupId = payemntGroupId;
            ViewBag.parentPaymentRecordSetNumber = paymentRecordSetNumber;

            return PartialView(lst);
        }

        public ActionResult IPClaimScheduleFundingDetails(string csUniqueNumber, int paymentRecordID, string parentPaymentRecordSetNumber)
        {
            List<ClaimSchedule> csList = GetClaimSchedulesForProcessorByUser();
            return base.DisplayCSFundingDetails(csList, csUniqueNumber, paymentRecordID, parentPaymentRecordSetNumber);
        }

        [AjaxOnly]
        public ActionResult IPCSRemittanceAdvice(int csID, int systemID = 1)
        {
            string strPEE_Name;
            RemittanceCSModel remittanceCSModel = PaymentProcessingQueries.GetRemittanceAdviceDataByCSID(csID, systemID, out strPEE_Name);
            ViewBag.PEE_Name = strPEE_Name;
            return PartialView(remittanceCSModel);
        }

        public JsonResult SaveRemittanceNote(string claimScheduleId, string note)
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
        #endregion

        #endregion

        #region Private Methods

        private List<PaymentSuperGroup> GetAssignedPaymentRecordHierarchyNotInCS()
        {
            CommonStatusPayload<List<PaymentSuperGroup>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<PaymentSuperGroup>>>
                                    (svc => svc.GetAssignedPaymentSuperGroupsForAddingToClaimSchedule(base.GetLoggedinUserID(), User.Identity.Name));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }

        private List<ClaimSchedule> GetClaimSchedulesForProcessorByUser()
        {
            CommonStatusPayload<List<ClaimSchedule>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<ClaimSchedule>>>
                                    (svc => svc.GetClaimSchedulesForProcessorByUser(base.GetLoggedinUserID(), User.Identity.Name));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }

        private void GetCounters()
        {
            List<PaymentSuperGroup> rList = GetAssignedPaymentRecordHierarchyNotInCS();
            List<ClaimSchedule> cList = GetClaimSchedulesForProcessorByUser();
            ViewBag.PaymentRecordCount = (rList != null) ? rList.Sum(t => t.PaymentGroupList.Count).ToString() : "";
            ViewBag.ClaimScheduleCount = (cList != null) ? cList.Sum(t => t.PaymentGroupList.Count).ToString() : "";
        }
        #endregion
    }
}