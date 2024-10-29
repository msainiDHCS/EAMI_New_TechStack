using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.ServiceManager
{    
    public class PaymentSetValidator : EAMIServiceValidator<EAMIServiceValidationContext<PaymentSet>>
    {
        public PaymentSetValidator()
            : base("PaymentSetValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<PaymentSet> context)
        {
            CommonStatus status = new CommonStatus(true);

            
            // validate PS total amount
            decimal pgTotalAmount = ValidationDataContext.GetPaymentRecordListTotalAmount(context.RequestEntity.PaymentRecList);
            if(pgTotalAmount <= 0)
            {
                status.Status = false;
                status.AddMessageDetail("The total amount for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is negative or  $0.00");   
            }

            //// validate PS max amount
            //if (status.Status && pgTotalAmount > 99999999)
            //{
            //    status.Status = false;
            //    status.AddMessageDetail("The total amount for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is greater than maximum allowed.");
            //}

            // validate PS has single distinct payee
            if (status.Status && context.RequestEntity.PaymentRecList.GroupBy(pr => (pr.PayeeInfo.EntityID + pr.PayeeInfo.EntityIDSuffix)).Count() > 1)
            {
                status.Status = false;
                status.AddMessageDetail("The payee entity for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is not distinct");
            }

            // validate PS payment type
            if (status.Status && context.RequestEntity.PaymentRecList.GroupBy(pr => pr.PaymentType).Count() > 1)
            {
                status.Status = false;
                status.AddMessageDetail("The PaymentType for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is not distinct");
            }

            //// validate PS payment date
            //if (status.Status && context.RequestEntity.PaymentRecList.GroupBy(pr => pr.PaymentDate).Count() > 1)
            //{
            //    status.Status = false;
            //    status.AddMessageDetail("The PaymentDate for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is not distinct");
            //}
            

            // validate PS fiscal year
            if (status.Status && context.RequestEntity.PaymentRecList.GroupBy(pr => pr.FiscalYear).Count() > 1)
            {
                status.Status = false;
                status.AddMessageDetail("The FiscalYear for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is not distinct");
            }

            // NOTE: below KVP validations will need to be refactored once EAMI takes other than CAPMAN payments

            // validate PS contract number
            if (status.Status && context.RequestEntity.PaymentRecList.GroupBy(pr => pr.GenericNameValueList["CONTRACT_NUMBER"]).Count() > 1)
            {
                status.Status = false;
                status.AddMessageDetail("The ContractNumber for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is not distinct");
            }

            // validate PS exclisive payment code
            if (status.Status && context.RequestEntity.PaymentRecList.GroupBy(pr => pr.GenericNameValueList["EXCLUSIVE_PYMT_CODE"]).Count() > 1)
            {
                status.Status = false;
                status.AddMessageDetail("The EXCLUSIVE_PYMT_CODE value for PaymentSetNumber '" + context.RequestEntity.PaymentSetNumber + "' is not distinct");
            }

            // exclusive payment code validator for specific active code
            string exclPymtCode = context.RequestEntity.PaymentRecList[0].GenericNameValueList["EXCLUSIVE_PYMT_CODE"];
            RefCode exclPymtRefCode = context.DataContext.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_EXCLUSIVE_PAYMENT_TYPE).FirstOrDefault(t => (t.Code.ToUpper() == exclPymtCode.ToUpper() && t.IsActive));            
            if (status.Status && exclPymtRefCode == null)
            {
                status.Status = false;
                status.AddMessageDetail("The EXCLUSIVE_PYMT_CODE value '" + exclPymtCode + "' is not valid");
            }          

            return status;
        }
    }


    public class PaymentSet
    {
        public PaymentSet(string psNumber)
        {
            this.PaymentSetNumber = psNumber;
            this.IsValid = true;
            this.ValidationMessage = string.Empty;
            this.PaymentRecList = new List<PaymentRecord>();

        }
        public string PaymentSetNumber { get; private set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public List<PaymentRecord> PaymentRecList { get; set; }
    }


}
