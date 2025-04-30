using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public InventoryItemData itemData {  get; private set; }
    public int stackSize {  get; private set; }

    public InventoryItem(InventoryItemData data)
    {
        this.itemData = data;
        var stackLimit = itemData.StackLimit;

        if (stackLimit > 1 && stackSize < stackLimit)
            AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
