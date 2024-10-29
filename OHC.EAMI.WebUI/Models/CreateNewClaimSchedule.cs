using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class CreateNewClaimSchedule
    {

        [Display(Name = "Claim Schedule Name")]
        public string ClaimScheduleName { get; set; }

    }
}