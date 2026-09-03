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
            ? item.rarity.ToString()
            : "未解锁";
    }
}