
using EAMI.DataEngine;
using EAMI.Common;
using EAMI.CommonEntity;

namespace EAMI.RuleEngine
{
    public class PaymentProcessingRE : IPaymentProcessingRE
    {
        public CommonStatusPayload<List<RefCodeList>> GetReferenceCodeList(params enRefTables[] refTableNames)
        {
            try
            {
                List<RefCodeList> lst = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableNames(refTableNames);

                return new CommonStatusPayload<List<RefCodeList>>(lst, true, "");
            }
            catch (Exception ex)
            {
                //EAMILogger.Instance.Error(ex);
                return new CommonStatusPayload<List<RefCodeList>>(null, false, true, ex.Message);
            }
        }
    }
}
