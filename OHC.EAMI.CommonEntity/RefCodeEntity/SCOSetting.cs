using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class SCOSetting : RefCode
    {
        [DataMember]
        public string SCOSettingValue { get; set; }

        [DataMember]
        public int SCOSettingTypeID { get; set; }

        [DataMember]
        public string SCOSettingTypeName { get; set; }

        protected override void PopulateInstanceFromDataRowActual(DataRow dr)
        {
            base.PopulateInstanceFromDataRowActual(dr);
            this.SCOSettingValue = dr["SCO_Property_Value"].ToString();
            this.SCOSettingTypeID = int.Parse(dr["SCO_Property_Type_ID"].ToString());
            this.SCOSettingTypeName = dr["SCO_Property_Type_Name"].ToString();
        }

        public static T GetSCOSettingValue<T>(RefCodeTableList refCodeTableList, string key)
        {
            SCOSettingList dsl = refCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY) as SCOSettingList;
            string value = dsl.GetSCOSettingByKeyAndType(key).SCOSettingValue;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetSCOSettingValue<T>(RefCodeTableList refCodeTableList, string key, string type)
        {
            SCOSettingList dsl = refCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY) as SCOSettingList;
            string value = dsl.GetSCOSettingByKeyAndType(key, type).SCOSettingValue;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
