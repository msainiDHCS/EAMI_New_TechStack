using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class ModelType
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public ModelType()
        {

        }

        #endregion

        #region | Public Properties  |

        [Display(Name = "Model Type ID")]
        public short ModelTypeID { get; set; }
        [Display(Name = "Model Type")]
        public string ModelName { get; set; }

        #region | Navagation Properties  |
      //  public ICollection<Invoice> Invoices { get; set; }
        #endregion

        #endregion
    }
}