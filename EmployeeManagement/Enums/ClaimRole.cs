using EmployeeManagement.Models;
using System.ComponentModel;

namespace EmployeeManagement.Enums
{
    public enum ClaimRole : int
    {
        [Description(ClaimsStore.CreateRole)]
        CreateRole = 1,

        [Description(ClaimsStore.EditRole)]
        EditRole = 2,

        [Description(ClaimsStore.DeleteRole)]
        DeleteRole = 3
    }
}
