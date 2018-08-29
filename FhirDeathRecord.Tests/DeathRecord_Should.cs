using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Xunit;

namespace FhirDeathRecord.Tests
{
    public class DeathRecord_Should
    {
        private DeathRecord XMLRecord;
        private DeathRecord FromScratchRecord;

        public DeathRecord_Should()
        {
            XMLRecord = new DeathRecord(File.ReadAllText(FixturePath("fixtures/xml/1.xml")));
            FromScratchRecord = new DeathRecord();
        }

        [Fact]
        public void ParsedCorrectBundleId()
        {
            Assert.Equal(XMLRecord.bundle.bundleId, "1");
        }

        [Fact]
        public void ParsedCorrectType()
        {
            Assert.Equal(XMLRecord.bundle.type, "document");
        }

        private string FixturePath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }
            else
            {
                return Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);
            }
        }
    }
}
