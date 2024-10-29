using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Common
{
    [DataContract]
    /// <summary>
    /// this class extends CommonStatus type to include generic payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonStatusPayload<T> : CommonStatus
    {
        public CommonStatusPayload(T payload, bool status)
            : base(status)
        {
            Payload = payload;
        }

        public CommonStatusPayload(T payload, bool status, string msgDetail)
            : base(status, msgDetail)
        {
            Payload = payload;
        }

        public CommonStatusPayload(T payload, bool status, bool isFatal, string msgDetail)
            : base(status, isFatal, msgDetail)
        {
            Payload = payload;
        }

        public CommonStatusPayload(T payload, bool status, List<string> msgDetailList)
            : base(status, msgDetailList)
        {
            Payload = payload;
        }

        public CommonStatusPayload(T payload, bool status, bool isFatal, List<string> msgDetailList)
            : base(status, isFatal, msgDetailList)
        {
            Payload = payload;
        }

        [DataMember]
        public virtual T Payload { get; private set; }
    }
}
