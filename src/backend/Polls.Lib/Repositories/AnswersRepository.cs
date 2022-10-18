using Polls.Lib.Database;
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


        public void AddAnswers(long userId, long pollId, AddAnswersDto model)
        {

        }
    }
}
