using System;
using System.Collections.Generic;
using UnityEngine;

public class BlindBoxService : MonoBehaviour
{
    [SerializeField] private List<ItemDefinition> items = new();

    public event Action<DrawResult> DrawCompleted;
    public event Action<IReadOnlyList<DrawResult>> MultiDrawCompleted;
    public event Action CollectionChanged;

    private SaveService saveService;
    private PlayerSaveData saveData;

    public IReadOnlyList<ItemDefinition> Items => items;
    public PlayerSaveData SaveData => saveData;

    public bool CanDraw
    {
        get
        {
            foreach (ItemDefinition item in items)
            {
                if (item != null &&
                    !string.IsNullOrWhiteSpace(item.itemId))
                {
                    return true;
                }
            }

            return false;
        }
    }

    private void Awake()
    {
        saveService = new SaveService();
        saveData = saveService.Load();
        saveData.unlockedItemIds ??= new List<string>();

        Debug.Log($"存档位置：{saveService.SavePath}");
    }

    public DrawResult Draw()
    {
        List<ItemDefinition> drawableItems = GetDrawableItems();

        if (drawableItems.Count == 0)
            throw new InvalidOperationException("没有配置可抽取的物品");

        ItemDefinition item = DrawByWeight(drawableItems);

        bool firstUnlock =
            !saveData.unlockedItemIds.Contains(item.itemId);

        if (firstUnlock)
            saveData.unlockedItemIds.Add(item.itemId);

        saveData.totalDrawCount++;
        saveService.Save(saveData);

        DrawResult result = new(item, firstUnlock);

        DrawCompleted?.Invoke(result);
        CollectionChanged?.Invoke();

        return result;
    }

    public IReadOnlyList<DrawResult> DrawMultiple(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        List<DrawResult> results = new(count);

        for (int i = 0; i < count; i++)
        {
            if (CanDraw)
                results.Add(Draw());
        }

        MultiDrawCompleted?.Invoke(results);
        return results;
    }

    public bool IsUnlocked(string itemId)
    {
        return saveData.unlockedItemIds.Contains(itemId);
    }

    public void ResetSave()
    {
        saveService.Reset();
        saveData = new PlayerSaveData();
        CollectionChanged?.Invoke();
    }

    private List<ItemDefinition> GetDrawableItems()
    {
        List<ItemDefinition> drawableItems = new();

        foreach (ItemDefinition item in items)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.itemId))
                continue;

            drawableItems.Add(item);
        }

        return drawableItems;
    }

    private ItemDefinition DrawByWeight(IReadOnlyList<ItemDefinition> candidates)
    {
        int totalWeight = 0;

        foreach (ItemDefinition item in candidates)
            totalWeight += Mathf.Max(1, item.weight);

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        foreach (ItemDefinition item in candidates)
        {
            randomValue -= Mathf.Max(1, item.weight);

            if (randomValue < 0)
                return item;
        }

        return candidates[^1];
    }
    public void ResetSaveFromButton()
{
    ResetSave();
}
}
