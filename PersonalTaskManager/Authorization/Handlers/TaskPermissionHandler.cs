using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace PersonalTaskManager.Authorization
{
public class TaskPermissionHandler : AuthorizationHandler<TaskPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TaskPermissionRequirement requirement)
    {
        //context class has info about user,aoth process and resource
        // Check if the user has any of the allowed claims
        var hasClaim = context.User.Claims
            .Any(c => c.Type == "TaskPermission" && requirement.AllowedRoles.Contains(c.Value));

        if (hasClaim)
        {
            context.Succeed(requirement);   //call this to mark the req satisfied
        }

        return Task.CompletedTask;
    }
}
}