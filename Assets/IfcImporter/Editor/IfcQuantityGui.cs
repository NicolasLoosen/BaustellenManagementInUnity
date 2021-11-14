using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcQuantities))]
public class IfcQuantityGui : Editor
{
    // This script changes the UI of IfcQuantities components in the Unity editor inspector
    new IfcQuantities target;

    public override void OnInspectorGUI()
    {
        target = (IfcQuantities)base.target;

        for(int i = 0; i < target.quantities.Count; i++){
            EditorGUILayout.LabelField(target.quantities[i], target.values[i]);
        }
    }
}
