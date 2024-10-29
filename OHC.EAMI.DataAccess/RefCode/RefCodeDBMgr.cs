using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using System.Data;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;

namespace OHC.EAMI.DataAccess
{
    public static class RefCodeDBMgr
    {
        public const string REF_CODE_TABLE_CACHE_KEY = "REFERENCE-TABLE-DATA";

        /// <summary>
        /// gets loaded RefCodeTableList entity from database
        /// </summary>
        /// <param name="refreshCache"></param>
        /// <returns></returns>       
        public static RefCodeTableList GetRefCodeTableList(bool refreshCache = false)
        {
            RefCodeTableList rctl = null;
            if (!refreshCache)
            {
                rctl = CacheManager<RefCodeTableList>.Get(REF_CODE_TABLE_CACHE_KEY);                
            }

            if (refreshCache || rctl == null)
            {
                DataSet ds = GetReferenceCodeTableDataSet();
                rctl = new RefCodeTableList(ds);
                CacheManager<RefCodeTableList>.Set(rctl, REF_CODE_TABLE_CACHE_KEY);
            }
            
            return rctl;
        }


        /// <summary>
        /// gets dataset of all ref code or lookup values
        /// </summary>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        private static DataSet GetReferenceCodeTableDataSet()
        {            
            // call sp to load fresh dataset
            Database db = new SqlDatabase(EAMIDBConnection.GetConnectionString());
            DataSet ds = null;
            try
            {
                using (DbConnection dbConnection = db.CreateConnection())
                {
                    dbConnection.Open();
                    ds = db.ExecuteDataSet(DbStoredProcs.spGetReferenceTableData);
                    foreach (DataTable dt in ds.Tables)
                    {
                        dt.TableName = dt.Rows[0]["TABLE_NAME"].ToString();
                    }
                }                
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading reference data", ex);
            }
        }        
    }
}
