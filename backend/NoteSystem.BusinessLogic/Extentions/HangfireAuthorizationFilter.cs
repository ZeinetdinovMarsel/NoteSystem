
using Hangfire.Dashboard;

namespace NoteSystem.BusinessLogic.Extentions;
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == false;
    }
}