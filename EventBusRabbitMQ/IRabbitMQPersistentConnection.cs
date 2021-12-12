using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ
{
   public interface IRabbitMQPersistentConnection:IDisposable
    {
        public bool IsConnection { get; }
        public bool TryConnect();
        public IModel CreateModel();
    }
}
