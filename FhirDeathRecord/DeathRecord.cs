using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Xml.Linq;

namespace FhirDeathRecord
{

    public static class FhirNamespace
    {
        public static XNamespace ns = "http://hl7.org/fhir";
    }

    public interface IValue
    {
        XElement ToXML();
    }

    public class ValueBoolean : IValue
    {
        public bool valueBool;

        public ValueBoolean(XElement value) {
            valueBool = Boolean.Parse(value.Attribute("value").Value);
        }

        public XElement ToXML()
        {
            return new XElement("valueBoolean", new XAttribute("value", valueBool.ToString().ToLower()));
        }
    }

    public class ValueString : IValue
    {
        public string valueString;

        public ValueString(XElement value) {
            valueString = value.Attribute("value").Value;
        }

        public XElement ToXML()
        {
            return new XElement("valueString", new XAttribute("value", valueString));
        }
    }

    public class ValueDateTime : IValue
    {
        public DateTime dateTime;

        public ValueDateTime(XElement value) {
            dateTime = DateTime.Parse(value.Attribute("value").Value);
        }

        public XElement ToXML()
        {
            return new XElement("valueDateTime", new XAttribute("value", dateTime.ToString("o")));
        }
    }

    public class ValueCodeableConcept : IValue
    {
        public string code;
        public string system;
        public string display;

        public ValueCodeableConcept(XElement value) {
            if (value.Element(FhirNamespace.ns + "coding").Element(FhirNamespace.ns + "code") != null)
            {
                code = value.Element(FhirNamespace.ns + "coding").Element(FhirNamespace.ns + "code").Attribute("value").Value;
            }
            if (value.Element(FhirNamespace.ns + "coding").Element(FhirNamespace.ns + "system") != null)
            {
                system = value.Element(FhirNamespace.ns + "coding").Element(FhirNamespace.ns + "system").Attribute("value").Value;
            }
            if (value.Element(FhirNamespace.ns + "coding").Element(FhirNamespace.ns + "display") != null)
            {
                display = value.Element(FhirNamespace.ns + "coding").Element(FhirNamespace.ns + "display").Attribute("value").Value;
            }
        }

        public XElement ToXML()
        {
            return new XElement("valueCodeableConcept",
                new XElement("coding",
                    new XElement("code", new XAttribute("value", code != null ? code : "")),
                    new XElement("system", new XAttribute("value", system != null ? system : "")),
                    new XElement("display", new XAttribute("value", display != null ? display : ""))
                )
            );
        }
    }

    public class Meta
    {
        public string profile;
        public Meta() {
            profile = "";
        }

        public Meta(XElement meta) {
            profile = meta.Element(FhirNamespace.ns + "profile").Attribute("value").Value;
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
        public IValue value;


        public Observation()
        {
            status = "final";
        }

        public Observation(XElement resource)
        {
            meta = new Meta(resource.Element(FhirNamespace.ns + "meta"));
            status = resource.Element(FhirNamespace.ns + "status").Attribute("value").Value;

            // Grab value
            if (resource.Element(FhirNamespace.ns + "valueCodeableConcept") != null)
            {
                value = new ValueCodeableConcept(resource.Element(FhirNamespace.ns + "valueCodeableConcept"));
            }
            else if (resource.Element(FhirNamespace.ns + "valueBoolean") != null)
            {
                value = new ValueBoolean(resource.Element(FhirNamespace.ns + "valueBoolean"));
            }
            else if (resource.Element(FhirNamespace.ns + "valueDateTime") != null)
            {
                value = new ValueDateTime(resource.Element(FhirNamespace.ns + "valueDateTime"));
            }
            else if (resource.Element(FhirNamespace.ns + "valueString") != null)
            {
                value = new ValueString(resource.Element(FhirNamespace.ns + "valueString"));
            }
        }

        public XElement ToXML()
        {
            return new XElement("Observation", meta.ToXML(),
                new XElement("status", new XAttribute("value", status)),
                value.ToXML()
            );
        }
    }

    public class Condition : IResource
    {
        public Meta meta;

        public Condition()
        {
        }

        public Condition(XElement resource)
        {
            meta = new Meta(resource.Element(FhirNamespace.ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Condition", meta.ToXML());
        }
    }

    public class Patient : IResource
    {
        public Meta meta;

        public Patient()
        {
        }

        public Patient(XElement resource)
        {
            meta = new Meta(resource.Element(FhirNamespace.ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Patient", meta.ToXML());
        }
    }

    public class Practitioner : IResource
    {
        public Meta meta;

        public Practitioner()
        {
        }

        public Practitioner(XElement resource)
        {
            meta = new Meta(resource.Element(FhirNamespace.ns + "meta"));
        }

        public XElement ToXML()
        {
            return new XElement("Practitioner", meta.ToXML());
        }
    }

    public class Composition : IResource
    {
        public Meta meta;

        public Composition()
        {
        }

        public Composition(XElement resource)
        {
            meta = new Meta(resource.Element(FhirNamespace.ns + "meta"));
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

        public Entry() {}

        public Entry(XElement entry)
        {
            fullUrl = entry.Element(FhirNamespace.ns + "fullUrl").Attribute("value").Value;

            if (entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Composition") != null)
            {
                resource = new Composition(entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Composition"));
            }
            else if (entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Patient") != null)
            {
                resource = new Patient(entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Patient"));
            }
            else if (entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Practitioner") != null)
            {
                resource = new Practitioner(entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Practitioner"));
            }
            else if (entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Condition") != null)
            {
                resource = new Condition(entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Condition"));
            }
            else if (entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Observation") != null)
            {
                resource = new Observation(entry.Element(FhirNamespace.ns + "resource").Element(FhirNamespace.ns + "Observation"));
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

        public Bundle(string id)
        {
            bundleId = id;
            type = "document";
            entries = new List<Entry>();
        }

        public Bundle(XElement bundle)
        {
            bundleId = bundle.Element(FhirNamespace.ns + "id").Attribute("value").Value;
            type = bundle.Element(FhirNamespace.ns + "type").Attribute("value").Value;
            entries = (from entry in bundle.Elements(FhirNamespace.ns + "entry") select new Entry(entry)).ToList();
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
        public Bundle bundle;

        public DeathRecord()
        {
            bundle = new Bundle("");
        }

        public DeathRecord(String record)
        {
            // TODO: For now assuming XML string; in future, need to also handle if
            // given JSON string.
            XDocument xmlRecord = XDocument.Parse(record);
            bundle = new Bundle(xmlRecord.Element(FhirNamespace.ns + "Bundle"));
        }

        public DeathRecord(XDocument record)
        {
            bundle = new Bundle(record.Element(FhirNamespace.ns + "Bundle"));
        }

        public XDocument ToXML()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", String.Empty),
                new XComment("Death Record"),
                bundle.ToXML()
            );
        }
    }

}
