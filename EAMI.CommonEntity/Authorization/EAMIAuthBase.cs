using System.Runtime.Serialization;

namespace EAMI.CommonEntity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class EAMIAuthBase : BaseEntity
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsSelected { get; set; }
    }
}
