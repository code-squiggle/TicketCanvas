using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketCanvas.Common.Application;

namespace TicketCanvas.Common.Http.Extensions;

public static class ClaimsPrincipalExtentions
{
    extension(ClaimsPrincipal claimsPrincipal)
    {
        public Guid GetUserId()
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
                throw new ApplicationException("User Id is not found.");

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                throw new ApplicationException("Invalid User Id.");

            return userId;
        }

        public UserRole GetUserRole()
        {
            var userRoleClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (string.IsNullOrEmpty(userRoleClaim?.Value))
                throw new ApplicationException("User Role is not found.");

            if (!Enum.TryParse(userRoleClaim.Value, true, out UserRole userRole) || userRole == UserRole.None)
                throw new ApplicationException("User Role is invalid.");

            return userRole;
        }
    }
}