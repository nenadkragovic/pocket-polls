using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.DTO
{
    public class AddAnswersDto
    {
        public ICollection<AddYesNoAnswerDto> YesNoAnswers { get; set; }
        public ICollection<AddTextAnswerDto> TextAnswers { get; set; }
        public ICollection<AddSingleChoiceAnswerDto> SingleChoiceAnswers { get; set; }
        public ICollection<AddMultipleChoiceAnswerDto> MultipleChoiceAnswers { get; set; }
    }

    public class AddYesNoAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public bool Answer { get; set; }
    }

    public class AddTextAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public string Answer { get; set; }
    }

    public class AddSingleChoiceAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public long ChoiceId { get; set; }
    }

    public class AddMultipleChoiceAnswerDto
    {
        [Required]
        public long QuestionId { get; set; }
        [Required]
        public ICollection<long> ChoiceIds { get; set; }
    }

}
