using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    public InventoryItemData itemData {  get; private set; }
    public int stackSize {  get; private set; }

    /// <summary>
    /// Fills the inventory item using the given scriptable object data.
    /// </summary>
    /// <param name="data"></param>
    public void InitItem(InventoryItemData data)
    {
        if (!data) return;

        this.itemData = data;
        this.icon.sprite = data.Icon;

        var stackLimit = this.itemData.StackLimit;

        if (stackLimit > 1 && this.stackSize < stackLimit)
            AddToStack();
    }
    //TODO: Stack objects
    public void AddToStack()
    {
        this.stackSize++;
    }

    public void RemoveFromStack()
    {
        this.stackSize--;
    }
}
