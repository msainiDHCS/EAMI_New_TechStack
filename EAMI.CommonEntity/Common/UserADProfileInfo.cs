using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.DataEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class UserADProfileInfo
    {
        #region | Public Properties  |

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int passExpiryDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime PasswordExpirationDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastLogOn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastBadPasswordAttempt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastPasswordSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BadLogonCount { get; set; }

        #endregion
    }
}
