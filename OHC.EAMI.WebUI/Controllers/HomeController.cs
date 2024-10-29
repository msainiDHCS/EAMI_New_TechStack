using OHC.EAMI.Common.ServiceInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Models;
using OHC.EAMI.WebUI.Filters;
using System.Security.Claims;
using System.Security.Principal;
using OHC.EAMI.Common;

namespace OHC.EAMI.WebUI.Controllers
{
    //  [EAMIAuthenticationAttribute]
    //[Authorize]

    public class HomeController : BaseController
    {
        private readonly WcfServiceInvoker _wcfService;

        public HomeController()
        {
            _wcfService = new WcfServiceInvoker();
        }



        // GET: Home
        public ActionResult Index()
        {

            //var a = HttpContext.User.Identity as ClaimsIdentity; 
            //var x = a.FindFirst(ClaimTypes.GivenName).Value; ;// a.Claims.Where(b => b.Type == "Name").FirstOrDefault().Value;

            try
            {
                List<EAMIElement> s = _wcfService.InvokeService<IEAMIWebUIDataService, List<EAMIElement>>
                                    (svc => svc.GetEAMIDataElements("a"), false);

                EAMIElement e = s.FirstOrDefault();

                ViewData["ServiceData"] = "ID: " + e.ID + Environment.NewLine + "" + e.Value;
            }
            catch (Exception ex)
            {
            }

            return View();
        }

        public ActionResult TestView()
        {
            return View(new EAMIElement());
        }

        public ActionResult SampleFormWithSubmit()
        {
            return View();
        }

        public ActionResult ViewCurrentEAMIUsers()
        {
            return View();
        }

        public ActionResult ChildGridExample()
        {
            List<ChildGridExampleFirst> lst = new List<ChildGridExampleFirst>();

            lst.Add(new ChildGridExampleFirst() { EmployeeID = 1, EmployeeName = "Peter", JoinDate = Convert.ToDateTime("01/10/1999"), Salary = 101 });
            lst.Add(new ChildGridExampleFirst() { EmployeeID = 2, EmployeeName = "John", JoinDate = Convert.ToDateTime("01/05/2007"), Salary = 110 });
            lst.Add(new ChildGridExampleFirst() { EmployeeID = 3, EmployeeName = "Eleana", JoinDate = Convert.ToDateTime("12/18/2012"), Salary = 110 });


            return View(lst);
        }

        //[AuthorizeResource(EAMIPermission.ASSIGN_INVOICE, EAMIPermission.SEND_TO_CALSTARS)]
        [AjaxOnly]
        public ActionResult ChildGridExampleSecond(int id)
        {
            List<ChildGridExampleSecond> lst = new List<ChildGridExampleSecond>();

            lst.Add(new ChildGridExampleSecond() { PatentID = 1, PaentName = "Sup Peter", ObtainDate = Convert.ToDateTime("01/10/2007"), Collaborators = "Jonathan, Satya, Xing" });
            lst.Add(new ChildGridExampleSecond() { PatentID = 2, PaentName = "Sup John", ObtainDate = Convert.ToDateTime("01/05/2008"), Collaborators = "Kipling, Yen, Mohan" });
            lst.Add(new ChildGridExampleSecond() { PatentID = 3, PaentName = "Sup Eleana", ObtainDate = Convert.ToDateTime("12/18/2012"), Collaborators = "Yusuf, Bollinger, Quang" });

            ViewBag.ID = id;

            return PartialView(lst);
        }

        public ActionResult ChildGridExampleSecondData(int id)
        {
            List<ChildGridExampleSecond> lst = new List<ChildGridExampleSecond>();

            lst.Add(new ChildGridExampleSecond() { PatentID = 1, PaentName = "Sup Peter", ObtainDate = Convert.ToDateTime("01/10/2007"), Collaborators = "Jonathan, Satya, Xing" });
            lst.Add(new ChildGridExampleSecond() { PatentID = 2, PaentName = "Sup John", ObtainDate = Convert.ToDateTime("01/05/2008"), Collaborators = "Kipling, Yen, Mohan" });
            lst.Add(new ChildGridExampleSecond() { PatentID = 3, PaentName = "Sup Eleana", ObtainDate = Convert.ToDateTime("12/18/2012"), Collaborators = "Yusuf, Bollinger, Quang" });

            return Json(new
            {
                aaData = lst.Select(x => new string[] { x.PatentID.ToString(), x.PaentName, x.ObtainDate.ToString(), x.Collaborators })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export()
        {
            ClaimScheduleResults csResult = new ClaimScheduleResults();
            List<ClaimScheduleRecordMaster> csMasterRecordList = new List<ClaimScheduleRecordMaster>();
            List<ClaimSchedule> csList = GetPendingClaimSchedules();

            if (csList != null && csList.Count > 0)
            {
                foreach (ClaimSchedule cs in csList)
                {
                    ClaimScheduleRecordMaster csMasterRecord = new ClaimScheduleRecordMaster();
                    csMasterRecord.CSPrimaryKeyId = cs.PrimaryKeyID;
                    csMasterRecord.UniqueNumber = cs.UniqueNumber;
                    csMasterRecord.PayeeName = cs.PayeeInfo.PEE_Name;
                    csMasterRecord.PayeeCode = cs.PayeeInfo.PEE.Code;
                    csMasterRecord.PayeeSuffix = cs.PayeeInfo.PEE_IdSfx;
                    csMasterRecord.PaymentType = cs.PaymentType;
                    csMasterRecord.ContractNumber = cs.ContractNumber;
                    csMasterRecord.FiscalYear = cs.FiscalYear;
                    csMasterRecord.TotalPaymentDollars = Double.Parse(cs.Amount.ToString());
                    csMasterRecord.GroupIdentifier = String.Join("|", cs.PaymentGroupList.Select(t => t.UniqueNumber).ToArray());
                    csMasterRecord.CurrentStatus = cs.CurrentStatus;

                    //Replace when bug is fixed for CS Entity IsIHSS starts populating correctly.
                    //if ((cs.PaymentGroupList != null && cs.PaymentGroupList.Count > 0) && (cs.PaymentGroupList.Where(t => t.IsIHSS == true).Count() > 0))
                    //    csMasterRecord.hasIHSS = true;

                    //if ((cs.PaymentGroupList != null && cs.PaymentGroupList.Count > 0) && (cs.PaymentGroupList.Where(t => t.IsSCHIP == true).Count() > 0))
                    //    csMasterRecord.hasSCHIP = true;

                    csMasterRecordList.Add(csMasterRecord);
                }
                csResult.csRecordMasterList = csMasterRecordList;
            }
            return View(csResult);
        }

        private List<ClaimSchedule> GetPendingClaimSchedules()
        {
            List<ClaimSchedule> pcsSearch = new List<ClaimSchedule>();
            CommonStatusPayload < List < ClaimSchedule >> cs = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<ClaimSchedule>>>
                                    (svc => svc.GetClaimSchedulesSubmettedForApproval());
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());
            return cs.Payload;
        }
    }
}