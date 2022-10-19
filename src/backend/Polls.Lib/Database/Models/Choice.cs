using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.Database.Models
{
    public class ChoiceOption
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long QuestionId { get; set; }
    }

    public class SingleChoiceOption : ChoiceOption
    {
        public SingleChoiceQuestion SingleChoiceQuestion { get; set; }
        public ICollection<SingleChoiceAnswer> SingleChoiceAnswers { get; set; }
    }

    public class MultipleChoiceOption : ChoiceOption
    {
        public MultipleChoiceQuestion MultipleChoiceQuestion { get; set; }
        public ICollection<MultipleChoiceQuestionsAnswer> MultipleChoiceQuestionsAnswers { get; set; }
    }

    public class MultipleChoiceQuestionsAnswer
    {
        [Key]
        public long Id { get; set; }
        public long AnswerId { get; set; }
        public long MultipleChoiceOptionId { get; set; }

        public MultipleChoiceAnswer Anwser { get; set; }
        public MultipleChoiceOption MultipleChoiceOption { get; set; }
    }
}
