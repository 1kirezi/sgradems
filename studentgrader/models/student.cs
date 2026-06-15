namespace studentgrader.models;

public enum GradeCategory
{
    A,  
    B,  
    C,  
    D,  
    F
}

///name and grade

public struct Student
{
    public string Name { get; set; }
    public int Grade { get; set; }

    public Student(string name, int grade)
    {
        Name = name;
        Grade = grade;
    }

    public GradeCategory Category => Grade switch
    {
        >= 90 => GradeCategory.A,
        >= 80 => GradeCategory.B,
        >= 70 => GradeCategory.C,
        >= 60 => GradeCategory.D,
        _     => GradeCategory.F
    };

    public override string ToString() => $"{Name}: {Grade} ({Category})";
}
