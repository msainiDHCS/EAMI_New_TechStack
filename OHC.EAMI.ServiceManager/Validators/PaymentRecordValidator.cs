
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.Common;
using OHC.EAMI.ServiceContract;

namespace OHC.EAMI.ServiceManager
{
    public class PaymentRecordValidator : EAMIServiceValidator<EAMIServiceValidationContext<PaymentRecord>>
    {
        public PaymentRecordValidator(List<PaymentRecord> prList, ValidationDataContext dataContext)
            : base("PaymentRecordValidator")
        {
            this.ValidatedRecordNumberList = new List<string>();
            this.PreviouslySubmittedRecordNumberList = new List<string>();
            this.PreviouslySubmittedPaymentSetNumberList = new List<string>();

            foreach (PaymentRecord pr in prList)
            {
                if (!this.PreviouslySubmittedPaymentSetNumberList.Contains(pr.PaymentSetNumber))
                {
                    this.PreviouslySubmittedPaymentSetNumberList.Add(pr.PaymentSetNumber);
                }
                if (!this.PreviouslySubmittedRecordNumberList.Contains(pr.PaymentRecNumber))
                {
                    this.PreviouslySubmittedRecordNumberList.Add(pr.PaymentRecNumber);
                }
            }

            // populate potential dup items from data context (which pulls it from database)
            this.PreviouslySubmittedPaymentSetNumberList = dataContext.GetExistingPaymentSetNumberList(this.PreviouslySubmittedPaymentSetNumberList);
            this.PreviouslySubmittedRecordNumberList = dataContext.GetExistingPaymentRecordNumList(this.PreviouslySubmittedRecordNumberList);
        }

        
        // holds PaymentSetNumbers values that were previously submitted in different transaction
        // used to prevent submission of duplicate PaymentSetNumbers
        private List<string> PreviouslySubmittedPaymentSetNumberList { get; set; }

        // holds PaymentRecordNumbers that were previously submitted in different transaction
        // used to prevent submission of duplicate records
        private List<string> PreviouslySubmittedRecordNumberList { get; set; }
        
        // holds payment record numbers that were already validated within current submission transaction
        // used to detect duplicate records within single submission transaction.
        private List<string> ValidatedRecordNumberList { get; set; }
       


        public override CommonStatus Execute(EAMIServiceValidationContext<PaymentRecord> context)
        {
            CommonStatus status = new CommonStatus(true);

            // validate required fields
            base.ValidateRequiredFields<PaymentRecord>(context.RequestEntity, status);            

            // check if rec is dup within the transaction or already exist within the system, or has PaymentSetNumber that already exist within the system
            if (status.Status)
            {
                if (this.ValidatedRecordNumberList.Contains(context.RequestEntity.PaymentRecNumber))
                {
                    status.Status = false;
                    status.AddMessageDetail("Duplicate record within request transaction");
                }
                else if (this.PreviouslySubmittedRecordNumberList.Contains(context.RequestEntity.PaymentRecNumber))
                {
                    status.Status = false;
                    status.AddMessageDetail("Duplicate record, already exist in the system");
                }
                else if (this.PreviouslySubmittedPaymentSetNumberList.Contains(context.RequestEntity.PaymentSetNumber))
                {
                    status.Status = false;
                    status.AddMessageDetail("The payment record has PaymentSetNumber that already exsit in the system");
                }

                // add to list to make sure no dups are found 
                this.ValidatedRecordNumberList.Add(context.RequestEntity.PaymentRecNumber);
            }
                        
            // validate max length on text feilds
            base.ValidateMaxTextLength(context.RequestEntity.PaymentRecNumber, 14, "PaymentRecNumber", status);
            base.ValidateMaxTextLength(context.RequestEntity.PaymentRecNumberExt, 28, "PaymentRecNumberExt", status);
            base.ValidateMaxTextLength(context.RequestEntity.PaymentType, 50, "PaymentType", status);
            base.ValidateMaxTextLength(context.RequestEntity.FiscalYear, 10, "FiscalYear", status);
            base.ValidateMaxTextLength(context.RequestEntity.IndexCode, 10, "IndexCode", status);
            base.ValidateMaxTextLength(context.RequestEntity.ObjectDetailCode, 10, "ObjectDetailCode", status);
            base.ValidateMaxTextLength(context.RequestEntity.ObjectAgencyCode, 10, "ObjectAgencyCode", status);
            base.ValidateMaxTextLength(context.RequestEntity.PCACode, 10, "PCACode", status);
            base.ValidateMaxTextLength(context.RequestEntity.ApprovedBy, 30, "ApprovedBy", status);
            base.ValidateMaxTextLength(context.RequestEntity.PaymentSetNumber, 14, "PaymentSetNumber", status);
            base.ValidateMaxTextLength(context.RequestEntity.PaymentSetNumberExt, 28, "PaymentSetNumberExt", status);

            // validate Payee info
            if (status.Status)
            {
                PaymentExchangeEntityValidator peev = 
                    new PaymentExchangeEntityValidator(PaymentExchangeEntityValidator.PaymentExchangeEntityType.Payee);
                CommonStatus peevs = 
                    peev.Execute(new EAMIServiceValidationContext<PaymentExchangeEntity>(context.DataContext, context.RequestEntity.PayeeInfo));
                status.AddCommonStatus(peevs);
            }            

            // validate amount text formating            
            base.ValidateTextAsAmount(context.RequestEntity.Amount, "PaymentRecord.Amount", status);

            // validate max amount
            if (status.Status && decimal.Parse(context.RequestEntity.Amount) > context.DataContext.MaxPaymentRecAmount)
            {
                status.Status = false;
                status.AddMessageDetail("'Amount' exceeds the max allowed for the PaymentRecNumber '" + 
                                            context.RequestEntity.PaymentRecNumber + 
                                            "' (value cannot be more than " + 
                                            context.DataContext.MaxPaymentRecAmount.ToString() + ")");
            }            

            // validate generic name/value fields
            if (status.Status)
            {                
                GenericNameValueListValidator glv = 
                    new GenericNameValueListValidator(GenericNameValueListValidator.ListOwner.PaymentRecord);
                CommonStatus glvs = 
                    glv.Execute(new EAMIServiceValidationContext<Dictionary<string, string>>(context.DataContext, context.RequestEntity.GenericNameValueList));
                status.AddCommonStatus(glvs);
            }  
          
            // validate binary attachment is pdf
            if (status.Status && context.RequestEntity.Attachment != null && context.RequestEntity.Attachment.Length > 0)
            {
                byte[] first4Chars = context.RequestEntity.Attachment.Take(4).ToArray();
                if (Encoding.UTF8.GetString(first4Chars, 0, first4Chars.Length) != "%PDF")
                {
                    status.Status = false;
                    status.AddMessageDetail("Incorrect Attachment file/content, expecting PDF");
                }                
            }

            // validate funding details
            if (status.Status)
            {
                CommonStatus fdvs = new FundingDetailsValidator().Execute(context);
                status.AddCommonStatus(fdvs);
            }

            return status;
        }
        
    }
}
