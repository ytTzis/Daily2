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

    public bool CanDraw
    {
        get
        {
            if (saveData?.unlockedItemIds == null)
                return false;

            foreach (ItemDefinition item in items)
            {
                if (item != null &&
                    !string.IsNullOrWhiteSpace(item.itemId) &&
                    !saveData.unlockedItemIds.Contains(item.itemId))
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
        List<ItemDefinition> availableItems = GetAvailableItems();

        if (availableItems.Count == 0)
            throw new InvalidOperationException("所有物品均已解锁");

        ItemDefinition item = DrawByWeight(availableItems);

        bool firstUnlock =
            !saveData.unlockedItemIds.Contains(item.itemId);

        if (item.rarity == ItemRarity.Gold)
        {
            UnlockAllItems();
        }
        else if (firstUnlock)
        {
            saveData.unlockedItemIds.Add(item.itemId);
        }

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

    private List<ItemDefinition> GetAvailableItems()
    {
        List<ItemDefinition> availableItems = new();

        foreach (ItemDefinition item in items)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.itemId))
                continue;

            if (!saveData.unlockedItemIds.Contains(item.itemId))
                availableItems.Add(item);
        }

        return availableItems;
    }

    private void UnlockAllItems()
    {
        foreach (ItemDefinition item in items)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.itemId))
                continue;

            if (!saveData.unlockedItemIds.Contains(item.itemId))
                saveData.unlockedItemIds.Add(item.itemId);
        }
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
