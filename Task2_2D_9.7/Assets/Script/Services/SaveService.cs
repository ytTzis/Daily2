using System;
using System.IO;
using UnityEngine;

public class SaveService
{
    private readonly string savePath;

    public string SavePath => savePath;

    public SaveService(string customPath = null)
    {
        savePath = string.IsNullOrEmpty(customPath)
            ? Path.Combine(
                Application.persistentDataPath,
                "blind_box_save.json")
            : customPath;
    }

    public void Save(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public PlayerSaveData Load()
    {
        if (!File.Exists(savePath))
            return new PlayerSaveData();

        try
        {
            string json = File.ReadAllText(savePath);
            PlayerSaveData data =
                JsonUtility.FromJson<PlayerSaveData>(json);

            return data ?? new PlayerSaveData();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"读取存档失败，将使用空存档：{exception.Message}");

            return new PlayerSaveData();
        }
    }

    public void Reset()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }
}