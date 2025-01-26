using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "InventoryItem_")]
public class InventoryItem : ScriptableObject
{
    public ShopItemCategory Category;
    public string Name;
    [TextArea(3, 5)] public string Description;

    public int Cost;
    public bool IsSinglePurchase;
    public int Quantity;
}
