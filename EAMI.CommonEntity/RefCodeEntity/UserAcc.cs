using System.Data;
using System.Runtime.Serialization;

namespace EAMI.CommonEntity
{
    [DataContract]
    public class UserAcc : RefCode
    {
        [DataMember]
        public string User_EmailAddr { get; set; }
        [DataMember]
        public string Domain_Name { get; set; }
        [DataMember]
        public string User_Type_Code { get; set; }
        [DataMember]
        public string User_Type_Name { get; set; }

        protected override void PopulateInstanceFromDataRowActual(DataRow dr)
        {
            base.PopulateInstanceFromDataRowActual(dr);
            this.User_EmailAddr = dr["User_EmailAddr"].ToString();
            this.Domain_Name = dr["Domain_Name"].ToString();
            this.User_Type_Code = dr["User_Type_Code"].ToString();
            this.User_Type_Name = dr["User_Type_Name"].ToString();
        }

    }
}
