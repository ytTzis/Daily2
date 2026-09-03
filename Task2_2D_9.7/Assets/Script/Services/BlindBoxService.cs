using System;
using System.Collections.Generic;
using UnityEngine;

public class BlindBoxService : MonoBehaviour
{
    [SerializeField] private List<ItemDefinition> items = new();

    public event Action<DrawResult> DrawCompleted;
    public event Action CollectionChanged;

    private SaveService saveService;
    private PlayerSaveData saveData;

    public IReadOnlyList<ItemDefinition> Items => items;
    public PlayerSaveData SaveData => saveData;

    private void Awake()
    {
        saveService = new SaveService();
        saveData = saveService.Load();

        Debug.Log($"存档位置：{saveService.SavePath}");
    }

    public DrawResult Draw()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("没有配置盲盒物品");

        ItemDefinition item = DrawByWeight();

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

    private ItemDefinition DrawByWeight()
    {
        int totalWeight = 0;

        foreach (ItemDefinition item in items)
            totalWeight += Mathf.Max(1, item.weight);

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        foreach (ItemDefinition item in items)
        {
            randomValue -= Mathf.Max(1, item.weight);

            if (randomValue < 0)
                return item;
        }

        return items[^1];
    }
    public void ResetSaveFromButton()
{
    ResetSave();
}
}