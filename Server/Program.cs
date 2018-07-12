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
            SubscribeInfo subscribeInfo = new SubscribeInfo(1);

            Parallel.For(0, 99, i =>
            {
                bool re = subscribeInfo.Incr(i.ToString());
                Console.WriteLine("{0},{1}", i, re);
            });

            subscribeInfo.Clear();
            Console.ReadLine();
        }
    }
}
