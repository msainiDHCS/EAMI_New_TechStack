using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OHC.EAMI.Util
{
    public static class Helper
    {
        #region | Methods |

        public static string GetConfigValue(string ConfigName)
        {
            string ConfigurationValue = "";
            try
            {
                ConfigurationValue = ConfigurationManager.AppSettings[ConfigName];
            }
            catch (Exception ex)
            { }
            finally
            {

            }
            return ConfigurationValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long GetLong(object val)
        {
            long conValue = default(long);

            if (val != null)
            {
                long.TryParse(val.ToString(), out conValue);
            }

            return conValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetString(object val)
        {
            string conValue = default(string);

            if (val != null)
            {
                conValue = val.ToString();
            }

            return conValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetStringProperCase(object val)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((val ?? string.Empty).ToString().ToLower());
        }

        #endregion

    }
}
