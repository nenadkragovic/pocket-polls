using Microsoft.EntityFrameworkCore;
using Polls.Api.Database;
using Polls.Api.Database.Models;
using Polls.Api.DTO;
using Serilog;

namespace Polls.Api.Repositories
{
    public class PollsRepository
    {
        private readonly Context _context;

        public PollsRepository(Context context)
        {
            _context = context;
        }

        public async Task<ListResult<Poll>> ListPolls(int offset, byte limit, string searchParam = "")
        {
            var query = _context.Polls.AsNoTracking();

            if (!string.IsNullOrEmpty(searchParam))
            {
                query = query.Where(p => p.Name.Contains(searchParam));
            }

            int total = await query.AsNoTracking().CountAsync();

            var records = await _context.Polls.Skip(offset).Take(limit).ToListAsync();

            return new ListResult<Poll>()
            {
                Records = records,
                TotalRecords = total
            };
        }

        public async Task<Poll?> GetPollById(long pollId)
        {
            return await _context.Polls
                .Include(p => p.YesNoQuestions)
                .Include(p => p.SingleChoiceQuestions)
                .Include(p => p.MultipleChoiceQuestions)
                .Include(p => p.TextQuestions)
                .AsNoTracking().FirstOrDefaultAsync(p => p.Id == pollId);
        }

        public async Task<Poll?> AddPoll(CreatePollDto model)
        {
            using (var transacton = _context.Database.BeginTransaction())
            {
                try
                {
                    var poll = _context.Polls.Add(new Poll()
                    {
                        Name = model.Name,
                        Description = model.Description,
                    });

                    await _context.SaveChangesAsync();

                    foreach (var question in model.Questions)
                    {
                        switch (question.QuestionType)
                        {
                            case Enums.QuestionType.YesNoAnswer:
                                _context.YesNoQuestions.Add(new YesNoQuestion()
                                {
                                    QuestionText = question.Text,
                                    QuestionType = question.QuestionType,
                                    PollId = poll.Entity.Id
                                });
                                break;
                            case Enums.QuestionType.SingleChoice:
                                var sQRecord = _context.SingleChoiceQuestions.Add(new SingleChoiceQuestion()
                                {
                                    QuestionText = question.Text,
                                    QuestionType = question.QuestionType,
                                    PollId = poll.Entity.Id
                                });

                                await _context.SaveChangesAsync();

                                if (question.Choices != null)
                                    foreach (var choice in question.Choices)
                                    {
                                        _context.SingleChoices.Add(new SingleChoice()
                                        {
                                            Name = choice.Name,
                                            Description = choice.Description,
                                            QuestionId = sQRecord.Entity.Id
                                        });
                                    }

                                break;
                            case Enums.QuestionType.MultipleChoice:
                                var mQRecord = _context.MultipleChoiceQuestions.Add(new MultipleChoiceQuestion()
                                {
                                    QuestionText = question.Text,
                                    QuestionType = question.QuestionType,
                                    PollId = poll.Entity.Id
                                });

                                await _context.SaveChangesAsync();

                                if (question.Choices != null)
                                    foreach (var choice in question.Choices)
                                    {
                                        _context.MultipleChoices.Add(new MultipleChoice()
                                        {
                                            Name = choice.Name,
                                            Description = choice.Description,
                                            QuestionId = mQRecord.Entity.Id
                                        });
                                    }
                                break;
                            case Enums.QuestionType.TextAnswer:
                                _context.TextQuestions.Add(new TextQuestion()
                                {
                                    QuestionText = question.Text,
                                    QuestionType = question.QuestionType,
                                    PollId = poll.Entity.Id
                                });
                                break;
                        }
                    }

                    await _context.SaveChangesAsync();

                    await transacton.CommitAsync();

                    return await GetPollById(poll.Entity.Id);
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);

                    await transacton.RollbackAsync();

                    throw;
                }

            }
        }

        public async Task<int> UpdatePoll(Poll poll)
        {
            _context.Polls.Update(poll);

            return await _context.SaveChangesAsync();
        }

        //public async Task<int> AddQuestions(ICollection<Question> questions)
        //{
        //    _context.Questions.AddRange(questions);

        //    return await _context.SaveChangesAsync();
        //}

        //public async Task DeleteQuestions(ICollection<long> questionIds)
        //{
        //    var questionsToDelete = _context.Questions.Where(q => questionIds.Contains(q.Id));

        //    _context.Questions.RemoveRange(questionsToDelete);

        //    await _context.SaveChangesAsync();
        //}
    }
}
