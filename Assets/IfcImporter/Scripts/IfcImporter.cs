using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Linq;

/*
For all your ifc importing needs.
Usage: 
    1) IfcImporter.RuntimeImport(file_to_import), returns the root GameObject
    2) Also used by various UnityEditor scripts to handle drag and drop files etc
        *The relevant functions are
            -ProcessIfc to generate obj, mtl and xml (runtime) or dae and xml (editor)
            -LoadDae (for saving as a prefab) or ObjectLoader.Load (for runtime imports)
            -IfcXmlParser.parseXmlFile to add the metadata
            -Prefabsaver.SavePrefab to save as prefab. Editor only, deletes GameObject.
    
Rough outline of what calls what:
User
    RuntimeImport
        ProcessIfc
        ObjectLoader.Load
            TreeBuilder.ReconstructTree
        IfcXmlParser.parseXmlFile
        return root_object

UnityEditor's events (drag and drop files etc)
    IfcProcessor.OnPreProcess
        ProcessIfc
    IfcProcessor.OnPostProcess
        LoadDae
        IfcXmlParser.parseXml
        PrefabSaver.savePrefab

UnityEditor's menu Assets->Import IFC
    IfcEditorExtension.OnGUI
        ProcessIfc
        LoadDae
        IfcXmlParser.parseXml
        PrefabSaver.savePrefab
*/
public class IfcImporter : MonoBehaviour
{
    public static GameObject RuntimeImport(string inputFile, Dictionary<string, bool> options = null)
    {
        bool editor = false;
        ProcessIfc(inputFile, options, editor);
        string resourceName = System.IO.Path.GetFileNameWithoutExtension(inputFile);
        string assetPath = "Assets/Resources/";
        string outputFile = assetPath + resourceName;
        GameObject root_object = ObjectLoader.Load(assetPath, resourceName + "_obj.obj");
        //Add the metadata
        string xml_path = assetPath + resourceName + "_xml.xml";
        IfcXmlParser.parseXmlFile(xml_path, root_object, options);
        return root_object;
    }

    public static void ProcessIfc(string inputFile, Dictionary<string, bool> options = null, bool editor = true)
    {
        // TODO: Move imports to this function from generate functions (return output filename from generate)?
        // TODO: See if asynchronously calling generate functions is any faster
        // Add tests for generate functions (e.g. createsfile, createslog, isxml, isdae, logcontainserroriferror)
        string ifcConvert = FindIfcConvert();
        if (String.IsNullOrEmpty(ifcConvert))
        {
            UnityEngine.Debug.Log("IfcConvert.exe not found! Please add ifc convert to the root of the project!");
            return;
        }

        List<Process> allProcesses = new List<Process>();

        //Create Assets/Resources folder if it doesn't exist
        System.IO.Directory.CreateDirectory("Assets/Resources/");
        //Save files under resources so that we can access them later to create the prefab
        string resourceName = System.IO.Path.GetFileNameWithoutExtension(inputFile);
        string assetPath = "Assets/Resources/";
        string outputFile = assetPath + resourceName;


        //On runtime we want an obj for ease of importing. In editor DAE is preferred for ease of saving prefabs and avoiding bugs with obj import
        if (editor)
            allProcesses.Add(GenerateDAE(ifcConvert, inputFile, outputFile));
        else
            allProcesses.Add(GenerateOBJ(ifcConvert, inputFile, outputFile));
        if (!IfcXmlParser.CheckMenuCondition("parallelProcessingEnabled", options))
        {
            WaitToFinish(allProcesses);
        }
        //Create .xml
        allProcesses.Add(GenerateXML(ifcConvert, inputFile, outputFile));

        //Wait for the processes to finish
        WaitToFinish(allProcesses);
    }

