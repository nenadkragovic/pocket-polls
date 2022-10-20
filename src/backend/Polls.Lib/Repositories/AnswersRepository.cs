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


        public async Task AddAnswers(Guid userId, long pollId, AddAnswersDto model)
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
        }

        public async Task<GetAnswersDto> GetAnswersByPollId(long pollId)
        {
            var result = new GetAnswersDto();

            result.YesNoAnswers = await _context.YesNoAnswers
                .Where(a => a.Question.PollId == pollId).ToListAsync();
            result.SingleChoiceAnswers = await _context.SingleChoiceAnswers
                .Include(c => c.SingleChoice)
                .Where(a => a.Question.PollId == pollId).ToListAsync();
            result.MultipleChoiceAnswers = await _context.MultipleChoiceAnswers
                .Include(c => c.MultipleChoiceQuestionsAnswers)
                    .ThenInclude(c => c.MultipleChoiceOption)
                .Where(a => a.Question.PollId == pollId).ToListAsync();
            result.TextAnswers = await _context.TextAnswers
                .Where(a => a.Question.PollId == pollId).ToListAsync();

            return result;
        }

        public async Task<GetAnswersDto> GetAnswersByPollIdForUser(long pollId, Guid userId)
        {
            var result = new GetAnswersDto();

            result.YesNoAnswers = await _context.YesNoAnswers
                .Where(a => a.Question.PollId == pollId && a.UserId == userId).ToListAsync();
            result.SingleChoiceAnswers = await _context.SingleChoiceAnswers
                .Include(c => c.SingleChoice)
                .Where(a => a.Question.PollId == pollId && a.UserId == userId).ToListAsync();
            result.MultipleChoiceAnswers = await _context.MultipleChoiceAnswers
                .Include(c => c.MultipleChoiceQuestionsAnswers)
                    .ThenInclude(c => c.MultipleChoiceOption)
                .Where(a => a.Question.PollId == pollId && a.UserId == userId).ToListAsync();
            result.TextAnswers = await _context.TextAnswers
                .Where(a => a.Question.PollId == pollId && a.UserId == userId).ToListAsync();

            return result;
        }

        public async Task<GetAnswersDto> GetAnswersByPollIdForUsers(long pollId, ICollection<Guid> userIds)
        {
            var result = new GetAnswersDto();

            result.YesNoAnswers = await _context.YesNoAnswers
                .Where(a => a.Question.PollId == pollId && userIds.Contains(a.UserId)).ToListAsync();
            result.SingleChoiceAnswers = await _context.SingleChoiceAnswers
                .Include(c => c.SingleChoice)
                .Where(a => a.Question.PollId == pollId && userIds.Contains(a.UserId)).ToListAsync();
            result.MultipleChoiceAnswers = await _context.MultipleChoiceAnswers
                .Include(c => c.MultipleChoiceQuestionsAnswers)
                    .ThenInclude(c => c.MultipleChoiceOption)
                .Where(a => a.Question.PollId == pollId && userIds.Contains(a.UserId)).ToListAsync();
            result.TextAnswers = await _context.TextAnswers
                .Where(a => a.Question.PollId == pollId && userIds.Contains(a.UserId)).ToListAsync();

            return result;
        }
    }
}
