
using Labb_4_SchoolDB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Labb_4_SchoolDB.Data;
using Labb_4_SchoolDB;


namespace DatabaseSchool.Menu
{
    //Create database again, because of new buggs with relations
    internal class MainMenu
    {
        private static readonly CultureInfo SwedishCulture = new CultureInfo("sv-SE");

        public static void DisplayMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            using SchoolInfoDbContext context = new SchoolInfoDbContext();
            while (true)
            {
                int userInput = InputValidation.ValidateIntInRange(
                    "1.Get all students\n" +
                    "2.Get all students in a specific class\n" +
                    "3.Adding new students\n" +
                    "4.Get staff\n" +
                    "5.Adding new staff\n" +
                    "6.Show how many employees work in the different departments\n" + 
                    "7.Show information about all students\n" +
                    "8.Show a list of all (active) courses\n" +
                    "9.Rate a student\n" + 
                    "10.Exi\n" +
                    "Input your choice:", 1, 10);
                Console.Clear();
                switch (userInput)
                {
                    case 1:

                        int sortField = InputValidation.ValidateIntInRange(
                            "How would you like to sort the students?\n(1 - First name, 2 - Last name):", 1, 2);
                        int sortOrder = InputValidation.ValidateIntInRange(
                            "Sort order?\n(1 - Ascending, 2 - Descending):", 1, 2);
                        Console.Clear();
                        GetAllStudents(context, sortField, sortOrder);
                        break;

                    case 2:
                        Console.WriteLine();
                        GetStudentsInClass(context);
                        break;

                    case 3:
                        Console.WriteLine();
                        AddNewStudent(context);
                        break;

                    case 4:
                        int optionShowingChoice = InputValidation.ValidateIntInRange(
                           "What would you like to view?\n(1 - View all employees, 2 - View employees by category):", 1, 2);
                        ShowAllStaff(context, optionShowingChoice);

                        break;

                    case 5:
                        Console.WriteLine();
                        AddNewStaff(context);
                        break;
                    case 6:
                        CountTeachersPerDepartment(context);
                        break;
                    case 7:
                        ShowInformationAboutAllStudents(context);
                        break;
                    case 8:
                        ShowAllActiveCourse(context);

                        break;
                    case 9:
                        SetGradeForStudent(context);
                        ClearConsoleAfterKeyPress();

                        break;
                    case 10:
                        Console.WriteLine("Goodbye");
                        return;
                }

            }
            static void GetAllStudents(SchoolInfoDbContext context, int sortField, int sortOrder)
            {
                var students = context.Students.ToList();
                switch (sortField)
                {
                    case 1: //First name
                        if (sortOrder == 1)
                        {
                            students = students.OrderBy(s => s.StudentName, StringComparer.Create(SwedishCulture, ignoreCase: true)).ToList();
                        }
                        else
                        {
                            students = students.OrderByDescending(s => s.StudentName, StringComparer.Create(SwedishCulture, ignoreCase: true)).ToList();
                        }
                        break;

                    case 2://Last name
                        if (sortOrder == 1)
                        {
                            students = students.OrderBy(s => s.StudentLastName, StringComparer.Create(SwedishCulture, ignoreCase: true)).ToList();
                        }
                        else
                        {
                            students = students.OrderByDescending(s => s.StudentLastName, StringComparer.Create(SwedishCulture, ignoreCase: true)).ToList();
                        }
                        break;
                }

                Console.Clear();
                Console.WriteLine("List of students:");
                foreach (var student in students)
                {
                    Console.WriteLine($"• {student.StudentName} {student.StudentLastName}");

                }

                ClearConsoleAfterKeyPress();
            }
            //Labb-4-New method to show all active courses
            static void ShowAllActiveCourse(SchoolInfoDbContext context)
            {
                var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);
                var allActiveCourses = context.CourseOfferings
                    .Include(co => co.Course)
                    .Where(co => co.EndDate > dateNow || co.EndDate == null)
                    .ToList();


