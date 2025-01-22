using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Coins { get; private set; }
    public Inventory Inventory { get; private set; }

    private void Awake()
    {
        Inventory = new Inventory();
        Coins = 100; 
    }

    public bool CanAfford(int cost)
    {
        return Coins >= cost;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (CanAfford(amount))
        {
            Coins -= amount;
            return true;
        }
        return false;
    }
}