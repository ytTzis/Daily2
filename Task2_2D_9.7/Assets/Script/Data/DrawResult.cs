public readonly struct DrawResult
{
    public ItemDefinition Item { get; }
    public bool IsFirstUnlock { get; }

    public DrawResult(ItemDefinition item, bool isFirstUnlock)
    {
        Item = item;
        IsFirstUnlock = isFirstUnlock;
    }
}