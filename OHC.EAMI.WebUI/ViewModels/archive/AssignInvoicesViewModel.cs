using OHC.EAMI.WebUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.ViewModels
{

    public class AssignInvoicesViewModel
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public AssignInvoicesViewModel()
        {

        }

        #endregion

        #region | Public Properties  |

        // Top Search Panel********************************************************************************************
        //[Display(Name = "System")]
        //public string EAMISystem { get; set; }
        //[Display(Name = "Invoice Status")]
        //public string InvoiceStatus { get; set; }
        [Display(Name = "Vendor Display Id")]
        public string VendorDisplayId { get; set; }
        [Display(Name = "Payee")]
        public string EntityName { get; set; }
        [Display(Name = "Model Type")]
        public string ModelName { get; set; }
        // ************************************************************************************************************
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
        //public IQueryable<Invoice> Invoices { get; set; }
        public IQueryable<Invoice> Invoices { get; set; }

        #endregion
    }
}
