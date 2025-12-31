using Microsoft.AspNetCore.Authorization;

namespace PersonalTaskManager.Authorization
{
public class TaskPermissionRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public TaskPermissionRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
}