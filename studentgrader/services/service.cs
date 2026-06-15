using studentgrader.models;

namespace studentgrader.services;

public class GradeService
{
    private readonly Dictionary<string, int> _students = new(StringComparer.OrdinalIgnoreCase);


    /// add student check all exceptions
    public void AddStudent(string name, int grade)
    {
        ValidateName(name);
        ValidateGrade(grade);

        if (_students.ContainsKey(name))
            throw new DuplicateStudentException(name);

        _students[name] = grade;
    }

    /// update
    public void UpdateGrade(string name, int newGrade)
    {
        ValidateGrade(newGrade);
        EnsureExists(name);
        _students[name] = newGrade;
    }

    /// delete student
    public void RemoveStudent(string name)
    {
        EnsureExists(name);
        _students.Remove(name);
    }



    public Student GetStudent(string name)
    {
        EnsureExists(name);
        return new Student(name, _students[name]);
    }

    /// get all students
    public List<Student> GetAllStudents() =>
        _students.Select(kv => new Student(kv.Key, kv.Value)).ToList();

    
    public bool TryFindStudent(string name, out Student student)
    {
        if (_students.TryGetValue(name, out int grade))
        {
            student = new Student(name, grade);
            return true;
        }
        student = default;
        return false;
    }


    public bool HasStudents => _students.Count > 0;
    public int Count => _students.Count;

    /// calculate mean grades
    public double CalculateAverage()
    {
        EnsureNotEmpty();
        return _students.Values.Average();
    }

    /// finds student with high grades
    public List<Student> GetTopStudents()
    {
        EnsureNotEmpty();
        int max = _students.Values.Max();
        return _students
            .Where(kv => kv.Value == max)
            .Select(kv => new Student(kv.Key, kv.Value))
            .ToList();
    }

    /// finds students with low grades
    public List<Student> GetBottomStudents()
    {
        EnsureNotEmpty();
        int min = _students.Values.Min();
        return _students
            .Where(kv => kv.Value == min)
            .Select(kv => new Student(kv.Key, kv.Value))
            .ToList();
    }

    public Dictionary<GradeCategory, int> GetGradeDistribution()
    {
        var dist = Enum.GetValues<GradeCategory>()
                       .ToDictionary(c => c, _ => 0);

        foreach (var s in GetAllStudents())
            dist[s.Category]++;

       return dist;
    }

    
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("cannot be empty.");
    }

    private static void ValidateGrade(int grade)
    {
        if (grade < 0 || grade > 100)
            throw new InvalidGradeException(grade);
    }

    private void EnsureExists(string name)
    {
        if (!_students.ContainsKey(name))
            throw new StudentNotFoundException(name);
    }

    private void EnsureNotEmpty()
    {
        if (_students.Count == 0)
            throw new InvalidOperationException("No students");
    }
}