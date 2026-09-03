using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public int version = 1;
    public int totalDrawCount;
    public List<string> unlockedItemIds = new();
}