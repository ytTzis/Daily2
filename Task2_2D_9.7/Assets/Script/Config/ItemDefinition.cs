using UnityEngine;

public enum ItemRarity
{
    Blue,
    Purple,
    Gold
}

[CreateAssetMenu(
    fileName = "ItemDefinition",
    menuName = "BlindBox/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string itemId;
    public string displayName;
    public Sprite icon;
    public ItemRarity rarity;
    [Min(1)] public int weight = 1;
}