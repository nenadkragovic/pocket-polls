namespace Polls.Lib.DTO
{
    public class UserValidationResult
    {
        public bool Authorized { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string ValidationMessage { get; set; }
    }
}
