using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;


namespace OHC.EAMI.WebUI.Controllers
{

    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class BaseController : Controller
    {
        private readonly WcfServiceInvoker _wcfService;

        public BaseController()
        {
            _wcfService = new WcfServiceInvoker();
        }

        public List<RefCode> GetReferenceCodes(enRefTables tbName)
        {
            List<RefCode> codes = new List<RefCode>();

            CommonStatusPayload<List<RefCodeList>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<RefCodeList>>>
                                        (svc => svc.GetReferenceCodeList(tbName));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            if (cs.Status && cs.Payload != null)
            {
                codes.AddRange(cs.Payload.FirstOrDefault().ToList());
            }

            return codes;
        }

        // GET: Base
        protected override void OnException(ExceptionContext filterContext)
        {
            //var etype = filterContext.Exception.GetType();
            Exception ex = filterContext.Exception;
            WebException wex = ex is WebException ? (WebException)ex : null;
            if (wex != null && wex.Status == WebExceptionStatus.Timeout)
            {
                //filterContext.ExceptionHandled = true;
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/ErrorHandler/SystemTimeout.cshtml"
                };
            }
            else
            {
                //Write to nlog
                EAMILogger.Instance.Error(filterContext.Exception);

                //filterContext.ExceptionHandled = true;

                //Send Error Email to EAMI Distribution List
                Data.Notification.SendErrorEmail(filterContext.Exception);

                // Redirect on error:
                //filterContext.Result = RedirectToAction("Index", "ErrorHandler");

                // OR set the result without redirection:
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/ErrorHandler/Index.cshtml"
                };
            }
        }

        public int GetLoggedinUserID()
        {
            int uid = 0;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var uidentity = (ClaimsIdentity)HttpContext.User.Identity;

                if (uidentity.HasClaim(a => a.Type == "UserID"))
                    int.TryParse(uidentity.FindFirst(a => a.Type == "UserID").Value, out uid);
            }

            return uid;
        }

        /// <summary>
        /// Displays the group funding details.
        /// </summary>
        /// <param name="psgs">The PSGS.</param>
        /// <param name="paymentRecordID">The payment record identifier.</param>
        /// <param name="topGroupID">The top group identifier.</param>
        /// <param name="ParentPaymentRecordSetNumber">The parent payment set number.</param>
        /// <returns></returns>
        [AjaxOnly]
        public ActionResult DisplayGroupFundingDetails(List<PaymentSuperGroup> psgs, int paymentRecordID, string topGroupID, string parentPaymentRecordSetNumber)
        {
            IPFundingDetails fds = new IPFundingDetails();
            fds = FillupFundingDetails(psgs, paymentRecordID, topGroupID, parentPaymentRecordSetNumber);
            return PartialView("~/views/Shared/_FundingDetails.cshtml", fds);
        }

        /// <summary>
        /// Displays the cs funding details.
        /// </summary>
        /// <param name="csList">The cs list.</param>
        /// <param name="csUniqueNumber">The cs unique number.</param>
        /// <param name="paymentRecordID">The payment record identifier.</param>
        /// <param name="parentPaymentRecordSetNumber">The parent payment set number.</param>
        /// <returns></returns>
        [AjaxOnly]
        public ActionResult DisplayCSFundingDetails(List<ClaimSchedule> csList, string csUniqueNumber, int paymentRecordID, string parentPaymentRecordSetNumber)
        {
            IPFundingDetails fds = new IPFundingDetails();
            fds = FillupCSFundingDetails(csList, csUniqueNumber, paymentRecordID, parentPaymentRecordSetNumber);
            return PartialView("~/views/Shared/_FundingDetails.cshtml", fds);
        }


        #region Private Methods
        /// <summary>
        /// Fillups the funding details.
        /// </summary>
        /// <param name="psgs">The PSGS.</param>
        /// <param name="paymentRecordID">The payment record identifier.</param>
        /// <param name="topGroupID">The top group identifier.</param>
        /// <param name="ParentPaymentRecordSetNumber">The parent payment set number.</param>
        /// <returns></returns>
        private IPFundingDetails FillupFundingDetails(List<PaymentSuperGroup> psgs, int paymentRecordID, string topGroupID, string ParentPaymentRecordSetNumber)
        {
            IPFundingDetails fds = new IPFundingDetails();
            List<PaymentFundingDetail> fdsList = null;
            int fiscalYear = 0;

            PaymentRec pr = psgs.Where(a => a.UniqueKey == topGroupID).FirstOrDefault().PaymentGroupList.Where(a => a.UniqueNumber == ParentPaymentRecordSetNumber).FirstOrDefault()
                .PaymentRecordList.Where(a => a.PrimaryKeyID == paymentRecordID).FirstOrDefault();

            if (pr != null)
            {
                fds.SFY = pr.FiscalYear;
                fiscalYear = Int32.Parse(pr.FiscalYear);
                fds.FFY = pr.FiscalYear;
                fds.Index = pr.IndexCode;
                fds.ContractTerm = pr.ContractDateFrom.ToShortDateString() + " - " + pr.ContractDateTo.ToShortDateString();
                fds.ContractNumber = pr.ContractNumber;
                fds.Object = pr.ObjDetailCode;
                fds.PCACode = pr.PCACode;
                fds.PayeeName = pr.PayeeInfo.PEE_Name;
                fds.PayeeCode = pr.PayeeInfo.PEE.Code;
                fds.PayeeSuffix = pr.PayeeInfo.PEE_IdSfx;
                fds.PaymentType = pr.PaymentType;
                fds.VendorCode = pr.PayeeInfo.PEE_FullCode;

                fdsList = pr.FundingDetailList;
            }

            if (fdsList == null)
            {
                CommonStatusPayload < List < PaymentFundingDetail >> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<PaymentFundingDetail>>>
                                           (svc => svc.GetPaymentFundingDetails(paymentRecordID));
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

                fdsList = cs.Payload;
                if (fdsList != null && fdsList.Count() > 0)
                {
                    psgs.Where(a => a.UniqueKey == topGroupID).FirstOrDefault().PaymentGroupList.Where(a => a.UniqueNumber == ParentPaymentRecordSetNumber)
                        .FirstOrDefault().PaymentRecordList.Where(a => a.PrimaryKeyID == paymentRecordID).FirstOrDefault().FundingDetailList = fdsList;
                }
            }

            fds.FundingDataGroups = new List<IPFundingListGroup>();

            if (fdsList != null)
            {
                //Order by Fiscal Year and then by Quarters
                fdsList = fdsList.OrderBy(t => t.FiscalYear).ThenBy(t => t.FiscalQuarter).ToList();
                                
                var uniqueQuartersWithFiscalYear = fdsList.Select(t => new { t.FiscalYear, t.FiscalQuarter }).Distinct();

                foreach (var quarterFiscalYear in uniqueQuartersWithFiscalYear)
                {
                    string fundingQuarter = quarterFiscalYear.FiscalQuarter;
                    string fundingFiscalYearString = quarterFiscalYear.FiscalYear;
                    int fundingFiscalYear = Convert.ToInt32(fundingFiscalYearString);

                    var fundingForThisQuarter = fdsList.Where(a => a.FiscalQuarter == fundingQuarter
                                        && a.FiscalYear == fundingFiscalYearString);
                    
                    IPFundingListGroup fdgInstance = new IPFundingListGroup();
                    string quarterHeader = string.Empty;
                    if (string.Compare(fundingQuarter, "Q1", true) == 0)
                    {                                           
                        quarterHeader = "SFQ: " + "07/" + fundingFiscalYear + "- 09/" + fundingFiscalYear;
                    }
                    if (string.Compare(fundingQuarter, "Q2", true) == 0)
                    {                                            
                        quarterHeader = "SFQ: " + "10/" + fundingFiscalYear + "- 12/" + fundingFiscalYear;
                    }
                    if (string.Compare(fundingQuarter, "Q3", true) == 0)
                    {                                           
                        quarterHeader = "SFQ: " + "01/" + (fundingFiscalYear + 1) + "- 03/" + (fundingFiscalYear + 1);
                    }
                    if (string.Compare(fundingQuarter, "Q4", true) == 0)
                    {                       
                        quarterHeader = "SFQ: " + "04/" + (fundingFiscalYear + 1) + "- 06/" + (fundingFiscalYear + 1);
                    }

                    fdgInstance.FundingListGroupIdentifierHeader = quarterHeader;
                    double totalFFPAmount = 0, totalSGFAmount = 0, totalTotalAmount = 0;
                    fdgInstance.FundingData = new List<IPFundingDetail>();

                    foreach (PaymentFundingDetail d in fundingForThisQuarter)
                    {
                        IPFundingDetail fdInstance = new IPFundingDetail();
                        fdInstance.FundingSource = d.FundingSourceName;
                        fdInstance.FFPAmount = Convert.ToDouble(d.FFPAmount);
                        totalFFPAmount += fdInstance.FFPAmount;
                        fdInstance.SGFAmount = Convert.ToDouble(d.SGFAmount);
                        totalSGFAmount += fdInstance.SGFAmount;
                        fdInstance.TotalAmount = Convert.ToDouble(d.FFPAmount + d.SGFAmount);
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

            return fds;
        }

        /// <summary>
        /// Fillups the cs funding details.
        /// </summary>
        /// <param name="csList">The cs list.</param>
        /// <param name="csUniqueNumber">The cs unique number.</param>
        /// <param name="paymentRecordID">The payment record identifier.</param>
        /// <param name="parentPaymentRecordSetNumber">The parent payment set number.</param>
        /// <returns></returns>
        private IPFundingDetails FillupCSFundingDetails(List<ClaimSchedule> csList, string csUniqueNumber, int paymentRecordID, string parentPaymentRecordSetNumber)
        {
            IPFundingDetails fds = new IPFundingDetails();
            List<PaymentFundingDetail> fdsList = null;
            int fiscalYear = 0;

            var pgList = csList.Where(t => t.UniqueNumber == csUniqueNumber).FirstOrDefault().PaymentGroupList;
            if (pgList != null)
            {
                var prset = pgList.Where(t => t.UniqueNumber == parentPaymentRecordSetNumber).FirstOrDefault();
                if (prset != null && prset.PaymentRecordList != null)
                {
                    PaymentRec pr = prset.PaymentRecordList.Where(t => t.PrimaryKeyID == paymentRecordID).FirstOrDefault();
                    if (pr != null)
                    {
                        fds.SFY = pr.FiscalYear;
                        fiscalYear = Int32.Parse(pr.FiscalYear);
                        fds.FFY = pr.FiscalYear;
                        fds.Index = pr.IndexCode;
                        fds.ContractTerm = pr.ContractDateFrom.ToShortDateString() + " - " + pr.ContractDateTo.ToShortDateString();
                        fds.ContractNumber = pr.ContractNumber;
                        fds.Object = pr.ObjDetailCode;
                        fds.PCACode = pr.PCACode;
                        fds.PayeeName = pr.PayeeInfo.PEE_Name;
                        fds.PayeeCode = pr.PayeeInfo.PEE.Code;
                        fds.PayeeSuffix = pr.PayeeInfo.PEE_IdSfx;
                        fds.PaymentType = pr.PaymentType;
                        fds.VendorCode = pr.PayeeInfo.PEE_FullCode;

                        fdsList = pr.FundingDetailList;
                    }

                    if (fdsList == null)
                    {
                        // call service to get funding for given PR id
                        CommonStatusPayload < List < PaymentFundingDetail >> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<PaymentFundingDetail>>>
                                                   (svc => svc.GetPaymentFundingDetails(paymentRecordID));
                        ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
                        fdsList = cs.Payload;
                    }

                    fds.FundingDataGroups = new List<IPFundingListGroup>();

                    if (fdsList != null)
                    {
                        //Order by Fiscal Year and then by Quarters
                        fdsList = fdsList.OrderBy(t => t.FiscalYear).ThenBy(t => t.FiscalQuarter).ToList();

                        var uniqueQuartersWithFiscalYear = fdsList.Select(t => new { t.FiscalYear, t.FiscalQuarter }).Distinct();

                        foreach (var quarterFiscalYear in uniqueQuartersWithFiscalYear)
                        {
                            string fundingQuarter = quarterFiscalYear.FiscalQuarter;
                            string fundingFiscalYearString = quarterFiscalYear.FiscalYear;
                            int fundingFiscalYear = Convert.ToInt32(fundingFiscalYearString);

                            var fundingForThisQuarter = fdsList.Where(a => a.FiscalQuarter == fundingQuarter
                                                && a.FiscalYear == fundingFiscalYearString);

                            IPFundingListGroup fdgInstance = new IPFundingListGroup();
                            string quarterHeader = string.Empty;
                            if (string.Compare(fundingQuarter, "Q1", true) == 0)
                            {                                
                                quarterHeader = "SFQ: " + "07/" + fundingFiscalYear + "- 09/" + fundingFiscalYear;
                            }
                            if (string.Compare(fundingQuarter, "Q2", true) == 0)
                            {                                
                                quarterHeader = "SFQ: " + "10/" + fundingFiscalYear + "- 12/" + fundingFiscalYear;
                            }
                            if (string.Compare(fundingQuarter, "Q3", true) == 0)
                            {                               
                                quarterHeader = "SFQ: " + "01/" + (fundingFiscalYear + 1) + "- 03/" + (fundingFiscalYear + 1);
                            }
                            if (string.Compare(fundingQuarter, "Q4", true) == 0)
                            {                                
                                quarterHeader = "SFQ: " + "04/" + (fundingFiscalYear + 1) + "- 06/" + (fundingFiscalYear + 1);
                            }

                            fdgInstance.FundingListGroupIdentifierHeader = quarterHeader;
                            double totalFFPAmount = 0, totalSGFAmount = 0, totalTotalAmount = 0;
                            fdgInstance.FundingData = new List<IPFundingDetail>();

                            foreach (PaymentFundingDetail d in fundingForThisQuarter)
                            {
                                IPFundingDetail fdInstance = new IPFundingDetail();
                                fdInstance.FundingSource = d.FundingSourceName;
                                fdInstance.FFPAmount = Convert.ToDouble(d.FFPAmount);
                                totalFFPAmount += fdInstance.FFPAmount;
                                fdInstance.SGFAmount = Convert.ToDouble(d.SGFAmount);
                                totalSGFAmount += fdInstance.SGFAmount;
                                fdInstance.TotalAmount = Convert.ToDouble(d.FFPAmount + d.SGFAmount);
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
            }
            return fds;
        }

        #endregion
    }
}