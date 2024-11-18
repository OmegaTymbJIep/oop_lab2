using System.Xml;
using System.Xml.Linq;

namespace XMLManager.Parser;

public class LinqParser : FilterableParser
{
    public override bool Load(Stream inputStream, XmlReaderSettings settings)
    {
        try
        {
            using var reader = XmlReader.Create(inputStream, settings);

            People = XDocument.Load(reader)
                .Descendants("Person")
                .Select(person => new Person.Person
                {
                    Name = new Person.Person.FullName
                    {
                        FirstName = person.Element("Name")?.Element("FirstName")?.Value ?? "",
                        LastName = person.Element("Name")?.Element("LastName")?.Value ?? ""
                    },
                    Faculty = person.Element("Faculty")?.Value ?? "",
                    Department = person.Element("Department")?.Value ?? "",
                    Role = person.Element("Role")?.Value ?? "",
                    Salary = person.Element("Salary")?.Value ?? "",
                    TimeTenure = person.Element("TimeTenure")?.Value ?? ""
                }).ToList();
        }
        catch
        {
            return false;
        }

        return true;
    }
}
