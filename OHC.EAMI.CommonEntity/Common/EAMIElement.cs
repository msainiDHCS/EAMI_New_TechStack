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
    public class EAMIElement
    {
        #region | Public Properties  |

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsSelected{ get; set; }


        #endregion
    }
}
