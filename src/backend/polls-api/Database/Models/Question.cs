using Polls.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Polls.Api.Database.Models
{
    public abstract class Question
    {
        [Key]
        public long Id { get; set; }
        public long PollId { get; set; }
        public string QuestionText { get; set; }
        public QuestionType QuestionType { get; set; }

        public Poll Poll { get; set; }
    }

    public class YesNoQuestion : Question
    {
        public ICollection<YesNoAnswer> YesNoAnswers { get; set; }
    }

    public class SingleChoiceQuestion : Question
    {
        public ICollection<SingleChoiceAnswer> SingleChoiceAnswers { get; set; }
        public ICollection<SingleChoice> Choices { get; set; }
    }

    public class MultipleChoiceQuestion : Question
    {
        public ICollection<MultipleChoiceAnswer> MultipleChoiceAnswer { get; set; }
        public ICollection<MultipleChoice> Choices { get; set; }
    }

    public class TextQuestion : Question
    {
        public ICollection<TextAnswer> TextAnswers { get; set; }
    }
}
