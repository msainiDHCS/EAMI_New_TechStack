using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using OHC.EAMI.Common;
using OHC.EAMI.Common.ServiceInvoke;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Controllers;
using OHC.EAMI.WebUI.ViewModels;

namespace OHC.EAMI.WebUI.Data
{
    public static class ReportQueries
    {
        public static FSReportDetails GetFaceSheet(int ecsID, int systemID)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            FSReportDetails fSReportDetails = new FSReportDetails();
            bool isProdEnv = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetFaceSheetDataByECSID(ecsID, systemID,isProdEnv));     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            fSReportDetails.Fund_Name = cspl.Payload.Tables[0].Rows[0]["Fund_Name"].ToString();
            fSReportDetails.Department_Name = cspl.Payload.Tables[0].Rows[0]["Department_Name"].ToString();
            fSReportDetails.Fund_Number = cspl.Payload.Tables[0].Rows[0]["Fund_Number"].ToString();
            fSReportDetails.Agency_Number = cspl.Payload.Tables[0].Rows[0]["Agency_Number"].ToString();
            fSReportDetails.Stat_Year = cspl.Payload.Tables[0].Rows[0]["Stat_Year"].ToString();
            fSReportDetails.Reference_Item = cspl.Payload.Tables[0].Rows[0]["Reference_Item"].ToString();
            fSReportDetails.FFY = cspl.Payload.Tables[0].Rows[0]["FFY"].ToString();
            fSReportDetails.Chapter = cspl.Payload.Tables[0].Rows[0]["Chapter"].ToString();
            fSReportDetails.Program = cspl.Payload.Tables[0].Rows[0]["Program"].ToString();
            fSReportDetails.Element = cspl.Payload.Tables[0].Rows[0]["Element"].ToString();
            fSReportDetails.Amount = cspl.Payload.Tables[0].Rows[0]["Amount"].ToString() == "" ? "" :
                Convert.ToDecimal(cspl.Payload.Tables[0].Rows[0]["Amount"]).ToString("C");
            fSReportDetails.Warrant_Count = cspl.Payload.Tables[0].Rows[0]["Warrant_Count"].ToString();
            fSReportDetails.Record_Count = cspl.Payload.Tables[0].Rows[0]["Record_Count"].ToString() == "0" ? "TBD" :
                cspl.Payload.Tables[0].Rows[0]["Record_Count"].ToString();
            fSReportDetails.RPI_Amount = cspl.Payload.Tables[0].Rows[0]["RPI_Amount"].ToString() == "" ? "" :
                Convert.ToDecimal(cspl.Payload.Tables[0].Rows[0]["RPI_Amount"]).ToString("C");
            fSReportDetails.RPI_Count = cspl.Payload.Tables[0].Rows[0]["RPI_Count"].ToString();
            fSReportDetails.ECS_Number = cspl.Payload.Tables[0].Rows[0]["ECS_Number"].ToString();
            fSReportDetails.ECS_File_Name = cspl.Payload.Tables[0].Rows[0]["ECS_File_Name"].ToString();
            fSReportDetails.Current_ECS_Status_Type_ID = Convert.ToInt32(cspl.Payload.Tables[0].Rows[0]["Current_ECS_Status_Type_ID"]);
            fSReportDetails.Payment_Method_Type_ID = Convert.ToInt32(cspl.Payload.Tables[0].Rows[0]["Payment_Method_Type_ID"]);
            fSReportDetails.CS_Table = new string[cspl.Payload.Tables[1].Rows.Count][];
            foreach (DataRow row in cspl.Payload.Tables[1].Rows)
            {
                int index = cspl.Payload.Tables[1].Rows.IndexOf(row);
                fSReportDetails.CS_Table[index] = new string[3];
                fSReportDetails.CS_Table[index][0] = cspl.Payload.Tables[1].Rows[index]["LINE NO."] is DBNull ? null :
                        cspl.Payload.Tables[1].Rows[index]["LINE NO."].ToString();
                fSReportDetails.CS_Table[index][1] = cspl.Payload.Tables[1].Rows[index]["PAYEE"].ToString();
                fSReportDetails.CS_Table[index][2] = cspl.Payload.Tables[1].Rows[index]["AMOUNT"].ToString() == "" ? "" :
                    Convert.ToDecimal(cspl.Payload.Tables[1].Rows[index]["AMOUNT"]).ToString("C");
            }
            return fSReportDetails;
        }

        public static CSReportDetails GetTransferLetter(int ecsID, string userName)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            CSReportDetails cSReportDetails = new CSReportDetails();

            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetTransferLetterDataByECSID(ecsID, userName));     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            cSReportDetails.Title_1 = cspl.Payload.Tables[0].Rows[0]["Title_1"].ToString();
            cSReportDetails.Title_2 = cspl.Payload.Tables[0].Rows[0]["Title_2"].ToString();
            cSReportDetails.Title_3 = cspl.Payload.Tables[0].Rows[0]["Title_3"].ToString();
            cSReportDetails.Fund_Name = cspl.Payload.Tables[0].Rows[0]["Fund_Name"].ToString();
            cSReportDetails.Fiscal_Year = cspl.Payload.Tables[0].Rows[0]["Fiscal_Year"].ToString();
            cSReportDetails.Month_Year = cspl.Payload.Tables[0].Rows[0]["Month_Year"].ToString();
            cSReportDetails.Preparer = cspl.Payload.Tables[0].Rows[0]["Preparer"].ToString();
            cSReportDetails.ECS_Number = cspl.Payload.Tables[0].Rows[0]["ECS_Number"].ToString();
            cSReportDetails.Exclusive_Payment_Type_Description = (cspl.Payload.Tables[0].Rows[0]["Exclusive_Payment_Type_Code"].ToString().ToUpper() != "NONE") 
                                                                ? cspl.Payload.Tables[0].Rows[0]["Exclusive_Payment_Type_Description"].ToString() 
                                                                : String.Empty;
            cSReportDetails.Pay_Date = cspl.Payload.Tables[0].Rows[0]["Pay_Date"].ToString();
            cSReportDetails.Current_ECS_Status_Type_ID = Convert.ToInt32(cspl.Payload.Tables[0].Rows[0]["Current_ECS_Status_Type_ID"]);
            cSReportDetails.listOfCategories = new List<Tuple<string,string,string>>();
            cSReportDetails.listOfFundingTuples = new List<Tuple<string, string, decimal, decimal, decimal,string,string>>();
            cSReportDetails.listOfFundingTotalsTuples = new List<Tuple<string, string, decimal, decimal, decimal,string,string>>();

            foreach (DataRow categoryRow in cspl.Payload.Tables[1].Rows)
            {
                cSReportDetails.listOfCategories.Add(new Tuple<string, string, string>(
                    categoryRow["Title"].ToString(),
                    categoryRow["FedFundCode"].ToString(),
                    categoryRow["StateFundCode"].ToString()
                ));

                foreach (DataRow row in cspl.Payload.Tables[2].Rows)
                {
                    //if ((categoryRow["Title"].ToString().Equals(row["Title"].ToString()))  && (categoryRow["FedFundCode"].ToString().Equals(row["FedFundCode"].ToString())) && (categoryRow["StateFundCode"].ToString().Equals(row["StateFundCode"].ToString())))                    
                    if ((categoryRow["Title"].ToString()==row["Title"].ToString()) && (categoryRow["FedFundCode"].ToString()==row["FedFundCode"].ToString()) && (categoryRow["StateFundCode"].ToString()==row["StateFundCode"].ToString()))
                    {
                        cSReportDetails.listOfFundingTuples.Add(new Tuple<string, string, decimal, decimal, decimal,string,string>(
                            row["Title"].ToString(),
                            row["Funding_Source_Name"].ToString(),
                            Convert.ToDecimal(row["TotalAmount"] is DBNull ? 0 : row["TotalAmount"]),
                            Convert.ToDecimal(row["FFPAmount"] is DBNull ? 0 : row["FFPAmount"]),
                            Convert.ToDecimal(row["SGFAmount"] is DBNull ? 0 : row["SGFAmount"]),
                            row["FedFundCode"].ToString(),
                            row["StateFundCode"].ToString()
                        ));
                    }
                }

                cSReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, string, decimal, decimal,decimal,string,string>(
                    categoryRow["Title"].ToString(),
                    " Total",                
                cSReportDetails.listOfFundingTuples.Where(t => ((categoryRow["Title"].ToString().Equals(t.Item1.ToString())) && (categoryRow["FedFundCode"].ToString().Equals(t.Item6.ToString())) && (categoryRow["StateFundCode"].ToString().Equals(t.Item7.ToString())))).Sum(c => c.Item3),
                cSReportDetails.listOfFundingTuples.Where(t => ((categoryRow["Title"].ToString().Equals(t.Item1.ToString())) && (categoryRow["FedFundCode"].ToString().Equals(t.Item6.ToString())) && (categoryRow["StateFundCode"].ToString().Equals(t.Item7.ToString())))).Sum(c => c.Item4),
                cSReportDetails.listOfFundingTuples.Where(t => ((categoryRow["Title"].ToString().Equals(t.Item1.ToString())) && (categoryRow["FedFundCode"].ToString().Equals(t.Item6.ToString())) && (categoryRow["StateFundCode"].ToString().Equals(t.Item7.ToString())))).Sum(c => c.Item5)
                , categoryRow["FedFundCode"].ToString(),               
                categoryRow["StateFundCode"].ToString()
                ));
            }

            cSReportDetails.fundingGrandTotalsTuple = new Tuple<string, decimal, decimal, decimal>(
                "Schedule Total",
                cSReportDetails.listOfFundingTotalsTuples.Sum(c => c.Item3),
                cSReportDetails.listOfFundingTotalsTuples.Sum(c => c.Item4),
                cSReportDetails.listOfFundingTotalsTuples.Sum(c => c.Item5)
            );

            return cSReportDetails;
        }

        public static CMSumReportDetails GetCashManagementSummary(DateTime payDate)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            CMSumReportDetails cMSumReportDetails = new CMSumReportDetails();
            cMSumReportDetails.currentDate = DateTime.Now;

            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetDrawSummaryReportData(payDate));     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            cMSumReportDetails.listOfFundingTuples = new List<Tuple<string, decimal, decimal, decimal, decimal, string>>();
            cMSumReportDetails.listOfFundingTotalsTuples = new List<Tuple<string, decimal, decimal, decimal, decimal, string>>();
            cMSumReportDetails.listOfScheduleTuples = new List<Tuple<string, decimal, decimal, decimal>>();
            cMSumReportDetails.listOfScheduleTotalsTuples = new List<Tuple<string, decimal, decimal, decimal>>();

            var listOfFundingTuples_StateSharePFA = new List<Tuple<string, decimal, decimal, decimal, decimal, string>>();
            var listOfFundingTuples_FederalShare = new List<Tuple<string, decimal, decimal, decimal, decimal, string>>();
            var listOfFundingTuples_FederalCash = new List<Tuple<string, decimal, decimal, decimal, decimal, string>>();
            var listOfFundingTuples_Other = new List<Tuple<string, decimal, decimal, decimal, decimal, string>>();

            foreach (DataRow row in cspl.Payload.Tables[0].Rows)
            {
                if (row["FundingCategory"].ToString() == "State Share PFA's")
                {
                    listOfFundingTuples_StateSharePFA.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                        row["Funding_Source_Name"].ToString(),
                        Convert.ToDecimal(row["DebitAmount"] is DBNull ? 0 : row["DebitAmount"]),
                        Convert.ToDecimal(row["CreditAmount"] is DBNull ? 0 : row["CreditAmount"]),
                        0,
                        0,
                        row["FundingCategory"].ToString()
                        ));
                }
                else if (row["FundingCategory"].ToString() == "Federal Share")
                {
                    listOfFundingTuples_FederalShare.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                        row["Funding_Source_Name"].ToString(),
                        Convert.ToDecimal(row["DebitAmount"] is DBNull ? 0 : row["DebitAmount"]),
                        Convert.ToDecimal(row["CreditAmount"] is DBNull ? 0 : row["CreditAmount"]),
                        0,
                        0,
                        row["FundingCategory"].ToString()
                        ));
                }
                else if (row["FundingCategory"].ToString() == "Federal Cash (Including Cash Receipts)**")
                {
                    listOfFundingTuples_FederalCash.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                        row["Funding_Source_Name"].ToString(),
                        Convert.ToDecimal(row["DebitAmount"] is DBNull ? 0 : row["DebitAmount"]),
                        Convert.ToDecimal(row["CreditAmount"] is DBNull ? 0 : row["CreditAmount"]),
                        0,
                        0,
                        row["FundingCategory"].ToString()
                        ));
                }
                else if (row["FundingCategory"].ToString() == "Other")
                {
                    listOfFundingTuples_Other.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                        row["Funding_Source_Name"].ToString(),
                        Convert.ToDecimal(row["DebitAmount"] is DBNull ? 0 : row["DebitAmount"]),
                        Convert.ToDecimal(row["CreditAmount"] is DBNull ? 0 : row["CreditAmount"]),
                        0,
                        0,
                        row["FundingCategory"].ToString()
                        ));
                }
            }

            cMSumReportDetails.listOfFundingTuples = listOfFundingTuples_Other.Concat(
                                                        listOfFundingTuples_FederalCash.Concat(
                                                        listOfFundingTuples_FederalShare.Concat(
                                                        listOfFundingTuples_StateSharePFA))).ToList();

            cMSumReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                null,
                0,
                0,
                listOfFundingTuples_StateSharePFA.Sum(c => c.Item2) + listOfFundingTuples_StateSharePFA.Sum(c => c.Item3),
                0,
                listOfFundingTuples_StateSharePFA.FirstOrDefault().Item6
            ));
            cMSumReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                null,
                0,
                0,
                listOfFundingTuples_FederalShare.Sum(c => c.Item2) + listOfFundingTuples_FederalShare.Sum(c => c.Item3),
                0,
                listOfFundingTuples_FederalShare.FirstOrDefault().Item6
            ));
            cMSumReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                null,
                0,
                0,
                listOfFundingTuples_FederalCash.Sum(c => c.Item2) + listOfFundingTuples_FederalCash.Sum(c => c.Item3),
                listOfFundingTuples_FederalCash.Sum(c => c.Item2) + listOfFundingTuples_FederalCash.Sum(c => c.Item3) +
                listOfFundingTuples_FederalShare.Sum(c => c.Item2) + listOfFundingTuples_FederalShare.Sum(c => c.Item3),
                listOfFundingTuples_FederalCash.FirstOrDefault().Item6
            ));
            cMSumReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                "Total Incoming to cover schedules",
                0,
                cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item3),
                0,
                0,
                "Total Incoming to cover schedules - before"
            ));
            cMSumReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                "Less Total Debits",
                cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item2),
                0,
                0,
                0,
                "Less Total Debits"
            ));
            cMSumReportDetails.listOfFundingTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal, decimal, string>(
                "Total Incoming to cover schedules",
                0,
                cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item2) + cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item3),
                0,
                0,
                "Total Incoming to cover schedules - after"
            ));

            foreach (DataRow row in cspl.Payload.Tables[1].Rows)
            {
                cMSumReportDetails.listOfScheduleTuples.Add(new Tuple<string, decimal, decimal, decimal>(
                    row["ECS_Number"].ToString(),
                    Convert.ToDecimal(row["Amount"] is DBNull ? 0 : row["Amount"]),
                    0,
                    0
                    ));
            }
            cMSumReportDetails.listOfScheduleTotalsTuples.Add(new Tuple<string, decimal, decimal, decimal>(
                null,
                cMSumReportDetails.listOfScheduleTuples.Sum(c => c.Item2),
                cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item2) + cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item3),
                cMSumReportDetails.listOfScheduleTuples.Sum(c => c.Item2) +
                cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item2) + cMSumReportDetails.listOfFundingTuples.Sum(c => c.Item3)
            ));

            return cMSumReportDetails;
        }

        public static PRSetHoldsReportDetails GetEAMIPaymentRecordSetHolds()
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            PRSetHoldsReportDetails pRSetHoldsReportDetails = new PRSetHoldsReportDetails();
            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetHoldReportData());     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            pRSetHoldsReportDetails.listOfPRSetTuples = new List<Tuple<string, string, string, string, string, string, string, Tuple<string, string, decimal>>>();

            foreach (DataRow row in cspl.Payload.Tables[0].Rows)
            {
                pRSetHoldsReportDetails.listOfPRSetTuples.Add(new Tuple<string, string, string, string, string, string, string, Tuple<string, string, decimal>>(
                    row["Hold_Date"] is DBNull ? string.Empty : Convert.ToDateTime(row["Hold_Date"]).ToString("MM'/'dd'/'yyyy"),
                    row["PaymentSet_Number"].ToString(),
                    row["Hold_Notes"].ToString(),
                    row["User"].ToString(),
                    row["Vendor_Name"].ToString(),
                    row["Model"].ToString(),
                    row["Vendor_Number"].ToString(),
                    new Tuple<string, string, decimal>(row["Contract_Number"].ToString(),
                                                        row["FiscalYear"].ToString(),
                                                        Convert.ToDecimal(row["Amount"] is DBNull ? 0 : row["Amount"]))
                ));
            }

            return pRSetHoldsReportDetails;
        }

        public static ReturnToSORReportDetails GetReturnToSORReportData(DateTime dateFrom, DateTime dateTo)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            ReturnToSORReportDetails returnToSORReportDetails = new ReturnToSORReportDetails();
            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetReturnToSORReportData(dateFrom, dateTo));     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            returnToSORReportDetails.listOfPRSetTuples = new List<Tuple<string, string, string, string, string, string, string, Tuple<string, string, string, decimal>>>();

            foreach (DataRow row in cspl.Payload.Tables[0].Rows)
            {
                returnToSORReportDetails.listOfPRSetTuples.Add(new Tuple<string, string, string, string, string, string, string, Tuple<string, string, string, decimal>>(
                    row["Date_Returned"] is DBNull ? string.Empty : Convert.ToDateTime(row["Date_Returned"]).ToString("MM'/'dd'/'yyyy"),
                    row["System"].ToString(),
                    row["PaymentSet_Number"].ToString(),
                    row["Returned_Notes"].ToString(),
                    row["User"].ToString(),
                    row["Vendor_Name"].ToString(),
                    row["Model"].ToString(),
                    new Tuple<string, string, string, decimal>(row["Vendor_Number"].ToString(),
                                                                row["Contract_Number"].ToString(),
                                                                row["FiscalYear"].ToString(),
                                                                Convert.ToDecimal(row["Amount"] is DBNull ? 0 : row["Amount"]))
                ));
            }

            return returnToSORReportDetails;
        }

        public static EClaimScheduleReportDetails GetEClaimScheduleReportData(DateTime dateFrom, DateTime dateTo)
        {
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> cspl;
            EClaimScheduleReportDetails eClaimScheduleReportDetails = new EClaimScheduleReportDetails();
            cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetECSReportData(dateFrom, dateTo));     //get from db via service
            ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

            eClaimScheduleReportDetails.listOfEClaimScheduleSetTuples = new List<Tuple<string, string, string, string, string, string, string, Tuple<string, string, decimal, string, string, string, decimal, Tuple<string, string, string, decimal>>>>();

            foreach (DataRow row in cspl.Payload.Tables[0].Rows)
            {
                eClaimScheduleReportDetails.listOfEClaimScheduleSetTuples.Add(new Tuple<string, string, string, string, string, string, string, Tuple<string, string, decimal, string, string, string, decimal, Tuple<string, string, string, decimal>>>(
                    row["ECS_Number"].ToString(),
                    row["ECS_Sent_Date"] is DBNull ? string.Empty : Convert.ToDateTime(row["ECS_Sent_Date"]).ToString("MM'/'dd'/'yyyy"),
                    row["ECS_Status"].ToString(),
                    row["ECS_Approver"].ToString(),
                    row["Entity_Name"].ToString(),
                    row["Payment_Type"].ToString(),
                    row["VendorCode"].ToString(),
                    new Tuple<string, string, decimal, string, string, string, decimal, Tuple<string, string, string, decimal>>(
                        row["ContractNumber"].ToString(),
                        row["Business_Indicator"].ToString(),
                        Convert.ToDecimal(row["ECS_Amount"] is DBNull ? 0 : row["ECS_Amount"]),
                        row["Claim_Schedule_Number"].ToString(),
                        row["CS_PayDate"] is DBNull ? string.Empty : Convert.ToDateTime(row["CS_PayDate"]).ToString("MM'/'dd'/'yyyy"),
                        row["FiscalYear"].ToString(),
                        Convert.ToDecimal(row["CS_Amount"] is DBNull ? 0 : row["CS_Amount"]),
                        new Tuple<string, string, string, decimal>(
                        row["CS_Approver"].ToString(),
                        row["WarrantDate"] is DBNull ? string.Empty : Convert.ToDateTime(row["WarrantDate"]).ToString("MM'/'dd'/'yyyy"),
                        row["WarrantNumber"].ToString(),
                        Convert.ToDecimal(row["WarrantAmount"] is DBNull ? 0 : row["WarrantAmount"]))
                    )
                ));
            }

            return eClaimScheduleReportDetails;
        }

        public static ESTOReportDetails GetESTOReportData(DateTime payDate)
        {
            ESTOReportDetails eSTOReport = new ESTOReportDetails();
            WcfServiceInvoker _wcfService = new WcfServiceInvoker();
            CommonStatusPayload<DataSet> dspl;
            dspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetESTOReport(payDate));     //get from db via service
            ErrorHandlerController.CheckFatalException(dspl.Status, dspl.IsFatal, dspl.GetCombinedMessage());

            eSTOReport.listOfSTOReportTuples = new List<Tuple<string, string, string, string, string, decimal, decimal, Tuple<decimal>>>();
            foreach (DataRow row in dspl.Payload.Tables[0].Rows)
            {
                eSTOReport.listOfSTOReportTuples.Add(new Tuple<string, string, string, string, string, decimal, decimal, Tuple<decimal>>(
                    row["Entity_Name"].ToString(),
                    row["Payment_Type"].ToString(),
                    row["Claim_Schedule_Number"].ToString(),
                    row["CS_PayDate"] is DBNull ? string.Empty : Convert.ToDateTime(row["CS_PayDate"]).ToString("MM'/'dd'/'yyyy"),
                    row["FiscalYear"].ToString(),
                    Convert.ToDecimal(row["CS_Amount"] is DBNull ? 0 : row["CS_Amount"]),
                    Convert.ToDecimal(row["CS_FFPAmount"] is DBNull ? 0 : row["CS_FFPAmount"]),
                    new Tuple<decimal>(Convert.ToDecimal(row["CS_SGFAmount"] is DBNull ? 0 : row["CS_SGFAmount"]))
                    ));
            }


            return eSTOReport;
        }

        /// <summary>
        /// Return the Data Summary report for the given date range
        /// </summary>
        /// <param name="dateFrom">start date</param>
        /// <param name="dateTo">end date</param>
        /// <returns></returns>
        public static IEnumerable<DataSummaryReportDetails> GetReturnToDataSummaryReport(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                WcfServiceInvoker _wcfService = new WcfServiceInvoker();
                CommonStatusPayload<DataSet> cspl;
                DataSummaryReportDetails returnDataSummaryReportDetails = new DataSummaryReportDetails();
                cspl = _wcfService.InvokeService<IEAMIWebUIDataService, CommonStatusPayload<DataSet>>(svc => svc.GetDataSummaryReportData(dateFrom, dateTo));
                ErrorHandlerController.CheckFatalException(cspl.Status, cspl.IsFatal, cspl.GetCombinedMessage());

                var lstDataSummaryReportDetails = cspl.Payload.Tables[0]
                    .AsEnumerable()
                    .Select(r => new DataSummaryReportDetails
                    {
                        Claim_Schedule_Number = r.Field<string>("Claim_Schedule_Number"),
                        ECS_Number = r.Field<string>("ECS_Number"),
                        CS_PayDate = r.Field<DateTime>("CS_PayDate"),
                        Entity_ID = r.Field<string>("Entity_ID"),
                        Entity_Name = r.Field<string>("Entity_Name"),
                        VendorName1 = r.Field<string>("VendorName1"),
                        PaymentSet_Number = r.Field<string>("PaymentSet_Number"),
                        PaymentSet_Received_Date = r.Field<DateTime>("PaymentSet_Received_Date"),
                        State_FiscalYear = r.Field<string>("State_FiscalYear"),
                        State_Service_Qtr = r.Field<string>("State_Service_Qtr"),
                        IndexCode = r.Field<string>("IndexCode"),
                        PCACode = r.Field<string>("PCACode"),
                        ObjectDetailCode = r.Field<string>("ObjectDetailCode"),
                        ServiceCategory = r.Field<string>("ServiceCategory"),
                        X_Type = r.Field<string>("X_Type"),
                        Funding_Source_Name = r.Field<string>("Funding_Source_Name"),
                        HCDFAmount = r.Field<decimal>("HCDFAmount"),
                        FFPAmount = r.Field<decimal>("FFPAmount"),
                        SGFAmount = r.Field<decimal>("SGFAmount")
                    });

                return lstDataSummaryReportDetails;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}