using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class InvoiceStatus
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public InvoiceStatus()
        {

        }

        #endregion

        #region | Public Properties  |

        [Display(Name = "Invoice Status ID")]
        public int InvoiceStatusID { get; set; }
        [Display(Name = "Invoice ID")]
        public int InvoiceID { get; set; }
        [Display(Name = "Status Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime StatusDate { get; set; }


        //[Display(Name = "Audit User")]
        //public string AuditUser { get; set; }  and error bits?????????????????  Audit user in another table???????????
        [Display(Name = "Status Note")]
        public string StatusNote { get; set; }

        #region | Navagation Properties  |
        public Invoice invoice { get; set; }
        public InvoiceStatusType InvoiceStatusType { get; set; }
        #endregion

        #endregion
    }
}