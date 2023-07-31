namespace AuthenticationService.Security;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
public interface IAuthorizeServiceToServiceRequirement : IAuthorizationRequirement
{
}

public class ServiceToServiceAuthorizationHandler : AuthorizationHandler<IAuthorizeServiceToServiceRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizeServiceToServiceRequirement requirement)
    {
        // Check if the user (service) has a specific claim indicating that it is a service
        if (context.User.HasClaim(c => c.Type == "ServiceClaim" && c.Value == "true"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

