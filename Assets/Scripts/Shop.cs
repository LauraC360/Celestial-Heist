using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{   
    //TODO UI for shop 
    public Transaction Transaction; 
    public List<Item> ShopItems = new List<Item>();

    public void DisplayShopItems()
    {
        //TODO for UI
    }
    
    public bool BuyItem(string itemName, int quantity = 1)
    {
        Item itemToBuy = ShopItems.Find(item => item.Name == itemName);

        if (itemToBuy != null)
        {
            return Transaction.BuyItem(itemToBuy, quantity);
        }
        else
        {
            return false;
        }
    }
}
