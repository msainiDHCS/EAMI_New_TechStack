using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.SCO
{
    public class EamiClaimRemittanceAdviceRecord
    {
        public double AMOUNT { get; set; }
        public string RA_LINE { get; set; }
        public bool PRINT_AMOUNT { get; set; }
    }
}
