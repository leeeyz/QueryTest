using MyHelper.Common.MQ;
using MyHelper.Common.Redis;
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
        MQHelper mq;

        string mqChannel = "mqChannel";
        string keyname = "key:{0}";
        string sumkeyname = "sumkey";
        int limit;

        public SubscribeInfo(int limit)
        {
            redis = new RedisHelper(1);
            mq = new MQHelper();
            this.limit = limit;
        }

        public void Subscribe(Action<MQMessage> action)
        {
            if (action != null)
            {
                mq.Subscribe(mqChannel, action);
            }
        }

        public void Subscribe(Func<MQMessage, Task> func)
        {
            if (func != null)
            {
                mq.Subscribe(mqChannel, func);
            }
        }

        public bool Incr(string id)
        {
            id = string.Format(keyname, id);
            if (redis.StringIncrement(id) == 1)
            {
                if (redis.StringIncrement(sumkeyname) <= limit)
                {
                    mq.Publish(new MQMessage() { Msg = id });
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            redis.Flush();
            mq.Dispose();
        }
    }
}
