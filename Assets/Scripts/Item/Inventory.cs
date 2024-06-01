using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Lijst van Consumable items
    public List<Consumable> items = new List<Consumable>();

    // Maximale aantal items dat het inventory kan bevatten
    public int maxItems;

    // Functie om een item toe te voegen aan het inventory
    public bool AddItem(Consumable item)
    {
        if (items.Count < maxItems)
        {
            items.Add(item);
            Debug.Log("Item added to inventory: " + item.name);
            return true;
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add item: " + item.name);
            return false;
        }
    }

    // Functie om een item uit het inventory te verwijderen
    public void DropItem(Consumable item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Item dropped from inventory: " + item.name);
        }
        else
        {
            Debug.LogWarning("Item not found in the inventory.");
        }
    }
}
