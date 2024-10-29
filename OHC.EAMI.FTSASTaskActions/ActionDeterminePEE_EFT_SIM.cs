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
    
    [TaskAction(ActionName = "ActionDeterminePEE_EFT_SIM", IsPhantom = false)]
    public class ActionDeterminePEE_EFT_SIM : TaskActionBase
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
                string peeCode = Context.GetExecutionArgTextValueByKey("PEE_CODE");
                string peeEin =  Context.GetExecutionArgTextValueByKey("PEE_EIN");
                string fiRoutingNbr = Context.GetExecutionArgTextValueByKey("FI_ROUTING_NBR");
                string fiAccountType = Context.GetExecutionArgTextValueByKey("FI_ACCOUNT_TYPE");
                string accountNbr = Context.GetExecutionArgTextValueByKey("ACCOUNT_NBR");
                string prenotedDate = Context.GetExecutionArgTextValueByKey("DATE_PRENOTED");


                // check and exit if no 'RECEIVED' payments are found for given SOR
                bool paymentsFound = PaymentDataDbMgr.CheckReceivedPayments(sysId);
                if(!paymentsFound)
                {
                    return result;
                }

                int receivedStatusId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_PAYMENT_STATUS_TYPE).GetRefCodeByCode("RECEIVED").ID;
                List<PaymentGroup> lpg = PaymentDataDbMgr.GetPaymentGroupsByStatus(receivedStatusId).GroupBy(t => t.PayeeInfo.PEE.Code).Select(t2 => t2.First()).ToList();           
                
                // here go through payment groups/sets with distinct vendor entities
                foreach(PaymentGroup pg in lpg)
                {
                    if (pg.PayeeInfo.PEE.EntityEIN == peeEin && pg.PayeeInfo.PEE.Code == peeCode)
                    {
                        PaymentDataDbMgr.InsertEFTInfo(sysId, pg.PayeeInfo.PEE.ID, fiRoutingNbr, fiAccountType, accountNbr, DateTime.Parse(prenotedDate));
                        atleastOneUpdate = true;
                    }
                }

                //List<RefCode> peeList = RefCodeDBMgr.GetRefCodeTableList(true).GetRefCodeListByTableName(enRefTables.TB_PAYMENT_EXCHANGE_ENTITY);
                //foreach (PayeeEntity pee in peeList)
                //{
                //    if (pee.Code == peeCode && pee.EntityEIN == peeEin)
                //    {
                //        PaymentDataDbMgr.InsertEFTInfo(sysId, pee.ID, fiRoutingNbr, fiAccountType, accountNbr, DateTime.Parse(prenotedDate));
                //        atleastOneUpdate = true;
                //    }
                //}

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
