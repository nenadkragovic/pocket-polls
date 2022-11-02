using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.Database.Models
{
    public class PushNotificationSubscription
    {
        [Key]
        public long Id { get; set; }
        public string Endpoint { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string P246dhKey { get; set; }
        public string AuthKey { get; set; }
    }
}