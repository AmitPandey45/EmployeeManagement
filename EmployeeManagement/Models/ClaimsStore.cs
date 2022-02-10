using EmployeeManagement.Enums;
using EmployeeManagement.Extensions;
using System.Collections.Generic;
using System.Security.Claims;

namespace EmployeeManagement.Models
{
    public static class ClaimsStore
    {
        public const string CreateRolePolicy = "CreateRolePolicy";
        public const string EditRolePolicy = "EditRolePolicy";
        public const string DeleteRolePolicy = "DeleteRolePolicy";
        public const string AdminRolePolicy = "AdminRolePolicy";

        public const string CreateRole = "Create Role";
        public const string EditRole = "Edit Role";
        public const string DeleteRole = "Delete Role";

        public const string AdminRole = "Admin";
        public const string SuperAdminRole = "Super Admin";

        public const string ClaimValueYes = "Yes";
        public const string ClaimValueNo = "No";

        public static List<Claim> AllClaims = new List<Claim>
        {
            new Claim(CreateRole, CreateRole),
            new Claim(EditRole, EditRole),
            new Claim(DeleteRole, DeleteRole),
        };

        public static Dictionary<ClaimPolicy, string> AllClaimsPolicy = EnumExtension.ToDictionaryWithEnumAsKeyValue<ClaimPolicy>();

        public static Dictionary<ClaimRole, string> AllClaimsRole = EnumExtension.ToDictionaryWithDescriptionAsValue<ClaimRole>();
    }
}
