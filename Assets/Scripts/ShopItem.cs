using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Shop/Item", fileName = "ShopItem_")]
public class ShopItem : ScriptableObject
{
    public ShopItemCategory Category;
    public string Name;
    [TextArea(3, 5)] public string Description;

    public int ID;
    public int Quantity;
    public bool Equipped;
    public int Cost;
    public bool IsSinglePurchase;
}