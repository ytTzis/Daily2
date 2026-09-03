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
        rarityText.text = result.Item.rarity.ToString();

        newTagText.text = result.IsFirstUnlock
            ? "首次获得！"
            : "重复获得";
    }
}