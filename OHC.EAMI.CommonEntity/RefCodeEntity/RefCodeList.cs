using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    [CollectionDataContract]
    public class RefCodeList : List<RefCode>
    {
        public RefCodeList()
        { }

        public RefCodeList(DataTable dt) : base()
        {
            if (dt == null || dt.Rows.Count == 0)
                throw new ArgumentNullException("datatable");

            this.TableName = EnumUtil.ToEnum<enRefTables>(dt.TableName);

            foreach (DataRow dr in dt.Rows)
            {
                RefCode obj = GetRefTableObjInstanceByTableName(EnumUtil.ToEnum<enRefTables>(dt.TableName));
                obj.PopulateInstanceFromDataRow(dr);
                this.Add(obj);
            }
        }

        [DataMember]
        public enRefTables TableName { get; set; }


        public RefCode GetRefCodeByID(int id)
        {
            return (this.FirstOrDefault(item => item.ID == id));
        }

        public RefCode GetRefCodeByCode(string code, bool isActive = true)
        {
            return (this.FirstOrDefault(item => item.Code == code && item.IsActive == isActive));
        }

        //public RefCode GetActiveRefCodeByCode(string code, bool isActive)
        //{
        //    return (this.FirstOrDefault(item => item.Code == code && item.IsActive ==isActive));
        //}

        public T GetRefCodeByID<T>(int id)
        {
            return (T)Convert.ChangeType(GetRefCodeByID(id), typeof(T));
        }

        public T GetRefCodeByCode<T>(string code)
        {
            return (T)Convert.ChangeType(GetRefCodeByCode(code), typeof(T));
        }


        private static RefCode GetRefTableObjInstanceByTableName(enRefTables refTable)
        {
            // note: this works a bit like factory pattern where a specialized 
            // refcode instance is created based on refTable name;

            RefCode retValue = null;
            switch (refTable)
            {
                case enRefTables.TB_PAYMENT_EXCHANGE_ENTITY:
                    retValue = new PayeeEntity();
                    break;
                case enRefTables.TB_SOR_KVP_KEY:
                    retValue = new KvpDefinition();
                    break;
                case enRefTables.TB_SCO_PROPERTY:
                    retValue = new SCOSetting();
                    break;
                case enRefTables.TB_SCO_FILE_PROPERTY:
                    retValue = new SCOFileSetting();
                    break;
                case enRefTables.TB_System:
                    retValue = new SystemProperty();
                    break;
                case enRefTables.TB_USER:
                    retValue = new UserAcc();
                    break;
                case enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE:
                    retValue = new ExclPmtType();
                    break;
                default:
                    retValue = new RefCode();
                    break;
            }
            return retValue;
        }
    }
}
