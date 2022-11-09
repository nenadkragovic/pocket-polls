namespace Polls.Lib.MessageBrokers
{
    public class BrokerMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public bool SendToAll { get; set; } = true;
    }
}
