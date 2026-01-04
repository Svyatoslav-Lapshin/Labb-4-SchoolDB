using System;
using System.Collections.Generic;

namespace Labb_4_SchoolDB.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public virtual ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
}
