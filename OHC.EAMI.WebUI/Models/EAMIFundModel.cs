using OHC.EAMI.CommonEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace OHC.EAMI.WebUI.Models
{
    public class EAMIFundModel
    {
        public int? Fund_ID { get; set; }
        [Display(Name = "Fund Name")]
        public string Fund_Name { get; set; }
        [Display(Name = "Fund Code")]
        public string Fund_Code { get; set; }
        [Display(Name = "Fund Description")]
        public string Fund_Description { get; set; }
        [Display(Name = "Stat Year")]
        public string Stat_Year { get; set; }
        public int System_ID { get; set; }
        [Display(Name = "System")]
        public string System_Code { get; set; }
        public bool IsActive{ get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string DeactivatedBy { get; set; }
        public DateTime? DeactivatedDate { get; set; }
    }
}