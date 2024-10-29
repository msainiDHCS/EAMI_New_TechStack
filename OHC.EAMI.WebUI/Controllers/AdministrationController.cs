using AutoMapper;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Data;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class AdministrationController : BaseController
    {
        private readonly WcfServiceInvoker _wcfService;
        private readonly ServiceInjector _serviceInjector;
        private readonly IMapper _mapper;

        public AdministrationController()
        {
            _wcfService = new WcfServiceInvoker();
            _serviceInjector = new ServiceInjector();
            _mapper = _serviceInjector.InjectService();
        }


        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
        public ActionResult logout()
        {
            string _authCacheKey = HttpContext.Session.SessionID + "_UserAuth";
            CacheManager<GenericPrincipal>.Remove(_authCacheKey, "AUTH");
            HttpContext.User = null;
            return RedirectToAction("Login", "Account");
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
        public ActionResult closeProgram()
        {
            Session["ProgramChoiceId"] = 0;
            return RedirectToAction("Index", "ProgramChoice");
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.SUPERVISOR)]
        public ActionResult Forms()
        {
            return View();
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        public ActionResult ManageUsers()
        {
            return View();
        }

        [AuthorizeResource(EAMIRole.SUPERVISOR)]
        public ActionResult ViewUsers()
        {
            return View();
        }


        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ManageUsersList()
        {
            List<EAMIUser> lstUsers = new List<EAMIUser>();            
            Common.CommonStatusPayload<List<EAMIUser>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<EAMIUser>>>
                                       (svc => svc.GetAllEAMIUsers());
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            if (cs.Payload != null && cs.Payload.Count() > 0)
                lstUsers.AddRange(cs.Payload.OrderByDescending(a => a.LastUpdateDate).ToList());

            return PartialView(lstUsers);
        }


        [AuthorizeResource(EAMIRole.SUPERVISOR)]
        [AjaxOnly]
        public ActionResult ViewUsersList()
        {
            List<EAMIUser> lstUsers = new List<EAMIUser>();
            Common.CommonStatusPayload<List<EAMIUser>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<EAMIUser>>>
                                       (svc => svc.GetAllEAMIUsers());
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            if (cs.Payload != null && cs.Payload.Count() > 0)
            {
                lstUsers.AddRange(cs.Payload.OrderByDescending(a => a.LastUpdateDate).Where(b => b.IsActive).ToList());
            }
            lstUsers.RemoveAll(x => x.DelimitedRoleNames.Contains("Administrator"));

            return PartialView(lstUsers);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        public JsonResult CheckIfUserNameExists(string userName, long userTypeID)
        {
            Common.CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                      (svc => svc.CheckEAMIUserValidity(userName, userTypeID));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            return Json(cs.Status.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        public JsonResult CheckIfUserNameIsValidADAccount(string userName, string domainname)
        {
            bool isUserValid = false;

            var userfn = Util.ActiveDirectoryAccess.Instance.GetUserFullName(domainname, userName);

            if (!string.IsNullOrEmpty(userfn))
                  isUserValid = true;
             //  isUserValid = false;

            return Json(isUserValid.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        public JsonResult CheckIfCurrentPasswordIsRight(string password, string currenthash)
        {
            return Json(EAMIHashGenerator.VerifyHash(password, "SHA512", currenthash).ToString(), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddNewUser2(EAMIUserModel input)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                EAMIUser user = new EAMIUser();
                user.User_Name = input.UserName;
                user.Display_Name = input.DisplayName;
                user.Domain_Name = input.DomainName;
                user.User_EmailAddr = Util.ActiveDirectoryAccess.Instance.GetUserEmailAddr(input.DomainName, input.UserName);
                user.IsActive = true;
                user.User_Password = input.UserPassword;
                user.User_Type = new EAMIAuthBase();
                user.User_Type.ID = input.UserTypeID;

                user.User_Roles = new List<EAMIAuthBase>();
                foreach (var s in input.UserRoles.Where(a => a.IsSelected))
                {
                    user.User_Roles.Add(new EAMIAuthBase() { ID = s.ID });
                }

                user.User_Systems = new List<EAMIAuthBase>();
                foreach (var s in input.UserSystems.Where(a => a.IsSelected))
                {
                    user.User_Systems.Add(new EAMIAuthBase() { ID = s.ID });
                }

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddUpdateEAMIUser(user, User.Identity.Name));

                if (cs.Status)
                {
                    status = "OK"; message = "";
                }
                else
                {
                    status = "ERROR"; message = cs.GetCombinedMessage();
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddNewUser()
        {
            EAMIUserModel model = new EAMIUserModel();

            CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>>>
                                      (svc => svc.GetEAMIAuthorizationLookUps());

            if (cs.Status)
            {
                model.UserRoles = new List<EAMIElement>();

                foreach (EAMIAuthBase role in cs.Payload.Where(a => a.Item1 == "ROLE").Select(a => a.Item2).ToList())
                {
                    model.UserRoles.Add(new EAMIElement() { ID = role.ID, Text = role.Name, IsSelected = false });
                }

                model.UserSystems = new List<EAMIElement>();

                foreach (EAMIAuthBase sys in cs.Payload.Where(a => a.Item1 == "SYSTEM").Select(a => a.Item2).ToList())
                {
                    model.UserSystems.Add(new EAMIElement() { ID = sys.ID, Text = sys.Name, IsSelected = true });
                }

                model.UserTypes = new List<SelectListItem>();

                foreach (EAMIAuthBase utype in cs.Payload.Where(a => a.Item1 == "USERTYPE").Select(a => a.Item2).ToList())
                {
                    model.UserTypes.Add(new SelectListItem() { Value = utype.ID.ToString(), Text = utype.Name });
                }

            }

            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            model.DomainName = Util.Helper.GetConfigValue("DefaultDomainNameForUserAdd");

            return PartialView(model);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        private bool CheckMasterData(List<EAMIAuthBase> data, long idToCheck)
        {
            bool doesDataExist = false;

            if (data != null && data.Count() > 0)
            {
                var t = data.Where(a => a.ID == idToCheck);

                if (t != null && t.Count() > 0)
                    doesDataExist = true;
            }

            return doesDataExist;
        }


        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditUser(long userID)
        {
            EAMIUserModel model = new EAMIUserModel();

            CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> cs1 = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>>>
                                      (svc => svc.GetEAMIAuthorizationLookUps());
            ErrorHandlerController.CheckFatalException(cs1.Status, cs1.IsFatal, cs1.GetCombinedMessage());

            CommonStatusPayload<EAMIUser> cs2 = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<EAMIUser>>
                                      (svc => svc.GetEAMIUserByID(userID));
            ErrorHandlerController.CheckFatalException(cs2.Status, cs2.IsFatal, cs2.GetCombinedMessage());
            
            if (cs1.Status)
            {
                model.UserRoles = new List<EAMIElement>();

                foreach (EAMIAuthBase role in cs1.Payload.Where(a => a.Item1 == "ROLE").Select(a => a.Item2).ToList())
                {
                    if (cs1.Payload.Where(a => a.Item1 == "ROLE_WITH_INACTIVE_USERROLE" && a.Item2.ID == (int)userID).Select(a => a.Item2.Code).ToList().Contains(role.Code))
                    {
                        model.UserRoles.Add(new EAMIElement() { ID = role.ID, Text = role.Name, IsSelected = false });
                    }
                    else
                    {
                        model.UserRoles.Add(new EAMIElement() { ID = role.ID, Text = role.Name, IsSelected = CheckMasterData(cs2.Payload.User_Roles, role.ID) });
                    }
                }

                model.UserSystems = new List<EAMIElement>();

                foreach (EAMIAuthBase sys in cs1.Payload.Where(a => a.Item1 == "SYSTEM").Select(a => a.Item2).ToList())
                {
                    if (cs1.Payload.Where(a => a.Item1 == "SYSTEM_WITH_INACTIVE_SYSTEM_USER" && a.Item2.ID == (int)userID).Select(a => a.Item2.Code).ToList().Contains(sys.Code))
                    {
                        model.UserSystems.Add(new EAMIElement() { ID = sys.ID, Text = sys.Name, IsSelected = false });
                    }
                    else
                    {
                        model.UserSystems.Add(new EAMIElement() { ID = sys.ID, Text = sys.Name, IsSelected = CheckMasterData(cs2.Payload.User_Systems, sys.ID) });
                    }
                }

                model.UserTypes = new List<SelectListItem>();

                foreach (EAMIAuthBase utype in cs1.Payload.Where(a => a.Item1 == "USERTYPE").Select(a => a.Item2).ToList())
                {
                    model.UserTypes.Add(new SelectListItem() { Value = utype.ID.ToString(), Text = utype.Name, Selected = (cs2.Payload.User_Type.ID == utype.ID) });
                }
            }

            model.DisplayName = cs2.Payload.Display_Name;
            model.DomainName = cs2.Payload.Domain_Name;
            model.IsActive = cs2.Payload.IsActive;
            model.UserID = cs2.Payload.User_ID ?? Convert.ToInt32(cs2.Payload.User_ID);
            model.UserName = cs2.Payload.User_Name;
            model.OriginalUserName = cs2.Payload.User_Name;//used for edit check
            model.UserTypeID = cs2.Payload.User_Type.ID;
            model.UserPassword = HttpUtility.UrlEncode(cs2.Payload.User_Password);
            return PartialView(model);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult EditUser2(EAMIUserModel input)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                List<PaymentRecForUser> userPayments = AdministrationQueries.GetPaymentRecordsByUser(input.UserID);
                if (userPayments.Count > 0 && input.IsActive)
                {
                    bool ProcessorRole;
                    foreach (var s in input.UserRoles)
                    {
                        if (s.Text == "Processor" && !s.IsSelected)
                        {
                            ProcessorRole = true;
                            status = "ERROR";
                            message = "Unable to change processor role at this time because of inflight payments, </br> please check back when payments are processed.";
                            var jsonResult = new
                            {
                                status = status,
                                message = message
                            };
                            return Json(jsonResult, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                if (!input.IsActive && userPayments.Count > 0)
                {
                    status = "ERROR";
                    message = "Unable to deactivate this user. </br> Some of the Payment set(s) currently being processed are associated with the user. </br> Please reassign the Payment set(s) to supervisor before you continue.";
                    var jsonResult = new
                    {
                        status = status,
                        message = message
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                EAMIUser user = new EAMIUser();
                user.User_ID = input.UserID;
                user.User_Name = input.UserName;
                user.Display_Name = input.DisplayName;
                user.Domain_Name = input.DomainName;
                user.IsActive = input.IsActive;
                if (!string.IsNullOrEmpty(input.NewUserPassword))
                    user.User_Password = input.NewUserPassword;
                else
                    user.User_Password = null;
                user.User_Type = new EAMIAuthBase();
                user.User_Type.ID = input.UserTypeID;

                user.User_Roles = new List<EAMIAuthBase>();
                foreach (var s in input.UserRoles.Where(a => a.IsSelected))
                {
                    user.User_Roles.Add(new EAMIAuthBase() { ID = s.ID });
                }

                user.User_Systems = new List<EAMIAuthBase>();
                foreach (var s in input.UserSystems.Where(a => a.IsSelected))
                {
                    user.User_Systems.Add(new EAMIAuthBase() { ID = s.ID });
                }

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddUpdateEAMIUser(user, User.Identity.Name));

                if (cs.Status)
                {
                    status = "OK"; message = "";

                    if (User.Identity.Name.ToLower() == user.User_Name.ToLower())
                    {
                        //clear the cache so credentials can be reloaded on next server get
                        string _authCacheKey = HttpContext.Session.SessionID + "_UserAuth";
                        CacheManager<GenericPrincipal>.Remove(_authCacheKey, "AUTH");
                    }
                }
                else
                {
                    status = "ERROR"; message = cs.GetCombinedMessage();
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeactivateUser(EAMIUserModel input)
        {
            string status = string.Empty;
            string message = string.Empty;
            string deactive = string.Empty;

            if (ModelState.IsValid)
            {
                EAMIUser user = new EAMIUser();
                user.User_ID = input.UserID;             
                user.IsActive = input.IsActive;

                List<PaymentRecForUser> userPayments = AdministrationQueries.GetPaymentRecordsByUser(input.UserID);
                if (userPayments.Count > 0)
                {
                    status = "ERROR";
                    message = "Unable to deactivate this user. </br> Some of the Payment set(s) currently being processed are associated with the user. </br> Please reassign the Payment set(s) to supervisor before you continue.";
                    var jsonResult = new
                    {
                        status = status,
                        message = message
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }

                // user.IsActive = Util.ActiveDirectoryAccess.Instance.GetUserEmailAddr(input.DomainName, input.UserName);
                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.DeactivateEAMIUser(user, User.Identity.Name));
                
                if (cs.Status)
                {
                    status = "OK"; message = "";

                    //if (User.Identity.Name.ToLower() == user.User_Name.ToLower())
                    //{
                        //clear the cache so credentials can be reloaded on next server get
                        string _authCacheKey = HttpContext.Session.SessionID + "_UserAuth";
                        CacheManager<GenericPrincipal>.Remove(_authCacheKey, "AUTH");
                        deactive = "User deactivated successfully in EAMI";


                   //}
                }
                else
                {
                    status = "ERROR"; message = cs.GetCombinedMessage();
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message

            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Active status EAMI UserInfo
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        public JsonResult GetActivestatusEAMIUserInfo(long userID)
        {
            Common.CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                      (svc => svc.GetActivestatusEAMIUserInfo(userID));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            return Json(cs.Status.ToString(), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="datatype">Value can be R/P/S</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        public ActionResult ManageMasterData(string datatype)
        {
            ViewBag.DataTypeO = datatype;

            if (datatype == "R")
            {
                ViewBag.DataTypeM = "Roles";
                ViewBag.DataTypeS = "Role";
            }
            else if (datatype == "P")
            {
                ViewBag.DataTypeM = "Permissions";
                ViewBag.DataTypeS = "Permission";
            }
            else if (datatype == "S")
            {
                ViewBag.DataTypeM = "Systems";
                ViewBag.DataTypeS = "System";
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datatype">Values can be R/P/S</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult ManageMasterDataList(string datatype)
        {
            if (datatype == "R")
            {
                ViewBag.DataTypeM = "Roles";
                ViewBag.DataTypeS = "Role";
            }
            else if (datatype == "P")
            {
                ViewBag.DataTypeM = "Permissions";
                ViewBag.DataTypeS = "Permission";
            }
            else if (datatype == "S")
            {
                ViewBag.DataTypeM = "Systems";
                ViewBag.DataTypeS = "System";
            }

            List<EAMIMasterData> lstData = new List<EAMIMasterData>();


            string pDataType = datatype == "R" ? "ROLE" : (datatype == "P" ? "PERMISSION" : (datatype == "S" ? "SYSTEM" : ""));

            CommonStatusPayload<List<EAMIMasterData>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<EAMIMasterData>>>
                                       (svc => svc.GetAllEAMIMasterData(pDataType));

            if (cs.Payload != null && cs.Payload.Count() > 0)
                lstData.AddRange(cs.Payload.OrderByDescending(a => a.LastUpdateDate).ToList());

            List<EAMIMasterDataModel> lstSystemDataModel = _mapper.Map<List<EAMIMasterData>, List<EAMIMasterDataModel>>(lstData);

            return PartialView(lstSystemDataModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datatype">Values can be ROLE/PERMISSION/SYSTEM</param>
        /// <param name="dataTypeid"></param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        public JsonResult CheckIfMasterDataExists(string datatype, string dataTypecode)
        {
            bool isdatavalid = false;

            if (!string.IsNullOrEmpty(datatype) || !string.IsNullOrEmpty(dataTypecode))
            {
                CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> masterdata = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>>>
                                          (svc => svc.GetEAMIAuthorizationLookUps(datatype));

                if (masterdata.Status)
                {
                    if (masterdata.Payload.Where(a => a.Item1 == datatype.ToUpper() && a.Item2.Code.ToLower().Trim() == dataTypecode.ToLower().Trim()).FirstOrDefault() == null)
                        isdatavalid = true;
                }
                ErrorHandlerController.CheckFatalException(masterdata.Status, masterdata.IsFatal, masterdata.GetCombinedMessage());
            }

            return Json(isdatavalid.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datatype">Values can be ROLE/PERMISSION/SYSTEM</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [AjaxOnly]
        public ActionResult AddMasterData(string datatype)
        {
            EAMIMasterData model = new EAMIMasterData();

            model.AssociatedData = new List<EAMIAuthBase>();

            if (datatype == "ROLE")
            {
                CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>>>
                                          (svc => svc.GetEAMIAuthorizationLookUps("PERMISSION"));

                if (cs.Status)
                {
                    model.AssociatedData = new List<EAMIAuthBase>();
                    model.AssociatedData.AddRange(cs.Payload.Where(a => a.Item1 == "PERMISSION").Select(a => a.Item2).ToList());
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }


            model.ID = null;
            model.DataType = datatype;


            ViewBag.DataTypeO = !string.IsNullOrEmpty(datatype) && datatype.Length > 1 ? datatype.Substring(0, 1) : "";

            if (datatype == "ROLE")
            {
                ViewBag.DataTypeM = "Roles";
                ViewBag.DataTypeS = "Role";
            }
            else if (datatype == "PERMISSION")
            {
                ViewBag.DataTypeM = "Permissions";
                ViewBag.DataTypeS = "Permission";
            }
            else if (datatype == "SYSTEM")
            {
                ViewBag.DataTypeM = "Systems";
                ViewBag.DataTypeS = "System";
            }

            return PartialView(model);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="datatype">Values can be ROLE/PERMISSION/SYSTEM</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddMasterData2(EAMIMasterData input)
        {
            string status = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                input.IsActive = true;
                input.ID = null;

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddUpdateEAMIMasterData(input, User.Identity.Name));

                if (cs.Status)
                {
                    status = "OK"; message = "";
                }
                else
                {
                    status = "ERROR"; message = cs.GetCombinedMessage();
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterDataID"></param>
        /// <param name="datatype">Values can be ROLE/PERMISSION/SYSTEM</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditSystemsInfo(long systemId)
        {
            string strSystemName = string.Empty;
            Common.CommonStatusPayload<List<EAMIMasterData>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<EAMIMasterData>>>
                                       (svc => svc.GetAllEAMIMasterData("SYSTEM"));
            if (cs.Payload != null && cs.Payload.Count() > 0)
            {
                strSystemName = cs.Payload.Where(s => s.ID == systemId).FirstOrDefault().Name;
            }
            ViewBag.SystemId = systemId;
            ViewBag.SystemName = strSystemName;
            return PartialView("../ManageSystems/index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterDataID"></param>
        /// <param name="datatype">Values can be ROLE/PERMISSION/SYSTEM</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult EditMasterData(long masterDataID, string datatype)
        {
            EAMIMasterData model = new EAMIMasterData();

            CommonStatusPayload<EAMIMasterData> cs2 = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<EAMIMasterData>>
                                      (svc => svc.GetEAMIMasterDataByID(datatype.ToUpper(), masterDataID));
            ErrorHandlerController.CheckFatalException(cs2.Status, cs2.IsFatal, cs2.GetCombinedMessage());

            if (cs2.Status)
            {
                model = cs2.Payload;
                model.OriginalCode = model.Code;

                if (model.AssociatedData == null)
                    model.AssociatedData = new List<EAMIAuthBase>();
            }

            if (datatype.ToUpper() == "ROLE")
            {
                CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>> cs1 = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<string, EAMIAuthBase>>>>
                                      (svc => svc.GetEAMIAuthorizationLookUps("PERMISSION"));
                ErrorHandlerController.CheckFatalException(cs1.Status, cs1.IsFatal, cs1.GetCombinedMessage());

                if (cs1.Status)
                {
                    List<EAMIAuthBase> lstOriginal = cs2.Payload.AssociatedData;

                    model.AssociatedData = new List<EAMIAuthBase>();

                    foreach (EAMIAuthBase perm in cs1.Payload.Where(a => a.Item1 == "PERMISSION").Select(a => a.Item2).ToList())
                    {
                        model.AssociatedData.Add(new EAMIAuthBase() { ID = perm.ID, Code = perm.Code, IsSelected = CheckMasterData(lstOriginal, perm.ID) });
                    }
                }
            }

            model.DataType = datatype.ToUpper();

            ViewBag.DataTypeO = !string.IsNullOrEmpty(model.DataType) && model.DataType.Length > 1 ? model.DataType.Substring(0, 1) : "";

            if (model.DataType == "ROLE")
            {
                ViewBag.DataTypeM = "Roles";
                ViewBag.DataTypeS = "Role";
            }
            else if (model.DataType == "PERMISSION")
            {
                ViewBag.DataTypeM = "Permissions";
                ViewBag.DataTypeS = "Permission";
            }
            else if (model.DataType == "SYSTEM")
            {
                ViewBag.DataTypeM = "Systems";
                ViewBag.DataTypeS = "System";
            }

          EAMIMasterDataModel systemDataModel = _mapper.Map<EAMIMasterData, EAMIMasterDataModel>(model);

            return PartialView(systemDataModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Values can be ROLE/PERMISSION/SYSTEM</param>
        /// <returns></returns>
        [AuthorizeResource(EAMIRole.ADMINISTRATOR)]
        [HttpPost]        
        public ActionResult EditMasterData2(EAMIMasterData input)
        {
            string status = string.Empty;
            string message = string.Empty;

           //EAMIMasterDataModel lstSystemDataModel = _mapper.Map<List<EAMIMasterData>, List<EAMIMasterDataModel>>(lstData);

            if (ModelState.IsValid)
            {
                input.DataType = input.DataType.ToUpper();

                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.AddUpdateEAMIMasterData(input, User.Identity.Name));

                if (cs.Status)
                {
                    status = "OK"; message = "";
                }
                else
                {
                    status = "ERROR"; message = cs.GetCombinedMessage();
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.SUPERVISOR)]
        [HttpGet]        
        public ActionResult YearlyDates()
        {
            return View();
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.SUPERVISOR)]
        [AjaxOnly]
        public JsonResult GetYearlyDates(int activeYear)
        {
            List<Tuple<EAMIDateType, string, DateTime>> lst = new List<Tuple<EAMIDateType, string, DateTime>>();

            CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>>>
                                          (svc => svc.GetYearlyCalendarEntries(activeYear, User.Identity.Name));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            if (cs.Status && cs.Payload != null && cs.Payload.Count() > 0)
                lst = cs.Payload;

            //var duplicateDatesList = lst.GroupBy(x => x.Item3).Where(g => g.Count() > 1).SelectMany(group => group).ToList().Select(x => x.Item3);
            //foreach (var tuple in lst.Select((value, i) => new { i, value }))
            //{
            //    if (duplicateDatesList.Contains(tuple.value.Item3))
            //    {
            //        ////using 2 for both P and D.
            //        lst[tuple.i] = new Tuple<EAMIDateType, string, DateTime>(EAMIDateType.PayAndDrawDate, tuple.value.Item2, tuple.value.Item3);

            //    }

            //}

            var tdates = lst.Select(a =>
                new
                {
                    //isPD = (a.Item1 == EAMIDateType.PayAndDrawDate ? true : false),
                    //type = (a.Item1 == EAMIDateType.PayDate || a.Item1 == EAMIDateType.PayAndDrawDate ? "P" : "D"),
                    type = (a.Item1 == EAMIDateType.PayDate ? "P" : "D"),
                    startMonth = a.Item3.Month,
                    startDay = a.Item3.Day,
                    endMonth = a.Item3.Month,
                    endDay = a.Item3.Day,
                    note = a.Item2
                }
            );

            return Json(tdates, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.SUPERVISOR)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult SaveYearlyDateAssignment(string currentDate, string payDateActionType, string drawDateActionType, string payDateNote, string drawDateNote)
        {
            string status = string.Empty;
            string message = string.Empty;

            DateTime activeDate = DateTime.Parse(currentDate);
            List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>> lst = new List<Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>>();

            if (drawDateActionType == "A")
                lst.Add(new Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>(EAMIDateType.DrawDate, EAMICalendarAction.Add, drawDateNote, activeDate));

            if (drawDateActionType == "D")
                lst.Add(new Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>(EAMIDateType.DrawDate, EAMICalendarAction.Delete, drawDateNote, activeDate));

            if (drawDateActionType == "U")
                lst.Add(new Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>(EAMIDateType.DrawDate, EAMICalendarAction.Update, drawDateNote, activeDate));


            if (payDateActionType == "A")
                lst.Add(new Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>(EAMIDateType.PayDate, EAMICalendarAction.Add, payDateNote, activeDate));                

            if (payDateActionType == "D")
                lst.Add(new Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>(EAMIDateType.PayDate, EAMICalendarAction.Delete, payDateNote, activeDate));

            if (payDateActionType == "U")
                lst.Add(new Tuple<EAMIDateType, EAMICalendarAction, string, DateTime>(EAMIDateType.PayDate, EAMICalendarAction.Update, payDateNote, activeDate));

            if (lst.Count() > 0)
            {
                CommonStatus cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatus>
                                        (svc => svc.UpdateYearlyCalendarEntry(lst, User.Identity.Name));

                if (cs.Status)
                {
                    status = "OK"; message = "";
                }
                else
                {
                    status = "ERROR"; message = cs.GetCombinedMessage();
                }
                ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            }

            var jsonData = new
            {
                status = status,
                message = message
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
        public ActionResult Landing()
        {
            return View();
        }


        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
        [AjaxOnly]
        public ActionResult GetEAMICounts()
        {
            return Json(AdministrationQueries.GetEAMICounts());
        }


        [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
        [AjaxOnly]
        public ActionResult GetAssignees()
        {
            return Json(AdministrationQueries.GetAssignees(base.GetLoggedinUserID()));
        }
    }


}