using Microsoft.EntityFrameworkCore;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Enums;
using Polls.Lib.Exceptions;

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
        public async Task<ListResult<LiustPollsDto>> ListPolls(int offset, int limit, string searchParam = "")
        {
            var query = _context.Polls.AsNoTracking();

            if (!string.IsNullOrEmpty(searchParam))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchParam.ToLower().Trim()));
            }

            int total = await query.AsNoTracking().CountAsync();

            var records = await _context.Polls
                .Skip(offset).Take(limit)
                .Select(p => new LiustPollsDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();

            return new ListResult<LiustPollsDto>()
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

        public async Task<Poll?> AddPoll(CreatePollDto model)
        {
            var record = new Poll()
            {
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            AttachQuestions(record, model.Questions);

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
                        _context.YesNoQuestions.RemoveRange(
                            await _context.YesNoQuestions
                                .Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id))
                                .ToListAsync());
                        break;
                    case QuestionType.SingleChoice:
                        _context.SingleChoiceQuestions.RemoveRange(
                            await _context.SingleChoiceQuestions
                                .Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id))
                                .ToListAsync());
                        break;
                    case QuestionType.MultipleChoice:
                        _context.MultipleChoiceQuestions.RemoveRange(
                            await _context.MultipleChoiceQuestions
                                .Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id))
                                .ToListAsync());
                        break;
                    case QuestionType.TextAnswer:
                        _context.TextQuestions.RemoveRange(
                            await _context.TextQuestions
                                .Where(q => q.PollId == pollId && questions.QuestionIds.Contains(q.Id))
                                .ToListAsync());
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletePoll(long pollId)
        {
            var poll = await _context.Polls.FindAsync(pollId);

            if (poll == null)
                throw new RecordNotFoundException($"Poll with id: {pollId} is not found.");

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
