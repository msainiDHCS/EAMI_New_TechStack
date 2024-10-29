using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using OfficeOpenXml.LoadFunctions.Params;
using OfficeOpenXml.Style;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Data;
using OHC.EAMI.WebUI.Filters;
using OHC.EAMI.WebUI.ViewModels;
using Rotativa.Core;
using Rotativa.MVC;

namespace OHC.EAMI.WebUI.Controllers
{
    [EAMIAuthentication]
    [AuthorizeResource(EAMIRole.ADMINISTRATOR, EAMIRole.PROCESSOR, EAMIRole.SUPERVISOR)]
    public class ReportsController : BaseController
    {
        private string RotativaExecutablePath 
        {
            get { return HostingEnvironment.MapPath("~/bin/Rotativa"); }
        }

        public ActionResult Reports()
        {
            ReportFilters filter = new ReportFilters();
            filter.Reports = new List<SelectListItem>();
            filter.Reports.Add(new SelectListItem() { Text = "EAMI Data Summary", Value = "EAMIDataSummaryReport" });
            filter.Reports.Add(new SelectListItem() { Text = "EAMI Payment Set Holds", Value = "EAMIPaymentRecordSetHolds" });
            filter.Reports.Add(new SelectListItem() { Text = "EAMI Returned Payment", Value = "EAMIReturnToSOR" });
            filter.Reports.Add(new SelectListItem() { Text = "EAMI Sent to SCO E-Claim Schedule", Value = "EAMIEClaimSchedule" });
            filter.Reports.Add(new SelectListItem() { Text = "EAMI STO", Value = "EAMISTOReport" });
            return View(filter);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetFaceSheet(int ecsID, int systemID = 1)
        {
            FSReportDetails fSReportDetails = ReportQueries.GetFaceSheet(ecsID, systemID);
            ViewBag.ExportFaceSheetPdfActionTriggered = false;
            return PartialView("_FaceSheetPartial", fSReportDetails);
        }

        [HttpGet]
        public ActionResult ExportFaceSheetPdf(int ecsID, string programId, int systemID = 1)
        {
            try
            {
                if (!string.IsNullOrEmpty(programId) && Session["ProgramChoiceId"].ToString() != programId)
                {
                    ViewBag.Message = "";
                    return PartialView("~/Views/ErrorHandler/Index.cshtml", ViewBag.Message);
                }
                FSReportDetails fSReportDetails = ReportQueries.GetFaceSheet(ecsID, systemID);
                var rotativaOptions = new DriverOptions()
                {
                    //CustomSwitches = CustomSwitch,
                    PageSize = Rotativa.Core.Options.Size.Letter,
                    PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Core.Options.Margins(12, 5, 12, 5),
                    //PageWidth = 210,
                    //PageHeight = 297
                    WkhtmltopdfPath = RotativaExecutablePath
                };

                ViewBag.ExportFaceSheetPdfActionTriggered = true;
                var retPdf = new PartialViewAsPdf("_FaceSheetPartial", fSReportDetails)
                {
                    FileName = "FaceSheet" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".pdf",
                    RotativaOptions = rotativaOptions
                };

                return retPdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetTransferLetter(int ecsID)
        {
            CSReportDetails cSReportDetails = ReportQueries.GetTransferLetter(ecsID, User.Identity.Name);
            ViewBag.ExportTransferLetterPdfActionTriggered = false;
            return PartialView("_TransferLetterPartial", cSReportDetails);
        }

        [HttpGet]
        public ActionResult ExportTransferLetterPdf(int ecsID, string programId, int systemID = 1)
        {
            try
            {
                if (!string.IsNullOrEmpty(programId) && Session["ProgramChoiceId"].ToString() != programId)
                {
                    ViewBag.Message = "";
                    return PartialView("~/Views/ErrorHandler/Index.cshtml", ViewBag.Message);
                }
                CSReportDetails cSReportDetails = ReportQueries.GetTransferLetter(ecsID, User.Identity.Name);
                string CustomSwitch = "--footer-line --footer-font-size \"8\" --footer-right \"Page [page] of [toPage]\"";

                var rotativaOptions = new DriverOptions()
                {
                    CustomSwitches = CustomSwitch,
                    PageSize = Rotativa.Core.Options.Size.Letter,
                    PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Core.Options.Margins(12, 5, 12, 5),
                    //PageWidth = 210,
                    //PageHeight = 297
                    WkhtmltopdfPath = RotativaExecutablePath
                };

                ViewBag.ExportTransferLetterPdfActionTriggered = true;
                var retPdf = new PartialViewAsPdf("_TransferLetterPartial", cSReportDetails)
                {
                    FileName = "TransferLetter" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".pdf",
                    RotativaOptions = rotativaOptions,
                };

                return retPdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetCashManagementSummary(DateTime payDate)
        {
            CMSumReportDetails cMSumReportDetails = ReportQueries.GetCashManagementSummary(payDate);
            return PartialView("_CashManagementSummaryPartial", cMSumReportDetails);
        }

        [HttpGet]
        public ActionResult ExportCashManagementSummaryPdf(DateTime payDate)
        {
            CMSumReportDetails cMSumReportDetails = ReportQueries.GetCashManagementSummary(payDate);
            string CustomSwitch = "--footer-line --footer-font-size \"8\" --footer-right \"Page [page] of [toPage]\"";

            var rotativaOptions = new DriverOptions()
            {
                CustomSwitches = CustomSwitch,
                PageSize = Rotativa.Core.Options.Size.Letter,
                PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Core.Options.Margins(12, 5, 12, 5),
                //PageWidth = 210,
                //PageHeight = 297
            };
            var retPdf = new PartialViewAsPdf("_CashManagementSummaryPartial", cMSumReportDetails)
            {
                FileName = "CashManagementSummary" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".pdf",
                RotativaOptions = rotativaOptions,
            };

            return retPdf;
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetEAMIPaymentRecordSetHolds()
        {
            PRSetHoldsReportDetails pRSetHoldsReportDetails = ReportQueries.GetEAMIPaymentRecordSetHolds();
            return PartialView("_EAMIPaymentRecordSetHoldsPartial", pRSetHoldsReportDetails);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetReturnToSORReportData(DateTime dateFrom, DateTime dateTo)
        {
            ReturnToSORReportDetails returnToSORReportDetails = ReportQueries.GetReturnToSORReportData(dateFrom, dateTo);
            return PartialView("_ReturnToSORPartial", returnToSORReportDetails);
        }
        
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetEClaimScheduleReportData(DateTime dateFrom, DateTime dateTo)
        {
            EClaimScheduleReportDetails eClaimScheduleReportDetails = ReportQueries.GetEClaimScheduleReportData(dateFrom, dateTo);
            return PartialView("_EClaimSchedulePartial", eClaimScheduleReportDetails);
        }

        /// <summary>
        /// This method fetches the data from database for a given date range and renders it on EAMI UI
        /// </summary>
        /// <param name="dateFrom">Start Date</param>
        /// <param name="dateTo">End Date</param>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetDataSummaryReport(DateTime dateFrom, DateTime dateTo)
        {
            IEnumerable<DataSummaryReportDetails> dataSummaryReportDetails = ReportQueries.GetReturnToDataSummaryReport(dateFrom, dateTo);
            ViewData["DataSummaryReport"] = dataSummaryReportDetails.ToList();
            return PartialView("_EAMIDataSummaryPartial", ViewData["DataSummaryReport"]);
        }

        /// <summary>
        /// This method is used to download the Data Summary report for the selected program.
        /// This is called on the click of "Download Report" button. 
        /// Specifically handles the heavy data without timeout issues. 
        /// The other Export functionalities from Preview on the screen freezes the browsers after 15 mb of data.
        /// This method will directly export the data to Excel.
        /// </summary>
        /// <param name="dateFrom">Start date</param>
        /// <param name="dateTo">End Date</param>
        /// <returns></returns>
        public ActionResult ExportToExcelDataSummary(DateTime dateFrom, DateTime dateTo, string programId)
        {
            try
            {
                if (!string.IsNullOrEmpty(programId) && Session["ProgramChoiceId"].ToString() != programId) {
                    ViewBag.Message = "";
                    return PartialView("~/Views/ErrorHandler/Index.cshtml", ViewBag.Message);
                }

                IEnumerable<DataSummaryReportDetails> dataSummaryReportDetails = ReportQueries.GetReturnToDataSummaryReport(dateFrom, dateTo);
                if (!dataSummaryReportDetails.Any())
                {
                    ViewBag.Message = "No data found for this date range.";
                    return PartialView("_EAMIDataSummaryPartial", ViewBag.Message);                   
                }
                string selectedProgram = string.Empty;
                if (Session["ProgramChoiceId"].ToString() == "2")
                {
                    selectedProgram = "Pharmacy";
                }
                else if (Session["ProgramChoiceId"].ToString() == "3")
                {
                    selectedProgram = "Dental";
                }
                else //if it's 4
                {
                    selectedProgram = "Managed Care";
                }
                //Using EPPlus ExcelPackage
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage workbook = new ExcelPackage();

                using (ExcelPackage excel = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("DataSummaryReportDetails");
                    string dateformat = "mm/dd/yyyy";

                    worksheet.Cells["A1:S1"].Merge = true;
                    worksheet.Cells["A2:S2"].Merge = true;
                    worksheet.Cells["A2:S2"].Style.Font.Italic = true;
                    worksheet.Cells["A1:S1"].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Value = selectedProgram + " : EAMI Data Summary" + " -[ " + dateFrom.ToShortDateString() + "-" + dateTo.ToShortDateString() + " ]";
                    worksheet.Cells[2, 1].Value = "The information on this report is as of" + " " + DateTime.Now;

                    //Format the date columns
                    worksheet.Column(3).Style.Numberformat.Format = dateformat;
                    worksheet.Column(8).Style.Numberformat.Format = dateformat;

                    //worksheet.Cells[row, col].Value.ToString().Trim())
                    worksheet.Cells["A3:S3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A3:S3"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[3, 1].LoadFromCollection(dataSummaryReportDetails, c =>
                    {
                        c.PrintHeaders = true;
                        c.HeaderParsingType = HeaderParsingTypes.Preserve;
                        c.TableStyle = OfficeOpenXml.Table.TableStyles.Light1;
                    });
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    //Save the file in downloads directory
                    excel.Save();
                    string fileName = "EAMIDataSummary_Date_" + DateTime.Now + ".xlsx";
                    return File(excel.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetESTOReportData(DateTime payDate)
        {
            ESTOReportDetails eSTOReportDetails = ReportQueries.GetESTOReportData(payDate);
            return PartialView("_EAMISTOReportPartial", eSTOReportDetails);
        }

        [AjaxOnly]
        public JsonResult GetPayDates(int activeYear)
        {
            List<Tuple<EAMIDateType, string, DateTime>> lst = new List<Tuple<EAMIDateType, string, DateTime>>();
            WcfServiceInvoker payWcfService = new WcfServiceInvoker();

            CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>> cs = payWcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<List<Tuple<EAMIDateType, string, DateTime>>>>
                                          (svc => svc.GetYearlyCalendarEntries(activeYear, User.Identity.Name));
            ErrorHandlerController.CheckFatalException(cs.Status, cs.IsFatal, cs.GetCombinedMessage());

            if (cs.Status && cs.Payload != null && cs.Payload.Count() > 0)
                lst = cs.Payload;

            var tdates = lst.Select(a =>
                new
                {
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
    }
}