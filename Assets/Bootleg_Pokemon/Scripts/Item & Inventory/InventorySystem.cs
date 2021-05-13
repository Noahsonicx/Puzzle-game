using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public struct ItemElement
    {
        Item item;
        int quantity;
        public ItemElement(Item _item)
        {
            item = _item;
            quantity = 1;
        }
        public ItemElement(Item _item, int _q)
        {
            item = _item;
            quantity = _q;
        }
        public Item GetItem()
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

    public ItemDetailGuide itemGuide;
    public bool itemGuideLoaded = false;


    // string is the type of item whether it be a consumable or key item and is to categorise the list of item element it is the key of
    private List<(string, List<ItemElement>)> Inventory = new List<(string, List<ItemElement>)>();

    public List<Item> inventoryInspector = new List<Item>();

    public void DebugInventoryContent()
    {
        foreach(var itemType in Inventory)
        {
            Debug.Log("Itemtype: " + itemType.Item1);
            foreach(var item in itemType.Item2)
            {
                Debug.Log("Item name: " + item.GetItem().GetItemName() + " Item Quantity: " + item.GetQuantity().ToString());

            }
        }
    }


    public List<(string, List<ItemElement>)>GetInventory()
    {
        return Inventory;
    }
    public void PickupItem(GameObject _itemVisual)
    {
        Item item = itemGuide.GetItemDetail(_itemVisual.GetComponent<ItemVisuals>().GetItemName());

        AddToInventory(item);
    }
    
    public void UseItem(string name)
    {

    } 

    public void PrepareInventory()
    {
        itemGuide = FindObjectOfType<ItemDetailGuide>();
        itemGuideLoaded = true;
    }
    public void LoadInventory(List<(string, int)> _inventoryRecord)
    {
        foreach((string,int) it in _inventoryRecord) {
            Item tmp = itemGuide.GetItemDetail(it.Item1);
            AddMultipleToInventory(tmp, it.Item2);
        }
    }

    public void AddToInventory(Item item)
    {
        bool itemtype_exist = false;

        foreach ((string, List<ItemElement>) t in Inventory)
        {
            Debug.Log("Item type in inventory is: " + t.Item1);
            Debug.Log("Item type passed in is: " + item.GetItemType());
            if (t.Item1 == item.GetItemType())
            {
                Debug.Log("Found same type");
                itemtype_exist = true;
                bool item_exist = false;
                foreach (ItemElement z in t.Item2)
                {

                    Debug.Log("item name: " + z.GetItem().GetItemName());
                    Debug.Log("item name passed in: " + item.GetItemName());
                    if (z.GetItem().GetItemName() == item.GetItemName())
                    {
                        Debug.Log("Found same item");
                        z.AddItemQuantity(1);
                    }
                }

                if (!item_exist)
                {
                    Debug.Log("Item picked up does not exist");
                    t.Item2.Add(new ItemElement(item));
                }
            }
        }

        if (!itemtype_exist)
        {
            List<ItemElement> tmp = new List<ItemElement>();
            tmp.Add(new ItemElement(item));
            Inventory.Add((item.GetItemType(), tmp));
        }

        inventoryInspector.Add(item);
    }
    public void AddMultipleToInventory(Item item, int _quantity)
    {
        bool itemtype_exist = false;
        foreach ((string, List<ItemElement>) t in Inventory)
        {
            if (t.Item1.Equals(item.GetType()))
            {
                itemtype_exist = true;
                bool item_exist = false;
                foreach (ItemElement z in t.Item2)
                {
                    if (z.GetItem().GetItemName() == item.GetItemName())
                    {
                        z.AddItemQuantity(_quantity);
                    }
                }

                if (!item_exist)
                {
                    t.Item2.Add(new ItemElement(item, _quantity));
                }
            }
        }

        if (!itemtype_exist)
        {
            List<ItemElement> tmp = new List<ItemElement>();
            tmp.Add(new ItemElement(item));

            Inventory.Add((item.GetItemType(), tmp));
        }

        inventoryInspector.Add(item);
    }
}
