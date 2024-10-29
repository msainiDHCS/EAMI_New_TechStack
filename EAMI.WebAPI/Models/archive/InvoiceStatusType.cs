using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class InvoiceStatusType
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public InvoiceStatusType()
        {

        }

        #endregion

        #region | Public Properties  |

        [Display(Name = "Invoice Status Type ID")]
        public short InvoiceStatusTypeID { get; set; }
        [Display(Name = "Invoice Status")]
        public string Code { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
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
        public ICollection<InvoiceStatus> InvoiceStatuses { get; set; }
        #endregion

        #endregion
    }
}