using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcUnits : MonoBehaviour
{
    /*
    This script stores IFC units data.
    It is attached to the ifc root gameobject by IfcXmlParser.cs
    IFC units describes the units used in the model, such as lengths are in meters
    */
    public List<string> unitType = new List<string>();
    public List<string> unitName = new List<string>();
    public List<string> si_equivalent = new List<string>();

    // Return a unit type's name
    // for example UnitTypeName("LENGTHUNIT") may return "FOOT"
    public string Find(string type){
        for(int i = 0; i < unitType.Count; i++){
            if(unitType[i] == type){
                return unitName[i];
            }
        }
        return null;
    }

    // Return a unit type's divisor to convert it to the international System of Units
    // if the unit is foot, the returned SI Equivalent would be "0.30480000000000002"
    public string FindSIEquivalent(string type){
        for(int i = 0; i < unitType.Count; i++){
            if(unitType[i] == type){
                return si_equivalent[i];
            }
        }
        return null;
    }
}
