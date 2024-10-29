using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.RuleEngine.Common
{
    public class IAuthorizeProfile
    {
        int ProgramId { get; set; }
        int UserId { get; set; }
        int UserTypeId { get; set; }
    }
}
