using MyHelper.Common;
using MyHelper.Common.MQ;
using MyHelper.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            SubscribeInfo subscribeInfo = new SubscribeInfo(10);

            Parallel.For(0, 99, i =>
            {
                int re = subscribeInfo.Incr(i.ToString());
                Console.WriteLine("{0},{1},{2}", i, re, subscribeInfo.IsFulled());
            });

            subscribeInfo.RedisFlush();
            Console.ReadLine();
        }
    }
}
