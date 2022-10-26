namespace Polls.Lib.DTO
{
    public class ListPollsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbUrl { get; set; } = string.Empty;
        public int NumberOfQuestions { get; set; }
    }
}
