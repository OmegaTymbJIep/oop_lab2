using System.Xml;

namespace XML_Manager;

public class SaxParser : Parser
{
    public SaxParser()
    {
        People = new List<Person>();
    }

    public override bool Load(Stream inputStream, XmlReaderSettings settings)
    {
        People.Clear();
        try
        {
            var reader = XmlReader.Create(inputStream, settings);
            while (reader.Read())
            {
                if (reader is not { NodeType: XmlNodeType.Element, Name: "Person" })
                {
                    continue;
                }

                var person = new Person();
                SkipToText(reader);
                person.Name.FirstName = reader.Value;
                SkipToText(reader);
                person.Name.LastName = reader.Value;
                SkipToText(reader);
                person.Faculty = reader.Value;
                SkipToText(reader);
                person.Department = reader.Value;
                SkipToText(reader);
                person.Role = reader.Value;
                SkipToText(reader);
                person.Salary = reader.Value;
                SkipToText(reader);
                person.TimeTenure = reader.Value;

                People.Add(person);
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    private static void SkipToText(XmlReader reader)
    {
        do
        {
            if (!reader.Read())
            {
                throw new Exception();
            }
        } while (reader.NodeType != XmlNodeType.Text);
    }
}
