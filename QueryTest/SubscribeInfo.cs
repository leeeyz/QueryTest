﻿using MyHelper.Common.MQ;
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

        string mqChannel = "mqChannel";
        string keyname = "key:{0}";
        string sumkeyname = "sumkey";
        int limit;

        public SubscribeInfo(int limit)
        {
            redis = new RedisHelper(1);
            this.limit = limit;
        }

        public void Subscribe(Action<MQMessage> action)
        {
            MQHelper.Instance.Subscribe(mqChannel, action);
        }

        public void Subscribe(Func<MQMessage, Task> func)
        {
            MQHelper.Instance.SubscribeAsync(mqChannel, func);
        }

        public void ErrorHandle(Action<dynamic, Exception> act)
        {
            MQHelper.ErrorHandle(act);
        }

        public void ErrorHandle(Func<dynamic, Exception, Task> func)
        {
            MQHelper.ErrorHandle(func);
        }

        public bool Incr(string id)
        {
            id = string.Format(keyname, id);
            if (redis.StringIncrement(id) == 1)
            {
                if (redis.StringIncrement(sumkeyname) <= limit)
                {
                    MQHelper.Instance.Publish(new MQMessage() { Msg = id });
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            redis.Flush();
            //MQHelper.Instance.Dispose();
        }
    }
}
