using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class SystemProperty : RefCode
    {
        [DataMember]
        public string DepartmentName { get; set; }

        [DataMember]
        public string OrganizationCode { get; set; }

        [DataMember]
        public string RADepartmentAddrLine { get; set; }

        [DataMember]
        public string RADepartmentAddrCSZ { get; set; }

        [DataMember]
        public string RAInquiryPhNo { get; set; }

        [DataMember]
        public string FEIN_Number { get; set; }

        [DataMember]
        public decimal MaxPmtRecAmt { get; set; }

        [DataMember]
        public int MaxPmtRecPerTran { get; set; }

        [DataMember]
        public int MaxFundingDtlPerPmtRec { get; set; }

        [DataMember]
        public bool TraceIncomingPmtData { get; set; }
       
        [DataMember]
        public bool ValidateFundingSource { get; set; }
        [DataMember]
        public string TitleTransferLetter { get; set; }


        //public static T GetSystemPropertyValue<T>(RefCodeTableList refCodeTableList, string key)
        //{
        //    SystemPropertyList dsl = refCodeTableList.GetRefCodeListByTableName(enRefTables.TB_System) as SystemPropertyList;
        //    string value = dsl.GetSCOSettingByKeyAndType(key).SCOSettingValue;
        //    return (T)Convert.ChangeType(value, typeof(T));
        //}

        //public static T GetSCOSettingValue<T>(RefCodeTableList refCodeTableList, string key, string type)
        //{
        //    SCOSettingList dsl = refCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SCO_PROPERTY) as SCOSettingList;
        //    string value = dsl.GetSCOSettingByKeyAndType(key, type).SCOSettingValue;
        //    return (T)Convert.ChangeType(value, typeof(T));
        //}


        protected override void PopulateInstanceFromDataRowActual(DataRow dr)
        {
           base.PopulateInstanceFromDataRowActual(dr);
            this.DepartmentName = dr["RA_DEPARTMENT_NAME"].ToString();
            this.OrganizationCode = dr["RA_ORGANIZATION_CODE"].ToString();
            this.RADepartmentAddrLine = dr["RA_DEPARTMENT_ADDR_LINE"].ToString();
            this.RADepartmentAddrCSZ = dr["RA_DEPARTMENT_ADDR_CSZ"].ToString();
            this.RAInquiryPhNo = dr["RA_INQUIRIES_PHONE_NUMBER"].ToString();
            this.FEIN_Number = dr["FEIN_Number"].ToString();
            this.MaxPmtRecAmt = decimal.Parse(dr["MAX_PYMT_REC_AMOUNT"].ToString());
            this.MaxPmtRecPerTran = int.Parse(dr["MAX_PYMT_REC_PER_TRAN"].ToString());
            this.MaxFundingDtlPerPmtRec = int.Parse(dr["MAX_FUNDING_DTL_PER_PYMT_REC"].ToString());
            this.TraceIncomingPmtData = bool.Parse(dr["TRACE_INCOMING_PAYMENT_DATA"].ToString());
            this.ValidateFundingSource = bool.Parse(dr["VALIDATE_FUNDING_SOURCE"].ToString());
            this.TitleTransferLetter = dr["TITLE_TRANSFER_LETTER"].ToString();



        }

       
    }
}
