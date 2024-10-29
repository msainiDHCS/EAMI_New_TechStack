using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class EntityGroup<TParent, TChild>
    {
        public TParent ParentItem { get; set; }
        public List<TChild> ListItems { get; set; }
    }
}
