using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcRootLists : MonoBehaviour
{
    /* 
    This is a script that stores and searches ifc ids, element types and presentation layers.
    It is attached to the ifc root gameobject by IfcXmlParser.cs
    */

    // These are populated by IfcXmlParser
    public List<GameObject> ifcGameObject = new List<GameObject>();
    public List<string> ifcId = new List<string>();
    public List<string> ifcElementType = new List<string>();
    public List<string> ifcPresentationLayer = new List<string>();

    // Return corresponding GameObject for ifc Id
    public GameObject FindIfcGameObject(string id){
        for(int i = 0; i < ifcId.Count; i++){
            if(ifcId[i] == id){
                return ifcGameObject[i];
            }
        }
        return null;
    }

    // Return GameObjects of named element type (e.g. IfcWallStandardCase)
    public List<GameObject> FindIfcElementTypeGameObjects(string elementTypeName){
        List<GameObject> elementGameObjects = new List<GameObject>();
        for(int i = 0; i < ifcElementType.Count; i++){
            if(ifcElementType[i] == elementTypeName){
                elementGameObjects.Add( ifcGameObject[i] );
            }
        }
        return elementGameObjects;
    }

    // Return GameObjects on ifc presentation layer
    public List<GameObject> FindIfcLayerGameObjects(string layerName){
        List<GameObject> layerGameObjects = new List<GameObject>();
        for(int i = 0; i < ifcPresentationLayer.Count; i++){
            if(ifcPresentationLayer[i] == layerName){
                layerGameObjects.Add( ifcGameObject[i] );
            }
        }
        return layerGameObjects;
    }

    // Enable or disable ifc GameObjects on layer
    public void IfcLayerSetActive(string layerName, bool onoff){
        for(int i = 0; i < ifcPresentationLayer.Count; i++){
            if(ifcPresentationLayer[i] == layerName){
                ifcGameObject[i].SetActive(onoff);
            }
        }
    }
    
    // Enable or disable ifc GameObjects of named element type (e.g. IfcWallStandardCase)
    public void IfcElementTypeSetActive(string elementTypeName, bool onoff){
        for(int i = 0; i < ifcElementType.Count; i++){
            if(ifcElementType[i] == elementTypeName){
                ifcGameObject[i].SetActive(onoff);
            }
        }
    }

}
