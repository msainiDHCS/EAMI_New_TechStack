using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.Common;
using OHC.EAMI.ServiceContract;
using OHC.EAMI.CommonEntity;

namespace OHC.EAMI.ServiceManager
{
    public class TransactionValidator : EAMIServiceValidator<EAMIServiceValidationContext<EAMITransaction>>
    {
        public TransactionValidator() 
            : base("TransactionValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<EAMITransaction> context)
        {
            CommonStatus status = new CommonStatus(true);

            // SenderID validations
            base.ValidateMaxTextLength(context.RequestEntity.SenderID, 10, "SenderID", status);
            if (status.Status && context.RequestEntity.SenderID != context.DataContext.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).GetRefCodeByCode(context.RequestEntity.SenderID).Code)
            {
                status.Status = false;
                status.AddMessageDetail("Incorrect SenderID value");
            }
            
            // ReceiverID validation
            base.ValidateMaxTextLength(context.RequestEntity.ReceiverID, 10, "ReceiverID", status);
            if (status.Status && context.RequestEntity.ReceiverID != SENDER_RECEIVER_ID.EAMI.ToString())
            {
                status.Status = false;
                status.AddMessageDetail("Incorrect ReceiverID value");
            }

            // TransactionID max length validation
            base.ValidateMaxTextLength(context.RequestEntity.TransactionID, 40, "TransactionID", status);

            // TransactionID already exists in db validation (only checks valid data submitting transactions)
            if (status.Status && context.DataContext.IsDuplicateTransaction(context.RequestEntity.TransactionID))
            {
                status.Status = false;
                status.AddMessageDetail("TransactionID [" + context.RequestEntity.TransactionID + "] already exist in the system");
            }

            // TransactionType validation
            if (status.Status && context.RequestEntity.TransactionType != context.RequestEntity.ActualTransactionType)
            {
                status.Status = false;
                status.AddMessageDetail("Incorrect TransactionType value");
            }

            // TransactionVersion validation
            base.ValidateMaxTextLength(context.RequestEntity.TransactionVersion, 5, "TransactionVersion", status);
            if (status.Status && context.RequestEntity.TransactionVersion != context.DataContext.TransactionVersion)
            {
                status.Status = false;
                status.AddMessageDetail("Incorrect TransactionVersion value");
            }

            //// TimeStamp validation
            //if (status.Status && (context.RequestEntity.TimeStamp > DateTime.Now || context.RequestEntity.TimeStamp < DateTime.Now.AddHours(-1)))
            //{
            //    status.Status = false;
            //    status.AddMessageDetail("TimeStamp value is out of range (can only be within 1 hour of data transmission)");
            //}     
            
            return status;

        }

        
    }
}
