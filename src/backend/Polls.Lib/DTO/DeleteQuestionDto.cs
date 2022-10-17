using Polls.Lib.Enums;
using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.DTO
{
    public class DeleteQuestionDto
    {
        [Required]
        public QuestionType QuestionType { get; set; }
        [Required]
        public ICollection<long> QuestionIds { get; set; }
    }
}
