using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcAttributes : MonoBehaviour
{
    /*
    This script stores IFC attribute data and returns values based on attribute name.
    It is attached to IFC gameobject by IfcXmlParser.cs
    IFC attribute data should contain information such as: 
        Element type (e.g. IfcWallStandardCase, IfcSlab, IfcWindow, IfcDoor etc.)
        Unique IFC id
        Name
    */
    public List<string> attributes = new List<string>();
    public List<string> values = new List<string>();

    // Return corresponding value for attribute name
    public string Find(string name){
        for(int i = 0; i < attributes.Count; i++){
            if(attributes[i] == name){
                return values[i];
            }
        }
        return null;
    }
}
