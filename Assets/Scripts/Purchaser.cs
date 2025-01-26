using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPurchaser
{
    float GetCurrentFunds();
    bool SpendFunds(int amount);
    bool HasPurchasedItem(ShopItem item); 
    void AddPurchasedItem(ShopItem item); 
    int GetPurchaseCount(ShopItem item);
    bool EquipItem(ShopItem item);
    Dictionary<ShopItem, int> GetPurchasedItems();
}


public class Purchaser : MonoBehaviour, IPurchaser
{
    [SerializeField] int CurrentFunds;

    private Dictionary<ShopItem, int> PurchasedItems = new Dictionary<ShopItem, int>();
    private ShopItem EquippedItem;

    public float GetCurrentFunds()
    {
        return CurrentFunds;
    }

    public bool SpendFunds(int amount)
    {
        if (CurrentFunds >= amount)
        {
            CurrentFunds -= amount;
            return true;
        }

        return false;
    }

    public bool HasPurchasedItem(ShopItem item)
    {
        return PurchasedItems.ContainsKey(item);
    }

    public void AddPurchasedItem(ShopItem item)
    {
        if (PurchasedItems.ContainsKey(item))
        {
            PurchasedItems[item]++;
        }
        else
        {
            PurchasedItems[item] = 1;
        }
    }

    public int GetPurchaseCount(ShopItem item)
    {
        if (PurchasedItems.ContainsKey(item))
        {
            return PurchasedItems[item];
        }
        return 0;
    }

    public bool EquipItem(ShopItem item)
    {
        if (PurchasedItems.ContainsKey(item))
        {
            PurchasedItems[item]--;
            
            if (PurchasedItems[item] <= 0)
            {
                PurchasedItems.Remove(item);
            }
            
            return true;
        }
        return false;
    }
    
    public Dictionary<ShopItem, int> GetPurchasedItems()
    {
        return new Dictionary<ShopItem, int>(PurchasedItems);
    }
}
