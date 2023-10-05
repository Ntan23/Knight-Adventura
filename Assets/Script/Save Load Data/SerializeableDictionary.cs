using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<Key,Value> : Dictionary<Key,Value>,ISerializationCallbackReceiver
{

    [SerializeField] private List<Key> keys = new List<Key>();
    [SerializeField] private List<Value> values = new List<Value>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<Key,Value> pair in this) 
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load the dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count) 
        {
            Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                + keys.Count + ") does not match the number of values (" + values.Count 
                + ") which indicates that something went wrong");
        }

        for (int i = 0; i < keys.Count; i++) 
        {
            this.Add(keys[i], values[i]);
        }
    }

}
