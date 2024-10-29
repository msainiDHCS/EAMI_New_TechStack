using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using System.Data.SqlClient;
using System.Configuration;

namespace OHC.EAMI.DataAccess
{
    public class DataAccessBase : IDisposable
    {
        private readonly string _DBconnString = EAMIDBConnection.GetConnectionString();

        public Database contextDb = null;

        public DataAccessBase()
        {
            contextDb = new SqlDatabase(_DBconnString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DataAccessBase()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources  
                if (contextDb != null)
                {
                    contextDb = null;
                }
            }
        }
    }
}
