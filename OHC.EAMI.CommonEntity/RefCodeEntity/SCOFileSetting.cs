using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class SCOFileSetting : RefCode
    {

        [DataMember]
        public string SCOFilePropertyValue { get; set; }
        [DataMember]
        public string SCOFileEnvironment { get; set; }
        [DataMember]
        public string SCOPaymentType { get; set; }
        [DataMember]
        public int SCOFundID { get; set; }
        [DataMember]
        public int SCOSystemID { get; set; }
        [DataMember]
        public string SCOPropertyTypeName { get; set; }
        [DataMember]
        public string SCOPropertyTypeID { get; set; }
        [DataMember]
        public string SCOPropertyEnumID { get; set; }

        protected override void PopulateInstanceFromDataRowActual(System.Data.DataRow dr)
        {
            base.PopulateInstanceFromDataRowActual(dr);

            this.SCOFilePropertyValue = dr["SCO_File_Property_Value"].ToString();
            this.SCOFileEnvironment = dr["Environment"].ToString();
            this.SCOPaymentType = dr["Payment_Type"].ToString();
            this.SCOFundID = int.Parse(dr["Fund_ID"].ToString());
            this.SCOSystemID = int.Parse(dr["System_ID"].ToString());
            this.SCOPropertyTypeName = dr["SCO_Property_Type_Name"].ToString();
            this.SCOPropertyTypeID = dr["SCO_Property_Type_ID"].ToString();
            this.SCOPropertyEnumID = dr["SCO_Property_Enum_ID"].ToString();


        }
    }
}