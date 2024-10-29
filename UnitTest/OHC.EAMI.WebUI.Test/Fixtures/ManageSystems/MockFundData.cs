using System;
using System.Collections.Generic;
using System.Windows.Documents;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Models;

namespace OHC.EAMI.WebUI.Test.Fixtures.ManageSystems
{
    public class MockFundData
    {
        public static List<EAMIFundModel> MockGetAllFunds()
        {
            List<EAMIFundModel> lstFunds = new List<EAMIFundModel>
            {
                new EAMIFundModel()
                {
                    Fund_ID = 9,
                    Fund_Name = "GENERAL FUND",
                    Fund_Code = "0001",                    
                    Fund_Description = "Description for 0001",
                    Stat_Year = "1998",
                    System_ID = 1,
                    System_Code = null,
                    IsActive = true,
                    CreatedBy = "msaini",
                    CreateDate = Convert.ToDateTime("2023-11-17 23:42:59.037"),
                    UpdatedBy = "msaini",
                    UpdateDate = Convert.ToDateTime("2023-12-26 15:19:30.803"),
                    DeactivatedBy = null,
                    DeactivatedDate = null
                },
                new EAMIFundModel()
                {
                    Fund_ID = 10,
                    Fund_Name = "Health Care Deposit Fund",
                    Fund_Code = "913",
                    Fund_Description = "Description for Health Care Deposit Fund",
                    Stat_Year = "1978",
                    System_ID = 1,
                    System_Code = null,
                    IsActive = true,
                    CreatedBy = "msaini",
                    CreateDate = Convert.ToDateTime("2023-11-18 00:46:11.450"),
                    UpdatedBy = "msaini",
                    UpdateDate = Convert.ToDateTime("2024-01-09 13:53:01.790"),
                    DeactivatedBy = null,
                    DeactivatedDate = null
                }
            };
            return lstFunds;
        }

        public static EAMIFundModel MockGetFund()
        {
            EAMIFundModel fund = new EAMIFundModel()
            {
                Fund_ID = 9,
                Fund_Name = "GENERAL FUND",
                Fund_Code = "0001",
                Fund_Description = "Description for 0001",
                Stat_Year = "1998",
                System_ID = 1,
                System_Code = null,
                IsActive = true,
                CreatedBy = "msaini",
                CreateDate = Convert.ToDateTime("2023-11-17 23:42:59.037"),
                UpdatedBy = "msaini",
                UpdateDate = Convert.ToDateTime("2023-12-26 15:19:30.803"),
                DeactivatedBy = null,
                DeactivatedDate = null
            };
           
            return fund;
        }
    }
}
