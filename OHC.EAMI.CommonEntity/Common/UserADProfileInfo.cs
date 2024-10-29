using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class UserADProfileInfo
    {
        #region | Public Properties  |

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int passExpiryDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime PasswordExpirationDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? LastLogOn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? LastBadPasswordAttempt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? LastPasswordSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int BadLogonCount { get; set; }

        #endregion
    }
}
