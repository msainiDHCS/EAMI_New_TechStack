using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class PaymentExchangeEntity
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public PaymentExchangeEntity()
        {

        }

        #endregion

        #region | Public Properties  |
        [Display(Name = "PaymentExchangeEntityID")]
        public int PaymentExchangeEntityID { get; set; }
        [Display(Name = "Payee Code")]
        public string EntityID { get; set; }
        [Display(Name = "EntityIDType")]
        public string EntityIDType { get; set; }
        [Display(Name = "Payee")]
        public string EntityName { get; set; }
        [Display(Name = "EntityNameSuffix")]
        public string EntityNameSuffix { get; set; }
        [Display(Name = "AddressLine")]
        public string AddressLineCSZ { get; set; }
        [Display(Name = "Is Payee?")]
        public bool IsPayee { get; set; }
        [Display(Name = "Is Payer?")]
        public bool IsPayer { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Sort Value")]
        public byte SortValue { get; set; }
        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        #region | Navagation Properties  |
        //public ICollection<Invoice> Invoices { get; set; }
        #endregion

        #endregion
    }
}