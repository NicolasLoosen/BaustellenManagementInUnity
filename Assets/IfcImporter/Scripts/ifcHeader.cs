using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcHeader : MonoBehaviour
{
    /*
    This script stores IFC header data and returns values based on header name.
    It is attached to the ifc root gameobject by IfcXmlParser.cs
    IFC header data may contain information such as: 
        author
        creation date
        design software used
        IFC schema such as IFC2X3 or IFC4
    */
    public List<string> headers = new List<string>();
    public List<string> values = new List<string>();

    // Return corresponding value for header name
    public string Find(string name){
        for(int i = 0; i < headers.Count; i++){
            if(headers[i] == name){
                return values[i];
            }
        }
        return null;
    }
}
