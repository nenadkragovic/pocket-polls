using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Polls.Api.Tests
{
    public class TestServerFixture : IDisposable
    {
        public TestServer TestServer { get; }
        public HttpClient Client { get; }

        public TestServerFixture()
        {
            var builder = WebHost.CreateDefaultBuilder()
                    .ConfigureAppConfiguration((hostContext, configApp) =>
                    {
                        configApp.AddJsonFile("appsettings.json", optional: false);
                        configApp.AddEnvironmentVariables();
                    })
                    .ConfigureTestServices(services =>
                    {
                    })
                    .UseEnvironment("Development")
                    .UseStartup<Program>();

            TestServer = new TestServer(builder);
            Client = TestServer.CreateClient();
        }

        public void Dispose()
        {
            TestServer?.Dispose();
            Client?.Dispose();
        }
    }

    [CollectionDefinition("PollsCollection")]
    public class PollsCollection : ICollectionFixture<TestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}