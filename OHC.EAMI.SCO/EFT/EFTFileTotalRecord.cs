using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common.FixedLengthFileGenerator;

namespace OHC.EAMI.SCO
{
    internal class EFTFileTotalRecord : SCOFileTotalRecordBase
    {
        #region Constructors

        public EFTFileTotalRecord()
        {
            RECORD_ID = "99EOF";
        }

        #endregion
    }
}
