
using EduSchedule.Ioc;
using EduSchedule.Worker.Jobs;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddHangfireServer(options => {
    options.Queues = new[] { "student-sync", "events-sync", "default" };
    options.WorkerCount = 10;
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    
    HangfireJobSetup.RegisterScheduledJobs(manager);
}

await host.RunAsync();
