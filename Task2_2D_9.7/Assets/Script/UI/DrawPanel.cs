using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawPanel : MonoBehaviour
{
    [SerializeField] private BlindBoxService service;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button drawFiveButton;
    [SerializeField] private TMP_Text progressText;

    private void OnEnable()
    {
        drawButton.onClick.AddListener(OnDrawClicked);
        drawFiveButton.onClick.AddListener(OnDrawFiveClicked);
        service.CollectionChanged += Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        drawButton.onClick.RemoveListener(OnDrawClicked);
        drawFiveButton.onClick.RemoveListener(OnDrawFiveClicked);
        service.CollectionChanged -= Refresh;
    }

    private void OnDrawClicked()
    {
        if (service.CanDraw)
            service.Draw();
    }

    private void OnDrawFiveClicked()
    {
        service.DrawMultiple(5);
    }

    private void Refresh()
    {
        if (progressText == null || service == null || service.SaveData == null)
        {
            Debug.LogError("DrawPanel 引用或存档数据尚未初始化", this);
            return;
        }

        progressText.text =
            $"已解锁：{service.SaveData.unlockedItemIds?.Count ?? 0}" +
            $"/{service.Items.Count}  " +
            $"抽取次数：{service.SaveData.totalDrawCount}";

        drawButton.interactable = service.CanDraw;
        drawFiveButton.interactable = service.CanDraw;
    }
}
