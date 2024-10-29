using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Common
{
    public static class EAMIDBConnection
    {
        private const string _eamiDBContext_default = "EAMIData";
        private static string _eamiDBContext = string.Empty;
        public static string EAMIDBContext
        {
            get
            {
                return string.IsNullOrEmpty(_eamiDBContext) ? _eamiDBContext_default : _eamiDBContext;
            }
            set { _eamiDBContext = value;}
        }

        public static string GetConnectionString()
        {
            string connString = string.Empty;
            try
            {
                connString = ConfigurationManager.ConnectionStrings[_eamiDBContext].ConnectionString;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return connString;
        }
    }
}
