using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Models
{
    public class FacesheetModel
    {
        public int? Facesheet_ID { get; set; }

        public int? Fund_ID { get; set; }
        [Display(Name = "Fund Code")]
        public string Fund_Code { get; set; }

        [Display(Name = "Fund Name")]
        public string Fund_Name { get; set; }
        [Display(Name = "Chapter Value")]
        public string Chapter { get; set; }

        [Display(Name = "Reference Item")]
        public string Reference_Item { get; set; }

        public string Program { get; set; }

        public string Element { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
        public int System_ID { get; set; }
        [Display(Name = "System")]
        public string System_Code { get; set; }
        [Display(Name = "Agency Number")]
        public string Agency_Number { get; set; }
        [Display(Name = "Agency Name")]
        public string Agency_Name { get; set; }
        [Display(Name = "Stat Year")]
        public string Stat_Year { get; set; }
        [Display(Name = "Fiscal Year")]
        public string Fiscal_Year { get; set; }
        public List<SelectListItem> List_Funds { get; set; }
    }
}