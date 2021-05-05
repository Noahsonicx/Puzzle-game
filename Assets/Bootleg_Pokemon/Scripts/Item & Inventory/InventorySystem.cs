using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public struct ItemElement
    {
        GameObject item;
        int quantity;
        public ItemElement(GameObject _item)
        {
            item = _item;
            quantity = 1;
        }

        public GameObject GetItem()
        {
            return item;
        }
        public int GetQuantity()
        {
            return quantity;
        }
        public void AddItemQuantity(int _q) 
        {
            quantity += _q;
        }
    }
    private List<(string, List<ItemElement>)> Inventory = new List<(string, List<ItemElement>)>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickupItem(GameObject item)
    {
        bool itemtype_exist = false;
        foreach((string, List<ItemElement>) t in Inventory)
        {
            if (t.Item1.Equals(item.GetComponent<Item>().GetType()))
            {
                itemtype_exist = true;
                bool item_exist = false;
                foreach(ItemElement z in t.Item2)
                {
                    if(z.GetItem().GetComponent<Item>().GetItemName() == item.GetComponent<Item>().GetItemName())
                    {
                        z.AddItemQuantity(1);
                    }
                }

                if(!item_exist)
                {
                    t.Item2.Add(new ItemElement(item));
                }
            }
        }

        if(!itemtype_exist)
        {
            List<ItemElement> tmp = new List<ItemElement>();
            tmp.Add(new ItemElement(item));
            Inventory.Add((item.GetComponent<Item>().GetItemType(), tmp));
        }
    }
}
