using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcAttributes))]
public class IfcAttributeGui : Editor
{
    // This script changes the UI of ifcAttributes components in the Unity editor inspector
    new IfcAttributes target;

    public override void OnInspectorGUI()
    {
        target = (IfcAttributes)base.target;

        for(int i = 0; i < target.attributes.Count; i++){
            EditorGUILayout.LabelField(target.attributes[i], target.values[i]);
        }
    }
}
