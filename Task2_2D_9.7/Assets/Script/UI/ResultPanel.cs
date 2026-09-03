using System.Collections.Generic;
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
    [SerializeField] private Button skipButton;
    [SerializeField] private GameObject resultCardPrefab;

    private readonly Queue<DrawResult> pendingResults = new();
    private readonly List<DrawResult> currentBatch = new();
    private readonly List<GameObject> resultCards = new();

    private bool isShowingResult;
    private bool isShowingBatchGrid;
    private RectTransform originalWindow;

    private void Awake()
    {
        originalWindow = itemImage.transform.parent as RectTransform;

        closeButton.onClick.AddListener(OnCloseClicked);
        skipButton.onClick.AddListener(OnSkipClicked);
        skipButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        service.DrawCompleted += ShowResult;
        service.MultiDrawCompleted += RegisterMultiDraw;
    }

    private void OnDisable()
    {
        service.DrawCompleted -= ShowResult;
        service.MultiDrawCompleted -= RegisterMultiDraw;
    }

    private void ShowResult(DrawResult result)
    {
        pendingResults.Enqueue(result);

        if (!isShowingResult)
            ShowNextResult();
    }

    private void RegisterMultiDraw(IReadOnlyList<DrawResult> results)
    {
        currentBatch.Clear();
        currentBatch.AddRange(results);

        skipButton.gameObject.SetActive(
            currentBatch.Count > 1 && pendingResults.Count > 0);
    }

    private void OnCloseClicked()
    {
        if (isShowingBatchGrid)
        {
            CloseAllResults();
            return;
        }

        ShowNextResult();
    }

    private void ShowNextResult()
    {
        if (pendingResults.Count == 0)
        {
            visualRoot.SetActive(false);
            skipButton.gameObject.SetActive(false);
            currentBatch.Clear();
            isShowingResult = false;
            return;
        }

        DrawResult result = pendingResults.Dequeue();
        isShowingResult = true;
        visualRoot.SetActive(true);

        PopulateResult(itemImage, nameText, rarityText, newTagText, result);
        skipButton.gameObject.SetActive(
            currentBatch.Count > 1 && pendingResults.Count > 0);
    }

    private void OnSkipClicked()
    {
        if (currentBatch.Count <= 1)
            return;

        pendingResults.Clear();
        ShowBatchGrid();
    }

    private void ShowBatchGrid()
    {
        ClearResultCards();

        isShowingBatchGrid = true;
        skipButton.gameObject.SetActive(false);
        originalWindow.gameObject.SetActive(false);

        const float spacing = 250f;
        float startX = -spacing * (currentBatch.Count - 1) * 0.5f;

        for (int i = 0; i < currentBatch.Count; i++)
        {
            CreateResultCard(
                currentBatch[i],
                new Vector2(startX + spacing * i, 20f));
        }
    }

    private void CreateResultCard(DrawResult result, Vector2 position)
    {
        if (resultCardPrefab == null)
        {
            Debug.LogError("ResultPanel 没有绑定 ResultCard 预制体", this);
            return;
        }

        GameObject card = Instantiate(resultCardPrefab, visualRoot.transform);
        card.name = $"ResultCard_{resultCards.Count + 1}";
        resultCards.Add(card);

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchoredPosition = position;

        Image cardItemImage =
            card.transform.Find("ResultImage")?.GetComponent<Image>();
        TMP_Text cardNameText =
            card.transform.Find("NameText")?.GetComponent<TMP_Text>();
        TMP_Text cardRarityText =
            card.transform.Find("RarityText")?.GetComponent<TMP_Text>();
        TMP_Text cardNewTagText =
            card.transform.Find("NewTagText")?.GetComponent<TMP_Text>();

        if (cardItemImage == null ||
            cardNameText == null ||
            cardRarityText == null ||
            cardNewTagText == null)
        {
            Debug.LogError(
                "ResultCard 预制体缺少必要的子对象",
                resultCardPrefab);
            return;
        }

        PopulateResult(
            cardItemImage,
            cardNameText,
            cardRarityText,
            cardNewTagText,
            result);
    }

    private void PopulateResult(
        Image targetItemImage,
        TMP_Text targetNameText,
        TMP_Text targetRarityText,
        TMP_Text targetNewTagText,
        DrawResult result)
    {

        Color rarityColor = GetRarityColor(result.Item.rarity);

        targetItemImage.sprite = result.Item.icon;

        targetNameText.text = result.Item.displayName;
        targetNameText.color = rarityColor;

        targetRarityText.text =
            $"稀有度：{GetRarityName(result.Item.rarity)}";
        targetRarityText.color = rarityColor;

        targetNewTagText.text = result.IsFirstUnlock
            ? "首次获得！"
            : "重复获得";
    }

    private void CloseAllResults()
    {
        ClearResultCards();
        pendingResults.Clear();
        currentBatch.Clear();

        originalWindow.gameObject.SetActive(true);

        isShowingBatchGrid = false;
        isShowingResult = false;
        skipButton.gameObject.SetActive(false);
        visualRoot.SetActive(false);
    }

    private void ClearResultCards()
    {
        foreach (GameObject card in resultCards)
        {
            if (card != null)
                Destroy(card);
        }

        resultCards.Clear();
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
            ItemRarity.Blue => new Color32(59, 130, 246, 255),
            ItemRarity.Purple => new Color32(168, 85, 247, 255),
            ItemRarity.Gold => new Color32(245, 158, 11, 255),
            _ => Color.white
        };
    }
}
