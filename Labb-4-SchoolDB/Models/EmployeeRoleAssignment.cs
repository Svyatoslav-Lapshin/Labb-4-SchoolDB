using System;
using System.Collections.Generic;

namespace Labb_4_SchoolDB.Models;

public partial class EmployeeRoleAssignment
{
    public int EmployeeId { get; set; }

    public int EmployeeRoleId { get; set; }

    public decimal Salary { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual EmployeeRole EmployeeRole { get; set; } = null!;
}
