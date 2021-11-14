using Assets.Scripts.PrefabClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SaveAndLoad
{

    /// <summary>
    /// Helper class to convert non-serializable objects lists to the serialized ones
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Converts the non-serializable placedObjectsStates-list to a serializable one
        /// </summary>
        /// <param name="placedObjectStates"></param>
        /// <returns>A List with Lists of SerializablePlacedObjectState</returns>
        public static List<List<SerializablePlacedObjectState>> SerializePlacedObjectsStates(List<Dictionary<int, PlacedObjectState>> placedObjectStates)
        {
            //Copy non serializable list to serializable
            List<List<SerializablePlacedObjectState>> states = new List<List<SerializablePlacedObjectState>>();
            foreach (Dictionary<int, PlacedObjectState> dic in placedObjectStates)
            {
                List<SerializablePlacedObjectState> newList = new List<SerializablePlacedObjectState>();
                states.Add(newList);
                foreach (KeyValuePair<int, PlacedObjectState> entry in dic)
                {
                    newList.Add(entry.Value.Serialization);
                }
            }
            return states;

        }


        /// <summary>
        /// Converts the non-serializable placedObjects-list (Dictionary) to a serializable one
        /// </summary>
        /// <param name="placedObjects"></param>
        /// <returns>A List of PrefabContainers</returns>
        public static List<PrefabContainer> SerializePlacedObjects(Dictionary<int, PrefabContainer> placedObjects)
        {
            List<PrefabContainer> newObjects = new List<PrefabContainer>();
            foreach (KeyValuePair<int, PrefabContainer> oldObject in placedObjects)
            {
                newObjects.Add(oldObject.Value);
            }
            return newObjects;
        }
    }
}
