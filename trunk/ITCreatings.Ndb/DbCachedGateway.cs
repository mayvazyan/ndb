using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace ITCreatings.Ndb
{
    /// <summary>
    /// Work with databaase using caching technologies
    /// </summary>
    public class DbCachedGateway
    {
        private static MemcachedClient cache;

        /// <summary>
        /// Provides access to MemcachedClient
        /// </summary>
        public static MemcachedClient Cache
        {
            get
            {
                //TODO: add multithread support?
                if (cache == null)
                    cache = new MemcachedClient();

                return cache;
            }
        }

        /// <summary>
        /// Underlayed gateway
        /// </summary>
        public DbGateway Gateway { get; set; }

        /// <summary>
        /// Creates new instance and set Gateway property
        /// </summary>
        /// <param name="gateway"></param>
        public DbCachedGateway(DbGateway gateway)
        {
            Gateway = gateway;
        }

        /// <summary>
        /// Loads list from database or cache
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="key">Key in cache</param>
        /// <param name="args">Filter</param>
        /// <returns>List</returns>
        public T[] LoadList<T>(string key, params object[] args) where T : new()
        {
            T[] result = Cache.Get(key) as T[];

            if (result == null)
            {
                result = Gateway.LoadList<T>(args);
                Cache.Store(StoreMode.Add, key, result);
            }

            return result;
        }
    }
}