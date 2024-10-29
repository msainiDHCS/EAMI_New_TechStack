using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    [DataContract]
    public class RefCode : RefCodeBase
    {
        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public int SortValue { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime UpdateDate { get; set; }

        [DataMember]
        public string UpdatedBy { get; set; }

        protected override void PopulateInstanceFromDataRowActual(DataRow dr)
        {
            if (dr == null)
                throw new ArgumentNullException("datarow");

            this.TableName = dr["TABLE_NAME"].ToString();
            this.ID = int.Parse(dr["ID"].ToString());
            this.Code = dr["Code"].ToString();
            this.Description = dr["Description"].ToString();
            this.IsActive = bool.Parse(dr["IsActive"].ToString());
            this.SortValue = DBNull.Value.Equals(dr["Sort_Value"]) ?
                            0 :
                            int.Parse(dr["Sort_Value"].ToString());                            
            this.CreateDate = DBNull.Value.Equals(dr["CreateDate"]) ?
                            DateTime.MinValue :
                            DateTime.Parse(dr["CreateDate"].ToString());
            this.CreatedBy = dr["CreatedBy"].ToString();
            this.UpdateDate = DBNull.Value.Equals(dr["UpdateDate"]) ?
                            DateTime.MinValue :
                            DateTime.Parse(dr["UpdateDate"].ToString());
            this.UpdatedBy = dr["UpdatedBy"].ToString();
        }
    }
}
