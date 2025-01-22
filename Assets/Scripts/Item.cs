using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    One,  
    Multiple  
}


public class Item : MonoBehaviour
{

    public string Name { get; private set; }
    public int Price { get; private set; }
    public string Description { get; private set; }
    public ItemType Type { get; private set; }

    public Item(string name, int price, string description, ItemType type)
    {
        Name = name;
        Price = price;
        Description = description;
        Type = type;
    }

    public override bool Equals(object obj)
    {
        if (obj is Item other)
        {
            return Name == other.Name && Price == other.Price && Description == other.Description && Type == other.Type;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() ^ Price.GetHashCode() ^ Description.GetHashCode() ^ Type.GetHashCode();
    }


}
