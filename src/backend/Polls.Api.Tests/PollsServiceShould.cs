using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Polls.Lib.DTO;
using Polls.Lib.Enums;
using System.Net;
using System.Text;

namespace Polls.Api.Tests
{
    [Collection("PollsCollection")]
    public class PollsServiceShould
    {
        private readonly TestServer _server;

        public PollsServiceShould(TestServerFixture fixture)
        {
            _server = fixture.TestServer;
        }

        [Fact]
        public async Task CreatePollShouldBeOk()
        {
            var createPollDto = new CreatePollDto()
            {
                Name = "Test poll",
                Description = "This is test poll",
                Questions = new List<CreateQuestionDto>()
                {
                    new CreateQuestionDto()
                    {
                        QuestionType = QuestionType.YesNoAnswer,
                        Text = "This is yes/no type question?"
                    },
                     new CreateQuestionDto()
                    {
                        QuestionType = QuestionType.SingleChoice,
                        Text = "This is single choice question?",
                        Choices = new List<CreateChoiceDto>()
                        {
                            new CreateChoiceDto()
                            {
                                Name = "Yes",
                                Description = "Yeah"
                            },
                            new CreateChoiceDto()
                            {
                                Name = "No",
                                Description = "Nope"
                            }
                        }
                    },
                      new CreateQuestionDto()
                    {
                        QuestionType = QuestionType.MultipleChoice,
                        Text = "This is multiple choice question?",
                        Choices = new List<CreateChoiceDto>()
                        {
                            new CreateChoiceDto()
                            {
                                Name = "Option 1",
                            },
                            new CreateChoiceDto()
                            {
                                Name = "Option 2",
                            },
                            new CreateChoiceDto()
                            {
                                Name = "Option 3",
                            },
                            new CreateChoiceDto()
                            {
                                Name = "Option 4",
                            }
                        }
                    },
                       new CreateQuestionDto()
                    {
                        QuestionType = QuestionType.TextAnswer,
                        Text = "This is test type question, write anything down."
                    },
                }
            };

            var response = await _server.CreateRequest($"/api/polls")
                .And(x => x.Content =
                    new StringContent(
                        JsonConvert.SerializeObject(createPollDto), Encoding.UTF8, "application/json"))
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
