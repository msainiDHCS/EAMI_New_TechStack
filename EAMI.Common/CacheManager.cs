using NLog.Internal;
using System.Runtime.Caching;

namespace EAMI.Common
{
    /// <summary>
    /// this class provides type safe memory caching functionality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CacheManager<T>
    {
        /* 
        NOTE: this class can be used in two ways:
        As Static Class: provides type safe memory cache Get and Set methods allowing to place and retreive
             objects into memory cachce with key, scope (optional), and slidingExpertation(optional) paremeters
        As Derived Instance: provides instance based functionality allowing to set a derived
            object instance into memory cache
        WARNING: use scope parameter to avoid key conflicts which can result in inconsistent behaviour or loss of cached data
        */

        private static int DEFAULT_SLIDING_EXPIRATION_IN_MINUTES = 1;
        private static string DEFAULT_SCOPE = "DEFAULT";

        #region constructors

        public CacheManager(string key)
            : this(key, DEFAULT_SCOPE)
        { }

        public CacheManager(string cacheKey, string cacheScope)
        {
            ValidateInputParams(cacheKey, cacheScope);
            CacheKey = cacheKey;
            CacheScope = cacheScope;
        }

        #endregion

        #region public/private properties

        public string CacheKey { get; private set; }
        public string CacheScope { get; private set; }

        #endregion

        #region instance methods

        /// <summary>
        /// Sets implemented instance into memory cache
        /// </summary>
        /// <returns></returns>
        public virtual bool Set()
        {
            return Set(GetInstance(), this.CacheKey, this.CacheScope, GetSlidingExpirationInMinutesFromConfiguration());
        }

        /// <summary>
        /// Sets implemented instance into memory cache
        /// </summary>
        /// <param name="slidingExpirationInMinutes"></param>
        /// <returns></returns>
        public virtual bool Set(int slidingExpirationInMinutes)
        {
            return Set(GetInstance(), this.CacheKey, this.CacheScope, slidingExpirationInMinutes);
        }

        /// <summary>
        /// removes implemented instance from memory cache
        /// </summary>
        public virtual void Remove()
        {
            Remove(CacheKey, CacheScope);
        }

        /// <summary>
        /// gets instance of implemented class
        /// </summary>
        /// <returns></returns>
        protected abstract T GetInstance();


        #endregion

        #region static methods

        /// <summary>
        /// Gets and validates T object instance from memory cache
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static T Get(string cacheKey)
        {
            return Get(cacheKey, DEFAULT_SCOPE);
        }

        /// <summary>
        /// removes an item from cache using a key
        /// </summary>
        /// <param name="cacheKey"></param>
        public static void Remove(string cacheKey)
        {
            Remove(cacheKey, DEFAULT_SCOPE);
        }


        /// <summary>
        /// removes an item from cache using a key + scope
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheScope"></param>
        public static void Remove(string cacheKey, string cacheScope)
        {
            MemoryCache.Default.Remove(GetCombinedKey(cacheKey, cacheScope));
        }


        /// <summary>
        /// removes all items from memory cache
        /// </summary>
        public static void ClearCache()
        {
            foreach (var element in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(element.Key);
            }
        }


        /// <summary>
        /// Gets and validates T object instance from memory cache
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheScope"></param>
        /// <returns></returns>
        public static T Get(string cacheKey, string cacheScope)
        {
            T retValue = default(T);
            Object cachedObject = MemoryCache.Default.Get(GetCombinedKey(cacheKey, cacheScope));
            if (cachedObject != null)
            {
                if (cachedObject is T)
                {
                    retValue = (T)cachedObject;
                }
                else
                {
                    // additional type casting validation
                    try
                    {
                        retValue = (T)Convert.ChangeType(cachedObject, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        throw new InvalidCastException();
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Sets T object instance into memory cache
        /// </summary>
        /// <param name="objInstance"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheScope"></param>
        /// <param name="slidingExpirationInMinutes"></param>
        /// <returns></returns>
        public static bool Set(T objInstance, string cacheKey, string cacheScope, int slidingExpirationInMinutes)
        {
            if (objInstance == null) throw new ArgumentNullException("objInstance");
            ValidateInputParams(cacheKey, cacheScope);
            CacheItemPolicy cip = new CacheItemPolicy();
            slidingExpirationInMinutes = (slidingExpirationInMinutes <= 0) ?
                                        GetSlidingExpirationInMinutesFromConfiguration() :
                                        slidingExpirationInMinutes;
            cip.SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationInMinutes);
            ValidateUnsuportedUseCase(objInstance, cacheKey, cacheScope);
            MemoryCache.Default.Set(GetCombinedKey(cacheKey, cacheScope), objInstance, cip);
            return true;
        }

        /// <summary>
        /// Sets T object instance into memory cache
        /// </summary>
        /// <param name="objInstance"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheScope"></param>
        /// <returns></returns>
        public static bool Set(T objInstance, string cacheKey, string cacheScope)
        {
            return Set(objInstance, cacheKey, cacheScope, GetSlidingExpirationInMinutesFromConfiguration());
        }

        /// <summary>
        /// Sets T object instance into memory cache
        /// </summary>
        /// <param name="objInstance"></param>
        /// <param name="cacheKey"></param>
        /// <param name="slidingExpirationInMinutes"></param>
        /// <returns></returns>
        public static bool Set(T objInstance, string cacheKey, int slidingExpirationInMinutes)
        {
            return Set(objInstance, cacheKey, DEFAULT_SCOPE, slidingExpirationInMinutes);
        }

        /// <summary>
        /// Sets T object instance into memory cache
        /// </summary>
        /// <param name="objInstance"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static bool Set(T objInstance, string cacheKey)
        {
            return Set(objInstance, cacheKey, DEFAULT_SCOPE, GetSlidingExpirationInMinutesFromConfiguration());
        }

        private static void ValidateInputParams(string cacheKey, string cacheScope)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentNullException("cacheKey");
            if (string.IsNullOrWhiteSpace(cacheScope)) throw new ArgumentNullException("cacheScope");
        }

        private static string GetCombinedKey(string cacheKey, string cacheScope)
        {
            return cacheScope + "." + cacheKey;
        }

        /// <summary>
        /// Validates the proper use of Set methods.
        /// </summary>
        /// <param name="objInstance"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheScope"></param>
        private static void ValidateUnsuportedUseCase(T objInstance, string cacheKey, string cacheScope)
        {
            // NOTE: although a corner case but it is possible to build an instance with one set of key/scope values, and use
            //  static Set method of CacheManager<> class to pass that instance along with different set of key/scope values
            //  as parameters. This may cause unpredictable behaviour - below validation blocks such usage.

            if (objInstance is CacheManager<T>)
            {
                CacheManager<T> cm = objInstance as CacheManager<T>;
                if (cacheKey != cm.CacheKey || cacheScope != cm.CacheScope)
                {
                    throw new Exception("The provided object instance implements/inherits from CacheManager<>. This is an unsupported use of CacheManager<> class, please use Set method of derived class instance.");
                }
            }
        }

        /// <summary>
        /// Retrives sliding expiration in minutes from config file
        /// </summary>
        /// <returns></returns>
        private static int GetSlidingExpirationInMinutesFromConfiguration()
        {
            int retValue = 0;
            try
            {
                //retValue = ConfigurationManager.AppSettings["MemoryCacheSlidingExpirationInMinutes"] != null ? int.Parse(ConfigurationManager.AppSettings["MemoryCacheSlidingExpirationInMinutes"].ToString()) : DEFAULT_SLIDING_EXPIRATION_IN_MINUTES;
                var configValue = "20";
                //    _configuration["AppSettings:MemoryCacheSlidingExpirationInMinutes"];
                //if (!string.IsNullOrEmpty(configValue))
                //{
                //    retValue = int.Parse(configValue);
                //}
            }
            catch
            {
                // if config does not exist use default expiration
                retValue = DEFAULT_SLIDING_EXPIRATION_IN_MINUTES;
            }
            return retValue;
        }

        #endregion
    }
}
