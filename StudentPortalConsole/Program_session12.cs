// =====================================================================
// StudentPortalConsole — SESSION PROJECT (Style Guide Rule 20/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 12 — Delegates, Lambdas, Generics + LINQ
//
// THIS PROJECT IS DAY-READY (Rule 39). Everything already taught in
// Sessions 8-11 — IPrintable, Person, Student, Instructor, Course — is
// present below as REAL, WORKING, RUNNABLE code, along with today's
// seed data. Nothing here needs re-typing. Open it, press run, and it
// builds and prints the seed roster immediately.
//
// Only TODAY'S NEW content is left as TODOs. Each TODO sits exactly
// where its code will be written (Rule 40), and the numbers run
// strictly top-to-bottom through this file in the same order the
// lecture teaches them:
//
//   TODO 1-5    Block 1/2 helper methods + the generic Tracker class
//   TODO 6-10   Block 1/2, inside Main
//   TODO 11-12  Block 3 (LINQ core), inside Main
//   TODO 13-15  Block 4 (aggregates, GroupBy, Join), inside Main
//   TODO 16-19  Block 5 (deferred execution), inside Main
//   TODO 20     Block 5 (your own extension method) — see its note for
//               why it is the only item that lives BELOW Main
//
// For the full, correct, runnable version (do NOT peek until you've
// tried it yourself, or you're using it to check your own work), see:
// ../StudentPortalConsole_Complete/Program.cs
// =====================================================================

namespace StudentPortalConsole
{
    // =================================================================
    // CARRIED FORWARD FROM SESSION 11 — already taught, already working.
    // Do not re-type any of this; it is here so today starts instantly.
    //
    // These live at namespace level (beside Program, not inside it)
    // because today's TODO 20 adds a public extension method, and C#
    // forbids a public method from taking a parameter of a less
    // accessible type (CS0051). Anything nested inside the internal
    // Program class would be effectively internal.
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

    // ================== END OF CARRIED-FORWARD CODE ==================


    //delegate bool StudentFilter(Student s); // assign Methods to it  

    class Tracker<T>
    {
        private List<T> items = new();
        public void Add(T item)
        {
            items.Add(item);
            Console.WriteLine($"Tracker added item ! , now have {items.Count} item(s).");
        }

        public List<T> GetAll()
        {
            return items;
        }

    }

    internal class Program
    {
        // ===== Block 1 — passing behaviour instead of data =====

        // TODO 1: Write a method that takes a list of Students and one
        //         piece of filtering LOGIC, and returns a new list
        //         holding only the students that logic approves of. For
        //         the logic parameter, use the built-in generic delegate
        //         type meaning "takes one Student, gives back true or
        //         false" — remember that in that delegate family, the
        //         LAST type inside the angle brackets is the return
        //         type and everything before it is the inputs. Inside,
        //         loop over the source list and call the logic parameter
        //         as though it were a method, because it is one, adding
        //         each approved student to the result list.
        //         (The lecture reaches this by first declaring a custom
        //         delegate type by hand and then deleting it — you only
        //         need the final built-in form here.)

        //static List<Student> FilterByHighGpa(List<Student> source)
        //{
        //    var result = new List<Student>();
        //    foreach (var student in source)
        //    {
        //        if (student.Gpa > 3.0) // paramtere : student , return is true or false
        //        {
        //            result.Add(student);
        //        }
        //    }
        //    return result;
        //}

        //static List<Student> FilterByFinalYear(List<Student> source)
        //{
        //    var result = new List<Student>();
        //    foreach (var student in source)
        //    {
        //        if (student.YearOfStudy == 4)
        //        {
        //            result.Add(student);
        //        }
        //    }
        //    return result;
        //}

        //static List<Student> FilterStudents(List<Student> source , StudentFilter condition)
        //{
        //    var result = new List<Student>();
        //    foreach (var student in source)
        //    {
        //        if (condition(student)) // paramtere : student , return is true or false
        //        {
        //            result.Add(student);
        //        }
        //    }
        //    return result;
        //}
        //static void SayHello()
        //{
        //    Console.WriteLine("Hello");
        //}

        static List<Student> FilterStudents(List<Student> source, Func<Student, bool> condition)
        {
            var result = new List<Student>();
            foreach (var student in source)
            {
                if (condition(student)) // paramtere : student , return is true or false
                {
                    result.Add(student);
                }
            }
            return result;
        }

