using System.Runtime.Serialization;
using System.Text;

namespace EAMI.Common
{
    [DataContract]
    /// <summary>
    /// this class defines a common status as a standard return type for a given action
    /// </summary>
    public class CommonStatus
    {

        // class Ctor's
        public CommonStatus(bool status)
        {
            Status = status;
            MessageDetailList = new List<string>();
        }
        
        public CommonStatus(bool status, bool isFatal)
        {
            Status = status;
            IsFatal = isFatal;
            MessageDetailList = new List<string>();
        }

        public CommonStatus(bool status, string msgDetail)
            : this(status)
        {
            if (!string.IsNullOrEmpty(msgDetail))
            {
                MessageDetailList.Add(msgDetail);
            }
        }

        public CommonStatus(bool status, bool isFatal, string msgDetail)
            : this(status, isFatal)
        {
            if (!string.IsNullOrEmpty(msgDetail))
            {
                MessageDetailList.Add(msgDetail);
            }
        }

        public CommonStatus(bool status, List<string> msgDetailList)
            : this(status)
        {
            if (msgDetailList != null) MessageDetailList = msgDetailList;
        }

        public CommonStatus(bool status, bool isFatal, List<string> msgDetailList)
            : this(status, isFatal)
        {
            if (msgDetailList != null) MessageDetailList = msgDetailList;
        }

        [DataMember]
        // public class properties get/set
        public bool Status { get; set; }
        
        [DataMember]
        // public class properties get/set
        public bool IsFatal { get; set; }

        [DataMember]
        public List<string> MessageDetailList { get; private set; }


        /// <summary>
        /// add message detail
        /// </summary>
        /// <param name="detail"></param>
        public void AddMessageDetail(string detail)
        {
            if (!string.IsNullOrEmpty(detail))
            {
                MessageDetailList.Add(detail);
            }
        }

        /// <summary>
        /// add/append message detail list
        /// </summary>
        /// <param name="msgDetails"></param>
        public void AddMessageDetails(List<string> msgDetails)
        {
            if (msgDetails != null && msgDetails.Count > 0)
            {
                foreach (string msgDet in msgDetails)
                {
                    MessageDetailList.Add(msgDet);
                }
            }
        }

        /// <summary>
        /// sets current instant status to the one provided, adds/appends message details to message existing list
        /// </summary>
        /// <param name="cStatus"></param>
        public void AddCommonStatus(CommonStatus cStatus)
        {
            if (cStatus != null)
            {
                // one importand detail here - if status is 'false'
                // we dont set it to 'true' with this method to avoid cases
                // when we execute multiple functions with return type CommonStatus
                // while trying to combine message details.
                Status = (Status && cStatus.Status);
                AddMessageDetails(cStatus.MessageDetailList);
            }
        }

        public string GetFirstDetailMessage()
        {
            return (HasDetails()) ? MessageDetailList[0] : string.Empty;
        }

        public string GetCombinedMessage()
        {
            return GetCombinedMessage("\r\n", true);
        }

        public string GetCombinedMessage(bool isNumbered)
        {
            return GetCombinedMessage("\r\n", isNumbered);
        }

        public string GetCombinedMessage(string separator, bool isNumbered)
        {
            string retValue = string.Empty;
            if (HasDetails())
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < MessageDetailList.Count; i++)
                {
                    // we only want to prepend separator on any following detail
                    string insert = (i != 0) ? separator : string.Empty;
                    string num = isNumbered ? string.Format("{0}. ", i + 1) : string.Empty;
                    sb.AppendFormat("{0}{1}{2}", insert, num, this.MessageDetailList[i]);
                }
                retValue = sb.ToString();
            }
            return retValue;
        }

        public bool HasDetails()
        {
            return MessageDetailList.Count > 0;
        }

    }
}
