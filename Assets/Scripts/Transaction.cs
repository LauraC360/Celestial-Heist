using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transaction : MonoBehaviour
{
    public Player Player;

    public bool BuyItem(Item item, int quantity = 1)
    {
        if (Player == null)
        {
            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        if (item.Type == ItemType.One && Player.Inventory.HasItem(item))
        {
            return false;
        }

        int totalPrice = item.Price * quantity;

        if (Player.CanAfford(totalPrice))
        {
            Player.SpendCoins(totalPrice);
            Player.Inventory.AddItem(item, quantity);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SellItem(Item item, int quantity = 1)
    {
        if (Player == null)
        {
            return false;
        }

        if (!Player.Inventory.HasItem(item) || Player.Inventory.GetQuantity(item) < quantity)
        {
            return false;
        }

        Player.Inventory.RemoveItem(item, quantity);
        Player.AddCoins(item.Price * quantity);
        
        return true;
    }
}
