using System.Xml;

namespace XML_Manager;

public abstract class Parser : IParser
{
    protected IList<Person> People = new List<Person>();

    public IList<Person> Find(Filters filters)
    {
        return People.Where(filters.ValidatePerson).ToList();
    }

    public abstract bool Load(Stream inputstream, XmlReaderSettings settings);
}