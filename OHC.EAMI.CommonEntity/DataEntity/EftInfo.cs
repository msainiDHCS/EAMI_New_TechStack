using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class EftInfo
    {
        public int EFTInfoID { get; set; }
        public string FIRoutingNumber { get; set; }
        public string FIAccountType { get; set; }
        public string PrvAccountNo { get; set; }

        public DateTime DatePrenoted { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
