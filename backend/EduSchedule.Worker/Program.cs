
using EduSchedule.Ioc;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddHangfireServer(options => {
    options.Queues = new[] { "student-sync", "default" };
    options.WorkerCount = 10;
});

var host = builder.Build();
host.Run();
