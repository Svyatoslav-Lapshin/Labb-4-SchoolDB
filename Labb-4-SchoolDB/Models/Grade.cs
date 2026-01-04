using System;
using System.Collections.Generic;

namespace Labb_4_SchoolDB.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public DateOnly DateOfIssue { get; set; }

    public int? GradeScaleId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseOfferingId { get; set; }

    public int? TeacherId { get; set; }

    public virtual CourseOffering? CourseOffering { get; set; }

    public virtual GradeScale? GradeScale { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Employee? Teacher { get; set; }
}
