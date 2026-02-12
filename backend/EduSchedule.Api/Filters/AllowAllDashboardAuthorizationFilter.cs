using Hangfire.Dashboard;

namespace EduSchedule.Api.Filters;

public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; 
    }
}
