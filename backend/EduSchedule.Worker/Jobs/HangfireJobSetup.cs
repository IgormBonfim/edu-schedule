using EduSchedule.Application.Students.Services.Interfaces;
using Hangfire;

namespace EduSchedule.Worker.Jobs
{
    public static class HangfireJobSetup
    {
        public static void RegisterScheduledJobs(IRecurringJobManager recurringJobManager)
        {   
            recurringJobManager.AddOrUpdate<ISyncStudentsAppService>(
                "sync-students-orchestrator", 
                service => service.StartStudensSyncProcessAsync(default), 
                Cron.Hourly);

            recurringJobManager.AddOrUpdate<ISyncStudentsAppService>(
                "sync-events-orchestrator", 
                service => service.StartEventsSyncProcessAsync(default), 
                Cron.Hourly);
        }
    }
}