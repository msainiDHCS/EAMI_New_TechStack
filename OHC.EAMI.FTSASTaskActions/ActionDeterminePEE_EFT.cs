using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using FTSAS.Integration;
using EAMI.EFTPreNote.ServiceContract;

using OHC.EAMI.CommonEntity;
using OHC.EAMI.DataAccess;
using System.ServiceModel.Channels;
using OHC.EAMI.Common;

namespace OHC.EAMI.FTSASTaskActions
{


    /* deprecated code     

    [TaskAction(ActionName = "ActionDeterminePEE_EFT", IsPhantom = false)]
    public class ActionDeterminePEE_EFT : TaskActionBase
    {
        public override TaskResult Execute()
        {
            return new TaskResult(true, string.Empty, enProductiveOutcome.NONE);

               
            TaskResult result = new TaskResult(true, string.Empty, enProductiveOutcome.NONE);
            bool atleastOneUpdate = false;
            try
            {
                string dataSourceKey = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                EAMIDBConnection.EAMIDBContext = dataSourceKey;

                string sysName = Context.GetExecutionArgTextValueByKey("SYS_NAME");
                int sysId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).GetRefCodeByCode(sysName).ID;
                string svcUri = Context.GetExecutionArgTextValueByKey("SVC_URI");
                bool isNetNamedPipeBinding = Context.GetExecutionArgValueByKey<bool>("NET-NAMED-PIPE-BINDING");

                // check and exit if no 'RECEIVED' payments are found for given SOR
                bool paymentsFound = PaymentDataDbMgr.CheckReceivedPayments(sysId);
                if(!paymentsFound)
                {
                    return result;
                }

                //// old implementation to identify received entities
                //List<RefCode> peeList = RefCodeDBMgr.GetRefCodeTableList(true).GetRefCodeListByTableName(enRefTables.TB_PAYMENT_EXCHANGE_ENTITY);
                //List<EFTEntityBankInfoRequest> request = new List<EFTEntityBankInfoRequest>();
                //foreach (PayeeEntity pee in peeList)
                //{
                //    EFTEntityBankInfoRequest req = new EFTEntityBankInfoRequest()
                //    {
                //        SysNameCode = sysName,
                //        EntityCode = pee.Code,
                //        EntityEIN = pee.EntityEIN
                //    };
                //    request.Add(req);
                //}


                int receivedStatusId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RECEIVED").ID;
                List<PaymentGroup> groupedPymtSetEntityList = PaymentDataDbMgr.GetPaymentGroupsByStatus(receivedStatusId).GroupBy(t => t.PayeeInfo.PEE.Code).Select(t2 => t2.First()).ToList();
                List<EFTEntityBankInfoRequest> request = new List<EFTEntityBankInfoRequest>();
                // here go through payment groups/sets with distinct vendor entities
                foreach (PaymentGroup pg in groupedPymtSetEntityList)
                {
                    EFTEntityBankInfoRequest req = new EFTEntityBankInfoRequest()
                    {
                        SysNameCode = sysName,
                        EntityCode = pg.PayeeInfo.PEE.Code,
                        EntityEIN = pg.PayeeInfo.PEE.EntityEIN
                    };
                    request.Add(req);                    
                }

                Binding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                EndpointAddress address = new EndpointAddress(svcUri);

                EftPrenoteSvcClient cli = new EftPrenoteSvcClient(binding, address);
                List<EFTEntityBankInfoResponse> response = cli.GetEftEntityBankInfoList(request);
                List<EFTEntityBankInfoResponse> filteredResponse = response.Where(t => t.Status == enEFTPrenoteStatus.RESULT_FOUND.ToString()).ToList();
                
                foreach (EFTEntityBankInfoResponse resp in filteredResponse)
                {
                    int peeId = groupedPymtSetEntityList.First(t => t.PayeeInfo.PEE.Code.ToUpper() == resp.EntityCode.ToUpper()).PayeeInfo.PEE.ID;
                    PaymentDataDbMgr.InsertEFTInfo(sysId, peeId, resp.FIRoutingNumber, resp.FIAccountType, resp.PrvAccountNo, DateTime.Parse(resp.DatePrenoted));
                    atleastOneUpdate = true;
                }

                result.Status = true;
                result.Outcome = atleastOneUpdate ? enProductiveOutcome.FULL : enProductiveOutcome.NONE;                
            }
            catch(Exception ex)
            {
                result.Status = false;                
                result.Outcome = atleastOneUpdate ? enProductiveOutcome.PARTIAL : enProductiveOutcome.NONE;
                result.Note = ex.Message + "; " + ex.StackTrace;
                EAMILogger.Instance.Error(ex);
            }
                       
            //throw new NotImplementedException();
            return result;
           
}
    }


     */
}
