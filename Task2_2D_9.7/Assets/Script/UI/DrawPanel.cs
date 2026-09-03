using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawPanel : MonoBehaviour
{
    [SerializeField] private BlindBoxService service;
    [SerializeField] private Button drawButton;
    [SerializeField] private TMP_Text progressText;

    private void OnEnable()
    {
        drawButton.onClick.AddListener(OnDrawClicked);
        service.CollectionChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        drawButton.onClick.RemoveListener(OnDrawClicked);
        service.CollectionChanged -= Refresh;
    }

    private void OnDrawClicked()
    {
        service.Draw();
    }

    private void Refresh()
    {
        progressText.text =
            $"已解锁：{service.SaveData.unlockedItemIds.Count}" +
            $"/{service.Items.Count}  " +
            $"抽取次数：{service.SaveData.totalDrawCount}";
    }
}