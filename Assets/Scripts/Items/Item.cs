using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Inventory Item Settings")]
    [SerializeField] private InventoryItemData inventoryItemData;
    public InventoryItemData InventoryItemData => inventoryItemData;

    public void OnHandlePickUpItem()
    {
        Destroy(gameObject);
    }
}
