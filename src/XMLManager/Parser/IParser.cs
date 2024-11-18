using System.Xml;
using XMLManager.Person;

namespace XMLManager.Parser;

public interface IParser
{
    public bool Load(Stream inputStream, XmlReaderSettings settings);

    public IList<Person.Person> Find(Filters filters);
}
