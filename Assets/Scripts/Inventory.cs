using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<Item, int> items;

    public Inventory()
    {
        items = new Dictionary<Item, int>();
    }

    public void AddItem(Item item, int quantity = 1)
    {
        if (item.Type == ItemType.One && items.ContainsKey(item))
        {
            return;
        }

        if (items.ContainsKey(item))
        {
            items[item] += quantity;
        }
        else
        {
            items[item] = quantity;
        }
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (!items.ContainsKey(item))
        {
            return false;
        }

        if (items[item] > quantity)
        {
            items[item] -= quantity; 
            return true;
        }
        else if (items[item] == quantity)
        {
            items.Remove(item); 
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasItem(Item item)
    {
        return items.ContainsKey(item);
    }

    public int GetQuantity(Item item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }

    public List<Item> GetItems()
    {
        return new List<Item>(items.Keys); 
    }
}
