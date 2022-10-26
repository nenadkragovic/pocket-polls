using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Polls.Lib.Database.Models;
using Polls.Lib.DTO;
using Polls.Lib.Enums;
using System.Net;
using System.Text;

namespace Polls.Api.Tests
{
    [Collection("PollsCollection")]
    public class PollsServiceShould : IClassFixture<WebApplicationFactory<Program>>
    {
        #region Fields and Constructor

        private readonly TestServer _server;
        private readonly HttpClient _client;
        private string Token = string.Empty;

        #region Shared Objects

        private static Poll AddQuestionsPoll = null;
        private static CreatePollDto CreatePollWithAllQuestions = new CreatePollDto()
        {
            Name = "Test poll",
            Description = "This is test poll",
            StartDate = DateTime.MinValue,
            EndDate = DateTime.MaxValue,
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
                            Description = "",
                        },
                        new CreateChoiceDto()
                        {
                            Name = "Option 2",
                            Description = "",
                        },
                        new CreateChoiceDto()
                        {
                            Name = "Option 3",
                            Description = "",
                        },
                        new CreateChoiceDto()
                        {
                            Name = "Option 4",
                            Description = "",
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

        #endregion

        public PollsServiceShould()
        {
            var application = new WebApplicationFactory<Program>();

            _client = application.CreateClient();

            _server = application.Server;

            var response = _server.CreateRequest($"/api/users/register")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(new CreateUserDto()
                    {
                        FullName = "Joe Doe",
                        Address = "Anytown, 1st street",
                        Email = "joe.doe@mail.com",
                        UserName = "joedoe",
                        Password = "4rfvBGT%",
                    }), Encoding.UTF8, "application/json"))
                .PostAsync().Result;

