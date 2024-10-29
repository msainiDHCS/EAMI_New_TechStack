using System.Runtime.Serialization;

namespace EAMI.CommonEntity
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
