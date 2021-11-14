using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Demonstrates a complete runtime import from start to finish.
 *
 * How to use:
 * 1) Create a GameObject and add this script to it. This should already be done in the demo scene.
 * 2) Download IfcConvert from here http://ifcopenshell.org/ifcconvert and place it in the project's root folder.
 * 3) Place an ifc file you wish to test inside the project folder.
 * 4) Edit the assetPath and filename below to point to your ifc-file.
 * 5) Press play. Your building should appear after a few seconds.
 * You can also build and run the scene. For this you should make sure both your ifc-file and IfcConvert executable are in the resulting build folder and the assetPath and the filename below are still correct.
*/
public class RuntimeImportDemo : MonoBehaviour
{
    public string assetPath = "Assets/";
    public string filename = "editor_test.ifc";
    void Start()
    {
        //An optional parameter to toggle various option available in the settings menu.
        //Any missing options are assumed to be true, all are true if options not given.
        Dictionary<string, bool> options = new Dictionary<string, bool>()
        {
            {"meshCollidersEnabled", true},
            {"materialsEnabled", true},
            {"propertiesEnabled", true},
            {"attributesEnabled", true},
            {"typesEnabled", true},
            {"headerEnabled", true},
            {"unitsEnabled", true},
            {"quantitiesEnabled", true},
            {"rootListsEnabled", true}
        };
        //Various parts of the building are rootObject's children.
        GameObject rootObject = IfcImporter.RuntimeImport(assetPath+filename, options);
        //Objs start out sideways because reasons. Should probably make it so our importer rotates it..
        rootObject.transform.Rotate(-90, 0, 0);
    }
}
