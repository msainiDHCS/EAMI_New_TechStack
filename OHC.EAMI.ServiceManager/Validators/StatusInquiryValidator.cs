
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.Common;

namespace OHC.EAMI.ServiceManager
{
    public class StatusInquiryValidator : EAMIServiceValidator<EAMIServiceValidationContext<PaymentStatusInquiryRequest>>
    {
        public StatusInquiryValidator()
            : base("StatusInquiryValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<PaymentStatusInquiryRequest> context)
        {
            CommonStatus status = new CommonStatus(true);

            // validate required fields
            base.ValidateRequiredFields<PaymentStatusInquiryRequest>(context.RequestEntity, status);

            // validate Transaction
            if (status.Status)
            {
                TransactionValidator tv = new TransactionValidator();
                CommonStatus tvs = tv.Execute(new EAMIServiceValidationContext<EAMITransaction>(context.DataContext, context.RequestEntity));
                status.AddCommonStatus(tvs);
            }


            // validate Status Record List
            if (status.Status && (context.RequestEntity.PaymentRecordList == null || context.RequestEntity.PaymentRecordList.Count == 0))
            {
                status.Status = false;
                status.AddMessageDetail("No PaymentStatusRecords found. At list one PaymentStatusRecord is required for EAMI Payment Status Inquiry ");
            }
            else if (status.Status && context.RequestEntity.PaymentRecordList.Count > context.DataContext.MaxStatusRecordsPerTtransaction)
            {
                status.Status = false;
                status.AddMessageDetail("Exceeded maximum number of payment records per transaction. Maximum allowed is " + 
                                            context.DataContext.MaxStatusRecordsPerTtransaction.ToString());
            }

            // status record Count
            int outPayRecordCount = 0;
            if (status.Status && !int.TryParse(context.RequestEntity.PaymentRecordCount, out outPayRecordCount))
            {
                status.Status = false;
                status.AddMessageDetail("Invalid 'PaymentRecordCount' field value");
            }
            else if (status.Status && int.Parse(context.RequestEntity.PaymentRecordCount) != context.RequestEntity.PaymentRecordList.Count)
            {
                status.Status = false;
                status.AddMessageDetail("'PaymentRecordCount' value does not match the actual 'PaymentRecordList.Count'");
            }


            // validate individual status record (BaseRecord)
            if (status.Status)
            {
                StatusRecordValidator srv = new StatusRecordValidator();
                CommonStatus srvs = null;
                foreach (BaseRecord br in context.RequestEntity.PaymentRecordList)
                {
                    srvs = srv.Execute(new EAMIServiceValidationContext<BaseRecord>(context.DataContext, br));
                    br.IsValid = srvs.Status;
                    br.ValidationMessage = srvs.GetCombinedMessage();
                }

                // fail if no valid payment record is found
                if (!ValidationDataContext.HasValidPaymentRecords(context.RequestEntity.PaymentRecordList))
                {
                    status.Status = false;
                    status.AddMessageDetail("No valid payment status records found");
                }
            }


            return status;
        }
    }
}
