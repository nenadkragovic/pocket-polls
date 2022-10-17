namespace Polls.Lib.DTO
{
    public class ListResult<T> where T : class
    {
        public long TotalRecords { get; set; }
        public ICollection<T> Records { get; set; }
    }
}