            response = _server.CreateRequest($"/api/users/login")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(new CreateUserDto()
                    {
                        UserName = "joedoe",
                        Password = "4rfvBGT%"
                    }), Encoding.UTF8, "application/json"))
                .PostAsync().Result;

            var tokenDto = JsonConvert.DeserializeObject<TokenDto>(response.Content.ReadAsStringAsync().Result);
            Token = tokenDto.Token;
        }

        #endregion

        #region Polls and Questions Management

        [Fact]
        public async Task ListPollsShouldReturnNoContent()
        {
            var response = await _server.CreateRequest($"/api/polls")
                .GetAsync();

            response.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.NoContent, HttpStatusCode.OK });
        }


        [Theory]
        [InlineData("empty poll", "empty poll desctiption")]
        public async Task<Poll> CreatePollWithNoQuestionsShouldBeOk(string pollName, string pollDescription)
        {
            var createPollDto = new CreatePollDto()
            {
                Name = pollName,
                Description = pollDescription,
                StartDate = DateTime.Now,
                EndDate = DateTime.UtcNow
            };

            var response = await _server.CreateRequest($"/api/polls/")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(createPollDto), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var pollObject = JsonConvert.DeserializeObject<Poll>(await response.Content.ReadAsStringAsync());

            pollObject.Should().NotBeNull();

            return pollObject;
        }

        [Fact]
        public async Task CreateGetListPollWithQuestionsShouldBeOk()
        {
            var response = await _server.CreateRequest($"/api/polls")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(CreatePollWithAllQuestions), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var pollObject = JsonConvert.DeserializeObject<Poll>(await response.Content.ReadAsStringAsync());

            pollObject.Should().NotBeNull();

            response = await _server.CreateRequest($"/api/polls")
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _server.CreateRequest($"/api/polls/{pollObject.Id}")
                .AddHeader("Authorization", $"Bearer {Token}")
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            pollObject = JsonConvert.DeserializeObject<Poll>(await response.Content.ReadAsStringAsync());

            pollObject.Should().NotBeNull();
        }

        [Theory]
        [InlineData(QuestionType.YesNoAnswer, "Yes/No Question")]
        [InlineData(QuestionType.TextAnswer, "Text Question")]
        [InlineData(QuestionType.SingleChoice, "Single choice Question", new[] { "choice1", "choice2" })]
        [InlineData(QuestionType.MultipleChoice, "Multiple choice Question", new[] { "choice1", "choice2", "choice3", "choice4" })]
        public async Task AddQuestionsShouldBeOk(QuestionType questionType, string questionText, string[] choices = null)
        {
            if (AddQuestionsPoll == null)
            {
                AddQuestionsPoll = await CreatePollWithNoQuestionsShouldBeOk("Test adding questions poll", "This is test poll which tests adding various questions");
            }

            var question = new CreateQuestionDto()
            {
                QuestionType = questionType,
                Text = questionText,
                Choices = new List<CreateChoiceDto>()
            };

            if (choices != null)
            {
                foreach (string choice in choices)
                {
                    question.Choices.Add(new CreateChoiceDto()
                    {
                        Name = choice,
                        Description = choice
                    });
                }
            }

            ICollection<CreateQuestionDto> questions = new List<CreateQuestionDto>() { question };

            var response = await _server.CreateRequest($"/api/polls/{AddQuestionsPoll.Id}/questions")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(questions), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddChoiceQuestionsWithoutChoicesShouldReturnBadRequest()
        {
            if (AddQuestionsPoll == null)
            {
                AddQuestionsPoll = await CreatePollWithNoQuestionsShouldBeOk("Test adding questions poll", "This is test poll which tests adding various questions");
            }

            var question = new CreateQuestionDto()
            {
                QuestionType = QuestionType.MultipleChoice,
                Text = "MC Q?",
                Choices = new List<CreateChoiceDto>()
            };

            ICollection<CreateQuestionDto> questions = new List<CreateQuestionDto>() { question };

            var response = await _server.CreateRequest($"/api/polls/{AddQuestionsPoll.Id}/questions")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(questions), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task DeleteQuestionsShouldBeOk()
        {
            var response = await _server.CreateRequest($"/api/polls")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(CreatePollWithAllQuestions), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var pollObject = JsonConvert.DeserializeObject<Poll>(await response.Content.ReadAsStringAsync());

            pollObject.Should().NotBeNull();

            ICollection<DeleteQuestionDto> questionsToDelete = new List<DeleteQuestionDto>()
            {
                new DeleteQuestionDto()
                {
                    QuestionType = QuestionType.YesNoAnswer,
                    QuestionIds = pollObject.YesNoQuestions.Select(q => q.Id).ToList()
                },
                new DeleteQuestionDto()
                {
                    QuestionType = QuestionType.TextAnswer,
                    QuestionIds = pollObject.TextQuestions.Select(q => q.Id).ToList()
                },
                new DeleteQuestionDto()
                {
                    QuestionType = QuestionType.SingleChoice,
                    QuestionIds = pollObject.SingleChoiceQuestions.Select(q => q.Id).ToList()
                },
                new DeleteQuestionDto()
                {
                    QuestionType = QuestionType.MultipleChoice,
                    QuestionIds = pollObject.MultipleChoiceQuestions.Select(q => q.Id).ToList()
                }
            };

            response = await _server.CreateRequest($"/api/polls/{pollObject.Id}/questions")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(questionsToDelete), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .SendAsync("DELETE");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeletePollShouldBeOk()
        {
            var response = await _server.CreateRequest($"/api/polls")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(CreatePollWithAllQuestions), Encoding.UTF8, "application/json"))
                 .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var pollObject = JsonConvert.DeserializeObject<Poll>(await response.Content.ReadAsStringAsync());

            pollObject.Should().NotBeNull();

            response = await _server.CreateRequest($"/api/polls/{pollObject.Id}")
                .AddHeader("Authorization", $"Bearer {Token}")
                .SendAsync("DELETE");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Answers

        [Fact]
        public async Task AddAnswersShouldBeOk()
        {
            var response = await _server.CreateRequest($"/api/polls")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(CreatePollWithAllQuestions), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var pollObject = JsonConvert.DeserializeObject<Poll>(await response.Content.ReadAsStringAsync());

            pollObject.Should().NotBeNull();

            var model = new AddAnswersDto()
            {
                YesNoAnswers = pollObject.YesNoQuestions.Select(q => new YesNoAnswerDto() { QuestionId = q.Id, Answer = true }).ToList(),
                TextAnswers = pollObject.TextQuestions.Select(q => new TextAnswerDto() { QuestionId = q.Id, Answer = "Answer to text question" }).ToList(),
                SingleChoiceAnswers = pollObject.SingleChoiceQuestions.Select(q => new SingleChoiceAnswerDto() { QuestionId = q.Id, ChoiceId = q.Choices.FirstOrDefault().Id }).ToList(),
                MultipleChoiceAnswers = pollObject.MultipleChoiceQuestions.Select(q => new MultipleChoiceAnswerDto() { QuestionId = q.Id, ChoiceIds = q.Choices.Select(c => c.Id).ToList()}).ToList()
            };

            response = await _server.CreateRequest($"/api/answers/{pollObject.Id}")
                .And(x => x.Content =
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"))
                .AddHeader("Authorization", $"Bearer {Token}")
                .PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion
    }
}
