using DevBoard.Application.Common;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Infrastructure.Persistence;
using DevBoard.Infrastructure.Repositories;
using DevBoard.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevBoard.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IApplicationStatusRepository, ApplicationStatusRepository>();

            return services;
        }
    }
}
