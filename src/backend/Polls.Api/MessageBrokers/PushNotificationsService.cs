using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Polls.Api.MessageBrokers
{
    public class PushNotificationsService
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        private readonly IModel _PublishChannel;

        public delegate void UserBrokerMessageToDelegate(BrokerMessage message, string routingKey);
        public static event UserBrokerMessageToDelegate UserMessageDelegate;

        public PushNotificationsService(IOptions<RabbitMqSettings> rabbitMqConnection,
                                           IServiceProvider serviceProvider)
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
            _PublishChannel = connection.CreateModel();

            if (_rabbitMqSettings.UsersNotificationsExchangeName != null)
            {
                _PublishChannel.ExchangeDeclare(
                    exchange: _rabbitMqSettings.UsersNotificationsExchangeName,
                    type: ExchangeType.Direct);

                //declare queues
            }

            UserMessageDelegate += PublishMessage;
        }

        public void PublishMessage<T>(T message, string routingKey = "*")
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            var key = $"{_rabbitMqSettings.UsersNotificationsExchangeName}"; // -{routingKey.ToUpperInvariant()}";

            var properties = _PublishChannel.CreateBasicProperties();
            properties.Persistent = true;

            _PublishChannel.BasicPublish(
                exchange: _rabbitMqSettings.UsersNotificationsExchangeName,
                routingKey: key,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(serializedMessage));
        }

        public static void SendUserMessage(BrokerMessage message, string routingKey)
        {
            UserMessageDelegate?.Invoke(message, routingKey);
        }
    }
}
