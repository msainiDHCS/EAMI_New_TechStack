using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Xml.Linq;

namespace EAMI.DataEngine
{
    public class DataAccessBase : IDisposable
    {
        //public DbConnection contextDb { get; private set; }
        private const string _eamiDBContext_default = "EAMIData";
        private readonly string _DBconnString;
        public Database contextDb = null;

        public DataAccessBase(BaseMiddlewareServiceDE baseMiddlewareServiceDE)
        {
            _DBconnString = baseMiddlewareServiceDE.GetConnectionString();
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