        static T? FindFirst<T>(List<T> items, Func<T, bool> condition) where T : class
        {
            foreach (var item in items)
            {
                if (condition(item))
                {
                    return item;
                }
            }
            return null;
        }


        // TODO 2: Write two small helper methods, each taking one
        //         Student and returning true or false, matching TODO 1's
        //         delegate shape exactly: one answering whether the
        //         student's GPA is above 3.0, the other answering
        //         whether the student is in year four.


        //static bool IsHighGpa(Student s) { return s.Gpa > 3.0; }
        //static bool IsFinalYear(Student s) { return s.YearOfStudy == 4; }
        //static bool IsInstructor(Instructor i) { return true; }

        // TODO 3: Write a second method taking a list of Students and
        //         one piece of logic that PERFORMS an action rather than
        //         answering a question — use the built-in delegate
        //         family that returns nothing at all, with Student as
        //         its type argument. Inside, loop over the list and call
        //         that logic once per student. Note there is no way to
        //         write this with the other delegate family, because the
        //         "returns nothing" keyword is not a real type and
        //         therefore cannot be used as a type argument.

        // ===== Block 2 — writing your own generics =====

        // TODO 4: Write a generic method named FindFirst that takes a
        //         list of some type plus a condition for that same type,
        //         and returns the first item satisfying the condition,
        //         or nothing at all if none does. Declare its type
        //         parameter in angle brackets after the method name, and
        //         use that same type parameter in all four places it
        //         belongs: the list's element type, the condition's
        //         input type, the loop variable's type, and the return
        //         type. Mark the return type as possibly-absent. Add a
        //         constraint after the parameter list stating the type
        //         parameter will always be a reference type — you need
        //         this specifically because the method returns nothing
        //         when no match is found, and that is only legal for
        //         reference types.

        // TODO 5: Define a generic class named Tracker, with one type
        //         parameter in angle brackets after the class name.
        //         Give it one private list whose element type is that
        //         type parameter, initialized empty. Add a public method
        //         taking one item of the type parameter's type, adding
        //         it to the list and printing how many items the tracker
        //         now holds. Add a second public method taking no
        //         parameters that returns the whole internal list.

