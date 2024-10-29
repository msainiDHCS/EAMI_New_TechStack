using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Models
{
    #region Payment Processing Screen Models 
    public class PaymentProcessingSearchResults
    {
        public List<PaymentProcessingRecordMaster> masterDataList { get; set; }
    }

    public class PaymentProcessingFilters
    {
        public List<SelectListItem> Systems { get; set; }
        public List<SelectListItem> PayeeNames { get; set; }
        public List<SelectListItem> PaymentTypes { get; set; }
        public List<SelectListItem> ContractNumbers { get; set; }
        public List<SelectListItem> ECSStatusTypes { get; set; }
        public List<SelectListItem> PayDates { get; set; }
    }

    public class PaymentProcessingRecordMaster
    {
        public string GroupIdentifier { get; set; }
        public string PayeeName { get; set; }
        public string PayeeCode { get; set; }
        public string PayeeSuffix { get; set; }
        public string PaymentType { get; set; }
        public string ContractNumber { get; set; }
        public string FiscalYear { get; set; }

        [DataType(DataType.Currency)]
        public double TotalPaymentDollars { get; set; }

        public string GroupName
        {
            get
            {
                var gname = (string.IsNullOrEmpty(PayeeName) ? "" : PayeeName.Trim());
                //gname = gname + " (" + PayeeCode + "-" + PayeeSuffix + ")";
                gname = gname + " (" + PayeeCode + ")";
                gname = gname + (string.IsNullOrEmpty(PaymentType) ? "" : (gname.Length > 0 ? " - " : "") + PaymentType.Trim());
                return gname;
            }
        }

        public bool hasHold { get; set; }
        [Display(Name = "HasExclusivePaymentType")]
        public bool HasExclusivePaymentType { get; set; }
        public bool hasReleaseFromSup { get; set; }
        //public bool hasIHSS { get; set; }
        //public bool hasSCHIP { get; set; }
        //public bool hasHQAF { get; set; }
        public PaymentProcessingRecordMaster()
        {
            hasHold = false;
            hasReleaseFromSup = false;
            //hasIHSS = false;
            //hasSCHIP = false;
            //hasHQAF = false;
        }
    }

    public class PaymentProcessingRecordChild
    {
        public string PaymentRecNumberSetNumber { get; set; }
        public DateTime AssignedPaymentDate { get; set; }
        public string AssignedUser { get; set; }

        [DataType(DataType.Currency)]
        public double PaymentSetTotalAmount { get; set; }
        public bool IsOnHold { get; set; }
        public bool IsReleaseFromSupervisor { get; set; }
        public string ExclusivePaymentType_Code { get; set; }

        public string Payment_Method_Type { get; set; }

        //public bool IsIHSS { get; set; }
        //public bool IsSCHIP { get; set; }
        //public bool IsHQAF { get; set; }
        //public string OnHold_HoldNote { get; set; }
        public string OnHold_StatusNote { get; set; }
        public string OnHold_CreatedBy { get; set; }
        public DateTime OnHold_StatusDate { get; set; }
        //public string ReleaseNote { get; set; }
        public string Denied_StatusNote { get; set; }
        public string Denied_CreatedBy { get; set; }
        public DateTime Denied_StatusDate { get; set; }
        public string PaymentSuperGroupKey { get; set; }
        //public string StatusNote { get; set; }
        public string Return_StatusNote { get; set; }
        public string Return_CreatedBy { get; set; }
        public DateTime Return_StatusDate { get; set; }
        public string Note { get; set; }
        public bool IsLinked { get; set; }
        public string LinkedSets { get; set; }
        public PaymentProcessingRecordChild()
        {
            IsOnHold = false;
            IsReleaseFromSupervisor = false;
            //IsIHSS = false;
            //IsSCHIP = false;
            //IsHQAF = false;
            IsLinked = false;
        }
    }

    public class PaymentProcessingRecord
    {
        public int PaymentRecID { get; set; }
        public string PaymentRecNumber { get; set; }
        public DateTime PaymentRecordDate { get; set; }
        public string PaymentSetNumber { get; set; }
        [DataType(DataType.Currency)]
        public double Amount { get; set; }
    }

    public class PaymentProcessingClaimScheduleMaster
    {
        public string PayeeName { get; set; }
        public string PaymentType { get; set; }
        public string ContractNumber { get; set; }
        public string FiscalYear { get; set; }
        public int ClaimScheduleID { get; set; }
        public string ClaimScheduleNumber { get; set; }
        public double TotalPaymentDollars { get; set; }
    }

    public class IPFundingDetails
    {
        public string SFY { get; set; }
        public string FFY { get; set; }
        public string ContractTerm { get; set; }
        public string ContractNumber { get; set; }
        public string Index { get; set; }
        public string Object { get; set; }
        public string PCACode { get; set; }
        public string VendorCode { get; set; }
        public List<string> WarningMessages { get; set; }
        public string PayeeName { get; set; }
        public string PayeeCode { get; set; }
        public string PayeeSuffix { get; set; }
        public string PaymentType { get; set; }
        public string GroupName
        {
            get
            {
                var gname = (string.IsNullOrEmpty(PayeeName) ? "" : PayeeName.Trim());               
                gname = gname + (string.IsNullOrEmpty(PaymentType) ? "" : (gname.Length > 0 ? " - " : "") + PaymentType.Trim());
                return gname;
            }
        }
        public List<IPFundingListGroup> FundingDataGroups { get; set; }
    }

    public class IPFundingListGroup
    {
        private int FundingListGroupIdentifier { get; set; }
        public string FundingListGroupIdentifierHeader { get; set; }
        public List<IPFundingDetail> FundingData { get; set; }
    }

    public class IPFundingDetail
    {
        public string FundingSource { get; set; }

        [DataType(DataType.Currency)]
        private double Families { get; set; }

        [DataType(DataType.Currency)]
        public double FFPAmount { get; set; }

        [DataType(DataType.Currency)]
        public double SGFAmount { get; set; }

        [DataType(DataType.Currency)]
        private double ExistingSPD { get; set; }

        [DataType(DataType.Currency)]
        private double SpecialProjectsSPD { get; set; }

        [DataType(DataType.Currency)]
        private double Duals { get; set; }

        [DataType(DataType.Currency)]
        private double NewAdultGroups { get; set; }

        [DataType(DataType.Currency)]
        private double FundingAmount { get; set; }

        [DataType(DataType.Currency)]
        public double TotalAmount { get; set; }

        public string FiscalYear { get; set; }
        public string FiscalQuarter { get; set; }
    }

    public class AddToClaimSchedule
    {
        public string ClaimScheduleID { get; set; }
        public List<SelectListItem> ClaimSchedules { get; set; }
    }

    #endregion

    #region Claim Schedule Screen Models

    public class ClaimScheduleResults
    {
        public List<ClaimScheduleRecordMaster> csRecordMasterList { get; set; }
    }

    public class ClaimScheduleRecordMaster
    {
        /* optional warrant info ***********************************************/
        public string WR_ECS_NUMBER { get; set; }
        public DateTime WR_ISSUE_DATE { get; set; }
        public string WR_WARRANT_NUMBER { get; set; }
        [DataType(DataType.Currency)]
        public decimal WR_WARRANT_AMOUNT { get; set; }
        public int WR_SEQ_NUMBER { get; set; }
        /***********************************************************************/
        public int CSPrimaryKeyId { get; set; }
        public string UniqueNumber { get; set; }
        public string ExclusivePaymentType_Code { get; set; }
        public string GroupIdentifier { get; set; }
        public string PayeeName { get; set; }
        public string PayeeCode { get; set; }
        public string PayeeSuffix { get; set; }
        public string PaymentType { get; set; }
        public string ContractNumber { get; set; }
        public string FiscalYear { get; set; }
        public EntityStatus CurrentStatus { get; set; }
        public DateTime PayDate { get; set; }
        public DateTime CurrentDate { get; set; }
        [DataType(DataType.Currency)]
        public double TotalPaymentDollars { get; set; }
        public string AssignedUser { get; set; }
        public bool IsLinked { get; set; }
        public string LinkedCSSets { get; set; }
        public bool hasNegativeFundingSource { get; set; }
        public RefCode PaymentMethodType { get; set; }
        public string GroupName
        {
            get
            {
                var gname = (string.IsNullOrEmpty(PayeeName) ? "" : PayeeName.Trim());
                gname = gname + " (" + PayeeCode + ")";
                gname = gname + (string.IsNullOrEmpty(PaymentType) ? "" : (gname.Length > 0 ? " - " : "") + PaymentType.Trim());
                return gname;                
            }
        }
        
        public bool hasReleaseFromSup { get; set; }
        //public bool hasIHSS { get; set; }
        //public bool hasSCHIP { get; set; }
        //public bool hasHQAF { get; set; }
        public ClaimScheduleRecordMaster()
        {            
            hasReleaseFromSup = false;            
            IsLinked = false;

            //hasIHSS = false;
            //hasSCHIP = false;
            //hasHQAF = false;
        }

    }

    public class RemittanceAdviceDetail
    {
        public string DepartmentName { get; set; }
        public string DepartmentAddress { get; set; }
        public string DepartmentAddressCSZ { get; set; }
        public string OrgranizationCode { get; set; }
        public string Agency_Inquiries_Phone_Number { get; set; }
    }

    public class RemittanceCSModel
    {
        public ClaimSchedule claimSchedule { get; set; }
        public RemittanceAdviceDetail remittanceAdviceDetail { get; set; }

        public RemittanceCSModel()
        {
            claimSchedule = new ClaimSchedule();
            remittanceAdviceDetail = new RemittanceAdviceDetail();
        }
    }
    #endregion

    #region EClaimSchedule Models
    public class ECSSearchResults
    {
        public List<EClaimScheduleMasterRecord> masterDataList { get; set; }
    }
    public class EClaimScheduleMasterRecord
    {
        public int EcsId { get; set; }
        public string EcsNumber { get; set; }
        [Display(Name = "HasExclusivePaymentType")]
        public bool HasExclusivePaymentType { get; set; }
        public DateTime TransferDate { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [DataType(DataType.Currency)]
        public decimal TotalPayment { get; set; }
        public bool canApproveOrDelete { get; set; }

        public bool canSenttoSCO{ get; set; }
        public EClaimScheduleMasterRecord()
        {
            canApproveOrDelete = true;
            
        }

        public RefCode PaymentMethodType { get; set; }
    }

    #endregion
}