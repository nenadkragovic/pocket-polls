using Polls.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Polls.Api.DTO
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
        public QuestionType QuestionType { get; set; }
        public ICollection<CreateChoiceDto>? Choices { get; set; }
    }

    public class CreateChoiceDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
