using UnityEngine;

public class TestInventory : MonoBehaviour
{
    [SerializeField] private InventoryItemData[] items;
    [SerializeField] private InventoryManager inventoryManager;

    public void AddRandomItem()
    {
        inventoryManager.AddItem(items[Random.Range(0, items.Length)]);
    }

    public void AddItem(InventoryItemData itemData)
    {
        inventoryManager.AddItem(itemData);
    }

    public void AddNewSlot()
    {
        inventoryManager.AddNewSlot();
    }
}
