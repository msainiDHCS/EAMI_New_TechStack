using EAMI.WebApi.Models;

namespace EAMI.Data
{
    public class ApprovalsQueries
    {
        public static RemittanceCSModel GetRemittanceAdviceDataByCSID(int csID, int systemID, out string strPEE_Name)
        {
            string innerStrPEE_Name;
            RemittanceCSModel remittanceCSModel = PaymentProcessingQueries.GetRemittanceAdviceDataByCSID(csID, systemID, out innerStrPEE_Name);
            strPEE_Name = innerStrPEE_Name;
            return remittanceCSModel;
        }
    }
}