using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<GameObject> item_list = new List<GameObject>();
    public Dictionary<string, GameObject> item_dictionary = new Dictionary<string, GameObject>();
    public bool load_status = false;

    private void Start()
    {
        foreach(GameObject obj in item_list)
        {
            item_dictionary.Add(obj.name, obj);
        }

        load_status = true;
    }

    public bool GetDictionaryReadyStatus()
    {
        return load_status;
    }
    public GameObject GetItem (string key)
    {
        if (!item_dictionary.ContainsKey(key)) return null;
        return item_dictionary[key];
    }
    public Dictionary<string, GameObject> GetItemDictionary()
    {
        return item_dictionary;
    }
}
