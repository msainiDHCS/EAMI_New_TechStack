using OHC.EAMI.WebUI.Data;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;
using OHC.EAMI.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    public class PaymentRecordController_12_29_17 : BaseController
    {

        public ActionResult Test()
        {
            ////The following will be replaced with service call
            PaymentExchangeEntity payee1 = new PaymentExchangeEntity();
            //payee1.PayeeCode = "PayeeCode99";
            //payee1.Name = "Bruce Wayne";
            //payee1.Address = "123 Joker Way";
            //payee1.City = "Gotham";
            //payee1.Zip = "95412";
            //payee1.State = "NY";
            //payee1.PhoneNumber = "916-425-8741";

            //Payee payee2 = new Payee();
            //payee2.PayeeCode = "PayeeCode777";
            //payee2.Name = "Optimus";
            //payee2.Address = "532 Prime Court";
            //payee2.City = "Autobotia";
            //payee2.Zip = "24158-6512";
            //payee2.State = "CA";
            //payee2.PhoneNumber = "619-145-8424";

            return View(payee1);
        }







        // GET: AssignPaymentRecords_GetPaymentRecords
        [HttpGet]
        public ActionResult AssignPaymentRecords_GetPaymentRecords(string payeeDisplayId, string payeeRefCode, string paymentTypeName)
        {
            return PartialView("_PaymentRecordsPartial",
                PaymentRecordQueries.GetPaymentRecords(payeeDisplayId, payeeRefCode, paymentTypeName));
        }





        // GET: Invoices
        //public ActionResult AssignInvoices()
        //{

        //    //The following will be replaced with service call
        //    List<AssignInvoicesViewModel> listAssignInvoicesViewModel = InvoiceQueries.GetVendorsDisplayList();

        //    return View(listAssignInvoicesViewModel.AsQueryable<AssignInvoicesViewModel>());

        //}
        // GET: PayeePaymentType DisplayList
        public ActionResult AssignPaymentRecords()
        {
            if (PaymentRecordQueries.GetFromOrPutInCache_ListPaymentRecsGroupedByPayeeAndPaymentType("CAPMAN", "0") != null)
            {
                // Return empty list
                List<AssignPaymentRecordsViewModel> listAssignPaymentRecordsViewModel = new List<AssignPaymentRecordsViewModel>();
                return View(listAssignPaymentRecordsViewModel.AsQueryable<AssignPaymentRecordsViewModel>());
            }
            else
            {
                //REPLACE BELOW WITH AN ERROR VIEW TO FORWARD TO LATER ON??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
                List<AssignPaymentRecordsViewModel> listAssignPaymentRecordsViewModel = null;
                return View(listAssignPaymentRecordsViewModel.AsQueryable<AssignPaymentRecordsViewModel>());
            }
        }

        [HttpGet]
        public ActionResult AssignPaymentRecords_Search(string system, string paymentRecordStatusId,
                                                        string[] arrayPayeeName, string[] arrayPaymentTypeName, string[] arrayContractNumber)
        {
            return PartialView("_AssignPaymentRecords_SearchPartial", 
                PaymentRecordQueries.GetListSearchResults(system, paymentRecordStatusId, arrayPayeeName, arrayPaymentTypeName, arrayContractNumber)
                .AsQueryable<AssignPaymentRecordsViewModel>());
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



        public ActionResult Detail(Invoice Invoice)
        {
            //The following will be replaced with service call
            Invoice.PaymentExchangeEntity = new PaymentExchangeEntity();
            Invoice.PaymentExchangeEntity.EntityID = "vCodeVal";
            Invoice.IndexCode = "iCodeVal";
            Invoice.ObjectAgencyCode = "oACode";
            Invoice.ObjectDetailCode = "oDCode";
            Invoice.PCACode = "pCACode";
            Invoice.Contract = new Contract();
            Invoice.Contract.EffectiveDateFrom = new DateTime(2003, 1, 2);
            Invoice.Contract.EffectiveDateTo = new DateTime(2007, 6, 23);
            Invoice.Contract.ContractNumber = "cNumber";
            Invoice.Contract.CurrentQuarter = "Q4";

            Invoice.FundingDetails = new List<FundingDetail>();
            Invoice.FundingDetails.Add(new FundingDetail
            {
                FundingDetailID = 777,
                //FundingSourceName = Invoice.InvoiceNumber + "100% State",
                FundingSourceName = "100% State",
                Amount = 987.04m,
                FiscalQuarter = "Q1",
                FiscalMonth = "12",
                WaiverName = "State_WaiverName"
            });

            Invoice.FundingDetails.Add(new FundingDetail
            {
                FundingDetailID = 555,
                FundingSourceName = "Real FMAP",
                Amount = 105m,
                FiscalQuarter = "Q4",
                FiscalMonth = "03",
                WaiverName = ""
            });

            Invoice.FundingDetails.Add(new FundingDetail
            {
                FundingDetailID = 227,
                FundingSourceName = "Regular FMAP",
                Amount = 24m,
                FiscalQuarter = "q2",
                FiscalMonth = "5",
                WaiverName = ""
            });

            //return Json(Invoice);


            //return PartialView("_InvoiceDetailsPartial", Invoice);
            return PartialView("_PaymentRecordDetailsPartial");
        }
    }
}