﻿using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;
using MediatR;
using Newtonsoft.Json;
using Ordering.Application.Commands.OrderCreate;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESourcing.Order.Consumers
{
    public class EventBusOrderCreateConsumer
    {
        private readonly IRabbitMQPersistentConnection _rabbitMQPersistentConnection;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public EventBusOrderCreateConsumer(IRabbitMQPersistentConnection rabbitMQPersistentConnection, IMediator mediator, IMapper mapper)
        {
            _rabbitMQPersistentConnection = rabbitMQPersistentConnection;
            _mediator = mediator;
            _mapper = mapper;
        }

        public void Consume()
        {
            if(!_rabbitMQPersistentConnection.IsConnection)
            {
                _rabbitMQPersistentConnection.TryConnect();
            }

            var channel = _rabbitMQPersistentConnection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.OrderCreateQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue:EventBusConstants.OrderCreateQueue,autoAck:true,consumer:consumer);
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);
            var @event = JsonConvert.DeserializeObject<OrderCreateEvent>(message);

            if (e.RoutingKey == EventBusConstants.OrderCreateQueue)
            {
                var command = _mapper.Map<OrderCreateCommand>(@event);
                command.CreateAt = DateTime.Now;
                command.TotalPrice = @event.Quantity * @event.Price;
                command.UnitPrice = @event.Price;

                var result = await _mediator.Send(command);
            }
        }
        public void Disconnect()
        {
            _rabbitMQPersistentConnection.Dispose();
        }
    }
}
