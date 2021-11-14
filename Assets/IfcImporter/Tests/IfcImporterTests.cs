using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class IfcImporterTests
    {

        [Test]
        public void IfcImporterTestsCannotImportNonexistentFile()
        {
            // Trying to import a file that does not exist should result in error
            IfcImporter.ProcessIfc("nosuch");
            LogAssert.Expect(LogType.Error, new Regex(".*DAE.*"));
            LogAssert.Expect(LogType.Error, new Regex(".*XML.*"));
        }

        [Test]
        public void IfcImporterTestsCanImportFilenamesWithSpaces()
        {
            // Trying to import a file with spaces in filename should not result in error.
            // Note that this test has a dependency on CannotImportNonexistentFile. Rationale: otherwise we would need a dependency on the filesystem to check if proper files have been created.
            IfcImporter.ProcessIfc("Assets/IfcImporter/Tests/garage 1.ifc");
        }
    }
}
