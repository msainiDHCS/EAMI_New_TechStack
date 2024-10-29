using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace OHC.EAMI.WebUI.Models
{
    public class UserProfileDataModel
    {
        #region | Constructor |
        /// <summary>
        /// Ctor
        /// </summary>
        public UserProfileDataModel()
        {

        }

        #endregion

        #region | Public Properties  |

        public long PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public long StatusID { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public long Organization { get; set; }
        public string OrganizationName { get; set; }
        public string Address { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string UserFullName
        {
            get
            {
                return (LastName == null ? (FirstName == null ? "" : FirstName.Trim()) : LastName.Trim() +
                    (FirstName == null ? "" : ", " + FirstName.Trim()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strUserName
        {
            get
            {
                if (UserName == null || UserName.Equals(string.Empty))
                    return string.Empty;
                else
                    return UserName.ToString();
            }
        }
        
        public string strAddress
        {
            get
            {
                return GetProperString(Address);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strPhone
        {
            get
            {
                if (string.IsNullOrEmpty(Phone))
                    return string.Empty;
                else
                    return Phone.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strFax
        {
            get
            {
                if (string.IsNullOrEmpty(Fax))
                    return string.Empty;
                else
                    return Fax.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string strEmail
        {
            get
            {
                if (EmailID == null)
                    return string.Empty;
                else
                    return EmailID.ToLower().ToString();
            }
        }
               

        /// <summary>
        /// 
        /// </summary>
        public string strOrganizationName
        {
            get
            {
                return string.IsNullOrEmpty(OrganizationName) ? string.Empty : GetProperString(OrganizationName.Replace('_', ' '));
            }
        }

        public string GetProperString(object val)
        {
            TextInfo txtInfo = new CultureInfo("en-US", false).TextInfo;
            return txtInfo.ToTitleCase((val ?? string.Empty).ToString().ToLower());
        }

        #endregion
    }
}