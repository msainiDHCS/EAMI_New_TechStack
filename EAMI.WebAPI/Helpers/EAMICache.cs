//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Configuration;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web;
//using System.Web.Caching;

//namespace OHC.EAMI.WebUI
//{
//    /// <summary>
//    /// with Help from https://codereview.stackexchange.com/questions/48148/generic-thread-safe-memorycache-manager-for-c
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public sealed class EAMIObjectCache<T> : MemoryCache where T : class
//    {
//        //private object WriteLock { get; } = new object();
//        private object WriteLock = new object();

//        static ConcurrentDictionary<string, EAMIObjectCache<T>> eamiDic = new ConcurrentDictionary<string, EAMIObjectCache<T>>();
//        public static EAMIObjectCache<T> GetInstance(string instanceName)
//        {
//            if (!eamiDic.ContainsKey(instanceName))
//            {
//                eamiDic.TryAdd(instanceName, new EAMIObjectCache<T>(instanceName));
//            }

//            return eamiDic[instanceName];
//        }

//        private CacheItemPolicy HardDefaultCacheItemPolicy = new CacheItemPolicy()
//        {
//            SlidingExpiration = new TimeSpan(0, 15, 0)
//        };

//        private CacheItemPolicy defaultCacheItemPolicy;

//        public EAMIObjectCache(string name, NameValueCollection nvc = null, CacheItemPolicy policy = null) : base(name, nvc)
//        {
//            defaultCacheItemPolicy = policy ?? HardDefaultCacheItemPolicy;
//        }

//        public void Set(string cacheKey, T cacheItem, CacheItemPolicy policy = null)
//        {
//            policy = policy ?? defaultCacheItemPolicy;
//            base.Set(cacheKey, cacheItem, policy);
//        }

//        public void Set(string cacheKey, Func<T> getData, CacheItemPolicy policy = null)
//        {
//            this.Set(cacheKey, getData(), policy);
//        }

//        public bool TryGetAndSet(string cacheKey, Func<T> getData, out T returnData, CacheItemPolicy policy = null)
//        {
//            if (TryGet(cacheKey, out returnData))
//                return true;

//            lock (WriteLock)
//            {
//                if (TryGet(cacheKey, out returnData))
//                    return true;

//                returnData = getData();
//                Set(cacheKey, returnData, policy);
//            }

//            return false;
//        }

//        public bool TryGet(string cacheKey, out T returnItem)
//        {
//            returnItem = (T)this[cacheKey];
//            return returnItem != null;
//        }
//    }
//}