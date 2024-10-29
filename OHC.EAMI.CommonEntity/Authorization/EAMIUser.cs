using OHC.EAMI.CommonEntity.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class EAMIUser : BaseEntity//, IComparer<EAMIUser>
    {
        [DataMember]
        public int? User_ID { get; set; }

        [DataMember]
        public string User_Name { get; set; }

        [DataMember]
        public string Display_Name { get; set; }

        [DataMember]
        public string User_EmailAddr { get; set; }

        [DataMember]
        public string User_Password { get; set; }

        [DataMember]
        public string Domain_Name { get; set; }

        [DataMember]
        public EAMIAuthBase User_Type { get; set; }

        [DataMember]
        public List<EAMIAuthBase> User_Roles { get; set; }

        [DataMember]
        public List<EAMIAuthBase> Permissions { get; set; }

        [DataMember]
        public List<EAMIAuthBase> User_Systems { get; set; }

        public string DelimitedRoleNames
        {
            get
            {
                return string.Join(", ", User_Roles.Select(a => a.Name).ToArray());
            }
        }

        public string DelimitedSystemNames
        {
            get
            {
                return string.Join(", ", User_Systems.Select(a => a.Name).ToArray());
            }
        }

        public string Status
        {
            get
            {
                return (base.IsActive ? "Active" : "Inactive");
            }
        }

        public DateTime LastUpdateDate
        {
            get
            {

                return UpdateDate != null ? CreateDate : Convert.ToDateTime(UpdateDate);
            }
        }

        public int Compare(EAMIUser a, EAMIUser b)
        {
            if (a.LastUpdateDate > b.LastUpdateDate)
                return 1;
            else
                return 0;
        }
    }
}
