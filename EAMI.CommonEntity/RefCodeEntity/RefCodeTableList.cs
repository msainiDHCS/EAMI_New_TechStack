using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.CommonEntity
{
    [CollectionDataContract]
    public class RefCodeTableList : List<RefCodeList>
    {
        public RefCodeTableList()
        { }

        public RefCodeTableList(DataSet ds) : base()
        {
            if (ds == null || ds.Tables.Count == 0)
                throw new ArgumentNullException("dataset");

            foreach (DataTable dt in ds.Tables)
            {
                RefCodeList rcl = GetRefTableListObjInstanceByTableName(dt);
                this.Add(rcl);
            }
        }

        public RefCodeList GetRefCodeListByTableName(enRefTables tableName)
        {
            return (this.FirstOrDefault(item => item.TableName == tableName));
        }

        public T GetRefCodeListByTableName<T>(enRefTables tableName)
        {
            return (T)Convert.ChangeType(GetRefCodeListByTableName(tableName), typeof(T));
        }

        public List<RefCodeList> GetRefCodeListByTableNames(params enRefTables[] tableNames)
        {
            return this.Where(item => tableNames.Contains(item.TableName)).ToList();
        }

        private RefCodeList GetRefTableListObjInstanceByTableName(DataTable dt)
        {
            // note: this works a bit like factory pattern where a specialized 
            // refcodelist instance is created according to table name 

            RefCodeList retValue = null;
            switch (dt.TableName)
            {
                case "TB_SCO_PROPERTY":
                    retValue = new SCOSettingList(dt);
                    break;
                default:
                    retValue = new RefCodeList(dt);
                    break;
            }
            return retValue;
        }
    }
}
