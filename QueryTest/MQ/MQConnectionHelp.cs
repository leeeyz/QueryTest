﻿using EasyNetQ;
using EasyNetQ.Consumer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Common.MQ
{
    public static class MQConnectionHelp
    {
        private static readonly string MQConnectionString = ConfigurationManager.ConnectionStrings["MQConnection"].ConnectionString;

        private static IBus _instance;
        private static readonly object Locker = new object();

        /// <summary>
        /// 单例获取
        /// </summary>
        public static IBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = GetManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private static IBus GetManager()
        {
            return RabbitHutch.CreateBus(MQConnectionString, serviceRegister => serviceRegister.Register<IConsumerErrorStrategy, DeadLetterStrategy>());
        }

        public static void Dispose()
        {
            _instance.Dispose();
        }
    }

    public class DeadLetterStrategy : DefaultConsumerErrorStrategy
    {
        public DeadLetterStrategy(IConnectionFactory connectionFactory, ISerializer serializer, IConventions conventions, ITypeNameSerializer typeNameSerializer, IErrorMessageSerializer errorMessageSerializer)
        : base(connectionFactory, serializer, conventions, typeNameSerializer, errorMessageSerializer)
        {
        }


        public override AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
        {
            object deathHeaderObject;
            if (!context.Properties.Headers.TryGetValue("x-death", out deathHeaderObject))
                return AckStrategies.NackWithoutRequeue;

            var deathHeaders = deathHeaderObject as IList;

            if (deathHeaders == null)
                return AckStrategies.NackWithoutRequeue;

            var retries = 0;
            foreach (IDictionary header in deathHeaders)
            {
                var count = int.Parse(header["count"].ToString());
                retries += count;
            }

            if (retries < 3)
                return AckStrategies.NackWithoutRequeue;
            return base.HandleConsumerError(context, exception);
        }
    }
}
