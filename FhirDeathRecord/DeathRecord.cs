using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Xml.Linq;

namespace FhirDeathRecord
{

    public class Meta
    {
        private XNamespace ns = "http://hl7.org/fhir";
        public string profile;
        public Meta() {
            profile = "";
        }

        public Meta(XElement meta) {
            profile = meta.Element(ns + "profile").Attribute("value").Value;
        }

        public XElement ToXML()
        {
            return new XElement("meta", new XElement("profile", new XAttribute("value", profile)));
        }
    }

    public class Observation : IResource
    {
        public Meta meta;
        public string status;
        private XNamespace ns = "http://hl7.org/fhir";

        public Observation()
        {
            status = "final";
        }

        public Observation(XElement resource)
        {
            meta = new Meta(resource.Element(ns + "meta"));
            status = resource.Element(ns + "status").Attribute("value").Value;
        }

        public XElement ToXML()
        {
            return new XElement("Observation", meta.ToXML(), new XElement("status", new XAttribute("value", status)));
        }
    }

    public class Condition : IResource
    {
        public Meta meta;
        private XNamespace ns = "http://hl7.org/fhir";

        public Condition()
        {
        }

        public Condition(XElement resource)
        {
            meta = new Meta(resource.Element(ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Condition", meta.ToXML());
        }
    }

    public class Patient : IResource
    {
        public Meta meta;
        private XNamespace ns = "http://hl7.org/fhir";

        public Patient()
        {
        }

        public Patient(XElement resource)
        {
            meta = new Meta(resource.Element(ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Patient", meta.ToXML());
        }
    }

    public class Practitioner : IResource
    {
        public Meta meta;
        private XNamespace ns = "http://hl7.org/fhir";

        public Practitioner()
        {
        }

        public Practitioner(XElement resource)
        {
            meta = new Meta(resource.Element(ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Practitioner", meta.ToXML());
        }
    }

    public class Composition : IResource
    {
        public Meta meta;
        private XNamespace ns = "http://hl7.org/fhir";

        public Composition()
        {
        }

        public Composition(XElement resource)
        {
            meta = new Meta(resource.Element(ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Composition", meta.ToXML());
        }
    }

    public interface IResource
    {
        XElement ToXML();
    }

    public class Entry
    {
        public string fullUrl;
        public IResource resource;
        private XNamespace ns = "http://hl7.org/fhir";

        public Entry() {}

        public Entry(XElement entry)
        {
            fullUrl = entry.Element(ns + "fullUrl").Attribute("value").Value;

            if (entry.Element(ns + "resource").Element(ns + "Composition") != null)
            {
                resource = new Composition(entry.Element(ns + "resource").Element(ns + "Composition"));
            }
            else if (entry.Element(ns + "resource").Element(ns + "Patient") != null)
            {
                resource = new Patient(entry.Element(ns + "resource").Element(ns + "Patient"));
            }
            else if (entry.Element(ns + "resource").Element(ns + "Practitioner") != null)
            {
                resource = new Practitioner(entry.Element(ns + "resource").Element(ns + "Practitioner"));
            }
            else if (entry.Element(ns + "resource").Element(ns + "Condition") != null)
            {
                resource = new Condition(entry.Element(ns + "resource").Element(ns + "Condition"));
            }
            else if (entry.Element(ns + "resource").Element(ns + "Observation") != null)
            {
                resource = new Observation(entry.Element(ns + "resource").Element(ns + "Observation"));
            }
        }

        public XElement ToXML()
        {
            return new XElement("entry",
                new XElement("fullUrl", new XAttribute("value", fullUrl)),
                new XElement("resource", resource.ToXML())
            );
        }
    }

    public class Bundle
    {
        public string bundleId { get; set; }
        public string type { get; set; }
        public List<Entry> entries;
        private XNamespace ns = "http://hl7.org/fhir";

        public Bundle(string id)
        {
            bundleId = id;
            type = "document";
            entries = new List<Entry>();
        }

        public Bundle(XElement bundle)
        {
            bundleId = bundle.Element(ns + "id").Attribute("value").Value;
            type = bundle.Element(ns + "type").Attribute("value").Value;
            entries = (from entry in bundle.Elements(ns + "entry") select new Entry(entry)).ToList();
        }

        public XElement ToXML()
        {
            XElement[] xmlEntries = entries.Select(entry => entry.ToXML()).ToArray();
            return new XElement("Bundle", new XAttribute("Xmlns", "http://hl7.org/fhir"),
                new XElement("id", new XAttribute("value", bundleId)),
                new XElement("type", new XAttribute("value", type)),
                xmlEntries
            );
        }
    }

    public class DeathRecord
    {
        private Bundle _Bundle;
        private XNamespace ns = "http://hl7.org/fhir";

        public DeathRecord()
        {
            _Bundle = new Bundle("");
        }

        public DeathRecord(XDocument record)
        {
            _Bundle = new Bundle(record.Element(ns + "Bundle"));
        }

        public XDocument ToXML()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", String.Empty),
                new XComment("Death Record"),
                _Bundle.ToXML()
            );
        }
    }

}
