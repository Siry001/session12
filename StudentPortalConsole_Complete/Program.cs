// =====================================================================
// StudentPortalConsole_Complete — FULL WORKING FALLBACK (Rule 20)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 12 — Delegates, Lambdas, Generics + LINQ
//
// Complete, correct, runnable version of everything taught live today.
// Matches Instructor_Guide_EN.md and Student_Guide.md exactly (Rule 15).
//
// Carries forward Session 11's Person/Student/Instructor/Course model
// (abstract Person, virtual/override, IPrintable) UNCHANGED, and adds
// today's five blocks:
//   Block 1 — delegates, Func<>/Action<>, lambdas
//   Block 2 — a generic method (FindFirst<T>) and generic class (Tracker<T>)
//   Block 3 — LINQ Where/Select/OrderBy, both syntaxes
//   Block 4 — aggregates, GroupBy, Join
//   Block 5 — deferred execution, multiple enumeration, custom extension method
//
// NOTE ON FILE SHAPE: the model classes sit at NAMESPACE level here,
// beside Program, rather than nested inside it. Rule 34 is fully
// satisfied — there are no top-level statements, and all executable
// code lives in class Program's static void Main. The classes must be
// at namespace level today specifically because this session adds an
// extension method, and an extension method's parameter type cannot be
// less accessible than the extension class itself (CS0051).
//
// Run with: dotnet run (or Visual Studio's Start Without Debugging)
// =====================================================================

using StudentPortalConsole;

namespace StudentPortalConsole
{
    // =================================================================
    // Session 11 model — carried forward UNCHANGED
    // =================================================================
    public interface IPrintable
    {
        void PrintDetails();
    }

    public abstract class Person
    {
        protected string fullName;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public Person(string fullName)
        {
            this.fullName = fullName;
        }

        public virtual void PrintBasicInfo()
        {
            Console.WriteLine($"Person: {FullName}");
        }

        public abstract string GetRoleDescription();
    }

    public class Student : Person, IPrintable
    {
        private static int totalStudentsCreated = 0;
        private int yearOfStudy;
        private double gpa;

        public int YearOfStudy
        {
            get { return yearOfStudy; }
            set { if (value >= 1 && value <= 4) yearOfStudy = value; }
        }

        public double Gpa
        {
            get { return gpa; }
            set { if (value >= 0.0 && value <= 4.0) gpa = value; }
        }

        public Student(string fullName, int yearOfStudy, double gpa)
            : base(fullName)
        {
            YearOfStudy = yearOfStudy;
            Gpa = gpa;
            totalStudentsCreated++;
        }

        public Student(string fullName, int yearOfStudy)
            : this(fullName, yearOfStudy, 0.0)
        {
        }

        public override void PrintBasicInfo()
        {
            base.PrintBasicInfo();
            Console.WriteLine($"  Year {YearOfStudy}, GPA {Gpa:F2}");
        }

        public override string GetRoleDescription()
        {
            return "Student";
        }

        public void PrintDetails()
        {
            PrintBasicInfo();
        }

        public static int GetTotalStudents()
        {
            return totalStudentsCreated;
        }
    }

    public class Instructor : Person, IPrintable
    {
        private int yearsOfExperience;

        public int YearsOfExperience
        {
            get { return yearsOfExperience; }
            set { if (value >= 0) yearsOfExperience = value; }
        }

        public string? AssignedCourseName { get; set; }

        public Instructor(string fullName, int yearsOfExperience)
            : base(fullName)
        {
            YearsOfExperience = yearsOfExperience;
        }

        public override void PrintBasicInfo()
        {
            base.PrintBasicInfo();
            Console.WriteLine($"  {YearsOfExperience} years of experience");
        }

        public override string GetRoleDescription()
        {
            return "Instructor";
        }

        public void PrintDetails()
        {
            PrintBasicInfo();
        }
    }

    public class Course : IPrintable
    {
        private static int totalCoursesCreated = 0;
        private string courseName;
        private int credits;
        private List<Student> enrolledStudents = new List<Student>();

        public string CourseName
        {
            get { return courseName; }
            set { courseName = value; }
        }

        public int Credits
        {
            get { return credits; }
            set { if (value >= 1 && value <= 6) credits = value; }
        }

        public Course(string courseName, int credits)
        {
            CourseName = courseName;
            Credits = credits;
            totalCoursesCreated++;
        }

        public void EnrollStudent(Student s)
        {
            enrolledStudents.Add(s);
        }

