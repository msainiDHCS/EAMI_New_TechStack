using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class FundingDetail
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public FundingDetail()
        {

        }

        #endregion

        #region | Public Properties  |

        [Display(Name = "Funding Detail ID")]
        public int FundingDetailID { get; set; }
        [Display(Name = "Funding Source Name")]
        public string FundingSourceName { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Fiscal Quarter")]
        public string FiscalQuarter { get; set; }
        [Display(Name = "Fiscal Month")]
        public string FiscalMonth { get; set; }
        [Display(Name = "Waiver Name")]
        public string WaiverName { get; set; }

        #region | Navagation Properties  |
       // public Invoice Invoice { get; set; }
        #endregion

        #endregion
    }
}