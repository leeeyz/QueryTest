using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.SystemMessages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Common.MQ
{
    public static class MQHelper
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
            var bus = RabbitHutch.CreateBus(MQConnectionString, serviceRegister => serviceRegister.Register<IConsumerErrorStrategy, DeadLetterStrategy>());
            return bus;
        }

        public static void ErrorHandle(Action<dynamic, Exception, Type> action)
        {
            string errorQueue = "EasyNetQ_Default_Error_Queue";
            var subscriptionErrorQueue = _instance.Advanced.QueueDeclare(errorQueue);
            var source = _instance.Advanced.ExchangeDeclare(errorQueue, "direct");
            _instance.Advanced.Bind(source, subscriptionErrorQueue, "#");
            _instance.Advanced.Consume<Error>(subscriptionErrorQueue, (error, info) => {

                try
                {

                    Type originalMsgType = null;

                    if (error.Body.BasicProperties.TypePresent)
                    {
                        var typeNameSerializer = _instance.Advanced.Container.Resolve<ITypeNameSerializer>();
                        originalMsgType = typeNameSerializer.DeSerialize(error.Body.BasicProperties.Type);
                    }
                    else
                    {

                    }

                    dynamic originalMessage = JsonConvert.DeserializeObject(error.Body.Message, originalMsgType);

                    Exception expc = error.Body.BasicProperties.Headers["expc"] as Exception;

                    if (action != null)
                        action(originalMessage, expc, originalMsgType);
                }
                catch(Exception ex)
                {

                }
            });
        }

        public static void ErrorHandle(Func<dynamic, Exception, Type, Task> func)
        {
            string errorQueue = "EasyNetQ_Default_Error_Queue";
            var subscriptionErrorQueue = _instance.Advanced.QueueDeclare(errorQueue);
            var source = _instance.Advanced.ExchangeDeclare(errorQueue, "direct");
            _instance.Advanced.Bind(source, subscriptionErrorQueue, "#");
            _instance.Advanced.Consume<Error>(subscriptionErrorQueue, async (error, info) =>
            {
                try
                {
                    Type originalMsgType = null;

                    if (error.Body.BasicProperties.TypePresent)
                    {
                        var typeNameSerializer = _instance.Advanced.Container.Resolve<ITypeNameSerializer>();
                        originalMsgType = typeNameSerializer.DeSerialize(error.Body.BasicProperties.Type);
                    }
                    else
                    {

                    }

                    dynamic originalMessage = JsonConvert.DeserializeObject(error.Body.Message, originalMsgType);

                    Exception expc = error.Body.BasicProperties.Headers["expc"] as Exception;

                    if (func != null)
                        await func(originalMessage, expc, originalMsgType);

                }
                catch (Exception ex)
                {

                }
            });
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
            context.Properties.Headers.Add("expc", exception);
            return base.HandleConsumerError(context, exception);
        }
    }
}