                foreach (var activeCourse in allActiveCourses)
                {
                    if (activeCourse.EndDate == null)
                    {
                        Console.WriteLine($"End date is not comming - {activeCourse.Course?.CourseName} ");
                    }
                    else
                    {
                        Console.WriteLine($"{activeCourse.EndDate:d} - {activeCourse.Course?.CourseName}");
                    }

                }
                ClearConsoleAfterKeyPress();

            }
            static void GetStudentsInClass(SchoolInfoDbContext context)
            {
                ShowAllClasees(context);
                string userInpputClass = InputValidation.ValidateStringInput("Enter the class name to get the students in that class:");
                while (!context.Classes.Any(c => c.ClassName.Equals(userInpputClass)))
                {
                    Console.WriteLine("Invalid class name. Please try again.");
                    userInpputClass = InputValidation.ValidateStringInput("Enter the class name to get the students in that class:");
                }

                var result = context.Students
                    .Include(с => с.Class)
                    .Where(s => s.Class != null && s.Class.ClassName == userInpputClass)
                    .ToList();

                var studentsClass = result.ToList();

                Console.WriteLine("Students from specific class:");
                foreach (var student in studentsClass)
                {
                    Console.WriteLine($"• {student.StudentName} {student.StudentLastName} Class: {student.Class?.ClassName}");
                }

                ClearConsoleAfterKeyPress();
            }


            static void AddNewStudent(SchoolInfoDbContext context)
            {
                ShowAllClasees(context);

                int classId = InputValidation.ValidateIntInput("Enter the class ID for the student:");
                while (!context.Classes.Any(c => c.ClassId == classId))
                {
                    Console.WriteLine("Invalid class ID. Please try again.");
                    classId = InputValidation.ValidateIntInput("Enter the class ID for the student:");
                }

                var newStudent = new Student
                {
                    StudentName = InputValidation.ValidateStringInput("Enter the student's first name:"),
                    StudentLastName = InputValidation.ValidateStringInput("Enter the student's last name:"),
                    Email = InputValidation.ValidateStringInput("Enter the student's email:"),
                    PersonNumber = InputValidation.ValidateStringInput("Enter the student's person number:"),
                    ClassId = classId
                };
                context.Students.Add(newStudent);
                context.SaveChanges();

                ClearConsoleAfterKeyPress();
            }

            static void ShowAllStaff(SchoolInfoDbContext context, int choice)
            {

                switch (choice)
                {
                    case 1:
                        var employees = context.Employees.ToList();
                        Console.WriteLine("Available staff:");
                        foreach (var employee in employees)
                        {
                            Console.WriteLine($"• {employee.EmployeeId} --> {employee.EmployeeName} {employee.EmployeeLastName}");
                        }

                        break;
                    case 2:
                        ShowAllRoles(context);
                        int roleId = InputValidation.ValidateIntInput("Enter the role ID to filter employees by role:");
                        while (!context.EmployeeRoles.Any(r => r.EmployeeRoleId == roleId))
                        {
                            Console.WriteLine("Invalid role ID. Please try again.");
                            roleId = InputValidation.ValidateIntInput("Enter the role ID to filter employees by role:");
                        }

                        var employeeRole = context.EmployeeRoleAssignments
                            .Include(e => e.Employee)
                            .Include(e => e.EmployeeRole)
                            .Where(e => e.EmployeeRoleId == roleId)
                            .ToList();

                        Console.WriteLine($"Employees role: {employeeRole.FirstOrDefault(e => e.EmployeeRoleId == roleId)?.EmployeeRole.RoleName}");
                        foreach (var employee in employeeRole)
                        {
                            Console.WriteLine($"•{employee.Employee.EmployeeName} {employee.Employee.EmployeeLastName}");
                        }



                        break;


                }
                ClearConsoleAfterKeyPress();

            }


            static void AddNewStaff(SchoolInfoDbContext context)
            {
                var newEmployee = new Employee
                {
                    EmployeeName = InputValidation.ValidateStringInput("Enter the employee's first name:"),
                    EmployeeLastName = InputValidation.ValidateStringInput("Enter the employee's last name:"),

                };
                context.Employees.Add(newEmployee);

                ShowAllRoles(context);
                int roleId = InputValidation.ValidateIntInput("Enter the role ID to filter employees by role:");
                while (!context.EmployeeRoles.Any(r => r.EmployeeRoleId == roleId))
                {
                    Console.WriteLine("Invalid role ID. Please try again.");
                    roleId = InputValidation.ValidateIntInput("Enter the role ID to filter employees by role:");
                }

                //Assign role to the new employee
                var role = context.EmployeeRoles.Find(roleId);

                var assignment = new EmployeeRoleAssignment
                {
                    EmployeeId = newEmployee.EmployeeId,
                    EmployeeRoleId = roleId
                };

                context.EmployeeRoleAssignments.Add(assignment);
                context.SaveChanges();
                //If role is teacher, add teacher-specific data
                if (roleId == 1)
                {
                    AddTeacherData(context, newEmployee.EmployeeId);
                }
                ClearConsoleAfterKeyPress();

            }


