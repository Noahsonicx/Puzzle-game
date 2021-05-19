using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVisuals : MonoBehaviour
{
    public string itemName;
    public float x;
    public float y;
    public ItemVisuals(string _name)
    {
        itemName = _name;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public void SetLocation(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
    public (float, float) GetLocation()
    {
        return (x, y);
    }
    
}
