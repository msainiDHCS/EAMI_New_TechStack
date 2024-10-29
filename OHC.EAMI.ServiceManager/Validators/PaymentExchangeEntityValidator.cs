
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.ServiceContract;
using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;

namespace OHC.EAMI.ServiceManager
{
    public class PaymentExchangeEntityValidator : EAMIServiceValidator<EAMIServiceValidationContext<PaymentExchangeEntity>>
    {

        public PaymentExchangeEntityValidator(PaymentExchangeEntityType enPaymentEntityType) 
            : base("PaymentExchangeEntityValidator")
        {
            this.PaymentEntityType = enPaymentEntityType;
        }

        public enum PaymentExchangeEntityType
        {
            Payer = 1,
            Payee = 2
        }

        public PaymentExchangeEntityType PaymentEntityType { get; private set; }

        public override CommonStatus Execute(EAMIServiceValidationContext<PaymentExchangeEntity> context)
        {
            CommonStatus status = new CommonStatus(true);

            // required field validation
            base.ValidateRequiredFields<PaymentExchangeEntity>(context.RequestEntity, status);

            // validate field length
            base.ValidateMaxTextLength(context.RequestEntity.EntityID, 10, "EntityID", status);
            base.ValidateMaxTextLength(context.RequestEntity.EntityIDSuffix, 10, "EntityIDSuffix", status);
            base.ValidateMaxTextLength(context.RequestEntity.EntityIDType, 30, "EntityIDType", status);
            base.ValidateMaxTextLength(context.RequestEntity.Name, 30, "Name(Payee)", status);
            base.ValidateMaxTextLength(context.RequestEntity.AddressLine1, 30, "AddressLine1", status);
            base.ValidateMaxTextLength(context.RequestEntity.AddressLine2, 30, "AddressLine2", status);
            base.ValidateMaxTextLength(context.RequestEntity.AddressLine3, 30, "AddressLine3", status);
            base.ValidateMaxTextLength(context.RequestEntity.AddressCity, 27, "AddressCity", status);
            base.ValidateMaxTextLength(context.RequestEntity.AddressState, 2, "AddressState", status);
            base.ValidateMaxTextLength(context.RequestEntity.AddressZip, 10, "AddressZip", status);
            base.ValidateMaxTextLength(context.RequestEntity.EIN, 10, "EIN", status);
            base.ValidateMaxTextLength(context.RequestEntity.VendorTypeCode, 1, "VendorTypeCode", status);

            //base.ValidatePEEZipCode(context.RequestEntity.AddressZip, "AddressZip", status);
            return status;
        }
    }
}
