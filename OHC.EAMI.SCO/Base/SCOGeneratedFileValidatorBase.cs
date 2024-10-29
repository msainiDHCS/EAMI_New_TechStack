using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.EAMI.Common;

namespace OHC.EAMI.SCO
{
    internal class SCOGeneratedFileValidatorBase : ValidatorBase<List<IClaimpayment>, CommonStatus>
    {
        int Organization_ID = 0;
        string Claim_Identifier;
        const double individualWarrantAmountMaximum = 99999999.99;
        const double EFTClaimFileTotalAmountMaximum = 9999999999.99;
        const int maxAllowedClaimSchedulesInEFTClaimFile = 100;

        public SCOGeneratedFileValidatorBase(int OrganizationID, string ClaimIdentifier)
            : base("SCOFileValidator", string.Empty, false)
        {
            Organization_ID = OrganizationID;
            Claim_Identifier = ClaimIdentifier;
        }

        public override CommonStatus Execute(List<IClaimpayment> paymentDataContext)
        {
            List<string> errorMessages = new List<string>();

            if (Organization_ID == 0)
            {
                errorMessages.Add("Invalid organization identifier.");
            }
            else if (Claim_Identifier.Trim().Length == 0)
            {
                errorMessages.Add("Claim Identifier value is empty");
            }
            else if (paymentDataContext == null || paymentDataContext.Count() <= 0)
            {
                errorMessages.Add("Invalid claim schedule data");
            }
            else
            {
                if (paymentDataContext.Where(a => a.ClaimScheduleNumber == 0).Count() > 0)
                    errorMessages.Add("Claim schedule number must be provided for each payment record");

                if (paymentDataContext.Select(a => a.ClaimScheduleNumber).Distinct().Count() > 100)
                    errorMessages.Add("An EFT claim file can contain no more thn " + maxAllowedClaimSchedulesInEFTClaimFile.ToString() + " claim schedules");

                if (paymentDataContext.Where(a => a.PAYMENT_AMOUNT == 0).Count() > 0)
                    errorMessages.Add("Payment amount must be greater than zero for each payment record");

                if (paymentDataContext.Where(a => a.PAYMENT_AMOUNT > individualWarrantAmountMaximum).Count() > 0)
                    errorMessages.Add("Individual warrant amount cannot be greater than " + string.Format("{0:C}", individualWarrantAmountMaximum));

                if (paymentDataContext.Sum(a => a.PAYMENT_AMOUNT) > EFTClaimFileTotalAmountMaximum)
                    errorMessages.Add("EFT claim file totals cannot be greater than " + string.Format("{0:C}", EFTClaimFileTotalAmountMaximum));

                if (paymentDataContext.Where(a => (a.VENDOR_NO == null || a.VENDOR_NO.Trim().Length == 0)).Count() > 0)
                    errorMessages.Add("Vendor number must be provided for each payment record");

                //if (paymentDataContext.Where(a => (a.VENDOR_FIRST_NAME == null || a.VENDOR_FIRST_NAME.Trim().Length == 0)).Count() > 0)
                //    errorMessages.Add("Vendor first name must be provided for each payment record");

                //if (paymentDataContext.Where(a => (a.VENDOR_LAST_NAME == null || a.VENDOR_LAST_NAME.Trim().Length == 0)).Count() > 0)
                //    errorMessages.Add("Vendor last name must be provided for each payment record");

                if (paymentDataContext.Where(a => (a.VENDOR_ZIPCODE_FIRST5 == null || a.VENDOR_ZIPCODE_FIRST5.Trim().Length == 0)).Count() > 0)
                    errorMessages.Add("Vendor five digit zip code must be provided for each payment record");
            }

            if (errorMessages.Count() > 0)
                return new CommonStatus(false, errorMessages);
            else
                return new CommonStatus(true);
        }
    }
}
