using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfcQuantities : MonoBehaviour
{
    public List<string> quantities = new List<string>();
    public List<string> values = new List<string>();

    // Return corresponding nominalValue for property name
    public string Find(string name){
        for(int i = 0; i < quantities.Count; i++){
            if(quantities[i] == name){
                return values[i];
            }
        }
        return null;
    }
}
