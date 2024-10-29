using EAMI.CommonEntity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EAMI.WebApi.Models
{
    public class EAMIUserModel
    {
        public int UserID { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string OriginalUserName { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

        public string NewUserPassword { get; set; }
        
        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }

        [Display(Name = "User Type")]
        public int UserTypeID { get; set; }
        public List<SelectListItem> UserTypes { get; set; }
        public List<EAMIElement> UserRoles { get; set; }
        public List<EAMIElement> UserSystems { get; set; }
        public bool IsActive { get; set; }
    }
}