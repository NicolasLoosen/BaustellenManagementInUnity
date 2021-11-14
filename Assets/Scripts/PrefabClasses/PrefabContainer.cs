using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrefabClasses
{
    /// <summary>
    /// This is a container to store a Prefab (of the type GameObject) an attach it with additional attributes. 
    /// </summary>
    [Serializable]
    public class PrefabContainer
    {

        #region Constructor

        /// <summary>
        /// Create an empty container
        /// </summary>
        public PrefabContainer()
        {

        }

        /// <summary>
        /// Create a container with the passed values
        /// </summary>
        /// <param name="id">ID of the container</param>
        /// <param name="type">Type of the container (needed for processing in code)</param>
        /// <param name="prefab">The prefab to store</param>
        public PrefabContainer(string id, PrefabType type, GameObject prefab)
        {
            this.Id = id;
            this.PrefabType = type;
            this.Prefab = prefab;
        }

        #endregion


        #region Non Serializable Attributes

        [NonSerialized]
        private GameObject prefab;
        /// <summary>
        /// The GameObject instance of the prefab.
        /// </summary>
        public GameObject Prefab
        {
            get { return this.prefab; }
            set { this.prefab = value; }
        }


        //Further attributes. Tag them also as [NonSerialized] e.g. Thumbnail



        #endregion


        #region Serializable Attributes


        private PrefabType prefabType;
        /// <summary>
        /// Type of the contained prefab
        /// </summary>
        public PrefabType PrefabType
        {
            get { return this.prefabType; }
            set { this.prefabType = value; }
        }

        private string id;
        /// <summary>
        /// Identifier for the prefab. Important for Save and Load referencing.
        /// </summary>
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        #endregion

    }
}
