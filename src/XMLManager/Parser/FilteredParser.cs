using System.Xml;
using XMLManager.Person;

namespace XMLManager.Parser;

public abstract class FilterableParser : IParser
{
    protected IList<Person.Person> People = new List<Person.Person>();

    public IList<Person.Person> Find(Filters filters)
    {
        return People.Where(filters.ValidatePerson).ToList();
    }

    public abstract bool Load(Stream inputstream, XmlReaderSettings settings);
}