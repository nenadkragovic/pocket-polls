using Polls.Lib.Database.Models;

namespace Polls.Lib.DTO
{
    public class GetAnswersDto
    {
        public virtual ICollection<YesNoAnswer> YesNoAnswers { get; set; }
        public virtual ICollection<SingleChoiceAnswer> SingleChoiceAnswers { get; set; }
        public virtual ICollection<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
        public virtual ICollection<TextAnswer> TextAnswers { get; set; }
    }
}
