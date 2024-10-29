using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.CommonEntity
{
    public class SCOSettingList : RefCodeList
    {
        public SCOSettingList(DataTable dt) : base(dt)
        { }

        public SCOSetting GetSCOSettingByKeyAndType(string key, string type = "GENERAL_SINGLE_KVP")
        {
            SCOSetting returnDs = null;
            foreach (SCOSetting ds in this)
            {
                if (ds.Code == key && ds.IsActive == true)
                {
                    returnDs = ds;
                    break;
                }
            }
            return returnDs;
        }

    }
}
