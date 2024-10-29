using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class PaymentType
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public PaymentType()
        {

        }

        #endregion

        #region | Public Properties  |

        [Display(Name = "Payment Type ID")]
        public short PaymentTypeID { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentTypeName { get; set; }

        #region | Navagation Properties  |
        public ICollection<Invoice> Invoices { get; set; }
        #endregion

        #endregion
    }
}