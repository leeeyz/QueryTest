using MyHelper.Common.MQ;
using MyHelper.Common.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Common
{
    public class SubscribeInfo
    {
        RedisHelper redis;

        string mqChannel = "mqChannel";
        string listkeyname = "listkey";
        int limit;
        private static readonly object Locker = new object();

        public SubscribeInfo(int limit)
        {
            if (redis == null)
            {
                lock (Locker)
                {
                    redis = new RedisHelper(1);
                    this.limit = limit;
                }
            }
        }

        public void Subscribe(Action<MQMessage> action)
        {
            if (action != null)
                MQHelper.Instance.Subscribe(mqChannel, action);
        }

        public void Subscribe(Func<MQMessage, Task> func)
        {
            if (func != null)
                MQHelper.Instance.SubscribeAsync(mqChannel, func);
        }

        public void ErrorHandle(Action<dynamic, Exception, Type> action)
        {
            MQHelper.ErrorHandle(action);
        }

        public void ErrorHandle(Func<dynamic, Exception, Type, Task> func)
        {
            MQHelper.ErrorHandle(func);
        }

        public int Incr(string id)
        {
            var trans = redis.CreateTransaction();
            trans.AddCondition(Condition.ListLengthLessThan(listkeyname, limit));
            trans.AddCondition(Condition.KeyNotExists(id));
            trans.ListRightPushAsync(listkeyname, id);
            trans.StringIncrementAsync(id);
            bool result = trans.Execute();
            if(result)
            {
                MQHelper.Instance.Publish<MQMessage>(new MQMessage() { Msg = id });
                //成功
                return 0;
            }
            else
            {
                if (redis.KeyExists(id))
                {
                    //已在队列
                    return 1;
                }
                else
                {
                    //失败
                    return 2;
                }
            }
        }

        public bool IsFulled()
        {
            return redis.ListLength(listkeyname) == limit;
        }

        public void RedisFlush()
        {
            redis.Flush();
        }

        public void Dispose()
        {
            redis.Flush();
            MQHelper.Instance.Dispose();
        }
    }
}
