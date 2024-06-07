using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Consumable> Items = new List<Consumable>();
    public int MaxItems = 8;

    public bool AddItem(Consumable item)
    {
        if (Items.Count < MaxItems)
        {
            Items.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropItem(Consumable item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
        }
    }
}