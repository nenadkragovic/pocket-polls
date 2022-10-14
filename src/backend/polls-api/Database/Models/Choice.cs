using System.ComponentModel.DataAnnotations;

namespace Polls.Api.Database.Models
{
    public class Choice
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long QuestionId { get; set; }
    }

    public class SingleChoice : Choice
    {
        public SingleChoiceQuestion SingleChoiceQuestion { get; set; }
    }

    public class MultipleChoice : Choice
    {
        public MultipleChoiceQuestion MultipleChoiceQuestion { get; set; }
        public ICollection<MultipleChoiceQuestionsAnswer> MultipleChoiceQuestionsAnswers { get; set; }
    }

    public class MultipleChoiceQuestionsAnswer
    {
        [Key]
        public long Id { get; set; }
        public long AnswerId { get; set; }
        public long MultipleChoiceId { get; set; }

        public MultipleChoiceAnswer Anwser { get; set; }
        public MultipleChoice MultipleChoice { get; set; }
    }
}
