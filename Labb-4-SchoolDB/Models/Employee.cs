using System;
using System.Collections.Generic;

namespace Labb_4_SchoolDB.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string EmployeeLastName { get; set; } = null!;

    public DateOnly? StartWorkDate { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<EmployeeRoleAssignment> EmployeeRoleAssignments { get; set; } = new List<EmployeeRoleAssignment>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
}
