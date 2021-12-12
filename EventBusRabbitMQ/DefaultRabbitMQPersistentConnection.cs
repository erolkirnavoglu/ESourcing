using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace EventBusRabbitMQ
{
  public  class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private readonly int _retryCount;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private bool _disposed;
        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory,int retryCount, ILogger<DefaultRabbitMQPersistentConnection> logger)
        {
            _connectionFactory = connectionFactory;
            _retryCount = retryCount;
            _logger = logger;
        }
        public bool IsConnection
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

     

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ client is trying to connect");
            var policy = RetryPolicy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                     {
                         _logger.LogWarning(ex,"RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",$"{time.TotalSeconds:n1}",ex.Message);
                     });
            policy.Execute(()=> 
            {
                _connection = _connectionFactory.CreateConnection();
            });
            if (IsConnection)
            {
                _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                _connection.CallbackException += _connection_CallbackException;
                _connection.ConnectionBlocked += _connection_ConnectionBlocked;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to {HostName} and is subscribed to failure events",_connection);
                return true;
            }
            else
            {
                _logger.LogCritical("Fatal error:RabbitMQ connections could not be created and openned");
                return false;
            }
        }

        private void _connection_ConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        private void _connection_CallbackException(object sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        public IModel CreateModel()
        {
            if (!IsConnection)
            {
                throw new InvalidOperationException("No RabbitMQ connections are availbis to perform this action");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                _connection.Dispose();
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
            }
        }
    }
}
