using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Polls.Lib.MessageBrokers
{
    public class PushNotificationsBroker
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        private readonly IModel _publishChannel;

        public delegate void BrokerMessageDelegate(BrokerMessage message);
        public event BrokerMessageDelegate OnMessageReceived;

        public PushNotificationsBroker(IOptions<RabbitMqSettings> rabbitMqConnection)
        {
            _rabbitMqSettings = rabbitMqConnection.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.HostName,
                Port = _rabbitMqSettings.Port,
                UserName = _rabbitMqSettings.Username,
                Password = _rabbitMqSettings.Password
            };

            var connection = factory.CreateConnection();
            _publishChannel = connection.CreateModel();

            if (_rabbitMqSettings.NotificationsExchangeName != null)
            {
                _publishChannel.QueueDeclare(queue: _rabbitMqSettings.NotificationsExchangeName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                _publishChannel.ExchangeDeclare(exchange: _rabbitMqSettings.NotificationsExchangeName,
                                                type: ExchangeType.Direct);

                _publishChannel.QueueBind(queue: _rabbitMqSettings.NotificationsExchangeName,
                                          exchange: _rabbitMqSettings.NotificationsExchangeName,
                                          routingKey: _rabbitMqSettings.NotificationsExchangeName);

                if (_rabbitMqSettings.ConsumeMessages)
                {
                    var consumer = new EventingBasicConsumer(_publishChannel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var brokerMessage = JsonConvert.DeserializeObject<BrokerMessage>(message);
                        OnMessageReceived?.Invoke(brokerMessage);
                    };

                    _publishChannel.BasicConsume(queue: _rabbitMqSettings.NotificationsExchangeName,
                                                 autoAck: true,
                                                 consumer: consumer);
                }

            }
        }

        public void PublishMessage<T>(T message)
        {
            Console.WriteLine($"Sending message: {message}");

            var serializedMessage = JsonConvert.SerializeObject(message);
            var key = $"{_rabbitMqSettings.NotificationsExchangeName}";

            var properties = _publishChannel.CreateBasicProperties();
            properties.Persistent = true;

            _publishChannel.BasicPublish(
                exchange: _rabbitMqSettings.NotificationsExchangeName,
                routingKey: key,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(serializedMessage));
        }
    }
}
