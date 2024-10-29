#region | Using |
using System;
using System.Collections;
using System.ServiceModel;
#endregion

namespace OHC.EAMI.Common.ServiceInvoke
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ServiceFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Hashtable Channels = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class
        {
            string key = typeof(T).Name;

            lock (Channels.SyncRoot)
            {
                if (Channels.ContainsKey(key))
                {
                    var clientChannel = ((IClientChannel)Channels[key]);

                    switch (clientChannel.State)
                    {
                        case CommunicationState.Closing:
                        case CommunicationState.Closed:
                        case CommunicationState.Faulted:
                            clientChannel.Abort();
                            clientChannel.Close();
                            Channels.Remove(key);
                            Channels.Add(key, OpenChannel<T>());
                            break;
                    }
                }
                else
                {
                    Channels.Add(key, OpenChannel<T>());
                }

                return Channels[key] as T;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T OpenChannel<T>() where T : class
        {
            var factory = new ChannelFactory<T>(string.Format("BasicHttpBinding_{0}", typeof(T).Name));
            factory.Open();

            T channel = factory.CreateChannel();
            ((IClientChannel)channel).Faulted += ServiceFactory_Faulted;

            return channel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ServiceFactory_Faulted(object sender, EventArgs e)
        {
            var communicationObject = ((ICommunicationObject)sender);

            communicationObject.Abort();
            communicationObject.Close();

            lock (Channels.SyncRoot)
            {
                communicationObject.Faulted -= ServiceFactory_Faulted;
                Channels.Remove((sender).GetType().Name);
            }

        }
    }
}
