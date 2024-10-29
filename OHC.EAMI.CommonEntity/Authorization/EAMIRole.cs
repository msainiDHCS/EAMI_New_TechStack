using OHC.EAMI.CommonEntity.Base;
using System;
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
    public class EAMIRole : EAMIAuthBase
    {
        public const string ADMINISTRATOR = "ADMIN";
        public const string SUPERVISOR = "SUPERVISOR";
        public const string PROCESSOR = "PROCESSOR";

        [DataMember]
        public List<EAMIAuthBase> Role_Permissions { get; set; }
    }
}
