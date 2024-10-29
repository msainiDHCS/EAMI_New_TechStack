using OHC.EAMI.CommonEntity;
using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.ViewModels
{

    public class AssignPaymentRecordsViewModel_12_29_17
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public AssignPaymentRecordsViewModel_12_29_17()
        {

        }

        #endregion

        #region | Public Properties  |
        [Display(Name = "Payee Display Id")]
        public string PayeeDisplayId { get; set; }
        [Display(Name = "Payee")]
        public string PayeeName { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentTypeName { get; set; }
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
        public string PayeeRefCode { get; set; }

        #region | Navagation Properties  |
        public IQueryable<PaymentRec> PaymentRecords { get; set; }
        #endregion

        #endregion
    }
}
