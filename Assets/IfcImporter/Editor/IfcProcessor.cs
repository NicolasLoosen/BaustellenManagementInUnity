using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Hooks into Unity's import pipeline to detect when an ifc-file needs importing, then imports it.
 * Usage: OnPreprocessAsset() and OnPostprocessAllAssets() are called automatically as needed.
*/
public class IfcProcessor : AssetPostprocessor
{
    void OnPreprocessAsset()
    {
        if (assetPath.EndsWith(".ifc"))
        {
            //UnityEngine.Debug.Log("Preprocessing " + assetPath);
            bool editor = true;
            IfcImporter.ProcessIfc(assetPath, IfcSettingsWindow.options, editor);
            
            //Import the newly created files
            string outputFile = "Assets/Resources/" + System.IO.Path.GetFileNameWithoutExtension(assetPath);
            AssetDatabase.ImportAsset(outputFile + "_dae.dae", ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(outputFile + "_xml.xml", ImportAssetOptions.ForceUpdate);
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".ifc"))
            {
                //UnityEngine.Debug.Log("Postprocessing " + assetPath);
                GameObject gameObject = IfcImporter.LoadDae(assetPath);

                string resourceName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                string xml_path = "Assets/Resources/" +resourceName + "_xml.xml";
                IfcXmlParser.parseXmlFile(xml_path, gameObject, IfcSettingsWindow.options);

                PrefabSaver.savePrefab(gameObject, assetPath);
            }
        }
    }
}

