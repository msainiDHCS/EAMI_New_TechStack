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
    public class EAMIFundingSourceModel
    {
        public int? Funding_Source_ID { get; set; }
        //[Display(Name = "Funding Source Name")]
        //public string Name { get; set; }
        [Display(Name = "Funding Source Name")]
        public string Code { get; set; }
        public string Description { get; set; }
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

       // public List Titles { get; set; }
    }
}