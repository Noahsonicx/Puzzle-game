using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetailGuide : MonoBehaviour
{

    public List<Item> itemGuideList = new List<Item>();

    public Dictionary<string,Item> itemGuideDictionary = new System.Collections.Generic.Dictionary<string,Item>();
    // Start is called before the first frame update
    void Start()
    {
        itemGuideList.Add(new HealthPotion());
        itemGuideList.Add(new EnergyPotion());

        foreach (Item t in itemGuideList)
        {
            itemGuideDictionary.Add(t.GetItemName(), t);
        }
    }

    public Item GetItemDetail(string _itemName)
    {
        return itemGuideDictionary [_itemName];
    }

    
}
