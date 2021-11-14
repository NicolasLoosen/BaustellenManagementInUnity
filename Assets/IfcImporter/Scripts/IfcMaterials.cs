using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcMaterials : MonoBehaviour
{
    /*
    This script stores IfcMaterial, IfcMaterialList and IfcMaterialLayerSetUsage data as material layers.
    The script also returns values based on material layer name.
    It is attached to each IFC gameobject with material data by IfcXmlParser.cs
    IFC material layers may contain one or more layers with or without thicknessess.
    */
    public List<string> materials = new List<string>();
    public List<string> thicknesses = new List<string>();

    // Return first thickness for material layer name
    public string Find(string name){
        for(int i = 0; i < materials.Count; i++){
            if(materials[i] == name){
                return thicknesses[i];
            }
        }
        return null;
    }

}
