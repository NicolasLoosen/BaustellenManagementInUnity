using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class IfcXmlParser : MonoBehaviour
{
    /*
    This script reads and adds semantic IFC data to imported gameobjects.
    It is called by the IfcImporter.cs editor script.
    */
    public static Dictionary<string, Dictionary<string, string>> ifcPropertySets;
    public static Dictionary<string, Dictionary<string, string>> ifcQuantities;
    public static Dictionary<string, Dictionary<string, string>> ifcTypes;
    public static List<string> distinctIfcTypes;
    public static Dictionary<string, Dictionary<string, string>> ifcMaterials;

    void Start()
    {
        // Usually this script is used when an ifc is dragged into Unity. Start() is used when this script is attached to a .dae file.
        //DateTime starttime = System.DateTime.Now;     // for measuring optimization

        string xml_path = "Assets/Resources/" + this.name.Remove(this.name.Length - 3) + "xml.xml";
        parseXmlFile(xml_path, this.gameObject, null);

        //DateTime endtime = System.DateTime.Now;       // for measuring optimization
        //Debug.Log(endtime - starttime);               // for measuring optimization
    }

    public static void parseXmlFile(string xml_path, GameObject parent = null, Dictionary<string, bool> options = null)
    {
        // Read data from ifc xml file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xml_path);

        // import ifc header hierarchy and store the data in a component
        if (CheckMenuCondition("headerEnabled", options))
        {
            XmlNodeList NodeHeader = xmlDoc.SelectNodes("//ifc/header");
            if (parent != null)
            {
                IfcHeader ifcHead = parent.AddComponent<IfcHeader>() as IfcHeader;
                ParseIfcHeader(NodeHeader, ifcHead);
            }
        }

        // import ifc units and store them in a component
        if (CheckMenuCondition("unitsEnabled", options))
        {
            XmlNodeList NodeUnits = xmlDoc.SelectNodes("//ifc/units");
            if (parent != null)
            {
                IfcUnits ifcUnits = parent.AddComponent<IfcUnits>() as IfcUnits;
                ParseIfcUnits(NodeUnits, ifcUnits);
            }
        }

        // ifc properties to dictionary
        if (CheckMenuCondition("propertiesEnabled", options))
        {
            XmlNodeList NodeListPsets = xmlDoc.SelectNodes("//ifc/properties/IfcPropertySet");
            ParseIfcProperties(NodeListPsets);
        }

        // ifc quantities to dictionary
        if (CheckMenuCondition("quantitiesEnabled", options))
        {
            XmlNodeList NodeListQuants = xmlDoc.SelectNodes("//ifc/quantities/IfcElementQuantity");
            ParseIfcQuantities(NodeListQuants);
        }

        // ifc types to dictionary
        if (CheckMenuCondition("typesEnabled", options))
        {
            XmlNodeList NodeListTypes = xmlDoc.SelectNodes("//ifc/types");
            ParseIfcTypes(NodeListTypes);
        }

        // itc material layers and materials to dictionary
        if (CheckMenuCondition("materialsEnabled", options))
        {
            XmlNodeList NodeListMats = xmlDoc.SelectNodes("//ifc/materials");
            ParseIfcMaterials(NodeListMats);
        }

        // component for ids and presentation layers in parent gameobject
        IfcRootLists ifcRootList = null;
        if (parent)
        {
            ifcRootList = parent.AddComponent<IfcRootLists>() as IfcRootLists;
        }

        // ifc decomposition hierarchy
        //string xmlPathPattern = "//ifc/decomposition/IfcProject";
        string xmlPathPattern = "//ifc/decomposition";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);
        ParseIfcHierarchy(myNodeList, ifcRootList, parent, options);
    }

    private static void ParseIfcHierarchy(XmlNodeList nlist, IfcRootLists ifcRootList, GameObject parent, Dictionary<string, bool> options)
    {
        // recursive loop for xml decomposition hierarchy
        foreach (XmlNode node in nlist)
        {
            //Skip elements without id attribute, elements without a corresponding gameobject and IfcProject
            //if(node.Attributes["id"] != null && GameObject.Find(node.Attributes["id"].InnerText) && node.Name != "IfcProject"){
            if ((node.Attributes["id"] != null && GameObject.Find(node.Attributes["id"].InnerText)) || node.Name == "IfcProject")
            {
                Debug.Log(node.Name);

                // FIND GAMEOBJECT BASED ON IFC ID AND FIX DUPLICATES
                string nodeId = node.Attributes["id"].InnerText;
                if (GameObject.Find(nodeId + " 1"))
                {
                    FixIfcIdDuplicate(nodeId);
                }
                GameObject nodeGameObj;
                if (node.Name != "IfcProject")
                {
                    nodeGameObj = GameObject.Find(nodeId);
                    // RENAME GAMEOBJECT
                    if (node.Attributes["Name"] != null)
                    {
                        nodeGameObj.name = node.Attributes["Name"].InnerText;
                        //ex: Basic Wall:Interior - Furring (152 mm Stud):189901
                    }
                    else
                    {
                        nodeGameObj.name = node.Name;
                        //ex: IfcBuilding or IfcWallStandardCase
                    }
                }
                else
                {
                    nodeGameObj = parent;
                }

                // ADD ATTRIBUTES TO GAMEOBJECT
                if (CheckMenuCondition("attributesEnabled", options))
                {
                    // Write ifc element type and attributes to GameObject IfcAttributes component
                    IfcAttributes ifcAttri = nodeGameObj.AddComponent<IfcAttributes>() as IfcAttributes;
                    ifcAttri.attributes.Add("IfcElementType");
                    ifcAttri.values.Add(node.LocalName);
                    for (int i = 0; i < node.Attributes.Count; i++)
                    {
                        ifcAttri.attributes.Add(node.Attributes[i].Name);
                        ifcAttri.values.Add(node.Attributes[i].InnerText);
                    }
                }

                // ADD PROPERTIES TO GAMEOBJECT
                if (CheckMenuCondition("propertiesEnabled", options))
                {
                    // Write property names and values to GameObject IfcProperties component
                    if (node.SelectNodes("IfcPropertySet").Count > 0)
                    {
                        // Attach IfcProperties component
                        IfcProperties ifcProps = nodeGameObj.AddComponent<IfcProperties>() as IfcProperties;
                        // Loop property sets
                        foreach (XmlNode propertyNode in node.SelectNodes("IfcPropertySet"))
                        {
                            string id = propertyNode.Attributes["xlink:href"].InnerText.Substring(1);
                            if (ifcPropertySets.ContainsKey(id) == false)
                            {
                                continue;
                            }
                            // Add property names and values to the IfcProperties component
                            foreach (KeyValuePair<string, string> kvpair in ifcPropertySets[id])
                            {
                                ifcProps.properties.Add(kvpair.Key);
                                ifcProps.nominalValues.Add(kvpair.Value);
                            }
                        }
                    }
                }

                // ADD QUANTITIES TO GAMEOBJECT
                if (CheckMenuCondition("quantitiesEnabled", options))
                {
                    if (node.SelectNodes("IfcElementQuantity").Count > 0)
                    {
                        // Attach IfcQuantities component
                        IfcQuantities ifcQuants = nodeGameObj.AddComponent<IfcQuantities>() as IfcQuantities;
                        // Loop IfcElementQuantity elements refered by hierarcy node
                        foreach (XmlNode elementQuantity in node.SelectNodes("IfcElementQuantity"))
                        {
                            // remove # from link string
                            string id = elementQuantity.Attributes["xlink:href"].InnerText.Substring(1);
                            if (ifcQuantities.ContainsKey(id) == false)
                            {
                                continue;
                            }
                            // Add quantity names and values to the IfcQuantities component
                            foreach (KeyValuePair<string, string> kvpair in ifcQuantities[id])
                            {
                                ifcQuants.quantities.Add(kvpair.Key);
                                ifcQuants.values.Add(kvpair.Value);
                            }
                        }
                    }
                }


                // ADD MATERIALS TO GAMEOBJECT
                // function for attaching and populating IfcMaterials components
                void AddMaterial(string materialNodeType)
                {
                    // Attach ifcMaterials component 
                    IfcMaterials ifcMats = nodeGameObj.AddComponent<IfcMaterials>() as IfcMaterials;
                    // Loop material layer sets (there should only be one)
                    XmlNode materialNode = node.SelectNodes(materialNodeType)[0];
                    string id = materialNode.Attributes["xlink:href"].InnerText.Substring(1);
                    // cancel if the material id in the hierarchy isn't in the ifcMaterials dictionary
                    if (ifcMaterials.ContainsKey(id) == false)
                    {
                        return;
                    }
                    // Add material layer names and thicknesses to the ifcMaterials component
                    foreach (KeyValuePair<string, string> kvpair in ifcMaterials[id])
                    {
                        //remove leading numbers from material name
                        ifcMats.materials.Add(kvpair.Key.Remove(0, 4));
                        ifcMats.thicknesses.Add(kvpair.Value);
                    }
                }
                // use function to add materials
                if (CheckMenuCondition("materialsEnabled", options))
                {
                    // First check for material layer sets
                    if (node.SelectNodes("IfcMaterialLayerSetUsage").Count > 0)
                    {
                        AddMaterial("IfcMaterialLayerSetUsage");
                    }
                    // alternatively use material list
                    else if (node.SelectNodes("IfcMaterialList").Count > 0)
                    {
                        AddMaterial("IfcMaterialList");
                    }
                    // alternatively use single materials
                    else if (node.SelectNodes("IfcMaterial").Count > 0)
                    {
                        AddMaterial("IfcMaterial");
                    }
                }

                // ADD TYPES TO GAMEOBJECT
                // loop for each unique type and style
                if (CheckMenuCondition("typesEnabled", options))
                {
                    foreach (string type in distinctIfcTypes)
                    {
                        if (node.SelectNodes(type).Count > 0)
                        {
                            IfcTypes ifcTyp = nodeGameObj.AddComponent<IfcTypes>() as IfcTypes;
                            // Loop various ifc types and styles if there's more than one
                            foreach (XmlNode typeNode in node.SelectNodes(type))
                            {
                                // remove # from link string
                                string id = typeNode.Attributes["xlink:href"].InnerText.Substring(1);
                                if (ifcTypes.ContainsKey(id) == false)
                                {
                                    continue;
                                }
                                // Add type attributes and values to the IfcTypes component
                                foreach (KeyValuePair<string, string> kvpair in ifcTypes[id])
                                {
                                    ifcTyp.types.Add(kvpair.Key);
                                    ifcTyp.values.Add(kvpair.Value);
                                }
                            }
                            // Attach IfcMaterials component 
                            IfcMaterials ifcMats = nodeGameObj.AddComponent<IfcMaterials>() as IfcMaterials;
                        }
                    }
                }



                // ADD GAMEOBJECT TO ROOT LISTS
                if (CheckMenuCondition("rootListsEnabled", options))
                {
                    ifcRootList.ifcGameObject.Add(nodeGameObj);
                    ifcRootList.ifcId.Add(nodeId);
                    ifcRootList.ifcElementType.Add(node.LocalName);
                    // add layer or no layer
                    if (node.SelectNodes("IfcPresentationLayerAssignment").Count > 0)
                    {
                        ifcRootList.ifcPresentationLayer.Add(node.SelectNodes("IfcPresentationLayerAssignment")[0].Attributes["xlink:href"].InnerText.Substring(1));
                    }
                    else
                    {
                        ifcRootList.ifcPresentationLayer.Add("No layer");
                    }
                }

                // ADD COLLIDER TO GAMEOBJECT
                if (CheckMenuCondition("meshCollidersEnabled", options))
                {
                    if (nodeGameObj.GetComponent<MeshFilter>() != null)
                    {
                        MeshCollider myCollider = nodeGameObj.AddComponent<MeshCollider>() as MeshCollider;
                        //myCollider.convex = true;
                        nodeGameObj.isStatic = true;
                    }
                }

            }
            //Repeat for node children
            ParseIfcHierarchy(node.ChildNodes, ifcRootList, parent, options);
        }
    }

    private static void ParseIfcHeader(XmlNodeList nlistHeader, IfcHeader headerComponent)
    {
        // recursive loop for xml header hierarchy
        foreach (XmlNode headerNode in nlistHeader)
        {
            if (headerNode.NodeType.ToString() == "Text")
            {
                headerComponent.headers.Add(headerNode.ParentNode.Name);
                headerComponent.values.Add(headerNode.Value);
            }
            ParseIfcHeader(headerNode.ChildNodes, headerComponent);
        }
    }

    private static void ParseIfcUnits(XmlNodeList unitsHierarchy, IfcUnits unitsComponent)
    {
        // Loop through the ifc units data and store it in the IfcUnits component
        foreach (XmlNode units in unitsHierarchy)
        {
            foreach (XmlNode unitNode in units.ChildNodes)
            {
                if (unitNode.Attributes["UnitType"] != null)
                {
                    unitsComponent.unitType.Add(unitNode.Attributes["UnitType"].InnerText);
                    // Note: some ifc model units include the "prefix" attribute
                    unitsComponent.unitName.Add(unitNode.Attributes["Name"].InnerText);
                    unitsComponent.si_equivalent.Add(unitNode.Attributes["SI_equivalent"].InnerText);
                }
            }
        }
    }

    private static void ParseIfcProperties(XmlNodeList nlistPsets)
    {
        //CREATE DICTIONARY OF IFC PROPERTY SETS
        //An entry in the dictionary looks like this: [id, [propertyName, propertyValue] ]
        ifcPropertySets = new Dictionary<string, Dictionary<string, string>>();

        foreach (XmlNode PsetNode in nlistPsets)
        {
            // dictionary for the single values of the property set
            Dictionary<string, string> PsetValues = new Dictionary<string, string>();
            PsetValues.Add("PsetName", PsetNode.Attributes["Name"].InnerText);

            // Single property names and values
            foreach (XmlNode PsetSingleValue in PsetNode.SelectNodes("IfcPropertySingleValue"))
            {
                if (!PsetValues.ContainsKey(PsetSingleValue.Attributes["Name"].InnerText))
                {
                    try
                    {
                        PsetValues.Add(PsetSingleValue.Attributes["Name"].InnerText, PsetSingleValue.Attributes["NominalValue"].InnerText);
                    }
                    catch
                    {
                        // some properties are missing their NominalValue
                        // Debug.Log("Property without NominalValue: " + PsetSingleValue.Attributes["Name"].InnerText);
                        PsetValues.Add(PsetSingleValue.Attributes["Name"].InnerText, "");
                    }
                }
            }
            if (!ifcPropertySets.ContainsKey(PsetNode.Attributes["id"].InnerText))
            {
                ifcPropertySets.Add(PsetNode.Attributes["id"].InnerText, PsetValues);
            }
        }
    }

    private static void ParseIfcQuantities(XmlNodeList nlistQuants)
    {
        //CREATE DICTIONARY OF IFC QUANTITIES
        //An entry in the dictionary looks like this: [id, [quantityName, quantityValue] ]
        ifcQuantities = new Dictionary<string, Dictionary<string, string>>();
        // loop IfcElementQuantity elements
        foreach (XmlNode elementQuantityNode in nlistQuants)
        {
            Dictionary<string, string> quantityValues = new Dictionary<string, string>();
            //loop IfcQuantityArea, IfcQuantityLength, IfcQauntityVolume elements
            foreach (XmlNode quantityNode in elementQuantityNode.ChildNodes)
            {
                XmlAttributeCollection qAttributes = quantityNode.Attributes;
                // add the first and the last attribute, i.e. name and value
                if (!quantityValues.ContainsKey(qAttributes[0].InnerText))
                {
                    quantityValues.Add(qAttributes[0].InnerText, qAttributes[qAttributes.Count - 1].InnerText);
                }
            }
            if (!ifcQuantities.ContainsKey(elementQuantityNode.Attributes["id"].InnerText))
            {
                ifcQuantities.Add(elementQuantityNode.Attributes["id"].InnerText, quantityValues);
            }
        }
    }

    private static void ParseIfcTypes(XmlNodeList nlistTypes)
    {
        //CREATE DICTIONARY OF IFC TYPES
        //An entry in the dictionary looks like this: [id, [attributeName, attributeValue] ]
        ifcTypes = new Dictionary<string, Dictionary<string, string>>();
        distinctIfcTypes = new List<string>();
        // loop ifc types
        foreach (XmlNode typeNode in nlistTypes[0].ChildNodes)
        {
            Dictionary<string, string> typeValues = new Dictionary<string, string>();
            // add IfcType
            typeValues.Add("type", typeNode.LocalName);
            // loop attributes of type
            foreach (XmlAttribute attribute in typeNode.Attributes)
            {
                typeValues.Add(attribute.Name, attribute.InnerText);
            }
            if (!ifcTypes.ContainsKey(typeNode.Attributes["id"].InnerText))
            {
                ifcTypes.Add(typeNode.Attributes["id"].InnerText, typeValues);
            }
            // populate list of distinct types
            if (!distinctIfcTypes.Contains(typeNode.LocalName))
            {
                distinctIfcTypes.Add(typeNode.LocalName);
            }
        }
    }

    private static void ParseIfcMaterials(XmlNodeList nlistMats)
    {
        // CREATE DICTIONARY OF VARIOUS IFC MATERIAL DATA
        // Including IFC Material Layer Set, IFC Material
        // An entry in the dictionary looks like: [id, [material, thickness] ]
        ifcMaterials = new Dictionary<string, Dictionary<string, string>>();

        // Parse material layer sets and save to dictionary
        foreach (XmlNode mSetNode in nlistMats[0].SelectNodes("IfcMaterialLayerSetUsage"))
        {
            // dictionary for single material layers in the material layer set
            Dictionary<string, string> MsetValues = new Dictionary<string, string>();

            // Single layer names and layer thicknesses
            int layerNum = 0;
            foreach (XmlNode MaterialLayer in mSetNode.SelectNodes("IfcMaterialLayer"))
            {
                // add leading numbers to avoid duplicate layer names
                string lNum = String.Format("{0:D4}", layerNum);
                MsetValues.Add(lNum + MaterialLayer.Attributes["Name"].InnerText, MaterialLayer.Attributes["LayerThickness"].InnerText);
                layerNum++;
            }
            if (!ifcMaterials.ContainsKey(mSetNode.Attributes["id"].InnerText))
            {
                ifcMaterials.Add(mSetNode.Attributes["id"].InnerText, MsetValues);
            }
        }

        // Parse (single) materials and save to dictionary
        // materials are treated as material layer sets with one layer and no thickness
        foreach (XmlNode mNode in nlistMats[0].SelectNodes("IfcMaterial"))
        {
            Dictionary<string, string> mValues = new Dictionary<string, string>();
            // add leading numbers to name and empty string thickness
            mValues.Add("0000" + mNode.Attributes["Name"].InnerText, "");
            if (!ifcMaterials.ContainsKey(mNode.Attributes["id"].InnerText))
            {
                ifcMaterials.Add(mNode.Attributes["id"].InnerText, mValues);
            }
        }

        // Parse material lists and save to dictionary
        // material lists are treated as material layers without thicknessess
        foreach (XmlNode mLNode in nlistMats[0].SelectNodes("IfcMaterialList"))
        {
            Dictionary<string, string> mListValues = new Dictionary<string, string>();

            int layerNum = 0;
            foreach (XmlNode ifcMaterial in mLNode.SelectNodes("IfcMaterial"))
            {
                string lNum = String.Format("{0:D4}", layerNum);
                mListValues.Add(lNum + ifcMaterial.Attributes["Name"].InnerText, "");
                layerNum++;
            }
            if (!ifcMaterials.ContainsKey(mLNode.Attributes["id"].InnerText))
            {
                ifcMaterials.Add(mLNode.Attributes["id"].InnerText, mListValues);
            }
        }
    }

    public static void FixIfcIdDuplicate(string id)
    {
        GameObject objA = GameObject.Find(id);
        GameObject objB = GameObject.Find(id + " 1");
        GameObject realObj;
        GameObject deleteObj;

        if (objA.GetComponent<MeshFilter>() != null)
        {
            realObj = objA;
            deleteObj = objB;
        }
        else
        {
            realObj = objB;
            deleteObj = objA;
        }
        Transform[] allChildren = deleteObj.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            //Remove child duplicates
            if (child.name.Substring(child.name.Length - 2) == ":1")
            {
                DestroyImmediate(child);
            }
            else
            {
                //move children to the correct object
                child.parent = realObj.transform;
            }
            child.parent = realObj.transform;
        }
        deleteObj.name = id + " delete";
        realObj.name = id;
        DestroyImmediate(deleteObj);
    }

    public static bool CheckMenuCondition(string menuOption, Dictionary<string, bool> options)
    {
        // A function for checking if a setting is enabled in the IFC Importer options
        if (options == null)
        {
            return true;
        }
        if (options.ContainsKey(menuOption) == false)
        {
            //Debug.LogWarning("Options does not contain key "+menuOption+". Plese report this issue to the developers.");
            return true;
        }
        return options[menuOption];
    }

}
