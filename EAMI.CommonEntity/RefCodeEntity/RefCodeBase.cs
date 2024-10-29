using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;

namespace EAMI.CommonEntity
{
    [DataContract]
    public abstract class RefCodeBase
    {
        public virtual void PopulateInstanceFromDataRow(DataRow dr)
        {
            PopulateInstanceFromDataRowActual(dr);
        }

        protected abstract void PopulateInstanceFromDataRowActual(DataRow dr);
    }                      
}
