
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.Common;

namespace OHC.EAMI.ServiceManager
{
    public class RejectedPaymentInquiryValidator : EAMIServiceValidator<EAMIServiceValidationContext<RejectedPaymentInquiryRequest>>
    {
        public RejectedPaymentInquiryValidator()
            : base("RejectedPaymentInquiryValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<RejectedPaymentInquiryRequest> context)
        {
            CommonStatus status = new CommonStatus(true);

            // validate required fields
            base.ValidateRequiredFields<RejectedPaymentInquiryRequest>(context.RequestEntity, status);

            // validate Transaction
            if (status.Status)
            {
                TransactionValidator tv = new TransactionValidator();
                CommonStatus tvs = tv.Execute(new EAMIServiceValidationContext<EAMITransaction>(context.DataContext, context.RequestEntity));
                status.AddCommonStatus(tvs);
            }    
       
            // validate from/to dates
            if (status.Status && (context.RequestEntity.RejectedDateFrom > context.RequestEntity.RejectedDateTo ||
                    context.RequestEntity.RejectedDateFrom > DateTime.Now))
            {
                status.Status = false;
                status.AddMessageDetail("Incorrect Date Range combination in 'RejectedDateFrom' and 'RejectedDateTo' fields.");                
            }

            return status;
        }
    }
}
