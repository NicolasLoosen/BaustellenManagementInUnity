using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcHeader))]
public class IfcHeaderGui : Editor
{
    // This script changes the UI of IfcHeader components in the Unity editor inspector
    new IfcHeader target;

    public override void OnInspectorGUI()
    {
        target = (IfcHeader)base.target;

        for(int i = 0; i < target.headers.Count; i++){
            EditorGUILayout.LabelField(target.headers[i], target.values[i]);
        }
    }
}
