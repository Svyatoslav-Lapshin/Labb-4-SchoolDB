using System;
using System.Collections.Generic;

namespace Labb_4_SchoolDB.Models;

public partial class EmployeeRole
{
    public int EmployeeRoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<EmployeeRoleAssignment> EmployeeRoleAssignments { get; set; } = new List<EmployeeRoleAssignment>();
}
