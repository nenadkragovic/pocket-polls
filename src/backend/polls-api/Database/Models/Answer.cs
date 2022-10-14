using Polls.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Polls.Api.Database.Models
{
    public abstract class Answer
    {
        [Key]
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }
    }

    public class YesNoAnswer : Answer
    {
        public YesNoQuestion Question { get; set; }
        public bool Answer { get; set; }
    }

    public class SingleChoiceAnswer : Answer
    {
        public long ChoiceId { get; set; }

        public SingleChoiceQuestion Question { get; set; }
        public SingleChoice SingleChoice { get; set; }
    }

    public class MultipleChoiceAnswer : Answer
    {
        public MultipleChoiceQuestion Question { get; set; }
        public ICollection<MultipleChoiceQuestionsAnswer> MultipleChoiceQuestionsAnswers { get; set; }
    }

    public class TextAnswer : Answer
    {
        public TextQuestion Question { get; set; }
        public string Answer { get; set; }
    }
}
