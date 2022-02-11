using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    public class SuperAdminHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            if (context.User.IsInRole(ClaimsStore.SuperAdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
