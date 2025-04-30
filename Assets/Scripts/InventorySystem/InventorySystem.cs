using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private Dictionary<InventoryItemData, InventoryItem> itemDictionary;
    public List<InventoryItem> inventory { get; private set; }

    private void Awake()
    {
        inventory = new List<InventoryItem>();
        itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public InventoryItem GetItem(InventoryItemData referenceData)
    {
        if(itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void AddItem(InventoryItemData referenceData)
    {
        if (itemDictionary.TryGetValue(referenceData, out InventoryItem value) && value.stackSize < referenceData.StackLimit)
        {
            value.AddToStack();
        }
        else
        {
            var newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            itemDictionary.Add(referenceData, newItem);
        }
    }
    public void RemoveItem(InventoryItemData referenceData)
    {
        if(itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();  

            if(value.stackSize == 0)
            {
                inventory.Remove(value);
                itemDictionary.Remove(referenceData);
            }
        }
    }
}
