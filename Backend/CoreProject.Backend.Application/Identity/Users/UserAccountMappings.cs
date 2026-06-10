using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.Identity.Users;

internal static class UserAccountMappings
{
    public static UserAccountResponse ToResponse(this UserAccount userAccount)
    {
        return new UserAccountResponse
        {
            Id = userAccount.Id,
            UserName = userAccount.UserName,
            Email = userAccount.Email,
            DisplayName = userAccount.DisplayName,
            IsActive = userAccount.IsActive,
            CreatedAtUtc = userAccount.CreatedAtUtc
        };
    }
}
