using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryItemView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject lockMask;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text rarityText;

    public void Bind(ItemDefinition item, bool unlocked)
    {
        icon.sprite = item.icon;
        icon.color = unlocked ? Color.white : Color.black;

        lockMask.SetActive(!unlocked);
        nameText.text = unlocked ? item.displayName : "？？？";
        rarityText.text = unlocked
            ? GetRarityName(item.rarity)
            : "未解锁";
    }

    private static string GetRarityName(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Blue => "蓝色品质",
            ItemRarity.Purple => "紫色品质",
            ItemRarity.Gold => "金色品质",
            _ => "未知品质"
        };
    }
}
