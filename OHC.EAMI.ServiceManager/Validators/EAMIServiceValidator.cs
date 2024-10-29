using OHC.EAMI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OHC.EAMI.ServiceManager
{
    /// <summary>
    /// this class extends ValidatorBase and serves as an abstract base type for all EAMI service data entities
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class EAMIServiceValidator<TContext> : ValidatorBase<TContext, CommonStatus> 
    {
        public EAMIServiceValidator(string validatorName) : base(validatorName)
        { }


        /// <summary>
        /// performs required-field validation on public property members of provided obj instance
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected virtual void ValidateRequiredFields<TEntity>(TEntity entity, CommonStatus status)
        {
            foreach (PropertyInfo ip in entity.GetType().GetProperties())
            {
                // only look into [DataMember] decorated class property members
                if (ip.GetCustomAttribute(typeof(DataMemberAttribute)) != null)
                {                    
                    DataMemberAttribute dma = ip.GetCustomAttribute(typeof(DataMemberAttribute)) as DataMemberAttribute;

                    // validatte any other required elements (complex class types and Lists)
                    if (status.Status && dma.IsRequired && ip.GetValue(entity, null) == null)
                    {
                        status.Status = false;
                        status.AddMessageDetail("No value is provided for a required field '" + ip.Name + "' in entity '" + entity.GetType().Name + "'");
                    }

                    // validate "no value" for required (String, Int, DataTime) fields
                    string textValue = (ip.GetValue(entity, null) != null ? ip.GetValue(entity, null).ToString() : string.Empty);
                    if (status.Status &&
                        (ip.PropertyType.ToString() == "System.String" && string.IsNullOrWhiteSpace(textValue) && dma.IsRequired) ||
                        (ip.PropertyType.ToString() == "System.Int32" && textValue == "0" && dma.IsRequired) ||
                        (ip.PropertyType.ToString() == "System.DateTime" && textValue == "1/1/0001 12:00:00 AM" && dma.IsRequired))
                    {
                        status.Status = false;
                        status.AddMessageDetail("No value is provided for a required field '" + ip.Name + "' in entity '" + entity.GetType().Name + "'");
                    }                    
                }
            }                      
        }


        /// <summary>
        /// performs MaxLength validation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <param name="fieldName"></param>
        /// <param name="status"></param>
        protected virtual void ValidateMaxTextLength(string value, int maxLength, string fieldName, CommonStatus status)
        {
            // here we check and exit if the value is null
            // the null value shold only appear for optional elements
            // the required field validation (procedure above) is executed prior to this 'max text len validation' 
            if (string.IsNullOrWhiteSpace(value))
                return;

            if (status.Status && value.Length > maxLength)
            {
                status.Status = false;
                status.AddMessageDetail("The value exceeds maximum allowed length for field '" + fieldName + "'");
            }
        }

        /// <summary>
        /// performs formatting validation for amount as text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="status"></param>
        protected virtual void ValidateTextAsAmount(string value, string fieldName, CommonStatus status)
        {            
            if(status.Status)
            {
                decimal outAmount = 0;
                Regex rgx = new Regex(@"^-?\d+\.\d{2}?$");
                if(decimal.TryParse(value, out outAmount) == false || rgx.IsMatch(value) == false)
                {
                    status.Status = false;
                    status.AddMessageDetail("The value for " + fieldName + " is not properly formatted");
                }
            }             
        }


        /// <summary>
        /// performs formatting validation for zip code as text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="status"></param>
        //protected virtual void ValidatePEEZipCode(string value, string fieldName, CommonStatus status)
        //{
        //    if (status.Status)
        //    {
        //        Regex rgx = new Regex("^[0-9]{5}(?:-[0-9]{4})?$");
                
        //        if (rgx.IsMatch(value) == false)
        //        {
        //            status.Status = false;
        //            status.AddMessageDetail("The value for " + fieldName + " is not properly formatted");
        //        }
        //    }
        //}

    }
}
