using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcProperties))]
public class IfcPropertyGui : Editor
{
    // This script changes the UI of IfcProperties components in the Unity editor inspector
    new IfcProperties target;

    public override void OnInspectorGUI()
    {
        target = (IfcProperties)base.target;

        for(int i = 0; i < target.properties.Count; i++){
            if(target.properties[i] == "PsetName"){
                if(i>1){
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.LabelField(target.nominalValues[i], "");
                EditorGUI.indentLevel++;
            }
            else{
                EditorGUILayout.LabelField(target.properties[i], target.nominalValues[i]);
            }
        }
        EditorGUI.indentLevel--;
    }
}
