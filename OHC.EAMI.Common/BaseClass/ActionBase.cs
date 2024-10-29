using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Common
{
    /// <summary>
    /// this class provides base/abstract implementation for encapsulating action logic
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class ActionBase<TContext, TResult>
    {
        public abstract TResult Execute(TContext context);
    }   
}
