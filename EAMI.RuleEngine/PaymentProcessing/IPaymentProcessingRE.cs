using EAMI.Common;
using EAMI.CommonEntity;

namespace EAMI.RuleEngine
{
    public interface IPaymentProcessingRE
    {
        public CommonStatusPayload<List<RefCodeList>> GetReferenceCodeList(params enRefTables[] refTableNames);
    }
}
