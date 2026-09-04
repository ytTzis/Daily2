using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class BlindBoxServiceTests
{
    private string testSavePath;
    private GameObject serviceObject;
    private BlindBoxService service;
    private ItemDefinition testItem;

    [SetUp]
    public void SetUp()
    {
        testSavePath = Path.Combine(
            Application.temporaryCachePath,
            $"blind_box_test_{Guid.NewGuid():N}.json");

        testItem = ScriptableObject.CreateInstance<ItemDefinition>();
        testItem.itemId = "test_item";
        testItem.displayName = "测试物品";
        testItem.rarity = ItemRarity.Blue;
        testItem.weight = 1;

        serviceObject = new GameObject("BlindBoxService_Test");
        service = serviceObject.AddComponent<BlindBoxService>();

        service.InitializeForTests(
            new List<ItemDefinition> { testItem },
            new SaveService(testSavePath));
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(testSavePath))
            File.Delete(testSavePath);

        UnityEngine.Object.DestroyImmediate(testItem);
        UnityEngine.Object.DestroyImmediate(serviceObject);
    }

    [Test]
    public void DrawMultiple_FiveTimes_ReturnsFiveResults()
    {
        IReadOnlyList<DrawResult> results = service.DrawMultiple(5);

        Assert.AreEqual(5, results.Count);
        Assert.AreEqual(5, service.SaveData.totalDrawCount);
    }

    [Test]
    public void DrawSameItemTwice_SecondResultIsDuplicate()
    {
        DrawResult first = service.Draw();
        DrawResult second = service.Draw();

        Assert.IsTrue(first.IsFirstUnlock);
        Assert.IsFalse(second.IsFirstUnlock);
        Assert.AreEqual(1, service.SaveData.unlockedItemIds.Count);
    }

    [Test]
    public void DrawResults_AreSavedAndCanBeLoaded()
    {
        service.DrawMultiple(5);

        PlayerSaveData loaded =
            new SaveService(testSavePath).Load();

        Assert.AreEqual(5, loaded.totalDrawCount);
        Assert.AreEqual(1, loaded.unlockedItemIds.Count);
        Assert.AreEqual("test_item", loaded.unlockedItemIds[0]);
    }

    [Test]
    public void ResetSave_ClearsAllDataAndDeletesFile()
    {
        service.DrawMultiple(5);
        service.ResetSave();

        Assert.AreEqual(0, service.SaveData.totalDrawCount);
        Assert.IsEmpty(service.SaveData.unlockedItemIds);
        Assert.IsFalse(File.Exists(testSavePath));
    }
    
}