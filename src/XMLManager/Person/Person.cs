namespace XMLManager.Person;

public class Person
{
    public class FullName
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
    }

    public FullName Name { get; init; } = new();

    public string Faculty { get; set; } = "";

    public string Department { get; set; } = "";

    public string Salary { get; set; } = "";

    public string TimeTenure { get; set; } = "";

    public string Role { get; set; } = "";

}
