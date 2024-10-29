using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Common
{
    /// <summary>
    /// this class extends ActionBase class and serves as base implementation for specific validator class
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class ValidatorBase<TContext, TResult> : ActionBase<TContext, TResult>
    {    
        public ValidatorBase(string validatorName)
            : this(validatorName, string.Empty, true)
        { }

        public ValidatorBase(string validatorName, string errorCode)
            : this(validatorName, errorCode, true)
        { }

        public ValidatorBase(string validatorName, string errorCode, bool isStopping)
        {
            this.ValidatorName = validatorName;
            this.ErrorCode = errorCode;
            this.IsStopping = isStopping;
        }

        public string ValidatorName { get; private set; }

        public string ErrorCode { get; private set; }

        public bool IsStopping { get; private set; }        

    }
}
