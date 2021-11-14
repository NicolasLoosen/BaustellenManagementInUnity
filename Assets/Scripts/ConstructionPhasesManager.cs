using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.PrefabClasses;
using System;
using System.Linq;
using System.IO;
using CommandUndoRedo;
using System.Threading;
using System.Threading.Tasks;

public class ConstructionPhasesManager : MonoBehaviour
{
    public PrefabRegistry prefabRegistry;

    public GameObject objectContainer;
    public MessagePrinter messagePrinter;

    private int currentPhase;
    private Dictionary<int, PrefabContainer> placedObjects;
    private List<Dictionary<int, PlacedObjectState>> placedObjectsStates;

    /// <summary>
    /// Used for the Database, contain status of tasks values and what is drawn around the objects
    /// TODO: remove from here and include in placedObjectState
    /// </summary>
    private Dictionary<int, Task<ConstructionValues>> databaseTasks;
    private Dictionary<int, ConstructionValues> databaseValues;
    private Dictionary<int, WhatIsDrawn> whatsDrawn;


    //Is called before start and once in lifetime
    private void Awake()
    {
        InitializePrefabRegistry();
    }

    // Use this for initialization
    void Start()
    {
        currentPhase = 0;

        //Avoid reinitialisation when Lists are already assigned from loading
        if (placedObjects == null)
        {
            placedObjects = new Dictionary<int, PrefabContainer>();
        }
        if (placedObjectsStates == null)
        {
            placedObjectsStates = new List<Dictionary<int, PlacedObjectState>>();
            placedObjectsStates.Add(new Dictionary<int, PlacedObjectState>());
        }

        if (databaseTasks == null)
        {
            databaseTasks = new Dictionary<int, Task<ConstructionValues>>();
        }

        if (databaseValues == null)
        {
            databaseValues = new Dictionary<int, ConstructionValues>();
        }

        if (whatsDrawn == null)
        {
            whatsDrawn = new Dictionary<int, WhatIsDrawn>();
        }
    }

    private void InitializePrefabRegistry()
    {
        //Building Prefabs
        string path = "Prefabs/Building/";
        List<PrefabContainer> prefabs = LoadApplicationPrefabs(path, PrefabType.BUILDING);
        prefabRegistry.AddPrefabs(prefabs);
        path ="ImportIFC/Building/";
        prefabs = LoadImportIfcFiles(path, PrefabType.BUILDING);
        prefabRegistry.AddPrefabs(prefabs);

        //Misc Prefabs
        path = "Prefabs/Misc/";
        prefabs = LoadApplicationPrefabs(path, PrefabType.MISC);
        prefabRegistry.AddPrefabs(prefabs);
        path ="ImportIFC/Misc/";
        prefabs = LoadImportIfcFiles(path, PrefabType.MISC);
        prefabRegistry.AddPrefabs(prefabs);

        //Vehicle Prefabs
        path = "Prefabs/Vehicle/";
        prefabs = LoadApplicationPrefabs(path, PrefabType.VEHICLE);
        prefabRegistry.AddPrefabs(prefabs);
        path ="ImportIFC/Vehicle/";
        prefabs = LoadImportIfcFiles(path, PrefabType.VEHICLE);
        prefabRegistry.AddPrefabs(prefabs);

        /* Imported Model
        /* not in use at the moment
        */
        //path = System.IO.Directory.GetCurrentDirectory() + "/ImportIFC/";
        //prefabs = LoadImportIfcFiles(path);
        //prefabRegistry.AddPrefabs(prefabs);

    }

    private List<PrefabContainer> LoadApplicationPrefabs(string path, PrefabType prefabType)
    {
        List<PrefabContainer> prefabContainers = new List<PrefabContainer>();
        GameObject[] prefabFiles = Resources.LoadAll<GameObject>(path);

        foreach (GameObject prefabFile in prefabFiles)
        {
            if (prefabFile != null)
            {
                PrefabContainer newPrefab = new PrefabContainer();
                newPrefab.Id = prefabFile.name;
                newPrefab.PrefabType = prefabType;
                newPrefab.Prefab = prefabFile;

                prefabContainers.Add(newPrefab);
            }
        }

        return prefabContainers;
    }

