using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.ViewModels
{
    #region Report Models
    public class ReportFilters
    {
        public List<SelectListItem> Reports { get; set; }
    }

    public class FSReportDetails
    {
        public string Fund_Name { get; set; }
        public string Department_Name { get; set; }
        public string Fund_Number { get; set; }
        public string Agency_Number { get; set; }
        public string Stat_Year { get; set; }
        public string Reference_Item { get; set; }
        public string FFY { get; set; }
        public string Chapter { get; set; }
        public string Program { get; set; }
        public string Element { get; set; }
        public string Amount { get; set; }
        public string Warrant_Count { get; set; }
        public string Record_Count { get; set; }
        public string RPI_Amount { get; set; }
        public string RPI_Count { get; set; }
        public string ECS_Number { get; set; }
        public string ECS_File_Name { get; set; }
        public int Current_ECS_Status_Type_ID { get; set; }
        public string[][] CS_Table { get; set; }
        public int Payment_Method_Type_ID { get; set; }
    }

    public class CSReportDetails
    {
        public string Title_1 { get; set; }
        public string Title_2 { get; set; }
        public string Title_3 { get; set; }
        public string Fund_Name { get; set; }
        public string Fiscal_Year { get; set; }
        public string Month_Year { get; set; }
        public string Preparer { get; set; }
        public string ECS_Number { get; set; }
        public string Exclusive_Payment_Type_Code { get; set; }
        public string Exclusive_Payment_Type_Description { get; set; }
        public string Pay_Date { get; set; }
        public int Current_ECS_Status_Type_ID { get; set; }
        public List<Tuple<string,string,string>> listOfCategories { get; set; }
        public List<Tuple<string, string, decimal, decimal, decimal, string,string>> listOfFundingTuples { get; set; }
        public List<Tuple<string, string, decimal, decimal, decimal, string, string>> listOfFundingTotalsTuples { get; set; }
        public Tuple<string, decimal, decimal, decimal> fundingGrandTotalsTuple { get; set; }
    }

    public class CMSumReportDetails
    {
        public DateTime currentDate { get; set; }
        public List<Tuple<string, decimal, decimal, decimal, decimal, string>> listOfFundingTuples { get; set; }
        public List<Tuple<string, decimal, decimal, decimal, decimal,string>> listOfFundingTotalsTuples { get; set; }
        public List<Tuple<string, decimal, decimal, decimal>> listOfScheduleTuples { get; set; }
        public List<Tuple<string, decimal, decimal, decimal>> listOfScheduleTotalsTuples { get; set; }
    }

    public class PRSetHoldsReportDetails
    {
        public List<Tuple<string, string, string, string, string, string, string, Tuple<string, string, decimal>>> listOfPRSetTuples { get; set; }
    }

    public class ReturnToSORReportDetails
    {
        public List<Tuple<string, string, string, string, string, string, string, Tuple<string, string, string, decimal>>> listOfPRSetTuples { get; set; }
    }

    public class EClaimScheduleReportDetails
    {
        public List<Tuple<string, string, string, string, string, string, string, Tuple<string, string, decimal, string, string, string, decimal, Tuple<string, string, string, decimal>>>> listOfEClaimScheduleSetTuples { get; set; }
    }

    public class ESTOReportDetails
    {
        public List<Tuple<string, string, string, string, string, decimal, decimal, Tuple<decimal>>> listOfSTOReportTuples { get; set; }
    }

    public class DataSummaryReportDetails
    {
        public string Claim_Schedule_Number { get; set; }
        public string ECS_Number { get; set; }
        public DateTime CS_PayDate { get; set; }
        public string Entity_ID { get; set; }
        public string Entity_Name { get; set; }
        public string VendorName1 { get; set; }
        public string PaymentSet_Number { get; set; }
        public DateTime PaymentSet_Received_Date { get; set; }
        public string State_FiscalYear { get; set; }
        public string State_Service_Qtr { get; set; }
        public string IndexCode { get; set; }
        public string PCACode { get; set; }
        public string ObjectDetailCode { get; set; }        
        public string ServiceCategory { get; set; }
        public string X_Type { get; set; }
        public string Funding_Source_Name { get; set; }
        public decimal HCDFAmount { get; set; }
        public decimal FFPAmount { get; set; }
        public decimal SGFAmount { get; set; }
    }

    #endregion
}