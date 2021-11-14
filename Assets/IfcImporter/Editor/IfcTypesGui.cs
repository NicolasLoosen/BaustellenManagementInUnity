using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcTypes))]
public class IfcTypesGui : Editor
{
    // This script changes the UI of IfcTypes components in the Unity editor inspector
    new IfcTypes target;

    public override void OnInspectorGUI()
    {
        target = (IfcTypes)base.target;

        for(int i = 0; i < target.types.Count; i++){
            EditorGUILayout.LabelField(target.types[i], target.values[i]);
        }
    }
}
