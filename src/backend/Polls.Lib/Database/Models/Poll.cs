using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.Database.Models
{
    public class Poll
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Owner Id
        /// </summary>
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<YesNoQuestion> YesNoQuestions { get; set; }
        public virtual ICollection<SingleChoiceQuestion> SingleChoiceQuestions { get; set; }
        public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public virtual ICollection<TextQuestion> TextQuestions { get; set; }
    }
}
