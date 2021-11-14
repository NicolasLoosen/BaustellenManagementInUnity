using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcTypes : MonoBehaviour
{
    /*
    This script stores IFC types and styles data and returns values based on entry name.
    It is attached to each IFC gameobject with type or style data by IfcXmlParser.cs
    The information in IFC types and styles is essentially similar to that in IFC property sets.
    */
    public List<string> types = new List<string>();
    public List<string> values = new List<string>();

    // Return corresponding value for type entry name
    public string Find(string name){
        for(int i = 0; i < types.Count; i++){
            if(types[i] == name){
                return values[i];
            }
        }
        return null;
    }
}
