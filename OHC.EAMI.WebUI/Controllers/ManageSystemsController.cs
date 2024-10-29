using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AutoMapper;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Data;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class ManageSystemsController : BaseController
    {
        private readonly WcfServiceInvoker _wcfService;
        private readonly ServiceInjector _serviceInjector;
        private readonly IMapper _mapper;
        enum Environment { Test = 1, Stage = 2, Prod = 3 }
        enum PaymentType { Warrant = 1, EFT = 2 }

        public ManageSystemsController()
        {
            _wcfService = new WcfServiceInvoker();
            _serviceInjector = new ServiceInjector();
            _mapper = _serviceInjector.InjectService();
        }

        #region Fund: CRUD public functions...

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddNewFund(int systemId, string systemCode)
        {
            EAMIFundModel model = new EAMIFundModel();
            model.System_ID = systemId;
            model.System_Code = systemCode;
            return PartialView("Fund/AddNewFund", model);
        }

        /// <summary>
        /// POST: Fund/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateFund(EAMIFundModel fundInput)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                Fund fundToBeCreated = _mapper.Map<Fund>(fundInput);
                fundToBeCreated.CreatedBy = User.Identity.Name;
                fundToBeCreated.IsActive = true;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddEAMIFund(fundToBeCreated));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund
        /// </summary>
        /// <param name="includeInactive">This flag controls the UI and Reports data to be shown.
        /// The same Stored Proc will show on Active Funds on UI and All Active + InActive funds in reports.
        /// </param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ViewFunds(int systemID, bool includeInactive = false)
        {
            //Initialize the mapper
            var lstFunds = AdministrationQueries.GetAllFunds(includeInactive, systemID);
            ViewBag.SystemId = systemID;
            ViewBag.SystemCode = GetSystemCode(systemID);

            List<EAMIFundModel> fundModelDto = _mapper.Map<List<Fund>, List<EAMIFundModel>>(lstFunds);

            return PartialView("Fund/ViewFunds", fundModelDto);
        }

        /// <summary>
        /// Edit a fund for a given systemID and Fund ID
        /// </summary>
        /// <param name="fundID">fund ID</param>
        /// <param name="systemID">system ID</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditFund(long fundID, long systemID)
        {
            Fund model = new Fund();
            model = GetFundByID(fundID, systemID);
            model.System_Code = GetSystemCode(systemID);

            EAMIFundModel fundDto = _mapper.Map<EAMIFundModel>(model);
            return PartialView("Fund/EditFund", fundDto);
        }

        /// <summary>
        /// GET: Fund/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateFund(EAMIFundModel fundInput, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                Fund fundToBeUpdated = _mapper.Map<Fund>(fundInput);
                fundToBeUpdated.IsActive = isActive;
                fundToBeUpdated.UpdatedBy = User.Identity.Name;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.UpdateEAMIFund(fundToBeUpdated));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteFund(EAMIFundModel softDeleteFund, bool isActive)
        {
            string response = string.Empty;

            var result = UpdateFund(softDeleteFund, isActive);
            var jsonData = new
            {
                response = ((JsonResult)result).Data
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Exclusive Payment Type: CRUD public functions...

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddNewExclusivePmtType(int systemId, string systemCode)
        {
            EAMIExclusivePmtTypeModel model = new EAMIExclusivePmtTypeModel();

            model.System_ID = systemId;
            model.System_Code = systemCode;
            bool includeInactive = false;

            var lstFunds = AdministrationQueries.GetAllFunds(includeInactive, systemId);

            model.Funds = new List<SelectListItem>();
            foreach (var fund in lstFunds)
            {

                model.Funds.Add(new SelectListItem() { Value = fund.Fund_ID.ToString(), Text = fund.Fund_Code + " (" + fund.Fund_Name + ")" });
            }



            return PartialView("ExclusivePmtType/AddNewExclusivePmtType", model);
        }

        /// <summary>
        /// POST: ExclusivePmtType/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateExclusivePmtType(EAMIExclusivePmtTypeModel exclusivePmtTypeInput)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                ExclusivePmtType exclusivePmtType = new ExclusivePmtType();
                exclusivePmtType.Exclusive_Payment_Type_Code = exclusivePmtTypeInput.Exclusive_Payment_Type_Code;
                exclusivePmtType.Exclusive_Payment_Type_Name = exclusivePmtTypeInput.Exclusive_Payment_Type_Name;
                exclusivePmtType.Exclusive_Payment_Type_Description = exclusivePmtTypeInput.Exclusive_Payment_Type_Description;


                exclusivePmtType.Fund_ID = exclusivePmtTypeInput.Fund_ID;
                exclusivePmtType.System_ID = exclusivePmtTypeInput.System_ID;

                exclusivePmtType.CreatedBy = User.Identity.Name;
                exclusivePmtType.IsActive = true;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddEAMIExclusivePmtType(exclusivePmtType));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: ExclusivePmtType
        /// </summary>
        /// <param name="includeInactive">This flag controls the UI and Reports data to be shown.
        /// The same Stored Proc will show on Active Funds on UI and All Active + InActive funds in reports.
        /// </param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ViewExclusivePmtType(int systemID, bool includeInactive = false)
        {
            var lstExclusivePmtType = AdministrationQueries.GetAllExclusivePmtTypes(includeInactive, systemID);

            ViewBag.SystemId = systemID;
            ViewBag.SystemCode = GetSystemCode(systemID);
            lstExclusivePmtType = lstExclusivePmtType.Where(e => e.System_ID == systemID && e.IsActive == true).ToList();

            return PartialView("ExclusivePmtType/ViewExclusivePmtType", lstExclusivePmtType);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditExclusivePmtType(long exclusivePmtTypeID, long systemID)
        {
            ExclusivePmtType model = new ExclusivePmtType();
            model = GetExclusivePmtTypeByID(exclusivePmtTypeID, systemID);

            model.System_Code = GetSystemCode(systemID);
            EAMIExclusivePmtTypeModel excModel = new EAMIExclusivePmtTypeModel();

            excModel.Exclusive_Payment_Type_ID = model.Exclusive_Payment_Type_ID;
            excModel.System_ID = model.System_ID;
            excModel.System_Code = model.System_Code;
            excModel.Fund_ID = model.Fund_ID;
            excModel.Fund_Code = model.Fund_Code;

            excModel.Exclusive_Payment_Type_Code = model.Exclusive_Payment_Type_Code;
            excModel.Exclusive_Payment_Type_Name = model.Exclusive_Payment_Type_Name;
            excModel.Exclusive_Payment_Type_Description = model.Exclusive_Payment_Type_Description;
            excModel.IsActive = model.IsActive;

            return PartialView("ExclusivePmtType/EditExclusivePmtType", excModel);
        }

        /// <summary>
        /// GET: ExclusivePmtType/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateExclusivePmtType(EAMIExclusivePmtTypeModel exclusivePmtTypeInput, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                ExclusivePmtType exclusivePmtType = new ExclusivePmtType();
                exclusivePmtType.Exclusive_Payment_Type_ID = exclusivePmtTypeInput.Exclusive_Payment_Type_ID; //3;
                exclusivePmtType.Exclusive_Payment_Type_Name = exclusivePmtTypeInput.Exclusive_Payment_Type_Name; //"Updated: DEPOSIT FUND";
                exclusivePmtType.Exclusive_Payment_Type_Description = exclusivePmtTypeInput.Exclusive_Payment_Type_Description; //"Updated: Testing From Code";
                exclusivePmtType.Fund_ID = exclusivePmtTypeInput.Fund_ID;
                exclusivePmtType.System_ID = exclusivePmtTypeInput.System_ID;
                exclusivePmtType.IsActive = isActive;

                exclusivePmtType.UpdatedBy = User.Identity.Name;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.UpdateEAMIExclusivePmtType(exclusivePmtType));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            return View();
        }

        /// <summary>
        /// GET: ExclusivePmtType/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteExclusivePmtType(EAMIExclusivePmtTypeModel exclusivePmtType, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            var result = UpdateExclusivePmtType(exclusivePmtType, isActive);
            var jsonData = new
            {
                status = "OK",
                message = "Exclusive Payment Type successfully deleted"
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Facesheet: CRUD functions...

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddNewFacesheet(int systemId, string systemCode)
        {
            FacesheetModel model = new FacesheetModel();
            model.System_ID = systemId;
            model.System_Code = systemCode;
            //get a list of active fund codes and fund names for the dropdown in UI...
            var lstFunds = AdministrationQueries.GetAllFunds(false, systemId);
            model.List_Funds = new List<SelectListItem>();
            foreach (var fs in lstFunds)
            {
                model.List_Funds.Add(new SelectListItem() { Value = fs.Fund_ID.ToString(), Text = fs.Fund_Code + " (" + fs.Fund_Name + ")" });
            }

            return PartialView("Facesheet/AddNewFacesheetValues", model);
        }

        /// <summary>
        /// POST: Fund/Create
        /// </summary>
        /// <param name="fsInput"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateFacesheet(FacesheetModel fsInput)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //check if their is already a Facesheet for a given fund code in the system.
                var lstFS = AdministrationQueries.GetAllFacesheetValues(false, fsInput.System_ID);
                FacesheetValues fsDtl = lstFS.Where(f => f.Fund_ID == Convert.ToInt32(fsInput.Fund_Code) && f.IsActive == true).FirstOrDefault();
                if (fsDtl != null)
                {
                    var data = new
                    {
                        status = "ERROR",
                        message = "Their is already a Facesheet associated with this Fund Code."
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                FacesheetValues fsToBeCreated = new FacesheetValues();
                fsToBeCreated.Fund_ID = Convert.ToInt32(fsInput.Fund_Code);
                fsToBeCreated.System_ID = fsInput.System_ID;
                fsToBeCreated.Reference_Item = fsInput.Reference_Item;
                fsToBeCreated.Program = fsInput.Program;
                fsToBeCreated.Chapter = fsInput.Chapter;
                fsToBeCreated.Element = fsInput.Element;
                fsToBeCreated.Description = fsInput.Description;
                fsToBeCreated.CreatedBy = User.Identity.Name;
                fsToBeCreated.IsActive = true;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddFacesheetValues(fsToBeCreated));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund
        /// </summary>
        /// <param name="includeInactive">This flag controls the UI and Reports data to be shown.
        /// The same Stored Proc will show on Active Funds on UI and All Active + InActive funds in reports.
        /// </param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ViewFacesheetValues(int systemID, bool includeInactive = false)
        {
            var lstFS = AdministrationQueries.GetAllFacesheetValues(includeInactive, systemID);
            ViewBag.SystemId = systemID;
            ViewBag.SystemCode = GetSystemCode(systemID);
            return PartialView("Facesheet/ViewFacesheetValues", lstFS);
        }

        /// <summary>
        /// Edit a fund for a given systemID and Fund ID
        /// </summary>
        /// <param name="fundID">fund ID</param>
        /// <param name="systemID">system ID</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditFacesheetValues(long fsID, long systemID)
        {
            //Initialize the mapper
            MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.CreateMap<FacesheetValues, FacesheetModel>());

            //Using automapper
            Mapper mapper = new Mapper(config);
            FacesheetValues model = GetFacesheetByID(fsID, systemID);
            model.System_Code = GetSystemCode(systemID);
            model.System_ID = Convert.ToInt32(systemID);
            model.System_Code = GetSystemCode(systemID);

            FacesheetModel fsDto = mapper.Map<FacesheetModel>(model);
            return PartialView("Facesheet/EditFacesheetValues", fsDto);
        }

        /// <summary>
        /// GET: Fund/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateFacesheet(FacesheetModel fsInput, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                FacesheetValues fsToBeUpdated = new FacesheetValues();
                fsToBeUpdated.Facesheet_ID = fsInput.Facesheet_ID;
                fsToBeUpdated.Fund_ID = fsInput.Fund_ID;
                //fsToBeUpdated.Fiscal_Year = fsInput.Fiscal_Year;
                fsToBeUpdated.Chapter = fsInput.Chapter;
                fsToBeUpdated.Program = fsInput.Program;
                fsToBeUpdated.Reference_Item = fsInput.Reference_Item;
                fsToBeUpdated.Element = fsInput.Element;
                fsToBeUpdated.Description = fsInput.Description; //"Updated: Testing From Code";
                fsToBeUpdated.IsActive = isActive;
                fsToBeUpdated.UpdatedBy = User.Identity.Name;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.UpdateFacesheetValues(fsToBeUpdated));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteFacesheet(FacesheetModel softDeleteFund, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            var result = UpdateFacesheet(softDeleteFund, isActive);
            var jsonData = new
            {
                status = "OK",
                message = "Facesheet values successfully deleted"
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Funding Source: CRUD public functions...

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddNewFundingSource(int systemId, string systemCode)
        {
            EAMIFundingSourceModel model = new EAMIFundingSourceModel();
            model.System_ID = systemId;
            model.System_Code = systemCode;


            return PartialView("FundingSource/AddNewFundingSource", model);
        }

        /// <summary>
        /// POST: FundingSource/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateFundingSource(EAMIFundingSourceModel fundingSourceInput)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                FundingSource fundingSource = new FundingSource();
                fundingSource.Code = fundingSourceInput.Code; //"0914";
                fundingSource.Description = fundingSourceInput.Description; //"DEPOSIT FUND";
                fundingSource.System_ID = fundingSourceInput.System_ID;
                //fundingSource.Title = fundingSourceInput.Title; //"Testing From Code";
                fundingSource.CreatedBy = User.Identity.Name;
                fundingSource.IsActive = true;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddEAMIFundingSource(fundingSource));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: FundingSource
        /// </summary>
        /// <param name="includeInactive">This flag controls the UI and Reports data to be shown.
        /// The same Stored Proc will show on Active Funding Source on UI and All Active + InActive funding source in reports.
        /// </param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ViewFundingSource(int systemID, bool includeInactive = false)
        {
            var lstFundingSources = AdministrationQueries.GetAllFundingSources(includeInactive, systemID);
            ViewBag.SystemId = systemID;
            ViewBag.SystemCode = GetSystemCode(systemID);
            return PartialView("FundingSource/ViewFundingSource", lstFundingSources);
        }

        /// <summary>
        /// Edit a funding source for a given systemID and Funding Source ID
        /// </summary>
        /// <param name="fundingSourceID"></param>
        /// <param name="systemID">system ID</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditFundingSource(long fundingSourceID, long systemID)
        {
            FundingSource model = new FundingSource();
            model = GetFundingSourceByID(fundingSourceID, systemID);
            model.System_Code = GetSystemCode(systemID);


            EAMIFundingSourceModel fundingSourceModel = new EAMIFundingSourceModel();

            fundingSourceModel.Funding_Source_ID = model.Funding_Source_ID;
            fundingSourceModel.System_ID = model.System_ID;
            fundingSourceModel.System_Code = model.System_Code;


            fundingSourceModel.Code = model.Code;
            fundingSourceModel.Description = model.Description;
            //fundingSourceModel.Title = model.Title;
            fundingSourceModel.IsActive = model.IsActive;


            return PartialView("FundingSource/EditFundingSource", fundingSourceModel);
        }

        /// <summary>
        /// GET: Fund/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateFundingSource(EAMIFundingSourceModel fundingSourceInput, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                FundingSource fundingSource = new FundingSource();
                fundingSource.Funding_Source_ID = fundingSourceInput.Funding_Source_ID; //3;
                fundingSource.System_ID = fundingSourceInput.System_ID;
                fundingSource.Code = fundingSourceInput.Code; //"Updated: DEPOSIT FUND";
                fundingSource.Description = fundingSourceInput.Description; //"Updated: DEPOSIT FUND";
                //fundingSource.Title = fundingSourceInput.Title; //"Updated: Testing From Code";
                fundingSource.IsActive = isActive;
                fundingSource.UpdatedBy = User.Identity.Name;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.UpdateEAMIFundingSource(fundingSource));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
                //ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Funding Source/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteFundingSource(EAMIFundingSourceModel softDeleteFundingSource, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            var result = UpdateFundingSource(softDeleteFundingSource, isActive);
            var jsonData = new
            {
                status = "OK",
                message = "Funding Source successfully deleted"
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region SCO Properties...

        /// <summary>
        /// GET: SCO Properties
        /// </summary>
        /// <param name="includeInactive">This flag controls the UI and Reports data to be shown.
        /// The same Stored Proc will show on Active SCO Properties on UI.
        /// </param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ViewSCOProperties(int systemID, bool includeInactive = false)
        {
            //Initialize the mapper
            var lstSCOProperties = AdministrationQueries.GetAllSCOProperties(includeInactive, systemID);
            ViewBag.SystemId = systemID;
            ViewBag.SystemCode = GetSystemCode(systemID);

            List<SCOPropertyModel> scoPrptyModelDto = _mapper.Map<List<SCOProperty>, List<SCOPropertyModel>>(lstSCOProperties);

            return PartialView("SCOProperty/ViewSCOProperties", scoPrptyModelDto);
        }

        /// <summary>
        /// Discplay the Add Partial view for SCO Properties
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddNewSCOProperty(int systemId, string systemCode)
        {
            //SCOPropertiesModel model = new SCOPropertiesModel();
            //model.System_ID = systemId;
            //model.System_Code = systemCode;
            SCOProperty scoProperties = new SCOProperty();
            List<Fund> lstFunds = AdministrationQueries.GetAllFunds(false, systemId);
            scoProperties = AdministrationQueries.GetAllSCOPropertyTypes();
            scoProperties.lstFunds = lstFunds;
            SCOPropertyModel scoPrptyModelDto = _mapper.Map<SCOProperty, SCOPropertyModel>(scoProperties);
            scoPrptyModelDto.System_ID = systemId;
            scoPrptyModelDto.System_Code = GetSystemCode(systemId);

            scoPrptyModelDto.SelectedFund = new List<SelectListItem>();
            scoPrptyModelDto.SelectedSCOPropertyTypesLookUp = new List<SelectListItem>();
            scoPrptyModelDto.SelectedSCOPropertiesEnumsLookUp = new List<SelectListItem>();
            scoPrptyModelDto.SelectedPaymentType = new List<SelectListItem>();
            scoPrptyModelDto.SelectedEnvironment = new List<SelectListItem>();

            scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(x => x.SCO_Property_Type_ID==1).ToList();

            foreach (var fund in scoPrptyModelDto.lstFunds)
            {
                scoPrptyModelDto.SelectedFund.Add(new SelectListItem() { Value = fund.Fund_ID.ToString(), Text = fund.Fund_Code + " (" + fund.Fund_Name + ")" });
            }
            foreach (var prptyType in scoPrptyModelDto.lstSCOPropertyTypeLookUp)
            {
                scoPrptyModelDto.SelectedSCOPropertyTypesLookUp.Add(new SelectListItem() { Value = prptyType.SCO_Property_Type_ID.ToString(), Text = prptyType.Code });
            }
            foreach (var prptyType in scoPrptyModelDto.lstSCOPropertyEnumsLookUp)
            {
                scoPrptyModelDto.SelectedSCOPropertiesEnumsLookUp.Add(new SelectListItem() { Value = prptyType.SCO_Property_Enum_ID.ToString(), Text = prptyType.Code });
            }
            var environments = (from SCOPRopertiesEnvironments e in Enum.GetValues(typeof(SCOPRopertiesEnvironments))
                                select new SCOPropertyModel
                                {
                                    EnvironmentID = (int)e,
                                    EnvironmentText = e.ToString()
                                }).ToList();
            foreach (var env in environments)
            {
                scoPrptyModelDto.SelectedEnvironment.Add(new SelectListItem() { Value = env.EnvironmentID.ToString(), Text = env.EnvironmentText });
            }
            var paymentTypes = (from SCOPRopertiesPaymentTypes e in Enum.GetValues(typeof(SCOPRopertiesPaymentTypes))
                                select new SCOPropertyModel
                                {
                                    PaymentTypeID = (int)e,
                                    PaymentTypeText = e.ToString()
                                }).ToList();
            foreach (var pt in paymentTypes)
            {
                scoPrptyModelDto.SelectedPaymentType.Add(new SelectListItem() { Value = pt.PaymentTypeID.ToString(), Text = pt.PaymentTypeText });
            }


            return PartialView("SCOProperty/AddNewSCOProperty", scoPrptyModelDto);
        }

        /// <summary>
        /// POST: SCOProperty/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateSCOProperties(SCOPropertyModel scoPropertyInput, int fundID, int propertyTypeID, string environment, string pmtType, string propertyName, int propertyEnumID)
        {
            //int fund = fundID;
            scoPropertyInput.Fund_ID = fundID;
            scoPropertyInput.SCO_Property_Type_ID = propertyTypeID;
            scoPropertyInput.EnvironmentText = environment;
            scoPropertyInput.PaymentTypeText = pmtType;
            scoPropertyInput.SCO_Property_Name = propertyName;
            scoPropertyInput.SCO_Property_Enum_ID = propertyEnumID;

            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                SCOProperty scoPropertyToBeCreated = _mapper.Map<SCOProperty>(scoPropertyInput);
                scoPropertyToBeCreated.CreatedBy = User.Identity.Name;
                scoPropertyToBeCreated.IsActive = true;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddSCOProperties(scoPropertyToBeCreated));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edit a SCO Property for a given systemID and sco Prpty ID, prpty type id
        /// </summary>
        /// <param name="scoPropertyId"></param>
        /// <param name="scoPropertyName"></param>
        /// <param name="scoPropertyValue"></param>
        /// <param name="propertyTypeId"></param>
        /// <param name="fundId"></param>
        /// <param name="paymentTypeText"></param>
        /// <param name="environmentText"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditSCOProperties([System.Web.Http.FromUri] SCOPropertyModel scoModel)
        {
            SCOProperty scoProperties = new SCOProperty();

            scoProperties = AdministrationQueries.GetAllSCOPropertyTypes();
            List<Fund> lstFunds = AdministrationQueries.GetAllFunds(false, (long)scoModel.System_ID.Value);

            scoProperties.lstFunds = lstFunds;
            SCOPropertyModel scoPrptyModelDto = _mapper.Map<SCOProperty, SCOPropertyModel>(scoProperties);

            scoPrptyModelDto.System_Code = GetSystemCode((long)scoModel.System_ID.Value);
            scoPrptyModelDto.Fund_ID = scoModel.Fund_ID.Value;
            scoPrptyModelDto.System_ID = scoModel.System_ID.Value;
            scoPrptyModelDto.SCO_Property_ID = scoModel.SCO_Property_ID.Value;
            scoPrptyModelDto.EnvironmentText = scoModel.EnvironmentText;
            scoPrptyModelDto.PaymentTypeText = scoModel.PaymentTypeText;
            if (scoModel.EnvironmentText != null) // for dropdown default selection of environment
            {
                scoPrptyModelDto.EnvironmentID = (int)Enum.Parse(typeof(SCOPRopertiesEnvironments), scoModel.EnvironmentText.ToUpper());
            }
            if (scoModel.PaymentTypeText != null)  // for dropdown default selection of Payment Type
            {
                scoPrptyModelDto.PaymentTypeID = (int)Enum.Parse(typeof(SCOPRopertiesPaymentTypes), scoModel.PaymentTypeText.ToUpper());
            }
            scoPrptyModelDto.SCO_Property_Type_ID = scoModel.SCO_Property_Type_ID;
            scoPrptyModelDto.SCO_Property_Name = scoModel.SCO_Property_Name;
            scoPrptyModelDto.SCO_Property_Value = scoModel.SCO_Property_Value;
            scoPrptyModelDto.Description = scoModel.Description;
            scoPrptyModelDto.SCO_Property_Enum_ID = scoModel.SCO_Property_Enum_ID;

            #region Fill dropdowns...

            PopulatePropertyTypesLookUp(scoPrptyModelDto);

            ////need only filtered data: Filter "SCO Property Name" dropdown as per PropertyId and PropertyTypeID
            // if it's a file type (3) property, pick names from [TB_SCO_FILE_PROPERTY]
            if (scoPrptyModelDto.SCO_Property_Type_ID == 3)
            {
                scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(names => names.SCO_Property_Type_ID == 3).ToList();
            }
            else if (scoPrptyModelDto.SCO_Property_Type_ID == 1)//pick names from [TB_SCO_PROPERTY] for Claim Threshold
            {
                scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(x => x.SCO_Property_Type_ID == 1).ToList();
            }
            else //pick names from [TB_SCO_PROPERTY] for Process Indicators
            {
                scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(x => x.SCO_Property_Type_ID == 2).ToList();
            }
            PopulateSCOPropertiesEnumLookUp(scoPrptyModelDto); //need only filtered data
            PopulateFunds(scoPrptyModelDto);
            PopulatePaymentTypes(scoPrptyModelDto);
            PopulateEnvironments(scoPrptyModelDto);

            #endregion

            return PartialView("SCOProperty/EditSCOProperty", scoPrptyModelDto);
            
        }

        /// <summary>
        /// If File is selected, get only file type enums else non-file type enums from DB...
        /// </summary>
        /// <param name="scoPropertyTypeId"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult Dropdown_SelectedIndexChanged(string scoPropertyTypeId)
        {
            int propertyTypeId = Convert.ToInt32(scoPropertyTypeId);
            SCOProperty scoProperties = new SCOProperty();
            scoProperties = AdministrationQueries.GetAllSCOPropertyTypes();
            SCOPropertyModel scoPrptyModelDto = _mapper.Map<SCOProperty, SCOPropertyModel>(scoProperties);

            scoPrptyModelDto.SCO_Property_Type_ID = Convert.ToInt32(scoPropertyTypeId);

            if (propertyTypeId == 3)
            {
                scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(names => names.SCO_Property_Type_ID == 3).ToList();
            }
            else //pick names from [TB_SCO_PROPERTY]
            {
                if (propertyTypeId == 1)
                {
                    scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(x => x.SCO_Property_Type_ID == 1).ToList();
                }
                else
                {
                    scoPrptyModelDto.lstSCOPropertyEnumsLookUp = scoPrptyModelDto.lstSCOPropertyEnumsLookUp.Where(x => x.SCO_Property_Type_ID ==2).ToList();
                }
            }
            PopulateSCOPropertiesEnumLookUp(scoPrptyModelDto);
            return Json(scoPrptyModelDto.lstSCOPropertyEnumsLookUp, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateSCOProperty(SCOPropertyModel dataToBeUpdated, bool isActive)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                SCOProperty prptyToBeUpdated = _mapper.Map<SCOProperty>(dataToBeUpdated);
                prptyToBeUpdated.IsActive = isActive;
                prptyToBeUpdated.UpdatedBy = User.Identity.Name;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.UpdateSCOProperties(prptyToBeUpdated));

                if (cs.Status)
                {
                    status = "OK";
                    message = cs.GetCombinedMessage();
                }
                else
                {
                    status = "ERROR";
                    message = cs.GetCombinedMessage();
                }
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Fund/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteSCOProperty(SCOPropertyModel softDeletescoPrpty, bool isActive)
        {
            string response = string.Empty;
            softDeletescoPrpty.EnvironmentText = PopulateEnvironments(softDeletescoPrpty).SelectedEnvironment.Where(env => env.Value == softDeletescoPrpty.EnvironmentID.ToString()).Select(x => x.Text).FirstOrDefault();
            softDeletescoPrpty.PaymentTypeText = PopulatePaymentTypes(softDeletescoPrpty).SelectedPaymentType.Where(env => env.Value == softDeletescoPrpty.PaymentTypeID.ToString()).Select(x => x.Text).FirstOrDefault();
            var result = UpdateSCOProperty(softDeletescoPrpty, isActive);
            var jsonData = new
            {
                response = ((JsonResult)result).Data
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods...

        /// <summary>
        /// This method returns fund details for the given fund id.
        /// </summary>
        /// <param name="fundID">fundID</param>
        /// <returns></returns>
        private Fund GetFundByID(long fundID, long systemID)
        {
            var lstFunds = AdministrationQueries.GetAllFunds(false, systemID);
            Fund fundDtl = lstFunds.Where(f => f.Fund_ID == fundID).FirstOrDefault();
            return fundDtl;
        }

        /// <summary>
        /// Get all systemCode for a given systemid
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        private string GetSystemCode(long systemID)
        {
            string pDataType = "SYSTEM";
            string strSystemCode = string.Empty;

            Common.CommonStatusPayload<List<EAMIMasterData>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<EAMIMasterData>>>
                                       (svc => svc.GetAllEAMIMasterData(pDataType));

            if (cs.Payload != null && cs.Payload.Count() > 0)
            {
                strSystemCode = cs.Payload.Where(s => s.ID == systemID).FirstOrDefault().Code;
            }

            return strSystemCode;
        }

        /// <summary>
        /// This method returns Exclusive Payment Type details for the given exclusive pmt type id.
        /// </summary>
        /// <param name="fundID">fundID</param>
        /// <returns></returns>
        private ExclusivePmtType GetExclusivePmtTypeByID(long exclusivePmtTypeID, long systemID)
        {
            var lstExclusivePmtTypes = AdministrationQueries.GetAllExclusivePmtTypes(false, systemID);
            var lstFunds = AdministrationQueries.GetAllFunds(false, systemID);
            ExclusivePmtType exclusivePmtTypeDtl = lstExclusivePmtTypes.Where(e => e.Exclusive_Payment_Type_ID == exclusivePmtTypeID).FirstOrDefault();
            Fund fundDtl = lstFunds.Where(e => e.Fund_ID == exclusivePmtTypeDtl.Fund_ID).FirstOrDefault();
            exclusivePmtTypeDtl.Fund_Code = fundDtl.Fund_Code;

            return exclusivePmtTypeDtl;
        }

        /// <summary>
        /// This method returns funding source details for the given funding source id.
        /// </summary>
        /// <param name="fundID">fundID</param>
        /// <returns></returns>
        private FundingSource GetFundingSourceByID(long fundingSourceID, long systemID)
        {
            var lstFundingSources = AdministrationQueries.GetAllFundingSources(false, systemID);
            FundingSource fundingSourceDtl = lstFundingSources.Where(f => f.Funding_Source_ID == fundingSourceID).FirstOrDefault();
            return fundingSourceDtl;
        }

        /// <summary>
        /// This method returns SCO Property details for the given SCO id.
        /// </summary>
        /// <param name="fundID">fundID</param>
        /// <returns></returns>
        private SCOProperty GetSCOByID(long scoPrptyID, long systemID)
        {
            var lstScoPrpty = AdministrationQueries.GetAllSCOProperties(false, systemID);
            SCOProperty scoPropertyDtl = lstScoPrpty.Where(s => s.SCO_Property_ID == scoPrptyID).FirstOrDefault();
            return scoPropertyDtl;
        }

        /// <summary>
        /// This method returns facesheet details for the given facesheet id.
        /// </summary>
        /// <param name="fsID">fundID</param>
        /// <param name="systemID"></param>
        /// <returns></returns>
        private FacesheetValues GetFacesheetByID(long fsID, long systemID)
        {
            var lstFS = AdministrationQueries.GetAllFacesheetValues(false, systemID);
            FacesheetValues fsDtl = lstFS.Where(f => f.Facesheet_ID == fsID).FirstOrDefault();
            return fsDtl;
        }

        /// <summary>
        /// Populates the Property Types from database...
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private SCOPropertyModel PopulatePropertyTypesLookUp(SCOPropertyModel dto)
        {
            dto.SelectedSCOPropertyTypesLookUp = new List<SelectListItem>();
            //set Property Types and selected item.
            foreach (var prptyType in dto.lstSCOPropertyTypeLookUp)
            {
                dto.SelectedSCOPropertyTypesLookUp.Add(new SelectListItem() { Value = prptyType.SCO_Property_Type_ID.ToString(), Text = prptyType.Code });
            }
            return dto;
        }

        /// <summary>
        /// Populates the Property names from database for the dropdowns...
        /// </summary>
        /// <param name="scoPrptyModelDto"></param>
        /// <returns></returns>
        private SCOPropertyModel PopulateSCOPropertiesEnumLookUp(SCOPropertyModel scoPrptyModelDto)
        {
            scoPrptyModelDto.SelectedSCOPropertiesEnumsLookUp = new List<SelectListItem>();
            foreach (var prptyType in scoPrptyModelDto.lstSCOPropertyEnumsLookUp)
            {
                scoPrptyModelDto.SelectedSCOPropertiesEnumsLookUp.Add(new SelectListItem() { Value = prptyType.SCO_Property_Enum_ID.ToString(), Text = prptyType.Code });
            }
            return scoPrptyModelDto;
        }

        /// <summary>
        /// Populates the Funds from database for the Dropdowns...
        /// </summary>
        /// <param name="scoPrptyModelDto"></param>
        /// <returns></returns>
        private SCOPropertyModel PopulateFunds(SCOPropertyModel scoPrptyModelDto)
        {
            scoPrptyModelDto.SelectedFund = new List<SelectListItem>();
            foreach (var fund in scoPrptyModelDto.lstFunds)
            {
                //enabling/disabling is done in Javascript function in Edit View.
                scoPrptyModelDto.SelectedFund.Add(new SelectListItem() { Value = fund.Fund_ID.ToString(), Text = fund.Fund_Code + " (" + fund.Fund_Name + ")", Disabled = false, Selected = true });
            }
            return scoPrptyModelDto;
        }


        /// <summary>
        /// Populates the Environments from Enum class...
        /// </summary>
        /// <param name="scoPrptyModelDto"></param>
        /// <returns></returns>
        private SCOPropertyModel PopulateEnvironments(SCOPropertyModel scoPrptyModelDto)
        {
            scoPrptyModelDto.SelectedEnvironment = new List<SelectListItem>();
            var environments = (from SCOPRopertiesEnvironments e in Enum.GetValues(typeof(SCOPRopertiesEnvironments))
                                select new SCOPropertyModel
                                {
                                    EnvironmentID = (int)e,
                                    EnvironmentText = e.ToString()
                                }).ToList();
            foreach (var env in environments)
            {
                scoPrptyModelDto.SelectedEnvironment.Add(new SelectListItem() { Value = env.EnvironmentID.ToString(), Text = env.EnvironmentText });
            }
            return scoPrptyModelDto;
        }

        /// <summary>
        /// Populates the Payment Types from Enum class...
        /// </summary>
        /// <param name="scoPrptyModelDto"></param>
        /// <returns></returns>
        private SCOPropertyModel PopulatePaymentTypes(SCOPropertyModel scoPrptyModelDto)
        {
            scoPrptyModelDto.SelectedPaymentType = new List<SelectListItem>();
            var paymentTypes = (from SCOPRopertiesPaymentTypes e in Enum.GetValues(typeof(SCOPRopertiesPaymentTypes))
                                select new SCOPropertyModel
                                {
                                    PaymentTypeID = (int)e,
                                    PaymentTypeText = e.ToString()
                                }).ToList();
            foreach (var pt in paymentTypes)
            {
                scoPrptyModelDto.SelectedPaymentType.Add(new SelectListItem() { Value = pt.PaymentTypeID.ToString(), Text = pt.PaymentTypeText });
            }
            return scoPrptyModelDto;
        }

        #endregion
    }
}
