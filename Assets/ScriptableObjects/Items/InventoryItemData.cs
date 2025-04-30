using UnityEngine;

public enum ItemType
{
    Resource,
    Consumable,
    Weapon,
    Default
}

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Inventory System/Items/InventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    [Header("General Info")]
    [SerializeField] private Sprite icon = null;
    [SerializeField] private string id = "NullItemID";
    [SerializeField] private string displayName = "Null Item";
    [SerializeField] private ItemType type = ItemType.Default;
    [SerializeField] private GameObject prefab = null;
    [SerializeField, TextArea(15,20)] private string description = "Item description.";

    [Header("Inventory")]
    [SerializeField, Min(1)] private int stackLimit = 1;

    // Getters
    public string ID => id;
    public string DisplayName => displayName;
    public ItemType Type => type;
    public int StackLimit => stackLimit;
}
