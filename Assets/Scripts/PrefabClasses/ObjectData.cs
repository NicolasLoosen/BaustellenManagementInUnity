using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour {
    public bool AllowMove = true;
    public bool AllowTransform = true;
    public bool AllowRotation = true;
    public Vector3 position = new Vector3(0,0,0);
    public Quaternion rotation = Quaternion.identity;
    public Vector3 localScale = new Vector3(1, 1, 1);
}