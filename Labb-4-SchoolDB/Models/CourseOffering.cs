using System;
using System.Collections.Generic;

namespace Labb_4_SchoolDB.Models;

public partial class CourseOffering
{
    public int CourseOfferingId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? CourseId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Employee> Teachers { get; set; } = new List<Employee>();
}
