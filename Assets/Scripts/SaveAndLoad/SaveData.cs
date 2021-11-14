using Assets.Scripts.PrefabClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SaveAndLoad
{
    /// <summary>
    /// Container to organize the data to de-/serialize a construction site
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>
        /// The List of PlacedObjects
        /// </summary>
        public List<PrefabContainer> PlacedObjects { get; set; }


        /// <summary>
        /// The List with the phases with PlacedObjectsStates in each phase.
        /// Uses the Serializable version of placedObjectState
        /// </summary>
        public List<List<SerializablePlacedObjectState>> PlacedObjectsStates { get; set; }

        /// <summary>
        /// Length of the ground from construction site
        /// </summary>
        public float Length { get; set; }


        /// <summary>
        /// Width of the ground from construction site
        /// </summary>
        public float Width { get; set; }
    }
}
