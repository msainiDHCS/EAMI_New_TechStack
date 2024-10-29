using OHC.EAMI.WebUI.Data;
using OHC.EAMI.WebUI.Models;
using OHC.EAMI.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Controllers
{
    public class InvoiceController : Controller
    {


    //    public ActionResult Test()
    //    {
    //        ////The following will be replaced with service call
    //        PaymentExchangeEntity payee1 = new PaymentExchangeEntity();
    //        //payee1.PayeeCode = "PayeeCode99";
    //        //payee1.Name = "Bruce Wayne";
    //        //payee1.Address = "123 Joker Way";
    //        //payee1.City = "Gotham";
    //        //payee1.Zip = "95412";
    //        //payee1.State = "NY";
    //        //payee1.PhoneNumber = "916-425-8741";

    //        //Payee payee2 = new Payee();
    //        //payee2.PayeeCode = "PayeeCode777";
    //        //payee2.Name = "Optimus";
    //        //payee2.Address = "532 Prime Court";
    //        //payee2.City = "Autobotia";
    //        //payee2.Zip = "24158-6512";
    //        //payee2.State = "CA";
    //        //payee2.PhoneNumber = "619-145-8424";

    //        return View(payee1);
    //    }







    //    // GET: Invoices
    //    public ActionResult Invoices(string outterMostTableId, string outterMostRowId, string vendorDisplayId)
    //    {

    //        //The following will be replaced with service call
    //        Invoice Invoice1 = new Invoice();
    //        Invoice1.FiscalYear = "2004";

    //        Invoice1.AssignChecked = true;

    //        Invoice1.InvoiceNumber = (vendorDisplayId + "_" + "InvoiceNumber3").Trim();
    //        Invoice1.InvoiceDate = new DateTime(2009, 5, 7);
    //        Invoice1.PaymentExchangeEntity = new PaymentExchangeEntity { EntityName = "Cal MediConnect" };
    //        Invoice1.Contract = new Contract { ContractNumber = "16423-8424" };
    //        //Invoice1.ModelType = new ModelType { ModelName = "COHS" };

    //        Invoice1.InvoiceStatuses = new List<InvoiceStatus>();
    //        Invoice1.InvoiceStatuses.Add(new InvoiceStatus { InvoiceStatusType = new InvoiceStatusType { Code = "Unassigned" } });

    //        Invoice1.Amount = 1.01m;
    //        Invoice1.PaymentExchangeEntity.EntityID = "CMC0000006-00";
    //        Invoice1.IndexCode = "IndexCode 7";
    //        Invoice1.ObjectDetailCode = "ObjectDetailCode 8";
    //        Invoice1.ObjectAgencyCode = "ObjectAgencyCode 9";
    //        Invoice1.PCACode = "PCACode 10";
    //        Invoice1.Contract.EffectiveDateFrom = Convert.ToDateTime("1/2/02");
    //        Invoice1.Contract.EffectiveDateTo = Convert.ToDateTime("5/17/16");
    //        Invoice1.Contract.CurrentQuarter = "Q4";     /////???????????????what is this?????????????????

    //        Invoice1.FundingDetails = new List<FundingDetail>();
    //        Invoice1.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 99,
    //            FundingSourceName = "FMAP",
    //            Amount = 5.52m,
    //            FiscalQuarter = "Q2",
    //            FiscalMonth = "8",
    //            WaiverName = "MyWaiverName"
    //        });

    //        Invoice Invoice2 = new Invoice();
    //        Invoice2.FiscalYear = "2008";

    //        Invoice2.AssignChecked = true;

    //        Invoice2.InvoiceNumber = (vendorDisplayId + "_" + "InvoiceNumber5").Trim();
    //        Invoice2.InvoiceDate = new DateTime(2009, 5, 7);
    //        Invoice2.PaymentExchangeEntity = new PaymentExchangeEntity { EntityName = "Cal MediConnect" };
    //        Invoice2.Contract = new Contract { ContractNumber = "16423-8424" };
    //        Invoice2.ModelType = new ModelType { ModelName = "COHS" };

    //        Invoice2.InvoiceStatuses = new List<InvoiceStatus>();
    //        Invoice2.InvoiceStatuses.Add(new InvoiceStatus { InvoiceStatusType = new InvoiceStatusType { Code = "Unassigned" } });

    //        Invoice2.Amount = 1.01m;
    //        Invoice2.PaymentExchangeEntity.EntityID = "CMC0000099-02";
    //        Invoice2.IndexCode = "IndexCode 7";
    //        Invoice2.ObjectDetailCode = "ObjectDetailCode 8";
    //        Invoice2.ObjectAgencyCode = "ObjectAgencyCode 9";
    //        Invoice2.PCACode = "PCACode 10";
    //        Invoice2.Contract.EffectiveDateFrom = Convert.ToDateTime("1/2/02");
    //        Invoice2.Contract.EffectiveDateTo = Convert.ToDateTime("5/17/16");
    //        Invoice2.Contract.CurrentQuarter = "Q4";     /////???????????????what is this?????????????????

    //        Invoice2.FundingDetails = new List<FundingDetail>();
    //        Invoice2.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 99,
    //            FundingSourceName = "FMAP",
    //            Amount = 5.52m,
    //            FiscalQuarter = "Q2",
    //            FiscalMonth = "8",
    //            WaiverName = "MyWaiverName"
    //        });

    //        Invoice2.PayDate = "11/15/1999";

    //        //List<Invoice> listOfInvoices = new List<Invoice>();
    //        //listOfInvoices.Add(Invoice1);
    //        //listOfInvoices.Add(Invoice2);

    //        //InvoicesViewModel InvoicesViewModel = new InvoicesViewModel();
    //        //InvoicesViewModel.Invoices = listOfInvoices.AsQueryable<Invoice>();


    //        Invoice Invoice4 = new Invoice();
    //        Invoice4.FiscalYear = "2017";

    //        Invoice4.AssignChecked = true;

    //        Invoice4.InvoiceNumber = (vendorDisplayId + "_" + "InvoiceNumber7").Trim();
    //        Invoice4.InvoiceDate = new DateTime(2009, 5, 7);
    //        Invoice4.PaymentExchangeEntity = new PaymentExchangeEntity { EntityName = "Cal MediConnect" };
    //        Invoice4.Contract = new Contract { ContractNumber = "16423-8424" };
    //        Invoice4.ModelType = new ModelType { ModelName = "COHS" };

    //        Invoice4.InvoiceStatuses = new List<InvoiceStatus>();
    //        Invoice4.InvoiceStatuses.Add(new InvoiceStatus { InvoiceStatusType = new InvoiceStatusType { Code = "Unassigned" } });

    //        Invoice4.Amount = 1.01m;
    //        Invoice4.PaymentExchangeEntity.EntityID = "CMC0000099-02";
    //        Invoice4.IndexCode = "IndexCode 7";
    //        Invoice4.ObjectDetailCode = "ObjectDetailCode 8";
    //        Invoice4.ObjectAgencyCode = "ObjectAgencyCode 9";
    //        Invoice4.PCACode = "PCACode 10";
    //        Invoice4.Contract.EffectiveDateFrom = Convert.ToDateTime("1/2/02");
    //        Invoice4.Contract.EffectiveDateTo = Convert.ToDateTime("5/17/16");
    //        Invoice4.Contract.CurrentQuarter = "Q4";     /////???????????????what is this?????????????????

    //        Invoice4.FundingDetails = new List<FundingDetail>();
    //        Invoice4.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 99,
    //            FundingSourceName = "FMAP",
    //            Amount = 5.52m,
    //            FiscalQuarter = "Q2",
    //            FiscalMonth = "8",
    //            WaiverName = "MyWaiverName"
    //        });


    //        Invoice Invoice3 = new Invoice();
    //        Invoice3.FiscalYear = "1999";

    //        Invoice3.AssignChecked = true;

    //        Invoice3.InvoiceNumber = (vendorDisplayId + "_" + "InvoiceNumber2").Trim();
    //        Invoice3.InvoiceDate = new DateTime(2009, 5, 7);
    //        Invoice3.PaymentExchangeEntity = new PaymentExchangeEntity { EntityName = "Cal MediConnect" };
    //        Invoice3.Contract = new Contract { ContractNumber = "16423-8424" };
    //        Invoice3.ModelType = new ModelType { ModelName = "COHS" };

    //        Invoice3.InvoiceStatuses = new List<InvoiceStatus>();
    //        Invoice3.InvoiceStatuses.Add(new InvoiceStatus { InvoiceStatusType = new InvoiceStatusType { Code = "Unassigned" } });

    //        Invoice3.Amount = 1.01m;
    //        Invoice3.PaymentExchangeEntity.EntityID = "CMC0000099-02";
    //        Invoice3.IndexCode = "IndexCode 7";
    //        Invoice3.ObjectDetailCode = "ObjectDetailCode 8";
    //        Invoice3.ObjectAgencyCode = "ObjectAgencyCode 9";
    //        Invoice3.PCACode = "PCACode 10";
    //        Invoice3.Contract.EffectiveDateFrom = Convert.ToDateTime("1/2/02");
    //        Invoice3.Contract.EffectiveDateTo = Convert.ToDateTime("5/17/16");
    //        Invoice3.Contract.CurrentQuarter = "Q4";     /////???????????????what is this?????????????????

    //        Invoice3.FundingDetails = new List<FundingDetail>();
    //        Invoice3.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 99,
    //            FundingSourceName = "FMAP",
    //            Amount = 5.52m,
    //            FiscalQuarter = "Q2",
    //            FiscalMonth = "8",
    //            WaiverName = "MyWaiverName"
    //        });

    //        Invoice3.PayDate = "01/06/2017";

    //        List<Invoice> listOfInvoices = new List<Invoice>();
    //        listOfInvoices.Add(Invoice1);
    //        listOfInvoices.Add(Invoice2);
    //        listOfInvoices.Add(Invoice3);
    //        listOfInvoices.Add(Invoice4);

    //        AssignInvoicesViewModel InvoicesViewModel = new AssignInvoicesViewModel();
    //        InvoicesViewModel.VendorDisplayId = vendorDisplayId;
    //        InvoicesViewModel.Invoices = listOfInvoices.AsQueryable<Invoice>();

    //        return PartialView("_InvoicesPartial", InvoicesViewModel);

    //        //return View(InvoicesViewModel);
    //    }

        



    //    // GET: Invoices
    //    //public ActionResult AssignInvoices()
    //    //{

    //    //    //The following will be replaced with service call
    //    //    List<AssignInvoicesViewModel> listAssignInvoicesViewModel = InvoiceQueries.GetVendorsDisplayList();

    //    //    return View(listAssignInvoicesViewModel.AsQueryable<AssignInvoicesViewModel>());

    //    //}





    ////[HttpGet]
    ////public ActionResult AssignInvoices_LoadPaymentExchangeEntityGroups()
    ////{

    ////    //The following will be replaced with service call
    ////    List<AssignInvoicesViewModel> listAssignInvoicesViewModel = InvoiceQueries.GetVendorsDisplayList();

    ////    return Json(new { data = listAssignInvoicesViewModel });



    ////}

    //[HttpGet]
    //    public ActionResult AssignInvoices_GetSearchCriteriaVendors()
    //    {
    //        //The following will be replaced with service call
    //        PaymentExchangeEntity payee1 = new PaymentExchangeEntity();
    //        PaymentExchangeEntity payee2 = new PaymentExchangeEntity();
    //        PaymentExchangeEntity payee3 = new PaymentExchangeEntity();
    //        PaymentExchangeEntity payee4 = new PaymentExchangeEntity();
    //        PaymentExchangeEntity payee5 = new PaymentExchangeEntity();

    //        payee1.EntityName = "Santa Cruz-Monterey-Merced Managed Medical Care Commission";
    //        payee2.EntityName = "Santa Cruz-Monterey-Merced Managed Medical Care Commission";
    //        payee3.EntityName = "Vendor 3";
    //        payee4.EntityName = "Vendor 4";
    //        payee5.EntityName = "Vendor 5";


    //        List<PaymentExchangeEntity> listPaymentExchangeEntity = new List<PaymentExchangeEntity>();
    //        listPaymentExchangeEntity.Add(payee1);
    //        listPaymentExchangeEntity.Add(payee2);
    //        listPaymentExchangeEntity.Add(payee3);
    //        listPaymentExchangeEntity.Add(payee4);
    //        listPaymentExchangeEntity.Add(payee5);

    //        return Json(listPaymentExchangeEntity, JsonRequestBehavior.AllowGet);
    //    }

    //    [HttpGet]
    //    public ActionResult AssignInvoices_GetSearchCriteriaModelTypes()
    //    {
    //        //The following will be replaced with service call
    //        ModelType modelType1 = new ModelType();
    //        ModelType modelType2 = new ModelType();
    //        ModelType modelType3 = new ModelType();
    //        ModelType modelType4 = new ModelType();
    //        ModelType modelType5 = new ModelType();
    //        ModelType modelType6 = new ModelType();
    //        ModelType modelType7 = new ModelType();
    //        ModelType modelType8 = new ModelType();
    //        ModelType modelType9 = new ModelType();
    //        ModelType modelType10 = new ModelType();

    //        modelType1.ModelName = "Two-Plan Local Initiative";
    //        modelType2.ModelName = "San Francisco Family Mosaic";
    //        modelType3.ModelName = "Model3";
    //        modelType4.ModelName = "Model4";
    //        modelType5.ModelName = "Model5";
    //        modelType6.ModelName = "Model6";
    //        modelType7.ModelName = "Model7";
    //        modelType8.ModelName = "Model8";
    //        modelType9.ModelName = "Model9";
    //        modelType10.ModelName = "Model10";

    //        List<ModelType> listModelType = new List<ModelType>();
    //        listModelType.Add(modelType1);
    //        listModelType.Add(modelType2);
    //        listModelType.Add(modelType3);
    //        listModelType.Add(modelType4);
    //        listModelType.Add(modelType5);
    //        listModelType.Add(modelType6);
    //        listModelType.Add(modelType7);
    //        listModelType.Add(modelType8);
    //        listModelType.Add(modelType9);
    //        listModelType.Add(modelType10);

    //        return Json(listModelType, JsonRequestBehavior.AllowGet);
    //    }

    //    public ActionResult Detail(Invoice Invoice)
    //    {
    //        //The following will be replaced with service call
    //        Invoice.PaymentExchangeEntity = new PaymentExchangeEntity();
    //        Invoice.PaymentExchangeEntity.EntityID = "vCodeVal";
    //        Invoice.IndexCode = "iCodeVal";
    //        Invoice.ObjectAgencyCode = "oACode";
    //        Invoice.ObjectDetailCode = "oDCode";
    //        Invoice.PCACode = "pCACode";
    //        Invoice.Contract = new Contract();
    //        Invoice.Contract.EffectiveDateFrom = new DateTime(2003, 1, 2);
    //        Invoice.Contract.EffectiveDateTo = new DateTime(2007, 6, 23);
    //        Invoice.Contract.ContractNumber = "cNumber";
    //        Invoice.Contract.CurrentQuarter = "Q4";

    //        Invoice.FundingDetails = new List<FundingDetail>();
    //        Invoice.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 777,
    //            //FundingSourceName = Invoice.InvoiceNumber + "100% State",
    //            FundingSourceName = "100% State",
    //            Amount = 987.04m,
    //            FiscalQuarter = "Q1",
    //            FiscalMonth = "12",
    //            WaiverName = "State_WaiverName"
    //        });

    //        Invoice.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 555,
    //            FundingSourceName = "Real FMAP",
    //            Amount = 105m,
    //            FiscalQuarter = "Q4",
    //            FiscalMonth = "03",
    //            WaiverName = ""
    //        });

    //        Invoice.FundingDetails.Add(new FundingDetail
    //        {
    //            FundingDetailID = 227,
    //            FundingSourceName = "Regular FMAP",
    //            Amount = 24m,
    //            FiscalQuarter = "q2",
    //            FiscalMonth = "5",
    //            WaiverName = ""
    //        });

    //        //return Json(Invoice);


    //        //return PartialView("_InvoiceDetailsPartial", Invoice);
    //        return PartialView("_PaymentRecordDetailsPartial");
    //    }
    }
}