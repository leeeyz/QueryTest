using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Common.MQ
{
    public class MQHelper
    {
        public void Subscribe(string subscriptionId, Action<MQMessage> onMessage)
        {
            MQConnectionHelp.Instance.Subscribe<MQMessage>(subscriptionId, onMessage);
        }

        public void Subscribe(string subscriptionId, Func<MQMessage, Task> onMessage)
        {
            MQConnectionHelp.Instance.SubscribeAsync<MQMessage>(subscriptionId, onMessage);
        }

        public void Publish(MQMessage message)
        {
            MQConnectionHelp.Instance.Publish<MQMessage>(message);
        }

        public void Dispose()
        {
            MQConnectionHelp.Instance.Dispose();
        }
    }
}
