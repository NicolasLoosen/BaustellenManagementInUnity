using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class IfcXmlParserTests
    {
        [Test]
        public void IfcXmlParserDoesNotCrashIfMissingPropertySets()
        {
            TextAsset xmlFile = (TextAsset)Resources.Load("garage_missing_pset");
            //Load the .dae
            GameObject IfcDae = (GameObject)Resources.Load("garage 1_dae");
            //Create an instance of it, so that it actually exists in the game world
            GameObject IfcDaeInstance = GameObject.Instantiate(IfcDae);
            string xmlText = xmlFile.text;
            IfcXmlParser.parseXmlFile(xmlText, IfcDaeInstance);
        }

        [Test]
        public void IfcXmlParserDoesNotCrashIfMissingMaterials()
        {
            TextAsset xmlFile = (TextAsset)Resources.Load("garage_missing_materials");
            //Load the .dae
            GameObject IfcDae = (GameObject)Resources.Load("garage 1_dae");
            //Create an instance of it, so that it actually exists in the game world
            GameObject IfcDaeInstance = GameObject.Instantiate(IfcDae);
            string xmlText = xmlFile.text;
            IfcXmlParser.parseXmlFile(xmlText, IfcDaeInstance);
        }

        [Test]
        public void IfcXmlParserDoesNotCrashIfNoDaeInstance()
        {
            TextAsset xmlFile = (TextAsset)Resources.Load("garage_missing_materials");
            string xmlText = xmlFile.text;
            IfcXmlParser.parseXmlFile(xmlText, null);
        }

    }

}
