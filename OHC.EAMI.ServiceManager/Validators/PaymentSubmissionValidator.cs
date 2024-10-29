using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.Common;
using OHC.EAMI.ServiceContract;

namespace OHC.EAMI.ServiceManager
{
    public class PaymentSubmissionValidator : EAMIServiceValidator<EAMIServiceValidationContext<PaymentSubmissionRequest>>
    {
        public PaymentSubmissionValidator()
            : base("PaymentSubmissionValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<PaymentSubmissionRequest> context)
        {
            CommonStatus status = new CommonStatus(true);

            // validate required fields
            base.ValidateRequiredFields<PaymentSubmissionRequest>(context.RequestEntity, status);

            // validate Transaction
            if (status.Status)
            {
                TransactionValidator tv = new TransactionValidator();
                CommonStatus tvs = tv.Execute(new EAMIServiceValidationContext<EAMITransaction>(context.DataContext, context.RequestEntity));
                status.AddCommonStatus(tvs);
            }

            /* Eugene - at this time we wont validate payer info comming into EAMI
            // validate Payer info
            if (status.Status)
            {
                PaymentExchangeEntityValidator peev = new PaymentExchangeEntityValidator(PaymentExchangeEntityValidator.PaymentExchangeEntityType.Payer);
                CommonStatus peevs = peev.Execute(new EAMIServiceValidationContext<PaymentExchangeEntity>(context.DataContext, context.RequestEntity.PayerInfo));
                status.AddCommonStatus(peevs);
            }*/

            // validate Payment Record List
            if (status.Status && (context.RequestEntity.PaymentRecordList == null || context.RequestEntity.PaymentRecordList.Count == 0))
            {
                status.Status = false;
                status.AddMessageDetail("No PaymentRecords found. At list one PaymentRecord is required for EAMI Payment Submission");                
            }
            else if(status.Status && context.RequestEntity.PaymentRecordList.Count > context.DataContext.MaxPaymentRecordsPerTransaction)
            {
                status.Status = false;
                status.AddMessageDetail("Exceeded maximum number of payment records per transaction. Maximum allowed is " + context.DataContext.MaxPaymentRecordsPerTransaction.ToString());
            }           

            // PaymentRecordCount
            int outPayRecordCount = 0;
            if (status.Status && !int.TryParse(context.RequestEntity.PaymentRecordCount, out outPayRecordCount))
            {
                status.Status = false;
                status.AddMessageDetail("Invalid 'PaymentRecordCount' field value");
            }
            else if(status.Status && int.Parse(context.RequestEntity.PaymentRecordCount) != context.RequestEntity.PaymentRecordList.Count)
            {
                status.Status = false;
                status.AddMessageDetail("'PaymentRecordCount' value does not match 'PaymentRecordList.Count'");
            }

            // PaymentRecordTotalAmount
            base.ValidateTextAsAmount(context.RequestEntity.PaymentRecordTotalAmount, "PaymentRecordTotalAmount", status);             

            // validate individual payment record
            if (status.Status)
            {
                PaymentRecordValidator prv = new PaymentRecordValidator(context.RequestEntity.PaymentRecordList, context.DataContext);
                CommonStatus prvs = null;
                Dictionary<string, PaymentSet> psList = new Dictionary<string, PaymentSet>();
                foreach (PaymentRecord pr in context.RequestEntity.PaymentRecordList)
                {                    
                    prvs = prv.Execute(new EAMIServiceValidationContext<PaymentRecord>(context.DataContext, pr));
                    pr.IsValid = prvs.Status;                                       
                    pr.ValidationMessage = pr.IsValid ? string.Empty : prvs.GetCombinedMessage();

                    // create psList
                    if (!psList.Keys.Contains(pr.PaymentSetNumber))
                    {
                        psList.Add(pr.PaymentSetNumber, new PaymentSet(pr.PaymentSetNumber));
                    }

                    // add pr into payment set
                    psList[pr.PaymentSetNumber].PaymentRecList.Add(pr);

                    // mark PS as invalid based on invalid pr
                    if (!pr.IsValid && !string.IsNullOrWhiteSpace(pr.PaymentSetNumber))
                    {
                        psList[pr.PaymentSetNumber].IsValid = false;
                    }                 
                }
                       
         
                // validate PSs and sync validation status between PSs and PRs 
                foreach (KeyValuePair<string, PaymentSet> kvp in psList)
                {
                    PaymentSetValidator pspsv = new PaymentSetValidator();
                    // step 1 : validate PS for distinct PR values
                    if (kvp.Value.IsValid)
                    {                                                
                        CommonStatus pgvStatus = pspsv.Execute(new EAMIServiceValidationContext<PaymentSet>(context.DataContext, kvp.Value));
                        kvp.Value.IsValid = pgvStatus.Status;
                        kvp.Value.ValidationMessage = pgvStatus.GetCombinedMessage();
                    }

                    // step 2: validate PS and make sure all PRs within PG have same valid status                    
                    if(!kvp.Value.IsValid)
                    {
                        string msg = "PaymentRecord is rejected because its PaymentSet is invalid or contains invalid payments.";
                        foreach(PaymentRecord pr in kvp.Value.PaymentRecList)
                        {                            
                            if(pr.IsValid)
                            {
                                pr.IsValid = false;                                
                                pr.ValidationMessage = msg + (string.IsNullOrWhiteSpace(kvp.Value.ValidationMessage) ? string.Empty : Environment.NewLine + kvp.Value.ValidationMessage);
                            }
                        }
                    }                    
                }

                // fail if no valid payment record is found
                if (ValidationDataContext.HasValidPaymentRecords(context.RequestEntity.PaymentRecordList) == false)
                {
                    status.Status = false;
                    status.AddMessageDetail("No valid payment records found");
                }                
            }           

            // validate PaymentRecordTotalAmount matches the sum of PaymentRecordList[*].Amount 
            if (status.Status &&
                decimal.Parse(context.RequestEntity.PaymentRecordTotalAmount) != ValidationDataContext.GetPaymentRecordListTotalAmount(context.RequestEntity.PaymentRecordList))
            {
                status.Status = false;
                status.AddMessageDetail("The 'PaymentRecordTotalAmount' value does not match the sum amount of all PaymentRecords");

                //because this is a request-transaction level failure - we must fail each nested record
                foreach (PaymentRecord pr in context.RequestEntity.PaymentRecordList)
                {
                    pr.IsValid = false;
                    pr.ValidationMessage = "Fail at request-transaction level. See response transaction status message.";
                }
            }
                        

            return status;
        }



        

    }
}
