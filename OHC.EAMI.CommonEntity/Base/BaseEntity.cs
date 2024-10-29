using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Globalization;

namespace OHC.EAMI.CommonEntity.Base
{
    [DataContract]
    public abstract class BaseEntity
    {
        public short Sort_Order { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string UpdatedBy { get; set; }

        [DataMember]
        public DateTime? UpdateDate { get; set; }

        public string GetProperString(object val)
        {
            TextInfo txtInfo = new CultureInfo("en-US", false).TextInfo;
            return txtInfo.ToTitleCase((val ?? string.Empty).ToString());
        }
    }
}
