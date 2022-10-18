using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.DTO
{
    public class AddAnswersDto
    {
        public ICollection<YesNoAnswerDto> YesNoAnswers { get; set; }
        public ICollection<TextAnswerDto> TextAnswers { get; set; }
        public ICollection<SingleChoiceAnswerDto> SingleChoiceAnswers { get; set; }
        public ICollection<MultipleChoiceAnswerDto> MultipleChoiceAnswer { get; set; }
    }

    public class YesNoAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public bool Answer { get; set; }
    }

    public class TextAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public string Answer { get; set; }
    }

    public class SingleChoiceAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public long ChoiceId { get; set; }
    }

    public class MultipleChoiceAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public ICollection<long> ChoiceIds { get; set; }
    }

}
