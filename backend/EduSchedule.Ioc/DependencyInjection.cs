using EduSchedule.Application.Auth.Services;
using EduSchedule.Application.Auth.Services.Interfaces;
using EduSchedule.Application.Students.Jobs.Interfaces;
using EduSchedule.Application.Students.Services;
using EduSchedule.Application.Students.Services.Interfaces;
using EduSchedule.Domain.Auth.Services.Interfaces;
using EduSchedule.Domain.Integrations.Services.Interfaces;
using EduSchedule.Domain.States.Repositories;
using EduSchedule.Domain.Students.Repositories;
using EduSchedule.Domain.Students.Services;
using EduSchedule.Domain.Students.Services.Interfaces;
using EduSchedule.Infrastructure.Auth.Services;
using EduSchedule.Infrastructure.Database;
using EduSchedule.Infrastructure.Database.Repositories;
using EduSchedule.Infrastructure.Integrations.Services;
using EduSchedule.Infrastructure.Jobs.Schedulers;
using EduSchedule.Infrastructure.Students.Repositories;
using EduSchedule.Ioc.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduSchedule.Ioc
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EduScheduleDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("EduSchedule.Infrastructure")
                )
            );

            services.AddGraph(configuration);
            services.AddHangfire(configuration);

            services.AddScoped<IStudentsAppService, StudentsAppService>();
            services.AddScoped<ISyncStudentsAppService, SyncStudentsAppService>();
            services.AddScoped<IStudentsService, StudentsService>();
            services.AddScoped<IStudentsRepository, StudentsRepository>();
            services.AddScoped<ISyncStatesRepository, SyncStatesRepository>();
            services.AddScoped<IStudentJobScheduler, HangfireStudentJobScheduler>();
            services.AddScoped<IAuthAppService, AuthAppService>();
            services.AddScoped<ITokenService, JwtService>();


            services.AddScoped<IGraphService, GraphService>();
        }
    }
}
