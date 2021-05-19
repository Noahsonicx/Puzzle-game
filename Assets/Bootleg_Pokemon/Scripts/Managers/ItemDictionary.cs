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
            Debug.Log("Adding: " + obj.name);
            item_dictionary.Add(obj.name, obj);
            if (item_dictionary.ContainsKey(obj.name)) Debug.Log("Successfully added: " + obj.name);
        }

        load_status = true;
    }

    public bool GetDictionaryReadyStatus()
    {
        return load_status;
    }
    public GameObject GetItem (string key)
    {
        Debug.Log("No of item in Itemdictionary: " + item_dictionary.Count);
        Debug.Log("Item key is: "+ "'" + key + "'");

        if (!item_dictionary.ContainsKey(key)) return null;

        return item_dictionary[key];
    }
    public Dictionary<string, GameObject> GetItemDictionary()
    {
        return item_dictionary;
    }
}
