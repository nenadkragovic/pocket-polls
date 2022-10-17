using Grpc.Core;
using Polls.Lib.Repositories;

namespace Polls.gRPC.Services
{
    public class PollsService : PollsGreeter.PollsGreeterBase
    {
        private readonly PollsRepository _pollsRepository;
        public PollsService(PollsRepository pollsRepository)
        {
            _pollsRepository = pollsRepository;
        }

        public override async Task<ListPollsResponse> ListPolls(ListPollsRequest request, ServerCallContext context)
        {
            var result = await _pollsRepository.ListPolls(request.Offset, request.Limit, request.SearchParam);

            return new ListPollsResponse()
            {
                TotalRecords = result.TotalRecords,
                Records = {
                    result.Records.Select(r => new ListPollsDto()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description
                    }).ToList()
                }
            };
        }
    }
}