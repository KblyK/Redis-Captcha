using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class RedisConnectionFactory
    {
        static RedisConnectionFactory()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("localhost:6379");//redis server conn string bilgisi, web config'den almak daha doğru ancak şimdilik buraya yazdık
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection => lazyConnection.Value;

        public static void DisposeConnection()
        {
            if (lazyConnection.Value.IsConnected)
                lazyConnection.Value.Dispose();
        }
    }
}
