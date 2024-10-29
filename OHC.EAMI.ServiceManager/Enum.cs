using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.ServiceManager
{
    public static class CONST
    {
        public const string TRANSACTION_VERSION = "1.1";
        //public const decimal MAX_PAYMENTRECORD_AMOUNT = 99999999.99M;
        //public const int MAX_PAYMENTRECORDS_PER_TRANSACTION = 2000;
    }


    public enum SENDER_RECEIVER_ID
    {
        EAMI = 1,
        CAPMAN = 2,
        MEDICAL_RX = 3,
        MDSDFFS = 4

    }

    public enum TRANSACTION_STATUS
    {
        None = 0,
        Rejected = 1,
        Accepted = 2,
        AcceptedPartially = 3
    }

    public enum PAYMENT_RECORD_STATUS
    {
        None = 0,
        RejectedFD = 1,
        Accepted = 2,
    }
    
}
