using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Polls.Lib.Database;
using Polls.Lib.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<PollsRepository>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDefaultIdentity<IdentityUser>
    (options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
.AddEntityFrameworkStores<Context>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
{
    using (var context = serviceScope.ServiceProvider.GetService<Context>())
    {
        context.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Configuration["StaticFilesPath"])),
    RequestPath = "/static"
});

app.Run();
