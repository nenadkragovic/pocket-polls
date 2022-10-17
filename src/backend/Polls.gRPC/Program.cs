using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Polls.gRPC.Services;
using Polls.Lib.Database;
using Polls.Lib.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddTransient<PollsRepository>();

// register database servcies
if (builder.Configuration["InMemoryDatabase"] == null || !Boolean.Parse(builder.Configuration["InMemoryDatabase"]))
{
    builder.Services.AddDbContext<Context>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformDb"), sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(Context).Assembly.GetName().Name);
        }), ServiceLifetime.Transient);
}
else
{
    builder.Services.AddDbContext<Context>(options =>
            options.UseInMemoryDatabase("InMemoryPlatformDb")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)),
        ServiceLifetime.Transient);
}

var app = builder.Build();

using (var serviceScope = app.Services
           .GetRequiredService<IServiceScopeFactory>()
           .CreateScope())
{
    using (var context = serviceScope.ServiceProvider.GetService<Context>())
    {
        context.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline.
app.MapGrpcService<PollsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
