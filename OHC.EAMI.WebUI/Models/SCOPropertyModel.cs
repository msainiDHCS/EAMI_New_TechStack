using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.CommonEntity.DataEntity;

namespace OHC.EAMI.WebUI.Models
{
    public class SCOPropertyModel
    {
        public int? System_ID { get; set; }

        public int? Fund_ID { get; set; }

        [Display(Name = "Fund Code")]
        public string Fund_Code { get; set; }

        [Display(Name = "System")]
        public string System_Code { get; set; }
        public int? SCO_Property_ID { get; set; }
        [Display(Name = "Property Name")]
        public string SCO_Property_Name { get; set; }
        [Display(Name = "Property Value")]
        public string SCO_Property_Value { get; set; }
    
        public int SCO_Property_Type_ID { get; set; }
        public int SCO_Property_Enum_ID { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }       
        public string Status { get; set; }
       
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }        
        public List<SCOPropertyTypeLookUpModel> lstSCOPropertyTypeLookUp = new List<SCOPropertyTypeLookUpModel>();
        public List<SCOPropertyEnumsLookUpModel> lstSCOPropertyEnumsLookUp = new List<SCOPropertyEnumsLookUpModel>();
        public List<EAMIFundModel> lstFunds = new List<EAMIFundModel>();

        //For dropdowns: - 

        [Display(Name = "Fund")]
        public List<SelectListItem> SelectedFund { get; set; }

        [Display(Name = "SCO Property Type")]
        public List<SelectListItem> SelectedSCOPropertyTypesLookUp { get; set; }

        [Display(Name = "SCO Property Name")]
        public List<SelectListItem> SelectedSCOPropertiesEnumsLookUp { get; set; }
        [Display(Name = "Payment Type")]
        public List<SelectListItem> SelectedPaymentType { get; set; }
        [Display(Name = "Environment")]
        public List<SelectListItem> SelectedEnvironment { get; set; }
        public int EnvironmentID { get; set; }
        public string EnvironmentText { get; set; }
        public int PaymentTypeID { get; set; }
        public string PaymentTypeText { get; set; }
    }
}