using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.Model
{
    public class AuthenticationModel
    {
        [Display(Name = "User Name")]
        public required string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
    }
}
