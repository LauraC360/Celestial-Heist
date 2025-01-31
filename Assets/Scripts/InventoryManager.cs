using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryManager", menuName = "Inventory/Inventory Manager")]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<ShopItem> ownedItems = new List<ShopItem>();
    private ShopItem equippedItem = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(ShopItem item)
    {
        if (!ownedItems.Contains(item))
            ownedItems.Add(item);
        
        Debug.Log("Owned items count: " + ownedItems.Count);
        
        // Current ownedItems
        foreach (var ownedItem in ownedItems)
        {
            Debug.Log("Owned items: " + ownedItem.Name);
        }
    }

    public List<ShopItem> GetOwnedItems()
    {
        Debug.Log("GetOwnedItems: " + ownedItems.Count);
        return ownedItems;
    }

    public void EquipItem(ShopItem item)
    {
        if (ownedItems.Contains(item))
        {
            equippedItem = item;
            Debug.Log($"Equipped {item.Name}");
        }
    }

    public void UnequipItem()
    {
        equippedItem = null;
    }

    public ShopItem GetEquippedItem()
    {
        return equippedItem;
    }
}