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
    [SerializeField, Min(1)] private int stackLimit = 1;
    [SerializeField, TextArea(15,20)] private string description = "Item description.";

    // Getters
    public Sprite Icon => icon;
    public string ID => id;
    public string DisplayName => displayName;
    public ItemType Type => type;
    public int StackLimit => stackLimit;
}
