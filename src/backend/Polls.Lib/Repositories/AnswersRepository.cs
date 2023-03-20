using Microsoft.EntityFrameworkCore;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;

namespace Polls.Lib.Repositories
{
    public class AnswersRepository
    {
        public readonly Context _context;

        public AnswersRepository(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Add anser
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pollId"></param>
        /// <param name="model"></param>
        /// <returns>Returns owner ID</returns>
        public async Task<Guid> AddAnswers(Guid userId, long pollId, AddAnswersDto model)
        {

            _context.YesNoAnswers.AddRange(model.YesNoAnswers.Select(a => new YesNoAnswer()
            {
                UserId = userId,
                QuestionId = a.QuestionId,
                QuestionType = Enums.QuestionType.YesNoAnswer,
                Answer = a.Answer,
            }));

            _context.SingleChoiceAnswers.AddRange(model.SingleChoiceAnswers.Select(a => new SingleChoiceAnswer()
            {
                UserId = userId,
                QuestionId = a.QuestionId,
                QuestionType = Enums.QuestionType.SingleChoice,
                SingleChoiceId = a.ChoiceId
            }));

            _context.MultipleChoiceAnswers.AddRange(model.MultipleChoiceAnswers.Select(a => new MultipleChoiceAnswer()
            {
                UserId = userId,
                QuestionId = a.QuestionId,
                QuestionType = Enums.QuestionType.MultipleChoice,
                MultipleChoiceQuestionsAnswers = a.ChoiceIds.Select(id => new MultipleChoiceQuestionsAnswer()
                {
                    MultipleChoiceOptionId = id,
                }).ToList()
            }));

            _context.TextAnswers.AddRange(model.TextAnswers.Select(a => new TextAnswer()
            {
                UserId = userId,
                QuestionId = a.QuestionId,
                QuestionType = Enums.QuestionType.TextAnswer,
                Answer = a.Answer
            }));

            await _context.SaveChangesAsync();

            var ownerId = await _context.Polls.AsNoTracking()
                .Where(p => p.Id == pollId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();

            return ownerId;
        }

        public async Task<GetAnswersDto> GetAnswersByPollId(long pollId)
        {
            var result = new GetAnswersDto();

            result.YesNoAnswers = await _context.YesNoQuestions
                .AsNoTracking()
                .Where(q => q.PollId == pollId)
                .Select(q => new YesNoAnswerDto()
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    YesCount = q.YesNoAnswers.Where(a => a.Answer).Count(),
                    Total = q.YesNoAnswers.Count()
                })
                .ToListAsync();

            result.SingleChoiceAnswers = await _context.SingleChoiceQuestions
                .AsNoTracking()
                .Where(q => q.PollId == pollId)
                .Select(q => new SingleMultiAnswersDto()
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    Total = q.SingleChoiceAnswers.Count(),
                    Choices = q.Choices.Select(c => new ChoiceAnswersDto()
                    {
                        ChoiceId = c.Id,
                        ChoiceName = c.Name,
                        Total = c.SingleChoiceAnswers.Count()
                    }).ToList()
                })
                .ToListAsync();

            result.MultipleChoiceAnswers = await _context.MultipleChoiceQuestions
                .AsNoTracking()
                .Where(q => q.PollId == pollId)
                .Select(q => new SingleMultiAnswersDto()
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    Total = q.MultipleChoiceAnswer.Count(),
                    Choices = q.Choices.Select(c => new ChoiceAnswersDto()
                    {
                        ChoiceId = c.Id,
                        ChoiceName = c.Name,
                        Total = c.MultipleChoiceQuestionsAnswers.Count()
                    }).ToList()
                })
                .ToListAsync();

            result.TextAnswers = await _context.TextQuestions
                .AsNoTracking()
                .Where(q => q.PollId == pollId)
                .Select(q => new TextAnswersDto()
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    Total = q.TextAnswers.Count(),
                    Answers = q.TextAnswers.Select(a => a.Answer).ToList()
                })
                .ToListAsync();

            return result;
        }
    }
}