            static void AddTeacherData(SchoolInfoDbContext context, int employeeId)
            {
                var employee = context.Employees.Find(employeeId);
                if (employee == null)
                {
                    Console.WriteLine("Employee not found.");
                    return;
                }
                ShowAllClasees(context);
                int classId = InputValidation.ValidateIntInput("Enter the class ID to assign to the teacher:");
                while (!context.Classes.Any(c => c.ClassId == classId))
                {
                    Console.WriteLine("Invalid class ID. Please try again.");
                    classId = InputValidation.ValidateIntInput("Enter the class ID to assign to the teacher:");
                }
                var classEntity = context.Classes.Find(classId);

                if (classEntity != null)
                {
                    employee?.Classes.Add(classEntity);

                }

                int amountOfCourse = InputValidation.ValidateIntInput("Enter the number of courses to assign to the teacher: ");
                int counter = 0;

                while (counter < amountOfCourse)
                {
                    ShowAllCourses(context);  //Additional logic to add teacher-specific data

                    int courseOfferingId = InputValidation.ValidateIntInput("Enter the course offering  ID to assign to the teacher:");
                    while (!context.CourseOfferings.Any(c => c.CourseOfferingId == courseOfferingId))
                    {
                        Console.WriteLine("Invalid course ID. Please try again.");
                        courseOfferingId = InputValidation.ValidateIntInput("Enter the course ID to assign to the teacher:");
                    }

                    //Add course and class assignment to the teacher
                    var course = context.CourseOfferings
                        .Include(co => co.Course)
                        .FirstOrDefault(co => co.CourseOfferingId == courseOfferingId);

                    if (course != null)
                    {
                        employee?.CourseOfferings.Add(course);
                    }

                    counter++;
                    Console.Clear();
                }



                context.SaveChanges();




            }

            //Labb-4-New methods for counting teachers per department
            static void CountTeachersPerDepartment(SchoolInfoDbContext context)
            {
                var departmentCounts = context.EmployeeRoleAssignments
                    .Include(es => es.EmployeeRole)
                    .Include(es => es.Employee)
                    .GroupBy(es => new
                    {
                        es.EmployeeRole.EmployeeRoleId,
                        es.EmployeeRole.RoleName
                    })
                    .Select(g => new
                    {
                        EmployeeRoleName = g.Key.RoleName,
                        EmployeeCount = g.Select(g => g.Employee.EmployeeId).Count(),
                        Employees = g.Select(e => new
                        {
                            e.Employee.EmployeeName,
                            e.Employee.EmployeeLastName
                        }).ToList()

                    })
                    .ToList();


                foreach (var role in departmentCounts)
                {
                    Console.Write($"{role.EmployeeRoleName}: {role.EmployeeCount}\n");

                    foreach (var employee in role.Employees)
                    {
                        Console.WriteLine($"•{employee.EmployeeName} {employee.EmployeeLastName}");
                    }



                    Console.WriteLine();
                }


                ClearConsoleAfterKeyPress();
            }
            //Labb-4- New method to show information about all students
            static void ShowInformationAboutAllStudents(SchoolInfoDbContext context)
            {
                var allStudents = context.Grades
                    .Include(g => g.GradeScale)
                    .Include(g => g.Student)
                        .ThenInclude(c => c.Class)
                    .Include(g => g.CourseOffering)
                        .ThenInclude(co => co.Course).
                        Select(g => new
                        {
                            StudentName = g.Student.StudentName,
                            StudentLastName = g.Student.StudentLastName,
                            Class = g.Student.Class.ClassName,
                            Grades = $"{g.CourseOffering.Course.CourseName}: {g.GradeScale.Letter} ({g.GradeScale.Value} points)"
                        }).ToList();

                foreach (var student in allStudents)
                {
                    Console.WriteLine($"{student.StudentName} {student.StudentName} {student.Class} {student.Grades} ");
                }

                ClearConsoleAfterKeyPress();

            }

