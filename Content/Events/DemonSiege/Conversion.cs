namespace AequusRemake.Content.Events.DemonSiege;

public struct Conversion {
    public readonly int OriginalItem;
    public readonly int NewItem;
    public EventTier Progression;
    public bool Hide;
    public bool DisableDecraft;

    public Conversion(int oldItem, int newItem, EventTier progression) {
        OriginalItem = oldItem;
        NewItem = newItem;
        Progression = progression;
        Hide = false;
        DisableDecraft = false;
    }

    public Item Convert(Item original) {
        original.Transform(NewItem);
        return original;
    }
}