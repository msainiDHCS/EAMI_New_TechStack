using System.ComponentModel.DataAnnotations;

namespace EAMI.WebApi.Models
{
    public class CreateNewClaimSchedule
    {

        [Display(Name = "Claim Schedule Name")]
        public string ClaimScheduleName { get; set; }

    }
}