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
                service => service.StartSyncProcessAsync(), 
                Cron.Hourly);
        }
    }
}