    private List<PrefabContainer> LoadImportIfcFiles(string importPath, PrefabType prefabType)
    {
        List<PrefabContainer> prefabContainers = new List<PrefabContainer>();
        
        string[] files = new string[0];
        if (Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "/" +importPath))
        {
            files = Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "/" + importPath, "*.ifc");
        }

        foreach (string path in files)
        {
            PrefabContainer impPrefab = new PrefabContainer();
            impPrefab.Id = Path.GetFileNameWithoutExtension(path);
            Debug.Log(impPrefab.Id);
            impPrefab.Prefab = IfcImporter.RuntimeImport(importPath + Path.GetFileName(path));
            impPrefab.Prefab.transform.Rotate(-90, 0, 0);
            impPrefab.PrefabType = prefabType;
            prefabContainers.Add(impPrefab);
            impPrefab.Prefab.SetActive(false);
        }

        return prefabContainers;
    }

    public Dictionary<int, PrefabContainer> PlacedObjects
    {
        get { return placedObjects; }
        set { this.placedObjects = value; }
    }

    public List<Dictionary<int, PlacedObjectState>> PlacedObjectsStates
    {
        get { return placedObjectsStates; }
        set { this.placedObjectsStates = value; }
    }

    public Dictionary<int, PlacedObjectState> PlacedObjectStatesInCurrentPhase
    {
        get { return placedObjectsStates[currentPhase]; }
    }

    /// <summary>
    /// Set up the PlacedObjects and PlacedObjectsStates Lists with the passed items.
    /// </summary>
    /// <param name="serialPlacedObjects"></param>
    /// <param name="serialPlacedObjectsStates"></param>
    public void LoadLists(List<PrefabContainer> serialPlacedObjects, List<List<SerializablePlacedObjectState>> serialPlacedObjectsStates)
    {
        //Initialize temp Lists. They will replace later the used ones.
        Dictionary<int, PrefabContainer> tempPlacedObjects = new Dictionary<int, PrefabContainer>();
        List<Dictionary<int, PlacedObjectState>> tempPlacedObjectsStates = new List<Dictionary<int, PlacedObjectState>>();

        List<SerializablePlacedObjectState> phaseZero = serialPlacedObjectsStates[0];
        PlacedObjectState deserializedState = new PlacedObjectState();
        PlacedObjectState instantiatonState = new PlacedObjectState();

        List<int> missingPrefabs = new List<int>();

        //Fill tempPlacedObjects-List
        for (int i = 0; i < serialPlacedObjects.Count; i++)
        {
            //Get Prefab from Registry, bc serialized container does not store the prefab itself
            PrefabContainer serialContainer = serialPlacedObjects[i];
            PrefabContainer registryEntry = prefabRegistry.GetPrefab(serialContainer.Id, serialContainer.PrefabType);

            //Check if prefab is available
            if (registryEntry == null)
            {
                missingPrefabs.Add(i);
                SendToMsgPrinter("Prefab is not in registry. This will be skipped. Name: " + serialContainer.Id);
                continue; //Skip this iteration
            }

            instantiatonState.Serialization = phaseZero[i];
            if (registryEntry.PrefabType == PrefabType.IMPORTED_MODEL)
            {   //Instantiating resets the rotation applied while importing the fbx
                instantiatonState.Rotation = Quaternion.Euler(90, 0, 0);
            }
            PrefabContainer instantiated = InstantiateObject(registryEntry, instantiatonState);
            deserializedState.Serialization = phaseZero[i];
            instantiated.Prefab.transform.rotation = deserializedState.Rotation;
            serialContainer.Prefab = instantiated.Prefab; //Needed as reference for later usage
            tempPlacedObjects.Add(instantiated.Prefab.GetInstanceID(), instantiated);
        }

        //Fill tempPlacedObjectsStates-List
        //For each phase-List in serialPlacedObjectsStates
        for (int currentPhaseIndex = 0; currentPhaseIndex < serialPlacedObjectsStates.Count; currentPhaseIndex++)
        {
            //Get reference to phase
            List<SerializablePlacedObjectState> serializedPhase = serialPlacedObjectsStates[currentPhaseIndex];

            //Create a new Phase in target list
            Dictionary<int, PlacedObjectState> tempPhase = new Dictionary<int, PlacedObjectState>();
            tempPlacedObjectsStates.Add(tempPhase);

            //For each entry in current serialized phase
            for (int i = 0; i < serializedPhase.Count; i++)
            {
                //If Prefab is missing, skip iteration
                if (missingPrefabs.Contains(i))
                {
                    continue;
                }
                //Get Instance ID of object in PlacedObjects list;
                int instanceId = serialPlacedObjects[i].Prefab.GetInstanceID();
                //Get state and deserialize
                PlacedObjectState state = new PlacedObjectState();
                state.Serialization = serializedPhase[i];
                //Add entry to tempPhase list
                tempPhase.Add(instanceId, state);
            }
        }

        //Reassign lists and trigger reload
        this.placedObjects = tempPlacedObjects;
        this.placedObjectsStates = tempPlacedObjectsStates;
        this.GoToPhase(0);

    }

    /// <summary>
    /// This Method adds a new object to the construction site and also generates the object states for all phases.
    /// </summary>
    public void AddObject(int index, PrefabType type, PrefabModificationOption modificationOption, Vector3 position, bool isActive = true)
    {

        PrefabContainer registryContainer = prefabRegistry.GetPrefab(index, type);
        PlacedObjectState instantiationState = new PlacedObjectState(position, registryContainer.Prefab.transform.rotation, registryContainer.Prefab.transform.localScale, isActive);
        PrefabContainer instantiated = InstantiateObject(registryContainer, instantiationState);

        instantiated.Prefab.SetActive(isActive);

        placedObjects.Add(instantiated.Prefab.GetInstanceID(), instantiated);
        PlacedObjectState objectState = new PlacedObjectState(position, instantiated.Prefab.transform.rotation, instantiated.Prefab.transform.lossyScale);

        CreateAddedObjectStates(instantiated.Prefab.GetInstanceID(), modificationOption, objectState);
    }

    /// <summary>
    /// This Method adds a new object to the construction site, by instantiating ist, adding it to the list of placed objects,
    /// and adding the provided state entries of the objects to the corresponding states list.
    /// </summary>
    public void AddObjectWithExistingPhases(int index, PrefabType type, List<PlacedObjectState> objectStates)
    {

        PlacedObjectState instantiationState = new PlacedObjectState(objectStates[currentPhase]);
        PrefabContainer registryContainer = prefabRegistry.GetPrefab(index, type);
        instantiationState.Rotation = registryContainer.Prefab.transform.rotation;
        PrefabContainer instantiated = InstantiateObject(registryContainer, instantiationState);


        /* 
         * Übernahme von Scalierung und Rotation von zu ersetzendem Object beim ersetzten.
         * 
        instantiated.Prefab.transform.rotation = objectStates[currentPhase].Rotation;
        instantiated.Prefab.transform.localScale = objectStates[currentPhase].LocalScale;
        */

        instantiated.Prefab.SetActive(objectStates[currentPhase].IsActive);

        placedObjects.Add(instantiated.Prefab.GetInstanceID(), instantiated);

        if (placedObjectsStates.Count == objectStates.Count)
        {
            for (int i = 0; i < placedObjectsStates.Count; i++)
            {
                placedObjectsStates[i].Add(instantiated.Prefab.GetInstanceID(), objectStates[i]);
            }
        }

    }

    public PrefabContainer InstantiateObject(PrefabContainer prefab, PlacedObjectState objectState)
    {
        PrefabContainer instantiated = new PrefabContainer();
        instantiated.Id = prefab.Id;
        instantiated.PrefabType = prefab.PrefabType;

        instantiated.Prefab = Instantiate(
            prefab.Prefab,
            objectState.Position,
            objectState.Rotation,
            objectContainer.transform
        );


        if (instantiated.PrefabType == PrefabType.IMPORTED_MODEL)
        {
            GameObject combinedMeshes = MeshCombineWizard.OnWizardCreate(instantiated.Prefab);
            Destroy(instantiated.Prefab);
            instantiated.Prefab = combinedMeshes;
        }

        instantiated.Prefab.transform.SetParent(objectContainer.transform);

        //Set MeshCollider.convex to true (enable Mesh Collider)
        /*MeshFilter meshFilter = instantiated.Prefab.gameObject.GetComponent<MeshFilter>();
        if (meshFilter)
        {
            Mesh mesh = meshFilter.mesh;
            if (mesh != null)
            {
                NonConvexMeshCollider collider = instantiated.Prefab.AddComponent(typeof(NonConvexMeshCollider)) as NonConvexMeshCollider;
                collider.boxesPerEdge = 50;
                //StartCoroutine(collider.Calculate()); //Does not work correctly, updating rotation fails
                collider.Calculate();
            }
        }*/

        AddObjectData(instantiated.Prefab, objectState);
        AddRigidBody(instantiated.Prefab);
        instantiated.Prefab.AddComponent<SnapToGrid>();
        //instantiated.Prefab.AddComponent<GenerateCollider>();

        return instantiated;

    }

    public void DeleteObject(GameObject objectToDelete, PrefabModificationOption deleteOption)
    {
        PlacedObjectState placedObjectState;
        PrefabContainer placedObject;
        bool objectExists = false;
        UndoRedoManager.Clear();

        switch (deleteOption)
        {
            case PrefabModificationOption.COMPLETE:
                foreach (Dictionary<int, PlacedObjectState> placedObjectsStatesInPhase in placedObjectsStates)
                {
                    placedObjectsStatesInPhase.Remove(objectToDelete.GetInstanceID());
                }
                break;

            case PrefabModificationOption.CURRENT_AND_FOLLOWING:
                if (currentPhase == 0)
                {
                    DeleteObject(objectToDelete, PrefabModificationOption.COMPLETE);
                }
                else
                {
                    placedObjects.TryGetValue(objectToDelete.GetInstanceID(), out placedObject);
                    placedObject.Prefab.SetActive(false);
                    for (int i = currentPhase; i < placedObjectsStates.Count; i++)
                    {
                        objectExists = placedObjectsStates[i].TryGetValue(objectToDelete.GetInstanceID(), out placedObjectState);
                        if (objectExists)
                        {
                            placedObjectState.IsActive = false;
                            objectExists = false;
                        }
                    }
                }
                break;

            case PrefabModificationOption.ONLY_IN_CURRENT:
                placedObjects.TryGetValue(objectToDelete.GetInstanceID(), out placedObject);
                placedObject.Prefab.SetActive(false);
                objectExists = placedObjectsStates[currentPhase].TryGetValue(objectToDelete.GetInstanceID(), out placedObjectState);
                if (objectExists)
                {
                    placedObjectState.IsActive = false;
                    objectExists = false;
                }
                break;
        }

        foreach (Dictionary<int, PlacedObjectState> placedObjectsStatesInPhase in placedObjectsStates)
        {
            objectExists = placedObjectsStatesInPhase.TryGetValue(objectToDelete.GetInstanceID(), out placedObjectState);
            if (objectExists)
            {
                break;
            }
        }
        if (!objectExists)
        {
            placedObjects.Remove(objectToDelete.GetInstanceID());
            Destroy(objectToDelete);
        }
    }

    public void UpdateObject(GameObject objectToUpdate, PrefabModificationOption option)
    {
        PlacedObjectState savedObjectState;
        float y = ConstructionSiteUiManager.GROUNDDIST;
        float gridSize = ConstructionSiteUiManager.snapValue;
        var position = new Vector3(
                objectToUpdate.transform.position.x,
                objectToUpdate.transform.position.y,
                objectToUpdate.transform.position.z
            );
        if (gridSize > 0)
        {
            if (position.y > ConstructionSiteUiManager.GROUNDDIST)
            {
                y = position.y;
            }
            position = new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize,
                y,
                Mathf.Round(position.z / gridSize) * gridSize
            );
        }
        else if (position.y < ConstructionSiteUiManager.GROUNDDIST)
        {
            position = new Vector3(
                position.x,
                y,
                position.z
            );
            
        }

        objectToUpdate.transform.position = position;

        switch (option)
        {
            case PrefabModificationOption.ONLY_IN_CURRENT:
                if (placedObjectsStates[currentPhase].TryGetValue(objectToUpdate.GetInstanceID(), out savedObjectState))
                {
                    savedObjectState.Position = objectToUpdate.transform.position;
                    savedObjectState.Rotation = objectToUpdate.transform.rotation;
                    savedObjectState.LocalScale = objectToUpdate.transform.localScale;
                    savedObjectState.IsActive = objectToUpdate.activeSelf; //only with option "Only in Current Phase" we update the active status
                }
                else
                {
                    Debug.LogError("No state for object with id " + objectToUpdate.GetInstanceID() + " in phase " + currentPhase + "!");
                }
                break;

            case PrefabModificationOption.COMPLETE:
                for (int i = 0; i < placedObjectsStates.Count; i++)
                {
                    if (placedObjectsStates[i].TryGetValue(objectToUpdate.GetInstanceID(), out savedObjectState))
                    {
                        savedObjectState.Position = objectToUpdate.transform.position;
                        savedObjectState.Rotation = objectToUpdate.transform.rotation;
                        savedObjectState.LocalScale = objectToUpdate.transform.localScale;
                    }
                    else
                    {
                        Debug.LogError("No state for object with id " + objectToUpdate.GetInstanceID() + " in phase " + i + "!");
                    }
                }
                break;

            case PrefabModificationOption.CURRENT_AND_FOLLOWING:
                for (int i = currentPhase; i < placedObjectsStates.Count; i++)
                {
                    if (placedObjectsStates[i].TryGetValue(objectToUpdate.GetInstanceID(), out savedObjectState))
                    {
                        savedObjectState.Position = objectToUpdate.transform.position;
                        savedObjectState.Rotation = objectToUpdate.transform.rotation;
                        savedObjectState.LocalScale = objectToUpdate.transform.localScale;
                    }
                    else
                    {
                        Debug.LogError("No state for object with id " + objectToUpdate.GetInstanceID() + " in phase " + i + "!");
                    }
                }
                break;
        }
    }

    public int HighestPhase
    {
        get { return placedObjectsStates.Count - 1; }
    }

    public int CurrentPhase
    {
        get { return currentPhase; }
    }

    public Dictionary<int, Task<ConstructionValues>> DatabaseTasks { get => databaseTasks; set => databaseTasks = value; }
    public Dictionary<int, ConstructionValues> DatabaseValues { get => databaseValues; set => databaseValues = value; }

    public int GoToNextPhase()
    {
        return GoToPhase(currentPhase + 1);
    }

    public int GoToPreviousPhase()
    {
        return GoToPhase(currentPhase - 1);
    }

    public int GoToPhase(int phase)
    {

        if (phase < 0)
        {
            return currentPhase;
        }

        if (phase + 1 > placedObjectsStates.Count)
        { //First phase is 0. Which means going to the first phase with the condition only being phase > ... would result in 1 > 1, 
            AddPhases(phase - currentPhase);       //which would mean that there won't be added a new phase. Therefore the +1 is necessary
        }
        Debug.Log("before" + currentPhase);
        currentPhase = phase;
        Debug.Log("lel"+currentPhase);
        foreach (KeyValuePair<int, PrefabContainer> keyValue in placedObjects)
        {
            PlacedObjectState placedObjectState;
            if (placedObjectsStates[currentPhase].TryGetValue(keyValue.Key, out placedObjectState) == false)
            {
                Debug.LogError("No State for " + keyValue.Key);
                //TODO: What to do if it happens?
            }
            else
            {
                Debug.Log("else" + currentPhase);
                GameObject gameObjectToChange = keyValue.Value.Prefab;
                if (placedObjectState.IsActive)
                {
                    gameObjectToChange.SetActive(true);
                    gameObjectToChange.transform.position = placedObjectState.Position;
                    gameObjectToChange.transform.localScale = placedObjectState.LocalScale;
                    gameObjectToChange.transform.rotation = placedObjectState.Rotation;
                }
                else
                {
                    gameObjectToChange.SetActive(false);
                    gameObjectToChange.transform.position = placedObjectState.Position;
                    gameObjectToChange.transform.localScale = placedObjectState.LocalScale;
                    gameObjectToChange.transform.rotation = placedObjectState.Rotation;
                }
            }

        }

        UndoRedoManager.Clear();
        return currentPhase;
    }

    public void AddPhases(int amountOfPhasesToAdd)
    {
        for (int i = 0; i < amountOfPhasesToAdd; i++)
        {
            Dictionary<int, PlacedObjectState> newPhase = new Dictionary<int, PlacedObjectState>();
        
            foreach (KeyValuePair<int, PlacedObjectState> keyValue in placedObjectsStates[currentPhase])
            {
                newPhase.Add(keyValue.Key, new PlacedObjectState(keyValue.Value));
            }
            placedObjectsStates.Add(newPhase);
        }
    }




    private void AddRigidBody(GameObject gameObject)
    {
        Rigidbody rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.freezeRotation = true;
    }

    private void AddObjectData(GameObject gameObject, PlacedObjectState objectState)
    {
        ObjectData objectData = gameObject.AddComponent(typeof(ObjectData)) as ObjectData;
        objectData.position = objectState.Position;
        objectData.rotation = objectState.Rotation;
        objectData.localScale = objectState.LocalScale;
        objectData.AllowMove = true;
        objectData.AllowTransform = true;
        objectData.AllowRotation = true;

    }

    /// <summary>
    /// This Method adds a new object to the list of objects, and generates state entries for all phases.
    /// Depending on the modification option, the objects IsActive state for a phase is set accordingly.
    /// </summary>
    private void CreateAddedObjectStates(int objectId, PrefabModificationOption modificationOption, PlacedObjectState objectState)
    {
        PlacedObjectState currentPhasePlacedObjectState = new PlacedObjectState(objectState);
        placedObjectsStates[currentPhase].Add(objectId, currentPhasePlacedObjectState);

        switch (modificationOption)
        {
            case PrefabModificationOption.COMPLETE:
                AddObjectStatesComplete(objectId, currentPhasePlacedObjectState);
                break;

            case PrefabModificationOption.CURRENT_AND_FOLLOWING:
                AddObjectsStatesCurrentAndFollowing(objectId, currentPhasePlacedObjectState);
                break;

            case PrefabModificationOption.ONLY_IN_CURRENT:
                AddObjectStatesOnlyInCurrent(objectId, currentPhasePlacedObjectState);
                break;
        }
    }

    private void AddObjectStatesComplete(int objectId, PlacedObjectState currentPhasePlacedObjectState)
    {
        for (int i = 0; i < placedObjectsStates.Count; i++)
        {
            if (i == currentPhase)
            {
                continue;
            }
            placedObjectsStates[i].Add(objectId, new PlacedObjectState(currentPhasePlacedObjectState));
        }
    }

    private void AddObjectsStatesCurrentAndFollowing(int objectId, PlacedObjectState currentPhasePlacedObjectState)
    {
        PlacedObjectState objectStateInPreviousPhases = new PlacedObjectState(currentPhasePlacedObjectState);
        objectStateInPreviousPhases.IsActive = false;

        for (int i = 0; i < placedObjectsStates.Count; i++)
        {
            if (i == currentPhase)
            {
                continue;
            }
            if (i < currentPhase)
            {
                placedObjectsStates[i].Add(objectId, new PlacedObjectState(objectStateInPreviousPhases));
            }
            if (i > currentPhase)
            {
                placedObjectsStates[i].Add(objectId, new PlacedObjectState(currentPhasePlacedObjectState));
            }
        }
    }

    private void AddObjectStatesOnlyInCurrent(int objectId, PlacedObjectState currentPhasePlacedObjectState)
    {
        PlacedObjectState objectStateInOtherPhases = new PlacedObjectState(currentPhasePlacedObjectState);
        objectStateInOtherPhases.IsActive = false;

        for (int i = 0; i < placedObjectsStates.Count; i++)
        {
            if (i == currentPhase)
            {
                continue;
            }
            placedObjectsStates[i].Add(objectId, new PlacedObjectState(objectStateInOtherPhases));
        }
    }

    /// <summary>Method<c>ReplaceObject</c> replaced one object with another. Currently only deletes the original 
    /// object in the current and following phases, and adds a new object in the same position.
    /// </summary>
    public void ReplaceObject(GameObject objectToReplace, int indexOfReplacementObject, PrefabType replacementType, PrefabModificationOption option)
    {
        List<PlacedObjectState> newObjectStates = new List<PlacedObjectState>();
        UndoRedoManager.Clear();

        switch (option)
        {
            case PrefabModificationOption.COMPLETE:
                newObjectStates = GetStatesForObjectCompleteReplacement(objectToReplace);
                DeleteObject(objectToReplace, PrefabModificationOption.COMPLETE);
                break;

            case PrefabModificationOption.ONLY_IN_CURRENT:
                newObjectStates = GetStatesForObjectOnlyInCurrenReplacement(objectToReplace);
                DeleteObject(objectToReplace, PrefabModificationOption.ONLY_IN_CURRENT);
                break;

            case PrefabModificationOption.CURRENT_AND_FOLLOWING:
                newObjectStates = GetStatesForObjectCurrentAndFollowingReplacement(objectToReplace);
                DeleteObject(objectToReplace, PrefabModificationOption.CURRENT_AND_FOLLOWING);
                break;
        }
        AddObjectWithExistingPhases(indexOfReplacementObject, replacementType, newObjectStates);
    }

    private List<PlacedObjectState> GetStatesForObjectCompleteReplacement(GameObject objectToReplace)
    {
        PlacedObjectState existingObjectState;
        List<PlacedObjectState> newObjectStates = new List<PlacedObjectState>();

        for (int i = 0; i < placedObjectsStates.Count; i++)
        {
            Dictionary<int, PlacedObjectState> placedObjectsStatesInPhase = placedObjectsStates[i];
            if (placedObjectsStatesInPhase.TryGetValue(objectToReplace.GetInstanceID(), out existingObjectState))
            {
                newObjectStates.Add(new PlacedObjectState(existingObjectState));
            }
            else
            {
                Debug.LogError("Missing Entry in state " + i + " for object with id " + objectToReplace.GetInstanceID());
            }
        }
        return newObjectStates;
    }

    private List<PlacedObjectState> GetStatesForObjectCurrentAndFollowingReplacement(GameObject objectToReplace)
    {
        PlacedObjectState existingObjectState;
        PlacedObjectState newObjectState;
        List<PlacedObjectState> newObjectStates = new List<PlacedObjectState>();

        for (int i = 0; i < placedObjectsStates.Count; i++)
        {
            if (placedObjectsStates[currentPhase].TryGetValue(objectToReplace.GetInstanceID(), out existingObjectState))
            {
                if (i < currentPhase)
                {
                    newObjectState = new PlacedObjectState(existingObjectState);
                    newObjectState.IsActive = false;
                    newObjectStates.Add(newObjectState);
                }
                if (i == currentPhase)
                {
                    newObjectStates.Add(new PlacedObjectState(existingObjectState));
                }
                if (i > currentPhase)
                {
                    newObjectStates.Add(new PlacedObjectState(existingObjectState));
                }

            }
            else
            {
                Debug.LogError("Missing Entry in state " + i + " for object with id " + objectToReplace.GetInstanceID());
            }
        }
        return newObjectStates;
    }

    private List<PlacedObjectState> GetStatesForObjectOnlyInCurrenReplacement(GameObject objectToReplace)
    {
        PlacedObjectState existingObjectState;
        PlacedObjectState newObjectState;
        List<PlacedObjectState> newObjectStates = new List<PlacedObjectState>();

        for (int i = 0; i < placedObjectsStates.Count; i++)
        {
            if (placedObjectsStates[currentPhase].TryGetValue(objectToReplace.GetInstanceID(), out existingObjectState))
            {
                if (i == currentPhase)
                {
                    newObjectStates.Add(new PlacedObjectState(existingObjectState));
                    existingObjectState.IsActive = false;
                }
                else
                {
                    newObjectState = new PlacedObjectState(existingObjectState);
                    newObjectState.IsActive = false;
                    newObjectStates.Add(newObjectState);
                }
            }
            else
            {
                Debug.LogError("Missing Entry in state " + i + " for object with id " + objectToReplace.GetInstanceID());
            }
        }
        return newObjectStates;
    }



    private void SendToMsgPrinter(string msg)
    {
        if (messagePrinter != null)
        {
            messagePrinter.PrintTextBoxMessage(msg);
        }
        else
        {
            Debug.LogError("ConstructionPhasesManager: Message Printer not assigned!");
            Debug.Log(msg);
        }
    }

    /// <summary>
    /// checks whether or not the object data can be requested from the database
    /// </summary>
    public bool isDatabaseRequestable(GameObject objectToCheck)
    {
        DataInterface.ConstructionVehicleNames name;
        String nameChanged = objectToCheck.name.Substring(0, objectToCheck.name.Length - 7);
        if (Enum.TryParse(nameChanged, out name))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// gives info about the database task for the given object
    /// </summary>
    public String giveTaskInfo(GameObject objectToGetInfo)
    {
        Task<ConstructionValues> task;
        databaseTasks.TryGetValue(objectToGetInfo.GetInstanceID(), out task);
        return task.Status.ToString();
    }

    /// <summary>
    /// requests the data for the given object and saves the task for it in the dictionary 
    /// </summary>
    public void request(GameObject objectToRequest)
    {
        Task<ConstructionValues> task;
        DataInterface.ConstructionVehicleNames name;
        String nameChanged = objectToRequest.name.Substring(0, objectToRequest.name.Length - 7);
        if (Enum.TryParse(nameChanged, out name))
        {
            task = DataInterface.RequestConstructionVehicleAsync(name);
            databaseTasks.Add(objectToRequest.GetInstanceID(), task);
        }
    }

    /// <summary>
    /// checks whether the database task for the given object is completed and if it is it transfers the values to the dictionary
    /// </summary>
    public bool checkTask(GameObject objectToCheck)
    {
        Task<ConstructionValues> task;
        databaseTasks.TryGetValue(objectToCheck.GetInstanceID(), out task);
        if (task.IsCompleted)
        {
            if (!databaseValues.ContainsKey(objectToCheck.GetInstanceID()))
            {
                databaseValues.Add(objectToCheck.GetInstanceID(), task.Result);
            }
            return true;
        }
        return false;
    }

    ///////////////////////// drawing methods ONLY USE WHEN DATA IS AVAILABLE /////////////////////////

    /// <summary>
    /// draws or deletes a circle around an object with the associated value for the arbeitsflaeche
    /// </summary>
    public bool drawCircleWithValuesForArbeitsflaeche(GameObject objectToDrawCircleAround)
    {
        ConstructionValues values;
        databaseValues.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out values);
        double radius = values.arbeitsflaeche.Radius;
        double breite = values.arbeitsflaeche.Breite;
        double laenge = values.arbeitsflaeche.Laenge;
        WhatIsDrawn drawn;
        if (radius != 0)
        {
            if (!(whatsDrawn.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out drawn)))
            {
                objectToDrawCircleAround.DrawCircle((float)radius, 0.2f, Color.red);
                whatsDrawn.Add(objectToDrawCircleAround.GetInstanceID(), WhatIsDrawn.Arbeitsflaeche);
                return true;
            }
            else if (drawn != WhatIsDrawn.Arbeitsflaeche)
            {
                objectToDrawCircleAround.DrawCircle((float)radius, 0.2f, Color.red);
                whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Arbeitsflaeche;
                return true;
            }
            objectToDrawCircleAround.deleteLines();
            whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Nothing;
            return false;
        }
        else
        {
            if (!(whatsDrawn.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out drawn)))
            {
                objectToDrawCircleAround.DrawRectangle((float)breite, (float)laenge, 0.2f, Color.red);
                whatsDrawn.Add(objectToDrawCircleAround.GetInstanceID(), WhatIsDrawn.Arbeitsflaeche);
                return true;
            }
            else if (drawn != WhatIsDrawn.Arbeitsflaeche)
            {
                objectToDrawCircleAround.DrawRectangle((float)breite, (float)laenge, 0.2f, Color.red);
                whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Arbeitsflaeche;
                return true;
            }
            objectToDrawCircleAround.deleteLines();
            whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Nothing;
            return false;
        }
    }

    /// <summary>
    /// draws or deletes a circle around an object with the associated value for the standflaeche
    /// </summary>
    public bool drawCircleWithValuesForStandflaeche(GameObject objectToDrawCircleAround)
    {
        ConstructionValues values;
        databaseValues.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out values);
        double radius = values.standflaeche.Radius;
        double breite = values.standflaeche.Breite;
        double laenge = values.standflaeche.Laenge;
        if (radius != 0)
        {
            WhatIsDrawn drawn;
            if (!(whatsDrawn.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out drawn)))
            {
                objectToDrawCircleAround.DrawCircle((float)radius, 0.2f, Color.green);
                whatsDrawn.Add(objectToDrawCircleAround.GetInstanceID(), WhatIsDrawn.Standflaeche);
                return true;
            }
            else if (drawn != WhatIsDrawn.Standflaeche)
            {
                objectToDrawCircleAround.DrawCircle((float)radius, 0.2f, Color.green);
                whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Standflaeche;
                return true;
            }
            objectToDrawCircleAround.deleteLines();
            whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Nothing;
            return false;
        }
        else
        {
            WhatIsDrawn drawn;
            if (!(whatsDrawn.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out drawn)))
            {
                objectToDrawCircleAround.DrawRectangle((float)breite, (float)laenge, 0.2f, Color.green);
                whatsDrawn.Add(objectToDrawCircleAround.GetInstanceID(), WhatIsDrawn.Standflaeche);
                return true;
            }
            else if (drawn != WhatIsDrawn.Standflaeche)
            {
                objectToDrawCircleAround.DrawRectangle((float)breite, (float)laenge, 0.2f, Color.green);
                whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Standflaeche;
                return true;
            }
            objectToDrawCircleAround.deleteLines();
            whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Nothing;
            return false;
        }
    }

    /// <summary>
    /// draws or deletes a circle around an object with the associated value for the arbeitsflaeche
    /// </summary>
    public bool drawCircleWithValuesForFahrweg(GameObject objectToDrawCircleAround)
    {
        ConstructionValues values;
        databaseValues.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out values);
        double fahrweg = values.fahrweg.Breite;
        objectToDrawCircleAround.DrawLine((float)fahrweg, 0.2f, Color.blue);
        WhatIsDrawn drawn;
        if (!(whatsDrawn.TryGetValue(objectToDrawCircleAround.GetInstanceID(), out drawn)))
        {
            objectToDrawCircleAround.DrawLine((float)fahrweg, 0.2f, Color.blue);
            whatsDrawn.Add(objectToDrawCircleAround.GetInstanceID(), WhatIsDrawn.Fahrweg);
            return true;
        }
        else if (drawn != WhatIsDrawn.Fahrweg)
        {
            objectToDrawCircleAround.DrawLine((float)fahrweg, 0.2f, Color.blue);
            whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Fahrweg;
            return true;
        }
        objectToDrawCircleAround.deleteLines();
        whatsDrawn[objectToDrawCircleAround.GetInstanceID()] = WhatIsDrawn.Nothing;
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// removes the task or values and DrawStates for given object
    /// </summary>
    public void removeObjectTaskOrValues(GameObject objectToDeleteValues)
    {
        if (databaseTasks.ContainsKey(objectToDeleteValues.GetInstanceID()))
        {
            databaseTasks.Remove(objectToDeleteValues.GetInstanceID());
        }
        if (databaseValues.ContainsKey(objectToDeleteValues.GetInstanceID()))
        {
            databaseValues.Remove(objectToDeleteValues.GetInstanceID());
        }
        if (whatsDrawn.ContainsKey(objectToDeleteValues.GetInstanceID()))
        {
            whatsDrawn.Remove(objectToDeleteValues.GetInstanceID());
        }
    }


    /// <summary>
    /// used for showing what is drawn around a gameobject
    /// TODO: move to placedObjectState
    /// </summary>
    public enum WhatIsDrawn
    {
        Arbeitsflaeche,
        Standflaeche,
        Fahrweg,
        Nothing
    }
}
