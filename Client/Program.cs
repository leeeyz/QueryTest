﻿using MyHelper.Common;
using MyHelper.Common.MQ;
using MyHelper.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SubscribeInfo subscribeInfo = new SubscribeInfo(10);

            //subscribeInfo.Subscribe(mqmessage =>
            //{
            //    Console.WriteLine("处理日志{0}", mqmessage.Msg);
            //});

            //subscribeInfo.Subscribe(mqmessage =>
            //{
            //    Thread.Sleep(10 * 1000);
            //    Console.WriteLine("处理日志{0}", mqmessage.Msg);
            //});

            subscribeInfo.Subscribe(async mqmessage =>
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(10 * 1000);
                });
                Console.WriteLine("处理日志{0}", mqmessage.Msg);
            });

            Console.ReadKey();
        }
    }
}