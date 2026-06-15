using studentgrader.models;
using studentgrader.services;

namespace studentgrader.ui;

/// for user interface
public class ConsoleUI(GradeService service)
{

    private const ConsoleColor Accent   = ConsoleColor.Cyan;
    private const ConsoleColor Success  = ConsoleColor.Green;
    private const ConsoleColor Warning  = ConsoleColor.Yellow;
    private const ConsoleColor Error    = ConsoleColor.Red;
    private const ConsoleColor Muted    = ConsoleColor.DarkGray;
    private const ConsoleColor Heading  = ConsoleColor.White;


    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DrawBanner();


        bool running = true;
        while (running)
        {
            DrawMenu();
            string choice = Prompt("Your choice").Trim();

            Console.WriteLine();
            running = HandleChoice(choice);
            if (running) PauseForUser();
        }

        Write("\n  Goodbye! \n", Accent);
    }


    private void DrawBanner()
    {
        Console.Clear();
        WriteColor(Accent);
        Console.WriteLine("student grade management system");
        WriteColor(Muted);
        Console.ResetColor();
    }

    private void DrawMenu()
    {
        Console.WriteLine();
        WriteColor(Heading);
        Console.WriteLine(" MAIN MENU ");
        Console.ResetColor();

        MenuItem("1", "Add student");
        MenuItem("2", "Show all students");
        MenuItem("3", "look for student");
        MenuItem("4", "Change grade");
        MenuItem("5", "Remove student");
        MenuItem("6", "Calculate average grade");
        MenuItem("7", "highest & lowest grades");
        MenuItem("8", "Grade distribution");
        MenuItem("0", "Exit");

        WriteColor(Heading);
        Console.ResetColor();
    }

    private static void MenuItem(string key, string label)
    {
        WriteColor(Accent);
        Console.Write($"  │  [{key}]");
        Console.ResetColor();
        Console.WriteLine($"  {label}");
    }


    private bool HandleChoice(string choice)
    {
        switch (choice)
        {
            case "1": AddStudent();           break;
            case "2": DisplayAll();           break;
            case "3": SearchStudent();        break;
            case "4": UpdateGrade();          break;
            case "5": RemoveStudent();        break;
            case "6": ShowAverage();          break;
            case "7": ShowHighLow();          break;
            case "8": ShowDistribution();     break;
            case "0": return false;
            default:
                WriteError("Invalid. Please enter a number from above.");
                break;
        }
        return true;
    }

    
    private void AddStudent()
    {
        WriteHeading("Add new student");
        try
        {
            string name  = Prompt("Student name");
            int    grade = PromptGrade("Grade (0-100)");

            service.AddStudent(name, grade);
            WriteSuccess($"student '{name}' added with grade {grade} ({new Student(name, grade).Category}).");
        }
        catch (Exception ex) { WriteError(ex.Message); }
    }

    private void DisplayAll()
    {
        WriteHeading("All students");

        var students = service.GetAllStudents();
        if (students.Count == 0)
        {
            WriteWarning("No students on record yet.");
            return;
        }

        // Sort alphabetically
        students.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

        WriteColor(Muted);
        Console.WriteLine($"  {"#",-4}{"Name",-25}{"Grade",6}{"Category",12}");
        Console.WriteLine($"  {new string('─', 50)}");
        Console.ResetColor();

        for (int i = 0; i < students.Count; i++)
        {
            var s = students[i];
            Console.Write($"  {i + 1,-4}{s.Name,-25}");
            WriteColor(GradeColor(s.Category));
            Console.WriteLine($"{s.Grade,6}{s.Category,12}");
            Console.ResetColor();
        }

        WriteColor(Muted);
        Console.WriteLine($"\n  Total: {students.Count} student(s)");
        Console.ResetColor();
    }

    private void SearchStudent()
    {
        WriteHeading("look for student");
        string name = Prompt("enter student name");

        if (service.TryFindStudent(name, out Student s))
        {
            WriteColor(Muted);
            Console.WriteLine($"\n  ┌─ Found ────────────────────────────────┐");
            Console.ResetColor();
            Console.WriteLine($"  │  Name    : {s.Name}");
            Console.WriteLine($"  │  Grade   : {s.Grade}");
            Console.Write($"  │  Category: ");
            WriteColor(GradeColor(s.Category));
            Console.WriteLine(s.Category);
            Console.ResetColor();
            WriteColor(Muted);
            Console.WriteLine($"  └────────────────────────────────────────┘");
            Console.ResetColor();
        }
        else
        {
            WriteError($"No student named '{name}' was found.");
        }
    }

    private void UpdateGrade()
    {
        WriteHeading("Update Student Grade");
        try
        {
            string name     = Prompt("Student name");
            int    newGrade = PromptGrade("New grade (0-100)");

            service.UpdateGrade(name, newGrade);
            WriteSuccess($"Grade for '{name}' updated to {newGrade} ({new Student(name, newGrade).Category}).");
        }
        catch (Exception ex) { WriteError(ex.Message); }
    }

    private void RemoveStudent()
    {
        WriteHeading("Remove Student");
        try
        {
            string name = Prompt("Student name to remove");
            Console.Write($"\n  Remove '{name}'? (y/N) ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (confirm == "y" || confirm == "yes")
            {
                service.RemoveStudent(name);
                WriteSuccess($"Student '{name}' removed.");
            }
            else
            {
                WriteWarning("Removal cancelled.");
            }
        }
        catch (Exception ex) { WriteError(ex.Message); }
    }

    private void ShowAverage()
    {
        WriteHeading("Average Grade");
        try
        {
            double avg = service.CalculateAverage();
            var category = new Student("_", (int)Math.Round(avg)).Category;

            Write($"\n  Class average: ", Muted);
            WriteColor(GradeColor(category));
            Console.Write($"{avg:F2}");
            Console.ResetColor();
            Write($"  ({category})\n", Muted);
        }
        catch (Exception ex) { WriteError(ex.Message); }
    }

    private void ShowHighLow()
    {
        WriteHeading("Highest & Lowest Grades");
        try
        {
            var top    = service.GetTopStudents();
            var bottom = service.GetBottomStudents();

            Console.WriteLine();
            Write(" Highest grade(s):\n", Heading);
            foreach (var s in top)
            {
                WriteColor(GradeColor(s.Category));
                Console.WriteLine($"     • {s.Name} — {s.Grade} ({s.Category})");
                Console.ResetColor();
            }

            Console.WriteLine();
            Write(" Lowest grade(s):\n", Heading);
            foreach (var s in bottom)
            {
                WriteColor(GradeColor(s.Category));
                Console.WriteLine($"     • {s.Name} — {s.Grade} ({s.Category})");
                Console.ResetColor();
            }
        }
        catch (Exception ex) { WriteError(ex.Message); }
    }

    private void ShowDistribution()
    {
        WriteHeading("Grade Distribution");
        try
        {
            var dist  = service.GetGradeDistribution();
            int total = service.Count;

            if (total == 0) { WriteWarning("No students on record."); return; }

            Console.WriteLine();
            foreach (var (cat, count) in dist)
            {
                int    barLen = total > 0 ? (int)Math.Round(count * 30.0 / total) : 0;
                string bar    = new string('█', barLen).PadRight(30);

                WriteColor(GradeColor(cat));
                Console.Write($"  {cat,2}  {bar}  {count,2} student(s)");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        catch (Exception ex) { WriteError(ex.Message); }
    }


    private static string Prompt(string label)
    {
        WriteColor(Accent);
        Console.Write($"\n  {label}: ");
        Console.ResetColor();
        return Console.ReadLine() ?? string.Empty;
    }

    private static int PromptGrade(string label)
    {
        while (true)
        {
            string raw = Prompt(label);
            if (int.TryParse(raw, out int grade) && grade is >= 0 and <= 100)
                return grade;

            WriteError("Please enter a whole number between 0 and 100.");
        }
    }

    private static void PauseForUser()
    {
        Console.WriteLine();
        WriteColor(Muted);
        Console.Write("  Press any key to continue…");
        Console.ResetColor();
        Console.ReadKey(intercept: true);
        Console.Clear();
    }


    private static void WriteHeading(string title)
    {
        Console.WriteLine();
        WriteColor(Heading);
        Console.WriteLine($"  ── {title.ToUpper()} {'─',0}".PadRight(54, '─'));
        Console.ResetColor();
    }

    private static void WriteSuccess(string msg) => Write($"\n  {msg}\n", Success);
    private static void WriteWarning(string msg) => Write($"\n  {msg}\n", Warning);
    private static void WriteError(string msg)   => Write($"\n  {msg}\n", Error);

    private static void Write(string text, ConsoleColor color)
    {
        WriteColor(color);
        Console.Write(text);
        Console.ResetColor();
    }

    private static void WriteColor(ConsoleColor c) => Console.ForegroundColor = c;

    private static ConsoleColor GradeColor(GradeCategory cat) => cat switch
    {
        GradeCategory.A => ConsoleColor.Green,
        GradeCategory.B => ConsoleColor.Cyan,
        GradeCategory.C => ConsoleColor.Yellow,
        GradeCategory.D => ConsoleColor.DarkYellow,
        GradeCategory.F => ConsoleColor.Red,
        _               => ConsoleColor.White
    };

    
}