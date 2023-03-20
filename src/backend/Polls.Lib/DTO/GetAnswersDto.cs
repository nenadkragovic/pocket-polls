using Polls.Lib.Enums;

namespace Polls.Lib.DTO
{
    public class GetAnswersDto
    {
        public ICollection<YesNoAnswerDto> YesNoAnswers { get; set; }
        public ICollection<SingleMultiAnswersDto> SingleChoiceAnswers { get; set; }
        public ICollection<SingleMultiAnswersDto> MultipleChoiceAnswers { get; set; }
        public ICollection<TextAnswersDto> TextAnswers { get; set; }
    }

    public class QuestionAnswersDto
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Total { get; set; }
    }

    public class YesNoAnswerDto : QuestionAnswersDto
    {
        public int YesCount { get; set; }
        public int NoCount => Total - YesCount;
    }

    public class ChoiceAnswersDto
    {
        public long ChoiceId { get; set; }
        public string ChoiceName { get; set; }
        public int Total { get; set; }
    }

    public class SingleMultiAnswersDto : QuestionAnswersDto
    {
        public ICollection<ChoiceAnswersDto> Choices { get; set; }
    }

    public class TextAnswersDto : QuestionAnswersDto
    {
        public ICollection<string> Answers { get; set; }
        public long Total { get; set; }
    }
}
