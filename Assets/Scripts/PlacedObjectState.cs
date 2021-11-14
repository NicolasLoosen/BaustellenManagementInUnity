using System;
using UnityEngine;

namespace Assets.Scripts
{

    /// <summary>
    /// Stores the State vairables for placed objects
    /// </summary>
    public class PlacedObjectState
    {
        #region Constructors

        /// <summary>
        /// Creates a new PlacedObject with no values
        /// </summary>
        public PlacedObjectState()
        {
            this.IsActive = false;
            Position = new Vector3();
            LocalScale = new Vector3();
            Rotation = new Quaternion();
        }

        

        /// <summary>
        /// Creates a new PlacedObject with the passed parameters
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="localScale"></param>
        /// <param name="isActive"></param>
        public PlacedObjectState(Vector3 position, Quaternion rotation, Vector3 localScale, bool isActive = true)
        {
            this.IsActive = isActive;
            this.Position = position;
            this.Rotation = rotation;
            this.LocalScale = localScale;
        }


        /// <summary>
        /// Creates a new PlacedObjectState based on the values of the passed one
        /// </summary>
        /// <param name="placedObjectState"></param>
        public PlacedObjectState(PlacedObjectState placedObjectState)
        {
            IsActive = placedObjectState.IsActive;
            Position = placedObjectState.Position;
            LocalScale = placedObjectState.LocalScale;
            Rotation = placedObjectState.Rotation;
        }


        /// <summary>
        /// Creates a new PlacedObjectState based on the values of the passed GameObject
        /// TODO: is drawn boolean
        /// </summary>
        /// <param name="gameObject"></param>
        public PlacedObjectState(GameObject gameObject)
        {
            IsActive = gameObject.activeSelf;
            Position = gameObject.transform.position;
            LocalScale = gameObject.transform.localScale;
            Rotation = gameObject.transform.rotation;
        }


        #endregion


        #region Attributes

        /// <summary>
        /// Indicates if the object is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Stores the position of the object
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Stores the scaling of the object
        /// </summary>
        public Vector3 LocalScale { get; set; }

        /// <summary>
        /// Stores the rotation of the object
        /// </summary>
        public Quaternion Rotation { get; set; }


        #endregion


        /// <summary>
        /// Returns a new Serialized instance with current values or set the attributes to the values from passed.
        /// </summary>
        public SerializablePlacedObjectState Serialization
        {
            get
            {
                return new SerializablePlacedObjectState(this);
            }

            set
            {
                this.IsActive = value.IsActive;
                this.Position = new Vector3(value.Position[0], value.Position[1], value.Position[2]);
                this.LocalScale = new Vector3(value.LocalScale[0], value.LocalScale[1], value.LocalScale[2]);
                this.Rotation = new Quaternion(value.Rotation[0], value.Rotation[1], value.Rotation[2], value.Rotation[3]);
            }
        }
    }



    /// <summary>
    /// This contains the same attributes as PlacedObjectStates, but stores them in a serializable form.
    /// </summary>
    [Serializable]
    public class SerializablePlacedObjectState
    {
        /// <summary>
        /// Create a new empty object
        /// </summary>
        public SerializablePlacedObjectState() { }
        /// <summary>
        /// Creates a Serializable Object with the values of the passed PlacedObjectState
        /// </summary>
        /// <param name="placedObjectState"></param>
        public SerializablePlacedObjectState(PlacedObjectState placedObjectState)
        {
            this.IsActive = placedObjectState.IsActive;
            this.Position = new float[] { placedObjectState.Position.x, placedObjectState.Position.y, placedObjectState.Position.z };
            this.LocalScale = new float[] { placedObjectState.LocalScale.x, placedObjectState.LocalScale.y, placedObjectState.LocalScale.z };
            this.Rotation = new float[] { placedObjectState.Rotation.x, placedObjectState.Rotation.y, placedObjectState.Rotation.z, placedObjectState.Rotation.w };
        }

        /// <summary>
        /// Indicates if the object is active or not
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Stores the position of the object as float[]
        /// </summary>
        public float[] Position { get; set; }
        /// <summary>
        /// Stores the scale of the object as float[]
        /// </summary>
        public float[] LocalScale { get; set; }
        /// <summary>
        /// Stores the rotation of the object as float[]
        /// </summary>
        public float[] Rotation { get; set; }
    }
}