        static void Main(string[] args)
        {
            // =========================================================
            // SEED DATA — carried forward and pre-typed (Rule 39).
            // These exact values are what every worked answer in the
            // Student Guide and Lab is based on. Do not change them.
            // =========================================================
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

            // Proof the project is day-ready: this runs as-is, right now,
            // before a single TODO below is completed.
            Console.WriteLine("=== Seed roster ===");
            foreach (Student s in students)
            {
                s.PrintBasicInfo();
            }
            Console.WriteLine();

            Action sayHello = () => Console.WriteLine("Hello"); // arrow function reads as goes to
            sayHello();



            //var student = new Student("Yara Adel", 2, 3.5);

            //Console.WriteLine(IsHighGpa(student));
            //Console.WriteLine(IsFinalYear(student));

            //StudentFilter highGpaFilter = IsHighGpa;
            //StudentFilter finalYearFilter = IsFinalYear;
            //Console.WriteLine(highGpaFilter(student));
            //Console.WriteLine(finalYearFilter(student));


            //var highGpaFilter = FilterStudents(students , IsHighGpa);
            //var finalYearsFilter = FilterStudents(students, IsFinalYear);

            var highGpaFilter = FilterStudents(students, s => s.Gpa > 3.0);
            var finalYearsFilter = FilterStudents(students, s => s.YearOfStudy == 4);

            Console.WriteLine($"High GPA students: {highGpaFilter.Count}");
            Console.WriteLine($"Final year students: {finalYearsFilter.Count}");

            // ===== Block 1, in Main =====

            // TODO 6: Call TODO 1's filter method twice, passing TODO 2's
            //         two helper methods BY NAME — with no parentheses
            //         after the name, since you are passing the method
            //         itself rather than calling it. Print the count of
            //         each resulting list. Expect 3 and 1.

            // TODO 7: Call TODO 1's filter method twice more, this time
            //         writing the two conditions inline as lambda
            //         expressions instead of naming the helper methods —
            //         a parameter name, then the arrow, then the same
            //         boolean expression each helper method's body used.
            //         Print both counts and confirm they match TODO 6's
            //         exactly.

            // TODO 8: Call TODO 3's action method once, passing a lambda
            //         that prints each student's full name.

            // ===== Block 2, in Main =====

            // TODO 9: Call TODO 4's generic method three separate times
            //         with three DIFFERENT collections — your students,
            //         your instructors, and your courses — each with an
            //         appropriate condition. Store each result in its own
            //         correctly-typed possibly-absent variable and print
            //         something from each, using the safe-navigation
            //         operator so a missing result cannot crash. Notice
            //         you never have to tell the compiler what the type
            //         parameter is; it works it out from the argument.

            var topStudent = FindFirst(students, s => s.Gpa > 3.5);
            var seniorInstructor = FindFirst(instructors, i => i.YearsOfExperience > 5);
            var heavyCourse = FindFirst(courses, c => c.Credits >= 4);

            Console.WriteLine($"Top student: {topStudent?.FullName}");
            Console.WriteLine($"Senior instructor: {seniorInstructor?.FullName}");
            Console.WriteLine($"Heavy course: {heavyCourse?.CourseName}");

            // TODO 10: Create two separate Tracker objects from TODO 5 —
            //          one whose type argument is Student, one whose type
            //          argument is Course — and add one appropriate item
            //          to each. Then, temporarily, try adding a Course to
            //          the Student tracker, observe the compile error the
            //          lecture demonstrates, write its error code as a
            //          comment, and comment that line out again.

            Tracker<Student> studentTracker = new();
            studentTracker.Add(students[0]);

            Tracker<Course> courseTracker = new();
            courseTracker.Add(courses[0]);

            // ===== Block 3, in Main =====

            // TODO 11: Using LINQ's filtering operator, build a list of
            //          students with GPA above 3.0, forcing it into a
            //          real list at the end. Print its count and confirm
            //          it matches TODO 6's answer — you have now solved
            //          the same problem three separate ways today.

            // Filtering with where LINQ
            var highGpaLinq = students.Where(s => s.Gpa > 3.0).ToList(); // Extenstion method
            Console.WriteLine($"High GPA students (LINQ): {highGpaLinq.Count}");

            // TODO 12: Build a chained query that filters students to GPA
            //          above 3.0, then sorts them by GPA highest first,
            //          then transforms each remaining student into just
            //          their full name, forcing the result into a real
            //          list. Print each name on its own line. Then write
            //          the EXACT same query a second time in query syntax
            //          — the SQL-like form beginning with the "from"
            //          keyword — and print its results too, confirming
            //          both produce identical output.

            List<string> names = students.Select(s => s.FullName).ToList();
            //foreach (var name in names)
            //{
            //    Console.WriteLine(name);
            //}

            // Method Syntax
            var topNames = students
                .Where(s => s.Gpa > 3.0)
                .OrderByDescending(s => s.Gpa)
                .Select(s => s.FullName) // TO Read : Anonymous Type
                .ToList();

            //Query Syntax
            var topNamesQuery = from student in students
                                where student.Gpa > 3.0
                                orderby student.Gpa descending
                                select student.FullName;


            // ===== Block 4, in Main =====

            // TODO 13: Print six aggregate values over your students: the
            //          total count; the count of those above GPA 3.0,
            //          passing the condition directly to the counting
            //          operator rather than filtering first; the average
            //          GPA formatted to two decimal places; the highest
            //          GPA; the lowest GPA; and whether any student at
            //          all has a GPA below 2.0.

            // TODO 14: Group your students by year of study using LINQ's
            //          grouping operator. Loop over the resulting groups
            //          with an outer loop printing each group's key and
            //          how many students it holds, then an inner loop
            //          printing each student's name within that group.
            //          Notice the groups do NOT come out sorted by key —
            //          they appear in the order each key was first
            //          encountered while walking your source list.

            // TODO 15: Join your instructors to your courses using LINQ's
            //          joining operator, matching each instructor's
            //          assigned-course-name against each course's name.
            //          Supply four things: the second collection, how to
            //          get the key from an item of the first collection,
            //          how to get the key from an item of the second, and
            //          what to build from each matched pair — here, a
            //          formatted line naming the instructor, the course,
            //          and its credit count. Print every resulting line.
            //          Then write the same join again in query syntax,
            //          remembering that join syntax uses its own
            //          dedicated matching keyword rather than the normal
            //          equality operator.

            // ===== Block 5, in Main =====

            // TODO 16: Prove deferred execution to yourself. Build a
            //          filtered query over students with GPA above 3.0
            //          but do NOT force it into a real list. On the very
            //          next line, add a fifth student — Layla Mostafa,
            //          year 2, GPA 3.7 — to the underlying list. Only
            //          THEN print the query's count. Write down your
            //          prediction before running it.

            // TODO 17: Remove that fifth student again, so every result
            //          below still matches the Student Guide's stated
            //          answers.

            // TODO 18: Write the multiple-enumeration anti-pattern
            //          deliberately, so you can recognize it in real code
            //          later: build a filtered query WITHOUT forcing it
            //          into a list, then consume it three separate times
            //          — once to print its count, once in a loop printing
            //          names, once to compute an average. Add a comment
            //          above it stating how many times the filtering work
            //          actually runs here.

            // TODO 19: Now write the corrected version directly beneath
            //          it: the same filter, but forced into a real list
            //          immediately, then the same three consumptions
            //          reading from that list instead. Note that reading
            //          the count of a real list uses a property with no
            //          parentheses, unlike the LINQ counting method.

            // TODO 20 (part two — the usage): Use your own extension
            //          method in a chain: call it on your students list,
            //          then chain LINQ's alphabetical sorting operator
            //          onto it, then transform each result to just the
            //          full name, then force it into a real list, then
            //          print each name. Confirm it returns exactly the
            //          two students at or above GPA 3.5. The method
            //          itself is defined in TODO 20's other half, at the
            //          very bottom of this file.



            // start solving the lab task for session 12 :

            // part B :
            // B1 : not compile because we can not use void in the context 
            //static void DoSomething(List<Student> list, Func<Student, void> action) { }

            // B2 : not compile because it was no exitance for isTopStudent in the current context
            //List<Student> result = FilterStudents(students, IsTopStudent());

            // B3 : not compile because list<bool> dont return student it return bool and cannot store the results
            //List<Student> result = students.Select(s => s.Gpa > 3.0).ToList();

            //  Part C: 

            // lab id = 23 so we can use it in the equation 2.0 + ((23 mod 5) * 0.4) = 3.2

            // c1 : we are make a list of students with gpa ( 3.2 ) and year of study ( 4 ) and return the list of students that match the condition you send
            static List<Student> FilterStudents(
            List<Student> students,
            Func<Student, bool> condition)
            {
                List<Student> result = new List<Student>();

                foreach (Student student in students)
                {
                    if (condition(student))
                    {
                        result.Add(student);
                    }
                }

                return result;
            }

            // c2 : we use the 3.2 as a condition to filter the students and return the above of 3.2 
            // number-Gpa = 3.2
            static bool IsAboveMyGpa(Student s)
            {
                return s.Gpa >= 3.2;
            }
            List<Student> result = FilterStudents(students, IsAboveMyGpa);
            Console.WriteLine($"Number of students with GPA >= 3.2 is ( {result.Count} )");


            // c3 : using the lambda to filter by gpa 3.2 
            List<Student> result2 = FilterStudents(students, s => s.Gpa >= 3.2);
            Console.WriteLine($"Number of students with GPA with lambda >= 3.2 is ( {result2.Count} )");

            // c4 : using the lambda and applytoall to get the fullname and gpa of students 
            static void ApplyToAll(List<Student> students, Action<Student> operation)
            {
                foreach (Student student in students)
                {
                    operation(student);
                }
            }
            ApplyToAll(students, s => Console.WriteLine($"Student: {s.FullName}, GPA: {s.Gpa}"));

            // c5 : in c# the function must return a value so we cannot use it in this case because it has  no retuen value it is just print the name,gps(console.write.line)


            // Part D :

            // my tracker capcity is 23 mode 3 +2 = 2

            // 1 : create a tracker for students and add a student to it
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
            // GPA Threshold = 2
            Student? firstStudent = FindFirst(students, s => s.Gpa >= 2);

            if (firstStudent != null)
            {
                Console.WriteLine($"Student: ( {firstStudent.FullName} ), GPA: {firstStudent.Gpa}");
            }
            else
            {
                Console.WriteLine("No matching student found.");
            }

            // 2 : caling it 3 times with student , instractor and courses

            // 2.1 : calling it with students
            // GPA Threshold = 3.2
            //Student? student = FindFirst(students, s => s.Gpa >= 3.2);

            //if (student != null)
            //{
            //    Console.WriteLine($"Student: {student.FullName}, GPA: {student.Gpa}");
            //}
            //else
            //{
            //    Console.WriteLine("No matching student found.");
            //} 
            // as the same in line 674

            // 2.2 : calling it with instructors
            Instructor? instructor = FindFirst(instructors, i => i.YearsOfExperience >= 10);

            if (instructor != null)
            {
                Console.WriteLine($"Instructor: {instructor.FullName}, Experience: {instructor.YearsOfExperience} years");
            }
            else
            {
                Console.WriteLine("No matching instructor found.");
            }

            // 2.3 : calling it with courses
            Course? course = FindFirst(courses, c => c.Credits >= 4);

            if (course != null)
            {
                Console.WriteLine($"Course: {course.CourseName}, Credits: {course.Credits}");
            }
            else
            {
                Console.WriteLine("No matching course found.");
            }


            // 3 : create a generic class tracker <T> 
            // written in line 786
            // add 5 objects to it 
            Tracker<Student> tracker = new Tracker<Student>();

            tracker.Add(new Student("Yara", 2, 3.5));
            tracker.Add(new Student("Omar", 3, 2.8));
            tracker.Add(new Student("Nada", 1, 3.9));
            tracker.Add(new Student("Kareem", 4, 3.2));

            // Fifth item (exceeds capacity)
            tracker.Add(new Student("Ahmed", 2, 3.1));

            // 4 : 
            Tracker<Student> studentTrackers= new Tracker<Student>();

        }


