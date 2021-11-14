using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcMaterials))]
public class ifcMaterialGui : Editor
{
    //This script changes the UI of IfcMaterials components in the Unity editor inspector
    new IfcMaterials target;

    public override void OnInspectorGUI()
    {
        target = (IfcMaterials)base.target;

        for(int i = 0; i < target.materials.Count; i++){
            EditorGUILayout.LabelField(target.materials[i], target.thicknesses[i]);
        }
    }
}
