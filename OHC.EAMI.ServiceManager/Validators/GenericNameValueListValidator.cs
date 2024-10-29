
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OHC.EAMI.Common;
using OHC.EAMI.CommonEntity;

namespace OHC.EAMI.ServiceManager
{
    public class GenericNameValueListValidator : EAMIServiceValidator<EAMIServiceValidationContext<Dictionary<string, string>>>
    {
        public enum ListOwner
        {
            PaymentRecord = 1,
            FundingDetail = 2
        }

        private ListOwner listOwner;

        public GenericNameValueListValidator(ListOwner owner)
            : base("GenericNameValueListValidator")
        {
            this.listOwner = owner;
        }        


        public override CommonStatus Execute(EAMIServiceValidationContext<Dictionary<string, string>> context)
        {
                CommonStatus status = new CommonStatus(true);
                Dictionary<string, KvpDefinition> expectedKvpList = new Dictionary<string, KvpDefinition>();
                if (listOwner == ListOwner.PaymentRecord)
                {
                    expectedKvpList = context.DataContext.ExpectedPaymentRecKvpList;
                }
                else if (listOwner == ListOwner.FundingDetail)
                {
                    expectedKvpList = context.DataContext.ExpectedFundingKvpList;
                }

                bool hasExpectedItems = (expectedKvpList.Count > 0);
                bool hasActualItems = (context.RequestEntity != null && context.RequestEntity.Count > 0);

                // fail if mismatch between expected and actual keys
                if (hasExpectedItems && !hasActualItems)
                {
                    status.Status = false;
                    status.AddMessageDetail(listOwner.ToString() + " is missing one or more expected Name/Value fields");                
                }                
                else if (status.Status && !hasExpectedItems && hasActualItems)
                {
                    status.Status = false;
                    status.AddMessageDetail(listOwner.ToString() + " contains one or more unexpected Name/Value fields");
                }                
                else if (status.Status && hasExpectedItems && hasActualItems && listOwner == ListOwner.PaymentRecord &&
                    expectedKvpList.Keys.All(context.RequestEntity.Keys.Contains) == false)
                {
                    status.Status = false;
                    status.AddMessageDetail(listOwner.ToString() + " is missing one or more expected Name/Value fields or contains one or more unexpected Name/Value fields.");
                }    
                
                // fail if no-value or too-large-value is provided for otherwize valid/expected key
                if (status.Status && hasExpectedItems && hasActualItems)
                {
                    foreach (KeyValuePair<string, string> kvp in context.RequestEntity)
                    {
                        // continue if FundingDetail value is empty or null
                        if (status.Status && listOwner == ListOwner.FundingDetail && string.IsNullOrWhiteSpace(kvp.Value))
                        {
                            continue;
                        }

                        // fail if empty string or white space
                        if (status.Status && listOwner == ListOwner.PaymentRecord && string.IsNullOrWhiteSpace(kvp.Value))
                        {
                            status.Status = false;
                            status.AddMessageDetail("No value was provided for generic Name/Value fields with name '"  + kvp.Key + "'"); 
                        }

                        // fail if value exeeds max length
                        if (status.Status && kvp.Value.Length > expectedKvpList[kvp.Key].ValueLength)
                        {
                            status.Status = false;
                            status.AddMessageDetail("The value length exceeded maximum allowed for generic Name/Value field with name '" + kvp.Key + "'");
                        }

                        // fail if value cannot be converted to appropriate data type
                        if (status.Status)
                        {
                            bool parseWorked = true;
                            DateTime dtResult;
                            bool blnResult;
                            int intResult;
                            decimal dmResult;

                            switch (expectedKvpList[kvp.Key].DataType)
                            {
                                case "DATE":
                                    parseWorked = DateTime.TryParse(kvp.Value, out dtResult);
                                    break;
                                case "BOOL":
                                    parseWorked = bool.TryParse(kvp.Value, out blnResult);
                                    break;
                                case "INT":
                                    parseWorked = int.TryParse(kvp.Value, out intResult);
                                    break;
                                case "DECIMAL":
                                    parseWorked = decimal.TryParse(kvp.Value, out dmResult);
                                    break;
                            }

                            if (parseWorked == false)
                            {
                                status.Status = false;
                                status.AddMessageDetail("The KVP value (for the key '" + kvp.Key + "') is not correctly formated");
                            }                            
                        }
                    }
                }

                return status;
        }
    }
}
