using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.RedisStore
{
    public class RedisStoreOptions
    {
        /// <summary>
        /// ConnectionString to a Redis Instance
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
