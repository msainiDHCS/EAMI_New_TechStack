using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.CommonEntity
{    
    [DataContract]
    public class KvpDefinition : RefCode 
    {
        public int SOR_ID { get; set; }
        public string DataType { get; set; }
        public int ValueLength { get; set; }
        public KvpOwnerEntity KvpOwner { get; set; }

        protected override void PopulateInstanceFromDataRowActual(DataRow dr)
        {
            base.PopulateInstanceFromDataRowActual(dr);
            this.SOR_ID = int.Parse(dr["SOR_ID"].ToString());
            this.DataType = dr["Kvp_Value_DataType"].ToString();
            this.ValueLength = int.Parse(dr["Kvp_Value_Length"].ToString());
            this.KvpOwner = EnumUtil.ToEnum<KvpOwnerEntity>(dr["OwnerEntity"].ToString());
        }

       
    }
}
