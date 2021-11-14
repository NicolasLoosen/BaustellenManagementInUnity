using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public class IfcSettingsWindow : EditorWindow
{
    public static Dictionary<string, bool> options = new Dictionary<string, bool>()
    {
        {"meshCollidersEnabled", true},
        {"materialsEnabled", true},
        {"propertiesEnabled", true},
        {"attributesEnabled", true},
        {"typesEnabled", true},
        {"headerEnabled", true},
        {"unitsEnabled", true},
        {"quantitiesEnabled", true},
        {"rootListsEnabled", true},
        {"parallelProcessingEnabled", true}
    };

    [MenuItem("Edit/IFC Importer Settings...")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(IfcSettingsWindow));
    }
    
    void OnGUI()
    {
        GUILayout.Label ("IFC Importer Settings", EditorStyles.boldLabel);
        float originalValue = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 400;
        options["meshCollidersEnabled"] = EditorGUILayout.Toggle ("Add mesh colliders", options["meshCollidersEnabled"]);
        options["materialsEnabled"] = EditorGUILayout.Toggle ("Import material layer sets", options["materialsEnabled"]);
        options["propertiesEnabled"] = EditorGUILayout.Toggle ("Import IFC property sets", options["propertiesEnabled"]);
        options["attributesEnabled"] = EditorGUILayout.Toggle ("Import IFC attributes", options["attributesEnabled"]);

        options["typesEnabled"] = EditorGUILayout.Toggle ("Import types", options["typesEnabled"]);
        options["headerEnabled"] = EditorGUILayout.Toggle ("Import header", options["headerEnabled"]);
        options["unitsEnabled"] = EditorGUILayout.Toggle ("Import units", options["unitsEnabled"]);
        options["quantitiesEnabled"] = EditorGUILayout.Toggle ("Import quantities", options["quantitiesEnabled"]);

        options["rootListsEnabled"] = EditorGUILayout.Toggle ("Import IFC root lists", options["rootListsEnabled"]);
        options["parallelProcessingEnabled"] = EditorGUILayout.Toggle ("Extract model and metadata in parallel (memory intensive)", options["parallelProcessingEnabled"]);
        EditorGUIUtility.labelWidth = originalValue;
    }

    void OnFocus()
    {
        // Need to call ToList() because we're modifying the dictionary inside the loop
        foreach(string key in options.Keys.ToList<string>())
        {
            if (EditorPrefs.HasKey(key))
            {
                options[key] = EditorPrefs.GetBool(key);
            }
        }
    }

    void OnLostFocus()
    {
        foreach(string key in options.Keys)
        {
            EditorPrefs.SetBool(key, options[key]);
        }
    }

    void OnDestroy()
    {
        foreach(string key in options.Keys)
        {
            EditorPrefs.SetBool(key, options[key]);
        }
    }
}