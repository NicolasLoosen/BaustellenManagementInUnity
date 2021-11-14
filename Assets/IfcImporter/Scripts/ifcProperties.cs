using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcProperties : MonoBehaviour {
    /*
    This script stores IFC property data and returns values based on property name.
    It is attached to each IFC gameobject with property data by IfcXmlParser.cs
    IFC property data contains a lot of varied information based on the element type, origin software and the designers preferences.
    A wall may include information such as:
        If it is load bearing
        If it is an external wall
        Its fire rating
        Its U-value
        Its assembly code
        Its manufacturer
        Or any other data that the designer chooses to add.
    */
    public List<string> properties = new List<string>();
    public List<string> nominalValues = new List<string>();

    // Return corresponding nominalValue for property name
    public string Find(string name){
        for(int i = 0; i < properties.Count; i++){
            if(properties[i] == name){
                return nominalValues[i];
            }
        }
        return null;
    }
}
