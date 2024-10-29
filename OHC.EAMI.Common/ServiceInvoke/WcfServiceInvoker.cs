#region | Using |
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Web;
#endregion

namespace OHC.EAMI.Common.ServiceInvoke
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceInvoker
    {
        TR InvokeService<T, TR>(Func<T, TR> invokeHandler, Boolean IsAnonymous = false)
            where T : class
            where TR : class;
    }

    /// <summary>
    /// 
    /// </summary>
    public class WcfServiceInvoker : IServiceInvoker
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ChannelFactoryManager _factoryManager = new ChannelFactoryManager();

        /// <summary>
        /// 
        /// </summary>
        private static readonly ClientSection _clientSection =
            ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

        #region IServiceInvoker Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invokeHandler"></param>
        /// <param name="IsAnonymous"></param>
        /// <returns></returns>
        public TResult InvokeService<T, TResult>(Func<T, TResult> invokeHandler, Boolean IsAnonymous = false)
            where T : class
            where TResult : class
        {

            ServicePointManager.ServerCertificateValidationCallback =
               new RemoteCertificateValidationCallback(IgnoreCertificateErrorHandler);

            var endpointNameAddressPair = GetEndpointNameAddressPair(typeof(T));
            var arg = _factoryManager.CreateChannel<T>(endpointNameAddressPair.Key, endpointNameAddressPair.Value, IsAnonymous);
            var obj2 = (ICommunicationObject)arg;
            try
            {
                return invokeHandler(arg);
            }
            finally
            {
                try
                {
                    if (obj2.State != CommunicationState.Faulted)
                    {
                        obj2.Close();
                    }
                }
                catch
                {
                    obj2.Abort();
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="invokeHandler"></param>
        public void InvokeService<T>(Action<T> invokeHandler) where T : class
        {
            var endpointNameAddressPair = GetEndpointNameAddressPair(typeof(T));
            var arg = _factoryManager.CreateChannel<T>(endpointNameAddressPair.Key, endpointNameAddressPair.Value);
            var obj2 = (ICommunicationObject)arg;
            try
            {
                invokeHandler(arg);
            }
            finally
            {
                try
                {
                    if (obj2.State != CommunicationState.Faulted)
                    {
                        obj2.Close();
                    }
                }
                catch
                {
                    obj2.Abort();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceContractType"></param>
        /// <returns></returns>
        private KeyValuePair<string, string> GetEndpointNameAddressPair(Type serviceContractType)
        {
            var configException =
                new ConfigurationErrorsException(
                    string.Format(
                        "No client endpoint found for type {0}. Please add the section <client><endpoint name=\"myservice\" address=\"http://address/\" binding=\"basicHttpBinding\" contract=\"{0}\"/></client> in the config file.",
                        serviceContractType));
            if (((_clientSection == null) || (_clientSection.Endpoints == null)) || (_clientSection.Endpoints.Count < 1))
            {
                throw configException;
            }

            int intProgramChoiceId = Convert.ToInt32(HttpContext.Current.Session["ProgramChoiceId"]);
            
            string serviceEndpointName = null;
            if (intProgramChoiceId == 2)    //Pharmacy
            {
                serviceEndpointName = "PharmacyServiceEndpoint";
            }
            else if (intProgramChoiceId == 3)    //Dental
            {
                serviceEndpointName = "DentalServiceEndpoint";
            }
            else if (intProgramChoiceId == 4)
            {
                serviceEndpointName = "ManagedCareServiceEndpoint";
            }

            var serviceEndpointObject = _clientSection.Endpoints.Cast<ChannelEndpointElement>()
                                          .SingleOrDefault(element => element.Name == serviceEndpointName
                                          && element.Contract == serviceContractType.ToString());

            return new KeyValuePair<string, string>(serviceEndpointObject.Name, serviceEndpointObject.Address.AbsoluteUri);

            throw configException;
        }

    }
}
