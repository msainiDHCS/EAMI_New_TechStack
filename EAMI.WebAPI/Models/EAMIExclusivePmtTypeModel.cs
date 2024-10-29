using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EAMI.WebApi.Models
{
    public class EAMIExclusivePmtTypeModel
    {
        public int? Exclusive_Payment_Type_ID { get; set; }

        [Display(Name = "Exclusive Payment Type Name")]
        public string Exclusive_Payment_Type_Name { get; set; }

        [Display(Name = "Exclusive Payment Type Code")]
        public string Exclusive_Payment_Type_Code { get; set; }

        [Display(Name = "Exclusive Payment Type Description")]
        public string Exclusive_Payment_Type_Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string DeactivatedBy { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        

        public int Fund_ID { get; set; }

        [Display(Name = "Fund Selection")]
        public List<SelectListItem> Funds { get; set; }


        public int System_ID { get; set; }
        [Display(Name = "System")]
        public string System_Code { get; set; }
        [Display(Name = "Fund Code")]
        public string Fund_Code { get; set; }
    }
}