        // =================================================================
        // TODO 20 (part one — the definition).
        //
        // This is the ONLY item in this file that does not sit next to the
        // code that uses it, and the reason is a hard C# rule rather than a
        // style choice: extension methods must be declared in a top-level,
        // non-generic static class. Putting this inside Program produces
        // CS1109. Its usage is the last TODO inside Main, directly above.
        // =================================================================

        // TODO 20: Define a class to hold today's custom LINQ operator. It
        //          must be marked both public and static, must NOT be
        //          generic, and must stay here at namespace level. Inside
        //          it, define a public static method named HonorRoll whose
        //          single parameter is preceded by the keyword `this` —
        //          that keyword is the entire thing that turns an ordinary
        //          static method into an extension method. The parameter's
        //          type should be the general "something you can walk
        //          through" collection interface with Student as its
        //          element type, and the method should return that same
        //          type. Its body should return the source collection
        //          filtered down to students whose GPA is at or above 3.5,
        //          using the same LINQ filtering operator from TODO 11.

        #region 📋 Full TODO Checklist (collapse this region for a quick overview)
        // --- In class Program, above Main ---
        // 1.  Filter method taking a "Student in, bool out" delegate            [Block 1]
        // 2.  Two named helper conditions: high GPA, final year                 [Block 1]
        // 3.  Action method taking a "Student in, nothing out" delegate         [Block 1]
        // 4.  Generic FindFirst method with a reference-type constraint         [Block 2]
        // 5.  Generic Tracker class                                             [Block 2]
        // --- Inside Main (seed data is already provided) ---
        // 6.  Call the filter passing the named methods (no parentheses)        [Block 1]
        // 7.  Call the filter again passing lambdas instead                     [Block 1]
        // 8.  Call the action method with a printing lambda                     [Block 1]
        // 9.  Call FindFirst three times, on three different collection types   [Block 2]
        // 10. Two Trackers with different type arguments; record the error code [Block 2]
        // 11. Repeat the GPA filter using LINQ's Where                          [Block 3]
        // 12. Chained Where/OrderByDescending/Select, in BOTH syntaxes          [Block 3]
        // 13. The six aggregate values                                          [Block 4]
        // 14. GroupBy year; note the groups are not sorted                      [Block 4]
        // 15. Join instructors to courses, in BOTH syntaxes                     [Block 4]
        // 16. Prove deferred execution with the late-added fifth student        [Block 5]
        // 17. Remove that fifth student again                                   [Block 5]
        // 18. Write the multiple-enumeration anti-pattern deliberately          [Block 5]
        // 19. Write the ToList() fix directly beneath it                        [Block 5]
        // 20. Use HonorRoll in a chain (definition is below Main — see note)    [Block 5]
        #endregion
    }
}



// generic class tracjer <T>
class Tracker<T>
{
    private List<T> items = new List<T>();

    private const int Capacity = 4;   // Tracker Capacity = 4

    public void Add(T item)
    {
        if (items.Count >= Capacity)
        {
            Console.WriteLine("Tracker is full. Cannot add more items.");
            return;
        }

        items.Add(item);
        Console.WriteLine($"Tracker now holds {items.Count} item(s).");
    }

    public List<T> GetAll()
    {
        return items;
    }
}