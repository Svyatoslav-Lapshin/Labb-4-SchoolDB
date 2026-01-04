using System;
using System.Collections.Generic;
using Labb_4_SchoolDB.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb_4_SchoolDB.Data;

public partial class SchoolInfoDbContext : DbContext
{
    public SchoolInfoDbContext()
    {
    }

    public SchoolInfoDbContext(DbContextOptions<SchoolInfoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseOffering> CourseOfferings { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<EmployeeRoleAssignment> EmployeeRoleAssignments { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeScale> GradeScales { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.;Database=SchoolInfoDb;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927C0A19BF3CA");

            entity.ToTable("Class");

            entity.Property(e => e.ClassName).HasMaxLength(100);

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Class_Teacher");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A788E37756");

            entity.ToTable("Course");

            entity.Property(e => e.CourseName).HasMaxLength(150);
        });

        modelBuilder.Entity<CourseOffering>(entity =>
        {
            entity.HasKey(e => e.CourseOfferingId).HasName("PK__CourseOf__AF9ECFB23839CD3D");

            entity.ToTable("CourseOffering");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseOfferings)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_CourseOffreing_Course");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F119B3EAE84");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeLastName).HasMaxLength(150);
            entity.Property(e => e.EmployeeName).HasMaxLength(150);

            entity.HasMany(d => d.CourseOfferings).WithMany(p => p.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "TeacherCourse",
                    r => r.HasOne<CourseOffering>().WithMany()
                        .HasForeignKey("CourseOfferingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CorseOffering_Teacher"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Teacher_Course"),
                    j =>
                    {
                        j.HasKey("TeacherId", "CourseOfferingId").HasName("PK_TeacherCourseId");
                        j.ToTable("TeacherCourse");
                    });
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId).HasName("PK__Employee__346186E628CEEF6D");

            entity.ToTable("EmployeeRole");

            entity.Property(e => e.RoleName).HasMaxLength(150);
        });

        modelBuilder.Entity<EmployeeRoleAssignment>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.EmployeeRoleId }).HasName("PK_Employee_Role");

            entity.ToTable("EmployeeRoleAssignment");

            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeRoleAssignments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_RoleAssign");

            entity.HasOne(d => d.EmployeeRole).WithMany(p => p.EmployeeRoleAssignments)
                .HasForeignKey(d => d.EmployeeRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Role");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grade__54F87A57D90AF126");

            entity.ToTable("Grade");

            entity.HasOne(d => d.CourseOffering).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseOfferingId)
                .HasConstraintName("FK_Grade_CourseOffering");

            entity.HasOne(d => d.GradeScale).WithMany(p => p.Grades)
                .HasForeignKey(d => d.GradeScaleId)
                .HasConstraintName("FK_Grade_GradeScale");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Grade_Student");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Grades)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_GRADE_Teacher");
        });

        modelBuilder.Entity<GradeScale>(entity =>
        {
            entity.HasKey(e => e.GradeScaleId).HasName("PK__GradeSca__B5AD3A67CB6A2773");

            entity.ToTable("GradeScale");

            entity.HasIndex(e => e.Letter, "UQ__GradeSca__31197A0589F6BE53").IsUnique();

            entity.Property(e => e.Letter)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Value).HasColumnType("decimal(5, 1)");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99044969E3");

            entity.ToTable("Student");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.PersonNumber).HasMaxLength(20);
            entity.Property(e => e.StudentLastName).HasMaxLength(150);
            entity.Property(e => e.StudentName).HasMaxLength(150);

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_Student_Class");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
