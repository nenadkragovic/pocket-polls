namespace Polls.Lib.MessageBrokers
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string NotificationsExchangeName { get; set; }

        public bool ConsumeMessages { get; set; }
    }
}
