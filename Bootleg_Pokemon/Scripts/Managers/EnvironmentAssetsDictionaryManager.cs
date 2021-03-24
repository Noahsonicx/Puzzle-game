using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAssetsDictionaryManager : MonoBehaviour
{

    public List<GameObject> asset_list = new List<GameObject>();

    public Dictionary<string ,GameObject> asset_dictionary= new Dictionary<string, GameObject>();

    public bool load_status = false;


    void Start()
    {
        foreach(GameObject obj in asset_list)
        {
            Debug.Log("Adding: " + obj.name);
            asset_dictionary.Add(obj.name, obj);
            if (asset_dictionary.ContainsKey("Wall")) Debug.Log("Wall in Dictionary");
        }

        load_status = true;
    }
    
    public bool GetDictionaryReadyStatus()
    {
        return load_status;
    }
    public GameObject GetAsset (string key)
    {
        if (!asset_dictionary.ContainsKey(key)) return null;
        return asset_dictionary[key];
    }

    public Dictionary<string, GameObject> GetAssetDictionary()
    {
        return asset_dictionary;
    }
}
