using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.PrefabClasses;

public class PrefabRegistry : MonoBehaviour
{
    public List<PrefabContainer> buildingsPrefabs;
    public List<PrefabContainer> vehiclesPrefabs;
    public List<PrefabContainer> miscPrefabs;
    public List<PrefabContainer> importedModels;


    /// <summary>
    /// Get a single PrefabContainer based on type and the index of the item in the list.
    /// </summary>
    /// <param name="index">Index of item in the list</param>
    /// <param name="type">Type (decides in which list will be looked)</param>
    /// <returns>The item at given index from the type-matching list</returns>
    public PrefabContainer GetPrefab(int index, PrefabType type)
    {
        PrefabContainer prefab;

        switch (type)
        {
            case PrefabType.BUILDING:
                prefab = buildingsPrefabs[index];
                break;
            case PrefabType.VEHICLE:
                prefab = vehiclesPrefabs[index];
                break;
            case PrefabType.MISC:
                prefab = miscPrefabs[index];
                break;
            case PrefabType.IMPORTED_MODEL:
                prefab = importedModels[index];
                break;
            default:
                prefab = new PrefabContainer();
                Debug.Log("Error, prefab not found: type: " + type.ToString() + "index: " + index);
                break;
        }
        return prefab;
    }

    /// <summary>
    /// Get a single PrefabContainer based on type and the id of the item in the list.
    /// </summary>
    /// <param name="id">Id of the wanted PrefabContainer</param>
    /// <param name="type">Type (decides in which list will be looked)</param>
    /// <returns>If found: The found item; else: an empty PrefabContainer</returns>
    public PrefabContainer GetPrefab(string id, PrefabType type)
    {
        PrefabContainer result;

        switch (type)
        {
            case PrefabType.BUILDING:
                result = buildingsPrefabs.Find(prefab => prefab.Id.Equals(id));
                break;
            case PrefabType.VEHICLE:
                result = vehiclesPrefabs.Find(prefab => prefab.Id.Equals(id));
                break;
            case PrefabType.MISC:
                result = miscPrefabs.Find(prefab => prefab.Id.Equals(id));
                break;
            case PrefabType.IMPORTED_MODEL:
                result = importedModels.Find(prefab => prefab.Id.Equals(id));
                break;
            default:
                result = new PrefabContainer();
                Debug.Log("Error, prefab not found: type: " + type.ToString() + " id: " + id);
                break;
        }
        return result;
    }

    /// <summary>
    /// Returns a list of PrefabContainers that matches the PrefabType
    /// </summary>
    /// <param name="type">The wanted type</param>
    /// <returns>List with PrefabContainers of the type</returns>
    public IList<PrefabContainer> GetPrefabs(PrefabType type)
    {
        switch (type)
        {
            case PrefabType.BUILDING:
                return buildingsPrefabs;
            case PrefabType.VEHICLE:
                return vehiclesPrefabs;
            case PrefabType.MISC:
                return miscPrefabs;
            case PrefabType.IMPORTED_MODEL:
                return importedModels;
            default:
                Debug.Log("Error, prefab not found: type: " + type.ToString());
                return new List<PrefabContainer>();
        }
    }

    /// <summary>
    /// Returns the last item from the imported prefabs
    /// </summary>
    /// <returns>Last imported prefab</returns>
    public PrefabContainer GetLastImportedFbx()
    {
        return importedModels[importedModels.Count - 1];
    }

    /// <summary>
    /// Adds the passed list of PrefabContainers to the registry.
    /// </summary>
    /// <param name="prefabs">The list with PrefabContainers</param>
    public void AddPrefabs(IEnumerable<PrefabContainer> prefabs)
    {
        foreach (PrefabContainer prefab in prefabs)
        {
            this.AddPrefab(prefab);
        }
    }


    /// <summary>
    /// Adds the passed PrefabContainer to the registry based on the type
    /// </summary>
    /// <param name="prefab">The prefab Container</param>
    public void AddPrefab(PrefabContainer prefab)
    {
        switch (prefab.PrefabType)
        {
            case PrefabType.BUILDING:
                buildingsPrefabs.Add(prefab);
                break;
            case PrefabType.VEHICLE:
                vehiclesPrefabs.Add(prefab);
                break;
            case PrefabType.MISC:
                miscPrefabs.Add(prefab);
                break;
            case PrefabType.IMPORTED_MODEL:
                importedModels.Add(prefab);
                break;
        }
    }


}
