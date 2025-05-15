using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventoryItemPrefab;

    private List<InventorySlot> slots;
    public List<InventorySlot> Slots => slots;

    private void Awake()
    {
        slots = new List<InventorySlot>();
        GetSlots();
    }

    /// <summary>
    /// Adds the given reference item to first free inventory slot if there are any.
    /// </summary>
    /// <param name="refItem"></param>
    public void AddItem(InventoryItemData refItem)
    {
        if (!refItem) return;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null)
            {
                AddNewItem(refItem, slot);
                return;
            }
        }
    }

    /// <summary>
    /// Creates a new instance of a inventory item given the reference data and free slot.
    /// </summary>
    /// <param name="refItem"></param>
    /// <param name="slot"></param>
    private void AddNewItem(InventoryItemData refItem, InventorySlot slot)
    {
        if (!inventoryItemPrefab) return;

        var newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        var inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitItem(refItem);
    }

    public void AddNewSlot()
    {
        if (!slotPrefab) return;

        var inst = Instantiate(slotPrefab);
        inst.transform.SetParent(transform, false);

        var slot = inst.GetComponent<InventorySlot>();
        slots.Add(slot);
    }

    /// <summary>
    /// Gets all the slots in the inventory and adds it to the inventory slots list variable.
    /// </summary>
    private void GetSlots()
    {
        foreach(Transform child in transform)
        {
            if (!child.GetComponent<InventorySlot>()) continue;
            slots.Add(child.GetComponent<InventorySlot>());
        }
    }
}
