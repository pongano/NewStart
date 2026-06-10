using Microsoft.AspNetCore.Authorization;

namespace CoreProject.Backend.API.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionCode)
    {
        Policy = permissionCode;
    }
}
