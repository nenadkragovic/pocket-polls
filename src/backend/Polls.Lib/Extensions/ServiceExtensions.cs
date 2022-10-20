using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polls.Lib.Database;
using Polls.Lib.Database.Models;
using Polls.Lib.Repositories;
using Polls.Lib.Repositories.Authentication;
using System.Text;

namespace Polls.Lib.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration["InMemoryDatabase"] == null || !Boolean.Parse(configuration["InMemoryDatabase"]))
            {
                services.AddDbContext<Context>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("PlatformDb"), sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Context).Assembly.GetName().Name);
                    }), ServiceLifetime.Transient);
            }
            else
            {
               services.AddDbContext<Context>(options =>
                    options.UseInMemoryDatabase("InMemoryPlatformDb")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)),
                    ServiceLifetime.Transient);
            }
        }

        public static void ConfigurePollsServices(this IServiceCollection services)
        {
            services.AddTransient<PollsRepository>();
            services.AddTransient<AnswersRepository>();
            services.AddTransient<UserManager<User>>();
            services.AddTransient<SignInManager<User>>();
            services.AddScoped<IUserAuthenticationRepository, UserAuthenticationRepository>();
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) => services.AddResponseCaching();

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole<Guid>>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            var secretKey = jwtConfig["Key"];
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["Issuer"],
                    ValidAudience = jwtConfig["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        public static void CreateDatabase(this WebApplication app)
        {
            using (var serviceScope = app.Services
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<Context>())
                {
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