    public static Process CallIFCConverter(string extension, string options, string ifcConvert, string inputFile, string outputFile)
    {
        //UnityEngine.Debug.Log("CallIFCCOnverter for " + inputFile + "." + extension);
        inputFile = System.IO.Path.GetFullPath(inputFile);
        outputFile = System.IO.Path.GetFullPath(outputFile) + "_" + extension;
        ifcConvert = System.IO.Path.GetFullPath(ifcConvert);
        inputFile = "\"" + inputFile + "\"";

        Process ifcConverter = new Process();
        //Number of threads for ifcOpenShell
        int threads = SystemInfo.processorCount;
        //Different handling for Linux and Windows
        //Note: cmd.exe doesn't like single quotes.
        if (IsWindows())
        {
            ifcConverter.StartInfo.FileName = "cmd.exe";
            //We have no way to answer to any confirmation queries, so include -y to deal with them. Otherwise Unity freezes when, for example, the file already exists.
            String args = " /C \"\"" + ifcConvert + "\" -y -j " + threads + " " + options + " " + inputFile + " \"" + outputFile + "." + extension + "\" > \"" + outputFile + "_log.txt\"\"";
            ifcConverter.StartInfo.Arguments = args;
        }
        if (IsUnix())
        {
            ifcConverter.StartInfo.FileName = "bash";
            String args = " -c \"\"" + ifcConvert + "\" -y -j " + threads + " " + options + " " + inputFile + " \"" + outputFile + "." + extension + "\"\"";
            ifcConverter.StartInfo.Arguments = args;
            //Redirect output to console
            ifcConverter.StartInfo.UseShellExecute = false;
            ifcConverter.StartInfo.RedirectStandardOutput = true;
            ifcConverter.StartInfo.RedirectStandardError = true;
        }
        ifcConverter.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        ifcConverter.Start();

        if (IsUnix())
        {
            /*//Line by line event handlers
            ifcConverter.OutputDataReceived += new DataReceivedEventHandler((s, e) => 
            {
                if (e.Data != null)
                {
                    UnityEngine.Debug.Log(e.Data);
                }
            });
            ifcConverter.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (e.Data != null)
                {
                    UnityEngine.Debug.LogError(e.Data);
                }
            });*/

            //Line by line output
            //ifcConverter.BeginOutputReadLine();
            //ifcConverter.BeginErrorReadLine();

            //Full output
            string standard_output = ifcConverter.StandardOutput.ReadToEnd();
            string standard_error = ifcConverter.StandardError.ReadToEnd();
            if (standard_output != "")
                UnityEngine.Debug.Log("output: " + standard_output);
            if (standard_error != "")
                UnityEngine.Debug.LogError("errors: " + standard_error);
        }


        return ifcConverter;

    }

    public static bool IsWindows()
    {
        RuntimePlatform[] windows_platforms = { RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor };
        return windows_platforms.Contains(Application.platform);
    }

    public static bool IsUnix()
    {
        RuntimePlatform[] unix_platforms = { RuntimePlatform.LinuxPlayer, RuntimePlatform.LinuxEditor, RuntimePlatform.OSXEditor, RuntimePlatform.OSXPlayer };
        return unix_platforms.Contains(Application.platform);
    }

    public static void WaitToFinish(List<Process> processes)
    {
        foreach (Process ifcConverter in processes)
        {
            ifcConverter.WaitForExit();
            if (ifcConverter.ExitCode == 1)
            {
                UnityEngine.Debug.LogError("IFCOpenShell could not generate something.");
            }
        }
    }

    public static Process GenerateDAE(string ifcConvert, string inputFile, string outputFile)
    {
        //--use-element-guids so that we can identify the elements later on
        return CallIFCConverter("dae", "--use-element-guids --use-element-hierarchy", ifcConvert, inputFile, outputFile);
    }

    public static Process GenerateXML(string ifcConvert, string inputFile, string outputFile)
    {
        return CallIFCConverter("xml", "", ifcConvert, inputFile, outputFile);
    }

    public static Process GenerateOBJ(string ifcConvert, string inputFile, string outputFile)
    {
        return CallIFCConverter("obj", "--use-element-guids", ifcConvert, inputFile, outputFile);
    }

    //Looks for the external IfcConvert program we use to convert ifc to obj/dae/xml
    //Will get horribly confused if there's more than one file with IfcConvert in the name
    private static string FindIfcConvert()
    {
        //For Windows: could do "*.exe" to look for exes only. Not really necessary though..
        string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "/ImportIFC", "*", SearchOption.TopDirectoryOnly);
        foreach (string fp in filePaths)
        {
            string[] splits = fp.Split('/');
            string f = splits[splits.Length - 1];
            if (f.Contains("IfcConvert"))
            {
                return f;
            }
        }
        return null;
    }

    public static GameObject LoadDae(string inputFile)
    {
        //Save files under resources so that we can access them later to create the prefab
        string resourceName = System.IO.Path.GetFileNameWithoutExtension(inputFile);
        string outputFile = "Assets/Resources/" + System.IO.Path.GetFileNameWithoutExtension(inputFile);

        GameObject IfcDaeInstance = null;
        GameObject IfcDae = (GameObject)Resources.Load(resourceName + "_dae");

        //Create an instance of it, so that it actually exists in the game world
        IfcDaeInstance = Instantiate(IfcDae);

        //Load and parse the xml, with the brand new IfcDaeInstance as the root object
        /*string xml_path = "Assets/Resources/" +resourceName + "_xml.xml";
        IfcXmlParser.parseXmlFile(xml_path, IfcDaeInstance);*/
        return IfcDaeInstance;
    }
}
