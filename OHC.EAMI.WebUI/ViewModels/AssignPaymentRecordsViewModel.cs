using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.ViewModels
{

    public class AssignPaymentRecordsViewModel
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public AssignPaymentRecordsViewModel()
        {

        }

        #endregion

        #region | Public Properties  |
        [Display(Name = "HasItemsOnHold")]
        public bool HasItemsOnHold { get; set; }
        [Display(Name = "HasExclusivePaymentType")]
        public bool HasExclusivePaymentType { get; set; }
        [Display(Name = "Payee")]
        public string PayeeName { get; set; }
        [Display(Name = "PayeeFullCode")]
        public string PayeeFullCode { get; set; }
        [Display(Name = "PaymentSuperGroup UniqueKey")]
        public string PaymentSuperGroup_UniqueKey { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentTypeName { get; set; }
        [Display(Name = "Contract #")]
        public string ContractNumber { get; set; }
        [Display(Name = "SFY")]
        public string SFY { get; set; }
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        #region | Navagation Properties  |
        public IQueryable<PaymentGroup> PaymentGroups { get; set; }
        #endregion

        #endregion
    }
}
