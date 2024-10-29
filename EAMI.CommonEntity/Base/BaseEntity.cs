using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Globalization;

namespace EAMI.CommonEntity
{
    public abstract class BaseEntity
    {
        public short Sort_Order { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string GetProperString(object val)
        {
            TextInfo txtInfo = new CultureInfo("en-US", false).TextInfo;
            return txtInfo.ToTitleCase((val ?? string.Empty).ToString());
        }
    }
}
