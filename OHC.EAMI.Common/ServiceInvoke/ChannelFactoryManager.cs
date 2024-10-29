#region | Using |
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Linq;

#endregion

namespace OHC.EAMI.Common.ServiceInvoke
{
    /// <summary>
    /// 
    /// </summary>
    public class ChannelFactoryManager : IDisposable
    {
        #region | Private readonly variables |

        private static readonly Dictionary<Type, ChannelFactory> _factories = new Dictionary<Type, ChannelFactory>();
        private static readonly Dictionary<Type, ChannelFactory> _factoriesAnonymous = new Dictionary<Type, ChannelFactory>();
        private static readonly object Locker = new object();
        
        #endregion

        #region IDisposable Members


        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T CreateChannel<T>() where T : class
        {
            return CreateChannel<T>("*", null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointConfigurationName"></param>
        /// <returns></returns>
        public virtual T CreateChannel<T>(string endpointConfigurationName) where T : class
        {
            return CreateChannel<T>(endpointConfigurationName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="IsAnonymous"></param>
        /// <returns></returns>
        public virtual T CreateChannel<T>(string endpointConfigurationName, string endpointAddress,
            Boolean IsAnonymous = false) where T : class
        {
            CheckFactory<T>(endpointConfigurationName, endpointAddress, IsAnonymous = false);
            T local = GetFactory<T>(endpointConfigurationName, endpointAddress, IsAnonymous).CreateChannel();
            ((IClientChannel)local).Faulted += ChannelFaulted;
            ((IClientChannel)local).Open();
            return local;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelFaulted(object sender, EventArgs e)
        {
            var channel = (IClientChannel)sender;
            try
            {
                channel.Close();
            }
            catch
            {
                channel.Abort();
            }

            throw new ApplicationException("Exc_ChannelFailure");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointConfiguration"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="IsAnonymous"></param>
        /// <returns></returns>
        protected virtual ChannelFactory<T> GetFactory<T>(string endpointConfiguration, string endpointAddress,
            Boolean IsAnonymous = false) where T : class
        {
            lock (Locker)
            {
                ChannelFactory factory;

                if (IsAnonymous)
                {
                    if (!_factoriesAnonymous.TryGetValue(typeof(T), out factory))
                    {
                        factory = CreateFactoryInstance<T>(endpointConfiguration, endpointAddress);
                        _factoriesAnonymous.Add(typeof(T), factory);
                    }
                }
                else if (!_factories.TryGetValue(typeof(T), out factory))
                {
                    factory = CreateFactoryInstance<T>(endpointConfiguration, endpointAddress);
                    _factories.Add(typeof(T), factory);
                }

                return (factory as ChannelFactory<T>);
            }
        }
                /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointConfiguration"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="IsAnonymous"></param>
        /// <returns></returns>

        //Clears cache when switching between RX and Dental
        protected virtual void CheckFactory<T>(string endpointConfiguration, string endpointAddress,
            Boolean IsAnonymous = false) where T : class
        {
           if(_factories.Values.Where(x => x.Endpoint.Address.ToString() == endpointAddress) != null)
            {
               _factories.Clear();
            }
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="endpointAddress"></param>
        /// <returns></returns>
        private ChannelFactory CreateFactoryInstance<T>(string endpointConfigurationName, string endpointAddress)
        {
            ChannelFactory factory;
            factory = !string.IsNullOrEmpty(endpointAddress)
                ? new ChannelFactory<T>(endpointConfigurationName, new EndpointAddress(endpointAddress))
                : new ChannelFactory<T>(endpointConfigurationName);
            factory.Faulted += FactoryFaulted;

            //var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            //factory.Endpoint.Behaviors.Remove(defaultCredentials);

            //ClientCredentials loginCredentials = new ClientCredentials();
            //loginCredentials.Windows.AllowNtlm = true;
            //loginCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            //factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            
            factory.Open();

            return factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void FactoryFaulted(object sender, EventArgs args)
        {
            var factory = (ChannelFactory)sender;
            try
            {
                factory.Close();
            }
            catch (Exception ex)
            {
                factory.Abort();
            }

            Type[] genericArguments = factory.GetType().GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                Type key = genericArguments[0];
                if (_factories.ContainsKey(key))
                {
                    _factories.Remove(key);
                }
                else if (_factoriesAnonymous.ContainsKey(key))
                {
                    _factoriesAnonymous.Remove(key);
                }
            }

            throw new ApplicationException("Exc_ChannelFactoryFailure");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (Locker)
                {
                    foreach (Type type in _factories.Keys)
                    {
                        ChannelFactory factory = _factories[type];

                        if (factory.State == CommunicationState.Faulted)
                        {
                            factory.Abort(); continue;
                        }
                        factory.Close();
                    }

                    _factories.Clear();

                    foreach (Type type in _factoriesAnonymous.Keys)
                    {
                        ChannelFactory factory = _factoriesAnonymous[type];

                        if (factory.State == CommunicationState.Faulted)
                        {
                            factory.Abort(); continue;
                        }
                        factory.Close();
                    }

                    _factoriesAnonymous.Clear();
                }
            }
        }

    }
}