            // Labb-4-New method to rate a student
            static void SetGradeForStudent(SchoolInfoDbContext context)
            {

                ShowAllStudents(context);
                int studentId = InputValidation.ValidateIntInput("Enter the student ID to the grade:");
                while (!context.Students.Any(c => c.StudentId == studentId))
                {
                    Console.WriteLine("Invalid student ID. Please try again.");
                    studentId = InputValidation.ValidateIntInput("Enter the student ID to the grade:");
                }

                ShowAllCourses(context);
                int courseOfferingId = InputValidation.ValidateIntInput("Enter the course offering ID to rate the student in:");
                while (!context.CourseOfferings.Any(c => c.CourseOfferingId == courseOfferingId))
                {
                    Console.WriteLine("Invalid course offering ID. Please try again.");
                    courseOfferingId = InputValidation.ValidateIntInput("Enter the course offering ID to rate the student in:");
                }

                GradeScaleOptions(context);
                int gradeScaleId = InputValidation.ValidateIntInput("Enter the grade scale ID to assign to the student:");
                while (!context.GradeScales.Any(c => c.GradeScaleId == gradeScaleId))
                {
                    Console.WriteLine("Invalid grade scale ID. Please try again.");
                    gradeScaleId = InputValidation.ValidateIntInput("Enter the grade scale ID to assign to the student:");
                }

                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                ShowAllTeachers(context);
                int teacherIdWhoGaveTheGrade = InputValidation.ValidateIntInput("Enter the ID of the teacher who assigned the grade:");
                while (!context.Employees.Any(c => c.EmployeeId == teacherIdWhoGaveTheGrade))
                {
                    Console.WriteLine("Invalid teacher ID. Please try again.");
                    teacherIdWhoGaveTheGrade = InputValidation.ValidateIntInput("Enter the ID of the teacher who assigned the grade:");
                }

                var student= context.Students.FirstOrDefault(s=>s.StudentId==studentId);
                if (student==null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }

                bool alreadyExists = context.Grades.Any(g =>
                g.StudentId == studentId &&
                g.CourseOfferingId == courseOfferingId);


                if (alreadyExists)
                {
                    Console.WriteLine("This student already has a grade for this course.");
                    return;
                }
                
                using var transaction = context.Database.BeginTransaction();
                try
                {        
                    

                    var newGrade = new Grade
                    {
                        StudentId = studentId,
                        CourseOfferingId = courseOfferingId,
                        GradeScaleId = gradeScaleId,
                        DateOfIssue = today,
                        TeacherId = teacherIdWhoGaveTheGrade

                    };
                    context.Grades.Add(newGrade);
                    context.SaveChanges();
                    // save this trancastion.
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    //roll  back 
                    transaction.Rollback();
                    throw new Exception("Failed to save grade to database", ex);
                }

            }
            //Helpter methods to show data from the database



            static void ShowAllStudents(SchoolInfoDbContext context)
            {
                var students = context.Students.ToList();
                Console.WriteLine("Available students:");
                foreach (var student in students)
                {
                    Console.WriteLine($"• {student.StudentId} --> {student.StudentName} {student.StudentLastName}");
                }
            }
            static void GradeScaleOptions(SchoolInfoDbContext context)
            {
                var gradeScales = context.GradeScales.ToList();
                Console.WriteLine("Available grade scales:");
                foreach (var gradeScale in gradeScales)
                {
                    Console.WriteLine($"• {gradeScale.GradeScaleId} --> {gradeScale.Letter} ({gradeScale.Value} points)");
                }
            }
            static void ShowAllCourses(SchoolInfoDbContext context)
            {
                var courses = context.CourseOfferings.Include(c => c.Course).ToList();
                Console.WriteLine("Available courses:");
                foreach (var course in courses)
                {
                    Console.WriteLine($"{course.CourseOfferingId} -->{course.Course?.CourseName}");
                }

            }

            static void ShowAllRoles(SchoolInfoDbContext context)
            {
                var roles = context.EmployeeRoles.ToList();
                Console.WriteLine("Available roles:");
                foreach (var role in roles)
                {
                    Console.WriteLine($"• {role.EmployeeRoleId} -->{role.RoleName}");
                }
            }

            static void ShowAllClasees(SchoolInfoDbContext context)
            {

                var classes = context.Classes.ToList();



                Console.WriteLine("Available classes:");
                foreach (var className in classes)
                {
                    Console.WriteLine($"• {className.ClassId} -->{className.ClassName}");
                }
            }


            static void ShowAllTeachers(SchoolInfoDbContext context)
            {
                var teachers = context.EmployeeRoleAssignments
                    .Include(e => e.Employee)
                    .Include(e => e.EmployeeRole)
                    .Where(e => e.EmployeeRole.RoleName == "Teacher")
                    .ToList();
                Console.WriteLine("Available teachers:");
                foreach (var teacher in teachers)
                {
                    Console.WriteLine($"• {teacher.Employee.EmployeeId} --> {teacher.Employee.EmployeeName} {teacher.Employee.EmployeeLastName}");
                }
            }
            //Helper method to clear console after key press
            static void ClearConsoleAfterKeyPress()
            {
                Console.WriteLine("\nPress any button to continue");
                Console.ReadKey();
                Console.Clear();
            }



        }


    }
}
