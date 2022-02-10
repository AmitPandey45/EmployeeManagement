using System.ComponentModel;

namespace EmployeeManagement.Enums
{
    public enum ClaimPolicy : int
    {
        [Description("Create Role Policy")]
        CreateRolePolicy = 1,

        [Description("Edit Role Policy")]
        EditRolePolicy = 2,

        [Description("Delete Role Policy")]
        DeleteRolePolicy = 3
    }
}
