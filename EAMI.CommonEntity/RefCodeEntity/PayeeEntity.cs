using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.CommonEntity
{
    [DataContract]
    public class PayeeEntity : RefCode
    {
        //[DataMember]
        //public int SOR_ID { get; set; }
        //[DataMember]
        //public string SOR_CODE { get; set; }

        [DataMember]
        public string EntityIDType { get; set; }
        [DataMember]
        public string EntityName { get; set; }
        [DataMember]
        public string EntityEIN { get; set; }
        //[DataMember]
        //public List<string> SystemCodeList { get; set; }

        protected override void PopulateInstanceFromDataRowActual(System.Data.DataRow dr)
        {
            base.PopulateInstanceFromDataRowActual(dr);
            //this.SOR_ID = int.Parse(dr["SOR_ID"].ToString());
            //this.SOR_CODE = dr["SOR_CODE"].ToString();
            this.EntityIDType = dr["Entity_ID_Type"].ToString();
            this.EntityName = dr["Description"].ToString();
            this.EntityEIN = dr["Entity_EIN"].ToString();
            //this.SystemCodeList = new List<string>() { enClientSysCode.PHARM_RX.ToString() };
        }
    }
}
