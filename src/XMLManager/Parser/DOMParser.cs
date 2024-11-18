using System.Xml;
using System.Xml.Serialization;

namespace XMLManager.Parser;

public class DomParser : FilterableParser
{
    public override bool Load(Stream inputStream, XmlReaderSettings settings)
    {
        People.Clear();

        var document = new XmlDocument();

        try
        {
            using var reader = XmlReader.Create(inputStream, settings);

            document.Load(reader);
            if (document.DocumentElement == null)
            {
                return true;
            }

            var serializer = new XmlSerializer(typeof(Person.Person));
            foreach (XmlNode child in document.DocumentElement.ChildNodes)
            {
                if (serializer.Deserialize(new StringReader(child.OuterXml)) is Person.Person person)
                {
                    People.Add(person);
                }
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
