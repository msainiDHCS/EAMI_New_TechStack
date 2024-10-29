
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.Common;

namespace OHC.EAMI.ServiceManager
{
    public class StatusRecordValidator : EAMIServiceValidator<EAMIServiceValidationContext<BaseRecord>>
    {
        public StatusRecordValidator()
            : base("StatusRecordValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<BaseRecord> context)
        {
            CommonStatus status = new CommonStatus(true);

            // validate required fields
            base.ValidateRequiredFields<BaseRecord>(context.RequestEntity, status);
             
            // validate max length
            base.ValidateMaxTextLength(context.RequestEntity.PaymentRecNumber, 14, "PaymentRecNumber", status);

            return status;
        }
    }
}
