using EasyNetQ;
using System;
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
            return RabbitHutch.CreateBus(MQConnectionString);
        }

        public static void Dispose()
        {
            _instance.Dispose();
        }
    }
}
