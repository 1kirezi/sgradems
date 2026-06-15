namespace studentgrader.models;

/// exception for when the user enters a name that is not in the system
///exception for when the user tries to re enter the same name
///exception for marks that are not 0-100


public class StudentNotFoundException(string name)
    : Exception($"Student '{name}' is not in the system.");


public class DuplicateStudentException(string name)
    : Exception($"student '{name}' already exists.");


public class InvalidGradeException(int grade)
    : Exception($"grade '{grade}' is invalid. grades should be between 0 and 100.");
