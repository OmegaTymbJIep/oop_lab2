namespace XMLManager.Person;

public struct Filters
{
    public Filters() {}

    public string FullName { get; set; } = "";

    public string Faculty { get; set; } = "";

    public string Department { get; set; } = "";

    public string Salary { get; set; } = "";

    public string TimeTenure { get; set; } = "";

    public string Role { get; set; } = "";

    public readonly bool ValidatePerson(Person person)
    {
        var fullName = $"{person.Name.FirstName} {person.Name.LastName}";

        var name = fullName.Contains(FullName, StringComparison.OrdinalIgnoreCase);
        var role = person.Role.Contains(Role, StringComparison.OrdinalIgnoreCase);
        var faculty = person.Faculty.Contains(Faculty, StringComparison.OrdinalIgnoreCase);
        var department = person.Department.Contains(Department, StringComparison.OrdinalIgnoreCase);
        var salary = person.Salary.Contains(Salary, StringComparison.OrdinalIgnoreCase);
        var timeTenure = person.TimeTenure.Contains(TimeTenure, StringComparison.OrdinalIgnoreCase);

        return name && role && faculty && department && salary && timeTenure;
    }
}
