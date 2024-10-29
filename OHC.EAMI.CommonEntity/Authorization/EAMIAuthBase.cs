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
