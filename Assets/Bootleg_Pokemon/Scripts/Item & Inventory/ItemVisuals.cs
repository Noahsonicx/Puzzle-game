using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVisuals : MonoBehaviour
{
    public string itemName;

    public ItemVisuals(string _name)
    {
        itemName = _name;
    }

    public string GetItemName()
    {
        return itemName;
    }
    
}
