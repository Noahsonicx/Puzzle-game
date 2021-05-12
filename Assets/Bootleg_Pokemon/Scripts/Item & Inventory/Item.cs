using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item 
{
    [SerializeField]
    protected string itemName;
    [SerializeField]
    protected string type;

    public string GetItemName()
    {
        return itemName;
    }

    public string GetItemType()
    {
        return type;
    }
    
}