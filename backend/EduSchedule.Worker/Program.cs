
using EduSchedule.Ioc;
using EduSchedule.Worker.Jobs;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddHangfireServer(options => {
    options.Queues = new[] { "student-sync", "default" };
    options.WorkerCount = 10;
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var manager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    
    // 3. Agora sim, com o manager injetado e o Storage pronto!
    HangfireJobSetup.RegisterScheduledJobs(manager);
}

await host.RunAsync();
