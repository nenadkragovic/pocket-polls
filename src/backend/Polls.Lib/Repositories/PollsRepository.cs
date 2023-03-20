using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Enums;
using Polls.Lib.Exceptions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Polls.Lib.Repositories
{
    public class PollsRepository
    {
        private readonly Context _context;

        public PollsRepository(Context context)
        {
            _context = context;
        }

        #region Public Methods
        public async Task<ListResult<ListPollsDto>> ListPolls(int offset, int limit, Guid userId, string searchParam = "")
        {
            var query = _context.Polls.AsNoTracking(); //.Where(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);

            if (userId != Guid.Empty)
            {
                query = query.Where(p => p.UserId == userId);
            }

            if (!string.IsNullOrEmpty(searchParam))
            {
                query = query.Where(p => p.Name!.ToLower().Contains(searchParam.Trim().ToLower()));
            }

            int total = await query.AsNoTracking().CountAsync();

            var records = await query
                .Skip(offset).Take(limit)
                .Select(p => new ListPollsDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ThumbUrl = p.ThumbUrl,
                    NumberOfQuestions =
                        p.MultipleChoiceQuestions.Count() +
                        p.SingleChoiceQuestions.Count() +
                        p.TextQuestions.Count() +
                        p.YesNoQuestions.Count()
                })
                .ToListAsync();

            return new ListResult<ListPollsDto>()
            {
                Records = records,
                TotalRecords = total
            };
        }

        public async Task<Poll?> GetPollById(long pollId, bool noTracking = true)
        {
            var query = _context.Polls.AsQueryable();

            if (noTracking) query = query.AsNoTracking();

            return await query
                .Include(p => p.YesNoQuestions)
                .Include(p => p.SingleChoiceQuestions)
                    .ThenInclude(q => q.Choices)
                .Include(p => p.MultipleChoiceQuestions)
                    .ThenInclude(q => q.Choices)
                .Include(p => p.TextQuestions)
                .AsNoTracking().FirstOrDefaultAsync(p => p.Id == pollId);
        }

        public async Task<Poll?> AddPoll(Guid userId, CreatePollDto model)
        {
            var record = new Poll()
            {
                UserId = userId,
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ThumbUrl = model.ThumbUrl
            };

            if (model.Questions != null)
            {
                AttachQuestions(record, model.Questions);
            }

            var poll = _context.Polls.Add(record);

            await _context.SaveChangesAsync();

            return poll.Entity;
        }

        public async Task AddQuestions(long pollId, ICollection<CreateQuestionDto> model)
        {
            foreach (var question in model)
            {
                switch (question.QuestionType)
                {
                    case QuestionType.YesNoAnswer:
                        _context.YesNoQuestions.Add(new YesNoQuestion()
                        {
                            QuestionText = question.Text,
                            QuestionType = question.QuestionType,
                            PollId = pollId
                        });
                        break;
                    case QuestionType.SingleChoice:
                        _context.SingleChoiceQuestions.Add(new SingleChoiceQuestion()
                        {
                            QuestionText = question.Text,
                            QuestionType = question.QuestionType,
                            PollId = pollId,
                            Choices = question.Choices?.Select(c => new SingleChoiceOption()
                            {
                                Name = c.Name,
                                Description = c.Description
                            }).ToList()
                        });
                        break;
                    case QuestionType.MultipleChoice:
                        _context.MultipleChoiceQuestions.Add(new MultipleChoiceQuestion()
                        {
                            QuestionText = question.Text,
                            QuestionType = question.QuestionType,
                            PollId = pollId,
                            Choices = question.Choices?.Select(c => new MultipleChoiceOption()
                            {
                                Name = c.Name,
                                Description = c.Description
                            }).ToList()
                        });
                        break;
                    case QuestionType.TextAnswer:
                        _context.TextQuestions.Add(new TextQuestion()
                        {
                            QuestionText = question.Text,
                            QuestionType = question.QuestionType,
                            PollId = pollId
                        });
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestions(long pollId, ICollection<DeleteQuestionDto> model)
        {
            foreach (var questions in model)
            {
                switch (questions.QuestionType)
                {
                    case QuestionType.YesNoAnswer:
                        _context.YesNoAnswers.RemoveRange(await _context.YesNoAnswers.Where(a => questions.QuestionIds.Contains(a.QuestionId)).ToListAsync());
                        _context.YesNoQuestions.RemoveRange(await _context.YesNoQuestions.Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id)).ToListAsync());
                        break;
                    case QuestionType.SingleChoice:
                        _context.SingleChoiceAnswers.RemoveRange(await _context.SingleChoiceAnswers.Where(a => questions.QuestionIds.Contains(a.QuestionId)).ToListAsync());
                        _context.SingleChoiceOption.RemoveRange(await _context.SingleChoiceOption.Where(a => questions.QuestionIds.Contains(a.QuestionId)).ToListAsync());
                        _context.SingleChoiceQuestions.RemoveRange(await _context.SingleChoiceQuestions.Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id)).ToListAsync());
                        break;
                    case QuestionType.MultipleChoice:
                        _context.MultipleChoiceQuestionsAnswer.RemoveRange(await _context.MultipleChoiceQuestionsAnswer.Where(a => questions.QuestionIds.Contains(a.MultipleChoiceOption.QuestionId)).ToListAsync());
                        _context.MultipleChoiceAnswers.RemoveRange(await _context.MultipleChoiceAnswers.Where(a => questions.QuestionIds.Contains(a.QuestionId)).ToListAsync());
                        _context.MultipleChoiceOption.RemoveRange(await _context.MultipleChoiceOption.Where(a => questions.QuestionIds.Contains(a.QuestionId)).ToListAsync());
                        _context.MultipleChoiceQuestions.RemoveRange(await _context.MultipleChoiceQuestions.Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id)).ToListAsync());
                        break;
                    case QuestionType.TextAnswer:
                        _context.TextAnswers.RemoveRange(await _context.TextAnswers.Where(a => questions.QuestionIds.Contains(a.QuestionId)).ToListAsync());
                        _context.TextQuestions.RemoveRange(await _context.TextQuestions.Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id)).ToListAsync());
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletePoll(long pollId, Guid userId)
        {
            var poll = await _context.Polls.FindAsync(pollId);

            if (poll == null)
                throw new RecordNotFoundException($"Poll with id: {pollId} is not found.");

            if (poll.UserId != userId)
                throw new ForbiddenException();

            _context.YesNoAnswers.RemoveRange(await _context.YesNoAnswers.Where(a => a.Question.PollId == pollId).ToListAsync());
            _context.YesNoQuestions.RemoveRange(await _context.YesNoQuestions.Where(q => q.PollId == pollId).ToListAsync());

            _context.SingleChoiceAnswers.RemoveRange(await _context.SingleChoiceAnswers.Where(a => a.Question.PollId == pollId).ToListAsync());
            _context.SingleChoiceOption.RemoveRange(await _context.SingleChoiceOption.Where(a => a.SingleChoiceQuestion.PollId == pollId).ToListAsync());
            _context.SingleChoiceQuestions.RemoveRange(await _context.SingleChoiceQuestions.Where(q => q.PollId == pollId ).ToListAsync());

            _context.MultipleChoiceQuestionsAnswer.RemoveRange(await _context.MultipleChoiceQuestionsAnswer.Where(a => a.MultipleChoiceOption.MultipleChoiceQuestion.PollId == pollId).ToListAsync());
            _context.MultipleChoiceAnswers.RemoveRange(await _context.MultipleChoiceAnswers.Where(a => a.Question.PollId == pollId).ToListAsync());
            _context.MultipleChoiceOption.RemoveRange(await _context.MultipleChoiceOption.Where(a => a.MultipleChoiceQuestion.PollId == pollId).ToListAsync());
            _context.MultipleChoiceQuestions.RemoveRange(await _context.MultipleChoiceQuestions.Where(q => q.PollId == pollId).ToListAsync());

            _context.TextAnswers.RemoveRange(await _context.TextAnswers.Where(a => a.Question.PollId == pollId).ToListAsync());
            _context.TextQuestions.RemoveRange(await _context.TextQuestions.Where(q => q.PollId == pollId).ToListAsync());

            _context.Polls.Remove(poll);

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Private Methods

        private void AttachQuestions(Poll poll, ICollection<CreateQuestionDto> questions)
        {

            poll.YesNoQuestions = questions
                            .Where(q => q.QuestionType == QuestionType.YesNoAnswer)
                            .Select(q => new YesNoQuestion()
                            {
                                QuestionText = q.Text,
                                QuestionType = q.QuestionType,
                            }).ToList();
            poll.SingleChoiceQuestions = questions
                        .Where(q => q.QuestionType == QuestionType.SingleChoice)
                        .Select(q => new SingleChoiceQuestion()
                        {
                            QuestionText = q.Text,
                            QuestionType = q.QuestionType,
                            Choices = q.Choices?.Select(c => new SingleChoiceOption()
                            {
                                Name = c.Name,
                                Description = c.Description
                            }).ToList()
                        }).ToList();
            poll.MultipleChoiceQuestions = questions
                         .Where(q => q.QuestionType == QuestionType.MultipleChoice)
                         .Select(q => new MultipleChoiceQuestion()
                         {
                             QuestionText = q.Text,
                             QuestionType = q.QuestionType,
                             Choices = q.Choices?.Select(c => new MultipleChoiceOption()
                             {
                                 Name = c.Name,
                                 Description = c.Description
                             }).ToList()
                         }).ToList();
            poll.TextQuestions = questions
                        .Where(q => q.QuestionType == QuestionType.TextAnswer)
                        .Select(q => new TextQuestion()
                        {
                            QuestionText = q.Text,
                            QuestionType = q.QuestionType,
                        }).ToList();
        }

        #endregion
    }
}
