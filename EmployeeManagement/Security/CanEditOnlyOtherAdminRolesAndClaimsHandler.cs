using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;

            if(authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInUserId =
                context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            string userIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            if(context.User.IsInRole(ClaimsStore.AdminRole) &&
                context.User.HasClaim(claim => claim.Type.Equals(ClaimsStore.EditRole) && claim.Value.Equals(ClaimsStore.ClaimValueYes)) &&
                !userIdBeingEdited.ToLower().Equals(loggedInUserId.ToLower()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