        public void PrintRoster()
        {
            Console.WriteLine($"=== {courseName} Roster ({enrolledStudents.Count} enrolled) ===");
            foreach (Student s in enrolledStudents)
            {
                s.PrintBasicInfo();
            }
        }

        public void PrintDetails()
        {
            PrintRoster();
        }

        public static int GetTotalCourses()
        {
            return totalCoursesCreated;
        }
    }

    // =================================================================
    // Block 2 — a generic CLASS. T is chosen once, when the object is
    // created, and shared by every member of that object.
    // =================================================================
    public class Tracker<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            if (items.Count >= 4) // Lab ID 23 => Capacity = 4
            {
                Console.WriteLine("Tracker is full.");
                return;
            }

            items.Add(item);
            Console.WriteLine($"Count = {items.Count}");
        }

        public List<T> GetAll()
        {
            return items;
        }
    }

    // =================================================================
    // Block 5 — Custom LINQ extension method.
    // MUST be a TOP-LEVEL, non-generic static class. Moving this inside
    // class Program produces CS1109: "Extension methods must be defined
    // in a top level static class".
    // =================================================================
    public static class StudentQueryExtensions
    {
        // The `this` keyword on the first parameter is what makes this an
        // extension method — it makes HonorRoll() appear to belong to
        // IEnumerable<Student>, so it chains like a built-in operator.
        // Deferred, because its body just returns a Where(...) query.
        public static IEnumerable<Student> HonorRoll(this IEnumerable<Student> source)
        {
            return source.Where(s => s.Gpa >= 3.5);
        }
    }

    internal class Program
    {
        // =============================================================
        // Block 1 — delegate-based filtering
        // =============================================================

        // The custom delegate type taught FIRST, then replaced by
        // Func<Student, bool>. Kept here (commented) purely as a reference
        // for what Func<> actually means underneath.
        // delegate bool StudentFilter(Student s);

        // ONE method replacing the Warm-Up's two near-identical filters.
        // The condition is passed IN, so this method never needs to know
        // what is actually being checked.
        static List<Student> FilterStudents(List<Student> source, Func<Student, bool> condition)
        {
            List<Student> result = new List<Student>();
            foreach (Student s in source)
            {
                if (condition(s))
                {
                    result.Add(s);
                }
            }
            return result;
        }

        // Named methods matching Func<Student, bool>'s shape. Passed WITHOUT
        // parentheses (the method itself, not its result).
        static bool IsHighGpa(Student s)
        {
            return s.Gpa > 3.0;
        }

        static bool IsFinalYear(Student s)
        {
            return s.YearOfStudy == 4;
        }

        // Action<Student> — takes a Student, returns NOTHING. Note there is
        // no such thing as Func<Student, void>, because void is not a type.
        static void ApplyToAll(List<Student> source, Action<Student> operation)
        {
            foreach (Student s in source)
            {
                operation(s);
            }
        }

        // =============================================================
        // Block 2 — a generic METHOD.
        // `where T : class` is required because this returns null when
        // nothing matches, and null is only legal for reference types.
        // =============================================================
        static T? FindFirst<T>(List<T> items, Func<T, bool> condition) where T : class
        {
            foreach (T item in items)
            {
                if (condition(item))
                {
                    return item;
                }
            }
            return null;
        }

        static void Main(string[] args)
        { 
        
        
        
        
        
            // ===== Seed data — identical in SG, IG, and Lab (Rule 15) =====
            List<Student> students = new List<Student>();
            students.Add(new Student("Yara Adel", 2, 3.5));
            students.Add(new Student("Omar Hesham", 3, 2.8));
            students.Add(new Student("Nada Samir", 1, 3.9));
            students.Add(new Student("Kareem Fouad", 4, 3.2));

            List<Instructor> instructors = new List<Instructor>();
            Instructor hamdy = new Instructor("Hamdy", 10);
            hamdy.AssignedCourseName = "Web Development Using .NET";
            instructors.Add(hamdy);

            Instructor mona = new Instructor("Mona Khalil", 6);
            mona.AssignedCourseName = "Database Fundamentals";
            instructors.Add(mona);

            List<Course> courses = new List<Course>();
            courses.Add(new Course("Web Development Using .NET", 4));
            courses.Add(new Course("Database Fundamentals", 3));

            // =========================================================
            // BLOCK 1 — delegates, Func<>, lambdas
            // =========================================================
            Console.WriteLine("===== BLOCK 1: Delegates and Lambdas =====");

            // Passing NAMED methods (no parentheses — the method itself)
            List<Student> highGpa = FilterStudents(students, IsHighGpa);
            List<Student> finalYears = FilterStudents(students, IsFinalYear);
            Console.WriteLine($"High GPA count: {highGpa.Count}");      // 3
            Console.WriteLine($"Final year count: {finalYears.Count}"); // 1

            // Exactly the same thing, written inline as lambdas
            List<Student> highGpaLambda = FilterStudents(students, s => s.Gpa > 3.0);
            List<Student> finalYearsLambda = FilterStudents(students, s => s.YearOfStudy == 4);
            Console.WriteLine($"High GPA (lambda): {highGpaLambda.Count}");
            Console.WriteLine($"Final year (lambda): {finalYearsLambda.Count}");

            // Action<Student> — does something, returns nothing
            Console.WriteLine("-- ApplyToAll printing every name --");
            ApplyToAll(students, s => Console.WriteLine($"   {s.FullName}"));

            // =========================================================
            // BLOCK 2 — generics
            // =========================================================
            Console.WriteLine();
            Console.WriteLine("===== BLOCK 2: Generics =====");

            // One generic method, three different types — T inferred each time
            Student? topStudent = FindFirst(students, s => s.Gpa > 3.5);
            Instructor? senior = FindFirst(instructors, i => i.YearsOfExperience >= 10);
            Course? heavyCourse = FindFirst(courses, c => c.Credits >= 4);

            Console.WriteLine($"Top student: {topStudent?.FullName}");        // Nada Samir
            Console.WriteLine($"Senior instructor: {senior?.FullName}");      // Hamdy
            Console.WriteLine($"Heavy course: {heavyCourse?.CourseName}");    // Web Development Using .NET

            // A generic class — each closed type accepts only its own T
            Tracker<Student> studentTracker = new Tracker<Student>();
            studentTracker.Add(new Student("Yara Adel", 2, 3.5));

            Tracker<Course> courseTracker = new Tracker<Course>();
            courseTracker.Add(new Course("Database Fundamentals", 3));

            // studentTracker.Add(new Course("Machine Learning", 3));
            //   ^ CS1503: cannot convert from 'Course' to 'Student' — the
            //     compile-time guarantee that List<object> could never give.

            // =========================================================
            // BLOCK 3 — LINQ core operators
            // =========================================================
            Console.WriteLine();
            Console.WriteLine("===== BLOCK 3: LINQ Core =====");

            // Where — the same job FilterStudents did, already written for us
            List<Student> highGpaLinq = students.Where(s => s.Gpa > 3.0).ToList();
            Console.WriteLine($"LINQ high GPA count: {highGpaLinq.Count}");   // 3

            // Select TRANSFORMS each item — it does not filter
            List<string> names = students.Select(s => s.FullName).ToList();
            Console.WriteLine($"All names: {string.Join(", ", names)}");

            // Chained: filter, then sort, then transform.
            // Order matters — Select must come last here, because once each
            // Student becomes a string, s.Gpa no longer exists to sort by.
            List<string> topNames = students
                .Where(s => s.Gpa > 3.0)
                .OrderByDescending(s => s.Gpa)
                .Select(s => s.FullName)
                .ToList();

            Console.WriteLine("-- Top names (method syntax) --");
            foreach (string n in topNames)
            {
                Console.WriteLine($"   {n}");   // Nada Samir, Yara Adel, Kareem Fouad
            }

            // Identical query in QUERY syntax — the compiler rewrites this
            // into the method-syntax version above before compiling.
            var topNamesQuery = from s in students
                                where s.Gpa > 3.0
                                orderby s.Gpa descending
                                select s.FullName;

            Console.WriteLine("-- Top names (query syntax) --");
            foreach (string n in topNamesQuery)
            {
                Console.WriteLine($"   {n}");
            }

            // =========================================================
            // BLOCK 4 — aggregates, GroupBy, Join
            // =========================================================
            Console.WriteLine();
            Console.WriteLine("===== BLOCK 4: Aggregates, GroupBy, Join =====");

            Console.WriteLine($"Total students: {students.Count()}");                  // 4
            Console.WriteLine($"Above 3.0: {students.Count(s => s.Gpa > 3.0)}");       // 3
            Console.WriteLine($"Average GPA: {students.Average(s => s.Gpa):F2}");      // 3.35
            Console.WriteLine($"Highest GPA: {students.Max(s => s.Gpa)}");             // 3.9
            Console.WriteLine($"Lowest GPA: {students.Min(s => s.Gpa)}");              // 2.8
            Console.WriteLine($"Anyone failing: {students.Any(s => s.Gpa < 2.0)}");    // False

            // GroupBy — buckets appear in FIRST-ENCOUNTERED key order (2, 3, 1, 4),
            // NOT sorted. Chain .OrderBy(g => g.Key) if sorted groups are wanted.
            Console.WriteLine("-- Students grouped by year --");
            var byYear = students.GroupBy(s => s.YearOfStudy);
            foreach (var group in byYear)
            {
                Console.WriteLine($"Year {group.Key}: {group.Count()} student(s)");
                foreach (Student s in group)
                {
                    Console.WriteLine($"   {s.FullName}");
                }
            }

            // Join — INNER JOIN semantics. An instructor whose AssignedCourseName
            // matches no Course produces NO row at all: no error, no blank line.
            Console.WriteLine("-- Who teaches what (method syntax) --");
            var teaching = instructors.Join(
                courses,                        // the second collection
                i => i.AssignedCourseName,      // key from the FIRST collection
                c => c.CourseName,              // key from the SECOND collection
                (i, c) => $"{i.FullName} teaches {c.CourseName} ({c.Credits} credits)"
            );

            foreach (string line in teaching)
            {
                Console.WriteLine($"   {line}");
            }

            // The same join in query syntax — note `equals`, not `==`
            Console.WriteLine("-- Who teaches what (query syntax) --");
            var teachingQuery = from i in instructors
                                join c in courses on i.AssignedCourseName equals c.CourseName
                                select $"{i.FullName} teaches {c.CourseName} ({c.Credits} credits)";

            foreach (string line in teachingQuery)
            {
                Console.WriteLine($"   {line}");
            }

            // =========================================================
            // BLOCK 5 — deferred execution + custom extension method
            // =========================================================
            Console.WriteLine();
            Console.WriteLine("===== BLOCK 5: Deferred Execution =====");

            // Deferred execution proof: the query is written BEFORE Layla is
            // added, but does not RUN until Count() consumes it — so Layla
            // is included, and this prints 4, not 3.
            var deferredQuery = students.Where(s => s.Gpa > 3.0);
            students.Add(new Student("Layla Mostafa", 2, 3.7));
            Console.WriteLine($"Deferred count (includes Layla): {deferredQuery.Count()}");   // 4

            // Remove Layla again so the remaining examples match the guides.
            students.RemoveAt(students.Count - 1);

            // ⚠️ MULTIPLE ENUMERATION — this runs the filter THREE times.
            // Invisible over 4 in-memory items; three database round-trips
            // once this same shape points at EF Core in Session 13.
            var highAchievers = students.Where(s => s.Gpa > 3.0);
            Console.WriteLine($"Count (run 1): {highAchievers.Count()}");
            foreach (Student s in highAchievers)                 // run 2
            {
                Console.WriteLine($"   {s.FullName}");
            }
            Console.WriteLine($"Average (run 3): {highAchievers.Average(s => s.Gpa):F2}");

            // ✅ THE FIX — one ToList() forces a single execution; every read
            // afterward is just reading a real in-memory list.
            var highAchieversList = students.Where(s => s.Gpa > 3.0).ToList();
            Console.WriteLine($"Fixed count: {highAchieversList.Count}");   // .Count property, not .Count()
            foreach (Student s in highAchieversList)
            {
                Console.WriteLine($"   {s.FullName}");
            }
            Console.WriteLine($"Fixed average: {highAchieversList.Average(s => s.Gpa):F2}");

            // Custom extension method, chaining exactly like a built-in operator
            Console.WriteLine("-- Honor roll (custom extension method) --");
            List<string> honorNames = students
                .HonorRoll()                       // ours
                .OrderBy(s => s.FullName)          // built-in, chained straight on
                .Select(s => s.FullName)
                .ToList();

            foreach (string n in honorNames)
            {
                Console.WriteLine($"   {n}");      // Nada Samir, Yara Adel
            }

            Console.WriteLine();
            Console.WriteLine("Done.");

            // My Threshold = 3.2

            static List<Student> FilterStudents(List<Student> students, Func<Student, bool> condition)
            {
                List<Student> result = new List<Student>();

                foreach (Student s in students)
                {
                    if (condition(s))
                    {
                        result.Add(s);
                    }
                }

                return result;
            }

            static bool IsAboveMyThreshold(Student s)
            {
                return s.Gpa > 3.2; // My Threshold = 3.2
            }

            List<Student> result1 = FilterStudents(students, IsAboveMyThreshold);

            Console.WriteLine(result1.Count);


            List<Student> result2 = FilterStudents(students, s => s.Gpa > 3.2); // My Threshold = 3.2

            Console.WriteLine(result2.Count);

            static void ApplyToAll(List<Student> students, Action<Student> action)
            {
                foreach (Student s in students)
                {
                    action(s);
                }
            }

            ApplyToAll(students, s =>
            {
                Console.WriteLine($"{s.FullName} - {s.Gpa}");
            });


            // We can't use Func<> because Func must return a value.
            // ApplyToAll only performs an action and doesn't return anything,
            // so Action<Student> is the correct delegate.





            static T? FindFirst<T>(List<T> items, Func<T, bool> condition) where T : class
            {
                foreach (T item in items)
                {
                    if (condition(item))
                    {
                        return item;
                    }
                }

                return null;
            }



            Student? student = FindFirst(students, s => s.Gpa > 3.5);

            if (student != null)
            {
                Console.WriteLine(student.FullName);
            }

            Instructor? instructor = FindFirst(instructors, i => i.YearsOfExperience > 5);

            if (instructor != null)
            {
                Console.WriteLine(instructor.FullName);
            }

            Course? course = FindFirst(courses, c => c.Credits == 4);

            if (course != null)
            {
                Console.WriteLine(course.CourseName);
            }


            Tracker<Student> studentTracker = new Tracker<Student>();
            studentTracker.Add(students[0]);
            studentTracker.Add(students[1]);
            studentTracker.Add(students[2]);
            studentTracker.Add(students[3]);
            studentTracker.Add(students[0]); // Should print: Tracker is full.


            Tracker<Student> studentTracker = new Tracker<Student>();

            Tracker<Course> courseTracker = new Tracker<Course>();

            // CS1503: Argument 1: cannot convert from 'Course' to 'Student'
            // studentTracker.Add(courses[0]);

            static void PrintAllNames<T>(List<T> items)
            {
                foreach (T item in items)
                {
                    Console.WriteLine(item.FullName);
                }
                PrintAllNames(students);
            }


            // Year Filter = 4

            List<string> yearStudents = students
                .Where(s => s.YearOfStudy == 4)
                .Select(s => s.FullName)
                .ToList();

            foreach (string name in yearStudents)
            {
                Console.WriteLine(name);
            }



            students
    .OrderByDescending(s => s.Gpa)
    .ToList()
    .ForEach(s =>
    {
        Console.WriteLine($"{s.FullName} - {s.Gpa:F2}");
    });


            // My Threshold = 3.2

            List<string> result = students
                .Where(s => s.Gpa > 3.2)
                .OrderBy(s => s.FullName)
                .Select(s => s.FullName)
                .ToList();

            foreach (string name in result)
            {
                Console.WriteLine(name);
            }



            var result =
    from s in students
    where s.Gpa > 3.0
    orderby s.FullName
    select s.FullName;

            foreach (string name in result)
            {
                Console.WriteLine(name);
            }


            var result =
    from s in students
    where s.YearOfStudy <= 2
    select $"{s.FullName} - Year {s.YearOfStudy}";

            foreach (string item in result)
            {
                Console.WriteLine(item);
            }


            // If Select comes before OrderBy,
            // Select changes Student into string.
            // Then s.Gpa no longer exists, so OrderBy(s => s.Gpa)
            // will not compile.
            // Error: CS1061



        }
        class Tracker<T>
        {
            private List<T> items = new List<T>();

            public void Add(T item)
            {
                if (items.Count >= 4) // Tracker Capacity = 4 (Lab ID = 23)
                {
                    Console.WriteLine("Tracker is full.");
                    return;
                }

                items.Add(item);
                Console.WriteLine($"Count = {items.Count}");
            }

            public List<T> GetAll()
            {
                return items;
            }

            
            

        }

    }

    }
}

// part b :

// b1
// Doesn't compile because Func<T, TResult> must return a value.
// It can't use void as the return type.
// Action<Student> should be used instead.

// b2
// Doesn't compile because IsTopStudent() is called without passing
// the required Student parameter.
// We should pass IsTopStudent instead of IsTopStudent().

// b3
// Doesn't compile because Select returns IEnumerable<bool>,
// and after ToList() it becomes List<bool>,
// not List<Student>.




// part c :
// labId = 23
// 23 % 5 = 3
// Threshold = 2.0 + (3 × 0.4) = 3.2
// My Threshold = 3.2



// part d :

// d1 
// Returns the first matching item or null.
// where T : class is required because only reference types can return null.


