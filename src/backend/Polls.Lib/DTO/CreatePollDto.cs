using Polls.Lib.Attributes;
using Polls.Lib.Enums;
using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.DTO
{
    public class CreatePollDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<CreateQuestionDto> Questions { get; set; }

    }

    public class CreateQuestionDto
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public QuestionType QuestionType { get; set; }

        [RequiredIfQuestionTypeIsChoice]
        public ICollection<CreateChoiceDto>? Choices { get; set; }
    }

    public class CreateChoiceDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
