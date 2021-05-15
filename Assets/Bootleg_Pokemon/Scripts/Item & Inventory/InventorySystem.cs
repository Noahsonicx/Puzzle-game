using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [System.Serializable]
    public class ItemElement
    {
        public Item item;
        public int quantity;
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
            Debug.Log("quantity before is: " + quantity);
            quantity += _q;
            Debug.Log("quantity after is: " + quantity);
        }
        public void UseItem()
        {
            quantity--;
        }
    }

    public ItemDetailGuide itemGuide;
    public bool itemGuideLoaded = false;


    // string is the type of item whether it be a consumable or key item and is to categorise the list of item element it is the key of
    private List<(string, List<ItemElement>)> inventory = new List<(string, List<ItemElement>)>();

    public List<ItemElement> inventoryInspector = new List<ItemElement>();

    public void ResetInventory()
    {
        inventory = new List<(string, List<ItemElement>)>();
    }

    public void DebugInventoryContent()
    {
        foreach(var itemType in inventory)
        {
            Debug.Log("DebugInventoryContent -Item type: " + itemType.Item1);
            foreach(var item in itemType.Item2)
            {
                Debug.Log("DebugInventoryContent -Item name: " + item.GetItem().GetItemName() + " Item Quantity: " + item.GetQuantity().ToString());

            }
        }
    }


    public List<(string, List<ItemElement>)>GetInventory()
    {
        return inventory;
    }
    public void PickupItem(GameObject _itemVisual)
    {
        Item item = itemGuide.GetItemDetail(_itemVisual.GetComponent<ItemVisuals>().GetItemName());

        AddToInventory(item);
    }
    
    public void UseItem(string name)
    {
        PlayerMovement player = this.gameObject.GetComponent<PlayerMovement>();
        int indexCategory = 0;
        int indexItem = 0;

        bool removeCategory = false;
        bool removeItem = false;
        foreach(var i in inventory)
        {
            
            foreach(var ie in i.Item2)
            {
                if(ie.GetItem().GetItemName().Contains(name))
                {
                    ie.UseItem();
                    switch(ie.GetItem().GetItemType())
                    {
                        case "Consumable":
                            Potion pot = (Potion)ie.GetItem();
                            switch(pot.GetStatAffected())
                            {
                                case "Health":
                                    player.HealDamage(pot.GetValueAffected());
                                    Debug.Log("Player Health being healed for: " + pot.GetValueAffected());
                                    break;
                                case "Energy":
                                    player.HealEnergy(pot.GetValueAffected());
                                    Debug.Log("Player Energy being healed for: " + pot.GetValueAffected());
                                    break;
                                default:
                                    Debug.LogError("Stat Affected not listed");
                                    break;
                            }
                            break;
                        default:
                            Debug.LogError("Item Type not listed");
                            break;
                    }
                    if(ie.GetQuantity() <= 0)
                    {
                        if(i.Item2.Count == 1)
                        {
                            removeCategory = true;
                            break;

                        }
                        else
                        {
                            removeItem = true;
                            break;
                        }
                    }
                }
                if(removeItem || removeCategory)
                {
                    break;
                }
                indexItem++;
            }
            if(removeItem || removeCategory)
            {
                break;
            }
            indexCategory++;
        }

        if(removeCategory)
        {
            inventory.RemoveAt(indexCategory);
        }
        else if (removeItem)
        {
            inventory[indexCategory].Item2.RemoveAt(indexItem);
        }
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
            for(int i = 0; i < it.Item2; i++)
            {
                AddToInventory(tmp);
            }
            //AddMultipleToInventory(tmp, it.Item2);
        }
    }

    public void AddToInventory(Item item)
    {
        bool itemtype_exist = false;

        foreach ((string, List<ItemElement>) t in inventory)
        {
            //Debug.Log("DebugInventory - Item type in inventory is: " + t.Item1);
            //Debug.Log("DebugInventory - Item type passed in is: " + item.GetItemType());
            if (t.Item1 == item.GetItemType())
            {
                //Debug.Log("DebugInventory - Found same type");
                itemtype_exist = true;
                bool item_exist = false;
                foreach (ItemElement z in t.Item2)
                {

                    //Debug.Log("DebugInventory - item name: " + z.GetItem().GetItemName());
                    //Debug.Log("DebugInventory - item name passed in: " + item.GetItemName());
                    if (z.GetItem().GetItemName() == item.GetItemName())
                    {
                        //Debug.Log("DebugInventory - Found same item");
                        z.AddItemQuantity(1);
                        //Debug.Log("DebugInventory - in Z, Item name: " + z.GetItem().GetItemName() + " Quantity: " + z.GetQuantity());
                        //DebugInventoryContent();
                        Debug.Log(z.GetQuantity());
                        item_exist = true;
                        foreach(var inventoryInspectorelem in inventoryInspector)
                        {
                            if (inventoryInspectorelem.GetItem().GetItemName() == z.GetItem().GetItemName())
                            {
                                inventoryInspectorelem.AddItemQuantity(1);
                                Debug.Log(inventoryInspectorelem.GetQuantity());
/*
 * #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPaused = true;
#endif
*/
                            }
                                    
                        }
                        return;
                    }
                }

                if (!item_exist)
                {
                    //Debug.Log("DebugInventory - Item picked up does not exist");
                    t.Item2.Add(new ItemElement(item));
                    inventoryInspector.Add(new ItemElement(item, 1));
                    return;
                }
            }
        }

        if (!itemtype_exist)
        {
            List<ItemElement> tmp = new List<ItemElement>();
            tmp.Add(new ItemElement(item));
            inventory.Add((item.GetItemType(), tmp));
            inventoryInspector.Add(new ItemElement(item, 1));
        }

       
    }
}
