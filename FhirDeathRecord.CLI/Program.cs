using System;
using System.IO;
using System.Linq;
using System.Text;
using FhirDeathRecord;
using System.Xml.Linq;

namespace csharp_fhir_death_record
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consuming " + args[0] + "...");

            DeathRecord record = new DeathRecord(XDocument.Load(args[0]));

            Console.WriteLine("Producing " + args[0] + " in XML...");

            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                record.ToXML().Save(writer);
            }
            Console.WriteLine(builder.ToString());
        }
    }
}
