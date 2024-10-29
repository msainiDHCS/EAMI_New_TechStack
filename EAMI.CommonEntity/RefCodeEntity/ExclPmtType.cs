using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.CommonEntity
{
    [DataContract]
    public class ExclPmtType : RefCode
    {
        [DataMember]
        public int System_ID { get; set; }
        [DataMember]
        public int Fund_ID { get; set; }



        protected override void PopulateInstanceFromDataRowActual(System.Data.DataRow dr)
        {
            base.PopulateInstanceFromDataRowActual(dr);

            this.System_ID = int.Parse(dr["System_ID"].ToString());
            this.Fund_ID = int.Parse(dr["Fund_ID"].ToString());

        }
    }
}
