using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private BlindBoxService service;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text newTagText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(
            () => visualRoot.SetActive(false));
    }

    private void OnEnable()
    {
        service.DrawCompleted += ShowResult;
    }

    private void OnDisable()
    {
        service.DrawCompleted -= ShowResult;
    }

    private void ShowResult(DrawResult result)
    {
        gameObject.SetActive(true);

        itemImage.sprite = result.Item.icon;
        nameText.text = result.Item.displayName;
        rarityText.text =
    $"稀有度：{GetRarityName(result.Item.rarity)}";
    rarityText.color =
    GetRarityColor(result.Item.rarity);
        newTagText.text = result.IsFirstUnlock
            ? "首次获得！"
            : "重复获得";
    }
    private string GetRarityName(ItemRarity rarity)
{
    return rarity switch
    {
        ItemRarity.Blue => "蓝色品质",
        ItemRarity.Purple => "紫色品质",
        ItemRarity.Gold => "金色品质",
        _ => "未知品质"
    };
}
private Color GetRarityColor(ItemRarity rarity)
{
    return rarity switch
    {
        ItemRarity.Blue =>
            new Color32(59, 130, 246, 255),

        ItemRarity.Purple =>
            new Color32(168, 85, 247, 255),

        ItemRarity.Gold =>
            new Color32(245, 158, 11, 255),

        _ => Color.white
    };
}
}