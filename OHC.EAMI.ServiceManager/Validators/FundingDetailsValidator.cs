
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.ServiceContract;

namespace OHC.EAMI.ServiceManager
{
    public class FundingDetailsValidator : EAMIServiceValidator<EAMIServiceValidationContext<PaymentRecord>>
    {
        public FundingDetailsValidator()
            : base("FundingDetailsValidator")
        { }

        public override CommonStatus Execute(EAMIServiceValidationContext<PaymentRecord> context)
        {
            CommonStatus status = new CommonStatus(true);

            if (context.RequestEntity.FundingDetailList == null || context.RequestEntity.FundingDetailList.Count == 0)
            {
                status.Status = false;
                status.AddMessageDetail("No Funding Details found. At least one funding detail is required for any submitted PaymentRecord");
                return status;
            }

            if (status.Status && context.RequestEntity.FundingDetailList.Count > context.DataContext.MaxFundingDetailsPerPaymentRec)
            {
                status.Status = false;
                status.AddMessageDetail("The Funding Detail count exceeds the maximum allowed of " + context.DataContext.MaxFundingDetailsPerPaymentRec.ToString());
                return status;
            }

            // validatre individual FDs
            decimal totalFundingAmount = 0;
            foreach (FundingDetail fd in context.RequestEntity.FundingDetailList)
            {
                // validate FDs required fields
                base.ValidateRequiredFields<FundingDetail>(fd, status);

                // validate max length on text feilds
                base.ValidateMaxTextLength(fd.FundingSourceName, 50, "FundingSourceName", status);
                base.ValidateMaxTextLength(fd.FFPAmount, 20, "FFPAmount", status);
                base.ValidateMaxTextLength(fd.SGFAmount, 20, "SGFAmount", status);
                base.ValidateMaxTextLength(fd.FiscalYear, 4, "FiscalYear", status);
                base.ValidateMaxTextLength(fd.FiscalQuarter, 2, "FiscalQuarter", status);
                base.ValidateMaxTextLength(fd.Title, 10, "Title", status);

                // FD FFPAmount
                base.ValidateTextAsAmount(fd.FFPAmount, "FundingDetail.FFPAmount", status);

                // FD SGFAmount
                base.ValidateTextAsAmount(fd.SGFAmount, "FundingDetail.SGFAmount", status);

                // validate funding generic name/value fields
                if (status.Status)
                {
                    GenericNameValueListValidator glv = new GenericNameValueListValidator(GenericNameValueListValidator.ListOwner.FundingDetail);
                    CommonStatus glvs = glv.Execute(new EAMIServiceValidationContext<Dictionary<string, string>>(context.DataContext, fd.GenericNameValueList));
                    status.AddCommonStatus(glvs);
                }

                // Validate Funding Source Name
                if (context.DataContext.ValidateFundingSource)
                {
                    string fundingSourceName = fd.FundingSourceName;
                    RefCode fs = context.DataContext.RefCodeTableList.GetRefCodeListByTableName(enRefTables.TB_FUNDING_SOURCE).FirstOrDefault(t => (t.Code.ToUpper() == fundingSourceName.ToUpper() && t.IsActive));
                    if (status.Status && fs == null)
                    {
                        status.Status = false;
                        status.AddMessageDetail("The FundingSourceName value '" + fundingSourceName + "' is not valid");
                    }
                }

                // put additional FundingDetail validation logic here


                // add up total funding amount
                totalFundingAmount = totalFundingAmount + (decimal.Parse(fd.SGFAmount) + decimal.Parse(fd.FFPAmount));

                // exit loop on the first invalid fundint
                if (status.Status == false) break;
            }

            // validate total funding amount equals payment record amount
            if (status.Status && decimal.Parse(context.RequestEntity.Amount) != totalFundingAmount)
            {
                status.Status = false;
                status.AddMessageDetail("The total amount for payment record Funding Details not equal to payment record amount.");
                return status;
            }

            return status;
        }

    }
}
