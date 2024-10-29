using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.CommonEntity
{
    public class EAMIResponse
    {
        public EAMIResponse()
        {
            Initialize(false);
        }
        
        public EAMIResponse(bool status)
        {
            Initialize(status);
        }

        private void Initialize(bool status)
        {
            Response_Message = new List<string>();
            Response_Status = status;
        }

        public List<string> Response_Message { get; set; }
        public bool Response_Status { get; set; }
    }
}
