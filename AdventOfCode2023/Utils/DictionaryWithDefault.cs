namespace AdventOfCode2023.Utils;

public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
{
    private TValue DefaultValue { get; }

    public DictionaryWithDefault(TValue defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public new TValue this[TKey key]
    {
        get => TryGetValue(key, out var t) ? t : DefaultValue;
        set => base[key] = value;
    }
}