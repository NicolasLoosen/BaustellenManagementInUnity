﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

/*
 * Handles the editor's Assets -> Import IFC menu option to manually import ifc-files.
 * IfcProcessor automatically imports files as Unity detects them, which makes this kind of pointless.
 * Could possibly be used to import files from outside the project folder with a bit of work?
*/
public class IfcEditorExtension : EditorWindow {

    string assetPath = "Assets/example.ifc";

    [MenuItem ("Assets/Import IFC")]

    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(IfcEditorExtension));
    }
        
    void OnGUI() {
        GUILayout.Label ("Enter the name of the .ifc file", EditorStyles.boldLabel);
        assetPath = EditorGUILayout.TextField ("Text Field", assetPath);
        if (GUI.Button (new Rect(100,200,100,20), "Import")) {
            UnityEngine.Debug.Log(assetPath);
            bool editor = true;
            IfcImporter.ProcessIfc(assetPath, IfcSettingsWindow.options, editor);
            string resourceName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            string outputFile = "Assets/Resources/" + resourceName;
            AssetDatabase.ImportAsset(outputFile + "_dae.dae", ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(outputFile + "_xml.xml", ImportAssetOptions.ForceUpdate);
            GameObject root_object = IfcImporter.LoadDae(assetPath);

            string xml_path = outputFile + "_xml.xml";
            IfcXmlParser.parseXmlFile(xml_path, root_object, IfcSettingsWindow.options);

            PrefabSaver.savePrefab(root_object, assetPath);
        }

    }
}
